// _Scripts/Weapons/Projectiles/BaseProjectile.cs
using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Serializing;
using UnityEngine;
using _Scripts.Data;
using _Scripts.Player;
using _Scripts.Game.CTF;
using _Scripts.Combat;

[RequireComponent(typeof(Rigidbody))]
public abstract class BaseProjectile : NetworkBehaviour
{
    [Header("Definition")]
    [SerializeField] protected WeaponDefinition def; // Filled on projectile prefab.

    [Header("Presentation")]
    [SerializeField] protected bool rotateToVelocity = true;

    protected Vector3 gravAcc;
    protected bool _smoothVisual = true;

    /* ─── deterministic spawn state ───────────────────────────── */
    protected Vector3 _initPos;
    protected Vector3 _initVel;
    protected uint _spawnTick;

    /* ─── runtime ------------------------------------------------ */
    Rigidbody _rb;
    Collider _projectileCollider;

    protected bool _despawning;
    bool _spawnStateApplied;

    protected Transform _shooterRoot;
    NetworkObject _shooterObj;
    protected Vector3 _velocity;
    int _shooterClientId = -1;
    bool _suppressSpawnFireAudio;

    /* ─── interpolation ------------------------------------------ */
    protected Vector3 _prev, _next;
    protected float _timer, _tickDt;
    
    /* ─── compression ------------------------------------------ */
    
    const float ProjectileSpeedQuantScale = 10f; // 0.1 m/s precision, max ~6553 m/s
    const byte NoShooterClientId = byte.MaxValue;

    /* ─── helpers ------------------------------------------------ */
    protected readonly Collider[] _buf = new Collider[32];

    TrailRenderer[] _trailRenderers;
    ProjectileBeamLink[] _beamLinks;
    readonly List<Collider> _ignoredShooterColliders = new();

    #region Initialization

    public void Init(Vector3 pos, Vector3 vel, uint tick, NetworkObject shooter)
    {
        ClearIgnoredShooterCollisions();
        
        int shooterClientId = shooter != null && shooter.Owner.IsValid
            ? shooter.Owner.ClientId
            : -1;

        ApplySpawnState(pos, vel, tick, shooter, shooterClientId, false);

        IgnoreShooterCollisions(shooter);
    }

    void ApplySpawnState(Vector3 pos, Vector3 vel, uint tick, NetworkObject shooter,
        int shooterClientId,
        bool playSpawnPresentation)
    {
        _shooterObj = shooter;
        _shooterRoot = shooter != null ? shooter.transform : null;

        _initPos = pos;
        _initVel = vel;
        _spawnTick = tick;

        _velocity = vel;
        _despawning = false;
        _spawnStateApplied = true;
        
        _shooterClientId = shooterClientId;
        _suppressSpawnFireAudio = IsLocalShooter(_shooterClientId) &&
                                  def != null &&
                                  def.playLocalPredictedFireSfx;

        gravAcc = def != null && def.gravityScale != 0f
            ? Physics.gravity * def.gravityScale
            : Vector3.zero;

        StopAndClearTrails();
        StopBeamLinks();

        transform.position = pos;
        ApplyVelocityRotation(vel);
        ResetInterpolation(pos);

        RestartTrails();
        StartBeamLinks(pos);
        
        if (playSpawnPresentation)
            PlaySpawnFireAudio(pos);
    }

    /*
     * Legacy fallback only.
     * Normal projectile initialization should now use FishNet spawn payloads.
     */
    [ObserversRpc(BufferLast = false, ExcludeOwner = true)]
    public void RpcInit(Vector3 pos, Vector3 vel, uint tick)
    {
        if (IsServer)
            return;

        ApplySpawnState(pos, vel, tick, null, -1, true);
    }

    #endregion

    #region Spawn Payload
    
    public override void WritePayload(NetworkConnection connection, Writer writer)
    {
        base.WritePayload(connection, writer);

        writer.WriteVector3(_initPos);

        EncodeProjectileVelocity(_initVel, out ushort yawQ, out ushort pitchQ, out ushort speedQ);
        writer.WriteUInt16(yawQ);
        writer.WriteUInt16(pitchQ);
        writer.WriteUInt16(speedQ);

        writer.WriteUInt32(_spawnTick);
        writer.WriteUInt8Unpacked(EncodeShooterClientId(_shooterClientId));
    }

    public override void ReadPayload(NetworkConnection connection, Reader reader)
    {
        base.ReadPayload(connection, reader);

        Vector3 pos = reader.ReadVector3();

        ushort yawQ = reader.ReadUInt16();
        ushort pitchQ = reader.ReadUInt16();
        ushort speedQ = reader.ReadUInt16();
        Vector3 vel = DecodeProjectileVelocity(yawQ, pitchQ, speedQ);

        uint tick = reader.ReadUInt32();
        int shooterClientId = DecodeShooterClientId(reader.ReadUInt8Unpacked());
        
        ApplySpawnState(pos, vel, tick, null, shooterClientId, true);
    }
    
    static void EncodeProjectileVelocity(Vector3 velocity, out ushort yawQ, out ushort pitchQ, out ushort speedQ)
    {
        float speed = velocity.magnitude;

        if (speed <= 0.0001f)
        {
            yawQ = 0;
            pitchQ = QuantizeProjectilePitch(0f);
            speedQ = 0;
            return;
        }

        Vector3 dir = velocity / speed;

        float yawDeg = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        yawDeg = (yawDeg + 360f) % 360f;

        float pitchDeg = Mathf.Asin(Mathf.Clamp(dir.y, -1f, 1f)) * Mathf.Rad2Deg;

        yawQ = (ushort)Mathf.Clamp(Mathf.RoundToInt(yawDeg * (65535f / 360f)), 0, 65535);

        pitchQ = QuantizeProjectilePitch(pitchDeg);

        speedQ = (ushort)Mathf.Clamp(Mathf.RoundToInt(speed * ProjectileSpeedQuantScale), 0, 65535);
    }

    static Vector3 DecodeProjectileVelocity(ushort yawQ, ushort pitchQ, ushort speedQ)
    {
        float speed = speedQ / ProjectileSpeedQuantScale;

        if (speed <= 0.0001f)
            return Vector3.zero;

        float yawRad = yawQ * (360f / 65535f) * Mathf.Deg2Rad;
        float pitchRad = DequantizeProjectilePitch(pitchQ) * Mathf.Deg2Rad;

        float cosPitch = Mathf.Cos(pitchRad);

        Vector3 dir = new Vector3(Mathf.Sin(yawRad) * cosPitch, Mathf.Sin(pitchRad), Mathf.Cos(yawRad) * cosPitch);

        return dir * speed;
    }

    static ushort QuantizeProjectilePitch(float pitchDeg)
    {
        float n = Mathf.InverseLerp(-90f, 90f, Mathf.Clamp(pitchDeg, -90f, 90f));

        return (ushort)Mathf.Clamp(Mathf.RoundToInt(n * 65535f), 0, 65535);
    }

    static float DequantizeProjectilePitch(ushort pitchQ)
    {
        float n = pitchQ / 65535f;
        return Mathf.Lerp(-90f, 90f, n);
    }

    static byte EncodeShooterClientId(int clientId)
    {
        // 0–254 = valid shooter id
        // 255   = no shooter / invalid / too large
        if (clientId < 0 || clientId >= NoShooterClientId)
            return NoShooterClientId;

        return (byte)clientId;
    }

    static int DecodeShooterClientId(byte encoded)
    {
        return encoded == NoShooterClientId
            ? -1
            : encoded;
    }
    #endregion

    #region Unity / FishNet Lifecycle

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;
        _rb.useGravity = false;

        _projectileCollider = GetComponent<Collider>();
        CacheTrailRenderers();
        _beamLinks = GetComponentsInChildren<ProjectileBeamLink>(true);
    }

    public override void OnStartServer()
    {
        TimeManager.OnTick += ServerTick;
    }

    public override void OnStopServer()
    {
        TimeManager.OnTick -= ServerTick;
    }

    public override void OnStartClient()
    {
        if (IsServer)
            return;

        TimeManager.OnTick += ClientTick;
        _tickDt = (float)TimeManager.TickDelta;

        /*
         * Important:
         * Do NOT StopAndClearTrails() here.
         * ReadPayload runs before OnStartClient and already applied spawn state / restarted trails.
         */
        if (!_spawnStateApplied)
            ResetInterpolation(transform.position);
    }

    public override void OnStopClient()
    {
        if (!IsServer)
            TimeManager.OnTick -= ClientTick;

        _spawnStateApplied = false;
        StopAndClearTrails();
        StopBeamLinks();
    }

    #endregion

    #region Server Simulation

    protected virtual void ServerTick()
    {
        if (_despawning || def == null)
            return;

        double dtD = TimeManager.TickDelta;
        float dt = (float)dtD;

        if ((TimeManager.Tick - _spawnTick) * dtD >= def.lifeTime)
        {
            DespawnSelf();
            return;
        }

        if (gravAcc != Vector3.zero)
            _velocity += gravAcc * dt;

        ApplyVelocityRotation(_velocity);

        Vector3 from = transform.position;
        Vector3 to = from + _velocity * dt;

        // 1) Overlap at origin.
        int n = Physics.OverlapSphereNonAlloc(
            from,
            def.castRadius,
            _buf,
            def.hitMask,
            QueryTriggerInteraction.Ignore);

        for (int i = 0; i < n; ++i)
        {
            Collider c = _buf[i];

            if (c == null)
                continue;

            if (_shooterRoot != null && c.transform.root == _shooterRoot)
                continue;

            ClearBuffer(n);
            Explode(from, Vector3.up, c);
            return;
        }

        ClearBuffer(n);

        // 2) Sweep.
        if (Sweep(from, to, out RaycastHit hit))
        {
            if (_shooterRoot == null || hit.collider.transform.root != _shooterRoot)
            {
                transform.position = hit.point;
                Explode(hit.point, hit.normal, hit.collider);
                return;
            }
        }

        // 3) Move.
        transform.position = to;
    }

    #endregion

    #region Client Interpolation

    protected virtual void ClientTick()
    {
        if (IsServer || _despawning || !_spawnStateApplied)
            return;

        if (TimeManager.Tick < _spawnTick)
            return;

        _prev = transform.position;

        uint elapsedTicks = TimeManager.Tick - _spawnTick;
        float dt = (float)TimeManager.TickDelta;
        float t = elapsedTicks * dt;

        _next = _initPos + _initVel * t + gravAcc * (0.5f * t * t);

        Vector3 currentVel = _initVel + gravAcc * t;
        ApplyVelocityRotation(currentVel);

        _tickDt = dt;
        _timer = 0f;
    }

    protected virtual void LateUpdate()
    {
        if (!_smoothVisual)
            return;

        if (IsServer || _despawning || !_spawnStateApplied || _tickDt <= 0f)
            return;

        _timer += Time.deltaTime;
        float t = Mathf.Clamp01(_timer / _tickDt);

        transform.position = Vector3.Lerp(_prev, _next, t);
    }

    #endregion

    #region Sweep / Explosion

    protected virtual bool Sweep(Vector3 from, Vector3 to, out RaycastHit hit)
    {
        Vector3 dir = to - from;
        float len = dir.magnitude;

        if (len <= 0.0001f)
        {
            hit = default;
            return false;
        }

        dir /= len;

        switch (def.castMode)
        {
            case CastMode.Sphere:
                return Physics.SphereCast(
                    from,
                    def.castRadius,
                    dir,
                    out hit,
                    len,
                    def.hitMask,
                    QueryTriggerInteraction.Ignore);

            case CastMode.Capsule:
            {
                Vector3 p1 = from + Vector3.up * def.castHalf;
                Vector3 p2 = from - Vector3.up * def.castHalf;

                return Physics.CapsuleCast(
                    p1,
                    p2,
                    def.castRadius,
                    dir,
                    out hit,
                    len,
                    def.hitMask,
                    QueryTriggerInteraction.Ignore);
            }

            case CastMode.Ray:
                return Physics.Raycast(
                    from,
                    dir,
                    out hit,
                    len,
                    def.hitMask,
                    QueryTriggerInteraction.Ignore);
        }

        hit = default;
        return false;
    }

    protected virtual void Explode(Vector3 pos, Vector3 normal, Collider directHitCol = null)
    {
        Vector3 shotDir = _velocity.sqrMagnitude > 0.0001f
            ? _velocity.normalized
            : transform.forward;

        Vector3 explodePos = pos;

        if (def.castRadius > 0f)
            explodePos = pos - shotDir * def.castRadius;

        ApplyExplosion(explodePos, shotDir, directHitCol);

        RpcSpawnImpact(explodePos, normal);

        DespawnSelf();
    }

    protected virtual void ApplyExplosion(Vector3 centre, Vector3 shotDir, Collider directHitCol)
    {
        int cnt = Physics.OverlapSphereNonAlloc(
            centre,
            def.blastRadius,
            _buf,
            def.playerMask,
            QueryTriggerInteraction.Ignore);

        bool any = false;

        for (int i = 0; i < cnt; ++i)
        {
            Collider c = _buf[i];

            if (c == null)
                continue;

            if (!ClearLineOfSight(centre, c))
                continue;

            any |= DealDamageAndKnockback(c, centre, shotDir);
        }

        if (!any && directHitCol != null)
            DealDamageAndKnockback(directHitCol, centre, shotDir);

        ClearBuffer(cnt);

        ApplyObjectiveImpulse(centre, shotDir, directHitCol);
    }

    protected virtual void ApplyObjectiveImpulse(Vector3 centre, Vector3 shotDir, Collider directHitCol)
    {
        if (def.knockbackForce <= 0f || def.blastRadius <= 0f)
            return;

        int cnt = Physics.OverlapSphereNonAlloc(
            centre,
            def.blastRadius,
            _buf,
            def.objectiveMask,
            QueryTriggerInteraction.Collide);

        for (int i = 0; i < cnt; ++i)
        {
            Collider c = _buf[i];

            if (c == null)
                continue;

            FlagObject flag = c.GetComponentInParent<FlagObject>();

            if (flag == null)
                continue;

            if (!ClearLineOfSight(centre, c))
                continue;

            Vector3 impulse = CalculateExplosionImpulse(c, centre, shotDir, out _);
            impulse *= def.objectiveKnockbackMultiplier;

            flag.Server_ApplyWeaponImpulse(impulse);
        }

        ClearBuffer(cnt);

        // Direct-hit fallback for cases where the flag collider is hit but not found by overlap.
        if (directHitCol != null)
        {
            FlagObject directFlag = directHitCol.GetComponentInParent<FlagObject>();

            if (directFlag != null)
            {
                Vector3 impulse = CalculateExplosionImpulse(directHitCol, centre, shotDir, out _);
                impulse *= def.objectiveKnockbackMultiplier;

                directFlag.Server_ApplyWeaponImpulse(impulse);
            }
        }
    }

    protected bool ClearLineOfSight(Vector3 blast, Collider target)
    {
        Vector3[] samples =
        {
            target.bounds.center,
            target.bounds.center + Vector3.up * 0.8f,
            target.bounds.center - Vector3.up * 0.8f,
            target.bounds.center + target.transform.right * 0.3f,
            target.bounds.center - target.transform.right * 0.3f
        };

        foreach (Vector3 origin in samples)
        {
            Vector3 dir = blast - origin;
            float dist = dir.magnitude;

            if (dist <= 0.01f)
                return true;

            dir /= dist;

            if (Physics.Raycast(origin, dir, out RaycastHit hit, dist - 0.01f, def.hitMask, QueryTriggerInteraction.Ignore))
            {
                Transform root = hit.collider.transform.root;

                // Ignore all players as LOS blockers.
                if (root.TryGetComponent<AdvancedPredictedController>(out _))
                    return true;

                if (root == target.transform.root || (_shooterRoot != null && root == _shooterRoot))
                    return true;
            }
            else
            {
                return true;
            }
        }

        return false;
    }

    protected virtual bool DealDamageAndKnockback(Collider col, Vector3 centre, Vector3 shotDir)
    {
        Transform root = col.transform.root;

        if (!root.TryGetComponent(out AdvancedPredictedController ctrl) ||
            !root.TryGetComponent(out PlayerHealth hp))
            return false;

        if (!hp.IsAlive)
            return false;

        Vector3 impulse = CalculateExplosionImpulse(col, centre, shotDir, out float power);

        int dmg = Mathf.Max(1, Mathf.RoundToInt(def.damage * power));

        Vector3 hitPoint = col.ClosestPoint(centre);
        Vector3 normal = shotDir.sqrMagnitude > 0.0001f ? -shotDir.normalized : Vector3.up;

        DamageType damageType = def != null ? def.damageType : DamageType.Explosion;

        byte weaponId = def != null ? def.weaponId : (byte)0;

        var info = new DamageInfo(amount: dmg, attacker: _shooterObj, source: NetworkObject, type: damageType,
            point: hitPoint, normal: normal, impulse: impulse, weaponId: weaponId);

        DamageResult result = hp.ApplyDamage(info);

        if (result.ShouldShowHitMarker)
            NotifyShooterHitMarker(root);

        if (def.knockbackForce > 0f)
            ctrl.ReceiveKnockback(impulse);

        return result.Applied;
    }
    
    [Server]
    void NotifyShooterHitMarker(Transform victimRoot)
    {
        if (_shooterObj == null)
            return;

        if (victimRoot != null &&
            victimRoot.TryGetComponent(out NetworkObject victimNob) &&
            victimNob == _shooterObj)
        {
            return; // no self-hit marker
        }

        if (_shooterObj.TryGetComponent(out _Scripts.Player.PlayerCombatFeedback feedback))
            feedback.ServerNotifyHitMarker();
    }

    protected Vector3 CalculateExplosionImpulse(Collider col, Vector3 centre, Vector3 shotDir, out float power)
    {
        Vector3 to = col.ClosestPoint(centre) - centre;
        float dist = to.magnitude;

        Vector3 dir = dist < def.minDirThreshold
            ? shotDir
            : to.normalized;

        power = Mathf.Pow(
            1f - Mathf.Clamp01(dist / def.blastRadius),
            def.knockFalloffExp);

        return dir * (def.knockbackForce * power);
    }

    [ObserversRpc(BufferLast = false)]
    protected void RpcSpawnImpact(Vector3 pos, Vector3 normal)
    {
        if (def == null || !def.spawnImpactVfx)
            return;

        if (string.IsNullOrWhiteSpace(def.impactVfxKey))
            return;

        Quaternion rot = normal.sqrMagnitude > 0.0001f
            ? Quaternion.LookRotation(normal.normalized)
            : Quaternion.identity;

        VfxPool.Spawn(def.impactVfxKey, pos, rot, def.impactVfxLifetime);
    }

    #endregion

    #region Despawn / Pool Cleanup

    [Server]
    protected void DespawnSelf()
    {
        if (_despawning)
            return;

        _despawning = true;
        _spawnStateApplied = false;

        StopAndClearTrails();
        StopBeamLinks();
        ClearIgnoredShooterCollisions();

        ServerManager.Despawn(gameObject, DespawnType.Pool);
    }

    void IgnoreShooterCollisions(NetworkObject shooter)
    {
        if (_projectileCollider == null || shooter == null)
            return;

        Collider[] shooterColliders = shooter.GetComponentsInChildren<Collider>();

        foreach (Collider c in shooterColliders)
        {
            if (!c)
                continue;

            Physics.IgnoreCollision(_projectileCollider, c, true);
            _ignoredShooterColliders.Add(c);
        }
    }

    void ClearIgnoredShooterCollisions()
    {
        if (_projectileCollider == null)
        {
            _ignoredShooterColliders.Clear();
            return;
        }

        foreach (Collider c in _ignoredShooterColliders)
        {
            if (!c)
                continue;

            Physics.IgnoreCollision(_projectileCollider, c, false);
        }

        _ignoredShooterColliders.Clear();
    }

    #endregion

    #region Visual / Audio / Interpolation Helpers

    protected void ApplyVelocityRotation(Vector3 vel)
    {
        if (!rotateToVelocity)
            return;

        if (vel.sqrMagnitude <= 0.0001f)
            return;

        transform.rotation = Quaternion.LookRotation(vel.normalized, Vector3.up);
    }

    protected void ResetInterpolation(Vector3 pos)
    {
        _prev = _next = pos;
        _timer = 0f;
        _tickDt = TimeManager != null ? (float)TimeManager.TickDelta : 0f;
    }

    void CacheTrailRenderers()
    {
        _trailRenderers ??= GetComponentsInChildren<TrailRenderer>(true);
    }

    protected void StopAndClearTrails()
    {
        CacheTrailRenderers();

        if (_trailRenderers == null)
            return;

        foreach (TrailRenderer tr in _trailRenderers)
        {
            if (!tr)
                continue;

            tr.emitting = false;
            tr.Clear();
        }
    }

    protected void RestartTrails()
    {
        CacheTrailRenderers();

        if (_trailRenderers == null)
            return;

        foreach (TrailRenderer tr in _trailRenderers)
        {
            if (!tr)
                continue;

            tr.emitting = false;
            tr.Clear();
            tr.emitting = true;
        }
    }

    protected void ClearBuffer(int count)
    {
        int n = Mathf.Min(count, _buf.Length);

        for (int i = 0; i < n; ++i)
            _buf[i] = null;
    }
    
    void StartBeamLinks(Vector3 startPos)
    {
        if (_beamLinks == null)
            return;

        foreach (ProjectileBeamLink beam in _beamLinks)
        {
            if (!beam)
                continue;

            beam.Init(startPos);
        }
    }

    void StopBeamLinks()
    {
        if (_beamLinks == null)
            return;

        foreach (ProjectileBeamLink beam in _beamLinks)
        {
            if (!beam)
                continue;

            beam.ResetBeam();
        }
    }
    
    bool IsLocalShooter(int shooterClientId)
    {
        if (shooterClientId < 0)
            return false;

        if (InstanceFinder.ClientManager == null ||
            InstanceFinder.ClientManager.Connection == null)
            return false;

        return InstanceFinder.ClientManager.Connection.ClientId == shooterClientId;
    }

    void PlaySpawnFireAudio(Vector3 pos)
    {
        if (_suppressSpawnFireAudio)
            return;

        if (def == null || def.fireSfx == null)
            return;

        float pitch = Random.Range(def.firePitchMin, def.firePitchMax);

        WeaponAudioPool.PlayOneShot(
            def.fireSfx,
            pos,
            def.fireVolume,
            pitch,
            def.fireSpatialBlend,
            def.fireMinDistance,
            def.fireMaxDistance);
    }

    #endregion
}
// _Scripts/Weapons/Projectiles/BaseProjectile.cs
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Serializing;
using UnityEngine;
using _Scripts.Data;
using _Scripts.Player;
using _Scripts.Game.CTF;

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

    /* ─── interpolation ------------------------------------------ */
    protected Vector3 _prev, _next;
    protected float _timer, _tickDt;

    /* ─── helpers ------------------------------------------------ */
    protected readonly Collider[] _buf = new Collider[32];

    TrailRenderer[] _trailRenderers;
    ProjectileBeamLink[] _beamLinks;
    readonly List<Collider> _ignoredShooterColliders = new();

    #region Initialization

    public void Init(Vector3 pos, Vector3 vel, uint tick, NetworkObject shooter)
    {
        ClearIgnoredShooterCollisions();

        ApplySpawnState(pos, vel, tick, shooter);

        IgnoreShooterCollisions(shooter);
    }

    void ApplySpawnState(Vector3 pos, Vector3 vel, uint tick, NetworkObject shooter)
    {
        _shooterObj = shooter;
        _shooterRoot = shooter != null ? shooter.transform : null;

        _initPos = pos;
        _initVel = vel;
        _spawnTick = tick;

        _velocity = vel;
        _despawning = false;
        _spawnStateApplied = true;

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

        ApplySpawnState(pos, vel, tick, null);
    }

    #endregion

    #region Spawn Payload

    public override void WritePayload(NetworkConnection connection, Writer writer)
    {
        base.WritePayload(connection, writer);

        writer.WriteVector3(_initPos);
        writer.WriteVector3(_initVel);
        writer.WriteUInt32(_spawnTick);
    }

    public override void ReadPayload(NetworkConnection connection, Reader reader)
    {
        base.ReadPayload(connection, reader);

        Vector3 pos = reader.ReadVector3();
        Vector3 vel = reader.ReadVector3();
        uint tick = reader.ReadUInt32();

        ApplySpawnState(pos, vel, tick, null);
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

        Vector3 impulse = CalculateExplosionImpulse(col, centre, shotDir, out float power);

        int dmg = Mathf.Max(1, Mathf.RoundToInt(def.damage * power));
        hp.ApplyDamage(dmg, _shooterObj);

        if (def.knockbackForce > 0f)
            ctrl.ReceiveKnockback(impulse);

        return true;
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

    #region Visual / Interpolation Helpers

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

    #endregion
}
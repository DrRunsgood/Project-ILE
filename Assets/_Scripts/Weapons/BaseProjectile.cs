// _Scripts/Weapons/Projectiles/BaseProjectile.cs
using FishNet.Object;
using UnityEngine;
using _Scripts.Data;
using _Scripts.Player;
using _Scripts.Game.CTF;

[RequireComponent(typeof(Rigidbody))]
public abstract class BaseProjectile : NetworkBehaviour
{
    [SerializeField] protected WeaponDefinition def;  // filled on the prefab
    protected Vector3 gravAcc;
    protected bool _smoothVisual = true;

    /* ─── deterministic spawn state (NO SyncVars!) ──────────────────── */
    protected Vector3 _initPos;
    protected Vector3 _initVel;
    protected uint    _spawnTick;

    /* ─── runtime ------------------------------------------------------ */
    Rigidbody       _rb;
    protected bool            _despawning;
    protected Transform       _shooterRoot;
    NetworkObject   _shooterObj;
    protected Vector3         _velocity;

    /* interpolation */
    protected Vector3 _prev, _next;
    protected float   _timer, _tickDt;

    /* helper */
    protected readonly Collider[] _buf = new Collider[32];
    TrailRenderer[] _trailRenderers;

    /* ───────────────────────────── */
    #region Initialisation
    public void Init(Vector3 pos, Vector3 vel, uint tick, NetworkObject shooter)
    {
        _shooterObj  = shooter;
        _shooterRoot = shooter.transform;

        _initPos   = pos;
        _initVel   = vel;
        _spawnTick = tick;

        _velocity  = vel;
        
        StopAndClearTrails();
        transform.position = pos;
        ApplyVelocityRotation(vel);
        ResetInterpolation(pos);
        RestartTrails();
        
        _despawning = false;
        
        gravAcc = def.gravityScale == 0 ? Vector3.zero : Physics.gravity * def.gravityScale;
        
        if (TryGetComponent(out Collider projCol))
            foreach (Collider c in shooter.GetComponentsInChildren<Collider>())
                Physics.IgnoreCollision(projCol, c, true);
    }

    /* called automatically on every non-owner right after Spawn() */
    [ObserversRpc(BufferLast = false, ExcludeOwner = true)]
    public void RpcInit(Vector3 pos, Vector3 vel, uint tick)
    {
        if (IsServer) return;                     // server already has them

        _initPos   = pos;
        _initVel   = vel;
        _spawnTick = tick;
        
        _velocity = vel;
        
        StopAndClearTrails();
        transform.position = pos;                 // correct spawn pose
        ApplyVelocityRotation(vel);
        ResetInterpolation(pos);
        RestartTrails();
        
        _despawning = false;
        
        gravAcc = def.gravityScale == 0 ? Vector3.zero : Physics.gravity * def.gravityScale;
    }
    #endregion

    /* ───────────────────────────── */
    #region Unity / Fish-Net
    protected virtual void Awake()
    {
        _rb            = GetComponent<Rigidbody>();
        _rb.isKinematic = true;
        _rb.useGravity  = false;
        _trailRenderers = GetComponentsInChildren<TrailRenderer>(true);
    }

    public override void OnStartServer() => TimeManager.OnTick += ServerTick;
    public override void OnStopServer () => TimeManager.OnTick -= ServerTick;

    public override void OnStartClient()
    {
        if (!IsServer)
        {
            TimeManager.OnTick += ClientTick;
            _prev = _next = transform.position;
            _tickDt = (float)TimeManager.TickDelta;
            StopAndClearTrails();
        }
    }
    public override void OnStopClient()
    {
        if (!IsServer) TimeManager.OnTick -= ClientTick;
    }
    #endregion

    /* ───────────────────────────── */
    #region Server simulation
    protected virtual void ServerTick()
    {
        if (_despawning || def == null) return;

        double dt = TimeManager.TickDelta;
        if ((TimeManager.Tick - _spawnTick) * dt >= def.lifeTime)
        { DespawnSelf(); return; }

        if (gravAcc != Vector3.zero)
            _velocity += gravAcc * (float)dt;

        Vector3 from = transform.position;
        Vector3 to   = from + _velocity * (float)dt;

        /* 1) overlap at origin */
        int n = Physics.OverlapSphereNonAlloc(from, def.castRadius, _buf, def.hitMask, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < n; ++i)
        {
            if (_buf[i] == null || _buf[i].transform.root == _shooterRoot) continue;
            Explode(from, Vector3.up, _buf[i]);      // direct hit
            return;
        }

        /* 2) sweep */
        if (Sweep(from, to, out RaycastHit hit) && hit.collider.transform.root != _shooterRoot)
        {
            transform.position = hit.point;
            Explode(hit.point, hit.normal, hit.collider);
            return;
        }

        /* 3) move */
        transform.position = to;
    }
    #endregion

    /* ───────────────────────────── */
    #region Client interpolation
    protected virtual void ClientTick()
    {
        if (IsServer || _despawning) return;
        if (TimeManager.Tick < _spawnTick) return;   // init not yet arrived

        _prev = transform.position;

        uint  e  = TimeManager.Tick - _spawnTick;    // elapsed ticks
        float dt = (float)TimeManager.TickDelta;
        float t  = e * dt;

        _next = _initPos + _initVel * t + gravAcc * (0.5f * t * t);

        _tickDt = dt;
        _timer  = 0f;
    }

    protected virtual void LateUpdate()
    {
        if (_smoothVisual == false) return;
        
        if (IsServer || _despawning || _tickDt <= 0f) return;
        _timer += Time.deltaTime;
        transform.position = Vector3.Lerp(_prev, _next, Mathf.Clamp01(_timer / _tickDt));
    }
    #endregion

    /* ───────────────────────────── */
    #region Helpers (sweep, explode, LOS check, despawn)

    /* generic sweep based on definition */
    protected virtual bool Sweep(Vector3 from, Vector3 to, out RaycastHit hit)
    {
        Vector3 dir = (to - from);
        float len   = dir.magnitude;
        dir        /= len;
        
        if (len <= 0.0001f)
        {
            hit = default;
            return false;
        }

        switch (def.castMode)
        {
            case CastMode.Sphere:
                return Physics.SphereCast(from, def.castRadius, dir, out hit, len, def.hitMask, QueryTriggerInteraction.Ignore);

            case CastMode.Capsule:
                Vector3 p1 = from + Vector3.up * def.castHalf;
                Vector3 p2 = from - Vector3.up * def.castHalf;
                return Physics.CapsuleCast(p1, p2, def.castRadius, dir, out hit, len, def.hitMask, QueryTriggerInteraction.Ignore);

            case CastMode.Ray:
                return Physics.Raycast(from, dir, out hit, len, def.hitMask, QueryTriggerInteraction.Ignore);
        }
        hit = default;
        return false;
    }
    
    /* ───────── explosion & knock-back ───────── */
    protected virtual void Explode(Vector3 pos, Vector3 normal, Collider directHitCol = null)
    {
        Vector3 explodePos = pos;

        if (def.castRadius > 0f)
            explodePos = pos - (_velocity.normalized * def.castRadius);

        ApplyExplosion(explodePos, _velocity.normalized, directHitCol);

        if (def.knockbackForce > 0f)
            RpcSpawnImpact(explodePos, normal);

        DespawnSelf();
    }
    
    // ───── multi-sample LOS: true if *any* ray is clear 
    protected bool ClearLineOfSight(Vector3 blast, Collider target)
    {
        Vector3[] samples = {
            target.bounds.center,
            target.bounds.center + Vector3.up * 0.8f,
            target.bounds.center - Vector3.up * 0.8f,
            target.bounds.center + target.transform.right * 0.3f,
            target.bounds.center - target.transform.right * 0.3f
        };

        foreach (Vector3 origin in samples)
        {
            Vector3 dir  = blast - origin;
            float   dist = dir.magnitude;
            if (dist <= 0.01f) return true;          // overlapping
            dir /= dist;

            if (Physics.Raycast(origin, dir, out RaycastHit hit, dist - 0.01f, def.hitMask, QueryTriggerInteraction.Ignore))
            {
                Transform root = hit.collider.transform.root;
                
                // ignore ALL players as blockers
                if (root.TryGetComponent<AdvancedPredictedController>(out _))
                    return true;             
                if (root == target.transform.root || (_shooterRoot != null && root == _shooterRoot))  //redundant if we keep the first if
                    return true;
            }
            else
            {
                return true;
            }
        }
        return false;
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

        for (int i = 0; i < cnt; ++i)
            _buf[i] = null;

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

        for (int i = 0; i < cnt; ++i)
            _buf[i] = null;

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

    // helper that contains your old fall-off, knock-back etc.
    protected virtual bool DealDamageAndKnockback(Collider col, Vector3 centre, Vector3 shotDir)
    {
        Transform root = col.transform.root;
        if (!root.TryGetComponent(out AdvancedPredictedController ctrl) ||
            !root.TryGetComponent(out PlayerHealth hp))
            return false;

        Vector3 imp = CalculateExplosionImpulse(col, centre, shotDir, out float pwr);

        int dmg = Mathf.Max(1, Mathf.RoundToInt(def.damage * pwr));
        hp.ApplyDamage(dmg, _shooterObj);

        if (def.knockbackForce > 0f)
            ctrl.ReceiveKnockback(imp);

        return true;
    }
    
    protected Vector3 CalculateExplosionImpulse(Collider col, Vector3 centre, Vector3 shotDir, out float power)
    {
        Vector3 to = col.ClosestPoint(centre) - centre;
        float dist = to.magnitude;

        Vector3 dir = (dist < def.minDirThreshold)
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
    
    [Server] protected void DespawnSelf()
    {
        if (_despawning) return;
        _despawning = true;
        StopAndClearTrails();
        ServerManager.Despawn(gameObject, DespawnType.Pool);
    }
    
    protected void ApplyVelocityRotation(Vector3 vel)
    {
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
    
    protected void StopAndClearTrails()
    {
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
}
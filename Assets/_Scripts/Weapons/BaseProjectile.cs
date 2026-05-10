// _Scripts/Weapons/Projectiles/BaseProjectile.cs
using FishNet.Object;
using UnityEngine;
using _Scripts.Data;
using _Scripts.Player;

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
    public   ParticleSystem projectileTrail;

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
        transform.position = pos;
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
        
        gravAcc = def.gravityScale == 0 ? Vector3.zero : Physics.gravity * def.gravityScale;

        transform.position = pos;                 // correct spawn pose
        _prev = _next = pos;
    }
    #endregion

    /* ───────────────────────────── */
    #region Unity / Fish-Net
    protected virtual void Awake()
    {
        _rb            = GetComponent<Rigidbody>();
        _rb.isKinematic = true;
        _rb.useGravity  = false;
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
        }

        if (projectileTrail)
        {
            projectileTrail.Clear();
            projectileTrail.Play();
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
        int cnt = Physics.OverlapSphereNonAlloc(centre, def.blastRadius, _buf, def.playerMask, QueryTriggerInteraction.Ignore);

        bool any = false;
        // ---------- players inside radius ----------
        for (int i = 0; i < cnt; ++i)
        {
            Collider c = _buf[i];
            if (c == null) continue;

            // new LOS gate
            if (!ClearLineOfSight(centre, c))
                continue;
            
            any |= DealDamageAndKnockback(c, centre, shotDir);
        }

        // ---------- direct-hit fallback ----------
        if (!any && directHitCol != null)
            DealDamageAndKnockback(directHitCol, centre, shotDir);

        // hygiene
        for (int i = 0; i < cnt; ++i) _buf[i] = null;
    }

    // helper that contains your old fall-off, knock-back etc.
    protected virtual bool DealDamageAndKnockback(Collider col, Vector3 centre, Vector3 shotDir)
    {
        Transform root = col.transform.root;
        if (!root.TryGetComponent(out AdvancedPredictedController ctrl) || !root.TryGetComponent(out PlayerHealth hp))
            return false;                        // not a player

        Vector3 to   = col.ClosestPoint(centre) - centre;
        float   dist = to.magnitude;
        Vector3 dir  = (dist < def.minDirThreshold) ? shotDir : to.normalized;
        float   pwr  = Mathf.Pow(1f - Mathf.Clamp01(dist / def.blastRadius), def.knockFalloffExp);

        int dmg = Mathf.Max(1, Mathf.RoundToInt(def.damage * pwr));
        hp.ApplyDamage(dmg, _shooterObj);

        if (def.knockbackForce > 0f)
        {
            Vector3 imp = dir * (def.knockbackForce * pwr);
            ctrl.ReceiveKnockback(imp);
        }
        return true;
    }
    
    [ObserversRpc(BufferLast = false)]
    protected void RpcSpawnImpact(Vector3 pos, Vector3 normal)
    {
        VfxPool.Spawn("VFX/RocketExplosion", pos, Quaternion.LookRotation(normal), 2.5f);
    }
    #endregion
    
    [Server] protected void DespawnSelf()
    {
        if (_despawning) return;
        _despawning = true;
        ServerManager.Despawn(gameObject, DespawnType.Pool);
    }
}
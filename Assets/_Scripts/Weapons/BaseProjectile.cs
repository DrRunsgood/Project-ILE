// _Scripts/Weapons/Projectiles/BaseProjectile.cs
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using _Scripts.Data;
using _Scripts.Player;

[RequireComponent(typeof(Rigidbody))]
public class BaseProjectile : NetworkBehaviour
{
    /* ─── immutable weapon data ─────────────────────────────────────── */
    WeaponDefinition _def;
    public void SetDefinition(WeaponDefinition d)
    {
        _def      = d;
        _gravAcc  = d.gravityScale != 0 ? Physics.gravity * d.gravityScale : Vector3.zero;
    }

    /* ─── deterministic spawn state (NO SyncVars!) ──────────────────── */
    Vector3 _initPos;
    Vector3 _initVel;
    uint    _spawnTick;
    Vector3 _gravAcc;

    /* ─── runtime ------------------------------------------------------ */
    Rigidbody       _rb;
    bool            _despawning;
    Transform       _shooterRoot;
    NetworkObject   _shooterObj;
    Vector3         _velocity;

    /* interpolation */
    Vector3 _prev, _next;
    float   _timer, _tickDt;

    /* helper */
    readonly Collider[] _buf = new Collider[32];
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

        if (TryGetComponent(out Collider projCol))
            foreach (Collider c in shooter.GetComponentsInChildren<Collider>())
                Physics.IgnoreCollision(projCol, c, true);
    }

    /* called automatically on every non-owner right after Spawn() */
    [ObserversRpc(BufferLast = false, ExcludeOwner = true)]
    public void RpcInit(Vector3 pos, Vector3 vel, uint tick, float gravScale)
    {
        if (IsServer) return;                     // server already has them

        _initPos   = pos;
        _initVel   = vel;
        _spawnTick = tick;
        _gravAcc   = gravScale != 0 ? Physics.gravity * gravScale : Vector3.zero;

        transform.position = pos;                 // correct spawn pose
        _prev = _next = pos;
    }
    #endregion

    /* ───────────────────────────── */
    #region Unity / Fish-Net
    void Awake()
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
    void ServerTick()
    {
        if (_despawning || _def == null) return;

        double dt = TimeManager.TickDelta;
        if ((TimeManager.Tick - _spawnTick) * dt >= _def.lifeTime)
        { DespawnSelf(); return; }

        if (_gravAcc != Vector3.zero)
            _velocity += _gravAcc * (float)dt;

        Vector3 from = transform.position;
        Vector3 to   = from + _velocity * (float)dt;

        /* 1) overlap at origin */
        int n = Physics.OverlapSphereNonAlloc(from, _def.castRadius, _buf, _def.hitMask, QueryTriggerInteraction.Ignore);
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
    void ClientTick()
    {
        if (IsServer || _despawning) return;
        if (TimeManager.Tick < _spawnTick) return;   // init not yet arrived

        _prev = transform.position;

        uint  e  = TimeManager.Tick - _spawnTick;    // elapsed ticks
        float dt = (float)TimeManager.TickDelta;
        float t  = e * dt;

        _next = _initPos + _initVel * t + _gravAcc * (0.5f * t * t);

        _tickDt = dt;
        _timer  = 0f;
    }

    void LateUpdate()
    {
        if (IsServer || _despawning || _tickDt <= 0f) return;
        _timer += Time.deltaTime;
        transform.position = Vector3.Lerp(_prev, _next, Mathf.Clamp01(_timer / _tickDt));
    }
    #endregion

    /* ───────────────────────────── */
    #region Helpers (sweep, explode, despawn) ── unchanged from your latest version

    /* generic sweep based on definition */
    bool Sweep(Vector3 from, Vector3 to, out RaycastHit hit)
    {
        Vector3 dir = (to - from);
        float len   = dir.magnitude;
        dir        /= len;

        switch (_def.castMode)
        {
            case CastMode.Sphere:
                return Physics.SphereCast(from, _def.castRadius, dir, out hit, len, _def.hitMask, QueryTriggerInteraction.Ignore);

            case CastMode.Capsule:
                Vector3 p1 = from + Vector3.up * _def.castHalf;
                Vector3 p2 = from - Vector3.up * _def.castHalf;
                return Physics.CapsuleCast(p1, p2, _def.castRadius, dir, out hit, len, _def.hitMask, QueryTriggerInteraction.Ignore);

            case CastMode.Ray:
                return Physics.Raycast(from, dir, out hit, len, _def.hitMask, QueryTriggerInteraction.Ignore);
        }
        hit = default;
        return false;
    }
    

    /* ───────── explosion & knock-back ───────── */
    void Explode(Vector3 pos, Vector3 normal, Collider directHitCol = null)
    {
        ApplyExplosion(pos, _velocity.normalized, directHitCol);
        if (_def.knockbackForce > 0f) // don't call RPC for chaingun
            RpcSpawnImpact(pos, normal);
        DespawnSelf();
    }

    void ApplyExplosion(Vector3 centre, Vector3 shotDir, Collider directHitCol) 
    {
        int cnt = Physics.OverlapSphereNonAlloc(centre, _def.blastRadius, _buf, _def.playerMask, QueryTriggerInteraction.Ignore);

        bool any = false;

        // ---------- players inside radius ----------
        for (int i = 0; i < cnt; ++i)
            any |= DealDamageAndKnockback(_buf[i], centre, shotDir);

        // ---------- direct-hit fallback ----------
        if (!any && directHitCol != null)
            DealDamageAndKnockback(directHitCol, centre, shotDir);

        // hygiene
        for (int i = 0; i < cnt; ++i) _buf[i] = null;
    }

    // helper that contains your old fall-off, knock-back etc.
    bool DealDamageAndKnockback(Collider col, Vector3 centre, Vector3 shotDir)
    {
        Transform root = col.transform.root;
        if (!root.TryGetComponent(out AdvancedPredictedController ctrl) || !root.TryGetComponent(out PlayerHealth hp))
            return false;                        // not a player

        Vector3 to   = col.ClosestPoint(centre) - centre;
        float   dist = to.magnitude;
        Vector3 dir  = (dist < _def.minDirThreshold) ? shotDir : to.normalized;
        float   pwr  = Mathf.Pow(1f - Mathf.Clamp01(dist / _def.blastRadius), _def.knockFalloffExp);

        int dmg = Mathf.Max(1, Mathf.RoundToInt(_def.damage * pwr));
        hp.ApplyDamage(dmg, _shooterObj);

        if (_def.knockbackForce > 0f)
        {
            Vector3 imp = dir * (_def.knockbackForce * pwr);
            ctrl.ReceiveKnockback(imp);
            RpcApplyKnockback(ctrl.NetworkObject, imp);
        }
        return true;
    }

    [ObserversRpc(BufferLast = false, ExcludeOwner = false)]
    void RpcApplyKnockback(NetworkObject target, Vector3 impulse)
    {
        if (target.TryGetComponent(out AdvancedPredictedController ctrl))
            ctrl.ReceiveKnockback(impulse);
    }

    [ObserversRpc(BufferLast = false)]
    void RpcSpawnImpact(Vector3 pos, Vector3 normal)
    {
        if (projectileTrail != null)
        {
            projectileTrail.Stop();
            VfxPool.Spawn("VFX/RocketExplosion", pos, Quaternion.LookRotation(normal), 2.5f);
        }
        // for now use projectile trail to determine explosion until we build out our individual projectile scripts
        //if (!string.IsNullOrEmpty("VFX/RocketExplosion"))
          //  VfxPool.Spawn("VFX/RocketExplosion", pos, Quaternion.LookRotation(normal), 2.5f);
    }
    #endregion

/* ───────── despawn ───────── */
    [Server] void DespawnSelf()
    {
        if (_despawning) return;
        _despawning = true;
        ServerManager.Despawn(gameObject, DespawnType.Pool);
    }
}

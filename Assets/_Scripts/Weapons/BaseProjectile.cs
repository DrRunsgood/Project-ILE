// _Scripts/Weapons/Projectiles/BaseProjectile.cs
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using _Scripts.Data;
using _Scripts.Player;

[RequireComponent(typeof(Rigidbody))]
public sealed class BaseProjectile : NetworkBehaviour
{
/* ───────── data from the shooter ───────── */
    WeaponDefinition _def;                              // set once
    public void SetDefinition(WeaponDefinition d)
    {
        _def = d;
        _gravScale.Value = d.gravityScale;
        _gAcc = (d.gravityScale != 0f) ? Physics.gravity * d.gravityScale : Vector3.zero;
    }
    
/* ───────── synced spawn state ───────── */
    readonly SyncVar<Vector3> _initPos   = new();
    readonly SyncVar<Vector3> _initVel   = new();
    readonly SyncVar<uint>    _spawnTick = new();
    readonly SyncVar<float> _gravScale = new();

/* ───────── runtime ───────── */
    Rigidbody   _rb;
    bool        _despawning;
    Transform   _shooterRoot;
    Vector3     _velocity;
    private Vector3 _gAcc;
    public ParticleSystem projectileTrail;
    public GameObject projectileExplosion;

/* interpolation buffer */
    Vector3 _prev, _next;
    float   _timer, _tickDt;

/* non-alloc helpers */
    readonly Collider[] _buf = new Collider[32];

/* ───────── initialisation ───────── */
    public void Init(Vector3 pos, Vector3 vel, uint tick, NetworkObject shooter)
    {
        _initPos.Value   = pos;
        _initVel.Value   = vel;
        _spawnTick.Value = tick;
        _shooterRoot     = shooter.transform;

        _velocity  = vel;
        transform.position = pos;
        _despawning = false;

        if (TryGetComponent(out Collider projCol))
            foreach (Collider c in shooter.GetComponentsInChildren<Collider>())
                Physics.IgnoreCollision(projCol, c, true);
    }

/* ───────── Unity / Fish-Net ───────── */
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;
        _rb.useGravity  = false;
        _gravScale.OnChange += OnGravityChanged;
    }
    
    void OnDestroy() => _gravScale.OnChange -= OnGravityChanged;
    
    void OnGravityChanged(float prev, float next, bool asServer)
    {
        _gAcc = (next != 0f) ? Physics.gravity * next : Vector3.zero;
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

        if (projectileTrail != null)
        {
            projectileTrail.Clear(); // Good for pooled objects
            projectileTrail.Play();  // Start emission
        }
    }

    public override void OnStopClient()
    {
        if (!IsServer) TimeManager.OnTick -= ClientTick; 
        
        if (projectileTrail != null && projectileTrail.transform.parent == this.transform)
        {
            projectileTrail.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

/* ───────── server-side physics ───────── */
    void ServerTick()
    {
        if (_despawning || _def == null) return;

        double dt = TimeManager.TickDelta;
        if ((TimeManager.Tick - _spawnTick.Value) * dt >= _def.lifeTime)
        { DespawnSelf(); return; }

        if (_gAcc != Vector3.zero)
            _velocity += _gAcc * (float)dt;

        Vector3 from = transform.position;
        Vector3 to   = from + _velocity * (float)dt;

        /* 1) overlap at origin */
        int cnt = Physics.OverlapSphereNonAlloc(from, _def.castRadius, _buf, _def.hitMask, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < cnt; i++)
        {
            if (_buf[i] == null) continue;
            if (_buf[i].transform.root == _shooterRoot) continue;
            Explode(from, Vector3.up);
            return;
        }
        
        /* 2) explicit sweep */
        if (Sweep(from, to, out RaycastHit hit) && hit.collider.transform.root != _shooterRoot)
        {
            transform.position = hit.point;
            Explode(hit.point, hit.normal);
            return;
        }

        /* 3) nothing hit – move */
        transform.position = to;
    }

    /* generic sweep based on definition */
    bool Sweep(Vector3 from, Vector3 to, out RaycastHit hit)
    {
        Vector3 dir = (to - from);
        float len   = dir.magnitude;
        dir        /= len;

        switch (_def.castMode)
        {
            case CastMode.Sphere:
                return Physics.SphereCast(from, _def.castRadius, dir,
                          out hit, len, _def.hitMask,
                          QueryTriggerInteraction.Ignore);

            case CastMode.Capsule:
                Vector3 p1 = from + Vector3.up * _def.castHalf;
                Vector3 p2 = from - Vector3.up * _def.castHalf;
                return Physics.CapsuleCast(p1, p2, _def.castRadius, dir,
                          out hit, len, _def.hitMask,
                          QueryTriggerInteraction.Ignore);

            case CastMode.Ray:
                return Physics.Raycast(from, dir, out hit, len,
                          _def.hitMask, QueryTriggerInteraction.Ignore);
        }
        hit = default;
        return false;
    }

/* ───────── explosion & knock-back ───────── */
    void Explode(Vector3 pos, Vector3 normal)
    {
        ApplyExplosion(pos, _velocity.normalized);
        RpcSpawnImpact(pos, normal);
        DespawnSelf();
    }

    // ---------------------------------------------------------------------
    void ApplyExplosion(Vector3 centre, Vector3 shotDir)
    {
        int hitCount = Physics.OverlapSphereNonAlloc(centre, _def.blastRadius, _buf, _def.playerMask, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < hitCount; ++i)
        {
            if (!_buf[i].TryGetComponent(out AdvancedPredictedController ctrl))
                continue;

            /* ----- radial vs. fallback direction -------------------------- */
            Vector3 to    = _buf[i].ClosestPoint(centre) - centre;
            float   dist  = to.magnitude;

            Vector3 dir   = (dist < _def.minDirThreshold)
                ? shotDir                              // straight-up/fallback
                : to / dist;                           // radial

            /* ----- fall-off power (0…1) ----------------------------------- */
            float power   = Mathf.Pow(1f - Mathf.Clamp01(dist / _def.blastRadius), _def.knockFalloffExp);

            Vector3 impulse = dir * (_def.knockbackForce * power);

            /* ----- apply locally + broadcast ------------------------------ */
            ctrl.ReceiveKnockback(impulse);
            RpcApplyKnockback(ctrl.NetworkObject, impulse);
        }

        for (int i = 0; i < hitCount; ++i) _buf[i] = null;   // hygiene
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
        projectileTrail.Stop();
        
        if (projectileExplosion != null)
            Instantiate(projectileExplosion, pos, projectileExplosion.transform.rotation, null);
    }

/* ───────── client interpolation ───────── 
    void ClientTick()
    {
        if (IsServer || _despawning || _spawnTick.Value == 0) return;
        if (TimeManager.Tick < _spawnTick.Value) return;

        _prev   = transform.position;
        uint e  = TimeManager.Tick - _spawnTick.Value;
        _tickDt = (float)TimeManager.TickDelta;
        _next   = _initPos.Value + _initVel.Value * (e * _tickDt);
        _timer  = 0f;
    }
    */

    void ClientTick()
    {
        if (IsServer || _despawning || _spawnTick.Value == 0) return;
        if (TimeManager.Tick < _spawnTick.Value) return;

        _prev   = transform.position;

        uint   e     = TimeManager.Tick - _spawnTick.Value;       // elapsed ticks
        float  dt    = (float)TimeManager.TickDelta;              // seconds / tick
        float  t     = e * dt;                                    // seconds since spawn

        /* ---- predicted position identical to server integration ---- */
        _next = _initPos.Value + _initVel.Value * t + _gAcc * (0.5f * t * t);

        _tickDt = dt;
        _timer  = 0f;
    }

    void LateUpdate()
    {
        if (IsServer || _despawning || _tickDt <= 0f || _spawnTick.Value == 0) return;
        _timer += Time.deltaTime;
        transform.position = Vector3.Lerp(_prev, _next, Mathf.Clamp01(_timer / _tickDt));
    }

/* ───────── despawn ───────── */
    [Server] void DespawnSelf()
    {
        if (_despawning) return;
        _despawning = true;
        ServerManager.Despawn(gameObject, DespawnType.Pool);
    }
}

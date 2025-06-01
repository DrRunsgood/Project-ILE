using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using _Scripts.Player;

[RequireComponent(typeof(Rigidbody))]
public sealed class OGBaseProjectile : NetworkBehaviour
{
    /* ───────── Inspector ───────── */
    [Header("General")]
    [SerializeField] float     lifeTime   = 3f;
    [SerializeField] float     radius     = 0.20f;            // physics body radius
    [SerializeField] LayerMask hitMask;
    [SerializeField] LayerMask playerMask;
    public ParticleSystem projectileTrail;

    [Header("Explosion / Knock‑back")]
    [SerializeField] float  blastRadius        = 6f;
    [SerializeField] float  knockbackForce     = 500f;
    [SerializeField, Tooltip("1 = linear, 2 = quadratic, 1.5 ≈ Tribes")]
    float knockbackExponent   = 1.5f;
    [SerializeField, Tooltip("Treat centre hits as straight‑up impulse below this distance")]
    float minDirectionThreshold = 0.01f;
    public GameObject projectileExplosion;

    /* ───────── Synced spawn state ───────── */
    readonly SyncVar<Vector3> _initPos   = new();
    readonly SyncVar<Vector3> _initVel   = new();
    readonly SyncVar<uint>    _spawnTick = new();

    /* ───────── Runtime ───────── */
    Rigidbody   _rb;
    bool        _despawning;
    Transform   _shooterRoot;
    Vector3     _currentVelocity;
    
    /* interpolation */
    Vector3 _prev, _next;
    float   _timer, _tickDt;

    /* Non‑alloc buffer for overlap queries */
    readonly Collider[] _overlapBuf = new Collider[32];

    /* ---------------- public API ---------------- */
    public void Init(Vector3 pos, Vector3 vel, uint tick, NetworkObject shooter)
    {
        _initPos.Value   = pos;
        _initVel.Value   = vel;
        _spawnTick.Value = tick;
        _shooterRoot     = shooter.transform;

        _currentVelocity = vel;
        transform.position = pos;
        _despawning = false;

        if (TryGetComponent(out Collider projCol))
            foreach (Collider c in shooter.GetComponentsInChildren<Collider>())
                Physics.IgnoreCollision(projCol, c, true);
    }

    /* ---------------- Unity / FishNet ---------------- */
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;
        _rb.useGravity  = false;
    }

    public override void OnStartServer() => TimeManager.OnTick += ServerTick;
    public override void OnStopServer () => TimeManager.OnTick -= ServerTick;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!IsServer)
        {
            TimeManager.OnTick += ClientTick;
            _prev = _next = transform.position;
            _tickDt = (float)(TimeManager?.TickDelta ?? (1f / 60f));
        }
        
        // Play projectile trail
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
    
    void ServerTick()
    {
        if (_despawning) return;

        double dt = TimeManager.TickDelta;
        if ((TimeManager.Tick - _spawnTick.Value) * dt >= lifeTime) { DespawnSelf(); return; }

        Vector3 disp   = _currentVelocity * (float)dt;
        Vector3 origin = transform.position;
        Vector3 target = origin + disp;
        Vector3 dir    = _currentVelocity.normalized;

        // overlap right at the start (spawns inside wall)
        int cnt = Physics.OverlapSphereNonAlloc(origin, radius, _overlapBuf, hitMask, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < cnt; i++)
        {
            Collider c = _overlapBuf[i];
            if (c.transform.root == _shooterRoot) continue;
            transform.position = origin;
            Explode(origin, Vector3.up);
            return;
        }

        // sphere‑cast forward
        if (disp.sqrMagnitude > 0.0001f && Physics.SphereCast(origin, radius, dir, out RaycastHit hit, disp.magnitude,
                               hitMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.transform.root != _shooterRoot)
            {
                transform.position = hit.point;
                Explode(hit.point, hit.normal);
                return;
            }
        }

        // 3) nothing hit → move
        transform.position = target;
    }

    /* ---------------- Explosion & knock‑back ---------------- */
    void Explode(Vector3 pos, Vector3 normal)
    {
        ApplyExplosionKnockback(pos, _currentVelocity.normalized);
        PlayImpactObservers(pos, normal);
        DespawnSelf();
    }

    void ApplyExplosionKnockback(Vector3 explosionCenter, Vector3 fallbackDir)
    {
        int hits = Physics.OverlapSphereNonAlloc(explosionCenter, blastRadius, _overlapBuf, playerMask, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < hits; i++)
        {
            Collider col = _overlapBuf[i];
            if (!col.TryGetComponent(out AdvancedPredictedController ctrl))
                continue;

            Vector3 hitPt       = col.ClosestPoint(explosionCenter);
            Vector3 toTarget    = hitPt - explosionCenter;
            float   distance    = toTarget.magnitude;

            /* ---------- direction ---------- */
            Vector3 knockDir;
            if (distance < minDirectionThreshold)                    // «inside» or almost inside
                knockDir = fallbackDir.normalized;                   // ← use projectile flight‑dir
            else
                knockDir = toTarget / distance;                      // ← normal radial dir

            /* ---------- power fall‑off ---------- */
            float normDist = Mathf.Clamp01(distance / blastRadius);  // 0..1
            float power    = Mathf.Pow(1f - normDist, knockbackExponent);

            Vector3 impulse = knockDir * (knockbackForce * power);

            // owner + host this tick, others via RPC
            ctrl.ReceiveKnockback(impulse);
            RpcApplyKnockback(ctrl.NetworkObject, impulse);
        }
        
        for (int i = 0; i < hits; i++) _overlapBuf[i] = null;  // clear buffer
    }


    /* RPC that lands on every client, incl. owner */
    [ObserversRpc(BufferLast = false, ExcludeOwner = false)]
    void RpcApplyKnockback(NetworkObject playerNO, Vector3 impulse)
    {
        if (playerNO.TryGetComponent(out AdvancedPredictedController ctrl))
            ctrl.ReceiveKnockback(impulse);
    }

    /* ---------------- Client interpolation ---------------- */
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

    void LateUpdate()
    {
        if (IsServer || _despawning || _tickDt <= 0f || _spawnTick.Value == 0) return;
        _timer += Time.deltaTime;
        transform.position = Vector3.Lerp(_prev, _next, Mathf.Clamp01(_timer / _tickDt));
    }

    // --------------- VFX --------------- 
    [ObserversRpc(BufferLast = false)]
    void PlayImpactObservers(Vector3 pos, Vector3 normal)
    {
        projectileTrail.Stop();
        
        if (projectileExplosion != null)
            Instantiate(projectileExplosion, pos, projectileExplosion.transform.rotation, null);
    }
    
    [Server] void DespawnSelf()
    {
        if (_despawning) return;
        _despawning = true;
        ServerManager.Despawn(gameObject, DespawnType.Pool);
    }
}

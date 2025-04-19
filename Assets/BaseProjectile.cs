using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public sealed class BaseProjectile : NetworkBehaviour
{
    [Header("Tuning")]
    [SerializeField] private float lifeTime                  = 3f;
    [SerializeField] private float radius                    = 0.2f;
    [SerializeField] private LayerMask hitMask;
    [SerializeField, Range(0f, 2f)] private float velocityInheritanceFactor = .5f;

    // --- Synced spawn state ---
    private readonly SyncVar<Vector3> _initPos   = new();
    private readonly SyncVar<Vector3> _initVel   = new();
    private readonly SyncVar<uint>    _spawnTick = new();

    // --- Runtime ---
    private Rigidbody _rb;
    private bool      _despawning;
    private Transform _shooterRoot;

    // --- Client interpolation ---
    private Vector3 _prev, _next;
    private float   _timer, _tickDt;

    /// <summary>
    /// Call before Spawn(): sets up position, velocity, tick and shooter.
    /// </summary>
    public void Init(Vector3 pos, Vector3 finalVelocity, uint tick, NetworkObject shooter)
    {
        _initPos.Value   = pos;
        _initVel.Value   = finalVelocity;
        _spawnTick.Value = tick;
        _shooterRoot     = shooter ? shooter.transform : null;

        transform.position = pos;
        _despawning        = false;

        if (_shooterRoot && TryGetComponent(out Collider projCol))
        {
            foreach (var c in shooter.GetComponentsInChildren<Collider>())
                Physics.IgnoreCollision(projCol, c, true);
        }
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;
        _rb.useGravity  = false;
    }

    public override void OnStartServer() => TimeManager.OnTick += ServerTick;
    public override void OnStopServer()  => TimeManager.OnTick -= ServerTick;

    public override void OnStartClient()
    {
        if (!IsServer) TimeManager.OnTick += ClientTick;
        _prev   = transform.position;
        _next   = transform.position;
        _tickDt = (float)(TimeManager?.TickDelta ?? (1f / 60f));
        _timer  = 0f;
    }
    public override void OnStopClient()
    {
        if (!IsServer) TimeManager.OnTick -= ClientTick;
    }

    private void ServerTick()
    {
        if (_despawning || !IsSpawned || TimeManager == null) return;

        double dt = TimeManager.TickDelta;
        if (dt <= 0) return;

        uint spawnTick   = _spawnTick.Value;
        uint currentTick = TimeManager.Tick;

        if ((currentTick - spawnTick) * dt >= lifeTime)
        {
            DespawnSelf();
            return;
        }

        Vector3 disp        = _initVel.Value * (float)dt;
        Vector3 currentPos  = transform.position;
        Vector3 targetPos   = currentPos + disp;
        Vector3 direction   = _initVel.Value.normalized;
        bool    firstTick   = (currentTick == spawnTick);
        float   castDist    = disp.magnitude;
        Vector3 castOrigin  = currentPos;

        if (firstTick && disp.sqrMagnitude > 0.001f)
        {
            float backOffset = radius * 0.5f;
            castOrigin      = currentPos - direction * backOffset;
            castDist       += backOffset;
        }

        if (disp.sqrMagnitude > 0.001f &&
            Physics.SphereCast(castOrigin, radius, direction, out RaycastHit hit, castDist, hitMask, QueryTriggerInteraction.Ignore))
        {
            // ignore self on first tick
            if (hit.collider.transform.root == _shooterRoot)
            {
                if (firstTick)
                {
                    transform.position = targetPos;
                    return;
                }
            }
            else
            {
                transform.position = hit.point;
                PlayImpactObservers(hit.point, hit.normal);
                Debug.Log("BOOOOOOOOOM!");
                DespawnSelf();
                return;
            }
        }

        transform.position = targetPos;
    }

    private void ClientTick()
    {
        if (IsServer || _despawning || _spawnTick.Value == 0 || TimeManager == null) return;
        if (TimeManager.Tick < _spawnTick.Value) return;

        _prev   = transform.position;
        uint e  = TimeManager.Tick - _spawnTick.Value;
        _tickDt = (float)TimeManager.TickDelta;
        if (_tickDt <= 0f) return;

        _next  = _initPos.Value + _initVel.Value * (e * _tickDt);
        _timer = 0f;
    }

    private void LateUpdate()
    {
        if (IsServer || _despawning || _tickDt <= 0f || _spawnTick.Value == 0) return;
        _timer += Time.deltaTime;
        transform.position = Vector3.Lerp(_prev, _next, Mathf.Clamp01(_timer / _tickDt));
    }

    [ObserversRpc(BufferLast = true)]
    private void PlayImpactObservers(Vector3 pos, Vector3 normal)
    {
        // TODO: VFX / sound
    }

    [Server]
    private void DespawnSelf()
    {
        if (_despawning) return;
        _despawning = true;
        if (IsSpawned) ServerManager.Despawn(gameObject, DespawnType.Pool);
    }
}

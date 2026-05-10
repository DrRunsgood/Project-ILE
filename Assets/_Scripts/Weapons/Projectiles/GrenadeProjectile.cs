// _Scripts/Weapons/Projectiles/GrenadeProjectile.cs
using UnityEngine;
using FishNet.Object;
using _Scripts.Player;

[RequireComponent(typeof(Rigidbody))]
public sealed class GrenadeProjectile : BaseProjectile
{
    /* ───────── Inspector ───────── */
    [Header("Fuse")]
    [SerializeField] float fuseTime = 2.25f;

    [Header("Bounce")]
    [Range(0f, 1f)]
    [SerializeField] float bounciness = 0.35f;
    [SerializeField] float stopSpeed = 0.35f;
    [SerializeField] float surfaceLift = 0.01f;

    [Header("Visual")]
    [SerializeField] float blastLift = 0.08f;

    /* ───────── Runtime ───────── */
    float _fuseRemaining;
    bool _fuseExpired;
    bool _sleeping;

    struct BounceState
    {
        public Vector3 Pos;
        public Vector3 Vel;
        public uint Tick;
        public bool IsSleeping;
    }

    /* ═════════ INITIALISATION ═════════ */
    void OnEnable()
    {
        _sleeping = false;
        _fuseExpired = false;
        _fuseRemaining = fuseTime;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        _fuseRemaining = fuseTime;
    }

    public new void Init(Vector3 pos, Vector3 vel, uint tick, NetworkObject shooter)
    {
        base.Init(pos, vel, tick, shooter);
        ResolvePenetration();
    }

    /* ═════════ Bounce correction RPC ═════════ */
    [ObserversRpc(BufferLast = false, ExcludeOwner = false)]
    void RpcCorrectBounce(BounceState s)
    {
        if (IsServer) return;

        // Preserve current visual position so LateUpdate can blend into the correction.
        Vector3 currentVisualPos = transform.position;

        _spawnTick = s.Tick;
        _initPos = s.Pos;
        _initVel = s.Vel;
        _velocity = s.Vel;
        _sleeping = s.IsSleeping;

        _prev = currentVisualPos;
        _next = s.Pos;

        // Snap only if error is huge; otherwise let LateUpdate smooth it.
        float snapDistSqr = (currentVisualPos - s.Pos).sqrMagnitude;
        const float hardSnapDist = 2.0f; // tune if needed

        if (snapDistSqr > hardSnapDist * hardSnapDist)
        {
            transform.position = s.Pos;
            _prev = _next = s.Pos;
        }

        _tickDt = (float)TimeManager.TickDelta;
        _timer = 0f;
    }

    protected override void ServerTick()
    {
        if (_despawning) return;
        ResolvePenetration();
        float dt = (float)TimeManager.TickDelta;

        if (!_fuseExpired)
        {
            _fuseRemaining -= dt;
            if (_fuseRemaining <= 0f) _fuseExpired = true;
        }

        if (_sleeping)
        {
            Explode(transform.position + Vector3.up * 0.05f, Vector3.up);
            return;
        }

        if (gravAcc != Vector3.zero) _velocity += gravAcc * dt;
        Vector3 from = transform.position;
        float speed = _velocity.magnitude;
        if (speed <= 0f) return;

        Vector3 dir = _velocity / speed;
        float cast = speed * dt + def.castRadius;

        if (Physics.SphereCast(from, def.castRadius, dir, out var hit, cast, def.hitMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.transform.root == _shooterRoot)
            {
                transform.position = from + _velocity * dt;
                return;
            }

            if (hit.collider.TryGetComponent(out AdvancedPredictedController _))
            {
                Explode(hit.point + hit.normal * blastLift, hit.normal, hit.collider);
                return;
            }

            transform.position = hit.point + hit.normal * (def.castRadius + surfaceLift);

            if (_fuseExpired)
            {
                Explode(transform.position + hit.normal * blastLift, hit.normal);
                return;
            }

            _velocity = Vector3.Reflect(_velocity, hit.normal) * bounciness;
            
            bool isNowSleeping = _velocity.sqrMagnitude < stopSpeed * stopSpeed;
            if (isNowSleeping)
            {
                _velocity = Vector3.zero;
                _sleeping = true;
            }

            RpcCorrectBounce(new BounceState { Pos = transform.position, Vel = _velocity, Tick = TimeManager.Tick, IsSleeping = isNowSleeping });
        }
        else
        {
            transform.position = from + _velocity * dt;
        }
    }

    /* ═════════════════ CLIENT TICK (simplified) ═════════════════ */
    protected override void ClientTick()
    {
        if (IsServer || _despawning) return;
        if (TimeManager.Tick < _spawnTick) return;
        if (_sleeping) return;

        _prev = transform.position;
        float dt = (float)TimeManager.TickDelta;

        if (gravAcc != Vector3.zero)
        {
            _velocity += gravAcc * dt;
        }
        transform.position += _velocity * dt;

        _next = transform.position;
        _tickDt = dt;
        _timer = 0f;
    }

    protected override bool Sweep(Vector3 a, Vector3 b, out RaycastHit h)
    {
        h = default;
        return false;
    }

    void ResolvePenetration()
    {
        const int ITERATIONS = 3;
        const float SKIN = 0.001f;
        for (int step = 0; step < ITERATIONS; ++step)
        {
            bool moved = false;
            int count = Physics.OverlapSphereNonAlloc(transform.position, def.castRadius, _buf, def.hitMask, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < count; ++i)
            {
                Collider col = _buf[i];
                if (col == null || col.transform.root == _shooterRoot) continue;

                Vector3 centre = transform.position;
                Vector3 cp = col.ClosestPoint(centre);
                Vector3 dir = centre - cp;
                float dist = dir.magnitude;

                if (dist < 1e-4f)
                {
                    dir = Vector3.up;
                    dist = 0f;
                }
                else
                {
                    dir /= dist;
                }

                float penetration = def.castRadius - dist;
                if (penetration > 0f)
                {
                    transform.position += dir * (penetration + SKIN);
                    moved = true;
                }
                _buf[i] = null;
            }
            if (!moved) break;
        }
    }
}
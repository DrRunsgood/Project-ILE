//  _Scripts/Physics/KinematicMover.cs
using FishNet;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;

namespace _Scripts.GamePhysics
{
    /// Deterministic kinematic motion helper for dropped items / projectiles.
    /// ▸ server authoritative + client‑predicted
    /// ▸ no Rigidbody; exactly one SphereCast per network‑tick
    public sealed class KinematicMover : NetworkBehaviour
    {
    /*──────────── CONFIG (per‑prefab) ────────────*/
        [Header("Motion")]
        [SerializeField] float gravityScale   = 1f;      // 0 → no gravity
        [SerializeField] float bounciness     = 0.35f;   // for Bounce
        [SerializeField] float slideFriction  = 6f;      // m/s² when sliding
        [SerializeField] float stopSpeed      = 0.05f;   // below → sleep

        [Header("Sphere Cast")]
        [SerializeField] float sphereRadius   = 0.15f;   // metres

        [Header("Collision")]
        [SerializeField] LayerMask worldMask  = ~0;
        public enum Reaction { Bounce, Slide, Stop }
        [SerializeField] Reaction reaction    = Reaction.Bounce;

        [Header("Lifetime  (‑1 = infinite)")]
        [SerializeField] float lifeTime       = -1f;     // seconds

    /*──────────── RUNTIME ────────────*/
        Vector3   _velocity;
        uint      _spawnTick;
        bool      _simulating;
        RaycastHit _hit;

    /*──────────── SPAWN PUSH‑OUT ──────*/
        void Awake()
        {
            // If the prefab spawns clipping the ground, nudge it up once.
            if (Physics.CheckSphere(transform.position, sphereRadius, worldMask))
                if (Physics.Raycast(transform.position + Vector3.up * 0.5f,
                                    Vector3.down, out var info, 2f, worldMask))
                    transform.position =
                        info.point + info.normal * (sphereRadius + 0.002f);
        }

    /*════════════ PUBLIC API ══════════*/
        /// Call **only on the server** immediately after spawning.
        public void InitVelocity(Vector3 vel)
        {
            _velocity   = vel;
            _spawnTick  = TimeManager.Tick;
            _simulating = true;
            RpcInit(_velocity, _spawnTick);            // buffered for late joiners
        }

    /*──────────── FishNet hooks ───────*/
        public override void OnStartServer()
        {
            if (_spawnTick == 0) _spawnTick = TimeManager.Tick;   // scene‑placed
            TimeManager.OnTick += ServerTick;
            _simulating = true;
        }
        public override void OnStopServer() =>
            TimeManager.OnTick -= ServerTick;

        public override void OnStartClient()
        {
            if (!IsServer) TimeManager.OnTick += ClientTick;      // host ticks once
        }
        public override void OnStopClient()
        {
            if (!IsServer) TimeManager.OnTick -= ClientTick;
        }

    /*──────────── Buffered init RPC ───*/
        [ObserversRpc(BufferLast = true)]
        void RpcInit(Vector3 vel, uint tick)
        {
            if (IsServer) return;      // host already has state
            _velocity   = vel;
            _spawnTick  = tick;
            _simulating = true;
        }

    /*──────────── Tick plumbing ───────*/
        void ServerTick() { if (_simulating) Step(); }   // authority
        void ClientTick() { if (_simulating) Step(); }   // prediction

    /*──────────── Core simulation ─────*/
        void Step()
        {
            double dt = TimeManager.TickDelta;

            /* 1) lifetime fuse */
            if (lifeTime > 0 &&
               (TimeManager.Tick - _spawnTick) * dt >= lifeTime)
            {
                _simulating = false;
                return;
            }

            /* 2) gravity */
            if (gravityScale != 0f && _velocity != Vector3.zero)
                _velocity += Physics.gravity * (gravityScale * (float)dt);

            if (_velocity == Vector3.zero) return;       // resting

            Vector3 from = transform.position;
            float   dist = _velocity.magnitude * (float)dt;

            bool hitWorld = Physics.SphereCast(from, sphereRadius,
                                               _velocity.normalized,
                                               out _hit, dist,
                                               worldMask,
                                               QueryTriggerInteraction.Ignore);

            /* 3) move / collide */
            if (hitWorld)
            {
                // Park so the sphere rests exactly on the surface.
                transform.position = _hit.point + _hit.normal * (sphereRadius + 0.001f);

                // Align local up (Y) to the surface normal for nicer visuals.
                Quaternion align = Quaternion.FromToRotation(transform.up, _hit.normal);
                transform.rotation = align * transform.rotation;

                switch (reaction)
                {
                    case Reaction.Bounce:
                        _velocity = Vector3.Reflect(_velocity, _hit.normal) * bounciness;
                        break;

                    case Reaction.Slide:
                        _velocity = Vector3.ProjectOnPlane(_velocity, _hit.normal);
                        _velocity = Vector3.MoveTowards(_velocity, Vector3.zero,
                                                        slideFriction * (float)dt);
                        break;

                    case Reaction.Stop:
                        _velocity = Vector3.zero;
                        break;
                }
            }
            else
            {
                transform.position = from + _velocity * (float)dt;
            }

            /* 4) sleep test */
            if (reaction != Reaction.Bounce &&
                _velocity.sqrMagnitude < stopSpeed * stopSpeed)
            {
                _velocity   = Vector3.zero;
                _simulating = false;
            }
        }
    }
}
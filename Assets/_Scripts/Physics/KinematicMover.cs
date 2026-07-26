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
        [SerializeField] float gravityScale   = 1f;
        [SerializeField] float bounciness     = 0.35f;   // for Bounce
        [SerializeField] float slideFriction  = 6f;      // m/s² when sliding
        [SerializeField] float stopSpeed      = 0.05f;   // below → sleep

        [Header("Sphere Cast")]
        [SerializeField] float sphereRadius   = 0.15f;   // metres

        [Header("Collision")]
        [SerializeField] LayerMask worldMask  = ~0;
        public enum Reaction { Bounce, Slide, Stop }
        [SerializeField] Reaction reaction    = Reaction.Bounce;

        [Header("Lifetime  (‑1 = infinite)")]
        [SerializeField] float lifeTime       = -1f;     // seconds

    /*──────────── RUNTIME ────────────*/
        Vector3   _velocity;
        uint      _spawnTick;
        bool      _simulating;
        RaycastHit _hit;
        
        private static readonly Collider[] InitialOverlapHits = new Collider[16];

    /*════════════ PUBLIC API ══════════*/
        /// Call **only on the server** immediately after spawning.
        [Server]
        public void InitVelocity(Vector3 vel, Transform ignoredRoot = null)
        {
            if (TimeManager == null)
            {
                Debug.LogError($"[KinematicMover] '{name}' cannot initialize without a TimeManager.", this);

                _velocity = Vector3.zero;
                _spawnTick = 0;
                _simulating = false;

                return;
            }

            _spawnTick = TimeManager.Tick;

            if (HasBlockingInitialOverlap(ignoredRoot))
            {
                Debug.LogWarning($"[KinematicMover] '{name}' began inside blocking geometry. Initial motion was cancelled.", this);

                _velocity = Vector3.zero;
                _simulating = false;

                RpcInit(Vector3.zero, _spawnTick, false);

                return;
            }

            _velocity = vel;
            _simulating = true;

            RpcInit(_velocity, _spawnTick, true);
        }

    /*──────────── FishNet hooks ───────*/
        public override void OnStartServer()
        {
            base.OnStartServer();

            TimeManager.OnTick += ServerTick;

            // Do not auto-start movement.
            // Spawner-created pickups should remain anchored until InitVelocity is explicitly called.
            _velocity = Vector3.zero;
            _spawnTick = TimeManager.Tick;
            _simulating = false;
        }
        public override void OnStopServer()
        {
            if (TimeManager != null)
                TimeManager.OnTick -= ServerTick;

            _velocity = Vector3.zero;
            _simulating = false;
            _spawnTick = 0;

            base.OnStopServer();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (!IsServer)
            {
                _velocity = Vector3.zero;
                _simulating = false;
                _spawnTick = TimeManager != null ? TimeManager.Tick : 0;

                TimeManager.OnTick += ClientTick;
            }
        }
        public override void OnStopClient()
        {
            if (!IsServer && TimeManager != null)
                TimeManager.OnTick -= ClientTick;

            if (!IsServer)
            {
                _velocity = Vector3.zero;
                _simulating = false;
                _spawnTick = 0;
            }

            base.OnStopClient();
        }

    /*──────────── Buffered init RPC ───*/
        [ObserversRpc(BufferLast = true)]
        void RpcInit(Vector3 vel, uint tick, bool simulating)
        {
            if (IsServer)
                return;

            _velocity = vel;
            _spawnTick = tick;
            _simulating = simulating;
        }
        
        private bool HasBlockingInitialOverlap(Transform ignoredRoot)
        {
            int hitCount = Physics.OverlapSphereNonAlloc(transform.position, sphereRadius, InitialOverlapHits, worldMask,
                    QueryTriggerInteraction.Ignore);

            for (int i = 0; i < hitCount; i++)
            {
                Collider hit = InitialOverlapHits[i];

                if (hit == null)
                    continue;

                Transform hitTransform = hit.transform;

                /*
                 * Ignore colliders belonging to this pickup itself.
                 * This supports colliders placed on the mover object,
                 * its children, or a shared prefab parent.
                 */
                if (IsSameHierarchy(hitTransform, transform))
                    continue;
                

                /*
                 * Player drops may intentionally begin close to the
                 * dropping player's capsule. That player is not world
                 * geometry and must not invalidate the drop.
                 */
                if (ignoredRoot != null && IsSameHierarchy(hitTransform, ignoredRoot))
                    continue;

                return true;
            }

            return false;
        }

        private static bool IsSameHierarchy(Transform first, Transform second)
        {
            if (first == null || second == null)
                return false;

            return first == second || first.IsChildOf(second) || second.IsChildOf(first);
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
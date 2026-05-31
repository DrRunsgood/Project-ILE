using FishNet.Object;
using UnityEngine;

namespace _Scripts.Game.CTF
{
    [DisallowMultipleComponent]
    public sealed class FlagMover : NetworkBehaviour
    {
        [Header("Movement")]
        [SerializeField] float gravityMultiplier = 1f;
        [SerializeField] float airDrag = 0.15f;
        [SerializeField] float groundFriction = 0.65f;
        [SerializeField, Range(0f, 1f)] float bounce = 0.35f;
        
        [Header("Collision")]
        [SerializeField] LayerMask collisionMask = ~0;
        [SerializeField] float radius = 0.35f;
        [SerializeField] float skinWidth = 0.025f;
        [SerializeField] int maxBouncesPerTick = 3;

        [Header("Sleep")]
        [SerializeField] float sleepSpeed = 0.75f;
        [SerializeField] float groundNormalY = 0.55f;

        [Header("Debug")]
        [SerializeField] bool drawDebug;

        Vector3 _velocity;
        bool _moving;

        public Vector3 Velocity => _velocity;
        public bool IsMoving => _moving;
        
        public override void OnStartNetwork()
        {
            base.OnStartNetwork();
            TimeManager.OnTick += HandleTick;
        }

        public override void OnStopNetwork()
        {
            base.OnStopNetwork();
            TimeManager.OnTick -= HandleTick;
        }

        void HandleTick()
        {
            if (!_moving)
                return;

            Simulate((float)TimeManager.TickDelta);
        }
        
        [Server]
        public void Server_BeginMove(Vector3 position, Vector3 velocity)
        {
            transform.position = position;
            _velocity = velocity;
            ApplyUprightRotation();
            _moving = true;
        }

        [Server]
        public void Server_Stop()
        {
            _velocity = Vector3.zero;
            _moving = false;
        }

        [Server]
        public void Server_AddImpulse(Vector3 impulse)
        {
            _velocity += impulse;
            _moving = true;
        }

        [Server]
        public void Server_SetPosition(Vector3 position)
        {
            transform.position = position;
        }
        
        void Simulate(float dt)
        {
            if (dt <= 0f)
                return;

            Vector3 pos = transform.position;

            // Gravity.
            _velocity += Physics.gravity * gravityMultiplier * dt;

            // Air drag as exponential-ish damping.
            float dragFactor = Mathf.Clamp01(1f - airDrag * dt);
            _velocity *= dragFactor;

            Vector3 remainingMove = _velocity * dt;

            for (int i = 0; i < maxBouncesPerTick; i++)
            {
                float distance = remainingMove.magnitude;
                if (distance <= 0.0001f)
                    break;

                Vector3 dir = remainingMove / distance;

                if (!Physics.SphereCast(
                        pos,
                        radius,
                        dir,
                        out RaycastHit hit,
                        distance + skinWidth,
                        collisionMask,
                        QueryTriggerInteraction.Ignore))
                {
                    pos += remainingMove;
                    remainingMove = Vector3.zero;
                    break;
                }

                float safeDistance = Mathf.Max(0f, hit.distance - skinWidth);
                pos += dir * safeDistance;

                Vector3 normal = hit.normal;

                if (drawDebug)
                    Debug.DrawRay(hit.point, normal, Color.yellow, 0.25f);

                // Split velocity into normal and tangent parts.
                Vector3 normalVel = Vector3.Project(_velocity, normal);
                Vector3 tangentVel = _velocity - normalVel;

                // Bounce away from surface.
                if (Vector3.Dot(normalVel, normal) < 0f)
                    normalVel = -normalVel * bounce;

                // Surface friction affects slide.
                bool groundedLike = normal.y >= groundNormalY;
                float friction = groundedLike ? groundFriction : groundFriction * 0.35f;
                tangentVel *= Mathf.Clamp01(1f - friction);

                _velocity = normalVel + tangentVel;

                // Continue remaining movement after bounce/slide.
                float remainingDistance = Mathf.Max(0f, distance - safeDistance);
                remainingMove = Vector3.ProjectOnPlane(dir * remainingDistance, normal);

                // Avoid sticky re-hit.
                pos += normal * skinWidth;

                if (_velocity.magnitude <= sleepSpeed && groundedLike)
                {
                    _velocity = Vector3.zero;
                    _moving = false;
                    remainingMove = Vector3.zero;
                    break;
                }
            }

            transform.position = pos;
            
            ApplyUprightRotation();

            if (_velocity.magnitude <= sleepSpeed)
            {
                // Only sleep freely if near ground below.
                if (Physics.SphereCast(
                        pos + Vector3.up * 0.05f,
                        radius,
                        Vector3.down,
                        out RaycastHit groundHit,
                        radius + 0.15f,
                        collisionMask,
                        QueryTriggerInteraction.Ignore) &&
                    groundHit.normal.y >= groundNormalY)
                {
                    _velocity = Vector3.zero;
                    _moving = false;
                }
            }
        }
        
        public void Client_BeginMove(Vector3 position, Vector3 velocity)
        {
            transform.position = position;
            _velocity = velocity;
            ApplyUprightRotation();
            _moving = true;
        }

        public void Client_Stop()
        {
            _velocity = Vector3.zero;
            _moving = false;
        }
        
        void ApplyUprightRotation()
        {
            Vector3 forward = transform.forward;
            forward.y = 0f;

            if (forward.sqrMagnitude < 0.001f)
                forward = Vector3.forward;

            transform.rotation = Quaternion.LookRotation(forward.normalized, Vector3.up);
        }
    }
}
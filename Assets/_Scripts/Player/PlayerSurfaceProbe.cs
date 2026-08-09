using UnityEngine;

namespace _Scripts.Player
{
    [System.Serializable]
    public sealed class PlayerSurfaceProbe
    {
        private readonly float _maxSlopeAngle;
        private readonly float _slopeCheckDistance;
        private readonly LayerMask _groundMask;

        private readonly Vector3 _feetOffset;
        private readonly float _feetRadius;

        private readonly LayerMask _wallProbeMask;
        private readonly LayerMask _wallInteractionBlockedMask;

        private readonly float _wallCheckDistance;
        private readonly float _wallProbeRadius;
        private readonly float _minWallAngle;
        private readonly float _maxWallAngle;
        private readonly float _minJumpHeight;

        public bool IsGrounded { get; private set; }
        public bool IsOnSlope { get; private set; }

        public RaycastHit GroundHit { get; private set; }

        // Compatibility with the controller's current naming.
        public RaycastHit SlopeHit => GroundHit;

        public Vector3 GroundNormal =>
            GroundHit.collider != null
                ? GroundHit.normal
                : Vector3.up;

        public bool HasWallContact { get; private set; }
        public bool WallLeft { get; private set; }
        public bool WallRight { get; private set; }

        public RaycastHit WallHit { get; private set; }

        public Vector3 WallNormal => WallHit.collider != null ? WallHit.normal : Vector3.zero;

        public PlayerSurfaceProbe(float maxSlopeAngle, float slopeCheckDistance, LayerMask groundMask, Vector3 feetOffset,
            float feetRadius, LayerMask wallProbeMask, LayerMask wallInteractionBlockedMask, float wallCheckDistance,
            float wallProbeRadius, float minWallAngle, float maxWallAngle, float minJumpHeight)
        {
            _maxSlopeAngle = maxSlopeAngle;
            _slopeCheckDistance = slopeCheckDistance;
            _groundMask = groundMask;

            _feetOffset = feetOffset;
            _feetRadius = feetRadius;

            _wallProbeMask = wallProbeMask;
            _wallInteractionBlockedMask = wallInteractionBlockedMask;

            _wallCheckDistance = wallCheckDistance;
            _wallProbeRadius = wallProbeRadius;
            _minWallAngle = minWallAngle;
            _maxWallAngle = maxWallAngle;
            _minJumpHeight = minJumpHeight;
        }

        public void RefreshGrounding(Rigidbody rb)
        {
            IsGrounded = false;
            IsOnSlope = false;
            GroundHit = default;

            Vector3 feetPosition = rb.position + _feetOffset;

            bool touchingGround = Physics.CheckSphere(feetPosition, _feetRadius, _groundMask, QueryTriggerInteraction.Ignore);

            if (!touchingGround)
                return;

            Vector3 castOrigin = feetPosition + Vector3.up * 0.1f;

            float castRadius = Mathf.Max(0.01f, _feetRadius * 0.9f);

            if (!Physics.SphereCast(castOrigin, castRadius, Vector3.down, out RaycastHit hit,
                    _slopeCheckDistance, _groundMask, QueryTriggerInteraction.Ignore))
                return;
            

            float slopeAngle = Vector3.Angle(Vector3.up, hit.normal);

            if (slopeAngle > _maxSlopeAngle)
                return;

            GroundHit = hit;
            IsGrounded = true;
            IsOnSlope = slopeAngle > 0.1f;
        }

        /// <summary>
        /// Persistent wallrun-style probe.
        /// Uses the player's forward-facing orientation.
        /// </summary>
        public bool RefreshWallProbe(Transform playerTransform, Transform orientation)
        {
            Vector3 origin = playerTransform.position;

            Vector3 direction = orientation.forward;

            bool foundWall = TryGetWallContact(origin, direction, _wallCheckDistance, out RaycastHit hit);

            ClearWallProbe();

            if (!foundWall)
                return false;

            WallHit = hit;
            HasWallContact = true;

            float side = Vector3.Dot(orientation.right, hit.normal);

            WallRight = side > 0f;
            WallLeft = !WallRight;

            return true;
        }

        /// <summary>
        /// Discrete look-directed query for airborne wall jumping.
        /// Near-contact is sufficient; physical collider contact is not required.
        /// </summary>
        public bool TryGetWallJumpContact(Vector3 origin, Vector3 lookDirection, float distance, out RaycastHit hit)
        {
            return TryGetWallContact(origin, lookDirection, distance, out hit);
        }

        private bool TryGetWallContact(Vector3 origin, Vector3 direction, float distance, out RaycastHit hit)
        {
            hit = default;

            if (direction.sqrMagnitude <= 0.0001f)
                return false;

            direction.Normalize();

            if (!Physics.SphereCast(origin, _wallProbeRadius, direction, out hit,
                    distance, _wallProbeMask, QueryTriggerInteraction.Ignore))
                return false;
            

            if (!IsWallSurfaceValid(hit))
            {
                hit = default;
                return false;
            }

            return true;
        }

        private bool IsWallSurfaceValid(RaycastHit hit)
        {
            if (hit.collider == null || hit.collider.isTrigger)
                return false;
            
            int colliderLayerMask = 1 << hit.collider.gameObject.layer;

            // Terrain and other explicitly blocked surfaces cannot
            // participate in wall movement.
            if ((_wallInteractionBlockedMask.value & colliderLayerMask) != 0)
                return false;
            
            /*
             * Angle is measured between world-up and the surface normal:
             *
             * 0 degrees   = upward-facing floor
             * 90 degrees  = vertical wall
             * 180 degrees = downward-facing ceiling
             *
             * Starting range:
             * 75 to 100 degrees
             */
            float surfaceAngle = Vector3.Angle(Vector3.up, hit.normal);

            return surfaceAngle >= _minWallAngle && surfaceAngle <= _maxWallAngle;
        }

        public bool IsAboveMinJumpHeight(Transform playerTransform)
        {
            return !Physics.Raycast(playerTransform.position, Vector3.down,
                _minJumpHeight, _groundMask, QueryTriggerInteraction.Ignore);
        }

        public void ClearWallProbe()
        {
            HasWallContact = false;
            WallLeft = false;
            WallRight = false;
            WallHit = default;
        }

        public void DrawGroundGizmo(
            Rigidbody rb)
        {
            if (rb == null)
                return;

            Vector3 checkPosition =
                rb.position + _feetOffset;

            Gizmos.DrawWireSphere(
                checkPosition,
                _feetRadius);
        }
    }
}
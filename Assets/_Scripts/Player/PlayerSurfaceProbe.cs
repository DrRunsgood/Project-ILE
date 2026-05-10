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

        private readonly LayerMask _wallMask;
        private readonly float _wallCheckDistance;
        private readonly float _minJumpHeight;

        public bool IsGrounded { get; private set; }
        public bool IsOnSlope { get; private set; }
        public RaycastHit SlopeHit { get; private set; }

        public bool WallLeft { get; private set; }
        public bool WallRight { get; private set; }
        public Vector3 WallNormal { get; private set; } = Vector3.zero;

        public PlayerSurfaceProbe(
            float maxSlopeAngle,
            float slopeCheckDistance,
            LayerMask groundMask,
            Vector3 feetOffset,
            float feetRadius,
            LayerMask wallMask,
            float wallCheckDistance,
            float minJumpHeight)
        {
            _maxSlopeAngle = maxSlopeAngle;
            _slopeCheckDistance = slopeCheckDistance;
            _groundMask = groundMask;
            _feetOffset = feetOffset;
            _feetRadius = feetRadius;
            _wallMask = wallMask;
            _wallCheckDistance = wallCheckDistance;
            _minJumpHeight = minJumpHeight;
        }

        public void RefreshGrounding(Rigidbody rb, Transform transform)
        {
            Vector3 checkPos = rb.position + _feetOffset;
            IsGrounded = Physics.CheckSphere(checkPos, _feetRadius, _groundMask);

            IsOnSlope = false;
            SlopeHit = default;

            if (!IsGrounded)
                return;

            if (Physics.Raycast(checkPos, Vector3.down, out RaycastHit hit, _slopeCheckDistance, _groundMask)) //transform.position
            {
                float angle = Vector3.Angle(Vector3.up, hit.normal);
                if (angle < _maxSlopeAngle && angle != 0f)
                {
                    IsOnSlope = true;
                    SlopeHit = hit;
                }
            }
        }

        public void RefreshWallProbe(Transform transform, Transform orientation)
        {
            Vector3 rayOrigin = transform.position;
            Vector3 rayDirection = orientation.forward;

            WallLeft = false;
            WallRight = false;
            WallNormal = Vector3.zero;

            if (!Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, _wallCheckDistance, _wallMask))
                return;

            // Reject floor / ceiling-ish surfaces
            if (Mathf.Abs(Vector3.Dot(hit.normal, Vector3.up)) >= 0.1f)
                return;

            WallNormal = hit.normal;

            float side = Vector3.Dot(orientation.right, WallNormal);
            WallRight = side > 0f;
            WallLeft = side <= 0f;
        }

        public bool IsAboveMinJumpHeight(Transform transform)
        {
            return !Physics.Raycast(transform.position, Vector3.down, _minJumpHeight, _groundMask);
        }

        public void ClearWallProbe()
        {
            WallLeft = false;
            WallRight = false;
            WallNormal = Vector3.zero;
        }

        public void DrawGroundGizmo(Rigidbody rb)
        {
            if (rb == null)
                return;

            Vector3 checkPos = rb.position + _feetOffset;
            Gizmos.DrawWireSphere(checkPos, _feetRadius);
        }
    }
}
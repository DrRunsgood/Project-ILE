// _Scripts/Game/WorldDropContext.cs

using UnityEngine;

namespace _Scripts.Game
{
    public readonly struct WorldDropContext
    {
        public Vector3 Origin { get; }
        public Vector3 Direction { get; }
        public Vector3 PlayerVelocity { get; }

        public WorldDropContext(Vector3 origin, Vector3 direction, Vector3 playerVelocity)
        {
            Origin = origin;
            Direction = direction;
            PlayerVelocity = playerVelocity;
        }
    }

    public static class WorldDropUtil
    {
        private static readonly RaycastHit[] CastHits = new RaycastHit[16];

        private static readonly Collider[] OverlapHits = new Collider[16];

        private static readonly float[] DirectionOffsets =
        {
            0f,
            45f,
            -45f,
            90f,
            -90f,
            135f,
            -135f,
            180f
        };

        private static readonly float[] DistanceScales =
        {
            1f,
            0.75f,
            0.5f,
            0.25f,
            0f
        };

        private static readonly float[] LiftMultipliers =
        {
            0f,
            1f,
            2f,
            3f
        };

        public static Vector3 GetSafeDirection(Vector3 direction, Vector3 fallback)
        {
            if (direction.sqrMagnitude > 0.0001f)
                return direction.normalized;

            if (fallback.sqrMagnitude > 0.0001f)
                return fallback.normalized;

            return Vector3.forward;
        }

        public static bool TryResolveDrop(Transform ownerRoot, Vector3 origin, Vector3 preferredDirection, float offset, float radius,
            float backoff, LayerMask blockMask, out Vector3 resolvedPosition, out Vector3 resolvedDirection)
        {
            Vector3 fallbackDirection = ownerRoot != null ? ownerRoot.forward : Vector3.forward;
            
            Vector3 baseDirection = GetSafeDirection(preferredDirection, fallbackDirection);

            float safeOffset = Mathf.Max(0f, offset);

            float safeRadius = Mathf.Max(0.01f, radius);

            float safeBackoff = Mathf.Max(0f, backoff);

            resolvedPosition = origin;
            resolvedDirection = baseDirection;

            for (int liftIndex = 0; liftIndex < LiftMultipliers.Length; liftIndex++)
            {
                float liftDistance = LiftMultipliers[liftIndex] * (safeRadius + 0.05f);

                Vector3 testOrigin = origin + Vector3.up * liftDistance;

                for (int directionIndex = 0; directionIndex < DirectionOffsets.Length; directionIndex++)
                {
                    Vector3 candidateDirection = Quaternion.AngleAxis(DirectionOffsets[directionIndex], Vector3.up) * baseDirection;

                    candidateDirection = GetSafeDirection(candidateDirection, baseDirection);

                    if (!TryResolveAlongDirection(ownerRoot, testOrigin, candidateDirection, safeOffset, safeRadius,
                            safeBackoff, blockMask, out Vector3 candidatePosition))
                    {
                        continue;
                    }

                    resolvedPosition = candidatePosition;
                    resolvedDirection = candidateDirection;

                    return true;
                }
            }

            /*
             * Do not return a known-blocked position. The caller can
             * apply its existing manual/terminal failure semantics.
             */
            return false;
        }

        private static bool TryResolveAlongDirection(Transform ownerRoot, Vector3 origin, Vector3 direction, float offset,
            float radius, float backoff, LayerMask blockMask, out Vector3 resolvedPosition)
        {
            float availableDistance = offset;

            if (offset > 0f)
            {
                int hitCount = Physics.SphereCastNonAlloc(origin, radius, direction, CastHits, offset,
                        blockMask, QueryTriggerInteraction.Ignore);

                float closestDistance = offset;
                bool foundBlockingHit = false;

                for (int i = 0; i < hitCount; i++)
                {
                    RaycastHit hit = CastHits[i];

                    if (hit.collider == null)
                        continue;

                    if (IsOwnedCollider(ownerRoot, hit.collider))
                        continue;
                    

                    if (hit.distance >= closestDistance)
                        continue;

                    closestDistance = hit.distance;
                    foundBlockingHit = true;
                }

                if (foundBlockingHit)
                    availableDistance = Mathf.Max(0f, closestDistance - backoff);
                
            }

            for (int i = 0; i < DistanceScales.Length; i++)
            {
                float candidateDistance = availableDistance * DistanceScales[i];

                Vector3 candidate = origin + direction * candidateDistance;

                if (HasBlockingOverlap(ownerRoot, candidate, radius, blockMask))
                    continue;
                

                resolvedPosition = candidate;
                return true;
            }

            resolvedPosition = origin;
            return false;
        }

        private static bool HasBlockingOverlap(Transform ownerRoot, Vector3 position, float radius, LayerMask blockMask)
        {
            int overlapCount = Physics.OverlapSphereNonAlloc(position, radius, OverlapHits, blockMask, QueryTriggerInteraction.Ignore);

            for (int i = 0; i < overlapCount; i++)
            {
                Collider hit = OverlapHits[i];

                if (hit == null)
                    continue;

                if (IsOwnedCollider(ownerRoot, hit))
                    continue;

                return true;
            }

            return false;
        }

        private static bool IsOwnedCollider(Transform ownerRoot, Collider collider)
        {
            if (ownerRoot == null || collider == null)
                return false;

            Transform colliderTransform = collider.transform;

            return colliderTransform == ownerRoot || colliderTransform.IsChildOf(ownerRoot);
        }
    }
}
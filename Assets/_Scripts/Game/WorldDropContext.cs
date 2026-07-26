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

        public static Vector3 GetSafeDirection(Vector3 direction, Vector3 fallback)
        {
            if (direction.sqrMagnitude > 0.0001f)
                return direction.normalized;

            if (fallback.sqrMagnitude > 0.0001f)
                return fallback.normalized;

            return Vector3.forward;
        }

        public static Vector3 ResolveSafePosition(Transform ownerRoot, Vector3 origin, Vector3 direction, float offset,
            float radius, float backoff, LayerMask blockMask)
        {
            direction = GetSafeDirection(direction, ownerRoot != null ? ownerRoot.forward : Vector3.forward);

            float safeOffset = Mathf.Max(0f, offset);
            float safeRadius = Mathf.Max(0.01f, radius);
            float safeBackoff = Mathf.Max(0f, backoff);

            float travelDistance = safeOffset;

            if (safeOffset > 0f)
            {
                int hitCount = Physics.SphereCastNonAlloc(origin, safeRadius, direction, CastHits,
                    safeOffset, blockMask, QueryTriggerInteraction.Ignore);

                float closestDistance = safeOffset;
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
                    travelDistance = Mathf.Max(0f, closestDistance - safeBackoff);
                
            }

            Vector3 candidate = origin + direction * travelDistance;

            if (!HasBlockingOverlap(ownerRoot, candidate, safeRadius, blockMask))
                return candidate;
            

            float nearDistance = Mathf.Min(Mathf.Max(safeBackoff, 0.02f), safeOffset * 0.25f);

            Vector3 nearOrigin = origin + direction * nearDistance;

            if (!HasBlockingOverlap(ownerRoot, nearOrigin, safeRadius, blockMask))
                return nearOrigin;
            
            return origin;
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
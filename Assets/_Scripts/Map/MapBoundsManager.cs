using UnityEngine;

namespace _Scripts.Map
{
    [DisallowMultipleComponent]
    public sealed class MapBoundsManager : MonoBehaviour
    {
        public static MapBoundsManager Instance { get; private set; }

        [Header("Bounds")]
        [SerializeField] private Vector3 center = Vector3.zero;
        [SerializeField] private Vector2 sizeXZ = new Vector2(500f, 500f);

        [Header("Orientation")]
        [Tooltip("Optional. Forward = map north, Right = map east. If null, world +Z/+X are used.")]
        [SerializeField] private Transform orientationReference;

        public Vector3 Center => center;
        public Vector2 SizeXZ => sizeXZ;
        public Vector2 HalfSizeXZ => sizeXZ * 0.5f;

        public Vector3 MapNorth
        {
            get
            {
                Vector3 north = orientationReference != null
                    ? orientationReference.forward
                    : Vector3.forward;

                north.y = 0f;
                return north.sqrMagnitude > 0.0001f ? north.normalized : Vector3.forward;
            }
        }

        public Vector3 MapEast
        {
            get
            {
                Vector3 east = orientationReference != null
                    ? orientationReference.right
                    : Vector3.right;

                east.y = 0f;
                return east.sqrMagnitude > 0.0001f ? east.normalized : Vector3.right;
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning($"[MapBoundsManager] Duplicate instance found. Destroying {name}.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public bool IsInsideBounds(Vector3 worldPosition)
        {
            Vector2 local = WorldToMapXZ(worldPosition);
            Vector2 half = HalfSizeXZ;

            return local.x >= -half.x &&
                   local.x <= half.x &&
                   local.y >= -half.y &&
                   local.y <= half.y;
        }

        public Vector2 WorldToMapXZ(Vector3 worldPosition)
        {
            Vector3 delta = worldPosition - center;
            delta.y = 0f;

            float x = Vector3.Dot(delta, MapEast);
            float z = Vector3.Dot(delta, MapNorth);

            return new Vector2(x, z);
        }

        public Vector2 WorldToNormalizedMap(Vector3 worldPosition)
        {
            Vector2 local = WorldToMapXZ(worldPosition);
            Vector2 half = HalfSizeXZ;

            return new Vector2(
                Mathf.InverseLerp(-half.x, half.x, local.x),
                Mathf.InverseLerp(-half.y, half.y, local.y)
            );
        }
        
        public float DistanceToBoundsEdgeXZ(Vector3 worldPosition)
        {
            Vector2 local = WorldToMapXZ(worldPosition);
            Vector2 half = HalfSizeXZ;

            float distanceToEast = half.x - local.x;
            float distanceToWest = local.x + half.x;
            float distanceToNorth = half.y - local.y;
            float distanceToSouth = local.y + half.y;

            return Mathf.Min(distanceToEast, distanceToWest, distanceToNorth, distanceToSouth);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Vector3 north = MapNorth;
            Vector3 east = MapEast;

            Vector2 half = sizeXZ * 0.5f;

            Vector3 a = center + east * -half.x + north * -half.y;
            Vector3 b = center + east * -half.x + north * half.y;
            Vector3 c = center + east * half.x + north * half.y;
            Vector3 d = center + east * half.x + north * -half.y;

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(a, b);
            Gizmos.DrawLine(b, c);
            Gizmos.DrawLine(c, d);
            Gizmos.DrawLine(d, a);

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(center, north * 20f);

            Gizmos.color = Color.red;
            Gizmos.DrawRay(center, east * 20f);
        }
#endif
    }
}
using UnityEngine;

namespace _Scripts.Map
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public sealed class BoundaryGridVisual : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MapBoundsManager boundsManager;

        [Header("Visual Planes")]
        [SerializeField] private Transform northPlane;
        [SerializeField] private Transform southPlane;
        [SerializeField] private Transform eastPlane;
        [SerializeField] private Transform westPlane;

        [Header("Visual Size")]
        [SerializeField] private float visualHeight = 150f;
        [SerializeField] private float verticalCenter = 75f;

        [Header("Plane Mesh Settings")]
        [Tooltip("Use true if the child visual mesh is a Unity Plane facing upward by default. Use false if using a vertical Quad.")]
        [SerializeField] private bool usingUnityPlaneMesh = false;

        [Tooltip("Unity built-in Plane is 10x10 units. Quad is 1x1.")]
        [SerializeField] private float baseMeshSize = 1f;

        private void Reset()
        {
            boundsManager = FindAnyObjectByType<MapBoundsManager>();
        }

        private void OnValidate()
        {
            if (boundsManager == null)
                boundsManager = FindAnyObjectByType<MapBoundsManager>();

            Apply();
        }

        private void Start()
        {
            Apply();
        }
        
        private void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                Apply();
#endif
        }

        public void Apply()
        {
            if (boundsManager == null)
                return;

            Vector3 center = boundsManager.Center;
            Vector2 half = boundsManager.HalfSizeXZ;
            Vector3 north = boundsManager.MapNorth;
            Vector3 east = boundsManager.MapEast;

            PositionSide(
                northPlane,
                center + north * half.y,
                east,
                north,
                boundsManager.SizeXZ.x);

            PositionSide(
                southPlane,
                center - north * half.y,
                -east,
                -north,
                boundsManager.SizeXZ.x);

            PositionSide(
                eastPlane,
                center + east * half.x,
                -north,
                east,
                boundsManager.SizeXZ.y);

            PositionSide(
                westPlane,
                center - east * half.x,
                north,
                -east,
                boundsManager.SizeXZ.y);
        }

        private void PositionSide(
            Transform side,
            Vector3 groundCenter,
            Vector3 horizontalAxis,
            Vector3 outwardNormal,
            float length)
        {
            if (side == null)
                return;

            Vector3 pos = groundCenter;
            pos.y = verticalCenter;
            side.position = pos;

            if (usingUnityPlaneMesh)
            {
                // Unity Plane lies in local X/Z. Rotate it vertical so its surface faces outward.
                side.rotation = Quaternion.LookRotation(outwardNormal, Vector3.up) * Quaternion.Euler(90f, 0f, 0f);
                side.localScale = new Vector3(length / baseMeshSize, visualHeight / baseMeshSize, 1f);
            }
            else
            {
                // Quad lies in local X/Y and faces local +Z.
                side.rotation = Quaternion.LookRotation(outwardNormal, Vector3.up);
                side.localScale = new Vector3(length / baseMeshSize, visualHeight / baseMeshSize, 1f);
            }
        }
        
        public Renderer[] GetRenderers()
        {
            return GetComponentsInChildren<Renderer>(true);
        }

    }
}
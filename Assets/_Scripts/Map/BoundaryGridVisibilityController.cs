using FishNet;
using FishNet.Object;
using UnityEngine;

namespace _Scripts.Map
{
    [DisallowMultipleComponent]
    public sealed class BoundaryGridVisibilityController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MapBoundsManager boundsManager;
        [SerializeField] private BoundaryGridVisual gridVisual;

        [Header("Fade")]
        [SerializeField] private float fadeStartDistance = 80f;
        [SerializeField] private float fullyVisibleDistance = 20f;
        [SerializeField] private float outsideVisibleAlpha = 0.8f;
        [SerializeField] private float maxInsideAlpha = 0.65f;
        [SerializeField] private float fadeSpeed = 8f;

        [Header("Material")]
        [SerializeField] private string alphaProperty = "_BoundaryAlpha";

        private Renderer[] _renderers;
        private MaterialPropertyBlock _mpb;
        private Transform _localPlayer;
        private float _currentAlpha;

        private void Awake()
        {
            if (boundsManager == null)
                boundsManager = FindFirstObjectByType<MapBoundsManager>();

            if (gridVisual == null)
                gridVisual = GetComponent<BoundaryGridVisual>();

            if (gridVisual == null)
                gridVisual = GetComponentInChildren<BoundaryGridVisual>();

            _renderers = gridVisual != null
                ? gridVisual.GetRenderers()
                : GetComponentsInChildren<Renderer>(true);

            _mpb = new MaterialPropertyBlock();

            SetAlphaImmediate(0f);
        }

        private void Update()
        {
            if (boundsManager == null)
                return;

            if (_localPlayer == null)
                TryFindLocalPlayer();

            if (_localPlayer == null)
            {
                FadeTo(0f);
                return;
            }

            float edgeDistance = boundsManager.DistanceToBoundsEdgeXZ(_localPlayer.position);

            float targetAlpha;

            if (edgeDistance < 0f)
            {
                // Player is outside. Boundary should be clearly visible.
                targetAlpha = outsideVisibleAlpha;
            }
            else if (edgeDistance >= fadeStartDistance)
            {
                // Far from edge. Invisible.
                targetAlpha = 0f;
            }
            else
            {
                // Fade in as player approaches edge.
                float t = Mathf.InverseLerp(fadeStartDistance, fullyVisibleDistance, edgeDistance);
                targetAlpha = Mathf.Lerp(0f, maxInsideAlpha, t);
            }

            FadeTo(targetAlpha);
        }

        private void TryFindLocalPlayer()
        {
            if (InstanceFinder.ClientManager == null)
                return;

            NetworkObject localPlayerObject = InstanceFinder.ClientManager.Connection?.FirstObject;
            if (localPlayerObject == null)
                return;

            _localPlayer = localPlayerObject.transform;
        }

        private void FadeTo(float targetAlpha)
        {
            _currentAlpha = Mathf.MoveTowards(
                _currentAlpha,
                targetAlpha,
                fadeSpeed * Time.deltaTime);

            ApplyAlpha(_currentAlpha);
        }

        private void SetAlphaImmediate(float alpha)
        {
            _currentAlpha = alpha;
            ApplyAlpha(_currentAlpha);
        }

        private void ApplyAlpha(float alpha)
        {
            if (_renderers == null)
                return;

            for (int i = 0; i < _renderers.Length; i++)
            {
                Renderer r = _renderers[i];
                if (r == null)
                    continue;

                r.GetPropertyBlock(_mpb);

                Material shared = r.sharedMaterial;
                if (shared != null && shared.HasProperty(alphaProperty))
                {
                    _mpb.SetFloat(alphaProperty, alpha);
                }
                else
                {
                    // Fallback for basic URP materials.
                    Color color = Color.white;

                    if (shared != null && shared.HasProperty("_BaseColor"))
                        color = shared.GetColor("_BaseColor");
                    else if (shared != null && shared.HasProperty("_Color"))
                        color = shared.GetColor("_Color");

                    color.a = alpha;

                    if (shared != null && shared.HasProperty("_BaseColor"))
                        _mpb.SetColor("_BaseColor", color);
                    else if (shared != null && shared.HasProperty("_Color"))
                        _mpb.SetColor("_Color", color);
                }

                r.SetPropertyBlock(_mpb);
            }
        }
    }
}
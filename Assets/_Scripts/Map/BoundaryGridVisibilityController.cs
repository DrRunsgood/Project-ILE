using FishNet;
using FishNet.Object;
using UnityEngine;
using _Scripts.Player;

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

        [SerializeField] private float localPlayerSearchInterval = 0.25f;
        
        [Header("Material")]
        [SerializeField] private string alphaProperty = "_BoundaryAlpha";

        private Renderer[] _renderers;
        private MaterialPropertyBlock _mpb;
        private Transform _localPlayer;
        private float _currentAlpha;
        private float _nextLocalPlayerSearchTime;

        private void Awake()
        {
            if (boundsManager == null)
                boundsManager = FindAnyObjectByType<MapBoundsManager>();

            if (_mpb == null)
                _mpb = new MaterialPropertyBlock();

            RefreshRenderers();
            SetAlphaImmediate(0f);
        }
        
        private void Start()
        {
            if (InstanceFinder.IsServerStarted && !InstanceFinder.IsClientStarted)
            {
                enabled = false;
                return;
            }
        }

        private void OnEnable()
        {
            if (boundsManager == null)
                boundsManager = FindAnyObjectByType<MapBoundsManager>();

            RefreshRenderers();
            
            _localPlayer = null;
            SetAlphaImmediate(0f);
        }

        private void Update()
        {
            if (InstanceFinder.IsServerStarted && !InstanceFinder.IsClientStarted)
                return;
            
            if (boundsManager == null)
                boundsManager = FindAnyObjectByType<MapBoundsManager>();

            if (_renderers == null || _renderers.Length == 0)
                RefreshRenderers();

            if (boundsManager == null)
            {
                FadeTo(0f);
                return;
            }

            if (_localPlayer == null || !_localPlayer.gameObject.activeInHierarchy)
            {
                if (Time.unscaledTime >= _nextLocalPlayerSearchTime)
                {
                    _nextLocalPlayerSearchTime = Time.unscaledTime + localPlayerSearchInterval;
                    TryFindLocalPlayer();
                }
            }

            if (_localPlayer == null)
            {
                FadeTo(0f);
                return;
            }

            float edgeDistance = boundsManager.DistanceToBoundsEdgeXZ(_localPlayer.position);

            float targetAlpha;

            if (edgeDistance < 0f)
            {
                targetAlpha = outsideVisibleAlpha;
            }
            else if (edgeDistance >= fadeStartDistance)
            {
                targetAlpha = 0f;
            }
            else
            {
                float t = Mathf.InverseLerp(fadeStartDistance, fullyVisibleDistance, edgeDistance);
                targetAlpha = Mathf.Lerp(0f, maxInsideAlpha, t);
            }
            
            FadeTo(targetAlpha);
        }

        private void TryFindLocalPlayer()
        {
            _localPlayer = null;

            if (LocalPlayerContext.IsReady && LocalPlayerContext.Controller != null)
            {
                _localPlayer = LocalPlayerContext.Controller.transform;

                return;
            }

            PlayerIdentity[] identities = FindObjectsByType<PlayerIdentity>(FindObjectsInactive.Exclude);

            foreach (var identity in identities)
            {
                if (identity == null)
                    continue;
                
                if (!identity.IsOwner)
                    continue;

                _localPlayer = identity.transform;

                return;
            }
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
        
        private void RefreshRenderers()
        {
            if (gridVisual == null)
                gridVisual = GetComponent<BoundaryGridVisual>();

            if (gridVisual == null)
                gridVisual = GetComponentInChildren<BoundaryGridVisual>();

            _renderers = gridVisual != null
                ? gridVisual.GetRenderers()
                : GetComponentsInChildren<Renderer>(true);
        }
    }
}
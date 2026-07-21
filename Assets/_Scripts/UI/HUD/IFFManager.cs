using System.Collections.Generic;
using UnityEngine;
using _Scripts.Game.Teams;
using _Scripts.Player;

public sealed class IFFManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private IFFWidget widgetPrefab;
    [SerializeField] private RectTransform root;

    [Header("Focused Target Panel")]
    [SerializeField] private FocusedIFFPanel focusedPanel;

    [Header("Distances")]
    [SerializeField] private float teammateMaxDistance = 250f;
    [SerializeField] private float enemyMaxDistance = 100f;

    [Header("Focus")]
    [SerializeField] private float focusScreenRadius = 60f;

    [Header("LOS")]
    [SerializeField] private LayerMask losMask = ~0;
    [SerializeField] [Min(0.01f)] private float losCheckInterval = 0.1f;
    [SerializeField] [Min(4)] private int losHitBufferSize = 32;

    private RaycastHit[] _losHitBuffer;

    [Header("Colors")]
    [SerializeField] private Color teammateColor = Color.cyan;
    [SerializeField] private Color enemyColor = Color.red;

    private readonly Dictionary<PlayerIFFTarget, IFFWidget> _widgets = new();
    private readonly Dictionary<PlayerIFFTarget, float> _nextLosCheck = new();
    private readonly Dictionary<PlayerIFFTarget, bool> _losVisible = new();
    private readonly List<PlayerIFFTarget> _staleTargets = new();

    private PlayerIdentity _localIdentity;
    private Transform _localTransform;

    private void Awake()
    {
        if (!root)
            root = (RectTransform)transform;
        
        _losHitBuffer = new RaycastHit[Mathf.Max(4, losHitBufferSize)];

        LocalPlayerContext.OnLocalPlayerReady += HandleLocalPlayerReady;
        LocalPlayerContext.OnLocalPlayerCleared += HandleLocalPlayerCleared;
    }

    private void OnEnable()
    {
        TryBindExistingLocalPlayer();

        if (!targetCamera)
            targetCamera = Camera.main;

        if (focusedPanel != null)
            focusedPanel.SetVisible(false);
    }

    private void OnDisable()
    {
        if (focusedPanel != null)
            focusedPanel.SetVisible(false);

        ClearWidgetState();
    }

    private void OnDestroy()
    {
        LocalPlayerContext.OnLocalPlayerReady -= HandleLocalPlayerReady;
        LocalPlayerContext.OnLocalPlayerCleared -= HandleLocalPlayerCleared;
    }

    private void LateUpdate()
    {
        CleanupStaleWidgets();

        if (!_localIdentity || !_localTransform)
            TryBindExistingLocalPlayer();

        if (!_localIdentity || !_localTransform || !widgetPrefab || !root)
        {
            HideFocusedPanel();
            return;
        }

        if (!targetCamera || !targetCamera.isActiveAndEnabled)
        {
            targetCamera = Camera.main;
        }

        /*
         * Camera.main is expected to be null on a dedicated server
         * and may briefly be null during client presentation binding.
         */
        if (!targetCamera || !targetCamera.isActiveAndEnabled)
        {
            HideFocusedPanel();
            return;
        }

        Transform cameraTransform = targetCamera.transform;

        Vector3 cameraPosition = cameraTransform.position;

        Vector2 screenCenter =
            new Vector2(
                Screen.width * 0.5f,
                Screen.height * 0.5f);

        if (!_localIdentity || !_localTransform || !targetCamera || !widgetPrefab || !root)
        {
            if (focusedPanel != null)
                focusedPanel.SetVisible(false);

            return;
        }

        PlayerIFFTarget focusedTarget = null;
        float focusedTargetScore = float.MaxValue;
        Color focusedTargetColor = Color.white;
        float focusedTargetHealth01 = 1f;
        string focusedTargetName = string.Empty;
        
        IReadOnlyList<PlayerIFFTarget> targets = PlayerIFFTarget.ActiveTargets;

        for (int i = 0; i < targets.Count; i++)
        {
            PlayerIFFTarget target = targets[i];

            if (!target || !target.Identity || target.Identity == _localIdentity)
                continue;

            bool visible = ShouldShow(target, cameraPosition, screenCenter, out bool focused, out float alpha, out Color color, out Vector3 screenPos);

            if (!visible)
            {
                if (_widgets.TryGetValue(target, out IFFWidget existingWidget) && existingWidget)
                    existingWidget.SetVisible(false);

                continue;
            }

            IFFWidget widget = GetWidget(target);
            widget.SetScreenPosition(ScreenToRootPosition(screenPos));
            widget.SetData(color, alpha);

            if (!focused)
                continue;

            Vector2 screenPoint = new Vector2(screenPos.x, screenPos.y);
            float score = Vector2.Distance(screenPoint, screenCenter);

            if (score >= focusedTargetScore)
                continue;

            focusedTargetScore = score;
            focusedTarget = target;
            focusedTargetColor = color;
            focusedTargetHealth01 = GetHealth01(target);
            focusedTargetName = GetDisplayName(target);
        }

        if (focusedPanel == null)
            return;

        if (focusedTarget != null)
            focusedPanel.SetData(focusedTargetName, focusedTargetHealth01, focusedTargetColor);
        else
            focusedPanel.SetVisible(false);
    }
    
    private void HideFocusedPanel()
    {
        if (focusedPanel != null)
            focusedPanel.SetVisible(false);
    }

    private void HandleLocalPlayerReady(AdvancedPredictedController controller)
    {
        if (controller == null)
            return;

        PlayerIdentity identity = controller.GetComponent<PlayerIdentity>();
        if (identity == null)
            return;

        bool changed =
            _localIdentity != identity ||
            _localTransform != controller.transform;

        _localIdentity = identity;
        _localTransform = controller.transform;

        targetCamera = Camera.main;

        if (changed)
            ClearWidgetState();
    }

    private void HandleLocalPlayerCleared()
    {
        _localIdentity = null;
        _localTransform = null;

        ClearWidgetState();

        if (focusedPanel != null)
            focusedPanel.SetVisible(false);
    }

    private void TryBindExistingLocalPlayer()
    {
        if (LocalPlayerContext.IsReady && LocalPlayerContext.Controller != null)
            HandleLocalPlayerReady(LocalPlayerContext.Controller);
    }

    private IFFWidget GetWidget(PlayerIFFTarget target)
    {
        if (_widgets.TryGetValue(target, out IFFWidget existing) && existing)
            return existing;

        IFFWidget created = Instantiate(widgetPrefab, root);
        _widgets[target] = created;
        return created;
    }

    private bool ShouldShow(PlayerIFFTarget target, Vector3 cameraPosition, Vector2 screenCenter, out bool focused, out float alpha,
        out Color color, out Vector3 screenPos)    
    {
        focused = false;
        alpha = 0f;
        color = Color.white;
        screenPos = Vector3.zero;

        if (target == null || target.Identity == null)
            return false;

        if (!target.CanShowIFF)
        {
            _nextLosCheck.Remove(target);
            _losVisible.Remove(target);

            return false;
        }
        
        Transform anchor = target.Anchor;
        if (anchor == null)
            return false;

        Vector3 worldPos = anchor.position;

        bool teammate = _localIdentity.Team != TeamId.None && target.Identity.Team != TeamId.None &&
                        target.Identity.Team == _localIdentity.Team;

        float maxDistance = teammate ? teammateMaxDistance : enemyMaxDistance;
        
        Vector3 toTarget = worldPos - cameraPosition;

        float distanceSqr = toTarget.sqrMagnitude;

        float maxDistanceSqr = maxDistance * maxDistance;

        if (distanceSqr > maxDistanceSqr)
            return false;
        
        float distance = Mathf.Sqrt(distanceSqr);
        
        screenPos = targetCamera.WorldToScreenPoint(worldPos);

        if (screenPos.z <= 0f)
            return false;

        if (!HasLineOfSight(target, cameraPosition, worldPos, distance))
            return false;
        
        Vector2 screenPoint = new Vector2(screenPos.x, screenPos.y);
        float screenDist = Vector2.Distance(screenPoint, screenCenter);

        focused = screenDist <= focusScreenRadius;

        alpha = Mathf.Clamp01(1f - distance / maxDistance);
        alpha = Mathf.Lerp(0.25f, 1f, alpha);

        color = teammate ? teammateColor : enemyColor;

        return true;
    }

    private bool HasLineOfSight(PlayerIFFTarget target, Vector3 origin, Vector3 worldPosition, float distance)
    {
        if (_nextLosCheck.TryGetValue(target, out float nextCheck) && Time.time < nextCheck)
        {
            return _losVisible.TryGetValue(target, out bool cachedVisible) && cachedVisible;
        }

        /*
         * A small deterministic offset prevents every target from
         * settling onto exactly the same recurring check frame.
         */
        float stagger = Mathf.Abs(target.GetInstanceID() % 5) * 0.01f;

        _nextLosCheck[target] = Time.time + losCheckInterval + stagger;

        Vector3 offset = worldPosition - origin;

        if (offset.sqrMagnitude <= 0.000001f)
        {
            _losVisible[target] = true;
            return true;
        }

        Vector3 direction = offset / distance;

        int hitCount = Physics.RaycastNonAlloc(origin, direction, _losHitBuffer, distance, losMask, QueryTriggerInteraction.Ignore);

        bool visible;

        if (hitCount < _losHitBuffer.Length)
        {
            visible = EvaluateNearestRelevantHit(target, _losHitBuffer, hitCount);
        }
        else
        {
            /*
             * Rare correctness fallback. The common path remains
             * allocation-free, but an unusually crowded ray cannot
             * silently truncate a nearer blocker.
             */
            RaycastHit[] overflowHits = Physics.RaycastAll(origin, direction, distance, losMask, QueryTriggerInteraction.Ignore);

            visible = EvaluateNearestRelevantHit(target, overflowHits, overflowHits.Length);
        }

        _losVisible[target] = visible;

        return visible;
    }
    
    private bool EvaluateNearestRelevantHit(PlayerIFFTarget target, RaycastHit[] hits, int hitCount)
    {
        float nearestDistance = float.PositiveInfinity;

        Transform nearestRoot = null;

        Transform localRoot = _localTransform != null ? _localTransform.root : null;

        Transform targetRoot = target.transform.root;
        
        for (int i = 0; i < hitCount; i++)
        {
            Collider hitCollider = hits[i].collider;

            if (hitCollider == null)
                continue;

            Transform hitRoot = hitCollider.transform.root;

            /*
             * Ignore the observing player's own colliders.
             */
            if (localRoot != null && hitRoot == localRoot)
                continue;
            

            float hitDistance = hits[i].distance;

            if (hitDistance >= nearestDistance)
                continue;

            nearestDistance = hitDistance;

            nearestRoot = hitRoot;
        }

        /*
         * No relevant collision means unobstructed.
         * Otherwise the nearest object must be the target player.
         */
        return nearestRoot == null || nearestRoot == targetRoot;
    }

    private float GetHealth01(PlayerIFFTarget target)
    {
        if (target == null || target.Health == null || target.Health.Max <= 0)
            return 1f;

        return Mathf.Clamp01(target.Health.Current / (float)target.Health.Max);
    }

    private string GetDisplayName(PlayerIFFTarget target)
    {
        if (target == null || target.Identity == null)
            return "Player";

        string displayName = target.Identity.DisplayName;

        return string.IsNullOrWhiteSpace(displayName)
            ? "Player"
            : displayName;
    }

    private Vector2 ScreenToRootPosition(Vector3 screenPos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            root,
            screenPos,
            null,
            out Vector2 localPoint);

        return localPoint;
    }

    private void CleanupStaleWidgets()
    {
        _staleTargets.Clear();

        foreach (KeyValuePair<PlayerIFFTarget, IFFWidget> kvp in _widgets)
        {
            PlayerIFFTarget target = kvp.Key;
            IFFWidget widget = kvp.Value;

            if (target == null || !target || !target.gameObject.activeInHierarchy)
            {
                if (widget != null)
                    Destroy(widget.gameObject);

                _staleTargets.Add(target);
            }
        }

        for (int i = 0; i < _staleTargets.Count; i++)
        {
            PlayerIFFTarget target = _staleTargets[i];

            _widgets.Remove(target);
            _nextLosCheck.Remove(target);
            _losVisible.Remove(target);
        }
    }

    private void ClearWidgetState()
    {
        foreach (IFFWidget widget in _widgets.Values)
        {
            if (widget != null)
                Destroy(widget.gameObject);
        }

        _widgets.Clear();
        _nextLosCheck.Clear();
        _losVisible.Clear();
        _staleTargets.Clear();
    }
}
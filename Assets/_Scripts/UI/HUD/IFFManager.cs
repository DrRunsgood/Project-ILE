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
    [SerializeField] private float losCheckInterval = 0.1f;

    [Header("Colors")]
    [SerializeField] private Color teammateColor = Color.cyan;
    [SerializeField] private Color enemyColor = Color.red;

    private float _nextFocusedHealthDebugTime;

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

        if (!targetCamera || !targetCamera.gameObject.activeInHierarchy)
            targetCamera = Camera.main;

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

        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

        PlayerIFFTarget[] targets = FindObjectsByType<PlayerIFFTarget>(FindObjectsInactive.Exclude);

        for (int i = 0; i < targets.Length; i++)
        {
            PlayerIFFTarget target = targets[i];

            if (!target || !target.Identity || target.Identity == _localIdentity)
                continue;

            bool visible = ShouldShow(
                target,
                out bool focused,
                out float alpha,
                out Color color,
                out Vector3 screenPos);

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

    private bool ShouldShow(
        PlayerIFFTarget target,
        out bool focused,
        out float alpha,
        out Color color,
        out Vector3 screenPos)
    {
        focused = false;
        alpha = 0f;
        color = Color.white;
        screenPos = Vector3.zero;

        if (target == null || target.Identity == null)
            return false;

        if (target.Health != null && target.Health.IsDead)
            return false;

        Transform anchor = target.Anchor;
        if (anchor == null)
            return false;

        Vector3 worldPos = anchor.position;

        Vector3 toTarget = worldPos - targetCamera.transform.position;
        float distance = toTarget.magnitude;

        bool teammate =
            _localIdentity.Team != TeamId.None &&
            target.Identity.Team != TeamId.None &&
            target.Identity.Team == _localIdentity.Team;

        float maxDistance = teammate ? teammateMaxDistance : enemyMaxDistance;

        if (distance > maxDistance)
            return false;

        screenPos = targetCamera.WorldToScreenPoint(worldPos);

        if (screenPos.z <= 0f)
            return false;

        if (!HasLineOfSight(target, worldPos, distance))
            return false;

        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector2 screenPoint = new Vector2(screenPos.x, screenPos.y);
        float screenDist = Vector2.Distance(screenPoint, screenCenter);

        focused = screenDist <= focusScreenRadius;

        alpha = Mathf.Clamp01(1f - distance / maxDistance);
        alpha = Mathf.Lerp(0.25f, 1f, alpha);

        color = teammate ? teammateColor : enemyColor;

        return true;
    }

    private bool HasLineOfSight(PlayerIFFTarget target, Vector3 worldPos, float distance)
    {
        if (!_nextLosCheck.TryGetValue(target, out float next) || Time.time >= next)
        {
            _nextLosCheck[target] = Time.time + losCheckInterval;

            Vector3 origin = targetCamera.transform.position;
            Vector3 dir = (worldPos - origin).normalized;

            bool visible = true;

            RaycastHit[] hits = Physics.RaycastAll(
                origin,
                dir,
                distance,
                losMask,
                QueryTriggerInteraction.Ignore);

            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                Transform hitRoot = hit.collider.transform.root;

                if (_localTransform != null && hitRoot == _localTransform.root)
                    continue;

                if (hitRoot == target.transform.root)
                {
                    visible = true;
                    break;
                }

                visible = false;
                break;
            }

            _losVisible[target] = visible;
        }

        return _losVisible.TryGetValue(target, out bool result) && result;
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
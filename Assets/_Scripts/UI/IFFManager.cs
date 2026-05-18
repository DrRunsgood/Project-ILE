using System.Collections.Generic;
using UnityEngine;
using _Scripts.Game.Teams;
using _Scripts.Player;

public sealed class IFFManager : MonoBehaviour
{
    [SerializeField] Camera targetCamera;
    [SerializeField] IFFWidget widgetPrefab;
    [SerializeField] RectTransform root;

    [Header("Distances")]
    [SerializeField] float teammateMaxDistance = 250f;
    [SerializeField] float enemyMaxDistance = 100f;

    [Header("Focus")]
    [SerializeField] float focusScreenRadius = 60f;

    [Header("LOS")]
    [SerializeField] LayerMask losMask = ~0;
    [SerializeField] float losCheckInterval = 0.1f;

    [Header("Colors")]
    [SerializeField] Color teammateColor = Color.cyan;
    [SerializeField] Color enemyColor = Color.red;

    readonly Dictionary<PlayerIFFTarget, IFFWidget> _widgets = new();
    readonly Dictionary<PlayerIFFTarget, float> _nextLosCheck = new();
    readonly Dictionary<PlayerIFFTarget, bool> _losVisible = new();

    PlayerIdentity _localIdentity;
    Transform _localTransform;

    void Awake()
    {
        if (!root)
            root = (RectTransform)transform;

        LocalPlayerContext.OnLocalPlayerReady += HandleLocalPlayerReady;
        LocalPlayerContext.OnLocalPlayerCleared += HandleLocalPlayerCleared;
    }

    void OnDestroy()
    {
        LocalPlayerContext.OnLocalPlayerReady -= HandleLocalPlayerReady;
        LocalPlayerContext.OnLocalPlayerCleared -= HandleLocalPlayerCleared;
    }

    void HandleLocalPlayerReady(AdvancedPredictedController controller)
    {
        _localIdentity = controller.GetComponent<PlayerIdentity>();
        _localTransform = controller.transform;

        if (!targetCamera)
            targetCamera = Camera.main;
    }

    void HandleLocalPlayerCleared()
    {
        _localIdentity = null;
        _localTransform = null;

        foreach (IFFWidget w in _widgets.Values)
            if (w) w.SetVisible(false);
    }

    void LateUpdate()
    {
        if (!_localIdentity || !_localTransform || !targetCamera || !widgetPrefab)
            return;

        PlayerIFFTarget[] targets = FindObjectsByType<PlayerIFFTarget>(FindObjectsSortMode.None);

        foreach (PlayerIFFTarget target in targets)
        {
            if (!target || !target.Identity || target.Identity == _localIdentity)
                continue;

            IFFWidget widget = GetWidget(target);

            bool visible = ShouldShow(target, out bool focused, out float alpha, out Color color, out Vector3 screenPos);

            if (!visible)
            {
                widget.SetVisible(false);
                continue;
            }

            float hp01 = target.Health != null && target.Health.Max > 0
                ? target.Health.Current / (float)target.Health.Max
                : 1f;

            widget.SetScreenPosition(ScreenToRootPosition(screenPos));
            widget.SetData(target.Identity.DisplayName, hp01, color, focused, alpha);
        }
    }

    IFFWidget GetWidget(PlayerIFFTarget target)
    {
        if (_widgets.TryGetValue(target, out IFFWidget existing) && existing)
            return existing;

        IFFWidget created = Instantiate(widgetPrefab, root);
        _widgets[target] = created;
        return created;
    }

    bool ShouldShow(
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

        if (target.Health != null && target.Health.IsDead)
            return false;

        Transform anchor = target.Anchor;
        Vector3 worldPos = anchor.position;

        Vector3 toTarget = worldPos - targetCamera.transform.position;
        float distance = toTarget.magnitude;

        bool teammate = target.Identity.Team == _localIdentity.Team;
        float maxDistance = teammate ? teammateMaxDistance : enemyMaxDistance;

        if (distance > maxDistance)
            return false;

        screenPos = targetCamera.WorldToScreenPoint(worldPos);

        if (screenPos.z <= 0f)
            return false;

        if (!HasLineOfSight(target, worldPos, distance))
            return false;

        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        float screenDist = Vector2.Distance(new Vector2(screenPos.x, screenPos.y), screenCenter);
        focused = screenDist <= focusScreenRadius;

        alpha = Mathf.Clamp01(1f - distance / maxDistance);
        alpha = Mathf.Lerp(0.25f, 1f, alpha);

        color = teammate ? teammateColor : enemyColor;

        return true;
    }

    bool HasLineOfSight(PlayerIFFTarget target, Vector3 worldPos, float distance)
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
                QueryTriggerInteraction.Ignore
            );

            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (RaycastHit hit in hits)
            {
                Transform hitRoot = hit.collider.transform.root;

                // Ignore local player/self
                if (_localTransform != null && hitRoot == _localTransform.root)
                    continue;

                // Ignore target player
                if (hitRoot == target.transform.root)
                {
                    visible = true;
                    break;
                }

                // Anything else in LOS mask blocks.
                visible = false;
                break;
            }

            _losVisible[target] = visible;
        }

        return _losVisible.TryGetValue(target, out bool result) && result;
    }

    Vector2 ScreenToRootPosition(Vector3 screenPos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            root,
            screenPos,
            null,
            out Vector2 localPoint);

        return localPoint;
    }
}
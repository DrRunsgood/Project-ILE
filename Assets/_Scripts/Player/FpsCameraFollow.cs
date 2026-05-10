using UnityEngine;
using _Scripts.Player;

[DisallowMultipleComponent]
public class FpsCameraFollow : MonoBehaviour
{
    AdvancedPredictedController target;
    Transform head;
    Camera cam;
    InputHandler ih;

    [Header("Hierarchy")] [SerializeField] Transform effectsRoot;

    [Header("Smoothing (owner)")] [SerializeField]
    float yawSmoothTime = 0.008f;

    [SerializeField] float pitchSmoothTime = 0.008f;
    float _renderYaw, _renderYawVel;
    float _renderPitch, _renderPitchVel;

    [Header("Third-Person Offset")] [SerializeField]
    float back = 10f;

    [SerializeField] float up = 2f;

    const string FP_LAYER = "FP_Only";
    const string TP_LAYER = "TP_Only";
    int _fpMask, _tpMask;
    bool inThirdPerson;

    void Awake()
    {
        cam = GetComponentInChildren<Camera>(true);
        if (!cam)
        {
            Debug.LogError("FpsCameraFollow: No child Camera found under CameraGimbal.");
            enabled = false;
            return;
        }

        int fpIdx = LayerMask.NameToLayer(FP_LAYER);
        int tpIdx = LayerMask.NameToLayer(TP_LAYER);
        if (fpIdx < 0 || tpIdx < 0)
            Debug.LogWarning($"FpsCameraFollow: Missing layer(s). Ensure layers '{FP_LAYER}' and '{TP_LAYER}' exist.");

        _fpMask = (fpIdx >= 0) ? (1 << fpIdx) : 0;
        _tpMask = (tpIdx >= 0) ? (1 << tpIdx) : 0;

        if (effectsRoot && cam.transform.parent != effectsRoot)
            cam.transform.SetParent(effectsRoot, worldPositionStays: false);

        if (effectsRoot)
        {
            effectsRoot.localPosition = Vector3.zero;
            effectsRoot.localRotation = Quaternion.identity;
        }

        cam.transform.localPosition = Vector3.zero;
        cam.transform.localRotation = Quaternion.identity;

        LocalPlayerContext.OnLocalPlayerReady += HandleLocalPlayerReady;
        LocalPlayerContext.OnLocalPlayerCleared += HandleLocalPlayerCleared;

        if (LocalPlayerContext.IsReady)
            HandleLocalPlayerReady(LocalPlayerContext.Controller);
    }

    void OnEnable()
    {
        if (cam)
            ApplyCulling(inThirdPerson);
    }

    void OnDestroy()
    {
        LocalPlayerContext.OnLocalPlayerReady -= HandleLocalPlayerReady;
        LocalPlayerContext.OnLocalPlayerCleared -= HandleLocalPlayerCleared;
    }

    void HandleLocalPlayerReady(AdvancedPredictedController controller)
    {
        SetTarget(controller);
        Debug.Log($"[FpsCameraFollow] Bound to local player: {controller.name}");
    }

    void HandleLocalPlayerCleared()
    {
        if (target != null)
            target.SetMainBodyVisibility(true);

        target = null;
        head = null;
        ih = null;
    }

    public void SetTarget(AdvancedPredictedController t)
    {
        if (target != null && target != t)
            target.SetMainBodyVisibility(true);

        target = t;
        head = t ? t.HeadAnchor : null;
        ih = t ? t.GetComponent<InputHandler>() : null;

        if (target == null)
        {
            ApplyCulling(false);
            return;
        }

        _renderYaw = target.transform.eulerAngles.y;
        _renderPitch = target.CurrentPitch;

        inThirdPerson = false;
        ApplyCulling(inThirdPerson);

        if (head)
        {
            transform.position = head.position;
            transform.rotation = Quaternion.Euler(_renderPitch, _renderYaw, 0f);
        }
    }

    void Update()
    {
        if (ih != null && ih.ConsumeViewToggle())
        {
            inThirdPerson = !inThirdPerson;
            UpdateView(inThirdPerson);
        }
    }

    
    void LateUpdate()
    {
        if (!target || !head)
            return;

        float targetYaw = target.transform.eulerAngles.y;
        float targetPitch = target.CurrentPitch;

        _renderYaw = (yawSmoothTime > 0f)
            ? Mathf.SmoothDampAngle(_renderYaw, targetYaw, ref _renderYawVel, yawSmoothTime)
            : targetYaw;

        _renderPitch = (pitchSmoothTime > 0f)
            ? Mathf.SmoothDampAngle(_renderPitch, targetPitch, ref _renderPitchVel, pitchSmoothTime)
            : targetPitch;

        transform.rotation = Quaternion.Euler(_renderPitch, _renderYaw, 0f);

        transform.position = inThirdPerson
            ? head.position - transform.forward * back + Vector3.up * up
            : head.position;
    }
    void UpdateView(bool isThirdPerson)
    {
        ApplyCulling(isThirdPerson);

        if (target != null)
            target.SetMainBodyVisibility(isThirdPerson);
    }

    void ApplyCulling(bool tpMode)
    {
        cam.cullingMask = tpMode
            ? (cam.cullingMask | _tpMask) & ~_fpMask
            : (cam.cullingMask | _fpMask) & ~_tpMask;
    }
}
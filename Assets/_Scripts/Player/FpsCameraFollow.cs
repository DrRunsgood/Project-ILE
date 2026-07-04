using UnityEngine;
using _Scripts.Player;
using _Scripts.Weapons;

[DisallowMultipleComponent]
public class FpsCameraFollow : MonoBehaviour
{
    AdvancedPredictedController target;
    Transform followTarget;
    Camera cam;
    InputHandler ih;
    
    [Header("Zoom")]
    [SerializeField] bool enableZoom = true;
    [SerializeField] float normalFov = 65f;
    [SerializeField] float zoomFov = 30f;
    [SerializeField] float zoomInTime = 0.08f;
    [SerializeField] float zoomOutTime = 0.08f;
    [SerializeField] float zoomSensitivityMultiplier = 0.45f;

    float _fovVelocity;
    bool _zoomHeld;
    bool _zoomAllowed = true;

    public bool IsZoomed => enableZoom && _zoomAllowed && _zoomHeld && !inThirdPerson;
    public float CurrentLookSensitivityMultiplier => IsZoomed ? zoomSensitivityMultiplier : 1f;

    [Header("Hierarchy")]
    [SerializeField] Transform effectsRoot;

    [Header("Position Smoothing")]
    [Tooltip("Enable camera-side render smoothing. This is separate from FishNet tick smoothing.")]
    [SerializeField] bool smoothPosition = true;

    [Tooltip("Start around 0.015–0.035. Lower = tighter, higher = smoother but laggier.")]
    [SerializeField] float positionSmoothTime = 0.025f;

    [Tooltip("Max camera catch-up speed. 0 or less means unlimited.")]
    [SerializeField] float maxPositionSpeed = 0f;

    Vector3 _positionVelocity;

    [Header("Rotation")]
    [Tooltip("Usually keep false for competitive FPS. Position can smooth; aim should stay responsive.")]
    [SerializeField] bool smoothRotation = false;

    [SerializeField] float yawSmoothTime = 0.008f;
    [SerializeField] float pitchSmoothTime = 0.008f;

    float _renderYaw;
    float _renderYawVel;
    float _renderPitch;
    float _renderPitchVel;

    [Header("Third-Person Offset")]
    [SerializeField] float back = 10f;
    [SerializeField] float up = 2f;

    const string FP_LAYER = "FP_Only";
    const string TP_LAYER = "TP_Only";

    int _fpMask;
    int _tpMask;

    bool inThirdPerson;
    bool _targetAlive = true;

    void Awake()
    {
        cam = GetComponentInChildren<Camera>(true);

        if (!cam)
        {
            Debug.LogError("FpsCameraFollow: No child Camera found under CameraGimbal.");
            enabled = false;
            return;
        }
        
        normalFov = cam.fieldOfView;
        cam.fieldOfView = normalFov;

        int fpIdx = LayerMask.NameToLayer(FP_LAYER);
        int tpIdx = LayerMask.NameToLayer(TP_LAYER);

        if (fpIdx < 0 || tpIdx < 0)
            Debug.LogWarning($"FpsCameraFollow: Missing layer(s). Ensure layers '{FP_LAYER}' and '{TP_LAYER}' exist.");

        _fpMask = fpIdx >= 0 ? 1 << fpIdx : 0;
        _tpMask = tpIdx >= 0 ? 1 << tpIdx : 0;

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
        
        _zoomHeld = false;
        _zoomAllowed = false;
        _fovVelocity = 0f;

        if (cam)
            cam.fieldOfView = normalFov;
        
        Debug.Log($"[FpsCameraFollow] Bound to local player: {controller.name}");
    }

    void HandleLocalPlayerCleared()
    {
        if (target != null)
            target.SetMainBodyVisibility(true);

        target = null;
        followTarget = null;
        ih = null;
        _positionVelocity = Vector3.zero;
    }

    public void SetTarget(AdvancedPredictedController t)
    {
        if (target != null && target != t)
            target.SetMainBodyVisibility(true);

        target = t;
        followTarget = t ? t.CameraFollowTarget : null;
        ih = t ? t.GetComponent<InputHandler>() : null;

        _positionVelocity = Vector3.zero;

        if (target == null)
        {
            ApplyCulling(false);
            return;
        }

        _renderYaw = target.transform.eulerAngles.y;
        _renderPitch = target.CurrentPitch;
        _renderYawVel = 0f;
        _renderPitchVel = 0f;
        
        _zoomHeld = false;
        _zoomAllowed = true;
        _fovVelocity = 0f;

        if (cam)
            cam.fieldOfView = normalFov;

        inThirdPerson = false;
        ApplyCulling(inThirdPerson);

        if (followTarget)
        {
            transform.position = GetDesiredPosition();
            transform.rotation = GetDesiredRotation();
        }

        SetupFirstPersonWeaponAnchor();
    }

    void Update()
    {
        if (ih != null && ih.ConsumeViewToggle())
        {
            inThirdPerson = !inThirdPerson;
            UpdateView(inThirdPerson);

            // Avoid damping from the old view position into the new mode.
            _positionVelocity = Vector3.zero;

            if (followTarget)
                transform.position = GetDesiredPosition();
        }
    }

    void LateUpdate()
    {
        UpdateZoomFov();
        
        if (!target || !followTarget || !_targetAlive)
            return;

        Quaternion desiredRot = GetDesiredRotation();
        transform.rotation = desiredRot;

        Vector3 desiredPos = GetDesiredPosition();

        if (!smoothPosition || positionSmoothTime <= 0f)
        {
            transform.position = desiredPos;
            return;
        }

        float maxSpeed = maxPositionSpeed > 0f ? maxPositionSpeed : Mathf.Infinity;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPos,
            ref _positionVelocity,
            positionSmoothTime,
            maxSpeed,
            Time.deltaTime);
    }

    Vector3 GetDesiredPosition()
    {
        if (!followTarget)
            return transform.position;

        if (inThirdPerson)
            return followTarget.position - transform.forward * back + Vector3.up * up;

        return followTarget.position;
    }

    Quaternion GetDesiredRotation()
    {
        if (!target)
            return transform.rotation;

        float targetYaw = target.transform.eulerAngles.y;
        float targetPitch = target.CurrentPitch;

        if (!smoothRotation)
            return Quaternion.Euler(targetPitch, targetYaw, 0f);

        _renderYaw = yawSmoothTime > 0f
            ? Mathf.SmoothDampAngle(_renderYaw, targetYaw, ref _renderYawVel, yawSmoothTime)
            : targetYaw;

        _renderPitch = pitchSmoothTime > 0f
            ? Mathf.SmoothDampAngle(_renderPitch, targetPitch, ref _renderPitchVel, pitchSmoothTime)
            : targetPitch;

        return Quaternion.Euler(_renderPitch, _renderYaw, 0f);
    }

    void SetupFirstPersonWeaponAnchor()
    {
        if (target == null || cam == null)
            return;

        if (!target.TryGetComponent(out WeaponManager wm))
            return;

        Transform fpAnchor = cam.transform.Find("FirstPersonItems");

        if (fpAnchor == null)
        {
            GameObject anchorGo = new GameObject("FirstPersonItems");
            fpAnchor = anchorGo.transform;
            fpAnchor.SetParent(cam.transform, false);
        }

        fpAnchor.localPosition = new Vector3(0.5f, -0.5f, 0.5f);
        fpAnchor.localRotation = Quaternion.identity;
        fpAnchor.localScale = Vector3.one;

        wm.SetFirstPersonAnchor(fpAnchor);
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

    public void SetTargetAlive(bool alive)
    {
        _targetAlive = alive;
    }
    
    public void SetZoomInput(bool held, bool allowed)
    {
        _zoomHeld = held;
        _zoomAllowed = allowed;
    }
    
    void UpdateZoomFov()
    {
        if (!cam)
            return;

        float targetFov = IsZoomed ? zoomFov : normalFov;
        float smoothTime = IsZoomed ? zoomInTime : zoomOutTime;

        if (!enableZoom || smoothTime <= 0f)
        {
            cam.fieldOfView = targetFov;
            _fovVelocity = 0f;
            return;
        }

        cam.fieldOfView = Mathf.SmoothDamp(
            cam.fieldOfView,
            targetFov,
            ref _fovVelocity,
            Mathf.Max(0.001f, smoothTime),
            Mathf.Infinity,
            Time.deltaTime);
    }
    
}


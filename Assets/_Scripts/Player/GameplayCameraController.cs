using UnityEngine;
using _Scripts.Player;
using _Scripts.Weapons;

[DisallowMultipleComponent]
public class GameplayCameraController : MonoBehaviour
{
    AdvancedPredictedController target;
    Transform followTarget;
    Camera cam;
    AudioListener audioListener;
    InputHandler ih;
    
    [Header("Zoom")]
    [SerializeField] bool enableZoom = true;
    [SerializeField] float zoomFov = 30f;
    [SerializeField] float zoomInTime = 0.08f;
    [SerializeField] float zoomOutTime = 0.08f;

    float _fovVelocity;

    public bool IsZoomed => enableZoom && target != null && ih != null && _targetAlive 
                            && !target.IsFrozen && ih.ZoomHeld && !inThirdPerson;
    
    [Header("Hierarchy")]
    [SerializeField] Transform effectsRoot;
    
    [Header("Weapon Presentation")]
    [SerializeField] private Transform firstPersonItemsAnchor;

    private bool _poseSnapRequested;
    private PlayerHealth targetHealth;
    private float _normalFov;
    
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
            Debug.LogError("GameplayCameraController: No child Camera found under GameplayCameraRig.");

            enabled = false;
            return;
        }

        audioListener = cam.GetComponent<AudioListener>();
        
        _normalFov = cam.fieldOfView;

        int fpIdx = LayerMask.NameToLayer(FP_LAYER);
        int tpIdx = LayerMask.NameToLayer(TP_LAYER);

        if (fpIdx < 0 || tpIdx < 0)
            Debug.LogWarning($"GameplayCameraController: Missing layer(s). Ensure layers '{FP_LAYER}' and '{TP_LAYER}' exist.");

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
        else
            SetGameplayCameraActive(false);
    }

    void OnEnable()
    {
        if (cam)
            ApplyCulling(inThirdPerson);
    }

    private void OnDestroy()
    {
        LocalPlayerContext.OnLocalPlayerReady -= HandleLocalPlayerReady;

        LocalPlayerContext.OnLocalPlayerCleared -= HandleLocalPlayerCleared;

        UnsubscribeFromTarget();
    }

    private void HandleLocalPlayerReady(AdvancedPredictedController controller)
    {
        SetTarget(controller);

        Debug.Log($"[GameplayCameraController] Bound to local player: {controller.name}");
    }

    private void HandleLocalPlayerCleared()
    {
        SetTarget(null);
    }

    private void SetTarget(AdvancedPredictedController newTarget)
    {
        UnsubscribeFromTarget();

        target = newTarget;
        targetHealth = null;
        followTarget = null;
        ih = null;

        _fovVelocity = 0f;
        _poseSnapRequested = false;

        inThirdPerson = false;
        _targetAlive = false;

        ApplyCulling(false);

        if (target == null)
        {
            if (cam != null)
                cam.fieldOfView = _normalFov;

            SetGameplayCameraActive(false);
            return;
        }

        target.OnLocalPoseResetApplied += HandleLocalPoseResetApplied;

        targetHealth = target.GetComponent<PlayerHealth>();

        if (targetHealth != null)
            targetHealth.OnClientAliveStateApplied += HandleTargetAliveStateApplied;
        

        followTarget = target.CameraFollowTarget;

        ih = target.GetComponent<InputHandler>();

        if (ih != null)
        {
            ih.ClearTransientBuffers();
            ih.SetThirdPersonView(false);
        }

        _targetAlive = targetHealth == null || targetHealth.IsAlive;

        SetGameplayCameraActive(true);

        if (cam != null)
            cam.fieldOfView = _normalFov;

        SnapToTarget();
        SetupFirstPersonWeaponAnchor();
    }
    
    private void UnsubscribeFromTarget()
    {
        if (target != null)
        {
            target.OnLocalPoseResetApplied -=
                HandleLocalPoseResetApplied;
        }

        if (targetHealth != null)
        {
            targetHealth.OnClientAliveStateApplied -=
                HandleTargetAliveStateApplied;
        }
    }
    
    private void HandleTargetAliveStateApplied(bool alive)
    {
        _targetAlive = alive;
    }
    
    private void SnapToTarget()
    {
        if (target == null || followTarget == null)
            return;
        
        /*
         * Position depends on transform.forward in third person,
         * so rotation must be assigned first.
         */
        transform.rotation = GetDesiredRotation();

        transform.position = GetDesiredPosition();
    }

    void Update()
    {
        if (ih != null &&
            ih.ConsumeViewToggle())
        {
            inThirdPerson = !inThirdPerson;

            ih.SetThirdPersonView(inThirdPerson);

            UpdateView(inThirdPerson);

            SnapToTarget();
        }
    }

    private void LateUpdate()
    {
        UpdateZoomFov();

        if (target == null || followTarget == null)
            return;

        /*
         * Pose resets are processed even while dead so the persistent
         * camera is already aligned when the player becomes alive again.
         */
        if (_poseSnapRequested)
        {
            _poseSnapRequested = false;
            SnapToTarget();
        }

        if (!_targetAlive)
            return;

        transform.rotation = GetDesiredRotation();

        transform.position = GetDesiredPosition();
    }

    Vector3 GetDesiredPosition()
    {
        if (!followTarget)
            return transform.position;

        if (inThirdPerson)
            return followTarget.position - transform.forward * back + Vector3.up * up;

        return followTarget.position;
    }

    private Quaternion GetDesiredRotation()
    {
        if (target == null)
            return transform.rotation;

        /*
         * Preserve current frozen behavior:
         * no local render-look preview while gameplay look is frozen.
         */
        Vector2 pendingLook = !target.IsFrozen && ih != null ? ih.PendingLookDelta : Vector2.zero;

        return target.GetRenderViewRotation(pendingLook);
    }

    private void SetupFirstPersonWeaponAnchor()
    {
        if (target == null)
            return;

        if (firstPersonItemsAnchor == null)
        {
            Debug.LogError("[GameplayCameraController] FirstPersonItems anchor is not assigned.", this);

            return;
        }

        if (!target.TryGetComponent(out WeaponManager weaponManager))
        {
            Debug.LogError($"[GameplayCameraController] {target.name} has no WeaponManager.", target);

            return;
        }

        weaponManager.SetFirstPersonAnchor(firstPersonItemsAnchor);
    }

    void UpdateView(bool isThirdPerson)
    {
        ApplyCulling(isThirdPerson);
    }

    void ApplyCulling(bool tpMode)
    {
        cam.cullingMask = tpMode
            ? (cam.cullingMask | _tpMask) & ~_fpMask
            : (cam.cullingMask | _fpMask) & ~_tpMask;
    }
    
    private void HandleLocalPoseResetApplied()
    {
        /*
         * PlayerPresentation restarts the player-side smoothers
         * synchronously. The camera performs its own snap in LateUpdate,
         * after all reset listeners have completed.
         */
        _poseSnapRequested = true;
    }
    
    private void SetGameplayCameraActive(
        bool active)
    {
        if (cam != null)
            cam.enabled = active; 

        if (audioListener != null)
            audioListener.enabled = active;
    }
    
    void UpdateZoomFov()
    {
        if (!cam)
            return;

        float targetFov = IsZoomed ? zoomFov : _normalFov;
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


// _Scripts/Player/InputHandler.cs
using UnityEngine;
using FishNet.Object;

namespace _Scripts.Player
{
    /// Capture local-player input every render frame and expose it
    /// as simple properties Fish-Net can read on the next simulation tick.
    [DisallowMultipleComponent]
    public sealed class InputHandler : NetworkBehaviour
    {
        /* ---------------- public read-only ---------------- */
        public Vector2       Move        { get; private set; }   // –1…+1 per axis
        public InputButtons  HeldButtons { get; private set; }   // held this frame
        
        public bool ZoomHeld { get; private set; }
        
        public Vector2 PendingLookDelta => _lookAccum;
        
        [Header("Look Scaling")]
        [SerializeField]
        [Range(0.01f, 1f)]
        private float zoomLookSensitivityMultiplier = 0.45f;

        private AdvancedPredictedController _controller;
        private bool _thirdPersonView;
        
        Vector2 _lookAccum;

        /* -------------------------------------------------- */
        
        /* --------------- weapon hot-keys ---------------- */
        public int  WeaponSlotInput  { get; private set; }   // –1 / 0 / 1 / 2
        public int  MouseWheelDelta  { get; private set; }   // –1 / 0 / +1
        
        bool _weaponDropRequested;   // “M”   – shown to WeaponManager
        bool _togglePackPressed;    // "F" - Activate/deactivate active packs
        bool _packDropRequested;     // “P”   – shown to PackManager
        bool _viewToggleRequested;   // “V”   – camera FP/TP switch
        bool _grenadeUseRequested;
        bool _medkitUseRequested;
        bool _beaconUseRequested;
        bool _flagThrowRequested;
        bool _suicideRequested;
        private bool _jumpPressedBuffered;
        
        private void Awake()
        {
            _controller = GetComponent<AdvancedPredictedController>();
        }
        
        void Update()
        {
            if (!IsOwner)
                return;             // ignore spectators / remote avatars

            CaptureMovement();
            CaptureLook();
            CaptureHeldGameplayButtons();
            CaptureHotkeys();
        }
        
        private void CaptureMovement()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");

            float vertical = Input.GetAxisRaw("Vertical");

            Vector2 move = new Vector2(horizontal, vertical);

            Move = move.sqrMagnitude > 1f ? move.normalized : move;
        }
        
        private void CaptureLook()
        {
            Vector2 rawFrameLook = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

            ZoomHeld = Input.GetKey(KeyCode.Z);

            bool zoomLookActive = ZoomHeld && !_thirdPersonView && (_controller == null || !_controller.IsFrozen);

            float lookScale = zoomLookActive ? zoomLookSensitivityMultiplier : 1f;

            _lookAccum += rawFrameLook * lookScale;
        }
        
        public Vector2 ConsumeLookDelta()
        {
            Vector2 value = _lookAccum;
            _lookAccum = Vector2.zero;
            return value;
        }
        
        public void SetThirdPersonView(bool thirdPerson)
        {
            _thirdPersonView = thirdPerson;
        }

        public void ClearTransientBuffers()
        {
            _lookAccum = Vector2.zero;

            _jumpPressedBuffered = false;

            _weaponDropRequested = false;
            _togglePackPressed = false;
            _packDropRequested = false;
            _viewToggleRequested = false;

            _grenadeUseRequested = false;
            _medkitUseRequested = false;
            _beaconUseRequested = false;
            _flagThrowRequested = false;
            _suicideRequested = false;

            WeaponSlotInput = -1;
            MouseWheelDelta = 0;
        }
        
        private void CaptureHeldGameplayButtons()
        {
            InputButtons held =
                InputButtons.None;

            if (Input.GetKey(KeyCode.X))
                held |= InputButtons.Crouch;

            if (Input.GetKey(KeyCode.Mouse1))
                held |= InputButtons.Jetpack;

            if (Input.GetKey(KeyCode.Space))
                held |= InputButtons.Ski;

            if (Input.GetKey(KeyCode.E))
                held |= InputButtons.WallRun;

            if (Input.GetKey(KeyCode.Mouse0))
                held |= InputButtons.Fire;

            HeldButtons = held;

            if (Input.GetKeyDown(KeyCode.LeftAlt))
                _jumpPressedBuffered = true;
        }
    
        void CaptureHotkeys()
        {
            /* --- weapon selection ------------------------------------ */
            WeaponSlotInput = -1;

            if (Input.GetKeyDown(KeyCode.Alpha1)) WeaponSlotInput = 0;
            if (Input.GetKeyDown(KeyCode.Alpha2)) WeaponSlotInput = 1;
            if (Input.GetKeyDown(KeyCode.Alpha3)) WeaponSlotInput = 2;

            float wheel = Input.GetAxis("Mouse ScrollWheel");
            MouseWheelDelta = wheel > 0f ? +1 : wheel < 0f ? -1 : 0;
                
            /* --- Pack activation --------------------------------- */
            if (Input.GetKeyDown(KeyCode.F)) _togglePackPressed = true;

            /* --- weapon & pack drops --------------------------------- */
            if (Input.GetKeyDown(KeyCode.M)) _weaponDropRequested = true;
            if (Input.GetKeyDown(KeyCode.P)) _packDropRequested   = true;

            /* --- camera view toggle ---------------------------------- */
            if (Input.GetKeyDown(KeyCode.V)) _viewToggleRequested = true;
            
            /* --- Item consumption ---------------------------------- */
            // inside CaptureHotkeys()
            if (Input.GetKeyDown(KeyCode.G)) _grenadeUseRequested = true;   // Grenade
            if (Input.GetKeyDown(KeyCode.H)) _medkitUseRequested  = true;   // Med-kit
            if (Input.GetKeyDown(KeyCode.B)) _beaconUseRequested  = true;   // Beacon
            if (Input.GetKeyDown(KeyCode.T)) _flagThrowRequested  = true;   // Flag toss
            
            if (Input.GetKeyDown(KeyCode.K)) _suicideRequested = true; // Suicide / self-kill
        }

        /* ================================================================ */
        /* one-shot accessors – read-once-per-frame by other scripts       */
        public bool ConsumeWeaponDrop()
        {
            bool v = _weaponDropRequested;
            _weaponDropRequested = false;
            return v;
        }
            
        public bool ConsumePackToggle()
        {
            bool v = _togglePackPressed;
            _togglePackPressed = false;
            return v;
        }

        public bool ConsumePackDrop()
        {
            bool v = _packDropRequested;
            _packDropRequested = false;
            return v;
        }

        public bool ConsumeViewToggle()
        {
            bool v = _viewToggleRequested;
            _viewToggleRequested = false;
            return v;
        }
        
        public bool ConsumeFlagThrow()
        {
            bool v = _flagThrowRequested;
            _flagThrowRequested = false;
            return v;
        }
        
        public bool ConsumeSuicide()
        {
            bool v = _suicideRequested;
            _suicideRequested = false;
            return v;
        }
        
        public bool ConsumeJumpPressed()
        {
            bool pressed = _jumpPressedBuffered;
            _jumpPressedBuffered = false;
            return pressed;
        }
        
        // one-shot accessors – UI or managers read once per frame
        public bool ConsumeGrenadeUse() { bool v = _grenadeUseRequested; _grenadeUseRequested = false; return v; }
        public bool ConsumeMedkitUse () { bool v = _medkitUseRequested ; _medkitUseRequested  = false; return v; }
        public bool ConsumeBeaconUse () { bool v = _beaconUseRequested;  _beaconUseRequested  = false; return v; }
        
    }
}

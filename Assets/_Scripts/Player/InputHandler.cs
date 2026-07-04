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
        public Vector2       Look        { get; private set; }   // raw mouse delta
        public InputButtons  HeldButtons { get; private set; }   // held this frame
        
        public bool ZoomHeld { get; private set; }
        
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
        
        void Update()
        {
            if (!IsOwner)
                return;             // ignore spectators / remote avatars

            /* 1) movement axes (WASD) */
            float mx = Input.GetAxisRaw("Horizontal");           // -1 / 0 / +1
            float mz = Input.GetAxisRaw("Vertical");
            Move = new Vector2(mx, mz).normalized;

            /* 2) raw mouse delta */
            float lx = Input.GetAxisRaw("Mouse X");
            float ly = Input.GetAxisRaw("Mouse Y");

            Vector2 frameLook = new Vector2(lx, ly);

            Look = frameLook;          // optional: useful for debug/UI
            _lookAccum += frameLook;   // authoritative tick-consumed look

            /* 3) buttons ----------------------------------- */
            InputButtons held  = InputButtons.None;
            InputButtons down  = InputButtons.None;

            // helper local function
            static void CaptureKey(ref InputButtons h, ref InputButtons d,
                KeyCode k, InputButtons flag)
            {
                if (Input.GetKey(k))     h |= flag;   // currently held
                if (Input.GetKeyDown(k)) d |= flag;   // went down this frame
            }

            CaptureKey(ref held, ref down, KeyCode.LeftAlt,   InputButtons.Jump);
            CaptureKey(ref held, ref down, KeyCode.LeftShift, InputButtons.Sprint);
            CaptureKey(ref held, ref down, KeyCode.X,         InputButtons.Crouch);
            CaptureKey(ref held, ref down, KeyCode.Mouse1,    InputButtons.Jetpack); // RMB
            CaptureKey(ref held, ref down, KeyCode.Space,     InputButtons.Ski);
            CaptureKey(ref held, ref down, KeyCode.E,         InputButtons.WallRun);
            CaptureKey(ref held, ref down, KeyCode.Mouse0,    InputButtons.Fire);    // LMB
            
            ZoomHeld = Input.GetKey(KeyCode.Z);

            HeldButtons = held;

            CaptureHotkeys();
        }
        
        public Vector2 ConsumeLookDelta()
        {
            Vector2 value = _lookAccum;
            _lookAccum = Vector2.zero;
            return value;
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
        
        // one-shot accessors – UI or managers read once per frame
        public bool ConsumeGrenadeUse() { bool v = _grenadeUseRequested; _grenadeUseRequested = false; return v; }
        public bool ConsumeMedkitUse () { bool v = _medkitUseRequested ; _medkitUseRequested  = false; return v; }
        public bool ConsumeBeaconUse () { bool v = _beaconUseRequested;  _beaconUseRequested  = false; return v; }
        
    }
}

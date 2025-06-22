// _Scripts/Player/InputHandler.cs
using UnityEngine;
using FishNet.Object;

namespace _Scripts.Player
{
    /// Captures Unity-input every render-frame, writes an InputCmd for
    /// client-side prediction, and exposes one-shot hot-key flags.
    [DisallowMultipleComponent]
    public sealed class InputHandler : NetworkBehaviour
    {
        /* --------------- prediction ring --------------- */
        public InputCmdRing CmdRing { get; } = new InputCmdRing();

        /* --------------- weapon hot-keys ---------------- */
        public int  WeaponSlotInput  { get; private set; }   // –1 / 0 / 1 / 2
        public int  MouseWheelDelta  { get; private set; }   // –1 / 0 / +1

        bool _weaponDropRequested;   // “M”   – shown to WeaponManager
        bool _togglePackPressed;    // "F" - Activate/deactivate active packs
        bool _packDropRequested;     // “P”   – shown to PackManager
        bool _viewToggleRequested;   // “V”   – camera FP/TP switch

        /* ================================================================ */
        void Update()
        {
            if (!IsOwner) return;                // ignore spectators

            uint tick = TimeManager.Tick;

            /* -------- 1. movement & look axes -------- */
            Vector2 move = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            Vector2 look = new(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

            /* -------- 2. button bits ----------------- */
            InputButtons btn = InputButtons.None;
            if (Input.GetKey(KeyCode.LeftAlt))     btn |= InputButtons.Jump;
            if (Input.GetKey(KeyCode.LeftShift))   btn |= InputButtons.Sprint;
            if (Input.GetKey(KeyCode.X))           btn |= InputButtons.Crouch;
            if (Input.GetMouseButton(1))           btn |= InputButtons.Jetpack;
            if (Input.GetKey(KeyCode.Space))       btn |= InputButtons.Ski;
            if (Input.GetKey(KeyCode.E))           btn |= InputButtons.WallRun;
            if (Input.GetMouseButton(0))           btn |= InputButtons.Fire;

            CmdRing.Push(new InputCmd
            {
                tick    = tick,
                move    = move,
                look    = look,
                buttons = btn
            });

            CaptureHotkeys();
        }

        /* ================================================================ */
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
    }
}

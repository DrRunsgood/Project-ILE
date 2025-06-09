// _Scripts/Player/InputHandler.cs
using UnityEngine;
using FishNet.Object;

namespace _Scripts.Player
{
    /// Reads Unity input every render-frame, writes an InputCmd for the
    /// prediction system, and exposes a few hot-keys to other scripts.
    [DisallowMultipleComponent]
    public sealed class InputHandler : NetworkBehaviour
    {
        /* ───────── prediction ring ───────── */
        public InputCmdRing CmdRing { get; } = new InputCmdRing();

        /* ───────── hot-keys queried by WeaponManager ───────── */
        public int  WeaponSlotInput { get; private set; }   // –1 = none, 0/1/2
        public int  MouseWheelDelta { get; private set; }   // –1 / 0 / +1

        /* “drop” is a one-shot flag – consumed by WeaponManager */
        bool _dropPressed;

        /* ================================================================= */
        void Update()
        {
            if (!IsOwner) return;                 // ignore spectators

            uint tick = TimeManager.Tick;

            /* ---------- 1) movement / look -------------------------------- */
            Vector2 move = new(
                Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            Vector2 look = new(
                Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

            /* ---------- 2) buttons --------------------------------------- */
            InputButtons btn = InputButtons.None;
            if (Input.GetKey(KeyCode.LeftAlt))     btn |= InputButtons.Jump;
            if (Input.GetKey(KeyCode.LeftShift))   btn |= InputButtons.Sprint;
            if (Input.GetKey(KeyCode.X))           btn |= InputButtons.Crouch;
            if (Input.GetMouseButton(1))           btn |= InputButtons.Jetpack;
            if (Input.GetKey(KeyCode.Space))       btn |= InputButtons.Ski;
            if (Input.GetKey(KeyCode.E))           btn |= InputButtons.WallRun;
            if (Input.GetMouseButton(0))           btn |= InputButtons.Fire;

            /* ---------- 3) write into ring-buffer ------------------------ */
            CmdRing.Push(new InputCmd
            {
                tick    = tick,
                move    = move,
                look    = look,
                buttons = btn
            });

            CaptureWeaponHotkeys();
        }

        /* ================================================================= */
        void CaptureWeaponHotkeys()
        {
            /* reset frame-local values */
            WeaponSlotInput = -1;
            MouseWheelDelta =  0;

            /* number keys – select slot */
            if (Input.GetKeyDown(KeyCode.Alpha1)) WeaponSlotInput = 0;
            if (Input.GetKeyDown(KeyCode.Alpha2)) WeaponSlotInput = 1;
            if (Input.GetKeyDown(KeyCode.Alpha3)) WeaponSlotInput = 2;

            /* drop key (use any key you like) */
            if (Input.GetKeyDown(KeyCode.M))
                _dropPressed = true;

            /* mouse-wheel scroll */
            float wheel = Input.GetAxis("Mouse ScrollWheel");
            MouseWheelDelta = wheel > 0f ? +1 :
                              wheel < 0f ? -1 : 0;
        }

        /// Called by WeaponManager once per frame.
        public bool ConsumeDropKey()
        {
            bool pressed = _dropPressed;
            _dropPressed = false;          // reset for next frame
            return pressed;
        }
    }
}

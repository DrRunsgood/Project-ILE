// Player/InputHandler.cs
using UnityEngine;
using FishNet.Object;

namespace _Scripts.Player
{
    /// Captures Unity-input every rendered frame and writes an InputCmd
    /// into the local ring-buffer for prediction and reconciliation.
    public class InputHandler : NetworkBehaviour
    {
        /* Public so AdvancedPredictedController can read it.                   */
        public InputCmdRing CmdRing { get; } = new InputCmdRing();
        public int  WeaponSlotInput   { get; private set; }   // -1 none, 0/1/2 pick slot
        public int  MouseWheelDelta   { get; private set; }   // -1,0,+1

        void Update()
        {
            if (!IsOwner)
                return;

            uint tick = TimeManager.Tick;     // Fish-Net’s current tick

            // ----- 1.  scalar / vector inputs --------------------------------
            Vector2 move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

            Vector2 look = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

            // ----- 2.  buttons -----------------------------------------------
            InputButtons btn = InputButtons.None;
            if (Input.GetKey(KeyCode.LeftAlt))     btn |= InputButtons.Jump;
            if (Input.GetKey(KeyCode.LeftShift))   btn |= InputButtons.Sprint;
            if (Input.GetKey(KeyCode.X))           btn |= InputButtons.Crouch;
            if (Input.GetMouseButton(1))           btn |= InputButtons.Jetpack;
            if (Input.GetKey(KeyCode.Space))       btn |= InputButtons.Ski;
            if (Input.GetKey(KeyCode.E))           btn |= InputButtons.WallRun;
            if (Input.GetButton("Fire1"))          btn |= InputButtons.Fire;

            // ----- 3.  assemble & store --------------------------------------
            InputCmd cmd = new InputCmd
            {
                tick    = tick,
                move    = move,
                look    = look,
                buttons = btn
            };

            CmdRing.Push(cmd);
            
            CaptureWeaponSelect();
        }
        
        void CaptureWeaponSelect()
        {
            WeaponSlotInput = -1;
            if (Input.GetKeyDown(KeyCode.Alpha1)) WeaponSlotInput = 0;
            if (Input.GetKeyDown(KeyCode.Alpha2)) WeaponSlotInput = 1;
            if (Input.GetKeyDown(KeyCode.Alpha3)) WeaponSlotInput = 2;

            float wheel = Input.GetAxis("Mouse ScrollWheel");
            MouseWheelDelta = wheel > 0f ? +1 :
                wheel < 0f ? -1 : 0;
        }
    }
}

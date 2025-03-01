// Scripts/Player/InputHandler.cs
using UnityEngine;
using FishNet.Object;

namespace _Scripts.Player
{
    public class InputHandler : NetworkBehaviour
    {
        // Movement Inputs
        public Vector2 MovementInput { get; private set; }
        public bool SprintInput { get; private set; }
        public bool CrouchInput { get; private set; }
        public bool JumpInput { get; private set; }

        // Ability Inputs
        public bool JetpackInput { get; private set; }
        public bool SkiInput { get; private set; }
        public bool WallRunInput { get; private set; }

        // Firing Input
        public bool FireInput { get; private set; }
        public bool PFireInput { get; private set; }

        // Look Inputs
        public Vector2 LookInput { get; private set; }

        void Update()
        {
            if (!IsOwner) return;

            CaptureMovementInput();
            CaptureAbilityInput();
            CaptureFiringInput();
            CaptureLookInput();
        }

        private void CaptureMovementInput()
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveZ = Input.GetAxisRaw("Vertical");
            MovementInput = new Vector2(moveX, moveZ).normalized;
            SprintInput = Input.GetKey(KeyCode.LeftShift);
            JumpInput = Input.GetKey(KeyCode.LeftAlt);
            CrouchInput = Input.GetKey(KeyCode.X);
        }

        private void CaptureAbilityInput()
        {
            JetpackInput = Input.GetMouseButton(1); // Right Mouse Button for Jetpack
            SkiInput = Input.GetKey(KeyCode.Space);
            WallRunInput = Input.GetKey(KeyCode.E);
        }

        private void CaptureFiringInput()
        {
            FireInput = Input.GetButton("Fire1"); // Left Mouse Button for Firing
            PFireInput = Input.GetKey(KeyCode.Alpha1);
        }

        private void CaptureLookInput()
        {
            float mouseX = Input.GetAxisRaw("Mouse X");
            float mouseY = Input.GetAxisRaw("Mouse Y");
            LookInput = new Vector2(mouseX, mouseY);
        }
    }
}
// Scripts/Player/Cameras/CameraController.cs

using FishNet.Object;
using UnityEngine;

namespace _Scripts.Player
{
    public class CameraController : NetworkBehaviour
    {
        [Tooltip("Reference to the player camera transform.")]
        public Transform playerCamera; // Assign via Inspector

        // Internal State
        private Camera _cam;
        private InputHandler _inputHandler;

        void Awake()
        {
            _inputHandler = GetComponent<InputHandler>();
            if (_inputHandler == null)
            {
                Debug.LogError("CameraController: InputHandler is missing.");
                enabled = false;
                return;
            }

            // Automatic Camera Assignment if not set
            if (playerCamera == null)
            {
                _cam = GetComponentInChildren<Camera>();
                if (_cam != null)
                {
                    playerCamera = _cam.transform;
                    Debug.LogWarning("CameraController: Player Camera was not assigned. Automatically assigned the first child Camera.");
                }
                else
                {
                    Debug.LogError("CameraController: No Camera found in children. Please assign a Camera.");
                }
            }
            else
            {
                _cam = playerCamera.GetComponent<Camera>();
                if (_cam == null) Debug.LogError("CameraController: Assigned playerCamera does not have a Camera component.");
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (IsOwner)
            {
                if (_cam != null) _cam.enabled = true;

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                if (_cam != null) _cam.enabled = false;
            }
        }

        void Update()
        {
            if (!IsOwner || _inputHandler == null) return;
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            if (IsOwner)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}

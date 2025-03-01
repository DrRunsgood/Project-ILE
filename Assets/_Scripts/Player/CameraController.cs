// Scripts/Player/Cameras/CameraController.cs

using FishNet.Object;
using UnityEngine;

namespace _Scripts.Player
{
    public class CameraController : NetworkBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private float mouseSensitivity = 100f;

        [Tooltip("Reference to the player camera transform.")]
        public Transform playerCamera; // Assign via Inspector

        // Internal State
        private float _xRotation;
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

            HandleCameraPitch();
        }

        private void HandleCameraPitch()
        {
            float mouseY = _inputHandler.LookInput.y * mouseSensitivity * Time.deltaTime;

            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f); // Prevent over-rotation

            if (playerCamera != null) playerCamera.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
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

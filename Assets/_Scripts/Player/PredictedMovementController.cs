using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;
using UnityEngine;

namespace _Scripts.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PredictedPlayerController : NetworkBehaviour
    {
        #region Fields & Properties

        public enum MovementState
        {
            Walking,
            Sprinting,
            Crouching,
            WallRunning,
            Jetpacking,
            Skiing,
            Grounded,
            Airborne
        }

        [Header("Movement Speeds & Forces")]
        [SerializeField] private float movementSpeed = 5f;            // Ground movement speed
        [SerializeField] private float jumpForce = 5f;                // Force for jumping
        [SerializeField] private float jetpackForce = 8f;             // Impulse for jetpack usage
        [Tooltip("Fraction of WSAD input to blend with upward thrust.")]
        [SerializeField] private float jetpackDirectionBlend = 0.5f;

        [Header("Rotation Settings")]
        [SerializeField] private float yawSensitivity = 2f;
        [SerializeField] private float pitchSensitivity = 2f;
        [SerializeField] private float minPitch = -90f;
        [SerializeField] private float maxPitch = 90f;

        [Header("Jump & Cooldowns")]
        [SerializeField] private float jumpCooldown = 0.2f;
        private float _jumpCooldownRemaining;

        [Header("Linear Dampings")]
        [SerializeField] private float groundDamping = 2f;
        [SerializeField] private float airDamping = 0.1f;
        [SerializeField] private float skiingDamping = 0f;

        [Header("Ground Check")]
        [Tooltip("Offset from the player's rigidbody center for ground checking.")]
        [SerializeField] private Vector3 feetOffset = new Vector3(0f, -1f, 0f);
        [SerializeField] private float feetRadius = 0.5f;
        [SerializeField] private LayerMask groundLayers;

        [Header("References for Remote Visibility")]
        [Tooltip("Renderers or child objects to disable for remote players.")]
        [SerializeField] private Renderer[] renderers;

        // Prediction / Physics
        private PredictionRigidbody _predictionRb;
        private Rigidbody _rigidbody;
        private float _currentPitch;

        // Input
        private InputHandler _inputHandler;

        // Camera
        private Transform _cameraTransform;

        // Movement State
        private MovementState _currentState = MovementState.Grounded;

        #endregion

        #region Unity & Network Events

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();

            // Initialize local RigidBody for prediction
            _rigidbody = GetComponent<Rigidbody>();
            _predictionRb = new PredictionRigidbody();
            _predictionRb.Initialize(_rigidbody);

            // Attempt to find a child camera
            Transform foundCamera = transform.Find("Camera");
            if (foundCamera != null)
                _cameraTransform = foundCamera;

            // Subscribe to FishNet’s tick events
            TimeManager.OnTick += OnTick;
            TimeManager.OnPostTick += OnPostTick;
        }

        public override void OnStopNetwork()
        {
            base.OnStopNetwork();
            TimeManager.OnTick -= OnTick;
            TimeManager.OnPostTick -= OnPostTick;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            // Grab our input handler
            _inputHandler = GetComponent<InputHandler>();

            // If not the owner, disable camera and renderers
            if (!IsOwner)
            {
                if (_cameraTransform != null)
                    _cameraTransform.gameObject.SetActive(false);

                foreach (Renderer renderer in renderers)
                    renderer.enabled = false;
            }
            else
            {
                if (_cameraTransform != null)
                    _cameraTransform.gameObject.SetActive(true);
            }
        }

        #endregion

        #region Prediction Flow

        private void OnTick()
        {
            if (IsOwner)
            {
                // Gather inputs from InputHandler
                float horizontal = _inputHandler.MovementInput.x;
                float vertical = _inputHandler.MovementInput.y;
                bool jump = _inputHandler.JumpInput;
                bool jetpack = _inputHandler.JetpackInput;
                float yawInput = _inputHandler.LookInput.x;
                float pitchInput = _inputHandler.LookInput.y;

                // Determine if skiing
                bool isSkiing = _inputHandler.SkiInput;

                // Pack them into MovementData
                MovementData data = new MovementData(horizontal, vertical, jump, yawInput, pitchInput, jetpack, isSkiing);
                Replicate(data);
            }
            else
            {
                // Non-owners replicate default data
                Replicate(default);
            }
        }

        private void OnPostTick()
        {
            if (!IsServerStarted)
                return;

            CreateReconcile();
        }

        [Replicate]
        private void Replicate(MovementData data, ReplicateState state = ReplicateState.Invalid, Channel channel = Channel.Unreliable)
        {
            // 1. Handle Rotation
            ApplyRotation(data.Yaw, data.Pitch);

            // 2. Determine movement state
            UpdateMovementState(data.Skiing);

            // 3. Update linearDamping based on state
            UpdateDamping();

            // 4. If Jetpack pressed, apply jet impulse; otherwise handle ground/air/ski movement
            if (data.Jetpack)
            {
                ApplyJetpackImpulse(data.Horizontal, data.Vertical);
            }
            else
            {
                ApplyCoreMovement(data.Horizontal, data.Vertical);
            }

            // 5. Handle Jump
            HandleJump(data.Jump);

            // 6. Simulate Physics
            _predictionRb.Simulate();
        }

        public override void CreateReconcile()
        {
            var recData = new ReconciliationData(
                _rigidbody.position,
                _rigidbody.rotation,
                _rigidbody.linearVelocity,
                _rigidbody.angularVelocity,
                _currentPitch
            );
            Reconcile(recData);
        }

        [Reconcile]
        private void Reconcile(ReconciliationData data, Channel channel = Channel.Unreliable)
        {
            _rigidbody.MovePosition(data.Position);
            _rigidbody.MoveRotation(data.Rotation);
            _rigidbody.linearVelocity = data.Velocity;
            _rigidbody.angularVelocity = data.AngularVelocity;

            _currentPitch = data.Pitch;
            if (_cameraTransform != null)
                _cameraTransform.localEulerAngles = new Vector3(_currentPitch, 0f, 0f);
        }

        #endregion

        #region Movement Methods

        private void ApplyRotation(float yawInput, float pitchInput)
        {
            // Yaw
            Quaternion currentRotation = _rigidbody.rotation;
            float yawDelta = yawInput * yawSensitivity;
            _rigidbody.MoveRotation(currentRotation * Quaternion.Euler(0f, yawDelta, 0f));

            // Pitch
            _currentPitch -= pitchInput * pitchSensitivity;
            _currentPitch = Mathf.Clamp(_currentPitch, minPitch, maxPitch);

            // Apply pitch to camera
            if (_cameraTransform != null)
                _cameraTransform.localEulerAngles = new Vector3(_currentPitch, 0f, 0f);
        }

        private void UpdateMovementState(bool skiingInput)
        {
            bool grounded = IsGrounded();
            if (grounded && skiingInput)
            {
                _currentState = MovementState.Skiing;
            }
            else if (grounded)
            {
                _currentState = MovementState.Grounded;
            }
            else
            {
                _currentState = MovementState.Airborne;
            }
        }

        private void UpdateDamping()
        {
            switch (_currentState)
            {
                case MovementState.Grounded:
                    _rigidbody.linearDamping = groundDamping;
                    break;
                case MovementState.Airborne:
                    _rigidbody.linearDamping = airDamping;
                    break;
                case MovementState.Skiing:
                    _rigidbody.linearDamping = skiingDamping;
                    break;
            }
        }

        private void ApplyCoreMovement(float horizontal, float vertical)
        {
            switch (_currentState)
            {
                case MovementState.Grounded:
                    ApplyGroundMovement(horizontal, vertical);
                    break;
                case MovementState.Skiing:
                    ApplySkiMovement(horizontal, vertical);
                    break;
                case MovementState.Airborne:
                    // If you eventually want air control, place it here
                    break;
            }
        }

        private void ApplyGroundMovement(float horizontal, float vertical)
        {
            Vector3 inputDir = CalculateInputDirection(horizontal, vertical);
            Vector3 groundForce = inputDir * (movementSpeed * 10f);
            _predictionRb.AddForce(groundForce, ForceMode.Force);

            ClampGroundSpeed();
        }

        private void ApplySkiMovement(float horizontal, float vertical)
        {
            // Basic example: minimal friction, keep momentum
            Vector3 inputDir = CalculateInputDirection(horizontal, vertical);
            Vector3 skiForce = inputDir * (movementSpeed * 5f); // e.g. 5x factor if you want to test
            _predictionRb.AddForce(skiForce, ForceMode.Force);

            // No clamp => preserve speed
        }

        private void ApplyJetpackImpulse(float horizontal, float vertical)
        {
            // Combine upward with partial horizontal
            Vector3 inputDir = CalculateInputDirection(horizontal, vertical);
            Vector3 jetpackDir = (Vector3.up + inputDir * jetpackDirectionBlend).normalized;

            _predictionRb.AddForce(jetpackDir * jetpackForce, ForceMode.Impulse);
        }

        private void HandleJump(bool jumpInput)
        {
            bool groundedOrSkiing =
                (_currentState == MovementState.Grounded || _currentState == MovementState.Skiing);

            if (jumpInput && groundedOrSkiing && _jumpCooldownRemaining <= 0f)
            {
                _predictionRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                _jumpCooldownRemaining = jumpCooldown;
            }

            if (_jumpCooldownRemaining > 0f)
                _jumpCooldownRemaining -= Time.fixedDeltaTime;
        }

        private Vector3 CalculateInputDirection(float horizontal, float vertical)
        {
            Quaternion currentRot = _rigidbody.rotation;
            Vector3 right = currentRot * Vector3.right;
            Vector3 forward = currentRot * Vector3.forward;
            return (right * horizontal + forward * vertical).normalized;
        }

        private void ClampGroundSpeed()
        {
            Vector3 velocity = _rigidbody.linearVelocity; // using linearVelocity
            Vector3 horizontalVel = new Vector3(velocity.x, 0f, velocity.z);
            if (horizontalVel.magnitude > movementSpeed)
            {
                Vector3 clamped = horizontalVel.normalized * movementSpeed;
                _predictionRb.Velocity(new Vector3(clamped.x, velocity.y, clamped.z));
            }
        }

        private bool IsGrounded()
        {
            Vector3 checkPos = _rigidbody.position + feetOffset;
            return Physics.CheckSphere(checkPos, feetRadius, groundLayers);
        }

        #endregion

        #region Data Structures

        private struct MovementData : IReplicateData
        {
            private uint _tick;
            public float Horizontal;
            public float Vertical;
            public bool Jump;
            public float Yaw;
            public float Pitch;
            public bool Jetpack;
            public bool Skiing;

            public MovementData(float horizontal, float vertical, bool jump,
                float yaw, float pitch, bool jetpack, bool skiing)
            {
                _tick = 0;
                Horizontal = horizontal;
                Vertical = vertical;
                Jump = jump;
                Yaw = yaw;
                Pitch = pitch;
                Jetpack = jetpack;
                Skiing = skiing;
            }

            public uint GetTick() => _tick;
            public void SetTick(uint value) => _tick = value;
            public void Dispose() { }
        }

        private struct ReconciliationData : IReconcileData
        {
            private uint _tick;
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 Velocity;
            public Vector3 AngularVelocity;
            public float Pitch;

            public ReconciliationData(
                Vector3 position, Quaternion rotation,
                Vector3 velocity, Vector3 angularVelocity,
                float pitch)
            {
                _tick = 0;
                Position = position;
                Rotation = rotation;
                Velocity = velocity;
                AngularVelocity = angularVelocity;
                Pitch = pitch;
            }

            public uint GetTick() => _tick;
            public void SetTick(uint value) => _tick = value;
            public void Dispose() { }
        }

        #endregion
    }
}

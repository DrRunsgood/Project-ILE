using System;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;
using GameKit.Dependencies.Utilities;
using Unity.VisualScripting;

namespace _Scripts.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class AdvancedPredictedController : NetworkBehaviour
    {
        #region Enums
        
        public enum MovementState : byte
        {
            None,
            Walking,
            Sprinting,
            WallRunning,
            Jetpacking,
            Skiing,
            Crouching,
            Airborne
        }

        #endregion

        #region Inspector Fields

        [Header("References")]
        [SerializeField] private Transform orientation;
        [Tooltip("If you have a separate camera transform, reference it here to apply pitch to the camera only.")]
        [SerializeField] private Transform cameraTransform;

        [Header("Look Settings")]
        [Tooltip("How fast we rotate horizontally.")]
        [SerializeField] private float yawSensitivity = 2f;
        [Tooltip("How fast we rotate vertically.")]
        [SerializeField] private float pitchSensitivity = 2f;
        [SerializeField] private float minPitch = -90f;
        [SerializeField] private float maxPitch = 90f;

        [Header("Movement Speeds")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float sprintSpeed = 10f;
        [SerializeField] private float crouchSpeed = 2f;
        
        [Header("Physics & Drag")]
        [SerializeField] private float groundDrag = 5f;
        [SerializeField] private float airDrag = 0.1f;
        [SerializeField] private float blendFactor = 0.005f;
        
        [Header("Jumping")]
        [SerializeField] private float jumpForce = 5f;

        [SerializeField] private float jumpCooldown = 0.5f;
        private float _jumpCooldownRemaining = 0f;   
        
        [Header("Crouching")]
        [SerializeField] private float crouchYScale = 0.5f;
        
        [Header("Slope Handling")]
        [SerializeField] private float maxSlopeAngle = 45f;
        [SerializeField] private float groundCheckRadius = 0.3f;
        [SerializeField] private float groundCheckDistance = 0.4f;
        [SerializeField] private float slopeCheckDistance = 1.2f;
        [SerializeField] private LayerMask groundMask;
        
        [Header("Ground Check")]
        [SerializeField] private Vector3 feetOffset = new Vector3(0f, -1f, 0f);
        [SerializeField] private float feetRadius = 0.5f;
        
        [Header("Wall Running")]
        [SerializeField] private LayerMask whatIsWall;
        [SerializeField] private float wallJumpUpForce = 8f;
        [SerializeField] private float wallJumpSideForce = 6f;
        [SerializeField] private float wallRunForce = 200f;
        [SerializeField] private float targetWallRunSpeedSlow = 15f;
        [SerializeField] private float slowWallRunThreshold = 25f;
        [SerializeField] private float targetWallRunSpeedMid = 25f;
        [SerializeField] private float midWallRunThreshold = 40f;
        [SerializeField] private float targetWallRunSpeedFast = 40f;
        [SerializeField] private float maxWallRunTime = 1.5f;
        [SerializeField] private float wallCheckDistance = 0.6f;
        [SerializeField] private float minJumpHeight = 1.2f;
        [SerializeField] private float exitWallTime = 0.2f;
        [SerializeField] private bool useGravity = true;
        [SerializeField] private float gravityCounterForce = 20f;
        [SerializeField] private float wallRunGraceTime = 0.2f;
        [SerializeField] private float wallRunLerpSpeed = 5f;

        [Header("Jetpack Settings")]
        [SerializeField] private float jetpackForce = 15f;
        [SerializeField] private float jetpackFuelBurnRate = 10f;
        [SerializeField] private float maxJetpackFuel = 100f;
        [SerializeField] private float jetpackFuelRegenRate = 5f;
        [SerializeField] private float jetpackFuelCutoff;
        [SerializeField] private float jetpackDirectionalBlend = 0.3f;
        [SerializeField] private float maxAdditionalForwardSpeed = 30f;
        [SerializeField] private float maxAdditionalLateralSpeed = 40f;

        [Header("Skiing Settings")]
        [SerializeField] private float skiControl = 0.1f;
        [SerializeField] private float skiDrag = 0.1f;

        [Header("Debug / State Info")]
        [SerializeField] private MovementState _state;

        private MovementState _previousState;
        public MovementState State => _state;

        #endregion

        #region Internals

        private Rigidbody _rb;
        private PredictionRigidbody _predictionRb;

        private float _moveSpeed;
        private float _startYScale;
        private bool _isGrounded;
        private RaycastHit _groundHit;
        private RaycastHit _slopeHit;

        // Movement input
        private float _horizontalInput;
        private float _verticalInput;
        private Vector3 _moveDirection;
       
        // Input
        private InputHandler _inputHandler;
        
        private float _horizontal;
        private float _vertical;
        private float _yawInput;
        private float _pitchInput;

        // Determine abilities
        private bool _jump;
        private bool _sprint;
        private bool _crouch;
        private bool _wallRun;
        private bool _jetpack;
        private bool _isSkiing;
        
        
        // For camera orientation
        private float _currentPitch; // we clamp pitch with minPitch, maxPitch

        // Wall Running
        private bool _canWallRun;
        private bool _wallLeft;
        private bool _wallRight;
        private bool _exitingWall;
        private float _wallRunTimer;
        private float _exitWallTimer;
        private Vector3 _wallRunNormal = Vector3.zero;
        private Vector3 _wallRunDirection;
        private float _targetWallRunSpeed;
        private Vector3 _storedWallNormal = Vector3.zero;
        private float _wallRunGraceTimer;
        private Vector3 _normVel;

        // Jetpack
        private bool _isJetpacking;
        private float _currentJetpackFuel;

        #endregion

        #region FishNet Data Structures

        private struct MovementData : IReplicateData
        {
            private uint _tick;
            public float Horizontal;
            public float Vertical;
            public float Yaw;
            public float Pitch;
            public MovementState State; // Replaces all individual movement bools
            public bool JumpPressed;
            public bool Jetpack;
            public bool Skiing;
            public bool WallRun;
            public bool Sprint;
            public bool Crouch;
            

            public MovementData(float horizontal, float vertical, float yaw, float pitch, MovementState state, bool jumpPressed, 
                bool jetpack, bool isSkiing, bool wallRun, bool sprint, bool crouch)
            {
                _tick = 0; // Ensure tick is initialized
                Horizontal = horizontal;
                Vertical = vertical;
                Yaw = yaw;
                Pitch = pitch;
                State = state;
                JumpPressed = jumpPressed;
                Jetpack = jetpack;
                Skiing = isSkiing;
                WallRun = wallRun;
                Sprint = sprint;
                Crouch = crouch;
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
            public Vector3 LinearVelocity;
            public Vector3 AngularVelocity;
            public MovementState State;
            public float CurrentPitch;
            public float Drag;

            public ReconciliationData(Vector3 position, Quaternion rotation, Vector3 linearVelocity, Vector3 angularVelocity,
                MovementState state, float currentPitch, float currentDrag)
            {
                _tick = 0;
                Position           = position;
                Rotation           = rotation;
                LinearVelocity     = linearVelocity;
                AngularVelocity    = angularVelocity;
                State              = state;
                CurrentPitch       = currentPitch;
                Drag               = currentDrag;
            }

            public uint GetTick() => _tick;
            public void SetTick(uint value) => _tick = value;
            public void Dispose() { }
        }

        #endregion

        #region Network Events

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();

            _rb = GetComponent<Rigidbody>();
            _predictionRb = new PredictionRigidbody();
            _predictionRb.Initialize(_rb);

            _startYScale = transform.localScale.y;
            _currentJetpackFuel = maxJetpackFuel;

            TimeManager.OnTick += OnTick;
            TimeManager.OnPostTick += OnPostTick;
        }
        
        public override void OnStartClient()
        {
            base.OnStartClient();

            // Grab our input handler
            _inputHandler = GetComponent<InputHandler>();
            
            // If not the owner, disable camera and renderers - DG - late add while debugging, just a test
            if (!IsOwner)
            {
                if (cameraTransform != null)
                    cameraTransform.gameObject.SetActive(false);

            }
            else
            {
                if (cameraTransform != null)
                    cameraTransform.gameObject.SetActive(true);
            }
        }

        public override void OnStopNetwork()
        {
            base.OnStopNetwork();
            TimeManager.OnTick -= OnTick;
            TimeManager.OnPostTick -= OnPostTick;
        }

        private void OnTick()
        {
            if (IsOwner)
            {
                // Movement and Look control
                _horizontal = _inputHandler.MovementInput.x;
                _vertical = _inputHandler.MovementInput.y;
                _yawInput = _inputHandler.LookInput.x;
                _pitchInput = _inputHandler.LookInput.y;

                // Determine abilities input
                _jump = _inputHandler.JumpInput;
                _sprint = _inputHandler.SprintInput;
                _crouch = _inputHandler.CrouchInput;
                _wallRun = _inputHandler.WallRunInput;
                _jetpack = _inputHandler.JetpackInput;
                _isSkiing = _inputHandler.SkiInput;
                
                // Update fuel
                //UpdateFuel();
                
                // Pack them into MovementData
                MovementData data = new MovementData(_horizontal, _vertical, _yawInput, _pitchInput, _state, _jump, 
                    _jetpack, _isSkiing, _wallRun, _sprint, _crouch);
                
                Replicate(data);
            }
            else
            {
                Replicate(default);
            }
        }

        private void OnPostTick()
        {
            if (!IsServerStarted)
                return;

            CreateReconcile();
        }
        
        public override void CreateReconcile()
        {
            var recData = new ReconciliationData(
                _rb.position,
                _rb.rotation,
                _rb.linearVelocity,
                _rb.angularVelocity,
                _state,
                _currentPitch,
                _rb.linearDamping
            );
 
            Reconcile(recData);
        }

        [Reconcile]
        private void Reconcile(ReconciliationData data, Channel channel = Channel.Unreliable)
        {
            /*
            if (Vector3.Distance(_rb.position, data.Position) > 0.25f)
            {
                _rb.position = data.Position;  // Hard correct large desyncs
            }
            else
            {
                _rb.MovePosition(Vector3.Lerp(_rb.position, data.Position, 0.5f));
            }
            */
            
            // Correct rotation using MoveRotation
            //_rb.MoveRotation(data.Rotation);
            
            // Apply velocity correction to the predicted Rigidbody
            //_predictionRb.Velocity(data.LinearVelocity);
            //_predictionRb.AngularVelocity(data.AngularVelocity);
            
            _rb.MovePosition(data.Position);
            _rb.MoveRotation(data.Rotation);
            _rb.linearVelocity = data.LinearVelocity;
            _rb.angularVelocity = data.AngularVelocity;
            _rb.linearDamping = data.Drag;
            
            // Correct movement state
            _state = data.State;

            _currentPitch = data.CurrentPitch;
            
            // If you want to apply camera pitch:
            if (cameraTransform != null)
            {
                cameraTransform.localEulerAngles = new Vector3(_currentPitch, 0f, 0f);
            }
        }

        #endregion

        #region Replicate

        [Replicate]
        private void Replicate(MovementData data, ReplicateState state = ReplicateState.Invalid, Channel channel = Channel.Unreliable)
        {
            // Ground check
            _isGrounded = IsGrounded();
            
            

            // 1) Apply rotation from yaw/pitch
            ApplyRotation(data.Yaw, data.Pitch);
            
            // Check for wall
            if (data.WallRun && data.State != MovementState.WallRunning)
            {
                CheckForWall(data);
            }

            // Update movement states
            UpdateMovementState(data);
                
            if (data.State == MovementState.WallRunning) // Only update wall run state if actively wall running
                UpdateWallRunState(data);
            
            ControlDrag(data);

            if (data.State == MovementState.Jetpacking)
            {
                if (!_isGrounded) MovePlayer(data);
                HandleJetpack(data);
            }
            
            else if (data.State == MovementState.Skiing)
                PerformSkiMovement(data);
            
            else if (data.State == MovementState.WallRunning && data.JumpPressed)  // DG - using jump input for walljump!
                            WallJump();

            else if (data.State == MovementState.WallRunning)
            {
                Physics.gravity = -_storedWallNormal * 60f;
                PerformWallRunMovement();
            }

            else
            {
                MovePlayer(data);
            }

            if (_isGrounded && data.JumpPressed) // Jump
                Jump();

            SpeedControl(data);
            
            Debug.Log($"{_rb.linearVelocity.magnitude}");
            
            _predictionRb.Simulate();
        }

        #endregion

        #region Rotation

        private void ApplyRotation(float yawInput, float pitchInput)
        {
            // Yaw => rotate _orientation horizontally
            float yawDelta = yawInput * yawSensitivity;
            Quaternion currentRot = _rb.rotation;
            Quaternion yawRot = currentRot * Quaternion.Euler(0f, yawDelta, 0f);
            _rb.MoveRotation(yawRot);

            // Pitch => track in _currentPitch
            _currentPitch -= pitchInput * pitchSensitivity;
            _currentPitch = Mathf.Clamp(_currentPitch, minPitch, maxPitch);

            // If you have a camera transform for local pitch:
            if (cameraTransform != null)
            {
                cameraTransform.localEulerAngles = new Vector3(_currentPitch, 0f, 0f);
            }
        }

        #endregion

        #region Jump

        private void Jump()
        {
            _predictionRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        
        

        private void ResetJump()
        {
            //_readyToJump = true;  // DG - currently unused as we removed jump timer - will reimplement later
        }

        #endregion

        #region Movement Methods

        private bool IsGrounded()
        {
            Vector3 checkPos = _rb.position + feetOffset;
            return Physics.CheckSphere(checkPos, feetRadius, groundMask);
        }
        private void UpdateMovementState(MovementData data)
        {
            // Always check Jetpacking first (highest priority)
            if (data.Jetpack)
            {
                _state = MovementState.Jetpacking;
                return; // Exit early since jetpack overrides all movement logic
            }

            // If not already wall running, check for wall run initiation using the wall run key.
            if (data.State != MovementState.WallRunning)
            {
                _canWallRun = CanWallRun();
                if (data.WallRun && _canWallRun)
                {
                    StartWallRun();
                    _state = MovementState.WallRunning;
                }
                
                else if (data.Skiing)
                    _state = MovementState.Skiing;
                
                else if (_isGrounded)
                {
                    if (data.Crouch)
                    {
                        _state = MovementState.Crouching;
                    }
                    else if (data.Sprint)
                    {
                        _state = MovementState.Sprinting;
                    }
                    else
                    {
                        _state = MovementState.Walking;
                    }
                }
                else
                {
                    _state = MovementState.Airborne;
                }
            }
            else
            {
                // If already in wall run, remain in wall run state.
                _state = MovementState.WallRunning;
            }
        }
        
        private void UpdateFuel()
        {
            if (!_state.HasFlag(MovementState.Jetpacking) && _currentJetpackFuel < maxJetpackFuel)
            {
                _currentJetpackFuel += jetpackFuelRegenRate * (float)base.TimeManager.TickDelta;
            }
            _currentJetpackFuel = Mathf.Clamp(_currentJetpackFuel, 0f, maxJetpackFuel);
        }

// --------------- MOVE PLAYER -----------------------------------------------        
        private void MovePlayer(MovementData data)
        {
            transform.localScale = new Vector3(transform.localScale.x, _startYScale, transform.localScale.z); // reset crouch
            
            if (data.State == MovementState.Walking)
            {
                _moveSpeed = walkSpeed; // Apply walk speed if walking
            }

            else if (data.State == MovementState.Sprinting)
                _moveSpeed = sprintSpeed; // Apply sprint speed if sprinting
            
            else if (data.State == MovementState.Crouching)
            {
                transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
                _moveSpeed = crouchSpeed; // Apply crouch speed if crouching
            }

            // Calculate move direction
            _moveDirection = (orientation.forward * data.Vertical) + (orientation.right * data.Horizontal);
            _moveDirection.Normalize();
            
            Vector3 currentVelocity = _rb.linearVelocity;

            if (OnSlope() && data.State != MovementState.Skiing && data.State != MovementState.Jetpacking)
            {
                Vector3 slopeMoveDir = Vector3.ProjectOnPlane(_moveDirection, _slopeHit.normal).normalized * _moveSpeed;
                _predictionRb.Velocity(new Vector3(slopeMoveDir.x, currentVelocity.y, slopeMoveDir.z));
                // DG - GPT recc using AddForce here: _predictionRb.AddForce(slopeMoveDir * _moveSpeed, ForceMode.Acceleration);

                // Apply anti-slide force to balance out gravity's downward pull
                float slopeForce = Mathf.Abs(Physics.gravity.y) * _rb.mass;
                _predictionRb.AddForce(-_slopeHit.normal * slopeForce, ForceMode.Force);
            }
            else if (_isGrounded)
            {
                // Normal movement on flat ground
                //Vector3 move = new Vector3(_moveDirection.x * _moveSpeed, currentVelocity.y, _moveDirection.z * _moveSpeed);
                _predictionRb.Velocity(new Vector3(_moveDirection.x * _moveSpeed, _rb.linearVelocity.y, _moveDirection.z * _moveSpeed));
                //_predictionRb.AddForce(move*15f, ForceMode.Force); // DG - testing
            }
            else
            {   // Air movement velocity blend
                Vector3 currentHorizVel = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
                if (_moveDirection != Vector3.zero && currentHorizVel.magnitude > 0.1f)
                {
                    Vector3 newHorizDir = Vector3.Slerp(currentHorizVel.normalized, _moveDirection, blendFactor).normalized;
                    Vector3 newHorizVel = newHorizDir * currentHorizVel.magnitude;
                    _predictionRb.Velocity(new Vector3(newHorizVel.x, _rb.linearVelocity.y, newHorizVel.z));
                }
            }
        }

        private bool OnSlope()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, slopeCheckDistance, groundMask))
            {
                float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
                return (angle < maxSlopeAngle && angle != 0f);
            }
            return false;
        }

        private void SpeedControl(MovementData data)
        {
            if (data.State == MovementState.Walking ||
                data.State == MovementState.Sprinting ||
                data.State == MovementState.Crouching ||
                data.State == MovementState.WallRunning) // DG - GPT suggested removing WallRun from here
            {
                Vector3 currentVelocity = _rb.linearVelocity;
                Vector3 flatVel = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
                if (flatVel.magnitude > _moveSpeed)
                {
                    Vector3 limited = flatVel.normalized * _moveSpeed;
                    _predictionRb.Velocity(new Vector3(limited.x, currentVelocity.y, limited.z)); // DG - GPT suggests using Force vs. direct Velocity changes due to potential desyc
                    //_predictionRb.AddForce(velocityDiff * speedLimitFactor, ForceMode.Acceleration);
                }
            }
        }

        private void ControlDrag(MovementData data)
        {
            float drag; // Default drag

            switch (data.State)
            {
                case MovementState.Airborne or MovementState.Jetpacking:
                    Physics.gravity = new Vector3(0, -30f, 0);  //reset grav from wallrun
                    drag = airDrag;
                    break;
                case MovementState.Skiing:
                    drag = skiDrag;
                    break;
                case MovementState.WallRunning:
                    drag = 0f;
                    break;
                default:
                    drag = groundDrag;
                    break;
            }
            _rb.linearDamping = drag;
        }

        #endregion

        #region Wall Run

        private void CheckForWall(MovementData data)
        {
            Vector3 rayOrigin = transform.position;
            Vector3 rayDirection = orientation.forward;

            if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, wallCheckDistance, whatIsWall))
            {
                // Ensure the detected surface is a valid wall (not floor/ceiling)
                if (Mathf.Abs(Vector3.Dot(hit.normal, Vector3.up)) < 0.1f)
                {
                    _wallRunNormal = hit.normal;

                    // Determine if the wall is to the left or right
                    float side = Vector3.Dot(orientation.right, _wallRunNormal);
                    _wallRight = side > 0;
                    _wallLeft  = side <= 0;

                    _wallRunGraceTimer = wallRunGraceTime;
                }
                else
                {
                    _wallRunNormal = Vector3.zero; // Reset wall normal
                    HandleWallLoss(data);  // DG - will need to pass state in to CheckForWall and into HandleWallLoss so we can check state
                }
            }
            else
            {
                _wallRunNormal = Vector3.zero; // Reset wall normal
                HandleWallLoss(data);
            }
        }

        private void HandleWallLoss(MovementData data)
        {
            if (data.State == MovementState.WallRunning)  // Need to pass movement data to change this to proper checking
            {
                _wallRunGraceTimer -= (float)base.TimeManager.TickDelta;
                
                if (_wallRunGraceTimer <= 0f) StopWallRun();  // DG - consider adding _wallRunGraceTimer = wallRunGraceTime;
            }
            else
            {
                if (_wallLeft || _wallRight) // Only reset if necessary
                {
                    _wallLeft = false;
                    _wallRight = false;
                }
            }
        }

        private bool IsAboveMinJumpHeight()
        {
            return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, groundMask);
        }

        private bool CanWallRun()
        {
            if (!(_wallLeft || _wallRight)) return false; // Skip unnecessary checks

            return IsAboveMinJumpHeight() && !_exitingWall;
        }

        private void StartWallRun()
        {
            _wallRunTimer = maxWallRunTime;
            _exitingWall  = false;
            _exitWallTimer= exitWallTime;
            _wallRunGraceTimer = wallRunGraceTime;
            _normVel = _rb.linearVelocity;

            _storedWallNormal = _wallRunNormal;
            Vector3 intendedWallRunDir = Vector3.ProjectOnPlane(orientation.forward, _storedWallNormal).normalized;
            
            Vector3 playerVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
            float speed = playerVelocity.magnitude;
            
            if (speed < slowWallRunThreshold)      _targetWallRunSpeed = targetWallRunSpeedSlow;
            else if (speed < midWallRunThreshold) _targetWallRunSpeed = targetWallRunSpeedMid;
            else                                   _targetWallRunSpeed = targetWallRunSpeedFast;

            if (Vector3.Dot(playerVelocity.normalized, intendedWallRunDir) < 0)
            {
                StopWallRun();
                return;
            }
            _wallRunDirection = intendedWallRunDir;
            _moveSpeed = speed;
        }

        private void UpdateWallRunState(MovementData data)
        {
            if (data.State == MovementState.WallRunning)
            {
                if (_canWallRun)
                {
                    if (_wallRunTimer > 0f)
                        _wallRunTimer -= (float)base.TimeManager.TickDelta;
                    else
                    {
                        _exitingWall  = true;
                        _exitWallTimer = exitWallTime;
                    }
                }
                else if (_exitingWall)
                {
                    Debug.Log($"_exitWallTimer: {_exitWallTimer}");
                    if (_exitWallTimer > 0f)
                        _exitWallTimer -= (float)base.TimeManager.TickDelta;
                    else
                    {
                        _exitingWall = false;
                        //Debug.Log("Calling StopWallRun - A");
                        StopWallRun();
                    }
                }
                else
                {
                    StopWallRun();
                   // Debug.Log("Calling StopWallRun - B");
                }
            }
        }

        private void PerformWallRunMovement()
        {
            if (!IsStillOnWall())
            {
                StopWallRun();
                return;
            }
            
            float currentSpeed = new Vector3(_normVel.x, 0f, _normVel.z).magnitude;
            float newSpeed = Mathf.Lerp(currentSpeed, _targetWallRunSpeed, wallRunLerpSpeed * (float)TimeManager.TickDelta);

            Vector3 newVel = _wallRunDirection * _targetWallRunSpeed; //newSpeed
            
            _predictionRb.Velocity(new Vector3(newVel.x, 0f, newVel.z));
            
            // Add a small velocity component pushing toward the wall
            Vector3 wallStickVelocity = -_storedWallNormal * 20f;

            // **Final velocity: move forward AND slightly into the wall**
            //_predictionRb.Velocity(new Vector3(newVel.x + wallStickVelocity.x, 0.5f, newVel.z));
            /*
            
            // 🔹 Grab current velocity every tick (but do NOT overwrite it!)
            Vector3 currentVelocity = _rb.linearVelocity;

            // 🔹 Extract horizontal movement speed (ignore Y)
            float currentSpeed = new Vector3(currentVelocity.x, 0f, currentVelocity.z).magnitude;

            // 🔹 Lerp speed toward target
            float newSpeed = Mathf.Lerp(currentSpeed, _targetWallRunSpeed, wallRunLerpSpeed * (float)TimeManager.TickDelta);

            // 🔹 Compute new desired velocity
            Vector3 desiredVelocity = _wallRunDirection * newSpeed;

            // 🔹 Compute force needed to achieve the velocity change
            Vector3 velocityDifference = (desiredVelocity - currentVelocity);
            
            // 🔹 Apply a force instead of setting velocity directly
            //_predictionRb.AddForce(velocityDifference * 10f, ForceMode.Acceleration);
            _predictionRb.AddForce(desiredVelocity + (Vector3.up * (gravityCounterForce)));
            
            */
        }

        private bool IsStillOnWall()
        {
            return Physics.Raycast(transform.position, -_storedWallNormal, wallCheckDistance, whatIsWall);
        }

        private void ApplyWallRunGravity()
        {
            //if (useGravity)
            _predictionRb.AddForce(Vector3.up * gravityCounterForce, ForceMode.Force); // DG - removed if to auto apply
        }

        private void StopWallRun()
        {
            _exitingWall   = false;
            _wallLeft      = false;
            _wallRight     = false;
            _canWallRun = false;
            _storedWallNormal = Vector3.zero;
            _state = MovementState.Airborne;
        }

        private void WallJump()
        {
            _exitingWall   = true;
            _exitWallTimer = exitWallTime;
            Vector3 forceToApply = (Vector3.up * wallJumpUpForce) + (_storedWallNormal * wallJumpSideForce);
            
            //_predictionRb.Velocity(new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z)); // DG - we may not even need this
            
            _predictionRb.AddForce(forceToApply, ForceMode.Impulse);
            StopWallRun();
        }

        #endregion
        
        #region Jetpack
        
        private void HandleJetpack(MovementData data)
        {
            if (data.State == MovementState.Jetpacking)  // && _currentJetpackFuel > jetpackFuelCutoff
            {
                ContinueJetpack(data);
                
                _currentJetpackFuel -= jetpackFuelBurnRate * (float)base.TimeManager.TickDelta;
                _currentJetpackFuel = Mathf.Clamp(_currentJetpackFuel, 0f, maxJetpackFuel);
            }
        }
        
        private void ContinueJetpack(MovementData data)
        {
            // 1. Vertical component: always applied full.
            Vector3 verticalComponent = Vector3.up * jetpackForce;

            // 2. Determine the desired horizontal (directional) input.
            Vector3 rawInput = new Vector3(data.Horizontal, 0f, data.Vertical);
            Vector3 desiredInput = rawInput.sqrMagnitude > 0.001f? orientation.TransformDirection(rawInput).normalized : Vector3.zero;

            // 3. Compute a candidate horizontal impulse from the desired input.
            Vector3 horizontalCandidate = desiredInput * (jetpackForce * jetpackDirectionalBlend);

            // 4. Optionally bias the horizontal component further toward forward if "W" is pressed.
            if (data.Vertical > 0) // Effectively checking if W is being held
            {
                horizontalCandidate = Vector3.Lerp(horizontalCandidate, orientation.forward * (jetpackForce * jetpackDirectionalBlend), jetpackDirectionalBlend);
            }

            // 5. Get the current horizontal velocity and its forward and lateral components.
            Vector3 currentHorizVel = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);  // DG - again I think we need to use the _rb to get velo values
            float currentForwardSpeed = Vector3.Dot(currentHorizVel, orientation.forward);
            float currentLateralSpeed = Mathf.Abs(Vector3.Dot(currentHorizVel, orientation.right));

            // 6. Only allow additional horizontal force if current forward speed is below our threshold  and current lateral speed is below _maxAdditionalLateralSpeed.
            Vector3 horizontalComponent = Vector3.zero;
            if (currentForwardSpeed < maxAdditionalForwardSpeed && currentLateralSpeed < maxAdditionalLateralSpeed)
            {
                horizontalComponent = horizontalCandidate*0.75f;  // 0.75 is thrust weakening to not overspeed at low speeds but keep directional control
            }
      
            // 7. Combine the vertical and horizontal components.
            Vector3 finalImpulse = verticalComponent + horizontalComponent;

            // 8. Apply the force as an impulse using ForceMode.Force.
            _predictionRb.AddForce(finalImpulse, ForceMode.Force);
        }
        
        #endregion

        #region Skiing

        private void PerformSkiMovement(MovementData data)
        {
            //if (!_isGrounded)
              //  return;

            Vector3 currentVel = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
            Vector3 baseDir = (currentVel.sqrMagnitude > 0.01f) ? currentVel.normalized : orientation.forward;

            // Process input and transform to world space
            Vector3 rawInput = new Vector3(_horizontalInput, 0f, Mathf.Min(_verticalInput, 0f));
            Vector3 desiredDir = orientation.TransformDirection(rawInput).normalized;

            // Blend input direction smoothly
            Vector3 steerDir = Vector3.Lerp(baseDir, desiredDir, skiControl).normalized;

            // Calculate steering force, proportional to current speed
            Vector3 steeringForce = (steerDir - baseDir) * (currentVel.magnitude * 1f);  // Adjust multiplier if needed
            _predictionRb.AddForce(steeringForce, ForceMode.Force);  // DG - revisit force type here for steering, acceleration is odd
        }

        #endregion

        /*
        #region Gizmos

        private void OnDrawGizmos()
        {
            // If grounded, draw sphere at the hit point; otherwise draw it at the fallback (down from player)
            Vector3 sphereCenter = _isGrounded ? _groundHit.point
                : (transform.position + Vector3.down * groundCheckDistance);
            Gizmos.DrawWireSphere(sphereCenter, groundCheckRadius);

            // --- CheckSphere Visualization (New) ---
            Gizmos.color = new Color(1f, 0.92f, 0.016f, 1f); // "Gold" to differentiate from SphereCast
            Vector3 checkSphereCenter = transform.position + feetOffset; // Match the exact position from your ground check logic
            Gizmos.DrawWireSphere(checkSphereCenter, feetRadius); // Match radius exactly

            // --- Wall Run Detection (existing code) ---
            Gizmos.color = Color.cyan;
            Vector3 rayOrigin = transform.position;
            Vector3 rayDirection = orientation.forward * wallCheckDistance;
            Gizmos.DrawLine(rayOrigin, rayOrigin + rayDirection);
        }


        #endregion
        */
    }
    
}

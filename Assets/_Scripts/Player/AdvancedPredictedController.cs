using UnityEngine;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;

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
        [SerializeField] private Transform firePoint;

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

        private NetworkObject _netObj;
        private Rigidbody _rb;
        private PredictionRigidbody _predictionRb;

        private float _moveSpeed;
        private float _startYScale;
        private bool _isGrounded;
        private bool _onSlope;
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
        //testing
        private float _currentWallRunSpeed; // Current speed for wall-running (maintained separately)

        // Jetpack
        private bool _isJetpacking;
        private float _currentJetpackFuel;
        
        // Knockback
        private Vector3? _pendingKnockback;
        private float? _pendingTempDrag;

        private const string LOCAL_LAYER   = "LocalPlayer";
        private const string REMOTE_LAYER  = "RemotePlayer";

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

        private static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            foreach (Transform t in go.transform)
                SetLayerRecursively(t.gameObject, layer);
        }

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();

            _rb = GetComponent<Rigidbody>();
            _netObj = GetComponent<NetworkObject>();
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
            int local  = LayerMask.NameToLayer(LOCAL_LAYER);
            int remote = LayerMask.NameToLayer(REMOTE_LAYER);

            SetLayerRecursively(gameObject, IsOwner ? local : remote);
            
            if (IsOwner)
                _inputHandler = GetComponent<InputHandler>();
            
            if (cameraTransform != null)
                cameraTransform.gameObject.SetActive(IsOwner);
        }

        public override void OnStartServer()
        {
            int remote = LayerMask.NameToLayer(REMOTE_LAYER);
            SetLayerRecursively(gameObject, remote);
        }

        public override void OnStopNetwork()
        {
            base.OnStopNetwork();
            TimeManager.OnTick -= OnTick;
            TimeManager.OnPostTick -= OnPostTick;
        }

        private void Update()
        {
            if (!IsOwner) return;
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
        }

        private void OnTick()
        {
            if (IsOwner)
            {
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
            _rb.MovePosition(data.Position);
            _rb.MoveRotation(data.Rotation);
            _rb.linearVelocity = data.LinearVelocity;
            _rb.angularVelocity = data.AngularVelocity;
            _rb.linearDamping = data.Drag;
            
            // Correct movement state
            _state = data.State;

            _currentPitch = data.CurrentPitch;
            
            cameraTransform.localEulerAngles = new Vector3(_currentPitch, 0f, 0f);
        }

        #endregion

        #region Replicate

        [Replicate]
        private void Replicate(MovementData data, ReplicateState state = ReplicateState.Invalid, Channel channel = Channel.Unreliable)
        {
            // Check for incoming Knockback
            if (_pendingKnockback.HasValue)
            {
                ApplyKnockback();
                _predictionRb.Simulate();
                return; 
            }
            
            // Ground check
            _isGrounded = IsGrounded();

            // Reset slope check, recheck if on ground - micro optimization over checking every ontick
            _onSlope = false;
            if(_isGrounded) _onSlope = OnSlope();
            
            // 1) Apply rotation from yaw/pitch
            ApplyRotation(data.Yaw, data.Pitch);
            
            // Check for wall
            if (data.WallRun && data.State != MovementState.WallRunning)
                CheckForWall(data);

            // Update movement states
            UpdateMovementState(data);
                
            if (data.State == MovementState.WallRunning) // Only update wall run state if actively wall running
                UpdateWallRunState(data);
            
            ControlEnv(data);

            if (data.State == MovementState.Jetpacking)
            {
                if (!_isGrounded) MovePlayer(data);
                HandleJetpack(data);
            }
            
            else if (data.State == MovementState.Skiing)
                PerformSkiMovement(data);
            
            else if (data.State == MovementState.WallRunning && data.JumpPressed)
                WallJump();

            else if (data.State == MovementState.WallRunning)
                PerformWallRunMovement();
            else
                MovePlayer(data);

            if (_isGrounded && data.JumpPressed) // Jump
                Jump();
            
            if (IsServer && LagCompensationManager.Instance != null)
            {
                LagCompensationManager.Instance.RecordSnapshot(_netObj, firePoint.position, firePoint.forward, _rb.linearVelocity, TimeManager.Tick);
            }
            
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

            cameraTransform.localEulerAngles = new Vector3(_currentPitch, 0f, 0f);
        }
        #endregion

        #region Jump

        private void Jump()
        {
            Physics.gravity = new Vector3(0, -30f, 0); // Can't determine a better place to put this - protective measure
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
                if (_state == MovementState.WallRunning) 
                    StopWallRun();
                
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
                _moveSpeed = walkSpeed; // Apply walk speed if walking
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

            if (_onSlope && data.State != MovementState.Skiing && data.State != MovementState.Jetpacking && _rb.linearVelocity.magnitude <= _moveSpeed)
            {
                Vector3 slopeMoveDir = Vector3.ProjectOnPlane(_moveDirection, _slopeHit.normal).normalized * _moveSpeed;
                _predictionRb.Velocity(new Vector3(slopeMoveDir.x, currentVelocity.y, slopeMoveDir.z));
            }
            else if (_isGrounded && _rb.linearVelocity.magnitude <= _moveSpeed)
            {
                // Normal movement on flat ground
                _predictionRb.Velocity(new Vector3(_moveDirection.x * _moveSpeed, _rb.linearVelocity.y, _moveDirection.z * _moveSpeed));
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
        
        private void ControlEnv(MovementData data)
        {
            float drag;

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
                    Physics.gravity = -_storedWallNormal * 200f; // Alter gravity to follow the wallrun normal to stick player to surface
                    drag = airDrag;
                    break;
                default:  // ground state
                    Physics.gravity = new Vector3(0, -30f, 0);
                    if (_onSlope)
                        Physics.gravity = -_slopeHit.normal * 45f;
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
                    HandleWallLoss(data);
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
            if (data.State == MovementState.WallRunning)
            {
                _wallRunGraceTimer -= (float)base.TimeManager.TickDelta;
                
                if (_wallRunGraceTimer <= 0f)
                {
                    _exitingWall = true;
                    _exitWallTimer = exitWallTime;
                    StopWallRun();  // DG - consider adding _wallRunGraceTimer = wallRunGraceTime;
                }
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
            
            _storedWallNormal = _wallRunNormal;
            
            Vector3 playerVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
            float speed = playerVelocity.magnitude;
            
            // Initialize _currentWallRunSpeed to the player's initial speed
            _currentWallRunSpeed = speed;
            
            if (speed < slowWallRunThreshold)      _targetWallRunSpeed = targetWallRunSpeedSlow;
            else if (speed < midWallRunThreshold) _targetWallRunSpeed = targetWallRunSpeedMid;
            else                                   _targetWallRunSpeed = targetWallRunSpeedFast;
            
            // Use orientation.forward as the base direction, projected onto the wall plane
            Vector3 intendedWallRunDir = Vector3.ProjectOnPlane(orientation.forward, _storedWallNormal).normalized;
            
            // Critical fix: Check velocity alignment with intended direction
            if (Vector3.Dot(playerVelocity, intendedWallRunDir) < 0f)
            {
                // Player velocity opposite intended direction, flip direction
                intendedWallRunDir = -intendedWallRunDir;
            }

            _wallRunDirection = intendedWallRunDir;
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
                        StopWallRun();
                    }
                }
                else if (_exitingWall)
                {
                    //Debug.Log($"_exitWallTimer: {_exitWallTimer}");
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
            // Check if we're still on the current wall
            Vector3 currentWallNormal;
            bool onCurrentWall = IsStillOnWall(out currentWallNormal);

            if (!onCurrentWall)
            {
                if (currentWallNormal != Vector3.zero)
                {
                    // We've transitioned to a new wall (normals differ); update the normal and direction
                    Vector3 previousWallRunDirection = _wallRunDirection; // Store the previous direction for comparison
                    _storedWallNormal = currentWallNormal;

                    // Update _wallRunDirection to align with the new wall's plane
                    Vector3 newWallRunDirection = Vector3.ProjectOnPlane(previousWallRunDirection, currentWallNormal).normalized;

                    // Ensure the new direction continues "forward" relative to the previous direction
                    if (Vector3.Dot(newWallRunDirection, previousWallRunDirection) < 0)
                    {
                        newWallRunDirection = -newWallRunDirection; // Flip to keep moving forward
                    }

                    // Update _wallRunDirection (no interpolation, ensure normalized)
                    _wallRunDirection = newWallRunDirection.normalized;

                    // Reset the grace timer since we found a new wall
                    _wallRunGraceTimer = wallRunGraceTime;
                }
                else
                {
                    // No wall contact at all; use grace timer before stopping
                    _wallRunGraceTimer -= (float)base.TimeManager.TickDelta;
                    if (_wallRunGraceTimer <= 0f)
                    {
                        StopWallRun();
                        return;
                    }
                }
            }
            else
            {
                // Still on the current wall; reset grace timer
                _wallRunGraceTimer = wallRunGraceTime;
            }

            // Lerp _currentWallRunSpeed toward _targetWallRunSpeed
            _currentWallRunSpeed = Mathf.Lerp(_currentWallRunSpeed, _targetWallRunSpeed, wallRunLerpSpeed * (float)TimeManager.TickDelta);

            // Apply the velocity using _currentWallRunSpeed
            Vector3 desiredVel = _wallRunDirection * _currentWallRunSpeed;
            _predictionRb.Velocity(new Vector3(desiredVel.x, 0f, desiredVel.z));
        }
        
        private bool IsStillOnWall(out Vector3 currentWallNormal)
        {
            currentWallNormal = Vector3.zero;
            if (Physics.Raycast(transform.position, -_storedWallNormal, out RaycastHit hit, wallCheckDistance, whatIsWall))
            {
                currentWallNormal = hit.normal;
                return currentWallNormal == _storedWallNormal;
            }
            return false;
        }

        private void StopWallRun()
        {
            _exitingWall   = false;
            _wallLeft      = false;
            _wallRight     = false;
            _canWallRun    = false;
            _storedWallNormal = Vector3.zero;
            _wallRunNormal = Vector3.zero;
            _wallRunDirection = Vector3.zero;
            _state = MovementState.Airborne;
            _currentWallRunSpeed = 0f; // Reset current speed
            _wallRunGraceTimer = 0f; // critical reset
            _exitWallTimer = 0f;     // critical reset
        }

        private void WallJump()
        {
            _exitingWall   = true;
            _exitWallTimer = exitWallTime;
            
            Vector3 forceToApply = (Vector3.up * wallJumpUpForce) + (_storedWallNormal * wallJumpSideForce);
            
            StopWallRun();
            _predictionRb.AddForce(forceToApply, ForceMode.Impulse);
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
            Vector3 rawInput = new Vector3(data.Horizontal, 0f, data.Vertical);
            Vector3 horizontalDirection = rawInput.sqrMagnitude > 0.01f?orientation.TransformDirection(rawInput).normalized:Vector3.zero;
            
            // Apply vertical lift
            Vector3 finalImpulse = Vector3.up * (jetpackForce * 0.7f); 
            
            Vector3 currentHorizVel = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
          
            // Apply horizontal thrust **only if below speed limits**
            if (currentHorizVel.magnitude <= maxAdditionalForwardSpeed)
                finalImpulse += horizontalDirection * (jetpackForce * 0.3f);
            
            // Apply force without normalization to preserve intended thrust balance
            _predictionRb.AddForce(finalImpulse, ForceMode.Impulse);
        }
        
        #endregion

        #region Skiing

        private void PerformSkiMovement(MovementData data)
        {
            //if (!_isGrounded)
              //  return;
            /*
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
            */
        }

        #endregion
        
        #region Knock‑back
        // Called from BaseProjectile via RPC on every client *and* the server.
        public void ReceiveKnockback(Vector3 impulse, float tempDrag = -1f)
        {
            if (_state == MovementState.WallRunning)
                StopWallRun();
            
            if (IsServer)
            {
                _pendingKnockback = impulse;
                if (tempDrag >= 0f) _pendingTempDrag = tempDrag;   // new helper field
            }
            else
            {
                if (tempDrag >= 0f) _rb.linearDamping = tempDrag;
                _predictionRb.AddForce(impulse, ForceMode.Impulse);
            }
        }

        private void ApplyKnockback()
        {
            float oldDrag = _rb.linearDamping;
            if (_pendingTempDrag.HasValue)
                _rb.linearDamping = _pendingTempDrag.Value;

            _predictionRb.AddForce(_pendingKnockback.Value, ForceMode.Impulse);

            _rb.linearDamping  = oldDrag;          // restore
            _pendingKnockback  = null;
            _pendingTempDrag   = null;
                
            _state = MovementState.Airborne;
        }
        #endregion
    }
}
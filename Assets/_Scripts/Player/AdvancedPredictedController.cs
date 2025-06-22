using System.Runtime.CompilerServices;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;
using FishNet.Object.Synchronizing;
using Cmd = _Scripts.Player.InputCmd;   // short-hand
using _Scripts.Packs;


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
        [SerializeField] private Transform headAnchor;
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
        [SerializeField] private float slopeCheckDistance = 1.2f;
        [SerializeField] private LayerMask groundMask;
        
        [Header("Ground Check")]
        [SerializeField] private Vector3 feetOffset = new Vector3(0f, -1f, 0f);
        [SerializeField] private float feetRadius = 0.5f;
        
        [Header("Wall Running")]
        [SerializeField] private LayerMask whatIsWall;
        [SerializeField] private float wallJumpUpForce = 8f;
        [SerializeField] private float wallJumpSideForce = 6f;
        [SerializeField] private float targetWallRunSpeedSlow = 15f;
        [SerializeField] private float targetWallRunSpeedMid = 25f;
        [SerializeField] private float targetWallRunSpeedFast = 40f;
        [SerializeField] private float maxWallRunTime = 1.5f;
        [SerializeField] private float wallCheckDistance = 0.6f;
        [SerializeField] private float minJumpHeight = 1.2f;
        [SerializeField] private float exitWallTime = 0.2f;
        [SerializeField] private float wallRunGraceTime = 0.2f;
        [Header("Wall Running (Forces)")]
        [SerializeField] private float wallStickForce = 50f;         // Force pulling player towards the wall
        [SerializeField] private float wallRunAcceleration = 20f;  // How quickly player reaches target wall run speed
        [SerializeField] private float wallRunVerticalDampFactor = 10f; // How strongly to suppress vertical movement

        [Header("Jetpack Settings")]
        [SerializeField] private float jetpackForce = 15f;
        [SerializeField] private float jetpackFuelBurnRate = 10f;
        [SerializeField] private float jetpackFuelCutoff = 5f;
        [SerializeField] private float jetpackDirectionalBlend = 0.3f;
        [SerializeField] private float maxAdditionalForwardSpeed = 30f;
        [SerializeField] private float maxAdditionalLateralSpeed = 40f;

        [Header("Skiing Settings")]
        [SerializeField] private float skiControl = 0.1f;
        [SerializeField] private float skiDrag = 0.1f;
        
        [Header("Energy Settings")]
        [SerializeField] private float maxEnergy = 100f;
        [SerializeField] private float energyRegenRate = 5f;

        [Header("Debug / State Info")]
        [SerializeField] private MovementState _state;

        private MovementState _previousState;
        public MovementState State => _state;
        
        public Transform HeadAnchor => headAnchor;
        public float CurrentPitch => _currentPitch;
        
        // SyncVar for soft death
        readonly SyncVar<bool> _isFrozen = new(false);
        
        public bool IsFrozen
        {
            get => _isFrozen.Value;
            set => _isFrozen.Value = value;   // call from the **server** only
        }

        #endregion

        #region Internals
        
        private Cmd _cmd; // Cache the command we are simulating this tick.
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool Btn(InputButtons mask, InputButtons flag) => (mask &  flag) != 0; // Bit-flag helper (keeps expressions readable).
        
        private NetworkObject _netObj;
        private Rigidbody _rb;
        private PredictionRigidbody _predictionRb;

        private float _moveSpeed;
        private float _startYScale;
        private bool _isGrounded;
        private bool _onSlope;
        private RaycastHit _groundHit;
        private RaycastHit _slopeHit;
        
        // Input
        private InputHandler _iH;
        
        // Pack Manager
        private PackManager _packMgr;
        
        // Movement vector
        private Vector3 _moveDirection;
        
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
        private float _currentWallRunSpeed; // Current speed for wall-running (maintained separately)
        
        private bool _isJetpacking;
        
        // Energy
        private float _energy;
        private float _burn;
        
        // Public helpers
        public float Energy     => _energy;
        public bool ShieldActive => _packMgr && _packMgr.Active && _packMgr.CurrentId == PackId.Shield;
        
        // Knockback
        private Vector3? _pendingKnockback;
        private float? _pendingTempDrag;

        // Player layers
        private const string LOCAL_LAYER   = "LocalPlayer";
        private const string REMOTE_LAYER  = "RemotePlayer";

        #endregion

        #region FishNet Data Structures
        // Replication struct found in InputCmd
        private struct ReconciliationData : IReconcileData
        {
            private uint _tick;

            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 LinearVelocity;
            public MovementState State;
            public float CurrentPitch;
            public float Energy;
            public float Drag;

            public ReconciliationData(Vector3 position, Quaternion rotation, Vector3 linearVelocity, MovementState state, 
                float currentPitch, float currentEnergy, float currentDrag)
            {
                _tick = 0;
                Position           = position;
                Rotation           = rotation;
                LinearVelocity     = linearVelocity;
                State              = state;
                CurrentPitch       = currentPitch;
                Energy             = currentEnergy;
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
            
            _packMgr = GetComponent<PackManager>();
            
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
            {
                _iH = GetComponent<InputHandler>();
                
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible   = false;
                
                FpsCameraFollow cam = Camera.main?.GetComponent<FpsCameraFollow>();
                if (cam != null) cam.SetTarget(this);
            }
        }

        public override void OnStartServer()
        {
            int remote = LayerMask.NameToLayer(REMOTE_LAYER);
            SetLayerRecursively(gameObject, remote);
            _energy = maxEnergy;
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
                // Pack them into MovementData
                _cmd = _iH.CmdRing.Get(TimeManager.Tick);   // ← store once
                Replicate(_cmd);
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
                _state,
                _currentPitch,
                _energy,
                _rb.linearDamping
            );
 
            Reconcile(recData);
        }

        [Reconcile]
        private void Reconcile(ReconciliationData data, Channel channel = Channel.Unreliable)
        {
            _rb.Move(data.Position, data.Rotation);
            _rb.linearVelocity = data.LinearVelocity;
            _rb.linearDamping = data.Drag;
            
            // Correct movement state
            _state = data.State;

            _currentPitch = data.CurrentPitch;
            
            _energy = data.Energy;
        }

        #endregion

        #region Replicate

        [Replicate]
        private void Replicate(Cmd cmd, ReplicateState state = ReplicateState.Invalid, Channel channel = Channel.Unreliable)
        {
            if (IsFrozen)
                return;
            
            float dt = (float)TimeManager.TickDelta;
            _cmd = cmd;
            
            RegenEnergy(dt); // Regen Energy first

            PackLogic(dt); // Check pack logic
            
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
            ApplyRotation(cmd.look.x, cmd.look.y);
            
            // Check for wall
            if (Btn(cmd.buttons, InputButtons.WallRun) && _state != MovementState.WallRunning)
                CheckForWall();

            // Update movement states
            UpdateMovementState();
                
            if (_state == MovementState.WallRunning) // Only update wall run state if actively wall running
                UpdateWallRunState();
            
            ControlEnv();
            
            switch (_state)
            {
                case MovementState.Jetpacking:
                    if (SpendEnergy(_burn * dt))
                    {
                        if (!_isGrounded) MovePlayer();   // optional air-control
                        Jetpack();                // apply impulses
                    }
                    else
                    {
                        _state = MovementState.Airborne;  // pack sputters out
                    }
                    break;

                case MovementState.Skiing:
                    PerformSkiMovement();
                    break;

                case MovementState.WallRunning:
                    if (Btn(cmd.buttons, InputButtons.Jump)) WallJump();
                    else PerformWallRunMovement();
                    break;

                default:
                    MovePlayer();
                    break;
            }

            if (_isGrounded && Btn(cmd.buttons, InputButtons.Jump))
                Jump();
            
            _predictionRb.Simulate();
            
            if (IsServer && LagCompensationManager.Instance != null)
            {
                LagCompensationManager.Instance.RecordSnapshot(_netObj, firePoint.position, firePoint.forward, _rb.linearVelocity, TimeManager.Tick);
            }
        }
        #endregion
        
        #region Rotation

        private void ApplyRotation(float yawDeltaRaw, float pitchDeltaRaw)
        {
            /* YAW ––– rotate the rigidbody ––––––––––––––––––––––––––––––– */
            float yawDelta = yawDeltaRaw * yawSensitivity;
            _rb.MoveRotation(_rb.rotation * Quaternion.Euler(0f, yawDelta, 0f));

            /* PITCH – just track a number; camera uses it every render frame */
            _currentPitch = Mathf.Clamp(_currentPitch - pitchDeltaRaw * pitchSensitivity, minPitch, maxPitch);

            headAnchor.localEulerAngles = new Vector3(_currentPitch, 0f, 0f); //was cameraTransform.
        }

        #endregion
        
        #region Pack Logic
        private void PackLogic(float dt)
        {
            _burn = jetpackFuelBurnRate; // No pack - default energy
            
            if (_packMgr && _packMgr.CurrentId == PackId.Energy)
                _burn = jetpackFuelBurnRate - _packMgr.CurrentDef.extraRegenPerSec;
            
            if (_packMgr && _packMgr.Active && _packMgr.CurrentId == PackId.Shield)
            {
                float shieldActiveDrain = _packMgr.CurrentDef.shieldDrainPerSec * dt;

                if (!SpendEnergy(shieldActiveDrain) && IsServer)
                    _packMgr.ForceActive(false);
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
    
        private void UpdateMovementState()
        {
            if (Btn(_cmd.buttons, InputButtons.Jetpack))
            {
                /* Already jet-packing? → keep going, Not jet-packing yet? → need at least jetpackFuelCutoff to ignite. */
                if (_state == MovementState.Jetpacking ? _energy > 0f : _energy >= jetpackFuelCutoff)
                {
                    // If we started jet-packing this frame, stop wall-run etc.
                    if (_state != MovementState.Jetpacking && _state == MovementState.WallRunning)
                        StopWallRun();

                    _state = MovementState.Jetpacking;
                    return;                     // jet-pack overrides all other states
                }
            }

            // If not already wall running, check for wall run initiation using the wall run key.
            if (_state != MovementState.WallRunning)
            {
                _canWallRun = CanWallRun();
                if (Btn(_cmd.buttons, InputButtons.WallRun) && _canWallRun)
                {
                    StartWallRun();
                    _state = MovementState.WallRunning;
                }
                
                else if (Btn(_cmd.buttons, InputButtons.Ski))
                    _state = MovementState.Skiing;
                
                else if (_isGrounded)
                {
                    if (Btn(_cmd.buttons, InputButtons.Crouch))
                    {
                        _state = MovementState.Crouching;
                    }
                    else if (Btn(_cmd.buttons, InputButtons.Sprint))
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

// --------------- MOVE PLAYER -----------------------------------------------        
        private void MovePlayer()
        {
            transform.localScale = new Vector3(transform.localScale.x, _startYScale, transform.localScale.z); // reset crouch
            
            if (_state == MovementState.Walking) _moveSpeed = walkSpeed; // Apply walk speed if walking
            else if (_state == MovementState.Sprinting) _moveSpeed = sprintSpeed; // Apply sprint speed if sprinting
            else if (_state == MovementState.Crouching)
            {
                transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
                _moveSpeed = crouchSpeed; // Apply crouch speed if crouching
            }

            // Calculate move direction
            _moveDirection = (orientation.forward * _cmd.move.y) + (orientation.right * _cmd.move.x);
            _moveDirection.Normalize();
            
            Vector3 currentVelocity = _rb.linearVelocity;

            if (_onSlope && _state != MovementState.Skiing && _state != MovementState.Jetpacking && _rb.linearVelocity.magnitude <= _moveSpeed)
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
        
        private void ControlEnv()
        {
            float drag;

            switch (_state)
            {
                case MovementState.Airborne or MovementState.Jetpacking:
                    drag = airDrag;
                    break;
                case MovementState.Skiing:
                    drag = skiDrag;
                    break;
                case MovementState.WallRunning:
                    drag = airDrag;
                    break;
                default:  // ground state
                    drag = groundDrag;
                    break;
            }
            _rb.linearDamping = drag;
        }

        #endregion

        #region Wall Run

        private void CheckForWall()
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
                    HandleWallLoss();
                }
            }
            else
            {
                _wallRunNormal = Vector3.zero; // Reset wall normal
                HandleWallLoss();
            }
        }

        private void HandleWallLoss()
        {
            if (_state == MovementState.WallRunning)
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
            
            if (speed < targetWallRunSpeedMid)      _targetWallRunSpeed = targetWallRunSpeedSlow;
            else if (speed < targetWallRunSpeedFast) _targetWallRunSpeed = targetWallRunSpeedMid;
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

        private void UpdateWallRunState()
        {
            if (_state == MovementState.WallRunning)
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
                    if (_exitWallTimer > 0f)
                        _exitWallTimer -= (float)base.TimeManager.TickDelta;
                    else
                    {
                        _exitingWall = false;
                        StopWallRun();
                    }
                }
                else
                {
                    StopWallRun();
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
            
            // 1. STICKING FORCE: Pulls player towards the wall, _storedWallNormal is the normal of the wall we are currently running on.
            if (_storedWallNormal != Vector3.zero)
            {
                _predictionRb.AddForce(-_storedWallNormal * wallStickForce, ForceMode.Force);
            }

            // 2. COUNTERACT GLOBAL GRAVITY
            _predictionRb.AddForce(-Physics.gravity, ForceMode.Acceleration);

            // 3. MOVEMENT FORCE ALONG THE WALL (Horizontally), _wallRunDirection is already calculated in StartWallRun to be along the wall's surface.
            if (_wallRunDirection != Vector3.zero)
            {
                Vector3 horizontalWallRunDir = _wallRunDirection;
                horizontalWallRunDir.y = 0f; // Ensure the target movement direction is purely horizontal

                if (horizontalWallRunDir.sqrMagnitude > 0.001f) // Avoid normalizing a zero vector
                {
                    horizontalWallRunDir.Normalize();

                    // Calculate current speed in the desired horizontal wall run direction
                    float currentSpeedInDir = Vector3.Dot(_rb.linearVelocity, horizontalWallRunDir);

                    // Calculate force to accelerate/decelerate to _targetWallRunSpeed
                    float speedError = _targetWallRunSpeed - currentSpeedInDir;
                    Vector3 movementForce = horizontalWallRunDir * (speedError * wallRunAcceleration);

                    _predictionRb.AddForce(movementForce, ForceMode.Acceleration); // Mass independent
                }
            }
            // 4. VERTICAL STABILIZATION FORCE: Keep player from moving up/down - This actively dampens any existing vertical velocity.
            float currentVerticalVelocity = _rb.linearVelocity.y;
            Vector3 verticalDampingForce = Vector3.down * (currentVerticalVelocity * wallRunVerticalDampFactor);
            _predictionRb.AddForce(verticalDampingForce, ForceMode.Acceleration); 
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
            _currentWallRunSpeed = 0f;
            _wallRunGraceTimer = 0f;
            _exitWallTimer = 0f;
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
        
        private void Jetpack()
        {
            Vector3 rawInput = new Vector3(_cmd.move.x, 0f, _cmd.move.y);
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

        private void PerformSkiMovement()
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
        
        #region respawn
        public void HardResetMovement()
        {
            // 1) clear physics
            _rb.Move(_rb.position, Quaternion.identity);
            _rb.linearVelocity    = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;

            // 4) misc state
            _state              = MovementState.Airborne;
            _pendingKnockback   = null;
            _onSlope            = false;
            _currentPitch       = 0f;
        }
        
        public void ResetEnergy()
        {
            _energy = maxEnergy;
        }
        #endregion
        
        #region energy
        bool SpendEnergy(float amount)
        {
            if (_energy <= 0.17f)
                return false;                      // totally empty

            float consumed = Mathf.Min(amount, _energy);
            _energy -= consumed;                   // burn what we have
            return true;                           // there *was* energy this tick
        }


        void RegenEnergy(float dt)
        {
            if (_energy < maxEnergy)
                _energy = Mathf.Min(maxEnergy, _energy + energyRegenRate * dt);
        }
        
        /*  Server-side shield absorb helper                                  */
        [Server] public int AbsorbDamageWithShield(int incoming)
        {
            // shield inactive? -> nothing absorbed
            if (!(_packMgr && _packMgr.Active && _packMgr.CurrentId == PackId.Shield))
                return incoming;

            // How much can we pay?
            int absorb = Mathf.Min(incoming, Mathf.CeilToInt(_energy));

            // Burn that energy
            _energy -= absorb;

            // Drop shield immediately if empty
            if (_energy <= 0f) _packMgr.ForceActive(false);

            // Return un-absorbed remainder (may be zero)
            return incoming - absorb;
        }

        #endregion
    }
}
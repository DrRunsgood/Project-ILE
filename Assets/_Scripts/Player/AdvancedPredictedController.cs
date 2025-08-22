using System.Runtime.CompilerServices;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;
using FishNet.Object.Synchronizing;
using _Scripts.Packs;

namespace _Scripts.Player
{
    public static class NetUtils
    {
        public static class MoveCodec
        {
            // 00 = 0   01 = +1   10 = –1
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static byte Pack(float x, float z)
            {
                byte b = 0;
                if      (x >  0.1f) b |= 0b01;        // bits 0-1
                else if (x < -0.1f) b |= 0b10;
                if      (z >  0.1f) b |= 0b01 << 2;   // bits 2-3
                else if (z < -0.1f) b |= 0b10 << 2;
                return b;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Vector2 Unpack(byte b)
            {
                int dx = (b & 0b11)     switch
                {
                    0b01 => +1, 0b10 => -1, _ => 0
                };
                int dz = ((b >> 2) & 0b11) switch
                {
                    0b01 => +1, 0b10 => -1, _ => 0
                };
                return new Vector2(dx, dz);
            }
        }
    }
    
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
        [SerializeField] private Transform viewOrigin;
        public Transform ViewOrigin => viewOrigin;

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
        
        [Header("Physics Materials")]
        [SerializeField] private PhysicsMaterial gripMat;
        [SerializeField] private PhysicsMaterial skiMat;
        
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
        [SerializeField] private float jetpackForce = 42f;
        [SerializeField] private float jetpackFuelBurnRate = 10f;
        [SerializeField] private float jetpackFuelCutoff = 5f;
        [SerializeField] private float jetUpliftRatio = 0.75f;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool Btn(InputButtons mask, InputButtons flag) => (mask &  flag) != 0; // Bit-flag helper (keeps expressions readable).
        
        private NetworkObject _netObj;
        private Rigidbody _rb;
        private PredictionRigidbody _predictionRb;
        Collider _col;

        private float _moveSpeed;
        private float _startYScale;
        private bool _isGrounded;
        private bool _onSlope;
        private RaycastHit _slopeHit;
        
        // Input
        private InputHandler _iH;
        
        // Pack Manager
        private PackManager _packMgr;
        
        // Movement vector
        private Vector3 _moveDirection;
        
        // For camera orientation
        private float _currentPitch; // we clamp pitch with minPitch, maxPitch
        private float _yaw;

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
        
        // Energy
        private float _energy;
        private float _burn;
        
        // Public helpers
        public float Energy     => _energy;
        public bool ShieldActive => _packMgr && _packMgr.Active && _packMgr.CurrentId == PackId.Shield;
        
        // Knockback
        private Vector3? _pendingKnockback;
        private float? _pendingTempDrag;
        private bool _heartBeat;  // Used to mark replicate packet dirty when receiving knockback to keep kb responsive but keep network efficienies

        // MovementData
        private MovementData _md;
        
        #endregion

        #region FishNet Data Structures
        // Replication struct
        
        #region helpers – 2-bit Move encoder
        
        #endregion

        #region replicate payload -------------------------------------------
        public struct MovementData : IReplicateData
        {
            public byte Heart;
            /* packed fields */
            private uint _tick;                     // Fish-Net needs this
            public  byte MoveXZ;                    // 4 bits
            public  short LookX, LookY;             // deltas
            public  InputButtons Held;              // held-down flags
            public  InputButtons Down;              // went-down-this-frame flags

            /* ctor helper you will call from OnTick() */
            public MovementData(uint tick, Vector2 move, Vector2 look, InputButtons held, InputButtons down, bool heart)
            {
                _tick   = tick;
                MoveXZ  = NetUtils.MoveCodec.Pack(move.x, move.y);
                LookX = (short)Mathf.Clamp(Mathf.RoundToInt(look.x * 100f), short.MinValue, short.MaxValue);
                LookY = (short)Mathf.Clamp(Mathf.RoundToInt(look.y * 100f), short.MinValue, short.MaxValue);
                Held    = held;
                Down    = down;
                Heart  = (byte)(heart ? 1 : 0);
            }

            /* IReplicateData boiler-plate */
            public uint  GetTick()            => _tick;
            public void  SetTick(uint value)  => _tick = value;
            public void  Dispose()            { }
        }
        #endregion
        
        private struct ReconciliationData : IReconcileData
        {
            uint _tick;
            public Vector3  Position;
            public ushort   Speed, Heading;
            public short    Vy;
            public byte     EnergyQ;
            public MovementState State;
            public ushort   YawQ;
            public ushort    CurrentPitchQ;

            public ReconciliationData(Vector3 pos, Vector3 vel, MovementState st, float yawDeg, float pitchDeg, byte energyQ)
            {
                _tick = 0;
                Position  = pos;
                EncodeVelocity(vel, out Speed, out Heading, out Vy);
                EnergyQ   = energyQ;          // just store
                State     = st;
                YawQ = (ushort)Mathf.Clamp(Mathf.RoundToInt(((yawDeg % 360f + 360f) % 360f) * (65535f / 360f)), 0, 65535);
                CurrentPitchQ = QuantizePitch(pitchDeg);
            }
            public uint  GetTick() => _tick;
            public void  SetTick(uint v) => _tick = v;
            public void  Dispose() { }
        }
        
        #endregion

        #region Network Events
        public override void OnStartNetwork()
        {
            base.OnStartNetwork();

            _rb = GetComponent<Rigidbody>();
            _netObj = GetComponent<NetworkObject>();
            _predictionRb = new PredictionRigidbody();
            _predictionRb.Initialize(_rb);
            _packMgr = GetComponent<PackManager>();
            _col = GetComponent<Collider>();
            
            _startYScale = transform.localScale.y;
            _yaw = transform.eulerAngles.y;
            
            TimeManager.OnTick += OnTick;
            TimeManager.OnPostTick += OnPostTick;
        }
        
        public override void OnStartClient()
        {
            base.OnStartClient();
                
            if (IsOwner)
            {
                _iH = GetComponent<InputHandler>();
                
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible   = false;
                
                FpsCameraFollow cam = FindFirstObjectByType<FpsCameraFollow>();
                if (cam != null) cam.SetTarget(this);

                SetPhysicMaterial(gripMat);
            }
        }

        public override void OnStartServer()
        {
            _energy = maxEnergy;
            SetPhysicMaterial(gripMat);
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
                var ih = _iH;
                _md = new MovementData(TimeManager.Tick, ih.Move, ih.Look, ih.HeldButtons, ih.DownButtons, _heartBeat);

                Replicate(_md);
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
                _rb.linearVelocity,
                _state,
                _yaw,
                _currentPitch,
                QuantiseEnergy(_energy)
            );
 
            Reconcile(recData);
        }

        [Reconcile]
        private void Reconcile(ReconciliationData data, Channel channel = Channel.Unreliable)
        {
            _rb.Move(data.Position, Quaternion.Euler(0f, data.YawQ * (360f / 65535f), 0f));
            _rb.linearVelocity = DecodeVelocity(data.Speed, data.Heading, data.Vy);

            // Correct movement state
            _state = data.State;
            _currentPitch = DequantizePitch(data.CurrentPitchQ);
            _yaw = data.YawQ * (360f / 65535f);
            _energy = DequantiseEnergy(data.EnergyQ);
        }
        #endregion

        #region Replicate

        [Replicate]
        private void Replicate(MovementData md, ReplicateState state = ReplicateState.Invalid, Channel channel = Channel.Unreliable)
        {
            if (IsFrozen) return;
            
            float dt = (float)TimeManager.TickDelta;
            
            Decompress(md, out Vector2 move, out Vector2 look, out InputButtons held, out InputButtons down);
            
            RegenEnergy(dt); // Regen Energy first

            PackLogic(dt); // Check pack logic
            
            if (_pendingKnockback.HasValue) // Check incoming Knockback
            {
                ApplyKnockback();
                _predictionRb.Simulate();
                return;
            }
            
            _isGrounded = IsGrounded();  // Ground check
            
            _onSlope = false; // Reset slope check, recheck if on ground
            if(_isGrounded) _onSlope = OnSlope();
            
            ApplyRotation(look.x, look.y);
            
            // Check for wall
            if (Btn(held, InputButtons.WallRun) && _state != MovementState.WallRunning)
                CheckForWall();

            // Update movement states
            UpdateMovementState(down, held);
                
            if (_state == MovementState.WallRunning) // Only update wall run state if actively wall running
                UpdateWallRunState();
            
            ControlEnv();
            
            switch (_state)
            {
                case MovementState.Jetpacking:
                    ApplyAirControl(move); // optional air-control

                    if (SpendEnergy(_burn * dt))
                    {
                        Jetpack(move); // apply impulses
                    }
                    else
                    {
                        _state = MovementState.Airborne;  // pack sputters out
                    }
                    break;
                
                case MovementState.Airborne:
                    ApplyAirControl(move); // Use the same air control helper
                    break;

                case MovementState.Skiing:
                    PerformSkiMovement(move);
                    break;

                case MovementState.WallRunning:
                    if (Btn(down, InputButtons.Jump)) WallJump();
                    else PerformWallRunMovement();
                    break;

                default:
                    MovePlayer(move);
                    break;
            }

            if (_isGrounded && Btn(held, InputButtons.Jump))
                Jump();
            
            _predictionRb.Simulate();
            
            if (viewOrigin != null && headAnchor != null)
            {
                viewOrigin.position = headAnchor.position;
                viewOrigin.rotation = headAnchor.rotation;
            }
            
            if (IsServer && LagCompensationManager.Instance != null) 
                LagCompensationManager.Instance.RecordSnapshot(_netObj, viewOrigin.position, viewOrigin.forward, _rb.linearVelocity, TimeManager.Tick);
        }
        #endregion

        #region Quantization and Decompression
        static void Decompress(in MovementData md, out Vector2 move, out Vector2 look, out InputButtons held, out InputButtons down)
        {
            move = NetUtils.MoveCodec.Unpack(md.MoveXZ);
            look = new Vector2(md.LookX * 0.01f, md.LookY * 0.01f);
            held = md.Held;
            down = md.Down;
        }
        
        // ─── Velocity polar helpers ───────────────────────────────
        static void EncodeVelocity(Vector3 v, out ushort speed, out ushort heading, out short vy)
        {
            float horiz = new Vector2(v.x, v.z).magnitude;
            speed = (ushort)Mathf.Clamp(Mathf.RoundToInt(horiz * 100f), 0, 65535);
            heading = (ushort)Mathf.Clamp(
                Mathf.RoundToInt(((Mathf.Atan2(v.x, v.z) * Mathf.Rad2Deg + 360f) % 360f) * 182.04444f), 0, 65535); // 65535 / 360 = 182.04444
            vy = (short)Mathf.Clamp(Mathf.RoundToInt(v.y * 100f), short.MinValue, short.MaxValue);
        }
        
        static Vector3 DecodeVelocity(ushort speed, ushort heading, short vy)
        {
            float horizSpeed = speed * 0.01f;
            float ang        = heading * (360f / 65535f) * Mathf.Deg2Rad;
            Vector3 horiz    = new Vector3(Mathf.Sin(ang), 0f, Mathf.Cos(ang)) * horizSpeed;
            return new Vector3(horiz.x, vy * 0.01f, horiz.z);
        }

        // ─── Energy quantiser ─────────────────────────────────────
        byte QuantiseEnergy(float e) => (byte)Mathf.Clamp(Mathf.RoundToInt(e / maxEnergy * 255f), 0, 255);
        float DequantiseEnergy(byte b) => b / 255f * maxEnergy;
        
        static ushort QuantizePitch(float pitch)
        {
            float n = Mathf.InverseLerp(-90f, 90f, Mathf.Clamp(pitch, -90f, 90f));
            return (ushort)Mathf.Clamp(Mathf.RoundToInt(n * 65535f), 0, 65535);
        }

        static float DequantizePitch(ushort q)
        {
            float n = q / 65535f;
            return Mathf.Lerp(-90f, 90f, n);
        }
        
        #endregion
        
        #region Pack Logic
        private void PackLogic(float dt)
        {
            _burn = jetpackFuelBurnRate; // No pack - default energy
            
            if (_packMgr == null || _packMgr.CurrentId == PackId.None)
                return; 
            
            if (_packMgr.CurrentId == PackId.Energy)
                _burn = jetpackFuelBurnRate - _packMgr.CurrentDef.extraRegenPerSec;
            
            if (_packMgr.Active && _packMgr.CurrentId == PackId.Shield)
            {
                float shieldActiveDrain = _packMgr.CurrentDef.shieldDrainPerSec * dt;

                if (!SpendEnergy(shieldActiveDrain) && IsServer)
                    _packMgr.ForceActive(false);
            }
        }
        #endregion
        
        #region Rotation

        private void ApplyRotation(float yawDeltaRaw, float pitchDeltaRaw)
        {
            
            _yaw += yawDeltaRaw * yawSensitivity;
            _rb.MoveRotation(Quaternion.Euler(0f, _yaw, 0f));

            // PITCH – just track a number; camera uses it every render frame 
            _currentPitch = Mathf.Clamp(_currentPitch - pitchDeltaRaw * pitchSensitivity, minPitch, maxPitch);

            headAnchor.localEulerAngles = new Vector3(_currentPitch, 0f, 0f);
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
    
        private void UpdateMovementState(InputButtons down, InputButtons held)
        {
            if (Btn(held, InputButtons.Jetpack))
            {
                /* Already jet-packing? → keep going, Not jet-packing yet? → need at least jetpackFuelCutoff to ignite. */
                if (_state == MovementState.Jetpacking ? _energy > 0f : _energy >= jetpackFuelCutoff)
                {
                    // If we started jet-packing this frame, stop wall-run etc.
                    if (_state != MovementState.Jetpacking && _state == MovementState.WallRunning)
                        StopWallRun();

                    _state = MovementState.Jetpacking;
                    return;                     // jetpack overrides all other states
                }
            }

            // If not already wall running, check for wall run initiation using the wall run key.
            if (_state != MovementState.WallRunning)
            {
                _canWallRun = CanWallRun();
                if (Btn(down, InputButtons.WallRun) && _canWallRun)
                {
                    StartWallRun();
                    _state = MovementState.WallRunning;
                }
                
                else if (Btn(held, InputButtons.Ski))
                    _state = MovementState.Skiing;
                
                else if (_isGrounded)
                {
                    if (Btn(held, InputButtons.Crouch))
                    {
                        _state = MovementState.Crouching;
                    }
                    else if (Btn(held, InputButtons.Sprint))
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
                _state = MovementState.WallRunning;
            }
            
            switch (_state) // Change material if skiing to remove friction
            {
                case MovementState.Skiing:
                    SetPhysicMaterial(skiMat);
                    break;
                
                case MovementState.WallRunning:
                    SetPhysicMaterial(skiMat);
                    break;

                default:
                    SetPhysicMaterial(gripMat);
                    break;
            }
            
        }

// --------------- MOVE PLAYER -----------------------------------------------        
        private void MovePlayer(Vector2 move)
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
            _moveDirection = (orientation.forward * move.y) + (orientation.right * move.x);
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
                _wallRunGraceTimer -= (float)TimeManager.TickDelta;
                
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
                        _wallRunTimer -= (float)TimeManager.TickDelta;
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
                        _exitWallTimer -= (float)TimeManager.TickDelta;
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
                    _wallRunGraceTimer -= (float)TimeManager.TickDelta;
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
        
        #region Airborne & Jetpack Control

        // --- Low-speed regime (equal ramp for any direction) ---
        [SerializeField] float lowTargetSpeed   = 30f;    // desired low-speed cap (m/s)
        [SerializeField] float blendWidth       = 10f;    // blend range into high-speed steering
        [SerializeField] float lowBaseAccel     = 10f;    // accel strength while under lowTargetSpeed
        [SerializeField] float strafeBias       = 1.0f;   // >1 treats wide A/D as pure strafe (removes forward leak)

// --- High-speed steering (momentum-respecting) ---
        [SerializeField] float airTurnAccel     = 10f;    // lateral steering accel
        [SerializeField] float airTurnFalloffK  = 0.0045f;// how fast steering weakens with speed
        [SerializeField] float airTurnMinFactor = 0.5f;   // steering floor at high speed
        [SerializeField] float airHorizHardCap  = 200f;   // optional horizontal speed clamp

// --- Retro-brake (S to bleed speed when moving forward) ---
        [SerializeField] float retroBrakeStrength = 10f;  // braking accel magnitude
        [SerializeField] float retroBrakeMinSpeed = 4f;   // don’t bother braking below this speed


        private void ApplyAirControl(Vector2 move)
        {
            if (move.sqrMagnitude < 1e-6f) return;

            // Wish vector (camera-relative), horizontal only.
            Vector3 wish = orientation.forward * move.y + orientation.right * move.x;
            wish.y = 0f;
            float wishLen = wish.magnitude;
            if (wishLen < 1e-6f) return;
            wish /= wishLen;

            // Horizontal velocity and speed.
            Vector3 v   = _rb.linearVelocity;
            Vector3 vH  = new Vector3(v.x, 0f, v.z);
            float   s   = vH.magnitude;
            Vector3 vDir = (s > 1e-6f) ? (vH / s) : wish;

            // ---------- LOW-SPEED FORCE (isotropic cap) ----------
            // If input is mostly strafe, strip forward so A/D doesn’t leak forward speed.
            Vector3 lowForceDir = wish;
            if (Mathf.Abs(move.x) > Mathf.Abs(move.y) * strafeBias)
                lowForceDir -= Vector3.Project(lowForceDir, orientation.forward);
            if (lowForceDir.sqrMagnitude > 1e-6f) lowForceDir.Normalize();
            else                                   lowForceDir = wish;

            // Equal ramp toward lowTargetSpeed regardless of direction.
            float lowEff = Mathf.Clamp01((lowTargetSpeed - s) / Mathf.Max(1e-3f, lowTargetSpeed));
            Vector3 lowForce = lowForceDir * (lowBaseAccel * lowEff);

            // ---------- HIGH-SPEED FORCE (lateral steering, no speed injection) ----------
            Vector3 highForce = Vector3.zero;
            if (s > 1e-4f)
            {
                // Lateral component of wish (perpendicular to velocity) bends the path.
                Vector3 lateral = wish - vDir * Vector3.Dot(wish, vDir);
                float latMag = lateral.magnitude;
                if (latMag > 1e-6f)
                {
                    lateral /= latMag;

                    // Steering weakens with speed but keeps a floor.
                    float steerFactor = airTurnMinFactor + (1f - airTurnMinFactor) / (1f + airTurnFalloffK * s);
                    highForce = lateral * (airTurnAccel * steerFactor);
                }
            }

            // ---------- BLEND BY SPEED ----------
            float t = Mathf.Clamp01((s - lowTargetSpeed) / Mathf.Max(1e-3f, blendWidth));
            Vector3 force = Vector3.Lerp(lowForce, highForce, t);

            // ---------- RETRO-BRAKE (S key) ----------
            // Only when pressing S and actually moving roughly forward relative to S (i.e., wish opposes vDir).
            if (move.y < -0.2f && s > retroBrakeMinSpeed)
            {
                // Opposing factor: 0 when same direction, 1 when perfectly opposite.
                float oppose = Mathf.Clamp01(-Vector3.Dot(vDir, wish));
                if (oppose > 1e-3f)
                {
                    // Stronger braking as speed rises from min to lowTargetSpeed; above that, already in high-speed regime.
                    float speedK = Mathf.Clamp01((s - retroBrakeMinSpeed) / Mathf.Max(1e-3f, (lowTargetSpeed - retroBrakeMinSpeed)));
                    Vector3 brake = -vDir * (retroBrakeStrength * oppose * speedK);
                    force += brake;
                }
            }

            _predictionRb.AddForce(force, ForceMode.Acceleration);

            // Optional safety cap (doesn’t fight terrain-earned speed much; just clamps extremes).
            if (airHorizHardCap > 0f && s > airHorizHardCap)
            {
                Vector3 vNew = vDir * airHorizHardCap;
                _rb.linearVelocity = new Vector3(vNew.x, v.y, vNew.z);
            }
        }



        private void Jetpack(Vector2 move)
        {
            Vector3 directionalThrustComponent = Vector3.zero;

                // Only calculate directional thrust if the player is giving WASD input
                if (move.sqrMagnitude > 0.01f)
                {
                    // 1. Determine the world-space direction of the player's input
                    Vector3 localMove = new Vector3(move.x, 0f, move.y);
                    Vector3 worldMoveDir = orientation.TransformDirection(localMove).normalized;

                    // 2. Get the player's current horizontal velocity and its speed
                    Vector3 currentHorizontalVelocity = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
                    
                    // 3. Calculate the player's speed specifically along the desired thrust axis
                    float speedInDesiredDirection = Vector3.Dot(currentHorizontalVelocity, worldMoveDir);

                    // 4. Only apply directional thrust if we are not already moving too fast in that direction
                    if (speedInDesiredDirection < 60f) //maxSpeedForDirectionalThrust
                    {
                        // Calculate a falloff multiplier to smoothly reduce thrust as we approach the max speed.
                        // This is 1.0 at 0 speed, and falls off to 0.0 at maxSpeedForDirectionalThrust.
                        // If speedInDesiredDirection is negative (moving opposite), Clamp01 makes it 0, so falloff is 1.0 (full power).
                        float falloff = 1f - Mathf.Clamp01(speedInDesiredDirection / 60f); //maxSpeedForDirectionalThrust
                        
                        // The total potential magnitude for directional thrust
                        float directionalThrustMagnitude = jetpackForce * (1f - jetUpliftRatio);

                        // The final directional thrust is scaled by the falloff
                        directionalThrustComponent = worldMoveDir * (directionalThrustMagnitude * falloff);
                    }
                    // If speedInDesiredDirection >= maxSpeedForDirectionalThrust, directionalThrustComponent remains Vector3.zero
                }
                // 5. Always calculate the upward lift component
                Vector3 liftThrust = Vector3.up * (jetpackForce * jetUpliftRatio);

                // 6. Combine lift and directional forces and apply them to the Rigidbody
                Vector3 finalJetForce = liftThrust + directionalThrustComponent;
                
                _predictionRb.AddForce(finalJetForce, ForceMode.Force);
        }
        
        #endregion

        #region Skiing

        private void PerformSkiMovement(Vector2 move)
        {
            //if (!_isGrounded)
              //  return;
            
            Vector3 currentVel = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
            Vector3 baseDir = (currentVel.sqrMagnitude > 0.01f) ? currentVel.normalized : orientation.forward;

            // Process input and transform to world space
            Vector3 rawInput = new Vector3(move.x, 0f, Mathf.Min(move.y, 0f));
            Vector3 desiredDir = orientation.TransformDirection(rawInput).normalized;

            // Blend input direction smoothly
            Vector3 steerDir = Vector3.Lerp(baseDir, desiredDir, skiControl).normalized;

            // Calculate steering force, proportional to current speed
            Vector3 steeringForce = (steerDir - baseDir) * (currentVel.magnitude * 1f);
            _predictionRb.AddForce(steeringForce, ForceMode.Force);
            
        }

        #endregion
        
        #region Knockback
        // Called from BaseProjectile via RPC on every client *and* the server.
        public void ReceiveKnockback(Vector3 impulse, float tempDrag = -1f)
        {
            if (_state == MovementState.WallRunning)
                StopWallRun();
            
            if (IsServer)
            {
                _pendingKnockback = impulse;
                if (tempDrag >= 0f) _pendingTempDrag = tempDrag; 
            }
            else
            {
                if (tempDrag >= 0f) _rb.linearDamping = tempDrag;
                _predictionRb.AddForce(impulse, ForceMode.Impulse);
            }
            _heartBeat = !_heartBeat;
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
        
        /*  Server-side helpers       */
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
        
        [Server]
        public void ServerSpendEnergy(float amount)
        {
            _energy = Mathf.Max(0f, _energy - amount);
        }
        #endregion

        void SetPhysicMaterial(PhysicsMaterial pm)
        {
            if (_col.sharedMaterial != pm)
                _col.sharedMaterial = pm;
        }
    }
}


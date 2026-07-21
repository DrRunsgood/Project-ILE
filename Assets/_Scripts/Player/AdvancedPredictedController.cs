using System.Runtime.CompilerServices;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;
using FishNet.Object.Synchronizing;
using _Scripts.Packs;
using _Scripts.Weapons;
using System;

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
        [Tooltip("The parent object of the main character model to be hidden in first person.")]
        [SerializeField] private Transform aimAnchor;
        [SerializeField] Transform cameraFollowTarget;
        
        [Header("Look Settings")]
        [Tooltip("How fast we rotate horizontally.")]
        [SerializeField] private float yawSensitivity = 2f;
        [Tooltip("How fast we rotate vertically.")]
        [SerializeField] private float pitchSensitivity = 2f;
        [SerializeField] private float minPitch = -90f;
        [SerializeField] private float maxPitch = 90f;

        [Header("Movement Speeds")]
        [SerializeField] private float groundMoveSpeed = 5f;
        [SerializeField] private float crouchSpeed = 2f;
        
        [Header("Ground Movement")]
        [SerializeField] private float groundAcceleration = 45f;
        [SerializeField] private float groundBraking = 80f;
        [SerializeField] private float groundStopSpeed = 0.2f;

        [Header("Physics Material")]
        [SerializeField] private PhysicsMaterial playerPhysicsMaterial;
        
        [Header("Jumping")]
        [SerializeField] private float jumpForce = 5f;
        
        [Header("Crouching")]
        [SerializeField] private float crouchYScale = 0.5f;
        
        [Header("Slope Handling")]
        [SerializeField] private float maxSlopeAngle = 45f;
        [SerializeField] private float slopeCheckDistance = 1.2f;
        [SerializeField] private LayerMask groundMask;
        
        [Header("Ground Check")]
        [SerializeField] private Vector3 feetOffset = new Vector3(0f, -1f, 0f);
        [SerializeField] private float feetRadius = 0.5f;
        
        [Header("Wall Surface Contact")]
        [Tooltip("Layers queried for wallrun and wall-jump geometry.")]
        [SerializeField] private LayerMask wallProbeMask;

        [Tooltip("Layers that may never support wall movement. Assign the Ground terrain layer.")]
        [SerializeField] private LayerMask wallInteractionBlockedMask;

        [SerializeField] private float wallProbeRadius = 0.15f;

        [Tooltip("Minimum angle between world-up and a valid wall normal.")]
        [SerializeField] private float minWallSurfaceAngle = 75f;

        [Tooltip("Maximum angle between world-up and a valid wall normal.")]
        [SerializeField] private float maxWallSurfaceAngle = 100f;
        
        [Header("Wall Jump")]
        [SerializeField] private float wallJumpProbeDistance = 0.9f;
        [SerializeField] private float wallJumpAwayForce = 7f;
        [SerializeField] private float wallJumpVerticalForce = 7f;

        [Tooltip("Normal grounded jump lockout in seconds.")]
        [SerializeField] private float jumpLockDuration = 0.3f;

        [Tooltip("Wall-jump lockout in seconds.")]
        [SerializeField] private float wallJumpLockDuration = 0.45f;
        
        [Header("Wall Running")]
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
        [SerializeField] private float jetpackFuelBurnRate = 10f;
        [SerializeField] private float jetpackFuelCutoff = 5f;
        [Tooltip("Minimum energy lost per second while actively jetting, even if configured regeneration exceeds burn.")]
        [SerializeField] private float minimumJetNetBurnRate = 1f;

        [Header("Skiing Settings")]
        [SerializeField] private float skiControl = 0.1f;
        [SerializeField] private float skiDrag = 0.02f;
        
        [Header("Energy Settings")]
        [SerializeField] private float maxEnergy = 100f;
        [SerializeField] private float energyRegenRate = 5f;

        [Header("Debug / State Info")]
        [SerializeField] private MovementState _state;
        
        public Transform CameraFollowTarget => cameraFollowTarget != null ? cameraFollowTarget : transform;
        public MovementState State => _state;
        public Transform AimAnchor => aimAnchor;
        public float CurrentPitch => _lookModule != null ? _lookModule.CurrentPitch : 0f;

        public event Action OnLocalPoseResetApplied;
        public event Action<byte, Vector3, Quaternion> OnObserverPoseResetReceived;

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
        
        private float _startYScale;
        
        // Input
        private InputHandler _iH;
        
        // Weapon Manager
        private WeaponManager _weaponManager;
        
        // Pack Manager
        private PackManager _packMgr;
        
        // For camera orientation
        private PlayerLookModule _lookModule;
        
        // Surface Probe
        private PlayerSurfaceProbe _surfaceProbe;
        
        // Wall Running
        private bool _canWallRun;
        private bool _exitingWall;
        private float _wallRunTimer;
        private float _exitWallTimer;
        private Vector3 _wallRunDirection;
        private float _targetWallRunSpeed;
        private Vector3 _storedWallNormal = Vector3.zero;
        private float _wallRunGraceTimer;
        
        // Energy
        private PlayerEnergyModule _energyModule; 
        
        // Jetpack
        private bool _jetLockedOut;
        
        // Jump Lock
        private byte _jumpLockTicks;
        
        // Pose Reset
        private byte _poseResetSequence;
        private byte _lastAppliedPoseResetSequence;
        private bool _poseResetSequenceInitialized;
        
        // Public helpers
        public float Energy => _energyModule?.Energy ?? 0f;
        public bool ShieldActive => _packMgr && _packMgr.Active && _packMgr.CurrentId == PackId.Shield;
        
        // Knockback
        private Vector3? _pendingKnockback;
        private float? _pendingTempDrag;
        private bool _knockbackDirtyToggle;  // Used to mark replicate packet dirty when receiving knockback to keep kb responsive but keep network efficienies
        
        const float LookQuantScale = 512f;
        private const byte MovementStateMask = 0b0000_0111;
        private const byte JetLockedOutMask = 0b0000_1000;
        
        private const byte MoveMask = 0b0000_1111;
        private const byte KnockbackDirtyToggleFlag = 0b0001_0000;
        private const byte JumpPressedEventFlag = 0b0010_0000;
        
        #endregion

        #region FishNet Data Structures
        // Replication struct
        public struct MovementData : IReplicateData
        {
            private uint _tick;

            public byte MoveAndEvents;
            public short LookX;
            public short LookY;
            public InputButtons Held;

            public MovementData(uint tick, Vector2 move, Vector2 look, InputButtons held, bool heart, bool jumpPressed)
            {
                _tick = tick;

                MoveAndEvents = (byte)(NetUtils.MoveCodec.Pack(move.x, move.y) & MoveMask);

                if (heart) MoveAndEvents |= KnockbackDirtyToggleFlag;

                if (jumpPressed) MoveAndEvents |= JumpPressedEventFlag;

                LookX = (short)Mathf.Clamp(Mathf.RoundToInt(look.x * LookQuantScale), short.MinValue, short.MaxValue);

                LookY = (short)Mathf.Clamp(Mathf.RoundToInt(look.y * LookQuantScale), short.MinValue, short.MaxValue);

                Held = held;
            }

            public uint GetTick() => _tick;
            public void SetTick(uint value) => _tick = value;
            public void Dispose() { }
        }
        
        // Reconciliation
        private struct ReconciliationData : IReconcileData
        {
            uint _tick;
            public Vector3  Position;
            public ushort   Speed, Heading;
            public short    Vy;
            public byte     EnergyQ;
            public byte StateFlags;
            public ushort   YawQ;
            public ushort    CurrentPitchQ;
            public byte JumpLockTicks;
            public byte PoseResetSequence;
            

            public ReconciliationData(Vector3 pos, Vector3 vel, MovementState state, bool jetLockedOut,
                byte jumpLockTicks, float yawDeg, float pitchDeg, byte energyQ, byte poseResetSequence)
            {
                _tick = 0;

                Position = pos;
                EncodeVelocity(vel, out Speed, out Heading, out Vy);

                StateFlags = (byte)((byte)state & MovementStateMask);
                JumpLockTicks = jumpLockTicks;

                if (jetLockedOut)
                    StateFlags |= JetLockedOutMask;

                EnergyQ = energyQ;
                
                PoseResetSequence = poseResetSequence;

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
            _rb.useGravity = false;
            _netObj = GetComponent<NetworkObject>();
            _predictionRb = new PredictionRigidbody();
            _predictionRb.Initialize(_rb);
            _weaponManager = GetComponent<WeaponManager>();
            _packMgr = GetComponent<PackManager>();
            _col = GetComponent<Collider>();
            
            SetPhysicMaterial(playerPhysicsMaterial);
            
            _energyModule = new PlayerEnergyModule(maxEnergy, energyRegenRate, maxEnergy);
            _lookModule = new PlayerLookModule(yawSensitivity, pitchSensitivity, minPitch, maxPitch, transform.eulerAngles.y, 0f);
            _surfaceProbe = new PlayerSurfaceProbe(maxSlopeAngle, slopeCheckDistance, groundMask, feetOffset, feetRadius, wallProbeMask,
                wallInteractionBlockedMask, wallCheckDistance, wallProbeRadius, minWallSurfaceAngle, maxWallSurfaceAngle, minJumpHeight);            
            _startYScale = transform.localScale.y;
            
            _poseResetSequence = 0;
            _lastAppliedPoseResetSequence = 0;
            _poseResetSequenceInitialized = false;
            _knockbackDirtyToggle = false;
            _pendingKnockback = null;
            _pendingTempDrag = null;
            _jumpLockTicks = 0;
            
            TimeManager.OnTick += OnTick;
            TimeManager.OnPostTick += OnPostTick;
        }
        
        
        public override void OnStartClient()
        {
            base.OnStartClient();
            
            if (IsOwner)
            {
                _iH = GetComponent<InputHandler>();
                LocalPlayerContext.Register(this);

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible   = false;
            }
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            _energyModule.ResetEnergy();
        }

        public override void OnStopNetwork()
        {
            base.OnStopNetwork();
            TimeManager.OnTick -= OnTick;
            TimeManager.OnPostTick -= OnPostTick;
            
            if (IsOwner)
                LocalPlayerContext.Clear(this);
        }
        
        private void OnTick()
        {
            if (IsOwner)
            {
                var ih = _iH;
                
                Vector2 lookDelta = ih.ConsumeLookDelta();

                bool jumpPressed = ih.ConsumeJumpPressed();

                MovementData movementData =
                    new MovementData(
                        TimeManager.Tick,
                        ih.Move,
                        lookDelta,
                        ih.HeldButtons,
                        _knockbackDirtyToggle,
                        jumpPressed);

                Replicate(movementData);
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
                _jetLockedOut,
                _jumpLockTicks,
                _lookModule.Yaw,
                _lookModule.CurrentPitch,
                _energyModule.QuantizeEnergy(),
                _poseResetSequence
            );
 
            Reconcile(recData);
        }

        [Reconcile]
        private void Reconcile(ReconciliationData data, Channel channel = Channel.Unreliable)
        {
            float yaw = data.YawQ * (360f / 65535f);
            float pitch = DequantizePitch(data.CurrentPitchQ);

            _rb.Move(data.Position, _rb.rotation);
            _rb.linearVelocity = DecodeVelocity(data.Speed, data.Heading, data.Vy);

            // Correct movement state
            _state = (MovementState)(data.StateFlags & MovementStateMask);

            _jetLockedOut = (data.StateFlags & JetLockedOutMask) != 0;
            _jumpLockTicks = data.JumpLockTicks;

            _lookModule.ApplyLookState(yaw, pitch, _rb, aimAnchor);
            _energyModule.ApplyQuantizedEnergy(data.EnergyQ);
            HandleAppliedPoseResetSequence(data.PoseResetSequence);
        }
        #endregion

        #region Replicate

        [Replicate]
        private void Replicate(MovementData md, ReplicateState state = ReplicateState.Invalid, Channel channel = Channel.Unreliable)
        {
            if (IsFrozen) return;
            
            float dt = (float)TimeManager.TickDelta;
            
            if (_jumpLockTicks > 0) _jumpLockTicks--;

            Decompress(md, out Vector2 move, out Vector2 look, out InputButtons held, out bool jumpPressed);
            
            if (_pendingKnockback.HasValue) // Check incoming Knockback
            {
                ApplyKnockback();
                ApplyMovementGravity();
                _predictionRb.Simulate();
                return;
            }
            
            _surfaceProbe.RefreshGrounding(_rb, transform);
            
            ApplyRotation(look.x, look.y);
            
            // Check for wall
            if (Btn(held, InputButtons.WallRun) && _state != MovementState.WallRunning)
            {
                _surfaceProbe.RefreshWallProbe(transform, orientation);

                if (_surfaceProbe.WallNormal != Vector3.zero)
                    _wallRunGraceTimer = wallRunGraceTime;
                else
                    HandleWallLoss();
            }

            // Update movement states
            UpdateMovementState(held);

            if (_state == MovementState.WallRunning)
                UpdateWallRunState();

            TryHandleJump(jumpPressed);

            ControlEnv();
            
            bool wantsJetThisTick = _state == MovementState.Jetpacking;

            bool jetPoweredThisTick = ResolveMovementEnergy(dt, wantsJetThisTick);

            if (wantsJetThisTick && !jetPoweredThisTick) _state = MovementState.Airborne;

            switch (_state)
            {
                case MovementState.Jetpacking:
                    ApplyPassiveAirShaping(move);
                    ApplyJetpackMovement(move);
                    break;

                case MovementState.Airborne:
                    ApplyPassiveAirShaping(move);
                    ApplyPassiveAirBrake(move);
                    break;

                case MovementState.Skiing:
                    PerformSkiMovement(move);
                    break;

                case MovementState.WallRunning:
                    PerformWallRunMovement();
                    break;

                default:
                    MovePlayer(move);
                    break;
            }

            ApplyMovementGravity();
            ApplyPlanarSpeedResistance();
            
            _predictionRb.Simulate();

            if (IsServer)
            {
                //Transform fireAnchor = viewOrigin != null ? viewOrigin : aimAnchor;
                Transform fireAnchor = aimAnchor != null ? aimAnchor : orientation;

                if (fireAnchor != null)
                {
                    FirePose pose = new FirePose(fireAnchor.position, fireAnchor.forward,
                        _rb.linearVelocity, TimeManager.Tick);

                    if (_weaponManager != null)
                        _weaponManager.Server_ProcessFireInput(held, pose);

                    if (LagCompensationManager.Instance != null)
                        LagCompensationManager.Instance.RecordSnapshot(_netObj, pose.Position, pose.Direction,
                            pose.Velocity, pose.Tick);
                }
            }
        }
        #endregion
        
        #region Quantization and Decompression
        private static void Decompress(in MovementData md, out Vector2 move, out Vector2 look, out InputButtons held, out bool jumpPressed)
        {
            move = NetUtils.MoveCodec.Unpack((byte)(md.MoveAndEvents & MoveMask));

            look = new Vector2(md.LookX / LookQuantScale, md.LookY / LookQuantScale);

            held = md.Held;

            jumpPressed = (md.MoveAndEvents & JumpPressedEventFlag) != 0;
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
        private float GetEnergyPackBonusRate()
        {
            if (_packMgr == null || _packMgr.CurrentId != PackId.Energy || _packMgr.CurrentDef == null)
                return 0f;
            
            return Mathf.Max(0f, _packMgr.CurrentDef.extraRegenPerSec);
        }
        
        private float GetShieldDrainRate()
        {
            if (_packMgr == null || !_packMgr.Active || _packMgr.CurrentId != PackId.Shield || _packMgr.CurrentDef == null)
                return 0f;

            return Mathf.Max(0f, _packMgr.CurrentDef.shieldDrainPerSec);
        }
        #endregion
        
        #region Rotation

        private void ApplyRotation(float yawDeltaRaw, float pitchDeltaRaw)
        {
            _lookModule.ApplyRotation(yawDeltaRaw, pitchDeltaRaw, _rb, aimAnchor);
        }
        
        public Quaternion GetRenderViewRotation(Vector2 pendingLookDelta)
        {
            if (_lookModule == null)
            {
                return Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
            }

            return _lookModule.GetPreviewRotation(pendingLookDelta);
        }

        #endregion

        #region Jump
        private bool TryHandleJump(bool jumpPressed)
        {
            if (!jumpPressed || _jumpLockTicks > 0)
                return false;

            if (_state == MovementState.WallRunning)
            {
                PerformWallRunJump();
                return true;
            }

            if (_surfaceProbe.IsGrounded)
            {
                PerformGroundJump();
                return true;
            }

            return TryPerformWallJump();
        }

        private void PerformGroundJump()
        {
            _predictionRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            _jumpLockTicks = SecondsToTickLock(jumpLockDuration);

            _state = MovementState.Airborne;
        }

        private bool TryPerformWallJump()
        {
            if (aimAnchor == null)
                return false;

            if (!_surfaceProbe.TryGetWallJumpContact(aimAnchor.position, aimAnchor.forward,
                    wallJumpProbeDistance, out RaycastHit hit))
            {
                return false;
            }

            Vector3 jumpImpulse = hit.normal * wallJumpAwayForce + Vector3.up * wallJumpVerticalForce;

            _predictionRb.AddForce(jumpImpulse, ForceMode.Impulse);

            _jumpLockTicks = SecondsToTickLock(wallJumpLockDuration);

            _state = MovementState.Airborne;

            return true;
        }

        #endregion

        #region Movement Methods
        
        private byte SecondsToTickLock(float seconds)
        {
            if (seconds <= 0f)
                return 0;

            float tickDelta = (float)TimeManager.TickDelta;

            if (tickDelta <= 0f)
                return 0;

            int ticks = Mathf.CeilToInt(seconds / tickDelta);

            return (byte)Mathf.Clamp(ticks, 1, byte.MaxValue);
        }
        
        private void OnDrawGizmosSelected()
        {
            if (_rb == null)
                _rb = GetComponent<Rigidbody>();

            Gizmos.color = Color.yellow;

            if (_surfaceProbe != null)
                _surfaceProbe.DrawGroundGizmo(_rb);
            else if (_rb != null)
                Gizmos.DrawWireSphere(_rb.position + feetOffset, feetRadius);
        }
    
        private void UpdateMovementState(InputButtons held)
        {
            // Once depleted, Jet remains locked until the ignition threshold is restored.
            if (_jetLockedOut && Energy >= jetpackFuelCutoff)
                _jetLockedOut = false;

            // Jetpack overrides all other states when available.
            if (Btn(held, InputButtons.Jetpack) && !_jetLockedOut)
            {
                bool canContinueJetting = _state == MovementState.Jetpacking ? Energy > 0f : Energy >= jetpackFuelCutoff;

                if (canContinueJetting)
                {
                    if (_state == MovementState.WallRunning)
                        StopWallRun();

                    _state = MovementState.Jetpacking;
                    return;
                }
            }
            

            // Preserve an active wallrun until its own update logic ends it.
            if (_state == MovementState.WallRunning)
            {
                _state = MovementState.WallRunning;
                return;
            }

            // Check for explicit wallrun initiation.
            _canWallRun = CanWallRun();

            if (Btn(held, InputButtons.WallRun) && _canWallRun)
            {
                StartWallRun();
                _state = MovementState.WallRunning;
                return;
            }

            // Ground-supported states.
            if (_surfaceProbe.IsGrounded)
            {
                if (Btn(held, InputButtons.Ski))
                {
                    _state = MovementState.Skiing;
                }
                else if (Btn(held, InputButtons.Crouch))
                {
                    _state = MovementState.Crouching;
                }
                else
                {
                    _state = MovementState.Walking;
                }

                return;
            }

            // Ski input remains buffered through Held, but does not replace
            // airborne movement until valid ground contact exists.
            _state = MovementState.Airborne;
        }
        
        private Vector3 GetGroundNormal()
        {
            if (_surfaceProbe != null &&
                _surfaceProbe.SlopeHit.collider != null)
            {
                return _surfaceProbe.SlopeHit.normal;
            }

            return Vector3.up;
        }
        
        private void ApplyMovementGravity()
        {
            Vector3 gravity = Physics.gravity;

            switch (_state)
            {
                case MovementState.Walking:
                case MovementState.Crouching:
                {
                    if (!_surfaceProbe.IsGrounded)
                    {
                        _predictionRb.AddForce(gravity, ForceMode.Acceleration);
                        return;
                    }

                    Vector3 groundNormal = GetGroundNormal();

                    float gravityIntoSurface = Vector3.Dot(gravity, groundNormal);

                    if (gravityIntoSurface < 0f)
                    {
                        Vector3 supportGravity = groundNormal * gravityIntoSurface;
                        _predictionRb.AddForce(supportGravity, ForceMode.Acceleration);
                    }

                    break;
                }

                case MovementState.Skiing:
                case MovementState.Airborne:
                case MovementState.Jetpacking:
                case MovementState.None:
                    _predictionRb.AddForce(gravity, ForceMode.Acceleration);
                    break;

                case MovementState.WallRunning:
                    break;

                default:
                    _predictionRb.AddForce(gravity, ForceMode.Acceleration);
                    break;
            }
        }

// --------------- MOVE PLAYER -----------------------------------------------       

        private void MovePlayer(Vector2 move)
        {
            transform.localScale = new Vector3(transform.localScale.x, _startYScale, transform.localScale.z);

            float moveSpeed = groundMoveSpeed;

            if (_state == MovementState.Crouching)
            {
                transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
                moveSpeed = crouchSpeed;
            }

            if (!_surfaceProbe.IsGrounded)
                return;

            Vector3 groundNormal = GetGroundNormal();

            Vector3 wishDir = orientation.forward * move.y + orientation.right * move.x;

            wishDir = Vector3.ProjectOnPlane(wishDir, groundNormal);

            float wishMagnitude = Mathf.Clamp01(move.magnitude);

            if (wishDir.sqrMagnitude > 0.0001f)
                wishDir.Normalize();

            Vector3 currentVelocity = _rb.linearVelocity;

            Vector3 currentGroundVelocity = Vector3.ProjectOnPlane(currentVelocity, groundNormal);

            Vector3 normalVelocity = currentVelocity - currentGroundVelocity;

            Vector3 desiredGroundVelocity = wishDir * (moveSpeed * wishMagnitude);

            float accel = wishMagnitude > 0.01f ? groundAcceleration : groundBraking;

            Vector3 newGroundVelocity = Vector3.MoveTowards(currentGroundVelocity,
                desiredGroundVelocity, accel * (float)TimeManager.TickDelta);

            if (wishMagnitude <= 0.01f && newGroundVelocity.magnitude <= groundStopSpeed)
                newGroundVelocity = Vector3.zero;
            
            _predictionRb.Velocity(normalVelocity + newGroundVelocity);
        }

        private void ControlEnv()
        {
            _rb.linearDamping = _state == MovementState.Skiing ? skiDrag : 0f;
        }

        #endregion

        #region Wall Run
    
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
                if (_surfaceProbe.WallLeft || _surfaceProbe.WallRight)
                {
                    _surfaceProbe.ClearWallProbe();
                }
            }
        }
        
        private bool CanWallRun()
        {
            if (!(_surfaceProbe.WallLeft || _surfaceProbe.WallRight))
                return false;

            return _surfaceProbe.IsAboveMinJumpHeight(transform) && !_exitingWall;
        }

        private void StartWallRun()
        {
            _wallRunTimer = maxWallRunTime;
            _exitingWall  = false;
            _exitWallTimer= exitWallTime;
            _wallRunGraceTimer = wallRunGraceTime;
            
            _storedWallNormal = _surfaceProbe.WallNormal;
            
            Vector3 playerVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
            float speed = playerVelocity.magnitude;
            
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
            if (Physics.Raycast(transform.position, -_storedWallNormal, out RaycastHit hit, wallCheckDistance, wallProbeMask))
            {
                currentWallNormal = hit.normal;
                return currentWallNormal == _storedWallNormal;
            }
            return false;
        }

        private void StopWallRun()
        {
            _exitingWall   = false;
            _surfaceProbe.ClearWallProbe();
            _canWallRun    = false;
            _storedWallNormal = Vector3.zero;
            _wallRunDirection = Vector3.zero;
            _state = MovementState.Airborne;
            _wallRunGraceTimer = 0f;
            _exitWallTimer = 0f;
        }

        private void PerformWallRunJump()
        {
            Vector3 wallNormal = _storedWallNormal;

            Vector3 forceToApply = Vector3.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

            StopWallRun();

            _predictionRb.AddForce(forceToApply, ForceMode.Impulse);

            _jumpLockTicks = SecondsToTickLock(wallJumpLockDuration);
        }

        #endregion
        
       
        #region Airborne & Jetpack Control

        
        [Header("Jetpack - Lift")]
        [Tooltip("Upward acceleration while jetting. Full gravity still applies.")]
        [SerializeField] private float jetLiftAcceleration = 33f;

        [Header("Passive Air Shaping")]
        [SerializeField] private float airShapeAcceleration = 10f;
        [SerializeField] private float airShapeStartSpeed = 12f;
        [SerializeField] private float airShapeFullSpeed = 40f;
        
        [Header("High-Speed Air Shaping")]
        [Tooltip("Speed where additional high-speed shaping begins.")]
        [SerializeField] private float airShapeBoostStartSpeed = 60f;

        [Tooltip("Speed where the additional shaping multiplier reaches its maximum.")]
        [SerializeField] private float airShapeBoostFullSpeed = 100f;

        [Tooltip("Maximum shaping multiplier at very high speed.")]
        [SerializeField] private float airShapeHighSpeedMultiplier = 1.6f;

        [Header("Passive Air Brake")]
        [SerializeField] private float airBrakeAcceleration = 10f;

        [Header("Air / Jet Input")]
        [SerializeField] private float airInputDeadzone = 0.1f;
        
        private void GetPlanarMovementAxes(out Vector3 forward, out Vector3 right)
        {
            forward = orientation.forward;
            forward.y = 0f;

            if (forward.sqrMagnitude <= 0.0001f) forward = transform.forward;

            forward.Normalize();

            right = orientation.right;
            right.y = 0f;

            if (right.sqrMagnitude <= 0.0001f) right = Vector3.Cross(Vector3.up, forward);

            right.Normalize();
        }

        private float GetAirShapeAuthority(float planarSpeed)
        {
            return Mathf.InverseLerp(airShapeStartSpeed, airShapeFullSpeed, planarSpeed);
        }

        private void ApplyPassiveAirShaping(Vector2 move)
        {
            if (Mathf.Abs(move.x) <= airInputDeadzone)
                return;

            Vector3 planarVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);

            float planarSpeed = planarVelocity.magnitude;

            if (planarSpeed <= 0.0001f)
                return;

            float authority = GetAirShapeAuthority(planarSpeed);

            if (authority <= 0f)
                return;

            GetPlanarMovementAxes(out _, out Vector3 right);

            Vector3 velocityDirection = planarVelocity / planarSpeed;
            Vector3 sideWish = right * Mathf.Sign(move.x);
            Vector3 shapingDirection = Vector3.ProjectOnPlane(sideWish, velocityDirection);

            if (shapingDirection.sqrMagnitude <= 0.0001f)
                return;

            shapingDirection.Normalize();

            float inputStrength = Mathf.Abs(move.x);
            float highSpeedT = Mathf.InverseLerp(airShapeBoostStartSpeed, airShapeBoostFullSpeed, planarSpeed);
            float highSpeedMultiplier = Mathf.Lerp(1f, airShapeHighSpeedMultiplier, highSpeedT);

            Vector3 shapingAcceleration = shapingDirection * (airShapeAcceleration * authority * highSpeedMultiplier * inputStrength);

            _predictionRb.AddForce(shapingAcceleration, ForceMode.Acceleration);
        }
        
        private void ApplyPassiveAirBrake(Vector2 move)
        {
            if (move.y >= -airInputDeadzone)
                return;

            GetPlanarMovementAxes(out Vector3 forward, out _);

            Vector3 planarVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);

            float forwardSpeed = Vector3.Dot(planarVelocity, forward);

            // Passive airborne braking cannot accelerate backward.
            if (forwardSpeed <= 0f)
                return;

            float dt = (float)TimeManager.TickDelta;

            float brakeAcceleration = Mathf.Min(airBrakeAcceleration, forwardSpeed / Mathf.Max(dt, 0.0001f));

            _predictionRb.AddForce(-forward * brakeAcceleration, ForceMode.Acceleration);
        }
        
        
        [Header("Jetpack - Simple Directional Thrust")]
        [Tooltip("Planar acceleration supplied by full directional input while jetting.")]
        [SerializeField] private float jetPlanarAcceleration = 14f;

        [Tooltip("Fraction of vertical lift traded away at full directional input.")]
        [Range(0f, 1f)]
        [SerializeField] private float jetDirectionalLiftTradeoff = 0.3f;
        
        [Header("Simple Jet - Forward Momentum Falloff")]
        [Tooltip("Speed where jet thrust along the current trajectory begins weakening.")]
        [SerializeField] private float jetTrajectoryFalloffStartSpeed = 30f;

        [Tooltip("Speed where along-trajectory thrust reaches its minimum.")]
        [SerializeField] private float jetTrajectoryFalloffEndSpeed = 50f;

        [Range(0f, 1f)]
        [Tooltip("Remaining jet thrust along the current trajectory at high speed.")]
        [SerializeField] private float jetTrajectoryMinimumAuthority = 0.15f;
        
        /*
        
        private void ApplyJetpackMovement(Vector2 move)
        {
            GetPlanarMovementAxes(out Vector3 forward, out Vector3 right);

            Vector3 planarInput = forward * move.y + right * move.x;

            float inputMagnitude = Mathf.Clamp01(planarInput.magnitude);

            Vector3 planarDirection = Vector3.zero;

            if (planarInput.sqrMagnitude > 0.0001f)
                planarDirection = planarInput.normalized;

            Vector3 planarVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);

            float planarSpeed = planarVelocity.magnitude;

            Vector3 adjustedPlanarDirection = planarDirection;

            if (planarSpeed > 0.0001f &&
                planarDirection.sqrMagnitude > 0.0001f)
            {
                Vector3 velocityDirection = planarVelocity / planarSpeed;

                // Split requested thrust into:
                // 1. Along current travel direction
                // 2. Perpendicular steering/strafe direction
                Vector3 trajectoryComponent = Vector3.Project(planarDirection, velocityDirection);

                Vector3 lateralComponent = planarDirection - trajectoryComponent;

                float speedT = Mathf.InverseLerp(jetTrajectoryFalloffStartSpeed, jetTrajectoryFalloffEndSpeed, planarSpeed);

                float trajectoryAuthority = Mathf.Lerp(1f, jetTrajectoryMinimumAuthority, speedT);

                // Only diminish thrust that adds speed along the current trajectory.
                // Opposing thrust remains available for braking/reversal.
                if (Vector3.Dot(trajectoryComponent, velocityDirection) > 0f)
                    trajectoryComponent *= trajectoryAuthority;

                adjustedPlanarDirection = trajectoryComponent + lateralComponent;
            }

            float liftFraction = 1f - inputMagnitude * jetDirectionalLiftTradeoff;

            Vector3 liftAcceleration = Vector3.up * (jetLiftAcceleration * liftFraction);

            Vector3 planarAcceleration = adjustedPlanarDirection * (jetPlanarAcceleration * inputMagnitude);

            _predictionRb.AddForce(liftAcceleration + planarAcceleration, ForceMode.Acceleration);
        }
        */
        
        
        [Header("Planar Speed Resistance")]
        [SerializeField] private float resistanceStartSpeed = 50f;
        [SerializeField] private float resistanceFullSpeed = 120f;
        [SerializeField] private float resistanceMaxAcceleration = 5f;

        [Tooltip("Additional resistance while actively skiing.")]
        [SerializeField] private float skiResistanceMultiplier = 1.0f;
        
        private void ApplyPlanarSpeedResistance()
        {
            Vector3 planarVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);

            float planarSpeed = planarVelocity.magnitude;

            if (planarSpeed <= resistanceStartSpeed)
                return;

            float speedT = Mathf.InverseLerp(resistanceStartSpeed, resistanceFullSpeed, planarSpeed);

            // Gentle near the threshold, stronger toward the upper band.
            float resistanceCurve = speedT * speedT;

            float stateMultiplier = _state == MovementState.Skiing ? skiResistanceMultiplier : 1f;

            float resistanceAcceleration = resistanceMaxAcceleration * resistanceCurve * stateMultiplier;

            _predictionRb.AddForce(-planarVelocity.normalized * resistanceAcceleration, ForceMode.Acceleration);
        }
        
        
        [Header("Legacy Jet Directional Limit")]
        [Tooltip("Directional speed where requested planar jet thrust falls to zero.")]
        [SerializeField] private float jetDirectionalTargetSpeed = 60f;
        private void ApplyJetpackMovement(Vector2 move)
        {
            GetPlanarMovementAxes(out Vector3 forward, out Vector3 right);

            Vector3 planarInput = forward * move.y + right * move.x;

            float inputMagnitude = Mathf.Clamp01(planarInput.magnitude);

            Vector3 directionalAcceleration = Vector3.zero;

            if (planarInput.sqrMagnitude > 0.0001f)
            {
                Vector3 requestedDirection = planarInput.normalized;
                Vector3 planarVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);

                float speedInRequestedDirection = Vector3.Dot(planarVelocity, requestedDirection);
  
                float directionalAuthority =
                    1f - Mathf.Clamp01(speedInRequestedDirection / Mathf.Max(jetDirectionalTargetSpeed, 0.0001f));

                directionalAcceleration = requestedDirection * (jetPlanarAcceleration * directionalAuthority * inputMagnitude);
            }

            float liftFraction = 1f - inputMagnitude * jetDirectionalLiftTradeoff;

            Vector3 liftAcceleration = Vector3.up * (jetLiftAcceleration * liftFraction);

            _predictionRb.AddForce(liftAcceleration + directionalAcceleration, ForceMode.Acceleration);
        }
        #endregion
        

        #region Skiing
        private void PerformSkiMovement(Vector2 move)
        {
            Vector3 groundNormal = GetGroundNormal();

            // Use velocity along the contacted surface rather than world-horizontal velocity.
            Vector3 currentGroundVelocity =
                Vector3.ProjectOnPlane(_rb.linearVelocity, groundNormal);

            Vector3 baseDir;

            if (currentGroundVelocity.sqrMagnitude > 0.01f)
            {
                baseDir = currentGroundVelocity.normalized;
            }
            else
            {
                baseDir = Vector3.ProjectOnPlane(orientation.forward, groundNormal);

                if (baseDir.sqrMagnitude > 0.0001f)
                    baseDir.Normalize();
            }

            // Preserve the current behavior: A/D steer laterally, S may steer backward, W does not actively add forward ski input.
            Vector3 rawInput = new Vector3(move.x, 0f, Mathf.Min(move.y, 0f));

            Vector3 desiredDir = orientation.TransformDirection(rawInput);

            desiredDir = Vector3.ProjectOnPlane(desiredDir, groundNormal);
            
            if (desiredDir.sqrMagnitude <= 0.0001f)
                return;

            desiredDir.Normalize();

            Vector3 steerDir = Vector3.Lerp(baseDir, desiredDir, skiControl);

            if (steerDir.sqrMagnitude <= 0.0001f)
                return;

            steerDir.Normalize();

            Vector3 steeringForce = (steerDir - baseDir) * currentGroundVelocity.magnitude;

            _predictionRb.AddForce(steeringForce, ForceMode.Acceleration);
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
            _knockbackDirtyToggle = !_knockbackDirtyToggle;
        }
        
        private void ApplyKnockback()
        {
            if (_pendingTempDrag.HasValue)
                _rb.linearDamping = _pendingTempDrag.Value;

            _predictionRb.AddForce(_pendingKnockback.Value, ForceMode.Impulse);
            
            _pendingKnockback  = null;
            _pendingTempDrag   = null;
                
            _state = MovementState.Airborne;
        }
        #endregion
        
        #region respawn
        [Server]
        public void HardResetMovement(Vector3 position, Quaternion rotation)
        {
            // Reset Rigidbody state.
            _rb.position = position;
            _rb.rotation = rotation;
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _rb.linearDamping = 0f;

            // Reset prediction wrapper state.
            _predictionRb.Velocity(Vector3.zero);

            // Reset movement state.
            _state = MovementState.Airborne;
            _jetLockedOut = false;
            _jumpLockTicks = 0;

            // Reset knockback state.
            _pendingKnockback = null;
            _pendingTempDrag = null;

            // Restore normal scale.
            transform.localScale = new Vector3(transform.localScale.x, _startYScale, transform.localScale.z);

            // Reset wall state.
            _surfaceProbe?.ClearWallProbe();

            _canWallRun = false;
            _exitingWall = false;

            _wallRunTimer = 0f;
            _exitWallTimer = 0f;
            _wallRunGraceTimer = 0f;

            _storedWallNormal = Vector3.zero;
            _wallRunDirection = Vector3.zero;
            _targetWallRunSpeed = 0f;

            // Reset look.
            float yaw = rotation.eulerAngles.y;

            _lookModule.ResetLook(_rb, aimAnchor, yaw, 0f);

            transform.SetPositionAndRotation(position, rotation);

            unchecked
            {
                _poseResetSequence++;
            }

            if (_poseResetSequence == 0)
                _poseResetSequence = 1;

            RpcNotifyObserverPoseReset(_poseResetSequence, position, rotation);
        }
        
        public void ResetEnergy()
        {
            _energyModule.ResetEnergy();
            _jetLockedOut = false;
        }
        
        private void HandleAppliedPoseResetSequence(byte sequence)
        {
            if (!IsOwner)
                return;

            if (!_poseResetSequenceInitialized)
            {
                _poseResetSequenceInitialized = true;
                _lastAppliedPoseResetSequence = sequence;

                /*
                 * Zero means no authoritative hard reset has been issued yet.
                 */
                if (sequence == 0)
                    return;
            }
            else
            {
                if (sequence == _lastAppliedPoseResetSequence)
                    return;

                _lastAppliedPoseResetSequence = sequence;
            }

            /*
             * Mouse input captured before the discontinuity must not be applied
             * on top of the new spawn/teleport-facing orientation.
             */
            _iH?.ClearTransientBuffers();

            OnLocalPoseResetApplied?.Invoke();
        }
        #endregion
        
        #region energy
        private bool ResolveMovementEnergy(float dt, bool wantsJetThisTick)
        {
            if (dt <= 0f)
                return false;

            float passiveRegenRate = _energyModule.BaseRegenRate + GetEnergyPackBonusRate();

            float shieldDrainRate = GetShieldDrainRate();

            /*
             * JETTING - Passive regeneration offsets raw jet burn.
             * Shield drain remains an additional shared-pool cost.
             */
            if (wantsJetThisTick && !_jetLockedOut)
            {
                float netJetBurnRate = jetpackFuelBurnRate - passiveRegenRate;

                netJetBurnRate = Mathf.Max(minimumJetNetBurnRate, netJetBurnRate);

                float totalDrainRate = netJetBurnRate + shieldDrainRate;

                float tickCost = totalDrainRate * dt;

                /*
                 * Resolve payment before applying thrust.
                 *
                 * If the remaining pool cannot fully fund this tick,
                 * deplete the pool, lock the jet, and do not apply thrust.
                 */
                if (Energy <= tickCost)
                {
                    _energyModule.SetEnergy(0f);
                    _jetLockedOut = true;

                    if (shieldDrainRate > 0f && IsServer)
                        _packMgr.ForceActive(false);

                    return false;
                }

                _energyModule.ConsumeForced(tickCost);
                return true;
            }

            /*
             * NOT JETTING - Apply regeneration and active shield drain as one net rate.
             */
            float netPassiveRate = passiveRegenRate - shieldDrainRate;

            _energyModule.ApplyEnergyDelta(netPassiveRate * dt);

            /* If active shield drain exhausted the pool,
             * deactivate the shield authoritatively. */
            
            if (shieldDrainRate > 0f && Energy <= 0f && IsServer)
                _packMgr.ForceActive(false);
            
            return false;
        }
        
        /*  Server-side helpers       */
        [Server] public int AbsorbDamageWithShield(int incoming)
        {
            // shield inactive? -> nothing absorbed
            if (!(_packMgr && _packMgr.Active && _packMgr.CurrentId == PackId.Shield))
                return incoming;

            // How much can we pay?
            int absorb = Mathf.Min(incoming, Mathf.CeilToInt(Energy));

            // Burn that energy
            _energyModule.ConsumeForced(absorb);

            // Drop shield immediately if empty
            if (Energy <= 0f) _packMgr.ForceActive(false);

            // Return un-absorbed remainder (may be zero)
            return incoming - absorb;
        }
        
        [Server]
        public void ServerSpendEnergy(float amount)
        {
            _energyModule.ConsumeForced(amount);
        }
        #endregion

        void SetPhysicMaterial(PhysicsMaterial pm)
        {
            if (_col == null || pm == null)
                return;
            
            if (_col.sharedMaterial != pm)
                _col.sharedMaterial = pm;
        }
        
        [ObserversRpc(ExcludeOwner = true, BufferLast = false)]
        private void RpcNotifyObserverPoseReset(byte sequence, Vector3 position, Quaternion rotation)
        {
            OnObserverPoseResetReceived?.Invoke(sequence, position, rotation);
        }
    }
}
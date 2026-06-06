using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using _Scripts.Game.Teams;
using _Scripts.Player;

namespace _Scripts.Game.CTF
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class FlagObject : NetworkBehaviour
    {
        [Header("Settings")]
        [SerializeField] float autoReturnTime = 20f;
        
        [Header("Drop/Throw")]
        [SerializeField] float deathVelocityInheritance = 0.85f;
        [SerializeField] float deathDropUpBias = 4f;
        [SerializeField] float deathDropForwardBias = 2f;
        
        [Header("Throw")]
        [SerializeField] float throwForce = 28f;
        [SerializeField] float throwUpBias = 4f;
        [SerializeField] float throwVelocityInheritance = 0.65f;
        [SerializeField] float throwSpawnOffset = 0.8f;

        [Header("Pickup Lockout")]
        [SerializeField] float pickupLockoutAfterDrop = 0.25f;
        
        [Header("Player Touch Sweep")]
        [SerializeField] LayerMask playerTouchMask = ~0;
        [SerializeField] float playerTouchRadius = 0.6f;
        [SerializeField] int maxPlayerTouchHits = 8;
        
        [Header("Impulse")]
        [SerializeField] float maxImpulseSpeed = 45f;


        readonly SyncVar<FlagState> _state = new();
        readonly SyncVar<NetworkObject> _carrierNob = new(null);
        readonly SyncVar<TeamId> _team = new(TeamId.None);
        readonly SyncVar<Vector3> _homePosition = new();
        readonly SyncVar<Quaternion> _homeRotation = new();

        Rigidbody _rb;

        FlagStand _homeStand;
        FlagCarrier _carrier;
        FlagCarrier _localCarrier;
        FlagMover _mover;
        NetworkObject _ignoredPickupNob;
        Collider _pickupCollider;
        Collider[] _playerTouchHits;
        float _ignoredPickupUntil;
        float _returnTimer;
        FlagVisualSmoother _visualSmoother;
        
        public FlagState State => _state.Value;

        public TeamId Team => _team.Value;

        void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _mover = GetComponent<FlagMover>();
            _pickupCollider = GetComponent<Collider>();
            _playerTouchHits = new Collider[maxPlayerTouchHits];
            _visualSmoother = GetComponent<FlagVisualSmoother>();

            _carrierNob.OnChange += OnCarrierChanged;
            _state.OnChange += OnStateChanged;
        }

        void OnDestroy()
        {
            _carrierNob.OnChange -= OnCarrierChanged;
            _state.OnChange -= OnStateChanged;
        }
        
        void OnCarrierChanged(NetworkObject prev, NetworkObject next, bool asServer)
        {
            _localCarrier = null;

            if (next != null)
                next.TryGetComponent(out _localCarrier);
        }
        
        void OnStateChanged(FlagState prev, FlagState next, bool asServer)
        {
            RefreshColliderState(next);

            if (_visualSmoother != null)
                _visualSmoother.SetSmoothing(next == FlagState.Dropped);
        }

        void RefreshColliderState(FlagState state)
        {
            if (_pickupCollider == null)
                return;

            _pickupCollider.enabled = state != FlagState.Carried;
        }

        [Server]
        public void Server_Initialize(FlagStand stand)
        {
            _homeStand = stand;
            _team.Value = stand.Team;

            _homePosition.Value = stand.HomePoint.position;
            _homeRotation.Value = stand.HomePoint.rotation;

            Server_ReturnHome();
            RefreshColliderState(_state.Value);
        }

        void Update()
        {
            if (IsServer)
                Server_Update();
        }

        void LateUpdate()
        {
            if (_state.Value == FlagState.Carried)
                FollowCarrierAnchor();
            else if (_state.Value == FlagState.Home)
                FollowHomePoint();
        }
        
        void FollowCarrierAnchor()
        {
            FlagCarrier carrier = IsServer ? _carrier : _localCarrier;

            if (carrier == null || carrier.CarryAnchor == null)
                return;

            transform.SetPositionAndRotation(
                carrier.CarryAnchor.position,
                carrier.CarryAnchor.rotation);
        }

        void FollowHomePoint()
        {
            transform.SetPositionAndRotation(
                _homePosition.Value,
                _homeRotation.Value);
        }

        [Server]
        void Server_Update()
        {
            if (_state.Value != FlagState.Dropped)
                return;

            _returnTimer -= Time.deltaTime;

            if (_returnTimer <= 0f)
                Server_ReturnHome();
        }

        [Server]
        public void Server_Pickup(FlagCarrier carrier)
        {
            if (carrier == null)
                return;

            _mover?.Server_Stop();
            RpcStopDropped();

            _carrier = carrier;
            _carrierNob.Value = carrier.NetworkObject;

            carrier.Server_SetFlag(this);

            _state.Value = FlagState.Carried;
            
            RefreshColliderState(_state.Value);

            SetKinematicFlagBody();
            
            _visualSmoother?.Snap();
        }

        [Server]
        public void Server_DropFromCarrier()
        {
            if (_carrier == null)
                return;

            Transform anchor = _carrier.CarryAnchor;

            Vector3 carrierVelocity = Vector3.zero;

            if (_carrier.TryGetComponent(out Rigidbody carrierRb))
                carrierVelocity = carrierRb.linearVelocity;

            Vector3 dropPos = transform.position;
            Quaternion dropRot = transform.rotation;

            if (anchor != null)
            {
                dropPos = anchor.position + anchor.forward * 0.75f + Vector3.up * 0.25f;
                dropRot = anchor.rotation;
            }

            Vector3 forward = anchor != null
                ? anchor.forward
                : transform.forward;

            Vector3 initialVelocity =
                carrierVelocity * deathVelocityInheritance +
                forward * deathDropForwardBias +
                Vector3.up * deathDropUpBias;

            NetworkObject previousCarrierNob = _carrier.NetworkObject;
            
            _carrier.Server_ClearFlag(this);
            _carrier = null;
            _carrierNob.Value = null;

            _state.Value = FlagState.Dropped;
            
            RefreshColliderState(_state.Value);

            SetKinematicFlagBody();

            transform.SetPositionAndRotation(dropPos, dropRot);

            _returnTimer = autoReturnTime;
            
            IgnorePickupFrom(previousCarrierNob, pickupLockoutAfterDrop);
            
            uint startTick = TimeManager.Tick;

            _mover?.Server_BeginMove(dropPos, initialVelocity);
            RpcBeginDropped(dropPos, initialVelocity, startTick);
        }

        [Server]
        public void Server_ReturnHome()
        {
            if (_carrier != null)
            {
                _carrier.Server_ClearFlag(this);
                _carrier = null;
            }

            _carrierNob.Value = null;
            _state.Value = FlagState.Home;
            
            RefreshColliderState(_state.Value);

            _mover?.Server_Stop();
            RpcStopDropped();

            SetKinematicFlagBody();

            Vector3 pos = _homePosition.Value;
            Quaternion rot = _homeRotation.Value;

            _rb.position = pos;
            _rb.rotation = rot;
            transform.SetPositionAndRotation(pos, rot);
            _visualSmoother?.Snap();
        }
        
        [ObserversRpc(BufferLast = true)]
        void RpcBeginDropped(Vector3 position, Vector3 velocity, uint startTick)
        {
            if (IsServer)
                return;

            _mover?.Client_BeginMove(position, velocity, startTick);
        }

        [ObserversRpc(BufferLast = true)]
        void RpcStopDropped()
        {
            if (IsServer)
                return;

            _mover?.Client_Stop();
            _visualSmoother?.Snap();
        }
        
        [Server]
        public void Server_ThrowFromCarrier(FlagCarrier carrier, uint clientTick)
        {
            if (carrier == null)
                return;

            if (_carrier != carrier)
                return;

            Transform anchor = carrier.CarryAnchor;

            uint serverNow = TimeManager.Tick;
            uint target = clientTick;

            if (target >= serverNow)
                target = serverNow > 0 ? serverNow - 1 : 0;

            Vector3 throwDir;
            Vector3 inheritedVelocity;
            Vector3 startPos;
            Quaternion startRot;

            if (TryGetCarrierSnapshot(carrier.NetworkObject, target, serverNow, out LagCompensationManager.FireSnapshot snap))
            {
                throwDir = snap.Direction.normalized;
                inheritedVelocity = snap.Velocity * throwVelocityInheritance;
                startPos = snap.Position + throwDir * throwSpawnOffset + Vector3.up * 0.15f;
                Vector3 flatDir = throwDir;
                flatDir.y = 0f;

                startRot = flatDir.sqrMagnitude > 0.001f
                    ? Quaternion.LookRotation(flatDir.normalized, Vector3.up)
                    : Quaternion.identity;
            }
            else
            {
                throwDir = anchor != null ? anchor.forward : transform.forward;
                inheritedVelocity = Vector3.zero;
                startPos = anchor != null
                    ? anchor.position + throwDir * throwSpawnOffset + Vector3.up * 0.15f
                    : transform.position;

                startRot = anchor != null
                    ? anchor.rotation
                    : transform.rotation;
            }

            Vector3 initialVelocity =
                throwDir * throwForce +
                Vector3.up * throwUpBias +
                inheritedVelocity;

            NetworkObject previousCarrierNob = carrier.NetworkObject;
            
            carrier.Server_ClearFlag(this);

            _carrier = null;
            _carrierNob.Value = null;

            _state.Value = FlagState.Dropped;
            
            RefreshColliderState(_state.Value);

            SetKinematicFlagBody();

            transform.SetPositionAndRotation(startPos, startRot);

            _returnTimer = autoReturnTime;
            IgnorePickupFrom(previousCarrierNob, pickupLockoutAfterDrop);
            
            uint startTick = TimeManager.Tick;

            _mover?.Server_BeginMove(startPos, initialVelocity);
            RpcBeginDropped(startPos, initialVelocity, startTick);
        }
        
        [Server]
        bool TryGetCarrierSnapshot(
            NetworkObject carrierNob,
            uint targetTick,
            uint serverNow,
            out LagCompensationManager.FireSnapshot snap)
        {
            if (carrierNob == null || LagCompensationManager.Instance == null)
            {
                snap = default;
                return false;
            }

            if (LagCompensationManager.Instance.TryGetSnapshot(carrierNob, targetTick, out snap, 0))
                return true;

            if (targetTick > 0 &&
                LagCompensationManager.Instance.TryGetSnapshot(carrierNob, targetTick - 1, out snap, 0))
                return true;

            if (LagCompensationManager.Instance.TryGetSnapshot(carrierNob, targetTick + 1, out snap, 0))
                return true;

            if (LagCompensationManager.Instance.TryGetSnapshot(carrierNob, targetTick, out snap, 2))
                return true;

            uint last = serverNow > 0 ? serverNow - 1 : 0;
            return LagCompensationManager.Instance.TryGetSnapshot(carrierNob, last, out snap, 2);
        }
        
        [Server]
        void IgnorePickupFrom(NetworkObject nob, float seconds)
        {
            _ignoredPickupNob = nob;
            _ignoredPickupUntil = Time.time + seconds;
        }

        [Server]
        bool IsIgnoredPickup(NetworkObject nob)
        {
            return nob != null &&
                   _ignoredPickupNob == nob &&
                   Time.time < _ignoredPickupUntil;
        }

        void OnTriggerEnter(Collider other)
        {
            if (!IsServer)
                return;

            Server_TryTouchPlayer(other);
        }
        
        [Server]
        bool Server_TryTouchPlayer(Collider other)
        {
            if (_state.Value == FlagState.Carried)
                return false;

            PlayerIdentity identity = other.GetComponentInParent<PlayerIdentity>();
            if (identity == null)
                return false;

            if (IsIgnoredPickup(identity.NetworkObject))
                return false;

            PlayerHealth hp = other.GetComponentInParent<PlayerHealth>();
            if (hp == null || hp.IsDead || !hp.CanPickup)
                return false;

            FlagCarrier carrier = other.GetComponentInParent<FlagCarrier>();
            if (carrier == null)
                return false;

            // Same-team interaction: return own dropped flag.
            // This must happen before checking if the player can carry another flag.
            if (identity.Team == Team)
            {
                if (_state.Value == FlagState.Dropped)
                {
                    Server_ReturnHome();
                    return true;
                }

                return false;
            }

            // Enemy interaction: pick up enemy flag if able.
            if (!carrier.Server_CanCarryFlag())
                return false;

            Server_Pickup(carrier);
            return true;
        }
        
        [Server]
        public void Server_CheckPlayerTouchAlongPath(Vector3 from, Vector3 to, float moverRadius)
        {
            if (_state.Value != FlagState.Dropped)
                return;
        
            Vector3 delta = to - from;
            float distance = delta.magnitude;
        
            float radius = Mathf.Max(playerTouchRadius, moverRadius);
        
            int hitCount;
        
            if (distance <= 0.001f)
            {
                hitCount = Physics.OverlapSphereNonAlloc(
                    to,
                    radius,
                    _playerTouchHits,
                    playerTouchMask,
                    QueryTriggerInteraction.Collide);
            }
            else
            {
                Vector3 center = (from + to) * 0.5f;
                float capsuleHalf = distance * 0.5f;
                Vector3 dir = delta / distance;
        
                Vector3 p1 = center - dir * capsuleHalf;
                Vector3 p2 = center + dir * capsuleHalf;
        
                hitCount = Physics.OverlapCapsuleNonAlloc(
                    p1,
                    p2,
                    radius,
                    _playerTouchHits,
                    playerTouchMask,
                    QueryTriggerInteraction.Collide);
            }
        
            for (int i = 0; i < hitCount; i++)
            {
                Collider hit = _playerTouchHits[i];
                if (hit == null)
                    continue;
        
                if (Server_TryTouchPlayer(hit))
                    break;
            }
        
            // Clear refs from reusable buffer.
            for (int i = 0; i < hitCount; i++)
                _playerTouchHits[i] = null;
        }
        
        // KNOCKBACK
        
        [Server]
        public void Server_ApplyWeaponImpulse(Vector3 impulse)
        {
            if (_state.Value != FlagState.Dropped)
                return;

            if (_mover == null)
                return;

            _mover.Server_AddImpulse(impulse, maxImpulseSpeed);

            RpcApplyWeaponImpulse(impulse, maxImpulseSpeed, TimeManager.Tick);
        }

        [ObserversRpc(BufferLast = false)]
        void RpcApplyWeaponImpulse(Vector3 impulse, float maxSpeed, uint impulseTick)
        {
            if (IsServer)
                return;

            _mover?.Client_AddImpulse(impulse, maxSpeed, impulseTick);
        }
        
        // HELPERS
        void SetKinematicFlagBody()
        {
            if (_rb == null)
                return;

            if (!_rb.isKinematic)
            {
                _rb.linearVelocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            }

            _rb.isKinematic = true;
        }
        
        [Server]
        void DebugFlagState(string source)
        {
            Debug.Log(
                $"[FlagObject:{source}] " +
                $"team={Team}, state={_state.Value}, " +
                $"carrier={(_carrier != null ? _carrier.name : "null")}, " +
                $"carrierNob={(_carrierNob.Value != null ? _carrierNob.Value.name : "null")}, " +
                $"colliderEnabled={(_pickupCollider != null && _pickupCollider.enabled)}, " +
                $"colliderTrigger={(_pickupCollider != null && _pickupCollider.isTrigger)}, " +
                $"layer={LayerMask.LayerToName(gameObject.layer)}, " +
                $"pos={transform.position}, " +
                $"moverMoving={(_mover != null && _mover.IsMoving)}, " +
                $"moverVel={(_mover != null ? _mover.Velocity.ToString() : "null")}");
        }
    }
}
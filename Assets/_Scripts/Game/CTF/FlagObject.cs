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
        float _ignoredPickupUntil;
        float _returnTimer;

        public FlagState State => _state.Value;

        public TeamId Team => _team.Value;

        void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _mover = GetComponent<FlagMover>();
            _pickupCollider = GetComponent<Collider>();

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
        void UpdateCarriedPosition()
        {
            if (_carrier == null)
                return;

            Transform anchor = _carrier.CarryAnchor;

            if (anchor == null)
                return;

            transform.SetPositionAndRotation(
                anchor.position,
                anchor.rotation);
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

            _rb.isKinematic = true;
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }

        [Server]
        public void Server_DropFromCarrier()
        {
            Debug.Log($"[FlagObject] DropFromCarrier called for {Team} flag. Carrier={_carrier?.name}");
            
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

            _rb.isKinematic = true;
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;

            transform.SetPositionAndRotation(dropPos, dropRot);

            _returnTimer = autoReturnTime;
            
            IgnorePickupFrom(previousCarrierNob, pickupLockoutAfterDrop);
            
            uint startTick = TimeManager.Tick;

            _mover?.Server_BeginMove(dropPos, initialVelocity);
            RpcBeginDropped(dropPos, initialVelocity, startTick);

            Debug.Log($"[FlagObject] {Team} flag dropped. State={_state.Value}, Pos={dropPos}, Vel={initialVelocity}");
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

            _mover?.Server_Stop();
            RpcStopDropped();

            _rb.isKinematic = true;
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;

            Vector3 pos = _homePosition.Value;
            Quaternion rot = _homeRotation.Value;

            _rb.position = pos;
            _rb.rotation = rot;
            transform.SetPositionAndRotation(pos, rot);
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

            _rb.isKinematic = true;
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;

            transform.SetPositionAndRotation(startPos, startRot);

            _returnTimer = autoReturnTime;
            IgnorePickupFrom(previousCarrierNob, pickupLockoutAfterDrop);
            
            uint startTick = TimeManager.Tick;

            _mover?.Server_BeginMove(startPos, initialVelocity);
            RpcBeginDropped(startPos, initialVelocity, startTick);

            Debug.Log($"[FlagObject] {Team} flag thrown. Pos={startPos}, Vel={initialVelocity}");
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

            if (_state.Value == FlagState.Carried)
                return;

            PlayerIdentity identity = other.GetComponentInParent<PlayerIdentity>();
            if (identity == null)
                return;

            if (IsIgnoredPickup(identity.NetworkObject))
                return;

            PlayerHealth hp = other.GetComponentInParent<PlayerHealth>();
            if (hp == null || hp.IsDead || !hp.CanPickup)
                return;

            FlagCarrier carrier = other.GetComponentInParent<FlagCarrier>();
            if (carrier == null)
                return;

            // Same-team interaction: return own dropped flag.
            // This must happen BEFORE checking whether the player can carry another flag.
            if (identity.Team == Team)
            {
                if (_state.Value == FlagState.Dropped)
                    Server_ReturnHome();

                return;
            }

            // Enemy flag interaction: only now do we care whether player can carry.
            if (!carrier.Server_CanCarryFlag())
                return;

            Server_Pickup(carrier);
        }
    }
}
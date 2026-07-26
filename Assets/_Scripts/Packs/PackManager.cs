// _Scripts/Packs/PackManager.cs
using System;
using _Scripts.FNPool;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using _Scripts.Player;
using _Scripts.Game;
using _Scripts.Weapons;

namespace _Scripts.Packs
{
    public sealed class PackManager : NetworkBehaviour
    {
        /* ───────── Sync-byte  (3 bits id • 1 bit active) ───────── */
        readonly SyncVar<byte> _packByte = new(0);

        public PackId CurrentId => (PackId)(_packByte.Value & 0b0000_0111);
        public bool Active => (_packByte.Value & 0b0000_1000) != 0;
        static byte Compose(PackId id, bool on) => (byte)((byte)id | (on ? 0b1000 : 0));

        public bool HasPack => CurrentId != PackId.None;
        public PackDefinition CurrentDef { get; private set; }

        /* ───────── visuals ───────── */
        [SerializeField] Transform packAnchor;
        NetworkObject heldNob; // third-person model
        
        [Header("Drop Settings")]
        [SerializeField] float dropOffset = 1.0f;
        [SerializeField] float dropSafetyRadius = 0.2f;
        [SerializeField] float dropBackoff = 0.02f;
        [SerializeField] LayerMask dropBlockMask = ~0;
        [SerializeField] float dropTossForward = 15f;
        [SerializeField] float pickupArmDelay = 0.5f;
        [SerializeField] [Range(0f, 1f)] private float dropVelocityInheritance = 0.5f;
        [SerializeField] private float terminalDropTossForward = 7f;
        [SerializeField] private float terminalDropTossUpward = 2f;

        /* ───────── input & HUD ───────── */
        InputHandler _ih;
        public event Action<PackId, bool> OnPackChanged;
        
        AdvancedPredictedController _controller;

        /* ================================================================== */

        #region Unity lifecycle

        void Awake()
        {
            _ih = GetComponent<InputHandler>();
            
            _controller = GetComponent<AdvancedPredictedController>();

            // Cache the correct ScriptableObject every time the sync-byte changes
            _packByte.OnChange += OnPackByteChanged;
        }

        void OnDestroy() => _packByte.OnChange -= OnPackByteChanged;

        void OnPackByteChanged(byte prev, byte next, bool asServer)
        {
            CurrentDef = PackDatabase.Get(CurrentId); // local SO lookup
            OnPackChanged?.Invoke(CurrentId, Active);
        }

        #endregion

        /* ================================================================== */

        void Update()
        {
            if (!IsOwner) return;

            if (_ih.ConsumePackToggle())
                Server_RequestToggle();

            if (_ih.ConsumePackDrop())
                Server_RequestDrop();
        }

        /* ================================================================== */

        #region Pickup / Drop

        [Server]
        public bool Server_GivePack(PackDefinition def)
        {
            if (def == null || def.heldPrefab == null)
                return false;

            if (HasPack)
                return false;

            if (packAnchor == null)
            {
                Debug.LogError("[PackManager] PackAnchor is not assigned.", this);

                return false;
            }

            /*
             * Clean any stale held object left behind by an earlier malformed
             * state before assigning a new pack.
             */
            if (heldNob != null)
                Server_ClearHeldPackState();

            NetworkObject nob = PoolUtil.TakeFromPool(def.heldPrefab);

            if (nob == null)
                return false;

            nob.transform.SetParent(packAnchor, false);
            nob.transform.localPosition = Vector3.zero;
            nob.transform.localRotation = Quaternion.identity;
            nob.transform.localScale = Vector3.one;

            ServerManager.Spawn(nob, Owner);

            heldNob = nob;
            CurrentDef = def;
            _packByte.Value = Compose(def.id, false);

            RpcSetHeld(nob);

            return true;
        }

        [ServerRpc(RequireOwnership = true)]
        void Server_RequestDrop()
        {
            WorldDropContext context = BuildManualDropContext();

            Server_TryDropPack(terminalDrop: false, context);
        }
        
        [Server]
        WorldDropContext BuildManualDropContext()
        {
            if (_controller != null && _controller.Server_TryGetLatestAuthoritativeFirePose(out FirePose pose))
                return new WorldDropContext(pose.Position, pose.Direction, pose.Velocity);
            

            Rigidbody playerBody = GetComponent<Rigidbody>();

            return new WorldDropContext(packAnchor != null ? packAnchor.position : transform.position + Vector3.up,
                transform.forward, playerBody != null ? playerBody.linearVelocity : Vector3.zero);
        }

        [Server]
        public void Server_Drop()
        {
            Rigidbody playerBody = GetComponent<Rigidbody>();

            WorldDropContext context = new WorldDropContext(transform.position + Vector3.up,
                    transform.forward, playerBody != null ? playerBody.linearVelocity : Vector3.zero);

            Server_TryDropPack(terminalDrop: true, context);
        }
        
        [Server]
        bool Server_TryDropPack(bool terminalDrop, WorldDropContext context)
        {
            if (!HasPack)
                return false;

            if (CurrentDef == null)
            {
                Debug.LogError("[PackManager] Pack SyncVar indicates a held pack, but CurrentDef is null.",
                    this);

                if (terminalDrop)
                    Server_ClearHeldPackState();

                return false;
            }

            PackDefinition definition = CurrentDef;

            if (definition.groundPrefab == null)
            {
                Server_HandlePackDropFailure(terminalDrop, definition, "Ground prefab is not assigned.");

                return false;
            }

            if (!definition.groundPrefab.TryGetComponent(out PackPickup _))
            {
                Server_HandlePackDropFailure(terminalDrop, definition, "Ground prefab has no PackPickup component.");

                return false;
            }

            Vector3 dropDirection = WorldDropUtil.GetSafeDirection(context.Direction, transform.forward);

            Vector3 dropPosition = WorldDropUtil.ResolveSafePosition(transform, context.Origin, dropDirection,
                    dropOffset, dropSafetyRadius, dropBackoff, dropBlockMask);

            NetworkObject ground = PoolUtil.TakeFromPool(definition.groundPrefab);

            if (ground == null)
            {
                Server_HandlePackDropFailure(terminalDrop, definition, "No pooled ground object was available.");

                return false;
            }

            if (!ground.TryGetComponent(out PackPickup packPickup))
            {
                Debug.LogError($"[PackManager] Pooled ground object for " + $"'{definition.name}' has no PackPickup component.", ground);

                Destroy(ground.gameObject);

                Server_HandlePackDropFailure(terminalDrop, definition, "Pooled ground object was malformed.");

                return false;
            }

            if (ground.TryGetComponent(out _Scripts.Pickups.Spawning.SpawnedPickupLink link))
                link.Clear();
            
            ground.transform.SetPositionAndRotation(dropPosition, Quaternion.identity);

            ServerManager.Spawn(ground);

            RoundScopedUtil.MarkRoundScoped(ground);

            if (ground.TryGetComponent(out _Scripts.GamePhysics.KinematicMover mover))
            {
                float tossSpeed = terminalDrop ? terminalDropTossForward : dropTossForward;

                Vector3 tossVelocity = context.PlayerVelocity * dropVelocityInheritance;

                tossVelocity += dropDirection * tossSpeed;

                if (terminalDrop)
                    tossVelocity += Vector3.up * terminalDropTossUpward;
                
                mover.InitVelocity(tossVelocity);
            }

            packPickup.Arm(pickupArmDelay);

            if (ground.TryGetComponent(out TimedDespawn timedDespawn))
                timedDespawn.ArmDefault();

            Server_ClearHeldPackState();

            return true;
        }

        [Server]
        private void Server_HandlePackDropFailure(bool terminalDrop, PackDefinition definition, string reason)
        {
            string packName = definition != null ? definition.name : "Unknown pack";

            if (!terminalDrop)
            {
                Debug.LogWarning($"[PackManager] Manual drop cancelled for " + $"'{packName}'. {reason}", this);

                // Manual drop failed: retain the pack and all current state.
                return;
            }

            Debug.LogWarning($"[PackManager] Terminal drop failed for " + $"'{packName}'. {reason} Removing held pack state.", this);

            // Death/disconnect cannot leave an attached held-pack object behind.
            Server_ClearHeldPackState();
        }

        #endregion

        /* ================================================================== */

        #region Toggle on/off

        [Server]
        internal void ForceActive(bool on) // Authoritative on/off from server logic
        {
            if (!HasPack) return;
            _packByte.Value = Compose(CurrentId, on);
        }

        [ServerRpc(RequireOwnership = true)]
        void Server_RequestToggle() // Normal player hotkey path: owner asks server to flip state
        {
            if (!HasPack) return;
            _packByte.Value = Compose(CurrentId, !Active);
        }

        #endregion

        /* ================================================================== */

        #region Visuals

        [ObserversRpc(BufferLast = true, RunLocally = true)]
        void RpcSetHeld(NetworkObject nob)
        {
            if (nob == null || packAnchor == null)
                return;

            nob.transform.SetParent(packAnchor, false);
            nob.transform.localPosition = Vector3.zero;
            nob.transform.localRotation = Quaternion.identity;
            nob.transform.localScale = Vector3.one;
        }

        #endregion
        
        #region Clear/Reset
        [Server]
        public void Server_ClearPackForRoundReset()
        {
            Server_ClearHeldPackState();
        }

        [Server]
        public void Server_ClearPackForTeardown()
        {
            Server_ClearHeldPackState();
        }
        
        [Server]
        void Server_ClearHeldPackState()
        {
            NetworkObject held = heldNob;

            RpcSetHeld(null);

            heldNob = null;
            CurrentDef = null;
            _packByte.Value = 0;

            if (held == null)
                return;

            held.transform.SetParent(null, false);

            if (held.IsSpawned)
                ServerManager.Despawn(held, DespawnType.Pool);
            
        }
        #endregion
        
        #region Coordinator facing API
        [Server]
        public int Server_GetTerminalDropCount()
        {
            return HasPack ? 1 : 0;
        }

        [Server]
        public bool Server_DropTerminal(WorldDropContext context)
        {
            return Server_TryDropPack(terminalDrop: true, context);
        }
        #endregion
    }
}
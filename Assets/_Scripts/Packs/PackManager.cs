// _Scripts/Packs/PackManager.cs
using System;
using _Scripts.FNPool;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using _Scripts.Player;
using _Scripts.Game;

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

        /* ───────── input & HUD ───────── */
        InputHandler _ih;
        public event Action<PackId, bool> OnPackChanged;

        /* ================================================================== */

        #region Unity lifecycle

        void Awake()
        {
            _ih = GetComponent<InputHandler>();

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
            if (HasPack) return false;

            CurrentDef = def;

            if (heldNob)
            {
                heldNob.transform.SetParent(null, false);
                ServerManager.Despawn(heldNob, DespawnType.Pool);
            }

            NetworkObject nob = PoolUtil.TakeFromPool(def.heldPrefab);
            if (nob == null) return false;

            nob.transform.SetParent(packAnchor, false);   // zeroed local TRS
            heldNob = nob;
            

            ServerManager.Spawn(nob, Owner);              // replicate
            _packByte.Value = Compose(def.id, false);
            RpcAttachHeld(nob);
            return true;
        }

        [ServerRpc(RequireOwnership = true)]
        void Server_RequestDrop() => Server_Drop();

        [Server]
        public void Server_Drop()
        {
            if (!HasPack || CurrentDef == null)
                return;

            uint serverNow = TimeManager.Tick;

            LagCompensationManager.FireSnapshot snap;

            if (LagCompensationManager.Instance.TryGetSnapshot(NetworkObject, serverNow, out snap, 0))
            {
            }
            else if (serverNow > 0 && LagCompensationManager.Instance.TryGetSnapshot(NetworkObject, serverNow - 1, out snap, 0))
            {
            }
            else if (LagCompensationManager.Instance.TryGetSnapshot(NetworkObject, serverNow + 1, out snap, 0))
            {
            }
            else if (LagCompensationManager.Instance.TryGetSnapshot(NetworkObject, serverNow, out snap, 2))
            {
            }
            else
            {
                uint last = serverNow > 0 ? serverNow - 1 : 0;
                if (!LagCompensationManager.Instance.TryGetSnapshot(NetworkObject, last, out snap, 2))
                    return;
            }

            Vector3 camPos = snap.Position;
            Vector3 fwd = snap.Direction.normalized;

            Vector3 pos = ResolveSafeDropPosition(camPos, fwd);
            Quaternion rot = Quaternion.identity;

            NetworkObject ground = PoolUtil.TakeFromPool(CurrentDef.groundPrefab);
            if (ground != null)
            {
                if (ground.TryGetComponent(out _Scripts.Pickups.Spawning.SpawnedPickupLink link))
                    link.Clear();
                
                ground.transform.SetPositionAndRotation(pos, rot);
                ServerManager.Spawn(ground);
                RoundScopedUtil.MarkRoundScoped(ground);

                if (ground.TryGetComponent(out _Scripts.GamePhysics.KinematicMover km))
                {
                    Vector3 playerVel = GetComponent<Rigidbody>()?.linearVelocity ?? Vector3.zero;
                    Vector3 tossForward = fwd * dropTossForward;
                    km.InitVelocity(playerVel * 0.5f + tossForward);
                }
                
                if (ground.TryGetComponent(out PackPickup pp))
                    pp.Arm(pickupArmDelay);
                
                if (ground.TryGetComponent(out TimedDespawn td))
                    td.ArmDefault();
            }

            if (heldNob)
            {
                heldNob.transform.SetParent(null, false);
                ServerManager.Despawn(heldNob, DespawnType.Pool);
            }

            heldNob = null;
            CurrentDef = null;
            _packByte.Value = 0; // None / inactive
        }
        
        Vector3 ResolveSafeDropPosition(Vector3 origin, Vector3 forward)
        {
            forward.Normalize();

            Vector3 desiredPos = origin + forward * dropOffset;

            if (Physics.CheckSphere(origin, dropSafetyRadius, dropBlockMask, QueryTriggerInteraction.Ignore))
                return origin;

            Vector3 finalPos = desiredPos;

            if (Physics.SphereCast(origin, dropSafetyRadius, forward, out RaycastHit hit, dropOffset, dropBlockMask, QueryTriggerInteraction.Ignore))
            {
                finalPos = hit.point - forward * dropBackoff;
            }

            if (Physics.CheckSphere(finalPos, dropSafetyRadius, dropBlockMask, QueryTriggerInteraction.Ignore))
            {
                Vector3 nearOrigin = origin + forward * Mathf.Min(dropBackoff, dropOffset * 0.25f);

                if (!Physics.CheckSphere(nearOrigin, dropSafetyRadius, dropBlockMask, QueryTriggerInteraction.Ignore))
                    return nearOrigin;

                return origin;
            }

            return finalPos;
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
        void RpcAttachHeld(NetworkObject nob)
        {
            if (packAnchor)
            {
                nob.transform.SetParent(packAnchor, false);
                nob.transform.localPosition = Vector3.zero;
                nob.transform.localRotation = Quaternion.identity;
                nob.transform.localScale    = Vector3.one;
            }
        }

        #endregion
        
        #region Clear/Reset
        [Server]
        public void Server_ClearPackForRoundReset()
        {
            if (heldNob)
            {
                heldNob.transform.SetParent(null, false);
                ServerManager.Despawn(heldNob, DespawnType.Pool);
            }
        
            heldNob = null;
            CurrentDef = null;
            _packByte.Value = 0;
        }
        #endregion
    }
}
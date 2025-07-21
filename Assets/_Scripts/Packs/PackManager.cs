// _Scripts/Packs/PackManager.cs
using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using _Scripts.Player;          // for InputHandler / PackId

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
            if (!HasPack) return;

            Vector3 pos = transform.position + transform.forward * 5f + Vector3.up * 0.3f;
            Quaternion rot = Quaternion.Euler(90, 0, 0);

            NetworkObject ground = PoolUtil.TakeFromPool(CurrentDef.groundPrefab);
            
            if (ground != null)
            {
                ground.transform.SetPositionAndRotation(pos, rot);
                ServerManager.Spawn(ground);              // show it to everyone
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
                nob.transform.SetParent(packAnchor, false);
        }

        #endregion
    }
}
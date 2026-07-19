// _Scripts/Items/ItemManager.cs
using System;
using System.Runtime.CompilerServices;
using _Scripts.FNPool;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using _Scripts.Player;
using _Scripts.Weapons;

namespace _Scripts.Items
{
    [DisallowMultipleComponent]
    public sealed class ItemManager : NetworkBehaviour
    {
        /* ───────── constants ───────── */
        const int MaxSlots = 4;     // two quick-bar entries
        const int BitsPer  = 6;     // 3-bit id + 3-bit count

        /* ───────── network state ───── */
        readonly SyncVar<uint> _bits = new();
        readonly ItemSlot[]      _slots = new ItemSlot[MaxSlots];

        /* ───────── cached refs ─────── */
        InputHandler _ih;
        Transform    _aimAnchor;               // camera origin (beacon)

        public event Action<int, ItemSlot> OnInventoryChanged;

    /* ═══════════════════════════════════════════════════════════════ */
    #region Unity lifecycle
    void Awake()
    {
        _ih = GetComponent<InputHandler>();

        AdvancedPredictedController controller = GetComponent<AdvancedPredictedController>();

        _aimAnchor = controller != null ? controller.AimAnchor : null;

        _bits.OnChange += OnBitsChanged;
    }

        void OnDestroy() => _bits.OnChange -= OnBitsChanged;
    #endregion
    /* ═══════════════════════════════════════════════════════════════ */

    #region Owner-side hot-keys
        void Update()
        {
            if (!IsOwner || _ih == null) return;

            if (_ih.ConsumeGrenadeUse()) Cmd_RequestGrenade();
            if (_ih.ConsumeMedkitUse ()) Cmd_RequestMedkit ();
            if (_ih.ConsumeBeaconUse ()) Cmd_RequestBeacon ();
        }

        /* one RPC per item keeps server logic clean */
        [ServerRpc(RequireOwnership = true)] void Cmd_RequestGrenade() => Server_DoGrenade();
        [ServerRpc(RequireOwnership = true)] void Cmd_RequestMedkit () => Server_DoMedkit ();
        [ServerRpc(RequireOwnership = true)] void Cmd_RequestBeacon () => Server_DoBeacon ();
    #endregion
    /* ═══════════════════════════════════════════════════════════════ */

    #region Item actions  (server only)
        /* ---- grenade ---- */
        [Server] void Server_DoGrenade()
        {
            if (!Server_Consume(ItemId.Frag)) return;      // deduct here

            if (GetComponentInChildren<GrenadeThrower>(true) is { } gt)
                gt.ArmQuickThrow();                        // spawns next tick
        }

        /* ---- med-kit ---- */
        [Server] void Server_DoMedkit()
        {
            if (!TryGetComponent(out PlayerHealth hp))
                return;
            
            if (hp.Current == hp.Max) // Don't use health kit if at max health
                return;
            
            if (!Server_Consume(ItemId.HealthKit))
                return;                      // inventory empty (shouldn’t happen)

            /* 4) apply the heal */
            hp.ApplyHealOverTime(30, 2f);
        }

        /* ---- beacon ---- */
        [Server] void Server_DoBeacon()
        {
            ItemDefinition def = ItemDatabase.Get(ItemId.Beacon);

            if (def == null || def.useSpawnPrefab == null || _aimAnchor == null)
                return;

            Vector3 start = _aimAnchor.position;

            Vector3 dir = _aimAnchor.forward;
            
            const float maxRange = 10f;

            if (!Physics.Raycast(start, dir, out var hit, maxRange, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                return; 

            if (!Server_Consume(ItemId.Beacon)) return;  // deduct after fail check
            

            NetworkObject nob = PoolUtil.TakeFromPool(def.useSpawnPrefab);
            if (nob == null) return;

            Vector3 pos = hit.point + hit.normal * 0.01f;            // lift 1cm
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
            
            nob.transform.SetPositionAndRotation(hit.point, rot);
            ServerManager.Spawn(nob, Owner);

            /* 3) auto‑despawn after 60 s (adjust as needed) */
            //StartCoroutine(DespawnLater(nob, 60f));
        }

        /* ---- generic consume helper ---- */
        [Server] bool Server_Consume(ItemId id)
        {
            int slot = FindSlotById(id);
            if (slot < 0) return false;

            ref ItemSlot s = ref _slots[slot];
            if (s.Count == 0) return false;

            if (--s.Count == 0) s.Def = null;
            PackBits();
            return true;
        }

        /* wrappers for other classes (GrenadeThrower) */
        [Server] public bool Server_ConsumeGrenade()  => Server_Consume(ItemId.Frag);
        [Server] public bool Server_ConsumeMedkit ()  => Server_Consume(ItemId.HealthKit);
        [Server] public bool Server_ConsumeBeacon ()  => Server_Consume(ItemId.Beacon);
    #endregion
    /* ═══════════════════════════════════════════════════════════════ */

    #region Pick-up entry point
        [Server]
        public bool Server_GiveItem(ItemDefinition def)
        {
            /* global cap across all stacks */
            int current = 0;
            foreach (var s in _slots)
                if (s.Def && s.Def.id == def.id) current += s.Count;
            if (current >= def.maxStack) return false;

            int slot = FindStackOrEmpty(def.id);
            if (slot < 0) return false;

            ref ItemSlot p = ref _slots[slot];
            if (p.Def == null) p.Def = def;
            p.Count++;

            PackBits();
            return true;
        }
    #endregion
    /* ═══════════════════════════════════════════════════════════════ */

    #region SyncVar packing
        void PackBits()
        {
            uint b = 0;
            for (int i = 0; i < MaxSlots; ++i)
            {
                var s = _slots[i];
                int sh = i * BitsPer;
                byte id = s.Def ? (byte)s.Def.id : (byte)0;
                byte ct = (byte)Mathf.Clamp(s.Count, 0, 7);

                b |= (uint)((id & 0b111) << sh);
                b |= (uint)((ct & 0b111) << (sh + 3));
            }
            _bits.Value = b;                      // fires OnChange
        }

        void OnBitsChanged(uint _, uint next, bool asServer)
        {
            if (asServer)             // ignore the callback on the authoritative side; slots are already valid
                return; 

            UnpackBits(next);         // ← clients only
            if (IsOwner)
                for (int i = 0; i < MaxSlots; ++i)
                    OnInventoryChanged?.Invoke(i, _slots[i]);
        }


        void UnpackBits(uint b)
        {
            for (int i = 0; i < MaxSlots; ++i)
            {
                int  sh  = i * BitsPer;
                byte id  = (byte)((b >> sh)       & 0b111);
                byte cnt = (byte)((b >> (sh + 3)) & 0b111);

                _slots[i].Def   = id == 0 ? null : ItemDatabase.Get((ItemId)id);
                _slots[i].Count = cnt;
            }
        }
    #endregion
    /* ═══════════════════════════════════════════════════════════════ */

    #region search helpers
        /* first NON-empty stack of that id (else −1) */
        int FindSlotById(ItemId id)
        {
            for (int i = 0; i < MaxSlots; ++i)
                if (_slots[i].Def &&
                    _slots[i].Def.id == id &&
                    _slots[i].Count > 0)
                    return i;
            return -1;
        }

        /* existing stack w/ room OR first empty slot */
        int FindStackOrEmpty(ItemId id)
        {
            int empty = -1;

            for (int i = 0; i < MaxSlots; ++i)
            {
                ref ItemSlot s = ref _slots[i];

                if (empty < 0 && s.Def == null)
                    empty = i;                         // remember first empty

                if (s.Def && s.Def.id == id &&
                    s.Count < s.Def.maxStack)
                    return i;                          // existing w/ space
            }
            return empty;                              // may be -1 if full
        }

        int TotalGrenades()
        {
            int sum = 0;
            for (int i = 0; i < MaxSlots; ++i)
                if (_slots[i].Def && _slots[i].Def.id == ItemId.Frag)
                    sum += _slots[i].Count;
            return sum;
        }
    #endregion
    /* ═══════════════════════════════════════════════════════════════ */

        public struct ItemSlot
        {
            public ItemDefinition Def;
            public byte           Count;
        }
    /* ═══════════════════════════════════════════════════════════════ */        
    #region Clear/Reset
        
        [Server]
        public void Server_ClearItemsForRoundReset()
        {
            for (int i = 0; i < MaxSlots; i++)
                _slots[i] = default;

            PackBits();
        }
    #endregion
    }
}
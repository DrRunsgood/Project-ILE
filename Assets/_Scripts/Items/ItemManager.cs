// _Scripts/Items/ItemManager.cs
using System;
using _Scripts.FNPool;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using _Scripts.Player;
using _Scripts.Weapons;
using _Scripts.Game;
using _Scripts.GamePhysics;

namespace _Scripts.Items
{
    [DisallowMultipleComponent]
    public sealed class ItemManager : NetworkBehaviour
    {
        [Header("Terminal Item Drops")]
        [SerializeField] private float itemDropOffset = 0.65f;
        [SerializeField] private float itemDropSafetyRadius = 0.15f;
        [SerializeField] private float itemDropBackoff = 0.02f;
        [SerializeField] private LayerMask itemDropBlockMask = ~0;

        [SerializeField]
        [Range(0f, 1f)]
        private float itemDropVelocityInheritance = 0.5f;

        [SerializeField] private float itemTerminalTossSpeed = 5f;
        [SerializeField] private float itemTerminalUpwardSpeed = 2.5f;
        [SerializeField] private float itemDropPickupArmDelay = 0.5f;
        
        /* ───────── constants ───────── */
        const int MaxSlots = 4;     // two quick-bar entries
        const int BitsPer  = 6;     // 3-bit id + 3-bit count
        const int MaxPackedCountPerSlot = 7;

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
        
        if (_ih.ConsumeMedkitUse ()) Cmd_RequestMedkit ();
        if (_ih.ConsumeBeaconUse ()) Cmd_RequestBeacon ();
    }

    /* one RPC per item keeps server logic clean */
    [ServerRpc(RequireOwnership = true)] void Cmd_RequestMedkit () => Server_DoMedkit ();
    [ServerRpc(RequireOwnership = true)] void Cmd_RequestBeacon () => Server_DoBeacon ();
    #endregion
    /* ═══════════════════════════════════════════════════════════════ */

    #region Item actions  (server only)
    [Server]
    public void Server_ProcessGrenadeInput(bool grenadePressed, FirePose pose)
    {
        if (!grenadePressed)
            return;

        /*
         * Validate inventory without deducting it.
         */
        if (FindSlotById(ItemId.Frag) < 0)
            return;

        GrenadeThrower grenadeThrower = GetComponentInChildren<GrenadeThrower>(true);

        if (grenadeThrower == null)
        {
            Debug.LogError("[ItemManager] Grenade input received, but no " + "GrenadeThrower exists beneath the player.",
                this);

            return;
        }

        /*
         * Cooldown, FirePoint, projectile prefab, pool acquisition,
         * spawn safety, and authoritative projectile creation are all
         * validated before inventory is consumed.
         */
        if (!grenadeThrower.Server_TryThrowFromPose(pose))
            return;

        /*
         * The server executes this method synchronously. Inventory was
         * verified immediately above and no other code executes between
         * the successful spawn and this deduction.
         */
        if (!Server_Consume(ItemId.Frag))
        {
            Debug.LogError("[ItemManager] Grenade spawned successfully, but its " + "inventory item could not be consumed.", this);
        }
    }
    
    
    /* ---- med-kit ---- */
    [Server] void Server_DoMedkit()
    {
        if (!TryGetComponent(out PlayerHealth hp))
            return;
        
        if (!hp.IsAlive || hp.Current >= hp.Max)
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

    #region Pickup entry point

    [Server]
    public bool Server_GiveItem(ItemDefinition def)
    {
        return Server_AddItems(def, 1) == 1;
    }

    [Server]
    public int Server_AddItems(ItemDefinition def, int requestedCount)
    {
        if (def == null || def.id == ItemId.None || requestedCount <= 0)
            return 0;
        

        int maximumInventoryCount = Mathf.Clamp(def.maxStack, 1, MaxSlots * MaxPackedCountPerSlot);

        int currentCount = GetTotalCount(def.id);
        int availableCapacity = maximumInventoryCount - currentCount;

        if (availableCapacity <= 0)
            return 0;

        int remaining = Mathf.Min(requestedCount, availableCapacity);

        int acceptedTarget = remaining;

        /*
         * Fill existing stacks first.
         */
        for (int i = 0; i < MaxSlots && remaining > 0; i++)
        {
            ref ItemSlot slot = ref _slots[i];

            if (slot.Def == null || slot.Def.id != def.id || slot.Count >= MaxPackedCountPerSlot)
                continue;
            

            int room = MaxPackedCountPerSlot - slot.Count;

            int add = Mathf.Min(remaining, room);

            slot.Count += (byte)add;
            remaining -= add;
        }

        /*
         * Then create new stacks in empty slots.
         */
        for (int i = 0; i < MaxSlots && remaining > 0; i++)
        {
            ref ItemSlot slot = ref _slots[i];

            if (slot.Def != null)
                continue;

            int add = Mathf.Min(remaining, MaxPackedCountPerSlot);

            slot.Def = def;
            slot.Count = (byte)add;
            remaining -= add;
        }

        int accepted = acceptedTarget - remaining;

        if (accepted > 0) PackBits();

        return accepted;
    }
    
    

    #endregion
    /* ═══════════════════════════════════════════════════════════════ */
    
    #region Terminal Item Drops

        [Server]
        public int Server_GetTerminalDropCount()
        {
            int count = 0;

            for (int i = 0; i < MaxSlots; i++)
            {
                ItemSlot slot = _slots[i];

                if (slot.Def != null && slot.Count > 0)
                    count++;
            }

            return count;
        }

        [Server]
        public bool Server_DropNextTerminalItemStack(WorldDropContext context)
        {
            for (int i = 0; i < MaxSlots; i++)
            {
                ItemSlot slot = _slots[i];

                if (slot.Def == null || slot.Count <= 0)
                    continue;

                bool spawned = Server_TrySpawnItemStack(slot.Def, slot.Count, context);

                if (!spawned)
                {
                    Debug.LogWarning($"[ItemManager] Terminal drop failed for '{slot.Def.displayName}' x{slot.Count}. " +
                                     $"Clearing the terminal inventory stack.", this);
                }

                /*
                 * Terminal semantics:
                 * the dead or exiting player cannot retain the stack.
                 */
                _slots[i] = default;
                PackBits();

                return spawned;
            }

            return false;
        }

        [Server]
        bool Server_TrySpawnItemStack(ItemDefinition definition, int count, WorldDropContext context)
        {
            if (definition == null || count <= 0 || definition.worldPickupPrefab == null)
                return false;

            NetworkObject prefab = definition.worldPickupPrefab;

            if (!prefab.TryGetComponent(out ItemPickup _))
            {
                Debug.LogError($"[ItemManager] World pickup prefab '{prefab.name}' has no ItemPickup component.", prefab);

                return false;
            }

            NetworkObject ground = PoolUtil.TakeFromPool(prefab);

            if (ground == null)
                return false;

            if (!ground.TryGetComponent(out ItemPickup itemPickup))
            {
                Debug.LogError($"[ItemManager] Pooled pickup '{ground.name}' has no ItemPickup component.", ground);

                Destroy(ground.gameObject);
                return false;
            }

            if (ground.TryGetComponent(out _Scripts.Pickups.Spawning.SpawnedPickupLink link))
                link.Clear();
            
            if (!WorldDropUtil.TryResolveDrop(transform, context.Origin, context.Direction, itemDropOffset, itemDropSafetyRadius,
                    itemDropBackoff, itemDropBlockMask, out Vector3 position, out Vector3 direction))
            {
                Debug.LogWarning($"[ItemManager] No collision-safe drop position was available for '{definition.displayName}'.",
                    this);

                return false;
            }

            Quaternion rotation = Quaternion.AngleAxis(UnityEngine.Random.Range(0f, 360f), Vector3.up);

            ground.transform.SetPositionAndRotation(position, rotation);

            ServerManager.Spawn(ground);

            /*
             * OnStartServer resets pooled count to one.
             * Apply the exact stored count after spawning.
             */
            itemPickup.ServerSetRuntimeCount(count);

            RoundScopedUtil.MarkRoundScoped(ground);

            if (ground.TryGetComponent(out KinematicMover mover))
            {
                Vector3 tossVelocity = context.PlayerVelocity * itemDropVelocityInheritance;

                tossVelocity += direction * itemTerminalTossSpeed;

                tossVelocity += Vector3.up * itemTerminalUpwardSpeed;

                mover.InitVelocity(tossVelocity, transform);
            }

            itemPickup.Arm(itemDropPickupArmDelay);

            if (ground.TryGetComponent(out TimedDespawn timedDespawn))
                timedDespawn.ArmDefault();

            return true;
        }

#endregion

    #region SyncVar packing
        void PackBits()
        {
            uint b = 0;
            for (int i = 0; i < MaxSlots; ++i)
            {
                var s = _slots[i];
                int sh = i * BitsPer;
                byte id = s.Def ? (byte)s.Def.id : (byte)0;
                byte ct = (byte)Mathf.Clamp(s.Count, 0, MaxPackedCountPerSlot);

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
        
        int GetTotalCount(ItemId id)
        {
            int total = 0;

            for (int i = 0; i < MaxSlots; i++)
            {
                ItemSlot slot = _slots[i];

                if (slot.Def != null && slot.Def.id == id)
                    total += slot.Count;
            
            }

            return total;
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
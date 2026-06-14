// _Scripts/Weapons/Manager/WeaponManager.cs
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using _Scripts.Data;
using _Scripts.GamePhysics;
using _Scripts.Player;
using _Scripts.Game;

namespace _Scripts.Weapons
{
    public sealed class WeaponManager : NetworkBehaviour
    {
        #region Inspector

        [Header("Anchors")]
        [SerializeField] Transform firstPersonAnchor;
        [SerializeField] Transform _anchor;

        [Header("Defaults")]
        [SerializeField] WeaponDefinition[] defaultQuickItems;

        [Header("Drop Settings")]
        [SerializeField] float dropOffset = 1.0f;
        [SerializeField] float dropSafetyRadius = 0.2f;
        [SerializeField] float dropBackoff = 0.02f;
        [SerializeField] LayerMask dropBlockMask = ~0;

        #endregion

        #region Public API

        public Transform WeaponAnchor => _anchor;

        public WeaponDefinition ActiveDefinition
        {
            get
            {
                WeaponInstance inst = _weapons.Find(w => w.NetworkObj == _activeNob.Value);
                if (inst == null)
                    return null;

                if (inst.Def != null)
                    return inst.Def;

                return inst.NetworkObj != null &&
                       inst.NetworkObj.TryGetComponent(out ProjectileWeapon pw)
                    ? pw.Definition
                    : null;
            }
        }

        public int ActiveAmmo => _activeAmmo.Value;
        public int ActiveMaxAmmo => _activeMaxAmmo.Value;

        #endregion

        #region Runtime State

        const int MaxSlots = 3;

        readonly List<WeaponInstance> _weapons = new();
        readonly Dictionary<NetworkObject, GameObject> _fpViews = new();

        readonly SyncVar<NetworkObject> _activeNob = new(null);
        readonly SyncVar<int> _activeAmmo = new(0);
        readonly SyncVar<int> _activeMaxAmmo = new(0);

        InputHandler _ih;

        #endregion

        #region Unity / FishNet Lifecycle

        void Awake()
        {
            EnsureWeaponAnchor();

            _ih = GetComponent<InputHandler>();
            _activeNob.OnChange += OnActiveWeaponChanged;
        }

        void OnDestroy()
        {
            _activeNob.OnChange -= OnActiveWeaponChanged;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            GiveDefaultQuickItems();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (!IsOwner)
                return;
        }
        
        

        public override void OnSpawnServer(NetworkConnection conn)
        {
            base.OnSpawnServer(conn);

            NetworkObject[] list = new NetworkObject[_weapons.Count];
            for (int i = 0; i < _weapons.Count; i++)
                list[i] = _weapons[i].NetworkObj;

            RpcClient_SyncFullInventory(conn, list);
        }

        void EnsureWeaponAnchor()
        {
            if (_anchor != null)
                return;

            Transform gfx = transform.Find("Graphics") ?? transform;
            _anchor = gfx.Find("HeldWeapons") ?? new GameObject("HeldWeapons").transform;
            _anchor.SetParent(gfx, false);
        }

        void OnActiveWeaponChanged(NetworkObject prev, NetworkObject next, bool asServer)
        {
            RefreshActive();
        }

        #endregion
        
        #region Fire Input

        [Server]
        public void Server_ProcessFireInput(InputButtons held, FirePose pose)
        {
            if ((held & InputButtons.Fire) == 0)
                return;

            NetworkObject active = _activeNob.Value;
            if (active == null)
                return;

            if (!active.TryGetComponent(out ProjectileWeapon weapon))
                return;

            // Hidden quick items keep their existing armed/TargetRpc/old-fire path for now.
            if (weapon.isHiddenQuickItem)
                return;

            if (!weapon.IsActive)
                return;

            weapon.Server_TryFireFromPose(pose);
        }

        #endregion

        #region Shared Attach / Visual Helpers

        static void ResetLocal(Transform t)
        {
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
        }

        void HandleAttachLogic(NetworkObject nob)
        {
            if (nob == null)
                return;

            nob.transform.SetParent(_anchor, false);
            ResetLocal(nob.transform);

            if (!_weapons.Exists(w => w.NetworkObj == nob))
                _weapons.Add(new WeaponInstance(null, nob));

            if (!nob.TryGetComponent(out ProjectileWeapon pw))
                return;

            pw.CachePlayerRefs(this, _ih);

            if (pw.isHiddenQuickItem)
                return;

            int defaultLayer = LayerMask.NameToLayer("Default");
            int tpLayer = LayerMask.NameToLayer("TP_Only");

            foreach (Transform t in nob.GetComponentsInChildren<Transform>(true))
                t.gameObject.layer = defaultLayer;

            if (IsOwner)
            {
                foreach (Transform t in nob.GetComponentsInChildren<Transform>(true))
                    t.gameObject.layer = tpLayer;
            }

            if (IsOwner && !_fpViews.ContainsKey(nob) && pw.Definition?.fpViewPrefab)
            {
                GameObject fp = Instantiate(pw.Definition.fpViewPrefab, firstPersonAnchor);
                ResetLocal(fp.transform);
                fp.transform.localScale = Vector3.one * 2f;
                _fpViews[nob] = fp;
            }
        }

        void RefreshActive()
        {
            NetworkObject want = _activeNob.Value;

            foreach (WeaponInstance w in _weapons)
            {
                bool active = w.NetworkObj == want;
                w.SetActive(active);

                if (IsOwner && _fpViews.TryGetValue(w.NetworkObj, out GameObject fp))
                    fp.SetActive(active);
            }
        }

        #endregion

        #region RPCs

        [ObserversRpc(RunLocally = true, BufferLast = true)]
        void RpcAttachHeld(NetworkObject nob)
        {
            HandleAttachLogic(nob);
            RefreshActive();
        }

        [TargetRpc]
        void RpcClient_SyncFullInventory(NetworkConnection _, NetworkObject[] list)
        {
            _weapons.Clear();

            foreach (NetworkObject nob in list)
                HandleAttachLogic(nob);

            RefreshActive();
        }

        [ObserversRpc(RunLocally = false)]
        void RpcRemoveHeld(NetworkObject nob)
        {
            int idx = _weapons.FindIndex(w => w.NetworkObj == nob);
            if (idx >= 0)
                _weapons.RemoveAt(idx);

            if (IsOwner && _fpViews.TryGetValue(nob, out GameObject fp))
            {
                Destroy(fp);
                _fpViews.Remove(nob);
            }

            RefreshActive();
        }

        #endregion

        #region Pickup / Add / Merge

        int ResolveStartingAmmo(WeaponDefinition def, int requestedAmmo)
        {
            if (def == null || !def.usesAmmo)
                return 0;

            int ammo = requestedAmmo >= 0 ? requestedAmmo : def.spawnAmmo;
            return Mathf.Clamp(ammo, 0, def.maxAmmo);
        }

        WeaponInstance FindWeapon(WeaponDefinition def)
        {
            return _weapons.Find(w => w.Def == def);
        }

        [Server]
        public bool Server_AddWeapon(WeaponDefinition def)
        {
            return Server_AddOrMergeWeapon(def, -1);
        }

        [Server]
        public bool Server_AddOrMergeWeapon(WeaponDefinition def, int startingAmmo = -1)
        {
            if (def == null || def.heldPrefab == null)
                return false;

            bool hidden = def.heldPrefab.GetComponent<ProjectileWeapon>()?.isHiddenQuickItem == true;

            WeaponInstance existing = FindWeapon(def);
            if (existing != null)
            {
                if (!def.usesAmmo)
                {
                    Debug.Log($"[WeaponManager] Duplicate rejected: {def.displayName} does not use ammo.");
                    return false;
                }

                int addAmmo = ResolveStartingAmmo(def, startingAmmo);
                int before = existing.CurrentAmmo;

                existing.CurrentAmmo = Mathf.Min(existing.CurrentAmmo + addAmmo, def.maxAmmo);

                if (existing.NetworkObj == _activeNob.Value)
                    UpdateActiveAmmoSync();

                Debug.Log($"[WeaponManager] Merged {def.displayName}: {before} + {addAmmo} => {existing.CurrentAmmo}/{def.maxAmmo}");

                return existing.CurrentAmmo > before;
            }

            if (!hidden && CountRegularWeapons() >= MaxSlots)
            {
                Debug.Log($"[WeaponManager] Pickup rejected: max weapon slots reached for {def.displayName}.");
                return false;
            }

            NetworkObject nob = PoolUtil.TakeFromPool(def.heldPrefab);
            if (nob == null)
                return false;

            nob.transform.SetParent(_anchor, false);
            ResetLocal(nob.transform);
            ServerManager.Spawn(nob, Owner);

            int ammo = ResolveStartingAmmo(def, startingAmmo);
            _weapons.Add(new WeaponInstance(def, nob, ammo));

            RpcAttachHeld(nob);

            if (!hidden && CurrentActiveIsEmptyOrQuickItem())
                SetActiveWeapon(nob);

            return true;
        }

        int CountRegularWeapons()
        {
            int regular = 0;

            foreach (WeaponInstance w in _weapons)
            {
                if (!w.IsQuickItem)
                    regular++;
            }

            return regular;
        }

        bool CurrentActiveIsEmptyOrQuickItem()
        {
            if (_activeNob.Value == null)
                return true;

            return _weapons.Find(w => w.NetworkObj == _activeNob.Value)?.IsQuickItem == true;
        }

        #endregion
        
        #region Selection Helpers
        
        int CountSelectableWeapons()
        {
            int count = 0;

            foreach (WeaponInstance w in _weapons)
                if (!w.IsQuickItem)
                    count++;

            return count;
        }

        int FindSelectableIndex(NetworkObject nob)
        {
            int selectableIndex = 0;

            foreach (WeaponInstance w in _weapons)
            {
                if (w.IsQuickItem)
                    continue;

                if (w.NetworkObj == nob)
                    return selectableIndex;

                selectableIndex++;
            }

            return -1;
        }

        NetworkObject GetSelectableByIndex(int index)
        {
            int selectableIndex = 0;

            foreach (WeaponInstance w in _weapons)
            {
                if (w.IsQuickItem)
                    continue;

                if (selectableIndex == index)
                    return w.NetworkObj;

                selectableIndex++;
            }

            return null;
        }
        
        #endregion

        #region Ammo

        [Server]
        public bool Server_TryConsumeAmmo(WeaponDefinition def, int amount)
        {
            if (def == null)
                return false;

            if (!def.usesAmmo)
                return true;

            if (amount <= 0)
                return true;

            WeaponInstance inst = FindWeapon(def);
            if (inst == null)
                return false;

            if (inst.CurrentAmmo < amount)
                return false;

            inst.CurrentAmmo -= amount;

            if (inst.NetworkObj == _activeNob.Value)
                UpdateActiveAmmoSync();
            
            return true;
        }

        [Server]
        void UpdateActiveAmmoSync()
        {
            WeaponInstance inst = _weapons.Find(w => w.NetworkObj == _activeNob.Value);

            if (inst == null || inst.Def == null || !inst.Def.usesAmmo)
            {
                _activeAmmo.Value = 0;
                _activeMaxAmmo.Value = 0;
                return;
            }

            _activeAmmo.Value = inst.CurrentAmmo;
            _activeMaxAmmo.Value = inst.Def.maxAmmo;
        }
        
        [Server]
        public bool Server_AddAmmo(AmmoType ammoType, int amount)
        {
            if (ammoType == AmmoType.None || amount <= 0)
                return false;

            foreach (WeaponInstance inst in _weapons)
            {
                WeaponDefinition def = inst.Def;

                if (def == null || !def.usesAmmo || def.ammoType != ammoType)
                    continue;

                if (inst.CurrentAmmo >= def.maxAmmo)
                    continue;

                int before = inst.CurrentAmmo;
                inst.CurrentAmmo = Mathf.Min(inst.CurrentAmmo + amount, def.maxAmmo);

                if (inst.NetworkObj == _activeNob.Value)
                    UpdateActiveAmmoSync();

                return inst.CurrentAmmo > before;
            }

            return false;
        }
        
        [Server]
        public bool Server_CanConsumeAmmo(WeaponDefinition def, int amount)
        {
            if (def == null)
                return false;

            if (!def.usesAmmo)
                return true;

            if (amount <= 0)
                return true;

            WeaponInstance inst = FindWeapon(def);
            return inst != null && inst.CurrentAmmo >= amount;
        }

        #endregion

        #region Active Weapon / Selection

        [Server]
        void GiveDefaultQuickItems()
        {
            foreach (WeaponDefinition d in defaultQuickItems)
            {
                if (d)
                    Server_AddWeapon(d);
            }
        }

        [Server]
        void SetActiveWeapon(NetworkObject nob)
        {
            _activeNob.Value = nob;
            UpdateActiveAmmoSync();
        }

        void Update()
        {
            if (!IsOwner)
                return;

            int selectableCount = CountSelectableWeapons();
            if (selectableCount == 0)
                return;

            int cur = FindSelectableIndex(_activeNob.Value);
            if (cur < 0)
                cur = 0;

            int want = cur;

            if (_ih.WeaponSlotInput >= 0)
                want = Mathf.Clamp(_ih.WeaponSlotInput, 0, selectableCount - 1);
            else if (_ih.MouseWheelDelta != 0)
                want = (cur + _ih.MouseWheelDelta + selectableCount) % selectableCount;

            if (want != cur)
                Server_SetActiveByIndex(want);

            if (_ih.ConsumeWeaponDrop())
                Server_RequestDropActive();
            
            ProcessLocalPredictedFireAudio();
        }

        [ServerRpc(RequireOwnership = true)]
        void Server_SetActiveByIndex(int idx)
        {
            NetworkObject nob = GetSelectableByIndex(idx);

            if (nob != null)
                SetActiveWeapon(nob);
        }

        #endregion

        #region Drop

        [ServerRpc(RequireOwnership = true)]
        void Server_RequestDropActive()
        {
            int idx = _weapons.FindIndex(w => w.NetworkObj == _activeNob.Value);
            Server_DropWeapon(idx);
        }

        [Server]
        void Server_DropWeapon(int idx)
        {
            if (idx < 0 || idx >= _weapons.Count)
                return;

            WeaponInstance inst = _weapons[idx];

            if (inst.Def == null)
            {
                _weapons.RemoveAt(idx);
                return;
            }

            uint serverNow = TimeManager.Tick;
            int remainingAmmo = inst.CurrentAmmo;

            if (!TryGetFireSnapshot(serverNow, out LagCompensationManager.FireSnapshot snap))
                return;

            Vector3 pos = ResolveSafeDropPosition(snap.Position, snap.Direction);

            NetworkObject ground = PoolUtil.TakeFromPool(inst.Def.groundPrefab);
            if (ground == null)
            {
                _weapons.RemoveAt(idx);
                return;
            }

            ground.transform.SetPositionAndRotation(pos, Quaternion.identity);
            ServerManager.Spawn(ground);
            RoundScopedUtil.MarkRoundScoped(ground);

            if (ground.TryGetComponent(out KinematicMover km))
            {
                Vector3 playerVel = GetComponent<Rigidbody>()?.linearVelocity ?? Vector3.zero;
                Vector3 tossForward = snap.Direction * 15f;
                km.InitVelocity(playerVel * 0.5f + tossForward);
            }

            if (ground.TryGetComponent(out WeaponPickup wp))
            {
                wp.ServerSetRuntimeAmmo(remainingAmmo);
                wp.Arm(0.5f);
            }

            if (ground.TryGetComponent(out TimedDespawn td))
                td.Arm(5f);

            _weapons.RemoveAt(idx);

            NetworkObject newActive = null;
            foreach (WeaponInstance w in _weapons)
            {
                if (!w.IsQuickItem)
                {
                    newActive = w.NetworkObj;
                    break;
                }
            }

            if (inst.NetworkObj == _activeNob.Value)
                SetActiveWeapon(newActive);

            RpcRemoveHeld(inst.NetworkObj);
            ServerManager.Despawn(inst.NetworkObj, DespawnType.Pool);
        }

        [Server]
        bool TryGetFireSnapshot(uint serverNow, out LagCompensationManager.FireSnapshot snap)
        {
            if (LagCompensationManager.Instance.TryGetSnapshot(NetworkObject, serverNow, out snap, 0))
                return true;

            if (serverNow > 0 && LagCompensationManager.Instance.TryGetSnapshot(NetworkObject, serverNow - 1, out snap, 0))
                return true;

            if (LagCompensationManager.Instance.TryGetSnapshot(NetworkObject, serverNow + 1, out snap, 0))
                return true;

            if (LagCompensationManager.Instance.TryGetSnapshot(NetworkObject, serverNow, out snap, 2))
                return true;

            uint last = serverNow > 0 ? serverNow - 1 : 0;
            return LagCompensationManager.Instance.TryGetSnapshot(NetworkObject, last, out snap, 2);
        }

        Vector3 ResolveSafeDropPosition(Vector3 origin, Vector3 forward)
        {
            forward.Normalize();

            Vector3 desiredPos = origin + forward * dropOffset;

            if (Physics.CheckSphere(origin, dropSafetyRadius, dropBlockMask, QueryTriggerInteraction.Ignore))
                return origin;

            Vector3 finalPos = desiredPos;

            if (Physics.SphereCast(origin, dropSafetyRadius, forward, out RaycastHit hit, dropOffset, dropBlockMask, QueryTriggerInteraction.Ignore))
                finalPos = hit.point - forward * dropBackoff;

            if (Physics.CheckSphere(finalPos, dropSafetyRadius, dropBlockMask, QueryTriggerInteraction.Ignore))
            {
                Vector3 nearOrigin = origin + forward * Mathf.Min(dropBackoff, dropOffset * 0.25f);

                if (!Physics.CheckSphere(nearOrigin, dropSafetyRadius, dropBlockMask, QueryTriggerInteraction.Ignore))
                    return nearOrigin;

                return origin;
            }

            return finalPos;
        }

        [Server]
        public void DropAll()
        {
            for (int i = _weapons.Count - 1; i >= 0; i--)
            {
                if (_weapons[i].Def != null && !_weapons[i].IsQuickItem)
                    Server_DropWeapon(i);
            }
        }

        #endregion

        #region Clear / Reset

        [Server]
        public void Server_ClearWeaponsForRoundReset()
        {
            for (int i = _weapons.Count - 1; i >= 0; i--)
            {
                NetworkObject nob = _weapons[i].NetworkObj;

                RpcRemoveHeld(nob);

                if (nob != null)
                {
                    nob.transform.SetParent(null, false);
                    ServerManager.Despawn(nob, DespawnType.Pool);
                }
            }

            _weapons.Clear();
            SetActiveWeapon(null);

            GiveDefaultQuickItems();
        }

        #endregion

        #region Helper Class

        sealed class WeaponInstance
        {
            public readonly WeaponDefinition Def;
            public readonly NetworkObject NetworkObj;
            public int CurrentAmmo;

            readonly ProjectileWeapon _pw;

            public bool IsQuickItem => _pw && _pw.isHiddenQuickItem;

            public WeaponInstance(WeaponDefinition def, NetworkObject nob, int ammo = 0)
            {
                Def = def;
                NetworkObj = nob;
                CurrentAmmo = ammo;
                _pw = nob.GetComponent<ProjectileWeapon>();
            }

            public void SetActive(bool active)
            {
                if (_pw && _pw.isHiddenQuickItem)
                {
                    _pw.IsActive = active;
                    return;
                }

                NetworkObj.gameObject.SetActive(active);

                if (_pw)
                    _pw.IsActive = active;
            }
        }

        #endregion
        
        public void SetFirstPersonAnchor(Transform anchor)
        {
            if (!IsOwner)
                return;

            if (anchor == null)
                return;

            firstPersonAnchor = anchor;

            foreach (WeaponInstance w in _weapons)
            {
                if (w.NetworkObj == null)
                    continue;

                if (_fpViews.ContainsKey(w.NetworkObj))
                    continue;

                if (!w.NetworkObj.TryGetComponent(out ProjectileWeapon pw))
                    continue;

                if (pw.isHiddenQuickItem)
                    continue;

                if (pw.Definition == null || pw.Definition.fpViewPrefab == null)
                    continue;

                GameObject fp = Instantiate(pw.Definition.fpViewPrefab, firstPersonAnchor);
                ResetLocal(fp.transform);
                fp.transform.localScale = Vector3.one * 2f;
                _fpViews[w.NetworkObj] = fp;
            }

            RefreshActive();
        }
        
        void ProcessLocalPredictedFireAudio()
        {
            if (_ih == null)
                return;

            if ((_ih.HeldButtons & InputButtons.Fire) == 0)
                return;

            NetworkObject active = _activeNob.Value;

            if (active == null)
                return;

            if (!active.TryGetComponent(out ProjectileWeapon weapon))
                return;

            if (weapon.isHiddenQuickItem)
                return;

            Vector3 pos = active.transform.position;
            weapon.Client_TryPlayPredictedFireSfx(pos);
        }
    }
}
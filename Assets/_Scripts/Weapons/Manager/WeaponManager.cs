// _Scripts/Weapons/Manager/WeaponManager.cs
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using _Scripts.Data;
using _Scripts.FNPool;
using _Scripts.GamePhysics;
using _Scripts.Player;
using _Scripts.Game;

namespace _Scripts.Weapons
{
    public sealed class WeaponManager : NetworkBehaviour
    {
        #region Inspector

        [Header("Weapon Anchors")]
        [SerializeField] private Transform firstPersonAnchor;

        [Tooltip("Raw authoritative/networked weapon objects.")]
        [SerializeField] private Transform gameplayWeaponAnchor;

        [Tooltip("Smoothed presentation-only third-person weapon models.")]
        [SerializeField] private Transform thirdPersonWeaponAnchor;
        
        [Tooltip("Presentation pivot beneath GraphicsRoot which receives replicated aim pitch.")]
        [SerializeField] private Transform thirdPersonAimPivot;
        
        [Header("Third-Person Aim Presentation")]
        [SerializeField] private float thirdPersonPitchSmoothTime = 0.035f;

        private float _renderPitch;
        private float _renderPitchVelocity;
        private bool _renderPitchInitialized;

        [Header("Defaults")]
        [SerializeField] WeaponDefinition[] defaultQuickItems;

        [Header("Drop Settings")]
        [SerializeField] float dropOffset = 1.0f;
        [SerializeField] float dropSafetyRadius = 0.2f;
        [SerializeField] float dropBackoff = 0.02f;
        [SerializeField] LayerMask dropBlockMask = ~0;
        #endregion
        
        private AdvancedPredictedController _controller;

        #region Public API

        public Transform WeaponAnchor => gameplayWeaponAnchor;

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
        readonly Dictionary<NetworkObject, GameObject> _tpViews = new();
        readonly SyncVar<NetworkObject> _activeNob = new(null);
        readonly SyncVar<int> _activeAmmo = new(0);
        readonly SyncVar<int> _activeMaxAmmo = new(0);

        InputHandler _ih;
        
        uint _serverActiveWeaponReadyTick;
        uint _localActiveWeaponReadyTick;

        #endregion

        #region Unity / FishNet Lifecycle

        void Awake()
        {
            ValidateWeaponAnchors();
            
            _controller = GetComponent<AdvancedPredictedController>();

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
        
        public override void OnSpawnServer(NetworkConnection conn)
        {
            base.OnSpawnServer(conn);

            NetworkObject[] list = new NetworkObject[_weapons.Count];
            for (int i = 0; i < _weapons.Count; i++)
                list[i] = _weapons[i].NetworkObj;

            RpcClient_SyncFullInventory(conn, list);
        }

        void ValidateWeaponAnchors()
        {
            if (gameplayWeaponAnchor == null)
            {
                Debug.LogError($"{name}: GameplayWeaponAnchor is not assigned.", this);
            }

            if (thirdPersonAimPivot == null)
            {
                Debug.LogError($"{name}: ThirdPersonAimPivot is not assigned.", this);
            }

            if (thirdPersonWeaponAnchor == null)
            {
                Debug.LogError($"{name}: ThirdPersonWeaponAnchor is not assigned.", this);
            }
        }

        void OnActiveWeaponChanged(NetworkObject prev, NetworkObject next, bool asServer)
        {
            RefreshActive();
            
            if (IsOwner)
                _localActiveWeaponReadyTick = CalculateWeaponReadyTick(next);
        }

        #endregion
        
        private void LateUpdate()
        {
            if (_controller == null || thirdPersonAimPivot == null)
                return;

            float targetPitch = _controller.CurrentPitch;

            if (!_renderPitchInitialized)
            {
                _renderPitch = targetPitch;
                _renderPitchVelocity = 0f;
                _renderPitchInitialized = true;
            }
            else if (thirdPersonPitchSmoothTime > 0f)
            {
                _renderPitch = Mathf.SmoothDampAngle(_renderPitch, targetPitch, ref _renderPitchVelocity,
                    thirdPersonPitchSmoothTime, Mathf.Infinity, Time.deltaTime);
            }
            else
            {
                _renderPitch = targetPitch;
            }

            thirdPersonAimPivot.localRotation = Quaternion.Euler(_renderPitch, 0f, 0f);
        }
        
        #region Fire Input

        [Server]
        public void Server_ProcessFireInput(InputButtons held, FirePose pose)
        {
            if ((held & InputButtons.Fire) == 0)
                return;
            
            if (TimeManager.Tick < _serverActiveWeaponReadyTick)
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

            nob.transform.SetParent(gameplayWeaponAnchor, false);

            ResetLocal(nob.transform);

            WeaponInstance weapon = _weapons.Find(w => w.NetworkObj == nob);

            if (weapon == null)
            {
                weapon = new WeaponInstance(null, nob);
                _weapons.Add(weapon);
            }

            if (!nob.TryGetComponent(out ProjectileWeapon projectileWeapon))
                return;
            

            projectileWeapon.CachePlayerRefs(this, _ih);

            if (projectileWeapon.isHiddenQuickItem)
                return;

            SetRenderersEnabled(nob.gameObject, false);

            CreateFirstPersonViewIfNeeded(weapon);

            CreateThirdPersonViewIfNeeded(weapon);
        }
        
        private void CreateFirstPersonViewIfNeeded(WeaponInstance weapon)
        {
            if (!IsOwner)
                return;

            if (firstPersonAnchor == null)
                return;

            if (weapon?.NetworkObj == null)
                return;

            if (_fpViews.ContainsKey(weapon.NetworkObj))
                return;

            if (!weapon.NetworkObj.TryGetComponent(out ProjectileWeapon projectileWeapon))
            {
                return;
            }

            if (projectileWeapon.isHiddenQuickItem)
                return;

            GameObject prefab = projectileWeapon.Definition?.fpViewPrefab;

            if (prefab == null)
                return;

            GameObject view = Instantiate(prefab, firstPersonAnchor, false);

            ResetLocal(view.transform);

            _fpViews[weapon.NetworkObj] =
                view;
        }
        
        private void CreateThirdPersonViewIfNeeded(
            WeaponInstance weapon)
        {
            if (weapon?.NetworkObj == null)
                return;

            if (_tpViews.ContainsKey(weapon.NetworkObj))
                return;

            if (!weapon.NetworkObj.TryGetComponent(out ProjectileWeapon projectileWeapon))
                return;

            if (projectileWeapon.isHiddenQuickItem)
                return;

            GameObject prefab = projectileWeapon.Definition?.tpViewPrefab;

            if (prefab == null || thirdPersonWeaponAnchor == null)
                return;
            

            GameObject view = Instantiate(prefab, thirdPersonWeaponAnchor, false);

            ResetLocal(view.transform);

            int viewLayer = LayerMask.NameToLayer(IsOwner ? "TP_Only" : "Default");

            SetLayerRecursively(view, viewLayer);

            _tpViews[weapon.NetworkObj] =
                view;
        }

        void RefreshActive()
        {
            NetworkObject wanted = _activeNob.Value;

            foreach (WeaponInstance weapon in _weapons)
            {
                bool active = weapon.NetworkObj == wanted;

                weapon.SetActive(active);

                if (IsOwner && _fpViews.TryGetValue(weapon.NetworkObj, out GameObject fpView))
                {
                    fpView.SetActive(active);
                }

                if (_tpViews.TryGetValue(weapon.NetworkObj, out GameObject tpView))
                {
                    tpView.SetActive(active);
                }
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
            
            if (_tpViews.TryGetValue(nob, out GameObject tpView))
            {
                Destroy(tpView);
                _tpViews.Remove(nob);
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

            nob.transform.SetParent(gameplayWeaponAnchor, false);
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
            bool changed = _activeNob.Value != nob;

            _activeNob.Value = nob;

            if (changed)
                _serverActiveWeaponReadyTick = CalculateWeaponReadyTick(nob);

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
        void Server_DropWeapon(int idx, bool allowTransformFallback = false)
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

            Vector3 dropOrigin;
            Vector3 dropDirection;

            if (TryGetFireSnapshot(serverNow, out LagCompensationManager.FireSnapshot snap))
            {
                dropOrigin = snap.Position;
                dropDirection = snap.Direction;
            }
            else if (allowTransformFallback)
            {
                dropOrigin = transform.position + Vector3.up;
                dropDirection = transform.forward;

                if (dropDirection.sqrMagnitude < 0.001f)
                    dropDirection = Vector3.forward;

                Debug.LogWarning("[WeaponManager] No fire snapshot available during drop. Using transform fallback.");
            }
            else
            {
                return;
            }

            Vector3 pos = ResolveSafeDropPosition(dropOrigin, dropDirection);

            NetworkObject ground = PoolUtil.TakeFromPool(inst.Def.groundPrefab);
            if (ground == null)
            {
                _weapons.RemoveAt(idx);
                return;
            }
            
            if (ground.TryGetComponent(out _Scripts.Pickups.Spawning.SpawnedPickupLink link))
                link.Clear();

            ground.transform.SetPositionAndRotation(pos, Quaternion.identity);
            ServerManager.Spawn(ground);
            RoundScopedUtil.MarkRoundScoped(ground);

            if (ground.TryGetComponent(out KinematicMover km))
            {
                Vector3 playerVel = GetComponent<Rigidbody>()?.linearVelocity ?? Vector3.zero;
                Vector3 tossForward = dropDirection.normalized * 15f;
                km.InitVelocity(playerVel * 0.5f + tossForward);
            }

            if (ground.TryGetComponent(out WeaponPickup wp))
            {
                wp.ServerSetRuntimeAmmo(remainingAmmo);
                wp.Arm(0.5f);
            }

            if (ground.TryGetComponent(out TimedDespawn td))
                td.ArmDefault();

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
                    Server_DropWeapon(i, true);
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
        
        WeaponDefinition GetDefinition(NetworkObject nob)
        {
            if (nob == null)
                return null;

            WeaponInstance inst = _weapons.Find(w => w.NetworkObj == nob);

            if (inst != null && inst.Def != null)
                return inst.Def;

            return nob.TryGetComponent(out ProjectileWeapon pw)
                ? pw.Definition
                : null;
        }

        uint CalculateWeaponReadyTick(NetworkObject nob)
        {
            if (TimeManager == null || nob == null)
                return 0;

            WeaponDefinition def = GetDefinition(nob);
            float delay = def != null ? Mathf.Max(0f, def.equipFireDelaySeconds) : 0f;

            if (delay <= 0f)
                return TimeManager.Tick;

            float tickDelta = (float)TimeManager.TickDelta;

            uint delayTicks = (uint)Mathf.Max(
                1,
                Mathf.CeilToInt(delay / tickDelta));

            return TimeManager.Tick + delayTicks;
        }

        #endregion
        
        static void SetRenderersEnabled(GameObject root, bool enabled)
        {
            if (root == null)
                return;

            Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);

            foreach (Renderer renderer in renderers)
            {
                if (renderer != null)
                    renderer.enabled = enabled;
            }
        }
        
        private static void SetLayerRecursively(GameObject root, int layer)
        {
            if (root == null || layer < 0)
                return;

            Transform[] children = root.GetComponentsInChildren<Transform>(true);

            foreach (Transform child in children)
            {
                if (child != null)
                    child.gameObject.layer = layer;
            }
        }
        
        public void SetFirstPersonAnchor(Transform anchor)
        {
            if (!IsOwner || anchor == null)
                return;

            firstPersonAnchor = anchor;

            foreach (WeaponInstance weapon in _weapons)
                CreateFirstPersonViewIfNeeded(weapon);

            RefreshActive();
        }
        
        void ProcessLocalPredictedFireAudio()
        {
            if (_ih == null)
                return;

            if ((_ih.HeldButtons & InputButtons.Fire) == 0)
                return;
            
            if (TimeManager.Tick < _localActiveWeaponReadyTick)
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
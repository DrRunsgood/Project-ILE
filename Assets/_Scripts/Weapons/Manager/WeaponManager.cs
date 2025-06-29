// _Scripts/Weapons/Manager/WeaponManager.cs
using System.Collections.Generic;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using _Scripts.Data;
using _Scripts.Player;

namespace _Scripts.Weapons
{
    public sealed class WeaponManager : NetworkBehaviour
    {
        /* ───────── inspector ───────── */
        [SerializeField] Transform firstPersonAnchor;      // created at runtime
        [SerializeField] Transform _anchor;                // TP models parent

        /* ───────── public API ──────── */
        public Transform  WeaponAnchor => _anchor;

        /* ───────── private fields ──── */
        const int MaxSlots = 3;

        readonly List<WeaponInstance>                  _weapons  = new();
        readonly Dictionary<NetworkObject, GameObject> _fpViews  = new();

        // NEW: active weapon replicated by NetworkObject reference
        readonly SyncVar<NetworkObject> _activeNob = new(null);

        InputHandler _ih;

        /* ═════════════════════════════ */
        #region Unity – setup
        void Awake()
        {
            if (_anchor == null)
            {
                Transform gfx = transform.Find("Graphics") ?? transform;
                _anchor = gfx.Find("HeldWeapons") ??
                          new GameObject("HeldWeapons").transform;
                _anchor.SetParent(gfx, false);
            }

            _ih = GetComponent<InputHandler>();

            _activeNob.OnChange += (_, __, ___) => RefreshActive();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!IsOwner) return;

            // ensure FP anchor
            Transform cam = Camera.main.transform;
            firstPersonAnchor = cam.Find("FirstPersonItems");
            if (firstPersonAnchor == null)
            {
                firstPersonAnchor = new GameObject("FirstPersonItems").transform;
                firstPersonAnchor.SetParent(cam, false);
            }
        }

        void OnDestroy() =>
            _activeNob.OnChange -= (_, __, ___) => RefreshActive();
        #endregion
        /* ═════════════════════════════ */

        #region Pick-up (called by WeaponPickup)
        [Server]
        public bool Server_AddWeapon(WeaponDefinition def)
        {
            if (_weapons.Count >= MaxSlots || HasWeapon(def))
                return false;

            NetworkObject nob = TakeFromPool(def.heldPrefab);
            if (nob == null) return false;
            nob.transform.SetParent(_anchor, false);
            ServerManager.Spawn(nob, Owner);

            _weapons.Add(new WeaponInstance(def, nob));
            RpcAttachHeld(nob);

            // auto-equip if none active
            if (_activeNob.Value == null)
                SetActiveWeapon(nob);

            return true;
        }

        bool HasWeapon(WeaponDefinition d)
        {
            foreach (var w in _weapons)
                if (w.Def == d) return true;
            return false;
        }
        #endregion

        #region RPC – attach held model
        [ObserversRpc(RunLocally = true)]    // removed BufferLast
        void RpcAttachHeld(NetworkObject nob)
        {
            /* 1) layer for owner TP gun */
            if (IsOwner)
            {
                int tpLayer = LayerMask.NameToLayer("TP_Only");
                foreach (Transform t in nob.GetComponentsInChildren<Transform>(true))
                    t.gameObject.layer = tpLayer;
            }

            nob.transform.SetParent(_anchor, false);

            /* 2) local bookkeeping */
            if (_weapons.Find(w => w.NetworkObj == nob) == null)
                _weapons.Add(new WeaponInstance(null, nob));

            if (nob.TryGetComponent(out ProjectileWeapon pw))
                pw.CachePlayerRefs(this, _ih);

            /* 3) owner FP view */
            if (IsOwner && !_fpViews.ContainsKey(nob))
            {
                WeaponDefinition def = pw.Definition;
                if (def && def.fpViewPrefab)
                {
                    GameObject fp = Instantiate(def.fpViewPrefab, firstPersonAnchor);
                    fp.transform.localPosition = Vector3.zero;
                    fp.transform.localRotation = Quaternion.identity;
                    _fpViews[nob] = fp;
                    fp.transform.localScale = Vector3.one * 2f; // DG - scaled to match the real representation, for now
                }
            }

            RefreshActive();
        }
        #endregion

        #region Active weapon helpers
        void RefreshActive()
        {
            NetworkObject wanted = _activeNob.Value;

            foreach (var w in _weapons)
            {
                bool active = w.NetworkObj == wanted;
                w.SetActive(active);

                if (IsOwner && _fpViews.TryGetValue(w.NetworkObj, out var fp))
                    fp.SetActive(active);
            }
        }

        /* server-side setter */
        [Server]
        void SetActiveWeapon(NetworkObject nob) => _activeNob.Value = nob;
        #endregion

        /* ═════════════════════════════ */
        #region Owner input
        void Update()
        {
            if (!IsOwner || _weapons.Count == 0) return;

            int currentIdx = _weapons.FindIndex(w => w.NetworkObj == _activeNob.Value);
            if (currentIdx < 0) currentIdx = 0;

            int wanted = currentIdx;

            if (_ih.WeaponSlotInput >= 0)
                wanted = _ih.WeaponSlotInput;
            else if (_ih.MouseWheelDelta != 0)
                wanted = Mathf.Clamp(
                    (currentIdx + _ih.MouseWheelDelta + _weapons.Count) % _weapons.Count,
                    0, _weapons.Count - 1);

            if (wanted != currentIdx)
                Server_SetActiveByIndex(wanted);

            if (_ih.ConsumeWeaponDrop())
                Server_RequestDropActive();
        }
        #endregion

        #region Slot change RPC
        [ServerRpc(RequireOwnership = true)]
        void Server_SetActiveByIndex(int idx)
        {
            if (idx >= 0 && idx < _weapons.Count)
                SetActiveWeapon(_weapons[idx].NetworkObj);
        }
        #endregion

        /* ═════════════════════════════ */
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
            if (idx < 0 || idx >= _weapons.Count) return;

            WeaponInstance inst = _weapons[idx];

            /* 1) ground pickup */
            Vector3 pos = transform.position + transform.forward * 5f + Vector3.up * .3f;
            NetworkObject ground = TakeFromPool(inst.Def.groundPrefab);
            if (ground != null)
            {
                ground.transform.SetPositionAndRotation(pos, Quaternion.identity);
                ServerManager.Spawn(ground);

                if (ground.TryGetComponent(out WeaponPickup wp))
                    wp.Arm(0.15f);
            }

            /* 2) bookkeeping */
            _weapons.RemoveAt(idx);

            /* 3) new active */
            NetworkObject newActive = _weapons.Count > 0 ? _weapons[0].NetworkObj : null;
            if (inst.NetworkObj == _activeNob.Value)
                SetActiveWeapon(newActive);

            /* 4) client cleanup */
            RpcRemoveHeld(inst.NetworkObj);

            /* 5) despawn TP */
            ServerManager.Despawn(inst.NetworkObj, DespawnType.Pool);
        }

        [Server]
        public void DropAll()
        {
            for (int i = _weapons.Count - 1; i >= 0; --i)
                Server_DropWeapon(i);
        }

        [ObserversRpc(RunLocally = false)]
        void RpcRemoveHeld(NetworkObject nob)
        {
            int idx = _weapons.FindIndex(w => w.NetworkObj == nob);
            if (idx >= 0) _weapons.RemoveAt(idx);

            if (IsOwner && _fpViews.TryGetValue(nob, out var fp))
            {
                Destroy(fp);
                _fpViews.Remove(nob);
            }

            RefreshActive();
        }
        #endregion

        /* ═════════════════════════════ */
        #region Helper classes + pooling
        sealed class WeaponInstance
        {
            public readonly WeaponDefinition Def;      // null on pure clients
            public readonly NetworkObject    NetworkObj;
            readonly ProjectileWeapon        _pw;

            public WeaponInstance(WeaponDefinition d, NetworkObject nob)
            {
                Def        = d;
                NetworkObj = nob;
                _pw        = nob.GetComponent<ProjectileWeapon>();
            }

            public void SetActive(bool state)
            {
                NetworkObj.gameObject.SetActive(state);
                if (_pw) _pw.IsActive = state;
            }
        }

        static NetworkObject TakeFromPool(NetworkObject prefab)
        {
            NetworkObject nob = InstanceFinder.NetworkManager.GetPooledInstantiated(prefab, true);
            if (nob == null) return null;

            nob.transform.SetParent(null, false);
            return nob;
        }
        #endregion
    }
}

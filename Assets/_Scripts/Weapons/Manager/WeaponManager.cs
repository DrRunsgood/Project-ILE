// _Scripts/Weapons/Manager/WeaponManager.cs
using System.Collections.Generic;
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
        [SerializeField] Transform firstPersonAnchor;          // filled at runtime
        [SerializeField] Transform _anchor;                    // “HeldWeapons” (TP)

        /* ───────── public API ──────── */
        public int       ActiveSlot   => _activeSlot.Value;    // –1 = none
        public Transform WeaponAnchor => _anchor;

        /* ───────── private fields ──── */
        const int MaxSlots = 3;

        readonly List<WeaponInstance>                  _weapons  = new();
        readonly Dictionary<NetworkObject, GameObject> _fpViews  = new();
        readonly SyncVar<int>                          _activeSlot = new(-1);

        InputHandler _ih;

        /* ═════════════════════════════ */
        #region Unity – setup
        void Awake()
        {
            /* ensure TP anchor exists */
            if (_anchor == null)
            {
                Transform gfx = transform.Find("Graphics") ?? transform;
                _anchor = gfx.Find("HeldWeapons")
                       ?? new GameObject("HeldWeapons").transform;
                _anchor.SetParent(gfx, false);
            }

            _ih = GetComponent<InputHandler>();
            _activeSlot.OnChange += (_, __, ___) => RefreshActiveSlot();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!IsOwner) return;

            /* ensure FirstPersonItems exists under MainCamera */
            Transform cam = Camera.main.transform;
            firstPersonAnchor = cam.Find("FirstPersonItems");
            if (firstPersonAnchor == null)
            {
                firstPersonAnchor = new GameObject("FirstPersonItems").transform;
                firstPersonAnchor.SetParent(cam, false);
            }
        }

        void OnDestroy() =>
            _activeSlot.OnChange -= (_, __, ___) => RefreshActiveSlot();
        #endregion
        /* ═════════════════════════════ */

        #region Pick-up (called by WeaponPickup)
        [Server]
        public bool Server_AddWeapon(WeaponDefinition def)
        {
            if (_weapons.Count >= MaxSlots || HasWeapon(def))
                return false;

            /* spawn TP weapon */
            NetworkObject nob = Instantiate(def.heldPrefab, _anchor);
            nob.transform.localPosition = Vector3.zero;
            nob.transform.localRotation = Quaternion.identity;
            ServerManager.Spawn(nob, Owner);

            _weapons.Add(new WeaponInstance(def, nob));
            RpcAttachHeld(nob);                           // client bookkeeping

            if (_activeSlot.Value == -1)
                _activeSlot.Value = 0;

            return true;
        }

        bool HasWeapon(WeaponDefinition d)
        {
            foreach (var w in _weapons)
                if (w.Def == d) return true;
            return false;
        }
        #endregion

        #region RPC – attach held model on every peer
        [ObserversRpc(BufferLast = true, RunLocally = true)]
        void RpcAttachHeld(NetworkObject nob)
        {
            /* 1) set layer so owner camera hides TP gun */
            if (IsOwner)
            {
                int tpLayer = LayerMask.NameToLayer("TP_Only");
                foreach (Transform t in nob.transform.GetComponentsInChildren<Transform>(true))
                    t.gameObject.layer = tpLayer;
            }
            
            nob.transform.SetParent(_anchor, false);

            /* 2) local lists */
            if (_weapons.Find(w => w.NetworkObj == nob) == null)
                _weapons.Add(new WeaponInstance(null, nob));

            if (nob.TryGetComponent(out ProjectileWeapon pw))
                pw.CachePlayerRefs(this, _ih);

            /* 3) spawn FP view for owner */
            if (IsOwner && !_fpViews.ContainsKey(nob))
            {
                WeaponDefinition def = pw.Definition;          // uses existing ref
                if (def && def.fpViewPrefab)
                {
                    GameObject fp = Instantiate(def.fpViewPrefab, firstPersonAnchor);
                    fp.transform.localPosition = Vector3.zero;
                    fp.transform.localRotation = Quaternion.identity;
                    _fpViews[nob] = fp;
                    fp.transform.localScale = Vector3.one * 2f;
                }
            }

            RefreshActiveSlot();
        }
        #endregion

        #region Active-slot handling
        void RefreshActiveSlot()
        {
            int sel = _activeSlot.Value;

            for (int i = 0; i < _weapons.Count; ++i)
            {
                bool active = (i == sel);
                _weapons[i].SetActive(active);

                if (IsOwner &&
                    _fpViews.TryGetValue(_weapons[i].NetworkObj, out var fp))
                    fp.SetActive(active);
            }
        }
        #endregion

        /* ═════════════════════════════ */
        #region Owner input
        void Update()
        {
            if (!IsOwner || _weapons.Count == 0) return;

            int wanted = _activeSlot.Value;

            if (_ih.WeaponSlotInput >= 0)
                wanted = _ih.WeaponSlotInput;
            else if (_ih.MouseWheelDelta != 0)
                wanted = Mathf.Clamp(
                    (_activeSlot.Value + _ih.MouseWheelDelta + _weapons.Count) % _weapons.Count,
                    0, _weapons.Count - 1);

            if (wanted != _activeSlot.Value)
                Server_SetActiveSlot(wanted);

            if (_ih.ConsumeWeaponDrop())
                Server_RequestDropActive();
        }
        #endregion

        #region Slot change RPC
        [ServerRpc(RequireOwnership = true)]
        void Server_SetActiveSlot(int slot)
        {
            if (slot >= 0 && slot < _weapons.Count)
                _activeSlot.Value = slot;
        }
        #endregion

        /* ═════════════════════════════ */
        #region Drop
        [ServerRpc(RequireOwnership = true)]
        void Server_RequestDropActive() =>
            Server_DropWeapon(_activeSlot.Value);
        
        [Server]
        void Server_DropWeapon(int slot)
        {
            if (slot < 0 || slot >= _weapons.Count) return;

            WeaponInstance inst = _weapons[slot];

            // 1) ground pickup 
            Vector3 pos = transform.position + transform.forward * 5f + Vector3.up * .3f;
            NetworkObject ground = Instantiate(inst.Def.groundPrefab, pos, Quaternion.identity);
            ServerManager.Spawn(ground);

            // 2) bookkeeping 
            _weapons.RemoveAt(slot);

            // 3) new active slot 
            _activeSlot.Value = (_weapons.Count == 0) ? -1 :
                                Mathf.Clamp(slot, 0, _weapons.Count - 1);

            // 4) client cleanup 
            RpcRemoveHeld(inst.NetworkObj);

            // 5) despawn TP model 
            ServerManager.Despawn(inst.NetworkObj);
        }
        
        [Server]
        public void DropAll()
        {
            for (int i = _weapons.Count - 1; i >= 0; --i)
                Server_DropWeapon(i);
        }

        [ObserversRpc(BufferLast = true, RunLocally = false)]
        void RpcRemoveHeld(NetworkObject nob)
        {
            int idx = _weapons.FindIndex(w => w.NetworkObj == nob);
            if (idx >= 0) _weapons.RemoveAt(idx);

            if (IsOwner && _fpViews.TryGetValue(nob, out var fp))
            {
                Destroy(fp);
                _fpViews.Remove(nob);
            }

            RefreshActiveSlot();
        }
        #endregion

        /* ═════════════════════════════ */
        #region Helper
        sealed class WeaponInstance
        {
            public readonly WeaponDefinition Def;      // null on clients
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
        #endregion
    }
}

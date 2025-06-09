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
        public int       ActiveSlot   => _activeSlot.Value;   // –1 = no weapon
        public Transform WeaponAnchor => _anchor;

        /* ------------------------------------------------ constants */
        const int   MaxSlots = 3;

        /* ------------------------------------------------ fields */
        readonly List<WeaponInstance> _weapons = new();
        [SerializeField] Transform    _anchor;                // “HeldWeapons”
        InputHandler                  _ih;

        readonly SyncVar<int> _activeSlot = new(-1);

        /* ========================================================= */
        #region Unity - setup
        void Awake()
        {
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
        void OnDestroy() =>
            _activeSlot.OnChange -= (_, __, ___) => RefreshActiveSlot();
        #endregion
        /* ========================================================= */

        #region Pick-up (called by WeaponPickup)
        [Server]
        public bool Server_AddWeapon(WeaponDefinition def)
        {
            if (_weapons.Count >= MaxSlots || HasWeapon(def))
                return false;

            NetworkObject nob = Instantiate(def.heldPrefab, _anchor);
            nob.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            ServerManager.Spawn(nob, Owner);

            _weapons.Add(new WeaponInstance(def, nob));
            RpcAttachHeld(nob);

            if (_activeSlot.Value == -1) _activeSlot.Value = 0;

            return true;
        }

        bool HasWeapon(WeaponDefinition d)
        {
            foreach (var w in _weapons) if (w.Def == d) return true;
            return false;
        }
        #endregion

        #region RPC – parent + cache on every peer
        [ObserversRpc(BufferLast = true, RunLocally = true)]
        void RpcAttachHeld(NetworkObject nob)
        {
            nob.transform.SetParent(_anchor, false);

            if (_weapons.Find(w => w.NetworkObj == nob) == null)
                _weapons.Add(new WeaponInstance(null, nob));

            if (nob.TryGetComponent(out ProjectileWeapon pw))
                pw.CachePlayerRefs(this, _ih);

            RefreshActiveSlot();
        }
        #endregion

        void RefreshActiveSlot()
        {
            int sel = _activeSlot.Value;
            for (int i = 0; i < _weapons.Count; ++i)
                _weapons[i].SetActive(i == sel);
        }

        /* ========================================================= */
        #region Owner input
        void Update()
        {
            if (!IsOwner || _weapons.Count == 0) return;

            int wanted = _activeSlot.Value;
            if (_ih.WeaponSlotInput >= 0)
                wanted = _ih.WeaponSlotInput;
            else if (_ih.MouseWheelDelta != 0)
                wanted = Mathf.Clamp((_activeSlot.Value + _ih.MouseWheelDelta + _weapons.Count) % _weapons.Count, 0, _weapons.Count - 1);

            if (wanted != _activeSlot.Value) Server_SetActiveSlot(wanted);

            if (_ih.ConsumeDropKey()) Server_RequestDropActive();
        }
        #endregion

        #region Slot change RPC
        [ServerRpc(RequireOwnership = true)]
        void Server_SetActiveSlot(int slot)
        {
            if (slot < 0 || slot >= _weapons.Count) return;
            _activeSlot.Value = slot;
        }
        #endregion

        /* ========================================================= */
        #region Drop
        [ServerRpc(RequireOwnership = true)]
        void Server_RequestDropActive() => Server_DropWeapon(_activeSlot.Value);

        [Server]
        void Server_DropWeapon(int slot)
        {
            if (slot < 0 || slot >= _weapons.Count) return;

            WeaponInstance inst = _weapons[slot];

            /* 1) spawn ground pickup */
            Vector3 spawnPos = transform.position + transform.forward * 2.5f + Vector3.up * .3f;
            NetworkObject ground = Instantiate(inst.Def.groundPrefab, spawnPos, Quaternion.identity);
            ServerManager.Spawn(ground);

            /* 2) server list bookkeeping FIRST */
            _weapons.RemoveAt(slot);

            /* 3) calculate new selected slot and replicate */
            _activeSlot.Value = (_weapons.Count == 0) ? -1 : Mathf.Clamp(slot, 0, _weapons.Count - 1);

            /* 4) tell clients to delete their entry */
            RpcRemoveHeld(inst.NetworkObj);

            /* 5) despawn held model lastly */
            ServerManager.Despawn(inst.NetworkObj);

#if UNITY_EDITOR
            Debug.Log($"[SVR] drop slot {slot}  newSel={_activeSlot.Value}");
#endif
        }

        [ObserversRpc(BufferLast = true, RunLocally = false)]
        void RpcRemoveHeld(NetworkObject nob)
        {
            int idx = _weapons.FindIndex(w => w.NetworkObj == nob);
            if (idx >= 0) _weapons.RemoveAt(idx);
            RefreshActiveSlot();
        }
        #endregion

        /* ========================================================= */
        #region Helper
        sealed class WeaponInstance
        {
            public readonly WeaponDefinition Def;        // null on clients
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

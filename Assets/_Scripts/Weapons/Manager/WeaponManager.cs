// Assets/_Scripts/Weapons/Manager/WeaponManager.cs
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using _Scripts.Data;
using _Scripts.Player;

namespace _Scripts.Weapons
{
    /// Holds up to three weapons, keeps server & all clients perfectly in-sync.
    /// One authoritative SyncVar (the active slot) + a single ObserversRpc.
    public sealed class WeaponManager : NetworkBehaviour
    {
        /* ---------- public query API ---------- */
        public int        ActiveSlot   => _activeSlot.Value;           // -1 == none
        public bool       HasFreeSlot  => _weapons.Count < MaxSlots;
        public Transform  WeaponAnchor => _anchor;                     // for artists

        /* ---------- constants ---------- */
        const int MaxSlots = 3;

        /* ---------- private state ---------- */
        readonly List<WeaponInstance> _weapons = new();                // index == slot
        [SerializeField] Transform    _anchor;                         // Graphics/HeldWeapons
        InputHandler                  _ih;

        /* one tiny SyncVar – which slot is active */
        readonly SyncVar<int> _activeSlot = new SyncVar<int>(-1);
        
        #region  Unity lifecycle
        void Awake()
        {
            /* --- find / create the anchor under the animated graphics --- */
            if (_anchor == null)
            {
                Transform gfx = transform.Find("Graphics") ?? transform;
                _anchor = gfx.Find("HeldWeapons");
                if (_anchor == null)
                {
                    _anchor = new GameObject("HeldWeapons").transform;
                    _anchor.SetParent(gfx, false);
                }
            }

            _ih = GetComponent<InputHandler>();

            /* one delegate for slot changes – fires on server & clients */
            _activeSlot.OnChange += (_, __, ___) => RefreshActiveSlot();
        }

        void OnDestroy() => _activeSlot.OnChange -= (_, __, ___) => RefreshActiveSlot();
        
        #endregion

        /* ---------- entry-point for WeaponPickup (server-side only) ------- */
        #region  server add
        [Server]
        public void Server_AddWeapon(WeaponDefinition def)
        {
            if (_weapons.Count >= MaxSlots) return;

            /* 1) instantiate + spawn the held prefab */
            NetworkObject nob = Instantiate(def.heldPrefab, _anchor);
            nob.transform.localPosition = Vector3.zero;
            nob.transform.localRotation = Quaternion.identity;
            ServerManager.Spawn(nob, Owner);

            /* 2) tell every peer (incl. server) to parent & register it      */
            RpcAttachHeld(nob);

            /* 3) auto-equip if this is the first weapon                       */
            if (_activeSlot.Value == -1)
                _activeSlot.Value = 0;        // hook will call RefreshActiveSlot
        }
        #endregion

        /* ---------- single-path list insert & parenting ------------------ */
        [ObserversRpc(BufferLast = true, RunLocally = true)]
        void RpcAttachHeld(NetworkObject heldNob)
        {
            if (heldNob == null || _anchor == null) return;

            heldNob.transform.SetParent(_anchor, false);

            /* add to local list (runs once per peer) */
            _weapons.Add(new WeaponInstance(heldNob));

            /* hand the gun a quick ref to manager + input (optional) */
            if (heldNob.TryGetComponent(out ProjectileWeapon pw))
                pw.CachePlayerRefs(this, _ih);

            RefreshActiveSlot();              // honour current active slot
        }

        /* ---------- updates visibility based on _activeSlot -------------- */
        void RefreshActiveSlot()
        {
            int slot = _activeSlot.Value;
            for (int i = 0; i < _weapons.Count; i++)
                _weapons[i].SetActive(i == slot);
        }

        /* ---------- owner hot-key logic (client-side only) ---------------- */
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
                Server_SetActiveSlot(wanted);           // deterministic owner→srv
        }

        [ServerRpc(RequireOwnership = true)]
        void Server_SetActiveSlot(int slot)
        {
            if (slot < 0 || slot >= _weapons.Count) return;
            _activeSlot.Value = slot;                   // hook fires everywhere
        }

        /* ================================================================== */
        #region  nested helper
        sealed class WeaponInstance
        {
            readonly NetworkObject    _nob;
            readonly ProjectileWeapon _pw;

            public WeaponInstance(NetworkObject nob)
            {
                _nob = nob;
                _pw  = nob.GetComponent<ProjectileWeapon>();
            }

            public void SetActive(bool state)
            {
                _nob.gameObject.SetActive(state);
                if (_pw) _pw.IsActive = state;
            }
        }
        #endregion
    }
}

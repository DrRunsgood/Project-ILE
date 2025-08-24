// _Scripts/Weapons/Manager/WeaponManager.cs
using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using _Scripts.Data;
using _Scripts.Player;
using _Scripts.GamePhysics;

namespace _Scripts.Weapons
{
    public sealed class WeaponManager : NetworkBehaviour
    {
/* ───────── inspector ───────── */
        [SerializeField] Transform firstPersonAnchor;
        [SerializeField] Transform _anchor;
        [SerializeField] WeaponDefinition[] defaultQuickItems;

/* ───────── public API ──────── */
        public Transform WeaponAnchor => _anchor;

/* ───────── private ─────────── */
        const int MaxSlots = 3;

        readonly List<WeaponInstance>                  _weapons = new();
        readonly Dictionary<NetworkObject, GameObject> _fpViews = new();
        readonly SyncVar<NetworkObject>                _activeNob = new(null);

        InputHandler _ih;

/* ═════════════════════════════ */
#region Unity lifecycle
        void Awake()
        {
            if (_anchor == null)
            {
                Transform gfx = transform.Find("Graphics") ?? transform;
                _anchor = gfx.Find("HeldWeapons") ?? new GameObject("HeldWeapons").transform;
                _anchor.SetParent(gfx, false);
            }
            _ih = GetComponent<InputHandler>();
            _activeNob.OnChange += (_,__,___) => RefreshActive();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            GiveDefaultQuickItems();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!IsOwner) return;

            var cam = Camera.main.transform;
            firstPersonAnchor = cam.Find("FirstPersonItems") ?? new GameObject("FirstPersonItems").transform;
            firstPersonAnchor.SetParent(cam, false);
        }

        // called on the SERVER every time a *new* connection begins observing this WeaponManager
        public override void OnSpawnServer(NetworkConnection conn)
        {
            base.OnSpawnServer(conn);

            // collect every held weapon’s NetworkObject
            NetworkObject[] list = new NetworkObject[_weapons.Count];
            for (int i = 0; i < _weapons.Count; ++i)
                list[i] = _weapons[i].NetworkObj;

            // send them only to that new client
            RpcClient_SyncFullInventory(conn, list);
        }
#endregion

/* ───────────────────────────── */
#region Local‑attach helper  (shared by all RPC paths)
        static void ResetLocal(Transform t)
        {
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale    = Vector3.one;
        }
        
        void HandleAttachLogic(NetworkObject nob)
        {
            if (nob == null) return;

            nob.transform.SetParent(_anchor, false);
            ResetLocal(nob.transform);

            if (!_weapons.Exists(w => w.NetworkObj == nob)) _weapons.Add(new WeaponInstance(null, nob));
            
            if (!nob.TryGetComponent(out ProjectileWeapon pw)) return;

            pw.CachePlayerRefs(this, _ih);

            /* quick‑items have no visible TP mesh */
            if (pw.isHiddenQuickItem) return;
            
            int defaultLay = LayerMask.NameToLayer("Default");
            int tpLay      = LayerMask.NameToLayer("TP_Only");

            foreach (Transform t in nob.GetComponentsInChildren<Transform>(true))
                t.gameObject.layer = defaultLay;          // step 1

            if (IsOwner)                                   // step 2
                foreach (Transform t in nob.GetComponentsInChildren<Transform>(true))
                    t.gameObject.layer = tpLay;

            //  first‑person view model (owner only, once)
            if (IsOwner && !_fpViews.ContainsKey(nob) && pw.Definition?.fpViewPrefab)
            {
                GameObject fp = Instantiate(pw.Definition.fpViewPrefab, firstPersonAnchor);
                ResetLocal(fp.transform);
                fp.transform.localScale = Vector3.one * 2f;
                _fpViews[nob] = fp;
            }
        }

#endregion

/* ───────────────────────────── */
#region RPCs – attach & full‑sync
        /* normal pick‑up → goes to current observers only */
        [ObserversRpc(RunLocally = true, BufferLast = true)]
        void RpcAttachHeld(NetworkObject nob)
        {
            HandleAttachLogic(nob);
            RefreshActive();
        }

        /* late‑join inventory dump (targeted to the joining client) */
        [TargetRpc]
        void RpcClient_SyncFullInventory(NetworkConnection _, NetworkObject[] list)
        {
            _weapons.Clear();
            foreach (NetworkObject nob in list)
                HandleAttachLogic(nob);

            RefreshActive();          // _activeNob already synced
        }
#endregion

/* ───────────────────────────── */
#region Server pick‑up (original)
        bool HasWeapon(WeaponDefinition d) =>
            _weapons.Exists(w => w.Def == d);

        [Server]
        public bool Server_AddWeapon(WeaponDefinition def)
        {
            bool hidden =
                def.heldPrefab.GetComponent<ProjectileWeapon>()
                    ?.isHiddenQuickItem == true;

            if (!hidden)
            {
                int regular = 0;
                foreach (var w in _weapons)
                    if (!w.IsQuickItem) regular++;
                if (regular >= MaxSlots) return false;
            }
            if (HasWeapon(def)) return false;

            NetworkObject nob = PoolUtil.TakeFromPool(def.heldPrefab);
            if (nob == null) return false;

            nob.transform.SetParent(_anchor, false);
            ResetLocal(nob.transform);
            ServerManager.Spawn(nob, Owner);

            _weapons.Add(new WeaponInstance(def, nob));
            RpcAttachHeld(nob);

            if (!hidden)
            {
                bool curHidden = _activeNob.Value == null ||
                    _weapons.Find(w => w.NetworkObj == _activeNob.Value)
                            ?.IsQuickItem == true;
                if (curHidden) SetActiveWeapon(nob);
            }
            return true;
        }
#endregion

/* ───────────────────────────── */
#region Active / selection (unchanged)
        void RefreshActive()
        {
            NetworkObject want = _activeNob.Value;
            foreach (var w in _weapons)
            {
                bool act = w.NetworkObj == want;
                w.SetActive(act);
                if (IsOwner && _fpViews.TryGetValue(w.NetworkObj, out var fp))
                    fp.SetActive(act);
            }
        }

        [Server] void GiveDefaultQuickItems()
        {
            foreach (var d in defaultQuickItems)
                if (d) Server_AddWeapon(d);
        }

        [Server] void SetActiveWeapon(NetworkObject nob) =>
            _activeNob.Value = nob;
#endregion

/* ═════════════════════════════ */
#region Owner input (unchanged)
        void Update()
        {
            if (!IsOwner) return;

            var selectable = _weapons.FindAll(w => !w.IsQuickItem);
            if (selectable.Count == 0) return;

            int cur = selectable.FindIndex(w => w.NetworkObj == _activeNob.Value);
            if (cur < 0) cur = 0;
            int want = cur;

            if (_ih.WeaponSlotInput >= 0)
                want = Mathf.Clamp(_ih.WeaponSlotInput, 0, selectable.Count - 1);
            else if (_ih.MouseWheelDelta != 0)
                want = (cur + _ih.MouseWheelDelta + selectable.Count) % selectable.Count;

            if (want != cur) Server_SetActiveByIndex(want);
            if (_ih.ConsumeWeaponDrop()) Server_RequestDropActive();
        }
#endregion

/* ───────────────────────────── */
#region RPC – set active by index (unchanged)
        [ServerRpc(RequireOwnership = true)]
        void Server_SetActiveByIndex(int idx)
        {
            var sel = _weapons.FindAll(w => !w.IsQuickItem);
            if (idx >= 0 && idx < sel.Count)
                SetActiveWeapon(sel[idx].NetworkObj);
        }
#endregion

/* ═════════════════════════════ */
#region Drop (NRE guard kept)
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
            if (inst.Def == null) { _weapons.RemoveAt(idx); return; }

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
                {
                    return;
                }
            }

            Vector3 camPos = snap.Position;
            Vector3 fwd    = snap.Direction;

            // Drop placement
            Vector3 pos = camPos + fwd;

            // 2) spawn the ground item
            NetworkObject ground = PoolUtil.TakeFromPool(inst.Def.groundPrefab);
            if (ground == null) { _weapons.RemoveAt(idx); return; }

            ground.transform.SetPositionAndRotation(pos, Quaternion.identity);
            ServerManager.Spawn(ground);                            // authority owns it

            // 3) give it an initial kick
            if (ground.TryGetComponent(out KinematicMover km))
            {
                Vector3 playerVel   = GetComponent<Rigidbody>()?.linearVelocity ?? Vector3.zero;
                Vector3 tossForward = fwd * 15f;
                km.InitVelocity(playerVel * 0.5f + tossForward);
            }

            // Arm timer for pickup
            if (ground.TryGetComponent(out WeaponPickup wp))
                wp.Arm(0.5f);

            // bookkeeping
            _weapons.RemoveAt(idx);
            NetworkObject newAct = null;
            foreach (var w in _weapons)
                if (!w.IsQuickItem) { newAct = w.NetworkObj; break; }

            if (inst.NetworkObj == _activeNob.Value) SetActiveWeapon(newAct);

            RpcRemoveHeld(inst.NetworkObj);
            ServerManager.Despawn(inst.NetworkObj, DespawnType.Pool);
        }



        [Server] public void DropAll()
        {
            for (int i = _weapons.Count - 1; i >= 0; --i)
                if (_weapons[i].Def != null && !_weapons[i].IsQuickItem)
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
#region Helper class
        sealed class WeaponInstance
        {
            public readonly WeaponDefinition Def;
            public readonly NetworkObject NetworkObj;
            readonly ProjectileWeapon _pw;

            public bool IsQuickItem => _pw && _pw.isHiddenQuickItem;

            public WeaponInstance(WeaponDefinition d, NetworkObject nob)
            {
                Def = d;
                NetworkObj = nob;
                _pw = nob.GetComponent<ProjectileWeapon>();
            }
            public void SetActive(bool s)
            {
                if (_pw && _pw.isHiddenQuickItem)
                    _pw.IsActive = s;
                else
                {
                    NetworkObj.gameObject.SetActive(s);
                    if (_pw) _pw.IsActive = s;
                }
            }
        }
#endregion
    }
}

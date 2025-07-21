// _Scripts/Weapons/ProjectileWeapon.cs
using _Scripts.Data;
using _Scripts.Player;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

namespace _Scripts.Weapons
{
    public class ProjectileWeapon : NetworkBehaviour
    {
        /* ───────── inspector ───────── */
        [SerializeField] protected WeaponDefinition def;
        [SerializeField] protected Transform        muzzle;

        /* ───────── quick-item hook ─────── */
        // Normal weapons return false            (default).
        // GrenadeThrower overrides to true.
        public virtual bool isHiddenQuickItem => def && def.hiddenQuickItem;

        /* ───────── cached refs ───────── */
        protected WeaponManager _wm;
        protected InputHandler  _ih;
        protected NetworkObject _shooterNO;  // for lag-comp snapshots

        public   WeaponDefinition Definition => def;

        /* ───────── runtime ───────── */
        public bool IsActive { get; set; }
        float _nextFireTime;
        
        static readonly RaycastHit[] _rayHits = new RaycastHit[8];
        float _fireInterval;

        /* ================================================================= */
        #region Wiring from WeaponManager

        public virtual void CachePlayerRefs(WeaponManager wm, InputHandler ih)
        {
            _wm        = wm;
            _ih        = ih;
            _shooterNO = wm.NetworkObject;

            if (!muzzle) muzzle = transform.Find("FirePoint");
        }

        #endregion
        /* ================================================================= */

        void Awake()
        {
            _fireInterval = def.fireRate > 0f ? 1f / def.fireRate : 0.1f;
        }
        
        void Update()
        {
            if (!IsOwner || _wm == null || _ih == null) return;
            if (!isHiddenQuickItem && !IsActive)        return;
            if (!CanFire())                             return;

            _nextFireTime = Time.time + _fireInterval;
            Server_RequestFire(TimeManager.Tick);
        }

        /* ---------- fire-eligibility ---------- */
        protected virtual bool CanFire()
        {
            if (isHiddenQuickItem) // Hidden quick-items never use LMB; subclasses decide themselves.
                return false;

            bool triggerHeld = (_ih.HeldButtons & InputButtons.Fire) != 0;
            return triggerHeld && Time.time >= _nextFireTime;
        }

        /* ================================================================= */
        #region  Server-authoritative spawn

        [ServerRpc(RequireOwnership = true)]
        void Server_RequestFire(uint clientFireTick, NetworkConnection sender = null)
        {
            if (def.projectilePrefab == null || _shooterNO == null || !sender?.IsValid == true)
                return;

            if (!ServerCanConsume()) return;

            /* ---- latency & rewind ---- */
            uint serverNow   = TimeManager.Tick;
            uint pktLocal    = sender.PacketTick.LocalTick;
            uint oneWayTicks = serverNow - pktLocal;

            const uint safety = 0;
            uint rewindTick   = clientFireTick > oneWayTicks + safety ? clientFireTick - (oneWayTicks + safety) : 0u;

            if (!LagCompensationManager.Instance.TryGetSnapshot(_shooterNO, rewindTick, out var snap, 1))
                return; // miss

            /* ---- aim point ---- */
            Vector3 aimPoint = GetAimPoint(snap.Position, snap.Direction, 1000f);
            
            /* ---- compute direction first ---- */
            Vector3 muzzlePos = muzzle.position;
            Vector3 fireDir   = aimPoint - muzzlePos;
            if (fireDir.sqrMagnitude < 0.0001f) fireDir = muzzle ? muzzle.forward : transform.forward;
            fireDir = fireDir.normalized;

            /* ---- choose safe spawn position ---- */
            Vector3 spawnPos = ChooseSpawnPos(snap.Position, muzzlePos, fireDir, def.castRadius);

            /* ---- final velocity and spawn ---- */
            Vector3 finalVel = fireDir * def.projectileSpeed + snap.Velocity * def.velocityInheritance;

            var nob = InstanceFinder.NetworkManager.GetPooledInstantiated(def.projectilePrefab, true);
            if (nob == null) return;

            nob.transform.SetPositionAndRotation(spawnPos, Quaternion.LookRotation(fireDir, muzzle ? muzzle.up : Vector3.up));

            if (nob.TryGetComponent(out BaseProjectile proj))
            {
                proj.Init(spawnPos, finalVel, serverNow, _shooterNO);
                ServerManager.Spawn(nob);

                // immutable spawn state to clients
                proj.RpcInit(spawnPos, finalVel, serverNow);
            }
            else
            {
                ServerManager.Despawn(nob, DespawnType.Pool);
            }
        }
        
        /* ----------------------------- aim helper --------------------------------- */
        Vector3 GetAimPoint(Vector3 origin, Vector3 dir, float range)
        {
            int hitCnt = Physics.RaycastNonAlloc(origin, dir, _rayHits, range, def.hitMask, QueryTriggerInteraction.Ignore);

            float closest = range + 0.01f;
            Vector3 best  = origin + dir * range;

            for (int i = 0; i < hitCnt; ++i)
            {
                if (_rayHits[i].collider.transform.root == _wm.transform.root)
                    continue;                         // ignore self

                if (_rayHits[i].distance < closest)
                {
                    closest = _rayHits[i].distance;
                    best    = _rayHits[i].point;      // nearest non‑self
                }
            }
            return best;
        }
        
        /* ----------------------------- spawn point --------------------------------- */
        Vector3 ChooseSpawnPos(Vector3 camPos, Vector3 muzzlePos, Vector3 dir, float radius)
        {
            // Is there an obstacle between camera and muzzle?
            if (Physics.Linecast(camPos, muzzlePos, out RaycastHit hit,def.hitMask, QueryTriggerInteraction.Ignore))
            {
                // Put spawn point just in front of the surface.
                return hit.point + dir * (radius + 0.01f);
            }
            // Otherwise spawn at the muzzle (slightly forward so we don't self‑hit).
            return muzzlePos + dir * radius;
        }

        #endregion
        /* ================================================================= */

        protected virtual bool ServerCanConsume() => true;
    }
}
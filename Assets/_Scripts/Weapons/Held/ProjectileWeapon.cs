using _Scripts.Data;
using _Scripts.Player;
using FishNet;
using FishNet.Object;
using FishNet.Connection;
using UnityEngine;

namespace _Scripts.Weapons
{
    public class ProjectileWeapon : NetworkBehaviour
    {
        /* ───────── inspector ───────── */
        [SerializeField] protected WeaponDefinition def;
        [SerializeField] protected Transform        muzzle;

        /* ───────── cached refs ───────── */
        protected WeaponManager _wm;    // living on the owning player
        protected InputHandler  _ih;
        protected NetworkObject _shooterNO; // player’s NetworkObject (for lag-comp)
        
        public WeaponDefinition Definition => def;   // add this one-liner

        /* ───────── state ───────── */
        public bool IsActive { get; set; }          // set by WeaponManager
        float _nextFireTime;                        // local rate-of-fire timer
        
        /* ------------------------------------------------------------------ */
        #region  wiring from WeaponManager
        public virtual void CachePlayerRefs(WeaponManager wm, InputHandler ih)
        {
            _wm        = wm;
            _ih        = ih;
            _shooterNO = wm.NetworkObject;
            
            if (!muzzle) muzzle = transform.Find("FirePoint");
        }

        #endregion
        /* ------------------------------------------------------------------ */
        void Update()
        {
            if (!IsOwner || !IsActive || _wm == null || _ih == null) return;

            if (!CanFire())
                return;
            
            _nextFireTime = Time.time + 1f / def.fireRate;
            
            Server_RequestFire(TimeManager.Tick);
        }
        
        protected virtual bool CanFire()
        {
            bool trigger = (_ih.HeldButtons & InputButtons.Fire) != 0;
            if (!trigger) return false;
            
            if (Time.time < _nextFireTime) return false;

            return true;
        }
        
        /* ------------------------------------------------------------ */
        #region server-side fire
        [ServerRpc(RequireOwnership = true)]
        void Server_RequestFire(uint clientFireTick, NetworkConnection sender = null)
        {
            if (def.projectilePrefab == null || _shooterNO == null || !sender?.IsValid == true)
                return;
            
            /* NEW: resource / ammo / energy gate -------------------- */
            if (!ServerCanConsume())
                return;  
            
            /* 1) one-way latency in *ticks* ------------------------------------ */
            uint serverNow   = TimeManager.Tick;                 // this frame on server
            uint pktLocal    = sender.PacketTick.LocalTick;      // server-tick when pkt arrived
            uint oneWayTicks = serverNow - pktLocal;             // Latency in ticks

            /* 2) rewind target ----------------------------------------------- */
            const uint safety = 1;                               // 0 or 1 is usual - try 0 when not local testing
            uint rewindTick   = clientFireTick > (oneWayTicks+safety) ? clientFireTick - (oneWayTicks+safety) : 0u;

            /* 3) fetch snapshot (±1 already tolerated inside) ---------------- */
            if (!LagCompensationManager.Instance.TryGetSnapshot(_shooterNO, rewindTick, out var snap, 1))
                return; // miss – give up

            Vector3 aimPoint = snap.Position + snap.Direction * 1000f;
            
            // Stop at first obstruction (same mask as projectiles)
            if (Physics.Raycast(snap.Position, snap.Direction, out RaycastHit hit, 1000f, def.hitMask,
                    QueryTriggerInteraction.Ignore))
            {
                aimPoint = hit.point;
            }

            /* ---------- 2. Determine muzzle-based spawn ---------- */

            Vector3 spawnPos = muzzle.position;
            Vector3 fireDir  = (aimPoint - spawnPos).normalized;
            
            /* 4) spawn ------------------------------------------------------- */
            Vector3 finalVel = fireDir * def.projectileSpeed + snap.Velocity * def.velocityInheritance;

            var nob = InstanceFinder.NetworkManager.GetPooledInstantiated(def.projectilePrefab, true);
            if (nob == null) return;
            
            nob.transform.SetPositionAndRotation(spawnPos, Quaternion.LookRotation(fireDir, Vector3.up));

            if (nob.TryGetComponent(out BaseProjectile proj))
            {
                proj.Init(spawnPos, finalVel, serverNow, _shooterNO);  // Server init

                ServerManager.Spawn(nob);                 // ← spawns on server

                // ↓ send immutable spawn-state to all observers (owner will ignore)
                proj.RpcInit(spawnPos, finalVel, serverNow);
            }
            else
                ServerManager.Despawn(nob, DespawnType.Pool);
        }
        #endregion
        
        /* ------------------------------------------------------------ *
         *  SERVER-side pre-flight resource check (override in subclass)*
         * ------------------------------------------------------------ */
        protected virtual bool ServerCanConsume() => true;
    }
}
using _Scripts.Data;
using _Scripts.Player;
using FishNet;
using FishNet.Object;
using FishNet.Connection;
using UnityEngine;

namespace _Scripts.Weapons
{
    public sealed class ProjectileWeapon : NetworkBehaviour
    {
        /* ───────── inspector ───────── */
        [SerializeField] WeaponDefinition def;
        [SerializeField] Transform        firePoint;

        /* ───────── cached refs ───────── */
        WeaponManager _wm;    // living on the owning player
        InputHandler  _ih;
        NetworkObject _shooterNO; // player’s NetworkObject (for lag-comp)
        
        public WeaponDefinition Definition => def;   // add this one-liner

        /* ───────── state ───────── */
        public bool IsActive { get; set; }          // set by WeaponManager
        float _nextFireTime;                        // local rate-of-fire timer
        
        /* ------------------------------------------------------------------ */
        #region  wiring from WeaponManager
        public void CachePlayerRefs(WeaponManager wm, InputHandler ih)
        {
            _wm        = wm;
            _ih        = ih;
            _shooterNO = wm.NetworkObject;
        }

        #endregion
        /* ------------------------------------------------------------------ */
        void Update()
        {
            if (!IsOwner || !IsActive || _wm == null || _ih == null) return;

            bool trigger = (_ih.CmdRing.Get(TimeManager.Tick).buttons & InputButtons.Fire) != 0;

            if (!trigger || Time.time < _nextFireTime)
                return;
            
            _nextFireTime = Time.time + 1f / def.fireRate;

            Server_RequestFire(TimeManager.Tick);
        }
        
        /* ------------------------------------------------------------ */
        #region server-side fire
        [ServerRpc(RequireOwnership = true)]
        void Server_RequestFire(uint clientFireTick, NetworkConnection sender = null)
        {
            if (def.projectilePrefab == null || _shooterNO == null || !sender?.IsValid == true)
                return;
            
            /* 1) one-way latency in *ticks* ------------------------------------ */
            uint serverNow   = TimeManager.Tick;                 // this frame on server
            uint pktLocal    = sender.PacketTick.LocalTick;      // server-tick when pkt arrived
            uint oneWayTicks = serverNow - pktLocal;             // == pktLocal - pktRemote

            /* 2) rewind target ----------------------------------------------- */
            const uint safety = 1;                               // 0 or 1 is usual - try 0 when not local testing
            uint rewindTick   = clientFireTick > (oneWayTicks+safety) ? clientFireTick - (oneWayTicks+safety) : 0u;

            /* 3) fetch snapshot (±1 already tolerated inside) ---------------- */
            if (!LagCompensationManager.Instance.TryGetSnapshot(_shooterNO, rewindTick, out var snap, 1))
                return; // miss – give up

            /* 4) spawn ------------------------------------------------------- */
            Vector3 dir      = snap.Direction.normalized;
            Vector3 finalVel = dir * def.projectileSpeed + snap.Velocity * def.velocityInheritance;

            var nob = InstanceFinder.NetworkManager.GetPooledInstantiated(
                def.projectilePrefab, true);
            if (nob == null) return;

            nob.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);

            if (nob.TryGetComponent(out BaseProjectile proj))
            {
                proj.SetDefinition(def);
                proj.Init(snap.Position, finalVel, serverNow, _shooterNO);  // Server init

                ServerManager.Spawn(nob);                 // ← spawns on server

                // ↓ send immutable spawn-state to all observers (owner will ignore)
                proj.RpcInit(snap.Position, finalVel, serverNow, def.gravityScale);
            }
            else
                ServerManager.Despawn(nob, DespawnType.Pool);
        }
        #endregion
    }
}
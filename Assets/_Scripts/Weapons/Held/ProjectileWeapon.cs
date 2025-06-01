using _Scripts.Data;
using _Scripts.Player;
using FishNet;
using FishNet.Object;
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

        /* ───────── state ───────── */
        public bool IsActive { get; set; }          // set by WeaponManager
        float _nextFireTime;                        // local rate-of-fire timer
        
        /* ------------------------------------------------------------------ */
        #region  wiring from WeaponManager
        public void CachePlayerRefs(WeaponManager wm, InputHandler ih)
        {
            _wm        = wm;
            _ih        = ih;
            _shooterNO = wm.NetworkObject;          // authoritative object for snapshots
        }

        #endregion
        /* ------------------------------------------------------------------ */
        void Update()
        {
            if (!IsOwner || !IsActive || _wm == null || _ih == null) return;

            bool trigger =
                (_ih.CmdRing.Get(TimeManager.Tick).buttons & InputButtons.Fire) != 0;

            if (!trigger || Time.time < _nextFireTime)
                return;
            
            _nextFireTime = Time.time + 1f / def.fireRate;

            /* work out ‘rewind tick’ exactly as before – no direction sent */
            double halfRttMs  = TimeManager.HalfRoundTripTime;
            double tickDt     = TimeManager.TickDelta;
            uint   lagTicks   = (uint)Mathf.CeilToInt((float)((halfRttMs * 0.001) / tickDt));
            const  uint safety = 1u;
            uint   rewindTick = (lagTicks + safety > TimeManager.Tick)
                ? 0u
                : TimeManager.Tick - (lagTicks + safety);

            Server_RequestFire(rewindTick);
        }
        
        /* ------------------------------------------------------------ */
        #region server-side fire
        [ServerRpc(RequireOwnership = true)]
        private void Server_RequestFire(uint clientTick)
        {
            if (def.projectilePrefab == null || _shooterNO == null) return;
            
            if (!LagCompensationManager.Instance.TryGetSnapshot(_shooterNO, clientTick, out var snap))
                return;

            Vector3 dir       = snap.Direction.normalized;
            Vector3 finalVel  = dir * def.projectileSpeed + snap.Velocity * def.velocityInheritance;
            
            var nob = InstanceFinder.NetworkManager.GetPooledInstantiated(def.projectilePrefab, true);
            if (nob == null) return;
            
            if (nob.TryGetComponent(out BaseProjectile proj))
            {
                proj.SetDefinition(def);
                // use lag-compensated muzzle pos
                proj.Init(snap.Position, finalVel, TimeManager.Tick, _shooterNO);
                ServerManager.Spawn(nob);
            }
            else
                ServerManager.Despawn(nob, DespawnType.Pool);
        }
        #endregion
    }
}
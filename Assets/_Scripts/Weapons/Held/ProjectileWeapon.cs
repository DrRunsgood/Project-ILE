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

        [Header("Spawn Settings")]
        [Tooltip("How far in front of the camera the projectile should appear (roughly barrel length from camera).")]
        [SerializeField] float spawnOffset = 1.5f;
        
        [Header("Spawn Safety")]
        [SerializeField] float spawnSafetyRadiusOverride = -1f;
        [SerializeField] float spawnBackoff = 0.02f;
        [SerializeField] LayerMask spawnBlockMask = ~0;
        
        public virtual bool isHiddenQuickItem => def && def.hiddenQuickItem;
        
        /* ───────── cached refs ─────── */
        protected WeaponManager _wm;
        protected InputHandler  _ih;
        protected NetworkObject _shooterNO;

        public WeaponDefinition Definition => def;

        /* ───────── runtime ───────── */
        public bool IsActive { get; set; }
        bool _fireTimingInitialized;
        uint _nextFireTick;
        uint _fireIntervalTicks;
        uint _nextServerFireTick;

        /* ================================================================= */
        #region Wiring

        public virtual void CachePlayerRefs(WeaponManager wm, InputHandler ih)
        {
            _wm        = wm;
            _ih        = ih;
            _shooterNO = wm.NetworkObject;
        }

        #endregion
        /* ================================================================= */

        void Update()
        {
            if (!IsOwner || _wm == null || _ih == null) return;
            if (!isHiddenQuickItem && !IsActive)        return;
            EnsureFireTimingInitialized();
            if (!CanFire()) return;
            
            uint nowTick = TimeManager.Tick;
            _nextFireTick = nowTick + _fireIntervalTicks;

            Server_RequestFire(nowTick);
        }
        
        void EnsureFireTimingInitialized()
        {
            if (_fireTimingInitialized)
                return;

            float fireIntervalSeconds = (def != null && def.fireRate > 0f) ? 1f / def.fireRate : 0.1f;
            float tickDelta = (float)TimeManager.TickDelta;

            _fireIntervalTicks = (uint)Mathf.Max(1, Mathf.RoundToInt(fireIntervalSeconds / tickDelta));
            _nextFireTick = 0;
            _fireTimingInitialized = true;
        }

        protected virtual bool CanFire()
        {
            if (isHiddenQuickItem) // Hidden quick-items never use LMB; subclasses decide themselves.
                return false;

            bool triggerHeld = (_ih.HeldButtons & InputButtons.Fire) != 0;
            return triggerHeld && TimeManager.Tick >= _nextFireTick;
        }

        /* ================================================================= */
        #region  Server-authoritative spawn

        [ServerRpc(RequireOwnership = true)]
        void Server_RequestFire(uint clientFireTick, NetworkConnection sender = null)
        {
            if (def.projectilePrefab == null || _shooterNO == null || !sender?.IsValid == true)
                return;

            if (!ServerCanConsume()) return;

            uint serverNow = TimeManager.Tick;
            
            if (serverNow < _nextServerFireTick)
                return;

            _nextServerFireTick = serverNow + _fireIntervalTicks;
            
            uint target    = clientFireTick;
            if (target >= serverNow)
                target = serverNow > 0 ? serverNow - 1 : 0;

            LagCompensationManager.FireSnapshot snap;

            // Robust snapshot lookup: exact → -1 → +1 → small tolerance → last-resort recent
            if (!LagCompensationManager.Instance.TryGetSnapshot(_shooterNO, target, out snap, 0) &&
                !(target > 0 && LagCompensationManager.Instance.TryGetSnapshot(_shooterNO, target - 1, out snap, 0)) &&
                !(LagCompensationManager.Instance.TryGetSnapshot(_shooterNO, target + 1, out snap, 0)) &&
                !LagCompensationManager.Instance.TryGetSnapshot(_shooterNO, target, out snap, 2))
            {
                uint last = serverNow > 0 ? serverNow - 1 : 0;
                if (!LagCompensationManager.Instance.TryGetSnapshot(_shooterNO, last, out snap, 2))
                    return; // no snapshots available; drop the shot
            }
            
            Vector3 fireDir = snap.Direction.normalized;   // camera forward at shot time
            Vector3 shotOrigin = snap.Position;
            Vector3 spawnPos = ResolveSafeSpawnPosition(shotOrigin, fireDir);

            // Final velocity: own projectile speed + inherited player velocity
            Vector3 finalVel = fireDir * def.projectileSpeed + snap.Velocity * def.velocityInheritance;

            // Spawn projectile
            var nob = InstanceFinder.NetworkManager.GetPooledInstantiated(def.projectilePrefab, true);
            if (nob == null) return;

            nob.transform.SetPositionAndRotation(spawnPos, Quaternion.LookRotation(fireDir, Vector3.up));

            if (nob.TryGetComponent(out BaseProjectile proj))
            {
                proj.Init(spawnPos, finalVel, serverNow, _shooterNO);
                ServerManager.Spawn(nob);
                proj.RpcInit(spawnPos, finalVel, serverNow);
            }
            else
            {
                ServerManager.Despawn(nob, DespawnType.Pool);
            }
        }
        
        protected virtual float GetSpawnSafetyRadius()
        {
            if (spawnSafetyRadiusOverride > 0f)
                return spawnSafetyRadiusOverride;

            if (def != null)
                return def.castRadius;

            return 0.25f; // safe fallback
        }

        protected virtual Vector3 ResolveSafeSpawnPosition(Vector3 shotOrigin, Vector3 fireDir)
        {
            fireDir.Normalize();

            float radius = GetSpawnSafetyRadius();
            Vector3 desiredSpawn = shotOrigin + fireDir * spawnOffset;

            // If origin is already obstructed, keep it here and let projectile immediately collide/explode.
            if (Physics.CheckSphere(shotOrigin, radius, spawnBlockMask, QueryTriggerInteraction.Ignore))
                return shotOrigin;

            Vector3 finalSpawn = desiredSpawn;

            if (Physics.SphereCast(shotOrigin, radius, fireDir, out RaycastHit hit, spawnOffset, spawnBlockMask, QueryTriggerInteraction.Ignore))
            {
                finalSpawn = hit.point - fireDir * spawnBackoff;
            }

            if (Physics.CheckSphere(finalSpawn, radius, spawnBlockMask, QueryTriggerInteraction.Ignore))
            {
                Vector3 nearOrigin = shotOrigin + fireDir * Mathf.Min(spawnBackoff, spawnOffset * 0.25f);

                if (!Physics.CheckSphere(nearOrigin, radius, spawnBlockMask, QueryTriggerInteraction.Ignore))
                    return nearOrigin;

                return shotOrigin;
            }

            return finalSpawn;
        }

        #endregion
        /* ================================================================= */
        
        public virtual void ResetRuntime()
        {
            _nextFireTick = 0;
        }

        protected virtual bool ServerCanConsume() => true;
    }
}

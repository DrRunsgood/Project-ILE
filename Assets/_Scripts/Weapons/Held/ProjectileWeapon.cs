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
        #region Inspector

        [Header("Definition")]
        [SerializeField] protected WeaponDefinition def;

        [Header("Fire Routing")]
        [Tooltip("Regular weapons fire through AdvancedPredictedController.MovementData.Held. Hidden quick-items still use the legacy armed path.")]
        [SerializeField] bool usePredictedInputFire = true;

        [Header("Spawn Settings")]
        [Tooltip("How far in front of the fire origin the projectile should appear.")]
        [SerializeField] float spawnOffset = 1.5f;

        [Header("Spawn Safety")]
        [SerializeField] float spawnSafetyRadiusOverride = -1f;
        [SerializeField] float spawnBackoff = 0.02f;
        [SerializeField] LayerMask spawnBlockMask = ~0;

        #endregion

        #region Public API

        public virtual bool isHiddenQuickItem => def && def.hiddenQuickItem;

        public WeaponDefinition Definition => def;

        public bool IsActive { get; set; }

        #endregion

        #region Cached References

        protected WeaponManager _wm;
        protected InputHandler _ih;
        protected NetworkObject _shooterNO;

        #endregion

        #region Runtime

        bool _fireTimingInitialized;
        uint _nextFireTick;        // client-side legacy quick-item/fallback timing
        uint _fireIntervalTicks;
        uint _nextServerFireTick;  // authoritative server cooldown

        #endregion

        #region Wiring

        public virtual void CachePlayerRefs(WeaponManager wm, InputHandler ih)
        {
            _wm = wm;
            _ih = ih;
            _shooterNO = wm != null ? wm.NetworkObject : null;
        }

        #endregion

        #region Client Legacy Update Path

        void Update()
        {
            if (!IsOwner || _wm == null || _ih == null)
                return;

            /*
             * Regular weapons now fire through:
             * AdvancedPredictedController.MovementData.Held
             * -> WeaponManager.Server_ProcessFireInput(...)
             * -> Server_TryFireFromPose(...)
             *
             * Hidden quick-items keep the old armed Update -> Server_RequestFire path for now.
             */
            if (usePredictedInputFire && !isHiddenQuickItem)
                return;

            if (!isHiddenQuickItem && !IsActive)
                return;

            EnsureFireTimingInitialized();

            if (!CanFire())
                return;

            uint nowTick = TimeManager.Tick;
            _nextFireTick = nowTick + _fireIntervalTicks;

            Server_RequestFire(nowTick);
        }

        protected virtual bool CanFire()
        {
            // Hidden quick-items never use normal LMB held-fire.
            // Subclasses such as GrenadeThrower decide when they are armed.
            if (isHiddenQuickItem)
                return false;

            bool triggerHeld = (_ih.HeldButtons & InputButtons.Fire) != 0;
            return triggerHeld && TimeManager.Tick >= _nextFireTick;
        }

        #endregion

        #region CSP Fire Path

        [Server]
        public bool Server_TryFireFromPose(FirePose pose)
        {
            if (!CanServerAttemptFire())
                return false;

            if (isHiddenQuickItem)
                return false;

            if (!IsActive)
                return false;

            EnsureFireTimingInitialized();

            Vector3 fireDir = GetSafeDirection(pose.Direction, transform.forward);
            Vector3 shooterVelocity = pose.Velocity;
            Vector3 shotOrigin = pose.Position;

            return Server_TrySpawnProjectile(shotOrigin, fireDir, shooterVelocity);
        }

        #endregion

        #region Legacy Server RPC Path

        // Legacy path for hidden quick items and temporary fallback only.
        // Regular weapons should use CSP MovementData.Held.
        [ServerRpc(RequireOwnership = true)]
        void Server_RequestFire(uint clientFireTick, NetworkConnection sender = null)
        {
            if (sender == null || !sender.IsValid)
                return;

            if (!CanServerAttemptFire())
                return;

            EnsureFireTimingInitialized();

            uint serverNow = TimeManager.Tick;
            uint target = clientFireTick;

            if (target >= serverNow)
                target = serverNow > 0 ? serverNow - 1 : 0;

            if (!TryGetFireSnapshot(target, serverNow, out LagCompensationManager.FireSnapshot snap))
                return;

            Vector3 fireDir = GetSafeDirection(snap.Direction, transform.forward);
            Vector3 shotOrigin = snap.Position;
            Vector3 shooterVelocity = snap.Velocity;

            Server_TrySpawnProjectile(shotOrigin, fireDir, shooterVelocity);
        }

        bool TryGetFireSnapshot(uint targetTick, uint serverNow, out LagCompensationManager.FireSnapshot snap)
        {
            snap = default;

            if (LagCompensationManager.Instance == null || _shooterNO == null)
                return false;

            // Exact -> -1 -> +1 -> tolerance -> recent fallback.
            if (LagCompensationManager.Instance.TryGetSnapshot(_shooterNO, targetTick, out snap, 0))
                return true;

            if (targetTick > 0 && LagCompensationManager.Instance.TryGetSnapshot(_shooterNO, targetTick - 1, out snap, 0))
                return true;

            if (LagCompensationManager.Instance.TryGetSnapshot(_shooterNO, targetTick + 1, out snap, 0))
                return true;

            if (LagCompensationManager.Instance.TryGetSnapshot(_shooterNO, targetTick, out snap, 2))
                return true;

            uint last = serverNow > 0 ? serverNow - 1 : 0;
            return LagCompensationManager.Instance.TryGetSnapshot(_shooterNO, last, out snap, 2);
        }

        #endregion

        #region Server Fire / Spawn Core

        bool CanServerAttemptFire()
        {
            return def != null &&
                   def.projectilePrefab != null &&
                   _shooterNO != null;
        }

        [Server]
        bool Server_TrySpawnProjectile(Vector3 shotOrigin, Vector3 fireDir, Vector3 shooterVelocity)
        {
            uint serverNow = TimeManager.Tick;

            if (serverNow < _nextServerFireTick)
                return false;

            if (_wm != null && !_wm.Server_CanConsumeAmmo(def, def.ammoPerShot))
                return false;

            Vector3 spawnPos = ResolveSafeSpawnPosition(shotOrigin, fireDir);
            Vector3 finalVel = fireDir * def.projectileSpeed + shooterVelocity * def.velocityInheritance;

            NetworkObject nob = InstanceFinder.NetworkManager.GetPooledInstantiated(def.projectilePrefab, true);

            if (nob == null)
                return false;

            if (!nob.TryGetComponent(out BaseProjectile proj))
            {
                ServerManager.Despawn(nob, DespawnType.Pool);
                return false;
            }

            /*
             * Important:
             * ServerCanConsume may be destructive. EnergyProjectileWeapon burns energy here.
             * Keep this after all cheap validation and call it once only.
             */
            if (!ServerCanConsume())
            {
                ServerManager.Despawn(nob, DespawnType.Pool);
                return false;
            }

            if (_wm != null && !_wm.Server_TryConsumeAmmo(def, def.ammoPerShot))
            {
                ServerManager.Despawn(nob, DespawnType.Pool);
                return false;
            }

            _nextServerFireTick = serverNow + _fireIntervalTicks;

            Quaternion rot = GetProjectileRotation(fireDir, finalVel);
            nob.transform.SetPositionAndRotation(spawnPos, rot);

            /*
             * BaseProjectile.Init writes deterministic spawn state.
             * FishNet spawn payload sends that state to observing clients.
             * Do not call RpcInit on the normal payload path.
             */
            proj.Init(spawnPos, finalVel, serverNow, _shooterNO);
            ServerManager.Spawn(nob);

            return true;
        }

        protected virtual bool ServerCanConsume() => true;

        #endregion

        #region Fire Timing

        void EnsureFireTimingInitialized()
        {
            if (_fireTimingInitialized)
                return;

            float fireIntervalSeconds = def != null && def.fireRate > 0f
                ? 1f / def.fireRate
                : 0.1f;

            float tickDelta = TimeManager != null
                ? (float)TimeManager.TickDelta
                : Time.fixedDeltaTime;

            _fireIntervalTicks = (uint)Mathf.Max(1, Mathf.RoundToInt(fireIntervalSeconds / tickDelta));
            _nextFireTick = 0;
            _nextServerFireTick = 0;
            _fireTimingInitialized = true;
        }

        public virtual void ResetRuntime()
        {
            _nextFireTick = 0;
            _nextServerFireTick = 0;
        }

        #endregion

        #region Spawn Position / Rotation

        protected virtual float GetSpawnSafetyRadius()
        {
            if (spawnSafetyRadiusOverride > 0f)
                return spawnSafetyRadiusOverride;

            if (def != null)
                return def.castRadius;

            return 0.25f;
        }

        protected virtual Vector3 ResolveSafeSpawnPosition(Vector3 shotOrigin, Vector3 fireDir)
        {
            fireDir = GetSafeDirection(fireDir, transform.forward);

            float radius = GetSpawnSafetyRadius();
            Vector3 desiredSpawn = shotOrigin + fireDir * spawnOffset;

            // If origin is already obstructed, keep it here and let projectile immediately collide/explode.
            if (Physics.CheckSphere(shotOrigin, radius, spawnBlockMask, QueryTriggerInteraction.Ignore))
                return shotOrigin;

            Vector3 finalSpawn = desiredSpawn;

            if (Physics.SphereCast(shotOrigin, radius, fireDir, out RaycastHit hit, spawnOffset, spawnBlockMask, QueryTriggerInteraction.Ignore))
                finalSpawn = hit.point - fireDir * spawnBackoff;

            if (Physics.CheckSphere(finalSpawn, radius, spawnBlockMask, QueryTriggerInteraction.Ignore))
            {
                Vector3 nearOrigin = shotOrigin + fireDir * Mathf.Min(spawnBackoff, spawnOffset * 0.25f);

                if (!Physics.CheckSphere(nearOrigin, radius, spawnBlockMask, QueryTriggerInteraction.Ignore))
                    return nearOrigin;

                return shotOrigin;
            }

            return finalSpawn;
        }

        static Vector3 GetSafeDirection(Vector3 direction, Vector3 fallback)
        {
            if (direction.sqrMagnitude > 0.0001f)
                return direction.normalized;

            if (fallback.sqrMagnitude > 0.0001f)
                return fallback.normalized;

            return Vector3.forward;
        }

        static Quaternion GetProjectileRotation(Vector3 fireDir, Vector3 finalVel)
        {
            Vector3 lookDir = finalVel.sqrMagnitude > 0.0001f
                ? finalVel.normalized
                : GetSafeDirection(fireDir, Vector3.forward);

            return Quaternion.LookRotation(lookDir, Vector3.up);
        }

        #endregion
    }
}
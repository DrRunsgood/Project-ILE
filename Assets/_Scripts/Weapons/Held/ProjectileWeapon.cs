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
        [SerializeField] float spawnOffset = 0.25f;
        [Tooltip("Optional muzzle/fire point. If empty, this weapon auto-finds a child named FirePoint.")]
        [SerializeField] Transform firePoint;

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
        uint _nextLocalFireAudioTick;
        
        static readonly RaycastHit[] AimHits = new RaycastHit[16];

        #endregion

        #region Wiring

        public virtual void CachePlayerRefs(WeaponManager wm, InputHandler ih)
        {
            _wm = wm;
            _ih = ih;
            _shooterNO = wm != null ? wm.NetworkObject : null;
        }
        
        Transform GetFirePoint()
        {
            if (firePoint != null)
                return firePoint;

            Transform found = transform.Find("FirePoint");

            if (found != null)
            {
                firePoint = found;
                return firePoint;
            }

            Transform[] children = GetComponentsInChildren<Transform>(true);

            for (int i = 0; i < children.Length; i++)
            {
                if (children[i] != null && children[i].name == "FirePoint")
                {
                    firePoint = children[i];
                    return firePoint;
                }
            }

            return transform;
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (firePoint == null)
            {
                Transform found = transform.Find("FirePoint");

                if (found != null)
                    firePoint = found;
            }
        }
#endif

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

            Vector3 viewOrigin = pose.Position;
            Vector3 viewDir = GetSafeDirection(pose.Direction, transform.forward);

            Transform muzzle = GetFirePoint();
            Vector3 muzzleOrigin = muzzle != null ? muzzle.position : viewOrigin;

            Vector3 aimPoint = ResolveAimPoint(viewOrigin, viewDir);
            Vector3 fireDir = ResolveMuzzleFireDirection(muzzleOrigin, aimPoint, viewDir);

            Vector3 shooterVelocity = pose.Velocity;

            return Server_TrySpawnProjectile(muzzleOrigin, fireDir, shooterVelocity, viewOrigin, viewDir, true);
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
            return Server_TrySpawnProjectile(
                shotOrigin,
                fireDir,
                shooterVelocity,
                shotOrigin,
                fireDir,
                false);
        }

        [Server]
        bool Server_TrySpawnProjectile(
            Vector3 shotOrigin,
            Vector3 fireDir,
            Vector3 shooterVelocity,
            Vector3 viewOrigin,
            Vector3 viewDir,
            bool useViewMuzzleProbe)
        {
            uint serverNow = TimeManager.Tick;

            if (serverNow < _nextServerFireTick)
                return false;

            if (def == null || def.projectilePrefab == null || _shooterNO == null)
                return false;

            // Validate prefab setup before consuming ammo/energy.
            if (!def.projectilePrefab.TryGetComponent(out BaseProjectile _))
            {
                Debug.LogError($"[ProjectileWeapon] Projectile prefab '{def.projectilePrefab.name}' has no BaseProjectile.");
                return false;
            }

            if (_wm != null && !_wm.Server_CanConsumeAmmo(def, def.ammoPerShot))
                return false;

            /*
             * Important: ServerCanConsume may be destructive.
             * EnergyProjectileWeapon burns energy here, so call this once only.
             *
             * Keep this BEFORE taking a pooled projectile so failed energy checks
             * do not create/despawn pooled objects.
             */
            if (!ServerCanConsume())
                return false;

            if (_wm != null && !_wm.Server_TryConsumeAmmo(def, def.ammoPerShot))
                return false;

            fireDir = GetSafeDirection(fireDir, transform.forward);

            Vector3 spawnDir = fireDir;
            Vector3 velocityDir = GetSafeDirection(AdjustProjectileVelocityDirection(fireDir), fireDir);

            if (Vector3.Dot(velocityDir.normalized, fireDir.normalized) <= 0.25f)
                velocityDir = fireDir;
            
            Vector3 impactPoint = shotOrigin;
            Vector3 impactNormal = -spawnDir;
            Collider impactCollider = null;

            bool hasMuzzleContact = TryGetImmediateMuzzleImpact(shotOrigin, spawnDir, viewOrigin, viewDir, useViewMuzzleProbe,
                out impactPoint, out impactNormal, out impactCollider);

            bool immediateImpact = hasMuzzleContact && def.resolveImmediateMuzzleImpact;

            Vector3 spawnPos = immediateImpact? impactPoint : 
                ResolveSpawnPosition(shotOrigin, spawnDir, hasMuzzleContact, impactPoint, impactNormal);

            Vector3 finalVel = velocityDir * def.projectileSpeed + shooterVelocity * def.velocityInheritance;

            NetworkObject nob = InstanceFinder.NetworkManager.GetPooledInstantiated(def.projectilePrefab, true);

            if (nob == null)
            {
                Debug.LogWarning($"[ProjectileWeapon] Could not get pooled projectile for '{def.displayName}'.");
                return false;
            }

            if (!nob.TryGetComponent(out BaseProjectile proj))
            {
                Debug.LogError($"[ProjectileWeapon] Spawned projectile '{nob.name}' has no BaseProjectile despite prefab validation.");
                return false;
            }

            Quaternion rot = GetProjectileRotation(finalVel.sqrMagnitude > 0.0001f ? finalVel.normalized : velocityDir, finalVel);

            nob.transform.SetPositionAndRotation(spawnPos, rot);

            proj.Init(spawnPos, finalVel, serverNow, _shooterNO);
            ServerManager.Spawn(nob);

            if (immediateImpact)
                proj.ServerExplodeImmediately(impactPoint, impactNormal, impactCollider);

            _nextServerFireTick = serverNow + _fireIntervalTicks;

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
            _nextLocalFireAudioTick = 0;
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

        protected virtual Vector3 ResolveSpawnPosition(Vector3 shotOrigin, Vector3 fireDir, bool hasMuzzleContact, Vector3 impactPoint,
            Vector3 impactNormal)
        {
            fireDir = GetSafeDirection(fireDir, transform.forward);

            float radius = GetSpawnSafetyRadius();
            float clearance = Mathf.Max(radius + spawnBackoff, 0.03f);

            /*
             * Known contact path:
             * We already know the muzzle/fire path contacted world geometry.
             * For non-immediate-impact projectiles, such as grenades, spawn the live
             * projectile just outside the contacted surface and let its own simulation
             * handle the bounce.
             */
            if (hasMuzzleContact)
            {
                Vector3 normal = impactNormal.sqrMagnitude > 0.0001f
                    ? impactNormal.normalized
                    : -fireDir;

                return impactPoint + normal * clearance;
            }

            /*
             * Clear path:
             * Spawn normally in front of the fire origin. If the short spawn corridor
             * is obstructed, back off from the hit point.
             */
            Vector3 desiredSpawn = shotOrigin + fireDir * spawnOffset;

            if (Physics.SphereCast(shotOrigin, radius, fireDir, out RaycastHit hit, spawnOffset, spawnBlockMask,
                    QueryTriggerInteraction.Ignore))
            {
                if (!IsShooterCollider(hit.collider))
                    return hit.point - fireDir * spawnBackoff;
            }

            return desiredSpawn;
        }
        
        [Server]
        bool TryGetImmediateMuzzleImpact(Vector3 shotOrigin, Vector3 fireDir, Vector3 viewOrigin, Vector3 viewDir, bool useViewMuzzleProbe,
            out Vector3 impactPoint, out Vector3 impactNormal, out Collider impactCollider)
        {
            fireDir = GetSafeDirection(fireDir, transform.forward);
            viewDir = GetSafeDirection(viewDir, fireDir);

            impactPoint = shotOrigin;
            impactNormal = -fireDir;
            impactCollider = null;

            float radius = GetSpawnSafetyRadius();

            /*
             * 1. View -> muzzle probe.
             *
             * Catches the common FPS case where the visual/gameplay muzzle is pushed
             * into or through a wall. We use hit.point/hit.normal from the cast, not
             * Collider.ClosestPoint, so this works with normal static mesh colliders.
             */
            if (useViewMuzzleProbe)
            {
                float viewToMuzzleDistance = Vector3.Distance(viewOrigin, shotOrigin);

                if (viewToMuzzleDistance > 0.01f)
                {
                    float probeDistance = viewToMuzzleDistance + Mathf.Max(spawnOffset, 0.05f);

                    if (Physics.SphereCast(viewOrigin, radius, viewDir, out RaycastHit viewHit, probeDistance,
                            spawnBlockMask, QueryTriggerInteraction.Ignore))
                    {
                        if (!IsShooterCollider(viewHit.collider))
                        {
                            impactPoint = viewHit.point;
                            impactNormal = viewHit.normal.sqrMagnitude > 0.0001f ? viewHit.normal : -fireDir;
                            impactCollider = viewHit.collider;
                            return true;
                        }
                    }
                }
            }

            /*
             * 2. Muzzle forward probe.
             *
             * Catches normal point-blank obstruction in front of the muzzle.
             */
            if (Physics.SphereCast(shotOrigin, radius, fireDir, out RaycastHit muzzleHit, spawnOffset,
                    spawnBlockMask, QueryTriggerInteraction.Ignore))
            {
                if (!IsShooterCollider(muzzleHit.collider))
                {
                    impactPoint = muzzleHit.point;
                    impactNormal = muzzleHit.normal.sqrMagnitude > 0.0001f ? muzzleHit.normal : -fireDir;
                    impactCollider = muzzleHit.collider;
                    return true;
                }
            }

            return false;
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
        
        protected virtual Vector3 AdjustProjectileVelocityDirection(Vector3 fireDir)
        {
            return fireDir;
        }
        
        [Server]
        protected virtual Vector3 ResolveAimPoint(Vector3 viewOrigin, Vector3 viewDir)
        {
            viewDir = GetSafeDirection(viewDir, transform.forward);

            if (def == null)
                return viewOrigin + viewDir * 600f;

            float distance = Mathf.Max(1f, def.convergenceDistance);

            switch (def.convergenceMode)
            {
                case ConvergenceMode.FixedDistance:
                    return viewOrigin + viewDir * distance;

                case ConvergenceMode.RaycastClamped:
                    // Future mode. For now, fall through to simple behavior.
                    return ResolveSimpleRaycastAimPoint(viewOrigin, viewDir, distance);

                case ConvergenceMode.SimpleRaycast:
                default:
                    return ResolveSimpleRaycastAimPoint(viewOrigin, viewDir, distance);
            }
        }

        [Server]
        protected virtual Vector3 ResolveSimpleRaycastAimPoint(
            Vector3 viewOrigin,
            Vector3 viewDir,
            float maxDistance)
        {
            if (TryGetAimHit(viewOrigin, viewDir, maxDistance, out RaycastHit hit))
                return hit.point;

            return viewOrigin + viewDir * maxDistance;
        }

        [Server]
        protected virtual bool TryGetAimHit(
            Vector3 viewOrigin,
            Vector3 viewDir,
            float maxDistance,
            out RaycastHit bestHit)
        {
            bestHit = default;

            if (def == null)
                return false;

            int hitCount = Physics.RaycastNonAlloc(
                viewOrigin,
                viewDir,
                AimHits,
                maxDistance,
                def.aimMask,
                QueryTriggerInteraction.Ignore);

            if (hitCount <= 0)
                return false;

            bool found = false;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit hit = AimHits[i];

                if (hit.collider == null)
                    continue;

                if (IsShooterCollider(hit.collider))
                    continue;

                if (hit.distance < closestDistance)
                {
                    closestDistance = hit.distance;
                    bestHit = hit;
                    found = true;
                }
            }

            return found;
        }

        protected virtual bool IsShooterCollider(Collider col)
        {
            if (col == null || _shooterNO == null)
                return false;

            NetworkObject hitNob = col.GetComponentInParent<NetworkObject>();

            if (hitNob == null)
                return false;

            return hitNob == _shooterNO;
        }
        
        static Vector3 ResolveMuzzleFireDirection(Vector3 muzzleOrigin, Vector3 aimPoint, Vector3 viewDir)
        {
            viewDir = GetSafeDirection(viewDir, Vector3.forward);

            Vector3 toAim = aimPoint - muzzleOrigin;

            if (toAim.sqrMagnitude <= 0.0001f)
                return viewDir;

            Vector3 dir = toAim.normalized;

            /*
             * Muzzle convergence may refine the shot direction, but it may not turn
             * the projectile sideways/backward when the camera ray hits near geometry.
             */
            if (Vector3.Dot(dir, viewDir) <= 0.25f)
                return viewDir;

            return dir;
        }

        #endregion
        
        #region Effects
        
        public void Client_TryPlayPredictedFireSfx(Vector3 pos)
        {
            if (!IsOwner)
                return;

            if (isHiddenQuickItem)
                return;

            if (!IsActive)
                return;

            if (def == null || def.fireSfx == null || !def.playLocalPredictedFireSfx)
                return;

            EnsureFireTimingInitialized();

            uint now = TimeManager.Tick;

            if (now < _nextLocalFireAudioTick)
                return;

            if (!ClientCanPlayPredictedFireSfx())
                return;

            _nextLocalFireAudioTick = now + _fireIntervalTicks;

            float pitch = Random.Range(def.firePitchMin, def.firePitchMax);

            WeaponAudioPool.PlayOneShot(
                def.fireSfx,
                pos,
                def.fireVolume * def.localFireVolumeMultiplier,
                pitch,
                def.localFireSpatialBlend,
                def.fireMinDistance,
                def.fireMaxDistance);
        }

        protected virtual bool ClientCanPlayPredictedFireSfx()
        {
            if (def == null)
                return false;

            if (def.usesAmmo && _wm != null && _wm.ActiveAmmo < def.ammoPerShot)
                return false;

            return true;
        }
        
        #endregion
    }
}
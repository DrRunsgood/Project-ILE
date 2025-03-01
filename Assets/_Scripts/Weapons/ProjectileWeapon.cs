using _Scripts.Player;
using UnityEngine;
using FishNet;
using FishNet.Object;
using YourGameNamespace.Weapons;       // For BaseWeapon
using YourGameNamespace.Player.Weapons; // For the Projectile script (assuming that's the namespace)

namespace YourGameNamespace.Weapons
{
    /// <summary>
    /// A concrete weapon class that fires a pooled projectile.
    /// </summary>
    public class ProjectileWeapon : BaseWeapon
    {
        [Header("Projectile Settings")]
        [Tooltip("Transform representing the fire point from where projectiles are spawned.")]
        [SerializeField] private Transform firePoint;

        [Tooltip("Speed at which the projectile moves.")]
        [SerializeField] private float projectileSpeed = 20f;

        [Tooltip("Projectile NetworkObject prefab (pooled).")]
        [SerializeField] private FishNet.Object.NetworkObject projectilePrefab;

        // Input
        private InputHandler _inputHandler;

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (!IsOwner)
                return;

            // Attempt to get InputHandler from the same GameObject
            _inputHandler = GetComponent<InputHandler>();
            if (_inputHandler == null)
            {
                Debug.LogError("ProjectileWeapon: InputHandler not found on the same GameObject.");
            }
        }

        private void Update()
        {
            if (!IsOwner) return;
            if (_inputHandler.FireInput)
                Fire(); // calls Fire(), which spawns a projectile on the server
        }

        public override void Fire()
        {
            if (!IsOwner) return;
            if (Time.time < nextFireTime) return;

            nextFireTime = Time.time + 1f / fireRate;

            // Determine direction from firePoint
            Vector3 aimingDirection = firePoint.forward.normalized;

            // Call ServerRpc to spawn the projectile
            SpawnProjectileServerRpc(aimingDirection);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnProjectileServerRpc(Vector3 direction)
        {
            if (projectilePrefab == null || firePoint == null)
            {
                Debug.LogError("ProjectileWeapon: Missing prefab or fire point.");
                return;
            }

            // Retrieve a pooled projectile from FishNet
            // 'parent' can be null or 'firePoint', your preference
            var nob = InstanceFinder.NetworkManager.GetPooledInstantiated(projectilePrefab, null, true);
            if (nob == null)
            {
                Debug.LogError("ProjectileWeapon: Failed to get pooled projectile.");
                return;
            }

            // Position & rotation
            nob.transform.position = firePoint.position;
            nob.transform.rotation = firePoint.rotation;

            // Initialize projectile
            Projectile proj = nob.GetComponent<Projectile>();
            if (proj != null)
            {
                Vector3 velocity = direction * projectileSpeed;
                proj.Initialize(velocity, transform);
            }
            else
            {
                Debug.LogError("ProjectileWeapon: Projectile script missing on prefab.");
            }

            Debug.Log($"ProjectileWeapon: Before setting owner, projectile owner is: {(nob.Owner != null ? nob.Owner.ToString() : "null")}");

            // Set the owner of the projectile to the firing player

            // Spawn the projectile
            InstanceFinder.ServerManager.Spawn(nob, base.Owner);
            Debug.Log($"ProjectileWeapon: After setting owner, projectile owner is: {(nob.Owner != null ? nob.Owner.ToString() : "null")}");
        }
    }
}

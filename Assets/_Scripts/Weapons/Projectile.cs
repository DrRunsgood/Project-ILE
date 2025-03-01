using UnityEngine;
using FishNet.Object;
using FishNet;
using System;

namespace YourGameNamespace.Player.Weapons
{
    /// <summary>
    /// Basic projectile that travels until it collides or times out, then despawns to the pool.
    /// </summary>
    public class Projectile : NetworkBehaviour
    {
        [SerializeField] private float lifetime = 5f;
        [Tooltip("Percentage (0-1) of the shooter’s horizontal velocity to inherit. " +
                 "For example, 0.5 = 50%, 0.75 = 75%.")]
        [Range(0f, 1f)]
        [SerializeField] private float horizontalInheritancePercent = 0.5f;

        private Rigidbody _rb;
        private bool _isDespawning;
        private float _spawnTime;
        private Transform _shooterTransform;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            if (_rb == null)
                Debug.LogError("Projectile: Missing Rigidbody!");
        }

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();
            if (IsServerStarted)
                _spawnTime = Time.time;
        }

        /// <summary>
        /// Initialize velocity and optionally ignore collision with shooter.
        /// Inherits horizontal (X and Z) velocity from shooter based on horizontalInheritancePercent.
        /// Also outputs a debug log with the raw speed of the projectile.
        /// </summary>
        public void Initialize(Vector3 velocity, Transform shooterTransform)
        {
            _isDespawning = false;
            _shooterTransform = shooterTransform;

            Vector3 finalVelocity = velocity;

            // If a shooter is provided, attempt to inherit their horizontal velocity.
            if (shooterTransform != null)
            {
                Rigidbody shooterRb = shooterTransform.GetComponent<Rigidbody>();
                if (shooterRb != null)
                {
                    // Extract only horizontal (x and z) components; ignore vertical (y) component.
                    Vector3 shooterHorizontalVel = shooterRb.linearVelocity;
                    shooterHorizontalVel.y = 0f;

                    // Calculate inherited velocity.
                    Vector3 inheritedVelocity = shooterHorizontalVel * horizontalInheritancePercent;

                    finalVelocity += inheritedVelocity;

                    Debug.Log($"Projectile: Shooter '{shooterTransform.name}' horizontal velocity = {shooterHorizontalVel}. " +
                              $"Inheriting {horizontalInheritancePercent * 100}% results in inherited velocity = {inheritedVelocity}.");
                }
                else
                {
                    Debug.LogWarning("Projectile: Shooter does not have a Rigidbody. No velocity inheritance applied.");
                }
            }
            else
            {
                Debug.Log("Projectile: No shooter transform provided; skipping velocity inheritance.");
            }

            // Set the final calculated velocity.
            if (_rb != null)
            {
                _rb.linearVelocity = finalVelocity;
                Debug.Log($"Projectile: Final initialized velocity = {finalVelocity}.");
                Debug.Log($"Projectile: Raw speed (magnitude) = {finalVelocity.magnitude}.");
            }

            // Ignore collision with shooter.
            Collider projCollider = GetComponent<Collider>();
            Collider shooterCollider = (shooterTransform != null) ? shooterTransform.GetComponent<Collider>() : null;

            if (projCollider != null && shooterCollider != null)
            {
                Physics.IgnoreCollision(projCollider, shooterCollider);
                Debug.Log($"Projectile: Ignoring collision with shooter '{shooterTransform.name}'.");
            }
            else
            {
                Debug.LogWarning("Projectile: Could not ignore collision. Missing colliders.");
            }
        }

        private void Update()
        {
            // Lifetime check
            if (IsServerStarted && !_isDespawning && (Time.time - _spawnTime >= lifetime))
            {
                Debug.Log("Projectile: Lifetime expired, despawning.");
                DespawnToPool();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!IsServerStarted || _isDespawning)
                return;

            Debug.Log($"Projectile: Collided with {collision.gameObject.name}.");

            // If we hit the shooter (shouldn't happen if IgnoreCollision works)
            if (collision.transform == _shooterTransform)
            {
                Debug.LogWarning("Projectile: Collided with shooter! Ignoring for now.");
                return;
            }

            // E.g., apply damage or do other collision logic here

            DespawnToPool();
        }

        private void DespawnToPool()
        {
            _isDespawning = true;

            if (_rb != null)
                _rb.linearVelocity = Vector3.zero;

            // Return to FishNet pool
            if (NetworkObject != null)
            {
                Debug.Log("Projectile: Despawning to pool.");
                InstanceFinder.ServerManager.Despawn(NetworkObject, DespawnType.Pool);
            }
        }
    }
}

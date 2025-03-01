using UnityEngine;
using FishNet.Object;

namespace YourGameNamespace.Weapons
{
    /// <summary>
    /// Abstract base weapon class. Defines common fields, 
    /// and forces child classes to implement Fire().
    /// </summary>
    public abstract class BaseWeapon : NetworkBehaviour
    {
        [Header("Shared Weapon Settings")]
        [SerializeField] protected float fireRate = 5f;
        protected float nextFireTime;

        /// <summary>
        /// Called when the player attempts to fire the weapon.
        /// Concrete weapon classes override this to do the actual logic.
        /// </summary>
        public abstract void Fire();
    }
}

// _Scripts/Data/WeaponDefinition.cs
using UnityEngine;
using FishNet.Object;                       // NetworkObject

namespace _Scripts.Data
{
    public enum CastMode { Sphere, Capsule, Ray }

    [CreateAssetMenu(menuName = "Weapons/Weapon Definition",
                     fileName   = "NewWeapon")]
    public class WeaponDefinition : ScriptableObject
    {
    /* ───────── General ───────── */
        [Header("Weapon Info")]
        public string displayName   = "Rocket-Launcher";
        [Min(0.01f)] public float fireRate      = 1f;   // shots / second
        public bool  usesAmmo       = false;
        public int   magazine       = 6;
        public float reloadTime     = 1.2f;             // seconds
        
    /* ───────── Projectile flight ───────── */
        [Header("Projectile")]
        public NetworkObject projectilePrefab;
        public float  projectileSpeed      = 120f;
        [Range(0f,1f)] public float velocityInheritance = 0.7f;
        public float  gravityScale         = 0f;        // 0 = no gravity
        public float lifeTime       = 15f;              // projectile life

    /* ───────── Hit sweep (server) ───────── */
        [Header("Sweep primitive")]
        public CastMode castMode     = CastMode.Sphere;
        [Min(0f)] public float castRadius = 0.15f;      // sphere or capsule
        [Min(0f)] public float castHalf   = 0.40f;      // half length capsule
        public LayerMask hitMask;                       // walls etc.
        public LayerMask playerMask;                    // players only

    /* ───────── Explosion / Damage ───────── */
        [Header("Explosion / Knock-back & Damage")]
        public float  blastRadius       = 6f;           // 0 = none
        public float  baseDamage        = 100f;
        public float  knockbackForce        = 500f;
        [Tooltip("1 = linear, 2 = quadratic, 1.5 ≈ Tribes")]
        public float  knockFalloffExp   = 1.5f;
        [Tooltip("Treat centre hits as straight-up impulse below this distance")]
        public float minDirThreshold    = 0.01f;

    /* ───────── Prefabs & Effects ───────── */
        [Header("Gun / FX")]
        public NetworkObject heldPrefab;                // gun in hand
        public NetworkObject groundPrefab;
        public GameObject     muzzleVFX;
        public GameObject     impactVFX;
        public AudioClip      fireSFX;
    }
}

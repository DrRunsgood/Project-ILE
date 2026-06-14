// _Scripts/Data/WeaponDefinition.cs
using UnityEngine;
using FishNet.Object;                       // NetworkObject

namespace _Scripts.Data
{
    public enum CastMode { Sphere, Capsule, Ray }
    
    public enum AmmoType : byte
    {
        None,
        Disc,
        Bullet,
        Grenade,
        Mortar,
        Rocket,
        Special
    }

    [CreateAssetMenu(menuName = "Weapons/Weapon Definition",
                     fileName   = "NewWeapon")]
    public class WeaponDefinition : ScriptableObject
    {
    /* ───────── General ───────── */
        [Header("Weapon Info")]
        public string displayName   = "Rocket-Launcher";
        [Min(0.01f)] public float fireRate      = 1f;   // shots / second

        [Header("Energy weapon")]
        public float energyPerShot = 4f;  // cost in “energy units”
        public bool  requiresEnergyPack;      // true  ⇒ can only be picked up
        
    /* ───────── Ammo ───────── */    
        [Header("Ammo")]
        public bool usesAmmo = false;
        public AmmoType ammoType = AmmoType.None;
        public int spawnAmmo = 10;
        public int maxAmmo = 15;
        public int ammoPerShot = 1;
        
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
        public float  damage        = 100f;
        public float  knockbackForce        = 500f;
        [Tooltip("1 = linear, 2 = quadratic, 1.5 ≈ Tribes")]
        public float  knockFalloffExp   = 1.5f;
        [Tooltip("Treat centre hits as straight-up impulse below this distance")]
        public float minDirThreshold    = 0.01f;
        
        [Header("Objective Interaction")]
        public LayerMask objectiveMask;
        [Range(0f, 2f)] public float objectiveKnockbackMultiplier = 1f;

    /* ───────── Prefabs & Effects ───────── */
        [Header("Gun / FX")]
        public GameObject    fpViewPrefab;  // Local visual game object for our first person player view
        public NetworkObject heldPrefab;                // gun in hand for others
        public NetworkObject groundPrefab;
        
        [Header("Projectile VFX")]
        public string impactVfxKey = "VFX/RocketExplosion";
        public float impactVfxLifetime = 2.5f;
        public bool spawnImpactVfx = true;
        
        [Header("Fire Audio")]
        public AudioClip fireSfx;

        [Range(0f, 1f)]
        public float fireVolume = 1f;

        public float firePitchMin = 0.96f;
        public float firePitchMax = 1.04f;

        [Tooltip("World/observer fire sound. 1 = full 3D.")]
        [Range(0f, 1f)]
        public float fireSpatialBlend = 1f;

        public float fireMinDistance = 3f;
        public float fireMaxDistance = 80f;

        [Header("Local Predicted Fire Audio")]
        public bool playLocalPredictedFireSfx = true;

        [Range(0f, 1f)]
        public float localFireSpatialBlend = 0f;

        [Range(0f, 2f)]
        public float localFireVolumeMultiplier = 1f;

        [Header("Impact Audio")]
        public AudioClip impactSfx;

        [Range(0f, 1f)]
        public float impactVolume = 1f;

        public float impactPitchMin = 0.95f;
        public float impactPitchMax = 1.05f;

        public float impactMinDistance = 4f;
        public float impactMaxDistance = 100f;
        
        [Header("Meta / UI")]
        public bool hiddenQuickItem = false;
    }
}

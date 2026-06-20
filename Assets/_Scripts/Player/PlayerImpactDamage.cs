using _Scripts.Combat;
using FishNet.Object;
using UnityEngine;

namespace _Scripts.Player
{
    [RequireComponent(typeof(PlayerHealth))]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class PlayerImpactDamage : NetworkBehaviour
    {
        [Header("Impact Damage")]
        [SerializeField] private bool enableImpactDamage = true;

        [Tooltip("Minimum speed into the surface required before impact damage can occur.")]
        [SerializeField] private float minDamageSpeed = 60f;

        [Tooltip("Speed into the surface where impact damage reaches its speed contribution cap.")]
        [SerializeField] private float maxDamageSpeed = 150f;

        [Tooltip("Minimum Unity collision impulse required before impact damage can occur.")]
        [SerializeField] private float minImpactImpulse = 20f;

        [Tooltip("Collision impulse where impact damage reaches its impulse contribution cap.")]
        [SerializeField] private float maxImpactImpulse = 120f;

        [Tooltip("Filters smooth/glancing contacts. 0 allows nearly all contacts; 1 requires a direct slam into the surface.")]
        [SerializeField, Range(0f, 1f)] private float minGlancingRatio = 0.55f;

        [Tooltip("Minimum damage dealt once an impact qualifies.")]
        [SerializeField] private int minDamage = 1;

        [Tooltip("Maximum damage dealt by a severe impact.")]
        [SerializeField] private int maxDamage = 100;

        [Tooltip("Higher values make moderate impacts less punishing. 1 = linear, 2 = squared, 3+ = very forgiving until severe impacts.")]
        [SerializeField, Min(0.25f)] private float severityExponent = 2.5f;

        [Tooltip("Prevents repeated collision spam from causing multiple damage events.")]
        [SerializeField] private float damageCooldownSeconds = 0.45f;

        [Tooltip("Only these layers can cause impact damage. Set this to World/Terrain/Environment layers.")]
        [SerializeField] private LayerMask damagingLayers = ~0;

        [Header("Debug")]
        [SerializeField] private bool debugImpacts;

        private PlayerHealth _health;
        private float _nextAllowedDamageTime;

        private void Awake()
        {
            _health = GetComponent<PlayerHealth>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!IsServerInitialized)
                return;

            if (!enableImpactDamage)
                return;

            if (_health == null || !_health.IsAlive)
                return;

            if (Time.time < _nextAllowedDamageTime)
                return;

            if (collision.collider == null)
                return;

            if ((damagingLayers.value & (1 << collision.collider.gameObject.layer)) == 0)
                return;

            if (collision.contactCount <= 0)
                return;

            ContactPoint contact = GetStrongestContact(collision);

            Vector3 relativeVelocity = collision.relativeVelocity;
            float totalSpeed = relativeVelocity.magnitude;

            if (totalSpeed <= 0.001f)
                return;

            // Speed directed into the contacted surface.
            // Tangential speed, such as smooth skiing along terrain, contributes little.
            float normalImpactSpeed = Mathf.Max(
                0f,
                Vector3.Dot(relativeVelocity, contact.normal));

            float glancingRatio = normalImpactSpeed / totalSpeed;
            float impulseMagnitude = collision.impulse.magnitude;

            if (normalImpactSpeed < minDamageSpeed)
            {
                DebugImpact(
                    "Below min normal speed",
                    normalImpactSpeed,
                    totalSpeed,
                    glancingRatio,
                    impulseMagnitude,
                    0);

                return;
            }

            if (glancingRatio < minGlancingRatio)
            {
                DebugImpact(
                    "Below glancing ratio",
                    normalImpactSpeed,
                    totalSpeed,
                    glancingRatio,
                    impulseMagnitude,
                    0);

                return;
            }

            if (impulseMagnitude < minImpactImpulse)
            {
                DebugImpact(
                    "Below min impulse",
                    normalImpactSpeed,
                    totalSpeed,
                    glancingRatio,
                    impulseMagnitude,
                    0);

                return;
            }

            int damage = CalculateDamage(normalImpactSpeed, impulseMagnitude);

            if (damage <= 0)
                return;

            var info = new DamageInfo(
                amount: damage,
                attacker: null,
                source: NetworkObject,
                type: DamageType.Impact,
                point: contact.point,
                normal: contact.normal,
                impulse: Vector3.zero,
                weaponId: 0);

            _health.ApplyDamage(info);

            _nextAllowedDamageTime = Time.time + damageCooldownSeconds;

            DebugImpact(
                "Applied",
                normalImpactSpeed,
                totalSpeed,
                glancingRatio,
                impulseMagnitude,
                damage);
        }

        private ContactPoint GetStrongestContact(Collision collision)
        {
            ContactPoint strongest = collision.GetContact(0);
            Vector3 relativeVelocity = collision.relativeVelocity;

            float strongestSpeed = Mathf.Max(
                0f,
                Vector3.Dot(relativeVelocity, strongest.normal));

            for (int i = 1; i < collision.contactCount; i++)
            {
                ContactPoint contact = collision.GetContact(i);

                float speed = Mathf.Max(
                    0f,
                    Vector3.Dot(relativeVelocity, contact.normal));

                if (speed > strongestSpeed)
                {
                    strongest = contact;
                    strongestSpeed = speed;
                }
            }

            return strongest;
        }

        private int CalculateDamage(float normalImpactSpeed, float impulseMagnitude)
        {
            float speedT = Mathf.InverseLerp(
                minDamageSpeed,
                maxDamageSpeed,
                normalImpactSpeed);

            float impulseT = Mathf.InverseLerp(
                minImpactImpulse,
                maxImpactImpulse,
                impulseMagnitude);

            // Both must be meaningful: fast into-surface speed AND strong collision impulse.
            float severity = speedT * impulseT;

            // Higher exponent makes medium impacts less punishing.
            severity = Mathf.Pow(severity, severityExponent);

            return Mathf.RoundToInt(Mathf.Lerp(minDamage, maxDamage, severity));
        }

        private void DebugImpact(
            string result,
            float normalImpactSpeed,
            float totalSpeed,
            float glancingRatio,
            float impulseMagnitude,
            int damage)
        {
            if (!debugImpacts)
                return;

            Debug.Log(
                $"[PlayerImpactDamage] {result} | " +
                $"normalSpeed={normalImpactSpeed:F2}, " +
                $"totalSpeed={totalSpeed:F2}, " +
                $"glancing={glancingRatio:F2}, " +
                $"impulse={impulseMagnitude:F2}, " +
                $"damage={damage}");
        }
    }
}
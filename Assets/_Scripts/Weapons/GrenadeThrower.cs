using FishNet.Object;
using UnityEngine;

namespace _Scripts.Weapons
{
    public sealed class GrenadeThrower : ProjectileWeapon
    {
        public override bool isHiddenQuickItem => true;

        [Header("Throw Arc")]
        [Tooltip("Extra upward throw angle when aiming horizontally.")]
        [SerializeField] [Range(0f, 45f)] private float throwArcDegrees = 15f;

        [Tooltip("Higher values make the added arc disappear faster " +
                 "while aiming vertically. A value of 1 is linear.")]
        [SerializeField] [Range(0.25f, 4f)] private float verticalArcDampingPower = 1f;

        [Server]
        public bool Server_TryThrowFromPose(FirePose pose)
        {
            /*
             * Grenades are hidden inventory actions, not selected weapons.
             * They still use the authoritative FirePoint and the common
             * projectile spawn/cooldown/safety pipeline.
             */
            return Server_TryFireFromPoseCore(pose, requireActive: false);
        }

        protected override Vector3 AdjustProjectileVelocityDirection(Vector3 fireDirection)
        {
            return ApplyThrowArc(fireDirection, throwArcDegrees, verticalArcDampingPower);
        }

        private static Vector3 ApplyThrowArc(Vector3 aimDirection, float arcDegrees, float dampingPower)
        {
            if (aimDirection.sqrMagnitude <= 0.0001f)
                return Vector3.forward;

            aimDirection.Normalize();

            if (arcDegrees <= 0.001f)
                return aimDirection;

            /*
             * 0 verticality means horizontal aim and receives the
             * complete configured throw arc.
             *
             * 1 verticality means straight up/down and receives no
             * additional arc.
             */
            float verticality = Mathf.Clamp01(Mathf.Abs(aimDirection.y));

            float arcBlend = 1f - verticality;

            dampingPower = Mathf.Max(0.01f, dampingPower);

            arcBlend = Mathf.Pow(arcBlend, dampingPower);

            float arcRadians = arcDegrees * arcBlend * Mathf.Deg2Rad;

            if (arcRadians <= 0.0001f)
                return aimDirection;

            return Vector3.RotateTowards(aimDirection, Vector3.up, arcRadians, 0f).normalized;
        }
    }
}
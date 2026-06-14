using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

namespace _Scripts.Weapons
{
    public sealed class GrenadeThrower : ProjectileWeapon
    {
        public override bool isHiddenQuickItem => true;

        [Header("Throw Arc")]
        [Tooltip("Extra upward throw angle when aiming horizontally.")]
        [SerializeField, Range(0f, 45f)] float throwArcDegrees = 15f;

        [Tooltip("Higher = arc disappears faster as you aim straight up/down. 1 = linear.")]
        [SerializeField, Range(0.25f, 4f)] float verticalArcDampingPower = 1f;

        volatile bool _armed;

        [Server]
        internal void ArmQuickThrow()
        {
            _armed = true;
            IsActive = true;
            gameObject.SetActive(true);

            Target_Arm(base.Owner);
        }

        [TargetRpc]
        void Target_Arm(NetworkConnection _)
        {
            _armed = true;
            IsActive = true;
            gameObject.SetActive(true);
        }

        protected override bool CanFire()
        {
            if (!_armed)
                return false;

            _armed = false;
            IsActive = false;
            return true;
        }

        protected override Vector3 AdjustProjectileVelocityDirection(Vector3 fireDir)
        {
            return ApplyThrowArc(fireDir, throwArcDegrees, verticalArcDampingPower);
        }

        static Vector3 ApplyThrowArc(Vector3 aimDir, float arcDegrees, float dampingPower)
        {
            if (aimDir.sqrMagnitude <= 0.0001f)
                return Vector3.forward;

            aimDir.Normalize();

            if (arcDegrees <= 0.001f)
                return aimDir;

            /*
             * verticality:
             * 0 = looking horizontally
             * 1 = looking straight up/down
             *
             * arcBlend:
             * 1 = full throw arc
             * 0 = no added arc
             */
            float verticality = Mathf.Clamp01(Mathf.Abs(aimDir.y));
            float arcBlend = 1f - verticality;

            dampingPower = Mathf.Max(0.01f, dampingPower);
            arcBlend = Mathf.Pow(arcBlend, dampingPower);

            float arcRadians = arcDegrees * arcBlend * Mathf.Deg2Rad;

            if (arcRadians <= 0.0001f)
                return aimDir;

            return Vector3.RotateTowards(aimDir,Vector3.up, arcRadians, 0f).normalized;
        }

        protected override bool ServerCanConsume() => true;
    }
}
using _Scripts.Items;
using FishNet.Object;
using FishNet.Connection;
using UnityEngine;

namespace _Scripts.Weapons
{
    public sealed class GrenadeThrower : ProjectileWeapon
    {
        public override bool isHiddenQuickItem => true;

        volatile bool _armed;

        //protected override Vector3 GetFireDir(Vector3 aimPoint, Vector3 spawnPos)
        //{
        //    return muzzle ? muzzle.forward.normalized : base.GetFireDir(aimPoint, spawnPos);
        //}
        /* called once by ItemManager – runs on SERVER */
        [Server] internal void ArmQuickThrow()
        {
            _armed   = true;
            IsActive = true;                  // make Update tick
            gameObject.SetActive(true);

            Target_Arm(base.Owner);           // tell owning client
        }

        /* arrived on OWNER-client */
        [TargetRpc] void Target_Arm(NetworkConnection _)
        {
            _armed   = true;
            IsActive = true;
            gameObject.SetActive(true);
        }

        /* owner-side gate; runs every frame */
        protected override bool CanFire()
        {
            if (!_armed) return false;

            _armed   = false;                 // consume one throw
            IsActive = false;                 // immediately dormant
            return true;                      // let base spawn grenade
        }
        
        protected override bool ServerCanConsume() => true;
    }
}
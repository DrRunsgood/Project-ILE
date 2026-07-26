// EnergyProjectileWeapon.cs

using _Scripts.Packs;
using _Scripts.Player;

namespace _Scripts.Weapons
{
    public class EnergyProjectileWeapon : ProjectileWeapon
    {
        private AdvancedPredictedController _controller;
        private PackManager _packManager;

        private float EnergyPerShot => def.energyPerShot;
        private bool NeedsEnergyPack => def.requiresEnergyPack;

        #region Wiring

        public override void CachePlayerRefs(WeaponManager weaponManager, InputHandler inputHandler)
        {
            base.CachePlayerRefs(weaponManager, inputHandler);

            _controller = weaponManager.GetComponent<AdvancedPredictedController>();

            _packManager = weaponManager.GetComponent<PackManager>();
        }

        #endregion

        #region Server Resource Validation

        protected override bool ServerTryConsumeResource()
        {
            if (_controller == null)
                return false;

            if (NeedsEnergyPack && (_packManager == null || _packManager.CurrentId != PackId.Energy))
                return false;

            if (_controller.Energy < EnergyPerShot)
                return false;

            _controller.ServerSpendEnergy(EnergyPerShot);

            return true;
        }

        #endregion

        #region Local Predicted Audio Validation

        protected override bool ClientCanPlayPredictedFireSfx()
        {
            if (!base.ClientCanPlayPredictedFireSfx())
                return false;

            if (_controller == null)
                return false;

            if (NeedsEnergyPack && (_packManager == null || _packManager.CurrentId != PackId.Energy))
                return false;

            return _controller.Energy >= EnergyPerShot;
        }

        #endregion
    }
}
// EnergyProjectileWeapon.cs
using _Scripts.Player;
using _Scripts.Packs;

namespace _Scripts.Weapons
{
    public class EnergyProjectileWeapon : ProjectileWeapon
    {
        private AdvancedPredictedController _ctrl;
        PackManager                 _pm;
        
        float  EnergyPerShot       => def.energyPerShot;    // e.g. 4f
        bool   NeedsEnergyPack     => def.requiresEnergyPack;

        #region wiring from WeaponManager
        public override void CachePlayerRefs(WeaponManager wm, InputHandler ih)
        {
            base.CachePlayerRefs(wm, ih);
            _ctrl = wm.GetComponent<AdvancedPredictedController>(); // never null on player
            _pm   = wm.GetComponent<PackManager>();
        }
        #endregion

        /* ----------------- client-side gate ----------------- */
        protected override bool CanFire()
        {
            if (!base.CanFire())
                return false;
            
            if (_ctrl == null) return false;

            // Optional local feedback (cross-hair click, SFX, etc.)
            if (NeedsEnergyPack && _pm?.CurrentId != PackId.Energy)
                return false;

            return _ctrl.Energy >= EnergyPerShot;
        }

        /* ----------------- server-side consumption ----------- */
        protected override bool ServerCanConsume()
        {
            if (_ctrl == null) return false;

            if (NeedsEnergyPack && _pm?.CurrentId != PackId.Energy)
                return false;

            // Enough juice?
            if (_ctrl.Energy < EnergyPerShot)
                return false;

            // Burn it (authoritative)
            _ctrl.ServerSpendEnergy(EnergyPerShot);
            return true;
        }
        
        protected override bool ClientCanPlayPredictedFireSfx()
        {
            if (!base.ClientCanPlayPredictedFireSfx())
                return false;

            if (_ctrl == null)
                return false;

            if (NeedsEnergyPack && _pm?.CurrentId != PackId.Energy)
                return false;

            return _ctrl.Energy >= EnergyPerShot;
        }
    }
}
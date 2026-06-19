using FishNet.Object;

namespace _Scripts.Combat
{
    public readonly struct DamageResult
    {
        public readonly bool Applied;
        public readonly bool Killed;
        public readonly int RawDamage;
        public readonly int FinalDamage;
        public readonly int ShieldAbsorbed;
        public readonly int HealthBefore;
        public readonly int HealthAfter;
        public readonly NetworkObject Attacker;
        public readonly NetworkObject Victim;
        public readonly DamageType Type;
        public readonly byte WeaponId;
        public readonly DamageRejectReason RejectReason;

        public bool ShouldShowHitMarker =>
            (Applied || ShieldAbsorbed > 0) &&
            Attacker != null &&
            Victim != null &&
            Attacker != Victim;

        public DamageResult(
            bool applied,
            bool killed,
            int rawDamage,
            int finalDamage,
            int shieldAbsorbed,
            int healthBefore,
            int healthAfter,
            NetworkObject attacker,
            NetworkObject victim,
            DamageType type,
            byte weaponId,
            DamageRejectReason rejectReason)
        {
            Applied = applied;
            Killed = killed;
            RawDamage = rawDamage;
            FinalDamage = finalDamage;
            ShieldAbsorbed = shieldAbsorbed;
            HealthBefore = healthBefore;
            HealthAfter = healthAfter;
            Attacker = attacker;
            Victim = victim;
            Type = type;
            WeaponId = weaponId;
            RejectReason = rejectReason;
        }

        public static DamageResult Rejected(
            in DamageInfo info,
            NetworkObject victim,
            int currentHealth,
            DamageRejectReason reason)
        {
            return new DamageResult(
                applied: false,
                killed: false,
                rawDamage: info.Amount,
                finalDamage: 0,
                shieldAbsorbed: 0,
                healthBefore: currentHealth,
                healthAfter: currentHealth,
                attacker: info.Attacker,
                victim: victim,
                type: info.Type,
                weaponId: info.WeaponId,
                rejectReason: reason);
        }
    }
}
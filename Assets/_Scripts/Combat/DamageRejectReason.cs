namespace _Scripts.Combat
{
    public enum DamageRejectReason : byte
    {
        None = 0,
        Invalid = 1,
        TargetDead = 2,
        NonPositiveDamage = 3,
        FullyAbsorbed = 4,
        BlockedByGameRules = 5,
        Invulnerable = 6
    }
}
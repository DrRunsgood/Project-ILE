using FishNet.Object;
using UnityEngine;

namespace _Scripts.Combat
{
    public readonly struct DamageInfo
    {
        public readonly int Amount;
        public readonly NetworkObject Attacker;
        public readonly NetworkObject Source;
        public readonly DamageType Type;
        public readonly Vector3 Point;
        public readonly Vector3 Normal;
        public readonly Vector3 Impulse;
        public readonly byte WeaponId;

        public bool IsSelfDamage =>
            Attacker != null &&
            Source != null &&
            Attacker == Source;

        public DamageInfo(
            int amount,
            NetworkObject attacker,
            NetworkObject source,
            DamageType type,
            Vector3 point,
            Vector3 normal,
            Vector3 impulse,
            byte weaponId = 0)
        {
            Amount = amount;
            Attacker = attacker;
            Source = source;
            Type = type;
            Point = point;
            Normal = normal;
            Impulse = impulse;
            WeaponId = weaponId;
        }
    }
}
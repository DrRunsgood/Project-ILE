// _Scripts/Packs/PackDefinition.cs
using UnityEngine;
using FishNet.Object;

namespace _Scripts.Packs
{
    [CreateAssetMenu(menuName = "Pack")]
    public class PackDefinition : ScriptableObject
    {
        public PackId        id;
        public NetworkObject heldPrefab;     // MUST have a NetworkObject on the root
        public NetworkObject groundPrefab;   // idem
        public Sprite        hudIcon;

        /* balance */
        public float extraRegenPerSec;   // Energy-pack bonus
        public float shieldDrainPerSec;  // Shield-pack drain while active
    }
}
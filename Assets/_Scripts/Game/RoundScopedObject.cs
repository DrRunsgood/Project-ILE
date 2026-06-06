using FishNet.Object;
using UnityEngine;

namespace _Scripts.Game
{
    [DisallowMultipleComponent]
    public sealed class RoundScopedObject : MonoBehaviour
    {
        public enum CleanupScope : byte
        {
            Round,
            Match
        }

        [SerializeField] CleanupScope scope = CleanupScope.Round;

        public CleanupScope Scope => scope;
    }
}
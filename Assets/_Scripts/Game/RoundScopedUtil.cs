using FishNet.Object;

namespace _Scripts.Game
{
    public static class RoundScopedUtil
    {
        public static void MarkRoundScoped(NetworkObject nob)
        {
            if (nob == null)
                return;

            if (!nob.TryGetComponent(out RoundScopedObject _))
                nob.gameObject.AddComponent<RoundScopedObject>();
        }
    }
}
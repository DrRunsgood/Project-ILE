using FishNet;
using FishNet.Object;

namespace _Scripts.FNPool
{
    public static class PoolUtil
    {
        public static NetworkObject TakeFromPool(NetworkObject prefab)
        {
            if (prefab == null || InstanceFinder.NetworkManager == null)
                return null;

            NetworkObject nob = InstanceFinder.NetworkManager.GetPooledInstantiated(prefab, true);

            if (nob == null)
                return null;

            if (nob.TryGetComponent(out PoolReset poolReset))
                poolReset.ResetForReuse();
            else
            {
                /*
                 * Generic fallback for pooled prefabs without PoolReset.
                 * Callers still assign the final parent or world pose.
                 */
                nob.transform.SetParent(null, false);

                nob.transform.localPosition =
                    prefab.transform.localPosition;

                nob.transform.localRotation =
                    prefab.transform.localRotation;

                nob.transform.localScale =
                    prefab.transform.localScale;
            }

            return nob;
        }
    }
}
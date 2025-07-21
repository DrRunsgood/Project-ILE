// _Scripts/Net/PoolUtil.cs
using FishNet;
using FishNet.Object;
using UnityEngine;

/// <summary>
/// One-liner helper: fetch a pooled NetworkObject and clear its parent
/// so users don’t accidentally inherit transforms.
/// </summary>
public static class PoolUtil
{
    /// <param name="prefab">Prefab registered with Fish-Net’s object pool.</param>
    /// <returns>Pooled instance or null if pool exhausted.</returns>
    public static NetworkObject TakeFromPool(NetworkObject prefab)
    {
        NetworkObject nob = InstanceFinder.NetworkManager.GetPooledInstantiated(prefab, true);
        if (nob == null) return null;

        // Important: reset hierarchy so callers decide where to parent it.
        nob.transform.SetParent(null, false);
        return nob;
    }
}
using UnityEngine;
using System;
using FishNet;
using FishNet.Object;

namespace YourGameNamespace.Pooling
{
    public class ObjectPool : MonoBehaviour
    {
        [Serializable]
        public class Pool
        {
            public string tag;          // still handy if you want non-network pools later
            public NetworkObject prefab;
            public int  size = 16;
        }

        [Header("Pool Configuration")]
        [SerializeField] private Pool[] pools;

        void Awake()
        {
            if (InstanceFinder.NetworkManager == null)
            {
                Debug.LogError("ObjectPool: NetworkManager not found. Cannot cache objects.");
                return;
            }

            foreach (Pool p in pools)
            {
                // Tell FishNet to create (size) disabled clones of that prefab. No local Instantiate loop – FishNet owns the instances.
                InstanceFinder.NetworkManager.CacheObjects(p.prefab, p.size, true);
            }
        }

        /// <summary>Hand back a pre-warmed NetworkObject (already inactive).</summary>
        public NetworkObject GetPooledNetworkObject(NetworkObject prefab)
        {
            return InstanceFinder.NetworkManager.GetPooledInstantiated(prefab, true);
        }
    }
}
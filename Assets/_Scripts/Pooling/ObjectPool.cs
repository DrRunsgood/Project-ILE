using UnityEngine;
using System.Collections.Generic;
using FishNet.Object;
using FishNet;
using System;

namespace YourGameNamespace.Pooling
{
    public class ObjectPool : MonoBehaviour
    {
        [Serializable]
        public class Pool
        {
            public string tag;
            public NetworkObject prefab;
            public int size;
        }

        [Header("Pool Configuration")]
        public List<Pool> pools;
        private Dictionary<string, Queue<NetworkObject>> poolDictionary;

        private void Awake()
        {
            if (InstanceFinder.NetworkManager == null)
            {
                Debug.LogError("ObjectPool: NetworkManager not found. Cannot cache objects.");
                return;
            }

            poolDictionary = new Dictionary<string, Queue<NetworkObject>>();

            foreach (Pool pool in pools)
            {
                // Prewarm FishNet’s pool for this prefab
                InstanceFinder.NetworkManager.CacheObjects(pool.prefab, pool.size, true);

                Queue<NetworkObject> objectPool = new Queue<NetworkObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    NetworkObject obj = Instantiate(pool.prefab);
                    obj.gameObject.SetActive(false);
                    obj.transform.parent = transform; // keep hierarchy tidy
                    objectPool.Enqueue(obj);
                }

                poolDictionary.Add(pool.tag, objectPool);
            }
        }

        /// <summary>
        /// Retrieves an object from the local queue, re-queues it immediately, 
        /// and sets it active/parents it (if desired).
        /// </summary>
        public NetworkObject GetPooledObject(string tag, Transform parent = null)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"ObjectPool: Pool with tag [{tag}] doesn't exist.");
                return null;
            }

            NetworkObject obj = poolDictionary[tag].Dequeue();
            // Re-enqueue immediately for next usage
            poolDictionary[tag].Enqueue(obj);

            // Re-activate and set parent
            obj.gameObject.SetActive(true);
            obj.transform.parent = parent;

            return obj;
        }
    }
}

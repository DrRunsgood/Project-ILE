using FishNet;
using FishNet.Object;
using UnityEngine;

namespace _Scripts.Pickups.Spawning
{
    [DisallowMultipleComponent]
    public sealed class SpawnedPickupLink : MonoBehaviour
    {
        PickupSpawner _spawner;
        NetworkObject _boundObject;
        bool _bound;

        public void Bind(PickupSpawner spawner, NetworkObject nob)
        {
            _spawner = spawner;
            _boundObject = nob;
            _bound = true;
        }

        public void Clear()
        {
            _spawner = null;
            _boundObject = null;
            _bound = false;
        }

        void OnDisable()
        {
            if (!_bound)
                return;

            if (!InstanceFinder.IsServerStarted)
                return;

            if (_spawner != null && _boundObject != null)
                _spawner.NotifyPickupDespawned(_boundObject);

            Clear();
        }
    }
}
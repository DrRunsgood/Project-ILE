using FishNet;
using FishNet.Object;
using UnityEngine;

namespace _Scripts.FNPool
{
    public class NetworkPoolWarmup : NetworkBehaviour
    {
        [Header("One of each NetworkObject that can exist in-game")]
        [SerializeField] private NetworkObject[] prefabsToWarmUp;

        public override void OnStartServer()
        {
            base.OnStartServer();

            foreach (NetworkObject prefab in prefabsToWarmUp)
            {
                NetworkObject nob = InstanceFinder.NetworkManager.GetPooledInstantiated(prefab, true);

                InstanceFinder.NetworkManager.ServerManager.Spawn(nob);
                InstanceFinder.NetworkManager.ServerManager.Despawn(nob, DespawnType.Pool);
            }

            Debug.Log($"[NetworkPoolWarmup] Pre-warmed {prefabsToWarmUp.Length} network prefabs.");
        }
    }
}
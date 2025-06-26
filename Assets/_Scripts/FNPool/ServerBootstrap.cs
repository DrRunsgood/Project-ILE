// _Scripts/FNPool/ServerBootstrap.cs
using FishNet;
using FishNet.Object;
using UnityEngine;

public class ServerBootstrap : NetworkBehaviour
{
    [Header("One of each NetworkObject that can exist in-game")]
    [SerializeField] NetworkObject[] prefabsToWarmUp;

    public override void OnStartServer()
    {
        base.OnStartServer();

        foreach (var prefab in prefabsToWarmUp)
        {
            // 1) grab from FishNet pool (or instantiate if pool empty)
            var nob = InstanceFinder.NetworkManager.GetPooledInstantiated(prefab, true);

            // 2) spawn once so FishNet builds observer tables → JIT done
            InstanceFinder.NetworkManager.ServerManager.Spawn(nob);

            // 3) immediately despawn back into the pool
            InstanceFinder.NetworkManager.ServerManager.Despawn(nob,
                DespawnType.Pool);
        }

        Debug.Log($"Pre-warmed {prefabsToWarmUp.Length} network prefabs.");
    }
}
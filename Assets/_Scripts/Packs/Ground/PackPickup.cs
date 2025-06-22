// _Scripts/Packs/PackPickup.cs
using FishNet.Object;
using UnityEngine;

namespace _Scripts.Packs
{
    [RequireComponent(typeof(Collider))]
    public sealed class PackPickup : NetworkBehaviour
    {
        [SerializeField] PackDefinition definition;

        void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out PackManager pm)) return;
            
            if (pm.HasPack) return;

            if (IsServer)
            {
                pm.Server_GivePack(definition);
                ServerManager.Despawn(gameObject);
            }
            else if (other.TryGetComponent(out NetworkObject nObj))
            {
                Server_RequestPickup(nObj);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        void Server_RequestPickup(NetworkObject player)
        {
            if (player.TryGetComponent(out PackManager pm))
            {
                pm.Server_GivePack(definition);
                ServerManager.Despawn(gameObject);
            }
        }
    }
}
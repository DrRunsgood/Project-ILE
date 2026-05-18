// _Scripts/Items/Ground/ItemPickup.cs
using FishNet.Object;
using UnityEngine;
using _Scripts.Items;

[RequireComponent(typeof(Collider))]
public sealed class ItemPickup : NetworkBehaviour
{
    [SerializeField] ItemDefinition definition;
    Collider col;

    void Awake() => col = GetComponent<Collider>();

    void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out ItemManager im)) return;
        
        if (other.TryGetComponent(out PlayerHealth hp) && !hp.CanPickup)
            return;

        if (IsServer)
            GiveAndDespawn(im);
        else
            Server_RequestPickup(im.NetworkObject);     // owner → server
    }

    [ServerRpc(RequireOwnership = false)]
    void Server_RequestPickup(NetworkObject player)
    {
        if (player.TryGetComponent(out PlayerHealth hp) && !hp.CanPickup)
            return;

        GiveAndDespawn(player.GetComponent<ItemManager>());
    }

    [Server] void GiveAndDespawn(ItemManager im)
    {
        if (im == null) return;
        if (!im.Server_GiveItem(definition)) return;

        col.enabled = false;                           // no double-triggers
        ServerManager.Despawn(gameObject, DespawnType.Pool);
    }
}
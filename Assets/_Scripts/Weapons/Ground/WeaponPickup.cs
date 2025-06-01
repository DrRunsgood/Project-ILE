// Assets/_Scripts/Weapons/Ground/WeaponPickup.cs
using FishNet.Object;
using UnityEngine;
using _Scripts.Data;
using _Scripts.Player;

namespace _Scripts.Weapons
{
    [RequireComponent(typeof(Collider))]
    public sealed class WeaponPickup : NetworkBehaviour
    {
        [SerializeField] WeaponDefinition definition;

        void OnTriggerEnter(Collider other)
        {
            // 1) client side ⇒ just ask the server
            if (!IsServer && other.TryGetComponent(out NetworkObject nObj))
            {
                Server_RequestPickup(nObj);
                return;
            }

            // 2) server side ⇒ do the authoritative work
            if (!IsServer)           return;
            if (!other.TryGetComponent(out WeaponManager wm)) return;

            wm.Server_AddWeapon(definition);
            ServerManager.Despawn(gameObject);
        }

        /* runs only on the server – this is the bridge from client */
        [ServerRpc(RequireOwnership = false)]
        void Server_RequestPickup(NetworkObject playerObj)
        {
            Debug.Log("Running Server RPC request to pick up weapon!");
            if (!playerObj.TryGetComponent(out WeaponManager wm))
            {
                Debug.Log("failed check for weapon manager!");
                return;
            }
            wm.Server_AddWeapon(definition);
            Debug.Log("Adding Weapon!");
            ServerManager.Despawn(gameObject);
        }
    }
}
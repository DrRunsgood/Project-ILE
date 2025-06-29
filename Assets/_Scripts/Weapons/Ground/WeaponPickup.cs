//  _Scripts/Weapons/Ground/WeaponPickup.cs
using FishNet.Object;
using UnityEngine;
using _Scripts.Data;
using _Scripts.Packs;

namespace _Scripts.Weapons
{
    [RequireComponent(typeof(Collider))]
    public sealed class WeaponPickup : NetworkBehaviour
    {
        [SerializeField] float armDelay = 0.15f;
        double _armedAt;
        
        [SerializeField] WeaponDefinition definition;
        float _pickupEnableTime;            // set by the server

        /* called by server *immediately after* spawning the ground item */
        [Server] public void Arm(float delay)
            => _pickupEnableTime = (float)Time.timeAsDouble + delay;

        public override void OnStartServer()
        {
            base.OnStartServer();
            _armedAt = Time.timeAsDouble + armDelay;   // disarm for next 150 ms
        }
        void OnTriggerEnter(Collider other)
        {
            if (Time.timeAsDouble < _armedAt) return;
            /* 0) still in grace period? → ignore */
            if (Time.timeAsDouble < _pickupEnableTime)
                return;

            /* 1) client side → just ask the server */
            if (!IsServer && other.TryGetComponent(out NetworkObject nObj))
            {
                Server_RequestPickup(nObj);
                return;
            }

            /* 2) server side → do authoritative work */
            if (!IsServer) return;
            if (!other.TryGetComponent(out WeaponManager wm)) return;
            
            /* ───── energy-weapon gate ───── */
            if (definition.requiresEnergyPack)
            {
                // look for a PackManager on the same player root
                if (!other.TryGetComponent(out PackManager pm) || pm.CurrentId != PackId.Energy)
                    return;                     // reject pickup – just ignore the overlap
            }

            if (wm.Server_AddWeapon(definition))
                ServerManager.Despawn(gameObject);          // consumed
        }

        /* -------- bridge from client to server ----------------------- */
        [ServerRpc(RequireOwnership = false)]
        void Server_RequestPickup(NetworkObject playerObj)
        {
            if (!playerObj.TryGetComponent(out WeaponManager wm)) return;

            /* energy-weapon gate */
            if (definition.requiresEnergyPack)
            {
                if (!playerObj.TryGetComponent(out PackManager pm) || pm.CurrentId != PackId.Energy)
                    return;
            }

            if (wm.Server_AddWeapon(definition)) ServerManager.Despawn(gameObject);
        }

    }
}
using FishNet.Object;
using UnityEngine;
using _Scripts.Data;
using _Scripts.Pickups.Spawning;

namespace _Scripts.Weapons
{
    [RequireComponent(typeof(Collider))]
    public sealed class AmmoPickup : NetworkBehaviour, ISpawnInitialized
    {
        [Header("Pickup")]
        [SerializeField] float defaultArmDelay = 0.15f;

        [Header("Ammo")]
        [SerializeField] AmmoType ammoType = AmmoType.None;
        [SerializeField] int amount = 5;

        double _pickupEnableTime;

        [Server]
        public void ServerConfigure(AmmoType type, int ammoAmount)
        {
            ammoType = type;
            amount = Mathf.Max(0, ammoAmount);
        }

        [Server]
        public void Arm(float delay)
        {
            double enable = Time.timeAsDouble + delay;
            SetEnableTime(enable);
            RpcSetEnableTime(enable);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            if (_pickupEnableTime == 0)
            {
                double enable = Time.timeAsDouble + defaultArmDelay;
                SetEnableTime(enable);
                RpcSetEnableTime(enable);
            }
        }

        [ObserversRpc(BufferLast = true)]
        void RpcSetEnableTime(double enable)
        {
            if (IsServer)
                return;

            SetEnableTime(enable);
        }

        void SetEnableTime(double enable) => _pickupEnableTime = enable;

        void OnTriggerEnter(Collider other) => TryPickup(other);
        void OnTriggerStay(Collider other) => TryPickup(other);

        void TryPickup(Collider other)
        {
            if (Time.timeAsDouble < _pickupEnableTime)
                return;

            if (!IsServer && other.TryGetComponent(out NetworkObject nObj))
            {
                if (!nObj.IsOwner)
                    return;

                Server_RequestPickup(nObj);
                return;
            }

            if (!IsServer)
                return;

            if (!other.TryGetComponent(out WeaponManager wm))
                return;

            if (other.TryGetComponent(out PlayerHealth hp) && !hp.CanPickup)
                return;

            if (wm.Server_AddAmmo(ammoType, amount))
                ServerManager.Despawn(gameObject, DespawnType.Pool);
        }

        [ServerRpc(RequireOwnership = false)]
        void Server_RequestPickup(NetworkObject playerObj)
        {
            if (!playerObj.TryGetComponent(out WeaponManager wm))
                return;

            if (playerObj.TryGetComponent(out PlayerHealth hp) && !hp.CanPickup)
                return;

            if (wm.Server_AddAmmo(ammoType, amount))
                ServerManager.Despawn(gameObject, DespawnType.Pool);
        }
        
        [Server]
        public void ServerInitializeFromSpawner(PickupSpawnPayload payload)
        {
            ammoType = payload.AmmoType;
            amount = Mathf.Max(0, payload.AmmoAmount);
        }
    }
}
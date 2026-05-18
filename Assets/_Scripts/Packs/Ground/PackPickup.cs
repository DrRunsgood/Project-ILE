using FishNet.Object;
using UnityEngine;

namespace _Scripts.Packs
{
    [RequireComponent(typeof(Collider))]
    public sealed class PackPickup : NetworkBehaviour
    {
        [SerializeField] float defaultArmDelay = 0.15f;   // scene-placed fallback
        [SerializeField] PackDefinition definition;

        double _pickupEnableTime;

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
            if (IsServer) return;
            SetEnableTime(enable);
        }

        void SetEnableTime(double enable) => _pickupEnableTime = enable;

        void OnTriggerEnter(Collider other) => TryPickup(other);
        void OnTriggerStay(Collider other)  => TryPickup(other);

        void TryPickup(Collider other)
        {
            // Grace period still active
            if (Time.timeAsDouble < _pickupEnableTime)
                return;

            // Client path
            if (!IsServer && other.TryGetComponent(out NetworkObject nObj))
            {
                if (!nObj.IsOwner)
                    return;

                Server_RequestPickup(nObj);
                return;
            }

            // Server path
            if (!IsServer) return;
            if (!other.TryGetComponent(out PackManager pm)) return;
            if (pm.HasPack) return;
            
            if (other.TryGetComponent(out PlayerHealth hp) && !hp.CanPickup)
                return;

            if (pm.Server_GivePack(definition))
                ServerManager.Despawn(gameObject, DespawnType.Pool);
        }

        [ServerRpc(RequireOwnership = false)]
        void Server_RequestPickup(NetworkObject player)
        {
            if (!player.TryGetComponent(out PackManager pm)) return;
            if (pm.HasPack) return;
            
            if (player.TryGetComponent(out PlayerHealth hp) && !hp.CanPickup)
                return;

            if (pm.Server_GivePack(definition))
                ServerManager.Despawn(gameObject, DespawnType.Pool);
        }
    }
}
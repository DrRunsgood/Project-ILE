using FishNet.Object;
using UnityEngine;
using FishNet.Connection;

namespace _Scripts.Packs
{
    [RequireComponent(typeof(Collider))]
    public sealed class PackPickup : NetworkBehaviour
    {
        [SerializeField] float defaultArmDelay = 0.15f;   // scene-placed fallback
        [SerializeField] PackDefinition definition;

        Collider _collider;
        bool _pickupClaimed;
        double _pickupEnableTime;

        void Awake()
        {
            _collider = GetComponent<Collider>();
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

            ResetServerRuntime();
            Arm(defaultArmDelay);
        }
        
        public override void OnStartClient()
        {
            base.OnStartClient();

            /*
             * Do not overwrite authoritative host state after OnStartServer.
             */
            if (IsServer)
                return;

            _pickupClaimed = false;

            if (_collider == null)
                _collider = GetComponent<Collider>();

            if (_collider != null)
                _collider.enabled = true;

            /*
             * Wait for the server's buffered enable-time RPC rather than
             * retaining the previous pooled lifetime's timestamp.
             */
            _pickupEnableTime = double.PositiveInfinity;
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
            if (!IsServer)
                return;

            if (_pickupClaimed)
                return;

            if (!other.TryGetComponent(out PackManager packManager))
                return;

            if (packManager.HasPack)
                return;

            if (other.TryGetComponent(out PlayerHealth health) &&
                !health.CanPickup)
            {
                return;
            }

            Server_TryGiveAndDespawn(packManager);
        }

        [ServerRpc(RequireOwnership = false)]
        void Server_RequestPickup(NetworkObject player, NetworkConnection sender = null)
        {
            if (_pickupClaimed)
                return;

            if (Time.timeAsDouble < _pickupEnableTime)
                return;

            if (sender == null || !sender.IsValid || player == null || player.Owner != sender)
                return;

            if (!player.TryGetComponent(out PackManager packManager))
                return;

            if (packManager.HasPack)
                return;

            if (player.TryGetComponent(out PlayerHealth health) && !health.CanPickup)
                return;

            Server_TryGiveAndDespawn(packManager);
        }
        
        [Server]
        void Server_TryGiveAndDespawn(PackManager packManager)
        {
            if (_pickupClaimed || packManager == null || definition == null)
                return;

            if (!packManager.Server_GivePack(definition))
                return;

            _pickupClaimed = true;

            if (_collider != null)
                _collider.enabled = false;

            ServerManager.Despawn(NetworkObject, DespawnType.Pool);
        }
        
        [Server]
        void ResetServerRuntime()
        {
            _pickupClaimed = false;
            _pickupEnableTime = 0d;

            if (_collider == null)
                _collider = GetComponent<Collider>();

            if (_collider != null)
                _collider.enabled = true;
        }
    }
}
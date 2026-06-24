using _Scripts.Bootstrap;
using _Scripts.Player.Sessions;
using FishNet.Connection;
using FishNet.Object;

namespace _Scripts.Player
{
    public sealed class PlayerIdentityNameSubmitter : NetworkBehaviour
    {
        private PlayerIdentity _identity;
        private bool _submitted;

        private void Awake()
        {
            _identity = GetComponent<PlayerIdentity>();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (!IsOwner)
                return;

            SubmitName();
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            _submitted = false;
        }

        private void SubmitName()
        {
            if (_submitted)
                return;

            _submitted = true;

            string requestedName = ClientBootstrapSettings.RuntimeSessionDisplayName;

            if (string.IsNullOrWhiteSpace(requestedName))
                requestedName = ClientBootstrapSettings.DisplayName;

            Server_RequestDisplayName(requestedName);
        }

        [ServerRpc(RequireOwnership = true)]
        private void Server_RequestDisplayName(string requestedName, NetworkConnection conn = null)
        {
            if (PlayerSessionManager.Instance != null && conn != null)
            {
                PlayerSessionManager.Instance.ServerSetDisplayName(conn, requestedName);
                return;
            }

            // Fallback for test scenes without PlayerSessionManager.
            if (_identity == null)
                _identity = GetComponent<PlayerIdentity>();

            if (_identity != null)
                _identity.ServerSetDisplayName(requestedName);
        }
    }
}
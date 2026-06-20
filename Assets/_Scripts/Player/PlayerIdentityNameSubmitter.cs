using _Scripts.Bootstrap;
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

        private void SubmitName()
        {
            if (_submitted)
                return;

            _submitted = true;

            string requestedName = ClientBootstrapSettings.DisplayName;
            Server_RequestDisplayName(requestedName);
        }

        [ServerRpc]
        private void Server_RequestDisplayName(string requestedName)
        {
            if (_identity == null)
                _identity = GetComponent<PlayerIdentity>();

            if (_identity == null)
                return;

            _identity.ServerSetDisplayName(requestedName);
        }
    }
}
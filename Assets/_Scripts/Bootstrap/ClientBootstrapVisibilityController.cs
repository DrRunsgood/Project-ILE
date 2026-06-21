using _Scripts.Player;
using UnityEngine;

namespace _Scripts.Bootstrap
{
    public sealed class ClientBootstrapVisibilityController : MonoBehaviour
    {
        [SerializeField] private ClientServerManager clientServerManager;

        private void Awake()
        {
            if (clientServerManager == null)
                clientServerManager = FindFirstObjectByType<ClientServerManager>();

            LocalPlayerContext.OnLocalPlayerReady += HandleLocalPlayerReady;
        }

        private void OnDestroy()
        {
            LocalPlayerContext.OnLocalPlayerReady -= HandleLocalPlayerReady;
        }

        private void HandleLocalPlayerReady(AdvancedPredictedController controller)
        {
            if (clientServerManager != null)
                clientServerManager.SetClientBootstrapVisible(false);
            else
                gameObject.SetActive(false);
        }
    }
}
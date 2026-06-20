using _Scripts.Player;
using UnityEngine;

namespace _Scripts.Bootstrap
{
    public sealed class ClientBootstrapVisibilityController : MonoBehaviour
    {
        private void Awake()
        {
            LocalPlayerContext.OnLocalPlayerReady += HandleLocalPlayerReady;
        }

        private void OnDestroy()
        {
            LocalPlayerContext.OnLocalPlayerReady -= HandleLocalPlayerReady;
        }

        private void HandleLocalPlayerReady(AdvancedPredictedController controller)
        {
            gameObject.SetActive(false);
        }
    }
}
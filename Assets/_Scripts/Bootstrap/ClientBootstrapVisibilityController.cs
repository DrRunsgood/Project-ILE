using _Scripts.Player;
using UnityEngine;

namespace _Scripts.Bootstrap
{
    public sealed class ClientBootstrapVisibilityController : MonoBehaviour
    {
        [SerializeField] private GameStartupManager gameStartupManager;

        private void Awake()
        {
            if (gameStartupManager == null)
                gameStartupManager = FindAnyObjectByType<GameStartupManager>();

            LocalPlayerContext.OnLocalPlayerReady += HandleLocalPlayerReady;
        }

        private void OnDestroy()
        {
            LocalPlayerContext.OnLocalPlayerReady -= HandleLocalPlayerReady;
        }

        private void HandleLocalPlayerReady(AdvancedPredictedController controller)
        {
            if (gameStartupManager != null)
                gameStartupManager.SetClientBootstrapVisible(false);
            else
                gameObject.SetActive(false);
        }
    }
}
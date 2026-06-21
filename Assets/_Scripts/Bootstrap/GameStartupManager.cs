using _Scripts.Networking;
using UnityEngine;

namespace _Scripts.Bootstrap
{
    [DefaultExecutionOrder(-10000)]
    public sealed class GameStartupManager : MonoBehaviour
    {
        [Header("Mode")]
        [SerializeField] private bool runAsDedicatedServer;

        [Header("Server Defaults")]
        [SerializeField] private ushort defaultServerPort = 7770;
        [SerializeField] private string defaultGameplaySceneName = "Arena_TestMap_01";

        [Header("References")]
        [SerializeField] private NetworkSessionManager networkSessionManager;
        [SerializeField] private GameObject clientBootstrapRoot;
        [SerializeField] private Camera menuCamera;

        private void Awake()
        {
            if (networkSessionManager == null)
                networkSessionManager = FindFirstObjectByType<NetworkSessionManager>();

            if (runAsDedicatedServer)
            {
                Debug.Log("[GameStartupManager] Starting in dedicated server mode.");

                SetClientBootstrapVisible(false);

                if (networkSessionManager == null)
                {
                    Debug.LogError("[GameStartupManager] Missing NetworkSessionManager.");
                    return;
                }

                networkSessionManager.StartDedicatedServer(defaultServerPort, defaultGameplaySceneName);
                return;
            }

            Debug.Log("[GameStartupManager] Starting in client menu mode.");
            SetClientBootstrapVisible(true);
        }

        public void SetClientBootstrapVisible(bool visible)
        {
            if (clientBootstrapRoot != null)
            {
                if (IsDangerousRoot(clientBootstrapRoot))
                {
                    Debug.LogError(
                        $"[GameStartupManager] Refusing to toggle dangerous root '{clientBootstrapRoot.name}'. " +
                        "Assign the specific ClientBootstrap object instead.");
                }
                else
                {
                    clientBootstrapRoot.SetActive(visible);
                }
            }

            if (menuCamera != null)
            {
                menuCamera.enabled = visible;
                menuCamera.gameObject.SetActive(visible);

                AudioListener listener = menuCamera.GetComponent<AudioListener>();

                if (listener != null)
                    listener.enabled = visible;
            }
        }

        private static bool IsDangerousRoot(GameObject target)
        {
            if (target == null)
                return true;

            string targetName = target.name;

            return targetName == "_Scene" ||
                   targetName == "NetworkManager" ||
                   targetName == "AppRoot" ||
                   targetName == "MapMagic";
        }
    }
}
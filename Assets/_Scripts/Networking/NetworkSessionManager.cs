using System.Collections;
using FishNet;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Transporting.Tugboat;
using UnityEngine;
using _Scripts.Bootstrap;

namespace _Scripts.Networking
{
    public sealed class NetworkSessionManager : MonoBehaviour
    {
        [Header("FishNet")]
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private Tugboat tugboat;

        [Header("Server")]
        [SerializeField] private DedicatedServerGraphicsStripper graphicsStripper;

        private bool _clientStartRequested;
        private bool _serverStartRequested;
        private bool _serverSceneLoadRequested;

        private void Awake()
        {
            if (networkManager == null)
                networkManager = InstanceFinder.NetworkManager;

            if (networkManager != null && tugboat == null)
                tugboat = networkManager.GetComponent<Tugboat>();

            if (graphicsStripper == null)
                graphicsStripper = FindAnyObjectByType<DedicatedServerGraphicsStripper>();
        }

        public bool StartClient(string address, ushort port)
        {
            if (_clientStartRequested)
            {
                Debug.LogWarning("[NetworkSessionManager] Client start already requested.");
                return false;
            }

            if (networkManager == null)
            {
                Debug.LogError("[NetworkSessionManager] Missing NetworkManager.");
                return false;
            }

            ApplyClientTransportSettings(address, port);

            Debug.Log($"[NetworkSessionManager] Starting client connection to {address}:{port}.");

            bool started = networkManager.ClientManager.StartConnection();

            if (!started)
            {
                Debug.LogError("[NetworkSessionManager] Client start failed.");
                return false;
            }

            _clientStartRequested = true;
            return true;
        }

        public bool StartDedicatedServer(ushort port, string gameplaySceneName)
        {
            if (_serverStartRequested)
            {
                Debug.LogWarning("[NetworkSessionManager] Server start already requested.");
                return false;
            }

            if (networkManager == null)
            {
                Debug.LogError("[NetworkSessionManager] Missing NetworkManager.");
                return false;
            }

            ApplyServerTransportSettings(port);

            if (graphicsStripper != null)
                graphicsStripper.ActivateForServer();

            Debug.Log($"[NetworkSessionManager] Starting dedicated server on port {port}.");

            bool started = networkManager.ServerManager.StartConnection();

            if (!started)
            {
                Debug.LogError("[NetworkSessionManager] Server start failed.");
                return false;
            }

            _serverStartRequested = true;

            if (!string.IsNullOrWhiteSpace(gameplaySceneName))
                StartCoroutine(LoadServerMapNextFrame(gameplaySceneName));

            return true;
        }

        public void LoadServerMap(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                Debug.LogError("[NetworkSessionManager] Cannot load server map. Scene name is empty.");
                return;
            }

            if (_serverSceneLoadRequested)
            {
                Debug.LogWarning("[NetworkSessionManager] Server scene load already requested.");
                return;
            }

            _serverSceneLoadRequested = true;

            Debug.Log($"[NetworkSessionManager] Loading server gameplay scene: {sceneName}");

            SceneLoadData sceneLoadData = new SceneLoadData(sceneName);
            InstanceFinder.SceneManager.LoadGlobalScenes(sceneLoadData);
        }

        private IEnumerator LoadServerMapNextFrame(string sceneName)
        {
            yield return null;
            LoadServerMap(sceneName);
        }

        private void ApplyClientTransportSettings(string address, ushort port)
        {
            if (tugboat == null)
            {
                Debug.LogWarning("[NetworkSessionManager] Tugboat reference missing. Using transport defaults.");
                return;
            }

            tugboat.SetClientAddress(address);
            tugboat.SetPort(port);
        }

        private void ApplyServerTransportSettings(ushort port)
        {
            if (tugboat == null)
            {
                Debug.LogWarning("[NetworkSessionManager] Tugboat reference missing. Using transport defaults.");
                return;
            }

            tugboat.SetPort(port);
        }
    }
}
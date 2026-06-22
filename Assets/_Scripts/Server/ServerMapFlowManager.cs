using _Scripts.Networking;
using FishNet;
using UnityEngine;

namespace _Scripts.Server
{
    public enum PostMatchMapAction : byte
    {
        RestartCurrentMap = 0,
        LoadNextMap = 1,
        StayOnCurrentMap = 2
    }
    
    public sealed class ServerMapFlowManager : MonoBehaviour
    {
        public static ServerMapFlowManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private NetworkSessionManager networkSessionManager;
        [Header("Post Match Flow")]
        [SerializeField] private PostMatchMapAction postMatchAction = PostMatchMapAction.LoadNextMap;

        [Header("Map Rotation")]
        [SerializeField] private string[] mapRotation =
        {
            "Arena_TestMap_01",
            "Arena_TestMap_02"
        };

        [SerializeField] private bool loopRotation = true;

        private int _currentMapIndex = -1;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("[ServerMapFlowManager] Duplicate instance found. Destroying duplicate.");
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (networkSessionManager == null)
                networkSessionManager = FindAnyObjectByType<NetworkSessionManager>();
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        [ContextMenu("Server Load Next Map")]
        public void ServerLoadNextMapContext()
        {
            ServerLoadNextMap();
        }

        [ContextMenu("Server Restart Current Map")]
        public void ServerRestartCurrentMapContext()
        {
            ServerRestartCurrentMap();
        }

        public void ServerHandleMatchComplete()
        {
            if (!InstanceFinder.IsServerStarted)
            {
                Debug.LogWarning("[ServerMapFlowManager] Ignoring match complete; server is not started.");
                return;
            }

            switch (postMatchAction)
            {
                case PostMatchMapAction.RestartCurrentMap:
                    Debug.Log("[ServerMapFlowManager] Match complete. Restarting current map.");
                    ServerRestartCurrentMap();
                    break;

                case PostMatchMapAction.LoadNextMap:
                    Debug.Log("[ServerMapFlowManager] Match complete. Loading next map.");
                    ServerLoadNextMap();
                    break;

                case PostMatchMapAction.StayOnCurrentMap:
                    Debug.Log("[ServerMapFlowManager] Match complete. Staying on current map.");
                    break;
            }
        }

        public void ServerLoadNextMap()
        {
            if (!InstanceFinder.IsServerStarted)
            {
                Debug.LogWarning("[ServerMapFlowManager] Cannot load next map; server is not started.");
                return;
            }

            if (networkSessionManager == null)
                networkSessionManager = FindAnyObjectByType<NetworkSessionManager>();

            if (networkSessionManager == null)
            {
                Debug.LogError("[ServerMapFlowManager] Missing NetworkSessionManager.");
                return;
            }

            string nextMap = GetNextMapSceneName();

            if (string.IsNullOrWhiteSpace(nextMap))
            {
                Debug.LogError("[ServerMapFlowManager] No next map available.");
                return;
            }

            Debug.Log($"[ServerMapFlowManager] Loading next map: {nextMap}");

            networkSessionManager.ChangeServerMap(nextMap);
        }

        public void ServerRestartCurrentMap()
        {
            if (!InstanceFinder.IsServerStarted)
            {
                Debug.LogWarning("[ServerMapFlowManager] Cannot restart map; server is not started.");
                return;
            }

            if (networkSessionManager == null)
                networkSessionManager = FindAnyObjectByType<NetworkSessionManager>();

            if (networkSessionManager == null)
            {
                Debug.LogError("[ServerMapFlowManager] Missing NetworkSessionManager.");
                return;
            }

            string currentMap = networkSessionManager.CurrentGameplaySceneName;

            if (string.IsNullOrWhiteSpace(currentMap))
            {
                currentMap = GetCurrentOrFirstMapSceneName();

                if (string.IsNullOrWhiteSpace(currentMap))
                {
                    Debug.LogError("[ServerMapFlowManager] Cannot restart map; no current map known.");
                    return;
                }
            }

            Debug.Log($"[ServerMapFlowManager] Restarting current map: {currentMap}");

            networkSessionManager.ChangeServerMap(currentMap);
        }

        public void ServerChangeMap(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                Debug.LogError("[ServerMapFlowManager] Cannot change map; scene name is empty.");
                return;
            }

            if (!InstanceFinder.IsServerStarted)
            {
                Debug.LogWarning("[ServerMapFlowManager] Cannot change map; server is not started.");
                return;
            }

            if (networkSessionManager == null)
                networkSessionManager = FindAnyObjectByType<NetworkSessionManager>();

            if (networkSessionManager == null)
            {
                Debug.LogError("[ServerMapFlowManager] Missing NetworkSessionManager.");
                return;
            }

            UpdateCurrentMapIndex(sceneName);

            Debug.Log($"[ServerMapFlowManager] Changing map to: {sceneName}");

            networkSessionManager.ChangeServerMap(sceneName);
        }

        private string GetNextMapSceneName()
        {
            if (mapRotation == null || mapRotation.Length == 0)
                return null;

            string currentMap = networkSessionManager != null
                ? networkSessionManager.CurrentGameplaySceneName
                : null;

            if (!string.IsNullOrWhiteSpace(currentMap))
                UpdateCurrentMapIndex(currentMap);

            int nextIndex = _currentMapIndex + 1;

            if (nextIndex >= mapRotation.Length)
            {
                if (!loopRotation)
                    return null;

                nextIndex = 0;
            }

            _currentMapIndex = nextIndex;
            return mapRotation[_currentMapIndex];
        }

        private string GetCurrentOrFirstMapSceneName()
        {
            if (networkSessionManager != null &&
                !string.IsNullOrWhiteSpace(networkSessionManager.CurrentGameplaySceneName))
            {
                return networkSessionManager.CurrentGameplaySceneName;
            }

            if (mapRotation == null || mapRotation.Length == 0)
                return null;

            if (_currentMapIndex < 0 || _currentMapIndex >= mapRotation.Length)
                _currentMapIndex = 0;

            return mapRotation[_currentMapIndex];
        }

        private void UpdateCurrentMapIndex(string sceneName)
        {
            if (mapRotation == null)
                return;

            for (int i = 0; i < mapRotation.Length; i++)
            {
                if (mapRotation[i] == sceneName)
                {
                    _currentMapIndex = i;
                    return;
                }
            }
        }
    }
}
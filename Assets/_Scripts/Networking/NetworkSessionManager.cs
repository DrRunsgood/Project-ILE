using System.Collections;
using FishNet;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Transporting.Tugboat;
using UnityEngine;
using _Scripts.Bootstrap;
using _Scripts.Player;
using FishNet.Transporting;
using _Scripts.Game;
using _Scripts.Player.Sessions;
using _Scripts.Server;

namespace _Scripts.Networking
{
    public sealed class NetworkSessionManager : MonoBehaviour
    {
        [Header("FishNet")]
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private Tugboat tugboat;

        [Header("Server")]
        [SerializeField] private DedicatedServerGraphicsStripper graphicsStripper;
        
        [SerializeField] private string bootSceneName = "BootScene";
        
        [SerializeField] private float serverMapChangeUnloadDelay = 0.25f;
        [SerializeField] private float serverMapChangeLoadDelay = 0.1f;

        public string CurrentGameplaySceneName => _loadedGameplaySceneName;

        private bool _serverMapChangeInProgress;

        private string _loadedGameplaySceneName;
        private bool _isReturningToMenu;

        private bool _clientStartRequested;
        private bool _serverStartRequested;
        private bool _serverSceneLoadRequested;
        private bool _eventsSubscribed;
        
        public NetworkSessionState CurrentState { get; private set; } = NetworkSessionState.Offline;

        public event System.Action<NetworkSessionState, NetworkSessionState> OnStateChanged;

        
        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        private void SubscribeEvents()
        {
            if (_eventsSubscribed)
                return;

            ResolveReferences();

            if (networkManager != null)
            {
                networkManager.ClientManager.OnClientConnectionState += HandleClientConnectionState;
                networkManager.ServerManager.OnServerConnectionState += HandleServerConnectionState;
            }
            else
            {
                Debug.LogWarning("[NetworkSessionManager] NetworkManager missing during event subscription.");
            }

            LocalPlayerContext.OnLocalPlayerReady += HandleLocalPlayerReady;
            LocalPlayerContext.OnLocalPlayerCleared += HandleLocalPlayerCleared;

            _eventsSubscribed = true;
        }

        private void UnsubscribeEvents()
        {
            if (!_eventsSubscribed)
                return;

            if (networkManager != null)
            {
                networkManager.ClientManager.OnClientConnectionState -= HandleClientConnectionState;
                networkManager.ServerManager.OnServerConnectionState -= HandleServerConnectionState;
            }

            LocalPlayerContext.OnLocalPlayerReady -= HandleLocalPlayerReady;
            LocalPlayerContext.OnLocalPlayerCleared -= HandleLocalPlayerCleared;

            _eventsSubscribed = false;
        }
        
        
        private void Awake()
        {
            ResolveReferences();
        }

        public bool StartClient(string address, ushort port)
        {
            if (CurrentState != NetworkSessionState.ClientMenu &&
                CurrentState != NetworkSessionState.Offline &&
                CurrentState != NetworkSessionState.Failed)
            {
                Debug.LogWarning($"[NetworkSessionManager] Cannot start client while state is {CurrentState}.");
                return false;
            }

            if (_clientStartRequested)
            {
                Debug.LogWarning("[NetworkSessionManager] Client start already requested.");
                return false;
            }

            if (networkManager == null)
            {
                Debug.LogError("[NetworkSessionManager] Missing NetworkManager.");
                SetState(NetworkSessionState.Failed);
                return false;
            }

            SetState(NetworkSessionState.Connecting);

            ApplyClientTransportSettings(address, port);

            Debug.Log($"[NetworkSessionManager] Starting client connection to {address}:{port}.");

            bool started = networkManager.ClientManager.StartConnection();

            if (!started)
            {
                Debug.LogError("[NetworkSessionManager] Client start failed.");
                SetState(NetworkSessionState.Failed);
                return false;
            }

            _clientStartRequested = true;
            return true;
        }

        public bool StartDedicatedServer(ushort port, string gameplaySceneName)
        {
            if (CurrentState != NetworkSessionState.Offline)
            {
                Debug.LogWarning($"[NetworkSessionManager] Cannot start server while state is {CurrentState}.");
                return false;
            }

            if (_serverStartRequested)
            {
                Debug.LogWarning("[NetworkSessionManager] Server start already requested.");
                return false;
            }

            if (networkManager == null)
            {
                Debug.LogError("[NetworkSessionManager] Missing NetworkManager.");
                SetState(NetworkSessionState.Failed);
                return false;
            }

            SetState(NetworkSessionState.ServerStarting);

            ApplyServerTransportSettings(port);

            if (graphicsStripper != null)
                graphicsStripper.ActivateForServer();

            Debug.Log($"[NetworkSessionManager] Starting dedicated server on port {port}.");

            bool started = networkManager.ServerManager.StartConnection();

            if (!started)
            {
                Debug.LogError("[NetworkSessionManager] Server start failed.");
                SetState(NetworkSessionState.Failed);
                return false;
            }

            _serverStartRequested = true;
            SetState(NetworkSessionState.ServerRunning);

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
            _loadedGameplaySceneName = sceneName;

            bool serverWasRunning = CurrentState == NetworkSessionState.ServerRunning;

            if (serverWasRunning)
                SetState(NetworkSessionState.ServerLoadingGameplay);

            Debug.Log($"[NetworkSessionManager] Loading server gameplay scene: {sceneName}");

            SceneLoadData sceneLoadData = new SceneLoadData(sceneName);
            InstanceFinder.SceneManager.LoadGlobalScenes(sceneLoadData);

            _serverSceneLoadRequested = false;

            // Temporary until FishNet scene completion events are wired.
            if (serverWasRunning)
                SetState(NetworkSessionState.ServerRunning);
        }

        private IEnumerator LoadServerMapNextFrame(string sceneName)
        {
            yield return null;
            LoadServerMap(sceneName);
        }
        
        public void ChangeServerMap(string nextSceneName)
        {
            if (string.IsNullOrWhiteSpace(nextSceneName))
            {
                Debug.LogError("[NetworkSessionManager] Cannot change server map. Next scene name is empty.");
                return;
            }

            if (!InstanceFinder.IsServerStarted)
            {
                Debug.LogWarning("[NetworkSessionManager] Cannot change server map. Server is not started.");
                return;
            }

            if (_serverMapChangeInProgress)
            {
                Debug.LogWarning("[NetworkSessionManager] Server map change already in progress.");
                return;
            }

            StartCoroutine(ChangeServerMapRoutine(nextSceneName));
        }
        
        private IEnumerator ChangeServerMapRoutine(string nextSceneName)
        {
            _serverMapChangeInProgress = true;

            string previousSceneName = _loadedGameplaySceneName;

            Debug.Log($"[NetworkSessionManager] Server map change requested. Previous={previousSceneName}, Next={nextSceneName}");

            bool serverWasRunning =
                CurrentState == NetworkSessionState.ServerRunning ||
                CurrentState == NetworkSessionState.ServerLoadingGameplay;

            if (serverWasRunning)
                SetState(NetworkSessionState.ServerLoadingGameplay);

            if (!string.IsNullOrWhiteSpace(previousSceneName))
            {
                Debug.Log($"[NetworkSessionManager] Preparing old gameplay scene for unload: {previousSceneName}");
                
                bool clearTeams = true;

                if (ServerMapFlowManager.Instance != null)
                    clearTeams = ServerMapFlowManager.Instance.ShouldRebuildTeamsOnMapChange;

                PlayerSessionManager.Instance?.ServerPrepareForMapChange(clearTeams);
                SpawnManager.Instance?.DespawnAllPlayers();
                RoundResetManager.Instance?.PrepareForMapUnload();

                // Give FishNet a couple ticks/frames to process despawns before Unity scene unload destroys objects.
                yield return null;
                yield return null;

                Debug.Log($"[NetworkSessionManager] Unloading previous gameplay scene globally: {previousSceneName}");

                SceneUnloadData unloadData = new SceneUnloadData(previousSceneName);
                InstanceFinder.SceneManager.UnloadGlobalScenes(unloadData);

                yield return new WaitForSeconds(serverMapChangeUnloadDelay);
            }

            yield return new WaitForSeconds(serverMapChangeLoadDelay);

            Debug.Log($"[NetworkSessionManager] Loading next gameplay scene globally: {nextSceneName}");

            _loadedGameplaySceneName = nextSceneName;

            SceneLoadData loadData = new SceneLoadData(nextSceneName);
            InstanceFinder.SceneManager.LoadGlobalScenes(loadData);

            yield return null;

            _serverMapChangeInProgress = false;

            if (serverWasRunning)
                SetState(NetworkSessionState.ServerRunning);
        }
        
        public void RestartCurrentServerMap()
        {
            if (string.IsNullOrWhiteSpace(_loadedGameplaySceneName))
            {
                Debug.LogWarning("[NetworkSessionManager] Cannot restart current map; no gameplay scene is known.");
                return;
            }

            ChangeServerMap(_loadedGameplaySceneName);
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
        
        public void EnterClientMenu()
        {
            SetState(NetworkSessionState.ClientMenu);
        }
        
        public void DisconnectClient()
        {
            if (CurrentState == NetworkSessionState.Offline ||
                CurrentState == NetworkSessionState.ClientMenu ||
                CurrentState == NetworkSessionState.Disconnecting)
            {
                Debug.Log($"[NetworkSessionManager] Disconnect ignored while state is {CurrentState}.");
                return;
            }

            if (!_isReturningToMenu)
            {
                PlayerDisconnectCleanup cleanup = null;

                if (LocalPlayerContext.IsReady && LocalPlayerContext.Controller != null)
                    cleanup = LocalPlayerContext.Controller.GetComponent<PlayerDisconnectCleanup>();

                if (cleanup != null && cleanup.TryBeginGracefulDisconnect(ContinueDisconnectClient))
                {
                    _isReturningToMenu = true;
                    SetState(NetworkSessionState.Disconnecting);
                    return;
                }
            }

            ContinueDisconnectClient();
        }
        
        private void ContinueDisconnectClient()
        {
            if (networkManager == null)
                ResolveReferences();

            if (networkManager == null)
            {
                Debug.LogError("[NetworkSessionManager] Cannot disconnect. Missing NetworkManager.");
                _isReturningToMenu = false;
                SetState(NetworkSessionState.ClientMenu);
                return;
            }

            Debug.Log("[NetworkSessionManager] Disconnecting client.");

            _isReturningToMenu = true;
            SetState(NetworkSessionState.Disconnecting);

            networkManager.ClientManager.StopConnection();
        }
        
        // Event Handlers
        private void HandleClientConnectionState(ClientConnectionStateArgs args)
        {
            Debug.Log($"[NetworkSessionManager] Client connection state: {args.ConnectionState}");

            switch (args.ConnectionState)
            {
                case LocalConnectionState.Starting:
                    if (CurrentState == NetworkSessionState.ClientMenu ||
                        CurrentState == NetworkSessionState.Offline ||
                        CurrentState == NetworkSessionState.Failed)
                    {
                        SetState(NetworkSessionState.Connecting);
                    }
                    break;

                case LocalConnectionState.Started:
                    if (CurrentState == NetworkSessionState.Connecting)
                    {
                        SetState(NetworkSessionState.Connected);
                        SetState(NetworkSessionState.LoadingGameplay);
                    }
                    break;

                case LocalConnectionState.Stopping:
                    if (CurrentState != NetworkSessionState.Offline &&
                        CurrentState != NetworkSessionState.ClientMenu)
                    {
                        SetState(NetworkSessionState.Disconnecting);
                    }
                    break;

                case LocalConnectionState.Stopped:
                    _clientStartRequested = false;

                    if (_isReturningToMenu ||
                        CurrentState == NetworkSessionState.Disconnecting ||
                        CurrentState == NetworkSessionState.Connecting ||
                        CurrentState == NetworkSessionState.Connected ||
                        CurrentState == NetworkSessionState.LoadingGameplay ||
                        CurrentState == NetworkSessionState.InGame ||
                        CurrentState == NetworkSessionState.Failed)
                    {
                        StartCoroutine(ReturnClientToMenuAfterDisconnect());
                    }
                    break;
            }
        }
        
        private IEnumerator ReturnClientToMenuAfterDisconnect()
        {
            Debug.Log("[NetworkSessionManager] Returning client to menu.");

            yield return null;

            UnloadGameplayScenesLocal();

            yield return null;

            _isReturningToMenu = false;
            SetState(NetworkSessionState.ClientMenu);
        }
            
        private void UnloadGameplayScenesLocal()
        {
            // For now, unload every loaded scene except BootScene.
            // This is simple and correct for the current architecture:
            // BootScene persists; gameplay scenes are additive.
            for (int i = UnityEngine.SceneManagement.SceneManager.sceneCount - 1; i >= 0; i--)
            {
                UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

                if (!scene.isLoaded)
                    continue;

                if (scene.name == bootSceneName)
                    continue;

                Debug.Log($"[NetworkSessionManager] Unloading local gameplay scene: {scene.name}");
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);
            }

            _loadedGameplaySceneName = null;
        }

        private void HandleServerConnectionState(ServerConnectionStateArgs args)
        {
            Debug.Log($"[NetworkSessionManager] Server connection state: {args.ConnectionState}");

            switch (args.ConnectionState)
            {
                case LocalConnectionState.Starting:
                    SetState(NetworkSessionState.ServerStarting);
                    break;

                case LocalConnectionState.Started:
                    if (CurrentState == NetworkSessionState.ServerStarting)
                        SetState(NetworkSessionState.ServerRunning);
                    break;

                case LocalConnectionState.Stopping:
                    SetState(NetworkSessionState.ServerStopping);
                    break;

                case LocalConnectionState.Stopped:
                    _serverStartRequested = false;
                    _serverSceneLoadRequested = false;
                    SetState(NetworkSessionState.Offline);
                    break;
            }
        }
        
        private void HandleLocalPlayerReady(AdvancedPredictedController controller)
        {
            if (controller == null)
                return;

            if (CurrentState == NetworkSessionState.Connecting ||
                CurrentState == NetworkSessionState.Connected ||
                CurrentState == NetworkSessionState.LoadingGameplay)
            {
                SetState(NetworkSessionState.InGame);
            }
        }

        private void HandleLocalPlayerCleared()
        {
            if (CurrentState != NetworkSessionState.InGame)
                return;

            // If we intentionally started a return-to-menu disconnect, then the local
            // player being cleared is part of disconnect cleanup.
            if (_isReturningToMenu)
            {
                SetState(NetworkSessionState.Disconnecting);
                return;
            }

            // If the client connection is still alive, the local player was probably
            // cleared because the gameplay scene/map is changing.
            if (InstanceFinder.IsClientStarted)
            {
                Debug.Log("[NetworkSessionManager] Local player cleared while client is still connected. Treating as map/loading transition.");
                SetState(NetworkSessionState.LoadingGameplay);
                return;
            }

            // Fallback: if the client is no longer running, this is a disconnect.
            SetState(NetworkSessionState.Disconnecting);
        }
        
        // Helper
        private void SetState(NetworkSessionState newState)
        {
            if (CurrentState == newState)
                return;

            NetworkSessionState previousState = CurrentState;
            CurrentState = newState;

            Debug.Log($"[NetworkSessionManager] State changed: {previousState} -> {newState}");

            OnStateChanged?.Invoke(previousState, newState);
        }
        
        private void ResolveReferences()
        {
            if (networkManager == null)
                networkManager = InstanceFinder.NetworkManager;

            if (networkManager != null && tugboat == null)
                tugboat = networkManager.GetComponent<Tugboat>();

            if (graphicsStripper == null)
                graphicsStripper = FindAnyObjectByType<DedicatedServerGraphicsStripper>();
        }
    }
}
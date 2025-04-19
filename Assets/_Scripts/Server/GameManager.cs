using FishNet.Connection;
using FishNet.Managing.Server;
using FishNet.Transporting;
using FishNet.Object;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab; // Player prefab.
    private SpawnManager spawnManager;

    private void Awake()
    {
        // Locate the SpawnManager in the scene.
        spawnManager = Object.FindFirstObjectByType<SpawnManager>();
        if (spawnManager == null)
        {
            Debug.LogError("SpawnManager is missing. Please add it to the scene.");
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        // Subscribe to client connection state events.
        ServerManager.OnRemoteConnectionState += HandleRemoteConnectionState;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        // Unsubscribe to prevent memory leaks.
        ServerManager.OnRemoteConnectionState -= HandleRemoteConnectionState;
    }

    private void HandleRemoteConnectionState(NetworkConnection conn, RemoteConnectionStateArgs args)
    {
        if (args.ConnectionState == RemoteConnectionState.Started)
        {
            // Wait for the connection to load start scenes.
            conn.OnLoadedStartScenes += OnClientLoadedStartScenes;
        }
        else if (args.ConnectionState == RemoteConnectionState.Stopped)
        {
            // Despawn the player for the disconnected client.
            spawnManager.DespawnPlayer(conn);
        }
    }

    private void OnClientLoadedStartScenes(NetworkConnection conn, bool asServer)
    {
        // Spawn the player after the start scenes have loaded.
        if (asServer)
        { 
            spawnManager.SpawnPlayer(conn, playerPrefab);

            // Unsubscribe after spawning to avoid duplicate calls.
            conn.OnLoadedStartScenes -= OnClientLoadedStartScenes;
        }
    }
}

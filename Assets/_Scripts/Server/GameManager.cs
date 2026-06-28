using FishNet.Connection;
using FishNet.Managing.Server;
using FishNet.Transporting;
using FishNet.Object;
using UnityEngine;
using System.Collections;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab; // Player prefab.
    [SerializeField] private float spawnExistingConnectionsDelay = 0.35f;
    private SpawnManager spawnManager;

    private void Awake()
    {
        // Locate the SpawnManager in the scene.
        spawnManager = Object.FindAnyObjectByType<SpawnManager>();
        if (spawnManager == null)
        {
            Debug.LogError("SpawnManager is missing. Please add it to the scene.");
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        ServerManager.OnRemoteConnectionState += HandleRemoteConnectionState;

        StartCoroutine(SpawnExistingConnectionsAfterSceneLoad());
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
            //spawnManager.DespawnPlayer(conn);
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
    
    private IEnumerator SpawnExistingConnectionsAfterSceneLoad()
    {
        yield return new WaitForSeconds(spawnExistingConnectionsDelay);

        if (!IsServerStarted)
            yield break;

        if (spawnManager == null)
            spawnManager = Object.FindAnyObjectByType<SpawnManager>();

        if (spawnManager == null)
        {
            Debug.LogError("[GameManager] Cannot spawn existing players. SpawnManager is missing.");
            yield break;
        }

        foreach (NetworkConnection conn in ServerManager.Clients.Values)
        {
            if (conn == null || !conn.IsValid)
                continue;

            Debug.Log($"[GameManager] Spawning existing connection after map load: {conn.ClientId}");
            spawnManager.SpawnPlayer(conn, playerPrefab);
        }
    }
}

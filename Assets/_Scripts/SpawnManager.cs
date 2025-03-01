using FishNet.Connection;
using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : NetworkBehaviour
{
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>(); // List of spawn points.
    private Dictionary<NetworkConnection, NetworkObject> spawnedPlayers = new Dictionary<NetworkConnection, NetworkObject>();

    /// <summary>
    /// Adds a spawn point to the list.
    /// </summary>
    public void AddSpawnPoint(Transform spawnPoint)
    {
        if (!spawnPoints.Contains(spawnPoint))
            spawnPoints.Add(spawnPoint);
    }

    /// <summary>
    /// Removes a spawn point from the list.
    /// </summary>
    public void RemoveSpawnPoint(Transform spawnPoint)
    {
        if (spawnPoints.Contains(spawnPoint))
            spawnPoints.Remove(spawnPoint);
    }

    /// <summary>
    /// Spawns a player at a random spawn point.
    /// </summary>
    public void SpawnPlayer(NetworkConnection conn, GameObject playerPrefab)
    {
        if (!IsServerInitialized)
            return;

        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab is null. Please assign a player prefab.");
            return;
        }

        // Choose a random spawn point.
        Vector3 spawnPosition;
        Quaternion spawnRotation;

        if (spawnPoints.Count > 0)
        {
            Transform chosenSpawn = spawnPoints[Random.Range(0, spawnPoints.Count)];
            spawnPosition = chosenSpawn.position;
            spawnRotation = chosenSpawn.rotation;
        }
        else
        {
            // Default to origin if no spawn points are configured.
            Debug.LogWarning("No spawn points set! Spawning player at origin.");
            spawnPosition = Vector3.zero;
            spawnRotation = Quaternion.identity;
        }

        // Instantiate and spawn the player for the connection.
        NetworkObject playerObject = Instantiate(playerPrefab, spawnPosition, spawnRotation).GetComponent<NetworkObject>();
        Spawn(playerObject, conn);
        spawnedPlayers.Add(conn, playerObject);
    }

    /// <summary>
    /// Despawns a player associated with the given connection.
    /// </summary>
    public void DespawnPlayer(NetworkConnection conn)
    {
        if (!IsServerInitialized || !spawnedPlayers.ContainsKey(conn))
            return;

        // Despawn and remove the player object.
        NetworkObject playerObject = spawnedPlayers[conn];
        Despawn(playerObject);
        spawnedPlayers.Remove(conn);
    }

    /// <summary>
    /// Despawns all players.
    /// </summary>
    public void DespawnAllPlayers()
    {
        if (!IsServerInitialized)
            return;

        foreach (var playerObject in spawnedPlayers.Values)
        {
            Despawn(playerObject);
        }
        spawnedPlayers.Clear();
    }
}

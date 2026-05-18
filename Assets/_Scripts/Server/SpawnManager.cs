using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

public class SpawnManager : NetworkBehaviour
{
    public static SpawnManager Instance { get; private set; }

    readonly List<Transform> _spawnPoints = new();
    readonly Dictionary<NetworkConnection, NetworkObject> _spawnedPlayers = new();

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void AddSpawnPoint(Transform t)
    {
        if (t == null)
            return;

        if (!_spawnPoints.Contains(t))
            _spawnPoints.Add(t);
    }

    public void RemoveSpawnPoint(Transform t)
    {
        if (t == null)
            return;

        _spawnPoints.Remove(t);
    }

    public Transform GetRandomSpawn()
    {
        _spawnPoints.RemoveAll(sp => sp == null);

        if (_spawnPoints.Count == 0)
            return null;

        return _spawnPoints[Random.Range(0, _spawnPoints.Count)];
    }

    [Server]
    public bool TryMovePlayerToSpawn(NetworkObject player)
    {
        if (player == null)
            return false;

        Transform sp = GetRandomSpawn();
        if (sp == null)
            return false;

        player.transform.SetPositionAndRotation(sp.position, sp.rotation);
        return true;
    }

    public void SpawnPlayer(NetworkConnection conn, GameObject prefab)
    {
        if (!IsServerStarted || prefab == null)
            return;

        Transform sp = GetRandomSpawn();
        Vector3 pos = sp ? sp.position : Vector3.zero;
        Quaternion rot = sp ? sp.rotation : Quaternion.identity;

        NetworkObject nob = Instantiate(prefab, pos, rot).GetComponent<NetworkObject>();
        Spawn(nob, conn);

        _spawnedPlayers[conn] = nob;
    }

    public void DespawnPlayer(NetworkConnection conn)
    {
        if (!IsServerStarted)
            return;

        if (!_spawnedPlayers.TryGetValue(conn, out NetworkObject nob))
            return;

        Despawn(nob);
        _spawnedPlayers.Remove(conn);
    }

    public void DespawnAllPlayers()
    {
        if (!IsServerStarted)
            return;

        foreach (NetworkObject nob in _spawnedPlayers.Values)
        {
            if (nob != null)
                Despawn(nob);
        }

        _spawnedPlayers.Clear();
    }
    
    [Server]
    public void RespawnAllPlayers()
    {
        if (!IsServerStarted)
            return;

        foreach (NetworkObject nob in _spawnedPlayers.Values)
        {
            if (nob == null)
                continue;

            if (nob.TryGetComponent(out PlayerHealth hp))
                hp.RespawnNow();
            else
                TryMovePlayerToSpawn(nob);
        }
    }

    [Server]
    public void SetAllPlayersFrozen(bool frozen)
    {
        if (!IsServerStarted)
            return;

        foreach (NetworkObject nob in _spawnedPlayers.Values)
        {
            if (nob == null)
                continue;

            if (nob.TryGetComponent(out _Scripts.Player.AdvancedPredictedController ctrl))
                ctrl.IsFrozen = frozen;
        }
    }
}
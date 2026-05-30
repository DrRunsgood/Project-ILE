using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using _Scripts.Game;

public class SpawnManager : NetworkBehaviour
{
    public static SpawnManager Instance { get; private set; }

    readonly List<Transform> _spawnPoints = new();
    readonly Dictionary<NetworkConnection, NetworkObject> _spawnedPlayers = new();
    readonly Dictionary<NetworkConnection, GameObject> _pendingSpawnPlayers = new();

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
        if (!IsServerStarted || prefab == null || conn == null)
            return;

        if (GameModeManager.Instance != null &&
            !GameModeManager.Instance.ShouldSpawnPlayerImmediately())
        {
            _pendingSpawnPlayers[conn] = prefab;
            return;
        }

        SpawnPlayerNow(conn, prefab);
    }
    
    [Server]
    void SpawnPlayerNow(NetworkConnection conn, GameObject prefab)
    {
        if (!IsServerStarted || prefab == null || conn == null)
            return;

        Transform sp = GetRandomSpawn();
        Vector3 pos = sp ? sp.position : Vector3.zero;
        Quaternion rot = sp ? sp.rotation : Quaternion.identity;

        NetworkObject nob = Instantiate(prefab, pos, rot).GetComponent<NetworkObject>();
        Spawn(nob, conn);

        _spawnedPlayers[conn] = nob;

        ApplyCurrentGameModeSpawnState(nob);
    }
    
    [Server]
    void ApplyCurrentGameModeSpawnState(NetworkObject nob)
    {
        if (nob == null || GameModeManager.Instance == null)
            return;

        if (!nob.TryGetComponent(out _Scripts.Player.AdvancedPredictedController ctrl))
            return;

        bool shouldFreeze =
            GameModeManager.Instance.Mode == GameModeType.Arena &&
            GameModeManager.Instance.State == MatchState.PreRound;

        ctrl.IsFrozen = shouldFreeze;
    }
    
    [Server]
    public void SpawnPendingPlayers()
    {
        if (!IsServerStarted || _pendingSpawnPlayers.Count == 0)
            return;

        foreach (var kvp in _pendingSpawnPlayers)
        {
            NetworkConnection conn = kvp.Key;
            GameObject prefab = kvp.Value;

            if (conn == null || !conn.IsValid || prefab == null)
                continue;

            if (_spawnedPlayers.ContainsKey(conn))
                continue;

            SpawnPlayerNow(conn, prefab);
        }

        _pendingSpawnPlayers.Clear();
    }

    public void DespawnPlayer(NetworkConnection conn)
    {
        if (!IsServerStarted)
            return;

        _pendingSpawnPlayers.Remove(conn);

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
    
    [Server]
    public void ResetAllPlayerInventoriesForRound()
    {
        if (!IsServerStarted)
            return;

        foreach (NetworkObject nob in _spawnedPlayers.Values)
        {
            if (nob == null)
                continue;

            if (nob.TryGetComponent(out _Scripts.Weapons.WeaponManager wm))
                wm.Server_ClearWeaponsForRoundReset();

            if (nob.TryGetComponent(out _Scripts.Packs.PackManager pm))
                pm.Server_ClearPackForRoundReset();

            if (nob.TryGetComponent(out _Scripts.Items.ItemManager im))
                im.Server_ClearItemsForRoundReset();
        }
    }
}
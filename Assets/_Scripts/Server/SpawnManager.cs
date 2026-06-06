using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using _Scripts.Game;
using _Scripts.Player;
using _Scripts.Game.Teams;
using FishNet.Transporting;

public class SpawnManager : NetworkBehaviour
{
    public static SpawnManager Instance { get; private set; }

    readonly List<PlayerSpawnPoint> _spawnPoints = new();
    readonly HashSet<PlayerSpawnPoint> _usedSpawnPointsThisWave = new();
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

    public override void OnStartServer()
    {
        base.OnStartServer();

        ServerManager.OnRemoteConnectionState += HandleRemoteConnectionState;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        ServerManager.OnRemoteConnectionState -= HandleRemoteConnectionState;
    }

    [Server]
    void HandleRemoteConnectionState(NetworkConnection conn, RemoteConnectionStateArgs args)
    {
        if (args.ConnectionState != RemoteConnectionState.Stopped)
            return;

        DespawnPlayer(conn);
    }
    
    public void AddSpawnPoint(PlayerSpawnPoint sp)
    {
        if (sp == null)
            return;

        if (!_spawnPoints.Contains(sp))
            _spawnPoints.Add(sp);
    }

    public void RemoveSpawnPoint(PlayerSpawnPoint sp)
    {
        if (sp == null)
            return;

        _spawnPoints.Remove(sp);
        _usedSpawnPointsThisWave.Remove(sp);
    }

    PlayerSpawnPoint GetSpawnPoint(SpawnTeam team)
    {
        _spawnPoints.RemoveAll(sp => sp == null);

        GameModeType currentMode =
            GameModeManager.Instance != null
                ? GameModeManager.Instance.Mode
                : GameModeType.Deathmatch;

        List<PlayerSpawnPoint> valid = new();

        foreach (PlayerSpawnPoint sp in _spawnPoints)
        {
            if (sp == null)
                continue;

            bool modeMatch =
                sp.AllowAnyMode ||
                sp.Mode == currentMode;

            if (!modeMatch)
                continue;

            bool teamMatch =
                sp.AllowAnyTeam ||
                sp.Team == team;

            if (!teamMatch)
                continue;

            if (_usedSpawnPointsThisWave.Contains(sp))
                continue;

            valid.Add(sp);
        }

        if (valid.Count == 0)
        {
            foreach (PlayerSpawnPoint sp in _spawnPoints)
            {
                if (sp == null)
                    continue;

                bool modeMatch =
                    sp.AllowAnyMode ||
                    sp.Mode == currentMode;

                if (!modeMatch)
                    continue;

                bool teamMatch =
                    sp.AllowAnyTeam ||
                    sp.Team == team;

                if (!teamMatch)
                    continue;

                valid.Add(sp);
            }
        }

        if (valid.Count == 0)
            return null;

        PlayerSpawnPoint chosen = valid[Random.Range(0, valid.Count)];

        _usedSpawnPointsThisWave.Add(chosen);

        return chosen;
    }
    
    SpawnTeam ResolveSpawnTeam(NetworkObject nob)
    {
        if (nob == null)
            return SpawnTeam.Any;

        if (!nob.TryGetComponent(out PlayerIdentity identity))
            return SpawnTeam.Any;

        return identity.Team switch
        {
            TeamId.TeamA => SpawnTeam.TeamA,
            TeamId.TeamB => SpawnTeam.TeamB,
            _ => SpawnTeam.Any
        };
    }

    [Server]
    public bool TryMovePlayerToSpawn(NetworkObject player)
    {
        if (player == null)
            return false;

        SpawnTeam team = ResolveSpawnTeam(player);

        PlayerSpawnPoint sp = GetSpawnPoint(team);

        if (sp == null)
            return false;

        if (player.TryGetComponent(out AdvancedPredictedController ctrl))
        {
            ctrl.HardResetMovement(
                sp.transform.position,
                sp.transform.rotation);
        }
        else
        {
            player.transform.SetPositionAndRotation(
                sp.transform.position,
                sp.transform.rotation);
        }

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

        if (_spawnedPlayers.TryGetValue(conn, out NetworkObject existing) && existing != null)
        {
            _spawnedPlayers.Remove(conn);

            if (existing.IsSpawned)
                Despawn(existing);
        }

        SpawnTeam spawnTeam = SpawnTeam.Any;

        if (GameModeManager.Instance != null &&
            (GameModeManager.Instance.Mode == GameModeType.Arena ||
             GameModeManager.Instance.Mode == GameModeType.CTF) &&
            TeamManager.Instance != null)
        {
            spawnTeam = ToSpawnTeam(TeamManager.Instance.GetBalancedTeamForNewPlayer());
        }

        PlayerSpawnPoint sp = GetSpawnPoint(spawnTeam);

        Vector3 spawnPos = sp != null ? sp.transform.position : Vector3.zero;
        Quaternion spawnRot = sp != null ? sp.transform.rotation : Quaternion.identity;

        NetworkObject nob = Instantiate(prefab, spawnPos, spawnRot)
            .GetComponent<NetworkObject>();

        Spawn(nob, conn);

        _spawnedPlayers[conn] = nob;

        // Safety: force controller/prediction state to match the spawn transform too.
        if (nob.TryGetComponent(out AdvancedPredictedController ctrl))
        {
            ctrl.HardResetMovement(spawnPos, spawnRot);
        }
        else
        {
            nob.transform.SetPositionAndRotation(spawnPos, spawnRot);
        }

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

        BeginSpawnWave();

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

    [Server]
    public void DespawnPlayer(NetworkConnection conn)
    {
        if (!IsServerStarted || conn == null)
            return;

        _pendingSpawnPlayers.Remove(conn);

        if (!_spawnedPlayers.TryGetValue(conn, out NetworkObject nob))
            return;

        _spawnedPlayers.Remove(conn);

        if (nob != null && nob.IsSpawned)
            Despawn(nob);
    }

    [Server]
    public void DespawnAllPlayers()
    {
        if (!IsServerStarted)
            return;

        foreach (NetworkObject nob in _spawnedPlayers.Values)
        {
            if (nob != null && nob.IsSpawned)
                Despawn(nob);
        }

        _spawnedPlayers.Clear();
    }
    
    [Server]
    public void RespawnAllPlayers()
    {
        if (!IsServerStarted)
            return;
        
        BeginSpawnWave();

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
    void BeginSpawnWave()
    {
        _usedSpawnPointsThisWave.Clear();
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
    
    SpawnTeam ToSpawnTeam(TeamId team)
    {
        return team switch
        {
            TeamId.TeamA => SpawnTeam.TeamA,
            TeamId.TeamB => SpawnTeam.TeamB,
            _ => SpawnTeam.Any
        };
    }
}
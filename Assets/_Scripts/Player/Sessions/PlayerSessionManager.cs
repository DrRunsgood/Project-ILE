using System;
using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;
using _Scripts.Game.Teams;

namespace _Scripts.Player.Sessions
{
    [DisallowMultipleComponent]
    public sealed class PlayerSessionManager : MonoBehaviour
    {
        public static PlayerSessionManager Instance { get; private set; }

        public event Action<PlayerSession> OnSessionConnected;
        public event Action<PlayerSession> OnSessionDisconnected;
        public event Action<PlayerSession> OnSessionBodyLinked;
        public event Action<PlayerSession> OnSessionBodyUnlinked;
        public event Action<PlayerSession> OnSessionIdentityChanged;
        public event Action<PlayerSession> OnSessionLifeStateChanged;
        public event Action<PlayerSession> OnSessionEligibilityChanged;

        private readonly Dictionary<NetworkConnection, PlayerSession> _sessionsByConnection = new();
        private readonly List<PlayerSession> _allSessions = new();

        private uint _nextSessionId = 1;
        private bool _subscribed;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("[PlayerSessionManager] Duplicate instance found. Destroying duplicate.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnEnable()
        {
            TrySubscribe();
        }

        private void Start()
        {
            TrySubscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void OnDestroy()
        {
            Unsubscribe();

            if (Instance == this)
                Instance = null;
        }

        private void TrySubscribe()
        {
            if (_subscribed)
                return;

            if (InstanceFinder.ServerManager == null)
                return;

            InstanceFinder.ServerManager.OnRemoteConnectionState += HandleRemoteConnectionState;
            _subscribed = true;
        }

        private void Unsubscribe()
        {
            if (!_subscribed)
                return;

            if (InstanceFinder.ServerManager != null)
                InstanceFinder.ServerManager.OnRemoteConnectionState -= HandleRemoteConnectionState;

            _subscribed = false;
        }

        private void HandleRemoteConnectionState(NetworkConnection conn, RemoteConnectionStateArgs args)
        {
            if (conn == null)
                return;

            switch (args.ConnectionState)
            {
                case RemoteConnectionState.Started:
                    ServerGetOrCreateSession(conn);
                    break;

                case RemoteConnectionState.Stopped:
                    ServerHandleDisconnected(conn);
                    break;
            }
        }

        public PlayerSession ServerGetOrCreateSession(NetworkConnection conn)
        {
            if (conn == null)
                return null;

            if (_sessionsByConnection.TryGetValue(conn, out PlayerSession existing))
                return existing;

            PlayerSession session = new PlayerSession(_nextSessionId++, conn)
            {
                DisplayName = $"Player {conn.ClientId}"
            };

            _sessionsByConnection[conn] = session;
            _allSessions.Add(session);

            Debug.Log($"[PlayerSessionManager] Session connected. SessionId={session.SessionId}, ClientId={session.ClientId}, Name={session.DisplayName}");

            OnSessionConnected?.Invoke(session);
            return session;
        }

        public bool TryGetSession(NetworkConnection conn, out PlayerSession session)
        {
            if (conn == null)
            {
                session = null;
                return false;
            }

            return _sessionsByConnection.TryGetValue(conn, out session);
        }

        public IReadOnlyList<PlayerSession> GetAllSessions()
        {
            return _allSessions;
        }

        public List<PlayerSession> GetConnectedSessionsNonAlloc(List<PlayerSession> results)
        {
            results.Clear();

            foreach (PlayerSession session in _allSessions)
            {
                if (session != null && session.IsConnected)
                    results.Add(session);
            }

            return results;
        }

        public void ServerSetDisplayName(NetworkConnection conn, string displayName)
        {
            PlayerSession session = ServerGetOrCreateSession(conn);

            if (session == null)
                return;

            session.DisplayName = SanitizeDisplayName(displayName);

            ApplySessionToSpawnedIdentity(session);

            Debug.Log($"[PlayerSessionManager] Display name set. SessionId={session.SessionId}, ClientId={session.ClientId}, Name={session.DisplayName}");

            OnSessionIdentityChanged?.Invoke(session);
        }

        public TeamId ServerEnsureTeam(NetworkConnection conn, TeamId fallbackTeam)
        {
            PlayerSession session = ServerGetOrCreateSession(conn);

            if (session == null)
                return fallbackTeam;

            if (session.Team == TeamId.None)
                ServerSetTeam(session, fallbackTeam);

            return session.Team;
        }

        public void ServerSetTeam(NetworkConnection conn, TeamId team)
        {
            PlayerSession session = ServerGetOrCreateSession(conn);

            if (session == null)
                return;

            ServerSetTeam(session, team);
        }

        public void ServerSetTeam(PlayerSession session, TeamId team)
        {
            if (session == null)
                return;

            session.Team = team;

            ApplySessionToSpawnedIdentity(session);

            Debug.Log($"[PlayerSessionManager] Team set. SessionId={session.SessionId}, ClientId={session.ClientId}, Team={session.Team}");

            OnSessionIdentityChanged?.Invoke(session);
        }

        public void ServerLinkSpawnedPlayer(NetworkConnection conn, NetworkObject spawnedObject)
        {
            PlayerSession session = ServerGetOrCreateSession(conn);

            if (session == null || spawnedObject == null)
                return;

            session.SetSpawnedBody(spawnedObject);
            
            session.IsAlive = true;
            session.IsEligibleThisRound = true;

            ApplySessionToSpawnedIdentity(session);

            Debug.Log($"[PlayerSessionManager] Linked spawned player. SessionId={session.SessionId}, ClientId={session.ClientId}, Object={spawnedObject.name}");

            OnSessionBodyLinked?.Invoke(session);
        }

        public void ServerUnlinkSpawnedPlayer(NetworkConnection conn, NetworkObject spawnedObject = null)
        {
            if (!TryGetSession(conn, out PlayerSession session))
                return;

            if (spawnedObject != null && session.SpawnedObject != spawnedObject)
                return;

            session.ClearSpawnedBody();

            Debug.Log($"[PlayerSessionManager] Unlinked spawned player. SessionId={session.SessionId}, ClientId={session.ClientId}");

            OnSessionBodyUnlinked?.Invoke(session);
        }

        public void ServerPrepareForMapChange(bool clearTeams = true)
        {
            foreach (PlayerSession session in _allSessions)
            {
                if (session == null)
                    continue;

                session.ClearSpawnedBody();

                session.IsAlive = false;
                session.IsEligibleThisRound = false;

                if (clearTeams)
                    session.Team = TeamId.None;
            }

            Debug.Log($"[PlayerSessionManager] Prepared sessions for map change. ClearTeams={clearTeams}");
        }
        
        public void ServerClearMapTeamAssignments()
        {
            foreach (PlayerSession session in _allSessions)
            {
                if (session == null)
                    continue;

                session.Team = TeamId.None;

                ApplySessionToSpawnedIdentity(session);

                Debug.Log($"[PlayerSessionManager] Cleared map team assignment. SessionId={session.SessionId}, ClientId={session.ClientId}");
            }
        }

        private void ServerHandleDisconnected(NetworkConnection conn)
        {
            if (!_sessionsByConnection.TryGetValue(conn, out PlayerSession session))
                return;

            _sessionsByConnection.Remove(conn);

            session.MarkDisconnected();

            Debug.Log($"[PlayerSessionManager] Session disconnected. SessionId={session.SessionId}, ClientId={session.ClientId}, Name={session.DisplayName}");

            OnSessionLifeStateChanged?.Invoke(session);
            OnSessionEligibilityChanged?.Invoke(session);
            OnSessionDisconnected?.Invoke(session);
        }
        
        public void ServerMarkSpawnedAlive(NetworkConnection conn)
        {
            if (!TryGetSession(conn, out PlayerSession session))
                return;

            session.IsAlive = true;
            session.IsEligibleThisRound = true;

            Debug.Log($"[PlayerSessionManager] Session alive/eligible. SessionId={session.SessionId}, ClientId={session.ClientId}, Team={session.Team}");

            OnSessionLifeStateChanged?.Invoke(session);
            OnSessionEligibilityChanged?.Invoke(session);
        }

        public void ServerMarkDead(NetworkConnection conn)
        {
            if (!TryGetSession(conn, out PlayerSession session))
                return;


            if (!session.IsAlive && !session.IsEligibleThisRound)
                return;

            session.IsAlive = false;
            session.IsEligibleThisRound = false;

            Debug.Log($"[PlayerSessionManager] Session dead/ineligible. SessionId={session.SessionId}, ClientId={session.ClientId}, Team={session.Team}");

            OnSessionLifeStateChanged?.Invoke(session);
            OnSessionEligibilityChanged?.Invoke(session);
        }

        public void ServerMarkDead(PlayerIdentity identity)
        {
            if (identity == null)
                return;

            foreach (PlayerSession session in _allSessions)
            {
                if (session == null)
                    continue;

                if (session.SpawnedIdentity != identity)
                    continue;

                session.IsAlive = false;
                session.IsEligibleThisRound = false;

                Debug.Log($"[PlayerSessionManager] Session dead/ineligible by identity. SessionId={session.SessionId}, ClientId={session.ClientId}, Team={session.Team}");

                OnSessionLifeStateChanged?.Invoke(session);
                OnSessionEligibilityChanged?.Invoke(session);
                return;
            }
        }

        public int CountConnectedEligiblePlayersOnTeam(TeamId team)
        {
            int count = 0;

            foreach (PlayerSession session in _allSessions)
            {
                if (session == null)
                    continue;

                if (!session.IsConnected)
                    continue;

                if (!session.IsEligibleThisRound)
                    continue;

                if (session.Team != team)
                    continue;

                count++;
            }

            return count;
        }

        public int CountConnectedPlayersOnTeam(TeamId team)
        {
            int count = 0;

            foreach (PlayerSession session in _allSessions)
            {
                if (session == null)
                    continue;

                if (!session.IsConnected)
                    continue;

                if (session.Team != team)
                    continue;

                count++;
            }

            return count;
        }

        private void ApplySessionToSpawnedIdentity(PlayerSession session)
        {
            if (session == null || session.SpawnedIdentity == null)
                return;

            session.SpawnedIdentity.ServerApplySessionData(
                session.DisplayName,
                session.Team
            );
        }

        private static string SanitizeDisplayName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "Player";

            value = value.Trim();

            if (value.Length > 24)
                value = value.Substring(0, 24);

            return value;
        }
    }
}
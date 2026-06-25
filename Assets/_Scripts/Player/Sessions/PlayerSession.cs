using FishNet.Connection;
using FishNet.Object;
using _Scripts.Game.Teams;

namespace _Scripts.Player.Sessions
{
    public sealed class PlayerSession
    {
        public uint SessionId { get; }
        public NetworkConnection Connection { get; private set; }
        public int ClientId { get; private set; }

        public string DisplayName { get; set; } = "Player";

        // Future account/backend identity.
        public string AccountId { get; set; } = string.Empty;

        // Current map/match team. This can be rebuilt on map change later.
        public TeamId Team { get; set; } = TeamId.None;

        public bool IsConnected { get; private set; }
        public bool IsAlive { get; set; }
        public bool IsEligibleThisRound { get; set; }

        public NetworkObject SpawnedObject { get; private set; }
        public PlayerIdentity SpawnedIdentity { get; private set; }

        public bool IsSpawned => SpawnedObject != null && SpawnedObject.IsSpawned;

        public PlayerSession(uint sessionId, NetworkConnection connection)
        {
            SessionId = sessionId;
            SetConnection(connection);
            IsConnected = true;
        }

        public void SetConnection(NetworkConnection connection)
        {
            Connection = connection;
            ClientId = connection != null ? connection.ClientId : -1;
            IsConnected = connection != null;
        }

        public void MarkDisconnected()
        {
            IsConnected = false;
            IsAlive = false;
            IsEligibleThisRound = false;
            Connection = null;
            ClearSpawnedBody();
        }

        public void SetSpawnedBody(NetworkObject spawnedObject)
        {
            SpawnedObject = spawnedObject;
            SpawnedIdentity = spawnedObject != null
                ? spawnedObject.GetComponent<PlayerIdentity>()
                : null;

            IsAlive = spawnedObject != null;
            IsEligibleThisRound = spawnedObject != null;
        }

        public void ClearSpawnedBody()
        {
            SpawnedObject = null;
            SpawnedIdentity = null;
            IsAlive = false;
            IsEligibleThisRound = false;
        }
    }
}
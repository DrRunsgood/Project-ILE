using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using _Scripts.Game.Teams;

namespace _Scripts.Player
{
    [DisallowMultipleComponent]
    public sealed class PlayerIdentity : NetworkBehaviour
    {
        readonly SyncVar<TeamId> _team = new(TeamId.None);
        
        readonly SyncVar<string> _displayName = new("Player");

        public string DisplayName => _displayName.Value;

        public TeamId Team => _team.Value;
        
        void Awake()
        {
            _team.OnChange += OnTeamChanged;
            _displayName.OnChange += OnDisplayNameChanged;
        }

        void OnDestroy()
        {
            _team.OnChange -= OnTeamChanged;
            _displayName.OnChange -= OnDisplayNameChanged;
        }

        void OnTeamChanged(TeamId prev, TeamId next, bool asServer)
        {
            Debug.Log($"[PlayerIdentity] {name} team changed: {prev} -> {next}");
        }
        
        void OnDisplayNameChanged(string prev, string next, bool asServer)
        {
            if (!string.IsNullOrWhiteSpace(next))
                gameObject.name = next;
        }

        [Server]
        public void ServerSetTeam(TeamId team)
        {
            _team.Value = team;
        }
        
        [Server]
        public void ServerSetDisplayName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                value = "Player";

            value = value.Trim();

            if (value.Length > 24)
                value = value.Substring(0, 24);

            _displayName.Value = value;
            gameObject.name = value;
        }
        
        [Server]
        public void ServerApplySessionData(string displayName, TeamId team)
        {
            ServerSetDisplayName(displayName);
            ServerSetTeam(team);
        }
    }
}
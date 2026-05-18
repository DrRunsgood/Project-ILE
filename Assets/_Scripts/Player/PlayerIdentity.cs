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

        public override void OnStartServer()
        {
            base.OnStartServer();

            if (TeamManager.Instance != null)
                TeamManager.Instance.RegisterPlayer(this);
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            if (TeamManager.Instance != null)
                TeamManager.Instance.UnregisterPlayer(this);
        }
        
        void Awake()
        {
            _team.OnChange += OnTeamChanged;
        }

        void OnDestroy()
        {
            _team.OnChange -= OnTeamChanged;
        }

        void OnTeamChanged(TeamId prev, TeamId next, bool asServer)
        {
            Debug.Log($"[PlayerIdentity] {name} team changed: {prev} -> {next}");
        }

        [Server]
        public void ServerSetTeam(TeamId team)
        {
            _team.Value = team;
        }
        
        [Server]
        public void ServerSetDisplayName(string value)
        {
            _displayName.Value = value;
            gameObject.name = value;
        }
    }
}
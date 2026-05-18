using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using _Scripts.Player;

namespace _Scripts.Game.Teams
{
    [DisallowMultipleComponent]
    public sealed class TeamManager : NetworkBehaviour
    {
        public static TeamManager Instance { get; private set; }

        readonly List<PlayerIdentity> _players = new();
        
        void Awake()
        {
            Instance = this;
        }

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        [Server]
        public void RegisterPlayer(PlayerIdentity player)
        {
            if (player == null || _players.Contains(player))
                return;

            _players.Add(player);

            if (player.Team == TeamId.None)
                player.ServerSetTeam(GetBalancedTeam());
            
            player.ServerSetDisplayName($"Player {_players.Count}");
            Debug.Log($"[TeamManager] Assigned {player.name} to {player.Team}");
        }

        [Server]
        public void UnregisterPlayer(PlayerIdentity player)
        {
            if (player == null)
                return;

            _players.Remove(player);
        }

        [Server]
        TeamId GetBalancedTeam()
        {
            int a = CountTeam(TeamId.TeamA);
            int b = CountTeam(TeamId.TeamB);

            return a <= b ? TeamId.TeamA : TeamId.TeamB;
        }

        public int CountTeam(TeamId team)
        {
            int count = 0;

            foreach (PlayerIdentity p in _players)
            {
                if (p != null && p.Team == team)
                    count++;
            }

            return count;
        }

        public IReadOnlyList<PlayerIdentity> Players => _players;

        [Server]
        public void AdminSetTeam(PlayerIdentity player, TeamId team)
        {
            if (player == null)
                return;

            player.ServerSetTeam(team);
        }
        
        public int CountAlive(TeamId team)
        {
            int count = 0;

            foreach (PlayerIdentity p in _players)
            {
                if (p == null || p.Team != team)
                    continue;

                if (p.TryGetComponent(out PlayerHealth hp) && !hp.IsDead)
                    count++;
            }

            return count;
        }

        public int CountPlayers(TeamId team)
        {
            int count = 0;

            foreach (PlayerIdentity p in _players)
            {
                if (p != null && p.Team == team)
                    count++;
            }

            return count;
        }
    }
}
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace _Scripts.Player
{
    [DisallowMultipleComponent]
    public sealed class PlayerStats : NetworkBehaviour
    {
        readonly SyncVar<int> _kills = new(0);
        readonly SyncVar<int> _deaths = new(0);

        public int Kills => _kills.Value;
        public int Deaths => _deaths.Value;

        void Awake()
        {
            _kills.OnChange += OnKillsChanged;
            _deaths.OnChange += OnDeathsChanged;
        }
        
        void OnDestroy()
        {
            _kills.OnChange -= OnKillsChanged;
            _deaths.OnChange -= OnDeathsChanged;
        }

        void OnKillsChanged(int prev, int next, bool asServer)
        {
            Debug.Log($"[PlayerStats] {name} kills: {prev} -> {next}");
        }

        void OnDeathsChanged(int prev, int next, bool asServer)
        {
            Debug.Log($"[PlayerStats] {name} deaths: {prev} -> {next}");
        }
        
        [Server]
        public void AddKill()
        {
            _kills.Value++;
        }

        [Server]
        public void AddDeath()
        {
            _deaths.Value++;
        }

        [Server]
        public void ResetStats()
        {
            _kills.Value = 0;
            _deaths.Value = 0;
        }
    }
}
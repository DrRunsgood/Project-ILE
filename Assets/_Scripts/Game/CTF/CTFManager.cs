using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using _Scripts.Game.Teams;

namespace _Scripts.Game.CTF
{
    [DisallowMultipleComponent]
    public sealed class CTFManager : NetworkBehaviour
    {
        public static CTFManager Instance { get; private set; }

        readonly SyncVar<int> _teamAScore = new(0);
        readonly SyncVar<int> _teamBScore = new(0);

        [Header("Flag Stands")]
        [SerializeField] FlagStand teamAStand;
        [SerializeField] FlagStand teamBStand;

        public int TeamAScore => _teamAScore.Value;
        public int TeamBScore => _teamBScore.Value;

        void Awake()
        {
            Instance = this;
        }

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public FlagStand GetStand(TeamId team)
        {
            return team switch
            {
                TeamId.TeamA => teamAStand,
                TeamId.TeamB => teamBStand,
                _ => null
            };
        }

        [Server]
        public bool Server_TryCapture(FlagCarrier carrier, TeamId scoringTeam)
        {
            if (carrier == null || !carrier.HasFlag)
                return false;

            FlagObject carriedFlag = carrier.CarriedFlag;
            if (carriedFlag == null)
                return false;

            if (carriedFlag.Team == scoringTeam)
                return false;

            FlagStand ownStand = GetStand(scoringTeam);
            if (ownStand == null || !ownStand.IsFlagHome)
                return false;

            AddScore(scoringTeam);
            
            carriedFlag.Server_ReturnHome();
            
            GameModeManager.Instance?.NotifyCTFCapture(scoringTeam, _teamAScore.Value, _teamBScore.Value);

            Debug.Log($"[CTFManager] {scoringTeam} captured {carriedFlag.Team} flag. Score A:{_teamAScore.Value} B:{_teamBScore.Value}");

            return true;
        }

        [Server]
        void AddScore(TeamId team)
        {
            switch (team)
            {
                case TeamId.TeamA:
                    _teamAScore.Value++;
                    break;

                case TeamId.TeamB:
                    _teamBScore.Value++;
                    break;
            }
        }
        
        [Server]
        public void Server_ResetForMatchStart()
        {
            _teamAScore.Value = 0;
            _teamBScore.Value = 0;

            if (teamAStand != null && teamAStand.CurrentFlag != null)
                teamAStand.CurrentFlag.Server_ReturnHome();

            if (teamBStand != null && teamBStand.CurrentFlag != null)
                teamBStand.CurrentFlag.Server_ReturnHome();

            Debug.Log("[CTFManager] Reset CTF state for match start.");
        }
    }
}
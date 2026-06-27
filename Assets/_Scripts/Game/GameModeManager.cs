using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using _Scripts.Game.Teams;
using _Scripts.Game.CTF;
using _Scripts.Combat;
using _Scripts.Player;
using _Scripts.Server;
using _Scripts.Player.Sessions;
using FishNet;

namespace _Scripts.Game
{
    public enum MatchState : byte
    {
        Waiting,
        Warmup,
        PreRound,
        Live,
        PostRound,
        PostMatch
    }

    public enum GameModeType : byte
    {
        Deathmatch,
        Arena,
        CTF
    }

    [DisallowMultipleComponent]
    public sealed class GameModeManager : NetworkBehaviour
    {
        public static GameModeManager Instance { get; private set; }

        #region Inspector Fields

        [Header("Mode")]
        [SerializeField] GameModeType startingMode = GameModeType.Deathmatch;
        
        [Header("Rules")]
        [SerializeField] bool allowTeamDamage = true;

        [Header("Timing")]
        [SerializeField] float warmupSeconds = 5f;
        [SerializeField] float matchSeconds = 600f;
        [SerializeField] float postMatchSeconds = 10f;

        [Header("Arena")]
        [SerializeField] int roundsToWin = 3;
        [SerializeField] float postRoundSeconds = 5f;

        [Header("Deathmatch")]
        [SerializeField] int killLimit = 25;
        
        [Header("CTF")]
        [SerializeField] int captureLimit = 5;
        [SerializeField] float ctfRespawnDelay = 5f;
        
        #endregion

        #region SyncVars / Public State

        readonly SyncVar<GameModeType> _mode = new(GameModeType.Deathmatch);
        readonly SyncVar<MatchState> _state = new(MatchState.Waiting);
        readonly SyncVar<uint> _stateEndTick = new(0);

        readonly SyncVar<int> _currentRound = new(0);
        readonly SyncVar<int> _teamAScore = new(0);
        readonly SyncVar<int> _teamBScore = new(0);

        public GameModeType Mode => _mode.Value;
        public MatchState State => _state.Value;
        public uint StateEndTick => _stateEndTick.Value;

        public int CurrentRound => _currentRound.Value;
        public int TeamAScore => _teamAScore.Value;
        public int TeamBScore => _teamBScore.Value;

        public bool IsLive => _state.Value == MatchState.Live;
        
        public bool AllowTeamDamage => allowTeamDamage;

        #endregion
        
        bool _postMatchFlowRequested;

        #region Unity / FishNet Lifecycle

        void Awake()
        {
            Instance = this;
            _state.OnChange += OnStateChanged;
        }

        void OnDestroy()
        {
            _state.OnChange -= OnStateChanged;

            if (Instance == this)
                Instance = null;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            
            if (PlayerSessionManager.Instance != null)
            {
                PlayerSessionManager.Instance.OnSessionEligibilityChanged += HandleSessionEligibilityChanged;
                PlayerSessionManager.Instance.OnSessionConnected += HandleSessionConnected;
                PlayerSessionManager.Instance.OnSessionBodyLinked += HandleSessionBodyLinked;
            }

            _mode.Value = startingMode;

            if (CanStartWarmup())
            {
                if (_mode.Value == GameModeType.Arena)
                    StartArenaBetweenRoundsOrWaiting();
                else
                    StartWarmup();
            }
            else
            {
                StartWaiting();
            }
        }
        
        public override void OnStopServer()
        {
            base.OnStopServer();

            if (PlayerSessionManager.Instance != null)
            {
                PlayerSessionManager.Instance.OnSessionEligibilityChanged -= HandleSessionEligibilityChanged;
                PlayerSessionManager.Instance.OnSessionConnected -= HandleSessionConnected;
                PlayerSessionManager.Instance.OnSessionBodyLinked -= HandleSessionBodyLinked;
            }
        }

        void Update()
        {
            if (!IsServerStarted)
                return;

            switch (_mode.Value)
            {
                case GameModeType.Deathmatch:
                    TickDeathmatch();
                    break;

                case GameModeType.Arena:
                    TickArena();
                    break;

                case GameModeType.CTF:
                    TickCTF();
                    break;
            }
        }

        #endregion

        #region State Entry Methods

        [Server]
        public void StartWaiting()
        {
            SetState(MatchState.Waiting, 1f);
        }

        [Server]
        public void StartWarmup()
        {
            if (_mode.Value == GameModeType.Arena)
            {
                StartArenaBetweenRoundsOrWaiting();
                return;
            }

            SpawnManager.Instance?.RespawnAllPlayers();
            SetState(MatchState.Warmup, warmupSeconds);
        }

        [Server]
        public void StartMatch()
        {
            SetState(MatchState.Live, matchSeconds);

            // Later:
            // - reset scores
            // - respawn players
            // - reset pickups
            // - announce match start
        }

        [Server]
        public void EndMatch()
        {
            SetState(MatchState.PostMatch, postMatchSeconds);

            // Later:
            // - freeze scoring
            // - show winner
            // - prepare map/mode rotation
        }
        
        [Server]
        void CompletePostMatchFlow()
        {
            if (_postMatchFlowRequested)
                return;

            _postMatchFlowRequested = true;

            if (ServerMapFlowManager.Instance != null)
            {
                Debug.Log("[GameModeManager] PostMatch complete. Handing off to ServerMapFlowManager.");
                ServerMapFlowManager.Instance.ServerHandleMatchComplete();
                return;
            }

            Debug.LogWarning("[GameModeManager] ServerMapFlowManager missing. Falling back to local restart behavior.");

            switch (_mode.Value)
            {
                case GameModeType.Arena:
                    ResetArenaMatch();
                    StartWaiting();
                    break;

                case GameModeType.CTF:
                    ResetCTFMatch();
                    StartWarmup();
                    break;

                case GameModeType.Deathmatch:
                default:
                    StartWarmup();
                    break;
            }
        }

        [Server]
        void SetState(MatchState next, float duration)
        {
            _state.Value = next;

            if (next != MatchState.PostMatch)
                _postMatchFlowRequested = false;

            uint durationTicks = TimeManager.TimeToTicks(Mathf.Max(0f, duration));
            _stateEndTick.Value = TimeManager.Tick + durationTicks;
        }

        void OnStateChanged(MatchState prev, MatchState next, bool asServer)
        {
            Debug.Log($"[GameModeManager] Match state changed: {prev} -> {next}");
        }

        #endregion

        #region Tick / Mode Flow

        [Server]
        void TickDeathmatch()
        {
            if (_state.Value == MatchState.Waiting)
            {
                if (CanStartWarmup())
                    StartWarmup();
                return;
            }

            if (!HasStateTimerExpired())
                return;

            switch (_state.Value)
            {
                case MatchState.Warmup:
                    if (CanStartMatch())
                    {
                        ResetAllPlayerStats();
                        StartMatch();
                    }
                    else
                    {
                        StartWaiting();
                    }
                    break;

                case MatchState.Live:
                    EndMatch();
                    break;

                case MatchState.PostMatch:
                    CompletePostMatchFlow();
                    break;
            }
        }

        [Server]
        void TickArena()
        {
            if (_state.Value == MatchState.Waiting)
            {
                if (CanStartWarmup())
                    StartArenaBetweenRoundsOrWaiting();

                return;
            }

            if (!HasStateTimerExpired())
                return;

            switch (_state.Value)
            {
                case MatchState.PreRound:
                    if (CanStartMatch())
                        StartArenaRound();
                    else
                        StartWaiting();
                    break;

                case MatchState.Live:
                    ResolveArenaRoundByTimer();
                    break;

                case MatchState.PostRound:
                    if (HasArenaMatchWinner())
                        EndMatch();
                    else
                        StartArenaBetweenRoundsOrWaiting();
                    break;

                case MatchState.PostMatch:
                    CompletePostMatchFlow();
                    //ResetArenaMatch();
                    //StartWaiting();
                    break;
            }
        }

        [Server]
        void TickCTF()
        {
            if (_state.Value == MatchState.Waiting)
            {
                if (CanStartWarmup())
                    StartWarmup();

                return;
            }

            if (!HasStateTimerExpired())
                return;

            switch (_state.Value)
            {
                case MatchState.Warmup:
                    if (CanStartMatch())
                    {
                        ResetAllPlayerStats();
                        StartCTFMatch();
                    }
                    else
                    {
                        StartWaiting();
                    }
                    break;

                case MatchState.Live:
                    EndMatch();
                    break;

                case MatchState.PostMatch:
                    CompletePostMatchFlow();
                    //ResetCTFMatch();
                    //StartWarmup();
                    break;
            }
        }

        #endregion

        #region Arena Logic

        [Server]
        void StartArenaPreRound()
        {
            if (_currentRound.Value == 0)
                ResetAllPlayerStats();

            RoundResetManager.Instance?.ResetForArenaRound();

            SpawnManager.Instance?.SpawnPendingPlayers();
            SpawnManager.Instance?.RespawnAllPlayers();
            SpawnManager.Instance?.SetAllPlayersFrozen(true);

            SetState(MatchState.PreRound, warmupSeconds);
        }

        [Server]
        void StartArenaRound()
        {
            _currentRound.Value++;
            SpawnManager.Instance?.SetAllPlayersFrozen(false);

            SetState(MatchState.Live, matchSeconds);

            // Later:
            // - enable scoring
            // - announce round start
        }

        [Server]
        void EndArenaRound(TeamId winner)
        {
            switch (winner)
            {
                case TeamId.TeamA:
                    _teamAScore.Value++;
                    break;

                case TeamId.TeamB:
                    _teamBScore.Value++;
                    break;

                case TeamId.None:
                default:
                    // Tie/no point.
                    break;
            }

            SpawnManager.Instance?.SetAllPlayersFrozen(true);
            SetState(MatchState.PostRound, postRoundSeconds);
        }

        [Server]
        bool HasArenaMatchWinner()
        {
            return _teamAScore.Value >= roundsToWin ||
                   _teamBScore.Value >= roundsToWin;
        }

        [Server]
        void ResetArenaMatch()
        {
            _currentRound.Value = 0;
            _teamAScore.Value = 0;
            _teamBScore.Value = 0;
        }

        [Server]
        void ResolveArenaRoundByTimer()
        {
            if (TeamManager.Instance == null)
            {
                EndArenaRound(TeamId.None);
                return;
            }

            int aliveA = TeamManager.Instance.CountAlive(TeamId.TeamA);
            int aliveB = TeamManager.Instance.CountAlive(TeamId.TeamB);

            if (aliveA > aliveB)
                EndArenaRound(TeamId.TeamA);
            else if (aliveB > aliveA)
                EndArenaRound(TeamId.TeamB);
            else
                EndArenaRound(TeamId.None);
        }

        [Server]
        void CheckArenaEliminationWin()
        {
            EvaluateArenaRoundEndFromSessions();
        }
        
        [Server]
        private bool HasEnoughArenaPlayersForRound()
        {
            if (PlayerSessionManager.Instance == null)
                return true;

            int teamAConnected = PlayerSessionManager.Instance.CountConnectedPlayersOnTeam(TeamId.TeamA);
            int teamBConnected = PlayerSessionManager.Instance.CountConnectedPlayersOnTeam(TeamId.TeamB);

            return teamAConnected > 0 && teamBConnected > 0;
        }
        
        [Server]
        private void StartArenaBetweenRoundsOrWaiting()
        {
            if (!HasEnoughArenaPlayersForRound())
            {
                Debug.Log("[GameModeManager] Arena waiting for players before next round.");
                StartWaiting();
                return;
            }

            StartArenaPreRound();
        }
        
        [Server]
        private void EvaluateArenaRoundEndFromSessions()
        {
            if (_mode.Value != GameModeType.Arena)
                return;

            if (_state.Value != MatchState.Live)
                return;

            if (PlayerSessionManager.Instance == null)
                return;

            int aliveA = PlayerSessionManager.Instance.CountConnectedEligiblePlayersOnTeam(TeamId.TeamA);
            int aliveB = PlayerSessionManager.Instance.CountConnectedEligiblePlayersOnTeam(TeamId.TeamB);

            if (aliveA > 0 && aliveB > 0)
                return;

            if (aliveA <= 0 && aliveB <= 0)
            {
                Debug.Log("[GameModeManager] Arena round ended with no eligible players alive. Tie/no-score.");
                EndArenaRound(TeamId.None);
                return;
            }

            TeamId winner = aliveA > 0 ? TeamId.TeamA : TeamId.TeamB;

            Debug.Log($"[GameModeManager] Arena round winner by session elimination: {winner}");
            EndArenaRound(winner);
        }

        #endregion

        #region Deathmatch Logic

        [Server]
        void HandleDeathmatchDeath(PlayerHealth victim, NetworkObject killer)
        {
            if (_state.Value != MatchState.Live)
                return;

            RecordKillDeath(victim, killer);

            if (killer != null &&
                killer != victim.NetworkObject &&
                killer.TryGetComponent(out Player.PlayerStats killerStats) &&
                killerStats.Kills >= killLimit)
            {
                EndMatch();
            }
        }
        
        [Server]
        void HandleDeathmatchDeath(PlayerHealth victim, DamageResult result)
        {
            if (_state.Value != MatchState.Live)
                return;

            RecordKillDeath(victim, result);

            NetworkObject killer = result.Attacker;

            if (killer != null &&
                killer != victim.NetworkObject &&
                killer.TryGetComponent(out Player.PlayerStats killerStats) &&
                killerStats.Kills >= killLimit)
            {
                EndMatch();
            }
        }

        #endregion
        
        #region CTF Logic
        
        [Server]
        void StartCTFMatch()
        {
            ResetAllPlayerStats();

            CTFManager.Instance?.Server_ResetForMatchStart();

            RoundResetManager.Instance?.ResetForArenaRound(); // Optional but useful if this clears world objects/inventories.
            SpawnManager.Instance?.SpawnPendingPlayers();
            SpawnManager.Instance?.RespawnAllPlayers();
            SpawnManager.Instance?.SetAllPlayersFrozen(false);

            SetState(MatchState.Live, matchSeconds);
        }
        
        [Server]
        void ResetCTFMatch()
        {
            ResetAllPlayerStats();

            RoundResetManager.Instance?.ResetForCTFMatchStart();

            SpawnManager.Instance?.SpawnPendingPlayers();
            SpawnManager.Instance?.RespawnAllPlayers();
            SpawnManager.Instance?.SetAllPlayersFrozen(false);
        }
        
        [Server]
        public void NotifyCTFCapture(TeamId scoringTeam, int teamAScore, int teamBScore)
        {
            if (_mode.Value != GameModeType.CTF)
                return;

            if (_state.Value != MatchState.Live)
                return;

            if (teamAScore >= captureLimit || teamBScore >= captureLimit)
                EndMatch();
        }
        
        #endregion

        #region Player Death / Respawn Rules
        
        [Server]
        public void NotifyPlayerDied(PlayerHealth victim, NetworkObject killer)
        {
            if (victim == null)
                return;

            switch (_mode.Value)
            {
                case GameModeType.Deathmatch:
                    HandleDeathmatchDeath(victim, killer);
                    break;

                case GameModeType.Arena:
                    if (_state.Value == MatchState.Live)
                    {
                        RecordKillDeath(victim, killer);
                        CheckArenaEliminationWin();
                    }
                    break;

                case GameModeType.CTF:
                    if (_state.Value == MatchState.Live || _state.Value == MatchState.Warmup)
                        RecordKillDeath(victim, killer);
                    break;
            }
        }

        [Server]
        public void NotifyPlayerDied(PlayerHealth victim, DamageResult result)
        {
            if (victim == null)
                return;

            switch (_mode.Value)
            {
                case GameModeType.Deathmatch:
                    HandleDeathmatchDeath(victim, result);
                    break;

                case GameModeType.Arena:
                    if (_state.Value == MatchState.Live)
                    {
                        RecordKillDeath(victim, result);
                        CheckArenaEliminationWin();
                    }
                    break;

                case GameModeType.CTF:
                    if (_state.Value == MatchState.Live || _state.Value == MatchState.Warmup)
                        RecordKillDeath(victim, result);
                    break;
            }
        }

        [Server]
        public void NotifyPlayerRespawned(PlayerHealth player)
        {
            // Later:
            // - update HUD/state
            // - reset spawn protection
        }

        [Server]
        public bool CanPlayerRespawn(PlayerHealth player)
        {
            if (player == null)
                return false;

            switch (_mode.Value)
            {
                case GameModeType.Deathmatch:
                    return _state.Value == MatchState.Waiting ||
                           _state.Value == MatchState.Warmup ||
                           _state.Value == MatchState.Live;

                case GameModeType.CTF:
                    return _state.Value == MatchState.Waiting ||
                           _state.Value == MatchState.Warmup ||
                           _state.Value == MatchState.Live;

                case GameModeType.Arena:
                    // Arena: no mid-round respawn once live.
                    return _state.Value == MatchState.Waiting ||
                           _state.Value == MatchState.PreRound ||
                           _state.Value == MatchState.Warmup;

                default:
                    return true;
            }
        }

        [Server]
        public float GetRespawnDelay(PlayerHealth player)
        {
            bool practiceState =
                _state.Value == MatchState.Waiting ||
                _state.Value == MatchState.Warmup ||
                _state.Value == MatchState.PreRound;

            if (practiceState)
                return 1f;

            return _mode.Value switch
            {
                GameModeType.Deathmatch => 3f,
                GameModeType.CTF => ctfRespawnDelay,

                GameModeType.Arena => 0f,

                _ => 3f
            };
        }
        
        [Server]
        public bool ShouldSpawnPlayerImmediately()
        {
            return _mode.Value switch
            {
                GameModeType.Deathmatch => true,

                GameModeType.CTF => true,

                GameModeType.Arena =>
                    _state.Value == MatchState.Waiting ||
                    _state.Value == MatchState.PreRound,

                _ => true
            };
        }

        #endregion

        #region Stats

        [Server]
        void RecordKillDeath(PlayerHealth victim, NetworkObject killer)
        {
            if (victim.TryGetComponent(out Player.PlayerStats victimStats))
                victimStats.AddDeath();

            if (killer != null &&
                killer != victim.NetworkObject &&
                killer.TryGetComponent(out Player.PlayerStats killerStats))
            {
                killerStats.AddKill();
            }
        }
        
        [Server]
        void RecordKillDeath(PlayerHealth victim, DamageResult result)
        {
            RecordKillDeath(victim, result.Attacker);
            BroadcastKillFeed(victim, result);
        }

        [Server]
        void ResetAllPlayerStats()
        {
            if (TeamManager.Instance == null)
                return;

            foreach (var id in TeamManager.Instance.Players)
            {
                if (id == null)
                    continue;

                if (id.TryGetComponent(out Player.PlayerStats stats))
                    stats.ResetStats();
            }
        }

        #endregion

        #region Match Start Conditions

        [Server]
        bool CanStartWarmup()
        {
            return _mode.Value switch
            {
                GameModeType.Deathmatch => ConnectedPlayerCount() >= 1,
                GameModeType.Arena => ConnectedPlayerCount() >= 2,
                GameModeType.CTF => ConnectedPlayerCount() >= 2,
                _ => true
            };
        }

        [Server]
        bool CanStartMatch()
        {
            return _mode.Value switch
            {
                GameModeType.Deathmatch => ConnectedPlayerCount() >= 1,
                GameModeType.Arena => ConnectedPlayerCount() >= 2,
                GameModeType.CTF => ConnectedPlayerCount() >= 2,
                _ => true
            };
        }

        [Server]
        int ConnectedPlayerCount()
        {
            return ServerManager.Clients.Count;
        }

        #endregion

        #region Timer Helpers

        bool HasStateTimerExpired()
        {
            return TimeManager.Tick >= _stateEndTick.Value;
        }

        public float GetStateTimeRemaining()
        {
            var timeManager = InstanceFinder.TimeManager;

            if (timeManager == null)
                return 0f;

            uint now = timeManager.Tick;
            uint end = _stateEndTick.Value;

            if (now >= end)
                return 0f;

            return (float)timeManager.TicksToTime(end - now);
        }

        #endregion

        #region Kill Feed
        
        [ObserversRpc(BufferLast = false)]
        private void RpcShowKillFeed(string killerName, string victimName, string sourceText, bool isSelfKill, bool isEnvironmental)
        {
            UI.HUD.KillFeedUI.Instance?.Push(killerName, victimName, sourceText, isSelfKill, isEnvironmental);
        }
        
        [Server]
        void BroadcastKillFeed(PlayerHealth victim, DamageResult result)
        {
            if (victim == null)
                return;

            string victimName = GetPlayerDisplayName(victim.NetworkObject);
            string killerName = GetPlayerDisplayName(result.Attacker);

            bool isSelfKill =
                result.Attacker != null &&
                victim.NetworkObject != null &&
                result.Attacker == victim.NetworkObject;

            bool isEnvironmental =
                result.Attacker == null ||
                result.Type == DamageType.Environment ||
                result.Type == DamageType.Impact;

            string sourceText = GetDamageSourceText(result);

            RpcShowKillFeed(
                killerName,
                victimName,
                sourceText,
                isSelfKill,
                isEnvironmental);
        }
        
        static string GetPlayerDisplayName(NetworkObject nob)
        {
            if (nob == null)
                return "Unknown";

            if (nob.TryGetComponent(out PlayerIdentity identity))
                return identity.DisplayName;

            return nob.name;
        }
        
        static string GetDamageSourceText(DamageResult result)
        {
            return result.Type switch
            {
                DamageType.Projectile => "projectile",
                DamageType.Explosion => "explosion",
                DamageType.Impact => "impact",
                DamageType.Environment => "environment",
                DamageType.Suicide => "suicide",
                _ => "eliminated"
            };
        }
        #endregion
        
        [Server]
        private void HandleSessionEligibilityChanged(PlayerSession session)
        {
            if (_mode.Value != GameModeType.Arena)
                return;

            if (_state.Value == MatchState.Live)
            {
                EvaluateArenaRoundEndFromSessions();
                return;
            }

            if (_state.Value == MatchState.Waiting)
                TryResumeArenaFromWaiting();
        }

        [Server]
        private void HandleSessionConnected(PlayerSession session)
        {
            TryResumeArenaFromWaiting();
        }

        [Server]
        private void HandleSessionBodyLinked(PlayerSession session)
        {
            TryResumeArenaFromWaiting();
        }

        [Server]
        private void TryResumeArenaFromWaiting()
        {
            if (_mode.Value != GameModeType.Arena)
                return;

            if (_state.Value != MatchState.Waiting)
                return;

            if (!HasEnoughArenaPlayersForRound())
                return;

            Debug.Log("[GameModeManager] Arena has enough players. Starting pre-round.");
            StartArenaPreRound();
        }
    }
}
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using _Scripts.Game.Teams;

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

        [Header("Timing")]
        [SerializeField] float warmupSeconds = 5f;
        [SerializeField] float matchSeconds = 600f;
        [SerializeField] float postMatchSeconds = 10f;

        [Header("Arena")]
        [SerializeField] int roundsToWin = 3;
        [SerializeField] float postRoundSeconds = 5f;

        [Header("Deathmatch")]
        [SerializeField] int killLimit = 25;

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

        #endregion

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

            _mode.Value = startingMode;

            if (CanStartWarmup())
            {
                if (_mode.Value == GameModeType.Arena)
                    StartArenaPreRound();
                else
                    StartWarmup();
            }
            else
            {
                StartWaiting();
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
                StartArenaPreRound();
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
        void SetState(MatchState next, float duration)
        {
            _state.Value = next;

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
                    StartWarmup();
                    break;
            }
        }

        [Server]
        void TickArena()
        {
            if (_state.Value == MatchState.Waiting)
            {
                if (CanStartWarmup())
                    StartArenaPreRound();

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
                        StartArenaPreRound();
                    break;

                case MatchState.PostMatch:
                    ResetArenaMatch();
                    StartWaiting();
                    break;
            }
        }

        [Server]
        void TickCTF()
        {
            // Same as Deathmatch for now.
            TickDeathmatch();
        }

        #endregion

        #region Arena Logic

        [Server]
        void StartArenaPreRound()
        {
            RoundResetManager.Instance?.ResetForArenaRound();

            SpawnManager.Instance?.SpawnPendingPlayers();
            SpawnManager.Instance?.RespawnAllPlayers();
            SpawnManager.Instance?.SetAllPlayersFrozen(true);

            SetState(MatchState.PreRound, warmupSeconds);

            // Later:
            // - clear projectiles
            // - reset round-only state
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
            if (TeamManager.Instance == null)
                return;

            int aliveA = TeamManager.Instance.CountAlive(TeamId.TeamA);
            int aliveB = TeamManager.Instance.CountAlive(TeamId.TeamB);

            if (aliveA <= 0 && aliveB <= 0)
            {
                EndArenaRound(TeamId.None);
                return;
            }

            if (aliveA <= 0)
            {
                EndArenaRound(TeamId.TeamB);
                return;
            }

            if (aliveB <= 0)
                EndArenaRound(TeamId.TeamA);
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
                    // Later.
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
                    return _state.Value == MatchState.Live ||
                           _state.Value == MatchState.Warmup;

                case GameModeType.Arena:
                    return _state.Value == MatchState.Waiting ||
                           _state.Value == MatchState.PreRound;

                case GameModeType.CTF:
                    return _state.Value == MatchState.Live ||
                           _state.Value == MatchState.Warmup;

                default:
                    return true;
            }
        }

        [Server]
        public float GetRespawnDelay(PlayerHealth player)
        {
            return _mode.Value switch
            {
                GameModeType.Deathmatch => 3f,

                GameModeType.CTF => 5f,

                GameModeType.Arena =>
                    _state.Value == MatchState.Waiting ||
                    _state.Value == MatchState.PreRound
                        ? 2f
                        : 0f,

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
            if (TimeManager == null)
                return 0f;

            uint now = TimeManager.Tick;
            uint end = _stateEndTick.Value;

            if (now >= end)
                return 0f;

            return (float)TimeManager.TicksToTime(end - now);
        }

        #endregion
    }
}
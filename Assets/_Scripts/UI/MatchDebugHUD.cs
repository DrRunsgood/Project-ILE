using TMPro;
using UnityEngine;
using _Scripts.Game;
using _Scripts.Game.Teams;
using _Scripts.Player;
using _Scripts.Game.CTF;

[RequireComponent(typeof(TMP_Text))]
public sealed class MatchDebugHUD : MonoBehaviour
{
    [SerializeField] float refreshRate = 0.2f;

    TMP_Text _label;
    PlayerIdentity _identity;
    PlayerStats _stats;

    void Awake()
    {
        _label = GetComponent<TMP_Text>();

        LocalPlayerContext.OnLocalPlayerReady += HandleLocalPlayerReady;
        LocalPlayerContext.OnLocalPlayerCleared += HandleLocalPlayerCleared;

        if (LocalPlayerContext.IsReady)
            HandleLocalPlayerReady(LocalPlayerContext.Controller);

        InvokeRepeating(nameof(Refresh), 0.1f, refreshRate);
    }

    void OnDestroy()
    {
        LocalPlayerContext.OnLocalPlayerReady -= HandleLocalPlayerReady;
        LocalPlayerContext.OnLocalPlayerCleared -= HandleLocalPlayerCleared;

        CancelInvoke(nameof(Refresh));
    }

    void HandleLocalPlayerReady(AdvancedPredictedController controller)
    {
        if (controller == null)
            return;

        _identity = controller.GetComponent<PlayerIdentity>();
        _stats = controller.GetComponent<PlayerStats>();

        Refresh();
    }

    void HandleLocalPlayerCleared()
    {
        _identity = null;
        _stats = null;

        if (_label != null)
            _label.text = "";
    }

    void Refresh()
    {
        GameModeManager gm = GameModeManager.Instance;

        if (gm == null)
        {
            _label.text = "No GameMode";
            return;
        }

        float remaining = gm.GetStateTimeRemaining();
        int minutes = Mathf.FloorToInt(remaining / 60f);
        int seconds = Mathf.FloorToInt(remaining % 60f);

        string playerName = _identity != null ? _identity.DisplayName : "Unknown";
        string team = _identity != null ? _identity.Team.ToString() : "None";
        int kills = _stats != null ? _stats.Kills : 0;
        int deaths = _stats != null ? _stats.Deaths : 0;

        int teamAScore = gm.TeamAScore;
        int teamBScore = gm.TeamBScore;

        if (gm.Mode == GameModeType.CTF && CTFManager.Instance != null)
        {
            teamAScore = CTFManager.Instance.TeamAScore;
            teamBScore = CTFManager.Instance.TeamBScore;
        }

        _label.text =
            $"Mode: {gm.Mode}\n" +
            $"State: {gm.State}  {minutes:00}:{seconds:00}\n" +
            $"Round: {gm.CurrentRound}\n" +
            $"Team A: {teamAScore} | Team B: {teamBScore}\n" +
            $"{playerName} [{team}]\n" +
            $"K/D: {kills}/{deaths}";
    }
    
}
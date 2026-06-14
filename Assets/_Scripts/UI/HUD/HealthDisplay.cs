using TMPro;
using UnityEngine;
using _Scripts.Player;

[RequireComponent(typeof(TMP_Text))]
public sealed class HealthDisplay : MonoBehaviour
{
    [Header("Refresh")]
    [SerializeField, Range(0.02f, 1f)] float refreshRate = 0.1f;

    TMP_Text _label;
    PlayerHealth _health;

    void Awake()
    {
        _label = GetComponent<TMP_Text>();

        LocalPlayerContext.OnLocalPlayerReady += HandleLocalPlayerReady;
        LocalPlayerContext.OnLocalPlayerCleared += HandleLocalPlayerCleared;

        if (LocalPlayerContext.IsReady)
            HandleLocalPlayerReady(LocalPlayerContext.Controller);
    }

    void HandleLocalPlayerReady(AdvancedPredictedController controller)
    {
        _health = LocalPlayerContext.Health;

        if (_health == null)
        {
            Debug.LogWarning("[HealthDisplay] Local player registered but PlayerHealth was not found.");
            return;
        }

        Debug.Log($"[HealthDisplay] Bound to local player health: {_health.name}");

        UpdateHealthDisplay();
        InvokeRepeating(nameof(UpdateHealthDisplay), refreshRate, refreshRate);
    }

    void HandleLocalPlayerCleared()
    {
        CancelInvoke();
        _health = null;
    }

    void UpdateHealthDisplay()
    {
        if (_health == null)
            return;

        _label.text = $"{_health.Current:0}";
    }

    void OnDestroy()
    {
        LocalPlayerContext.OnLocalPlayerReady -= HandleLocalPlayerReady;
        LocalPlayerContext.OnLocalPlayerCleared -= HandleLocalPlayerCleared;

        CancelInvoke();
    }
}
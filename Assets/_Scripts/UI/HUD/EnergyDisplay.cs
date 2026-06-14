using TMPro;
using UnityEngine;
using _Scripts.Player;
using _Scripts.Packs;

[RequireComponent(typeof(TMP_Text))]
public sealed class EnergyDisplay : MonoBehaviour
{
    [Header("Shield Tint")]
    [SerializeField] Color shieldColour = new Color(0.7f, 0.3f, 1f);

    [Header("Refresh")]
    [SerializeField, Range(0.02f, 1f)] float refreshRate = 0.1f;

    TMP_Text _label;
    Color _baseColour;

    AdvancedPredictedController _ctrl;
    PackManager _packMgr;

    /* ───────────────────────────────────────────── */

    void Awake()
    {
        _label = GetComponent<TMP_Text>();
        _baseColour = _label.color;

        // Subscribe to local player lifecycle
        LocalPlayerContext.OnLocalPlayerReady += HandleLocalPlayerReady;
        LocalPlayerContext.OnLocalPlayerCleared += HandleLocalPlayerCleared;

        // In case player already exists (late UI init)
        if (LocalPlayerContext.IsReady)
            HandleLocalPlayerReady(LocalPlayerContext.Controller);
    }

    void HandleLocalPlayerReady(AdvancedPredictedController controller)
    {
        _ctrl = controller;
        _packMgr = controller.GetComponent<PackManager>();

        Debug.Log($"[EnergyDisplay] Bound to local player: {_ctrl.name}");

        if (_packMgr != null)
        {
            _packMgr.OnPackChanged -= HandlePackChange;
            _packMgr.OnPackChanged += HandlePackChange;

            HandlePackChange(_packMgr.CurrentId, _packMgr.Active);
        }

        UpdateEnergyNumeric();
        InvokeRepeating(nameof(UpdateEnergyNumeric), refreshRate, refreshRate);
    }

    void HandleLocalPlayerCleared()
    {
        CancelInvoke();

        if (_packMgr != null)
            _packMgr.OnPackChanged -= HandlePackChange;

        _ctrl = null;
        _packMgr = null;
    }

    /* ---------- numeric value ---------- */
    void UpdateEnergyNumeric()
    {
        if (_ctrl == null) return;
        _label.text = $"{_ctrl.Energy:0}";
    }

    /* ---------- colour change ---------- */
    void HandlePackChange(PackId id, bool active)
    {
        bool shieldOn = (id == PackId.Shield) && active;
        _label.color = shieldOn ? shieldColour : _baseColour;
    }

    void OnDestroy()
    {
        LocalPlayerContext.OnLocalPlayerReady -= HandleLocalPlayerReady;
        LocalPlayerContext.OnLocalPlayerCleared -= HandleLocalPlayerCleared;

        CancelInvoke();

        if (_packMgr != null)
            _packMgr.OnPackChanged -= HandlePackChange;
    }
}
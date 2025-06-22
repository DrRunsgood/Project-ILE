// _Scripts/UI/EnergyDisplay.cs
using FishNet;
using FishNet.Object;
using TMPro;
using UnityEngine;
using _Scripts.Player;
using _Scripts.Packs;      // PackManager

[RequireComponent(typeof(TMP_Text))]
public sealed class EnergyDisplay : MonoBehaviour
{
    [Header("Shield Tint")]
    [SerializeField] Color shieldColour = new Color(0.7f, 0.3f, 1f);

    [Header("Refresh")]
    [SerializeField, Range(0.02f, 1f)] float refreshRate = 0.1f;

    TMP_Text _label;
    Color    _baseColour;
    AdvancedPredictedController _ctrl;
    PackManager _packMgr;

    /* ───────────────────────────────────────────── */
    void Awake()
    {
        _label      = GetComponent<TMP_Text>();
        _baseColour = _label.color;   // whatever the designer set
        StartCoroutine(FindLocalPlayer());
    }

    System.Collections.IEnumerator FindLocalPlayer()
    {
        // Wait until the local player spawns
        while (_ctrl == null)
        {
            NetworkObject local = InstanceFinder.ClientManager.Connection?.FirstObject;
            if (local && local.TryGetComponent(out AdvancedPredictedController c)) _ctrl = c;
            if (_ctrl == null) yield return null;     // retry next frame
        }

        _packMgr = _ctrl.GetComponent<PackManager>();
        if (_packMgr != null)
        {
            // Immediate first colour update
            HandlePackChange(_packMgr.CurrentId, _packMgr.Active);
            // Subscribe for future changes
            _packMgr.OnPackChanged += HandlePackChange;
        }

        // Start periodic numeric update
        InvokeRepeating(nameof(UpdateEnergyNumeric), refreshRate, refreshRate);
    }

    /* ---------- numeric value, still on timer ---------- */
    void UpdateEnergyNumeric()
    {
        if (_ctrl == null) return;
        _label.text = $"{_ctrl.Energy:0}";
    }

    /* ---------- colour change via event ---------- */
    void HandlePackChange(PackId id, bool active)
    {
        bool shieldOn = (id == PackId.Shield) && active;
        _label.color = shieldOn ? shieldColour : _baseColour;
    }

    void OnDestroy()
    {
        if (_packMgr != null) _packMgr.OnPackChanged -= HandlePackChange;
    }
}

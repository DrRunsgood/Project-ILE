// _Scripts/UI/EnergyUpdater.cs
using FishNet;
using FishNet.Object;
using TMPro;
using UnityEngine;
using _Scripts.Player;      // AdvancedPredictedController

[RequireComponent(typeof(TMP_Text))]
public sealed class EnergyDisplay : MonoBehaviour
{
    [SerializeField, Range(0.02f, 1f)]
    float refreshRate = 0.1f;          // seconds

    TMP_Text                     _label;
    AdvancedPredictedController  _ctrl;

    /* ------------------------------------------------------------------ */
    void Awake()
    {
        _label = GetComponent<TMP_Text>();
        StartCoroutine(FindLocalPlayer());
    }

    System.Collections.IEnumerator FindLocalPlayer()
    {
        // wait until the client has a spawned player
        while (_ctrl == null)
        {
            NetworkObject local = InstanceFinder.ClientManager.Connection?.FirstObject;
            if (local && local.TryGetComponent(out AdvancedPredictedController c))
                _ctrl = c;

            if (_ctrl == null)
                yield return null;      // try again next frame
        }

        // we have the controller – start periodic update
        InvokeRepeating(nameof(UpdateEnergy), refreshRate, refreshRate);
    }

    void UpdateEnergy()
    {
        if (_ctrl == null) return;

        float e   = _ctrl.Energy;     // getter added below
        float max = _ctrl.MaxEnergy;  // getter added below (optional)

        // simple numeric read-out – adjust as you like
        _label.text = $"{e:0}";
    }
}
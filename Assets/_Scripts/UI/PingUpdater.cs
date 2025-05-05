using FishNet;
using FishNet.Managing.Timing;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public sealed class PingUpdater : MonoBehaviour
{
    [SerializeField] float refreshRate = 1f;   // seconds

    TMP_Text _label;

    void Awake()
    {
        _label = GetComponent<TMP_Text>();
        InvokeRepeating(nameof(UpdatePing), refreshRate, refreshRate);
    }

    void UpdatePing()
    {
        TimeManager tm = InstanceFinder.TimeManager;
        if (tm == null)
            return;                   // Network not running yet

        double rttMs = tm.RoundTripTime;   // round‑trip in seconds

        _label.text = $"Ping: {rttMs} ms";
    }
}
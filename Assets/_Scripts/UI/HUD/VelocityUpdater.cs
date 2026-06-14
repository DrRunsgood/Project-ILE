using TMPro;
using UnityEngine;
using _Scripts.Player;

[RequireComponent(typeof(TMP_Text))]
public sealed class VelocityUpdater : MonoBehaviour
{
    [SerializeField, Range(0.01f, 1f)]
    float refreshRate = 0.2f;

    TMP_Text _label;
    Rigidbody _rb;

    void Awake()
    {
        _label = GetComponent<TMP_Text>();

        LocalPlayerContext.OnLocalPlayerReady += HandleLocalPlayerReady;
        LocalPlayerContext.OnLocalPlayerCleared += HandleLocalPlayerCleared;

        if (LocalPlayerContext.IsReady)
            HandleLocalPlayerReady(LocalPlayerContext.Controller);
    }

    void OnDestroy()
    {
        LocalPlayerContext.OnLocalPlayerReady -= HandleLocalPlayerReady;
        LocalPlayerContext.OnLocalPlayerCleared -= HandleLocalPlayerCleared;

        CancelInvoke(nameof(UpdateVelocity));
    }

    void HandleLocalPlayerReady(AdvancedPredictedController controller)
    {
        if (controller == null)
            return;

        _rb = controller.GetComponent<Rigidbody>();

        CancelInvoke(nameof(UpdateVelocity));
        InvokeRepeating(nameof(UpdateVelocity), refreshRate, refreshRate);

        UpdateVelocity();

        Debug.Log($"[VelocityUpdater] Bound to local player: {controller.name}");
    }

    void HandleLocalPlayerCleared()
    {
        _rb = null;
        CancelInvoke(nameof(UpdateVelocity));

        if (_label != null)
            _label.text = "0.0 m/s";
    }

    void UpdateVelocity()
    {
        if (_rb == null)
            return;

        float speedMps = _rb.linearVelocity.magnitude;
        _label.text = $"{speedMps:F1} m/s";
    }
}
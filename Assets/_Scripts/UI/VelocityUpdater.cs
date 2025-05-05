using FishNet;
using FishNet.Object;
using TMPro;
using UnityEngine;
using _Scripts.Player;

[RequireComponent(typeof(TMP_Text))]
public sealed class VelocityUpdater : MonoBehaviour
{
    [SerializeField, Range(0.01f, 1f)]
    float refreshRate = 0.2f;          // seconds

    TMP_Text  _label;
    Rigidbody _myRb;

    void Awake()
    {
        _label = GetComponent<TMP_Text>();
        StartCoroutine(FindRb());
    }
    System.Collections.IEnumerator FindRb()
    {
        // Wait until the local connection and player exist
        while (_myRb == null)
        {
            NetworkObject local = InstanceFinder.ClientManager.Connection?.FirstObject;
            if (local != null && local.TryGetComponent(out AdvancedPredictedController ctrl))
                _myRb = ctrl.GetComponent<Rigidbody>();

            if (_myRb == null)
                yield return null;      // try again next frame
        }

        // Now we have the rigidbody – start the repeated update
        InvokeRepeating(nameof(UpdateVelocity), refreshRate, refreshRate);
    }

    void UpdateVelocity()
    {
        float speedMps = _myRb.linearVelocity.magnitude;
        _label.text    = $"{speedMps:F1} m/s";
    }
}

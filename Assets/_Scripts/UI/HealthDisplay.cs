using System.Collections;
using FishNet;
using FishNet.Object;
using TMPro;
using UnityEngine;
using _Scripts.Player;     // where PlayerHealth lives

[RequireComponent(typeof(TMP_Text))]
public sealed class HealthDisplay : MonoBehaviour
{
    TMP_Text      _label;
    PlayerHealth  _health;          // cached once we find it

    void Awake()
    {
        _label = GetComponent<TMP_Text>();
        StartCoroutine(FindLocalPlayer());
    }

    IEnumerator FindLocalPlayer()
    {
        while (_health == null)
        {
            /* first NetworkObject that belongs to *our* connection */
            NetworkObject me =
                InstanceFinder.ClientManager.Connection?.FirstObject;

            if (me != null && me.TryGetComponent(out PlayerHealth ph))
            {
                /* guard-check – we only want the object we OWN */
                if (ph.IsOwner)
                {
                    _health = ph;
                    _health.OnHealthChanged += UpdateLabel;
                    UpdateLabel(_health.Current, _health.Max);   // initial draw
                    yield break;
                }
            }
            yield return null;        // wait one frame, then try again
        }
    }

    void OnDestroy()
    {
        if (_health != null)
            _health.OnHealthChanged -= UpdateLabel;
    }

    void UpdateLabel(int current, int max)
    {
        _label.text = $"{current}";
        // or $"{current}/{max}"  if you want both numbers
    }
}
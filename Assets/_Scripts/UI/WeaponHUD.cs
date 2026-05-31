using TMPro;
using UnityEngine;
using _Scripts.Player;
using _Scripts.Weapons;

[RequireComponent(typeof(TMP_Text))]
public sealed class WeaponHUD : MonoBehaviour
{
    [Header("Refresh")]
    [SerializeField, Range(0.02f, 1f)] float refreshRate = 0.1f;

    TMP_Text _label;
    WeaponManager _wm;

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
        if (controller == null)
            return;

        _wm = controller.GetComponent<WeaponManager>();

        if (_wm == null)
        {
            Debug.LogWarning("[WeaponHUD] Local player registered but WeaponManager was not found.");
            return;
        }

        UpdateWeaponDisplay();
        InvokeRepeating(nameof(UpdateWeaponDisplay), refreshRate, refreshRate);
    }

    void HandleLocalPlayerCleared()
    {
        CancelInvoke(nameof(UpdateWeaponDisplay));
        _wm = null;

        if (_label != null)
            _label.text = "";
    }

    void UpdateWeaponDisplay()
    {
        if (_wm == null)
            return;

        var def = _wm.ActiveDefinition;

        if (def == null)
        {
            _label.text = "";
            return;
        }

        if (!def.usesAmmo)
        {
            _label.text = def.displayName;
            return;
        }

        _label.text = $"{def.displayName}\n{_wm.ActiveAmmo} / {_wm.ActiveMaxAmmo}";
    }

    void OnDestroy()
    {
        LocalPlayerContext.OnLocalPlayerReady -= HandleLocalPlayerReady;
        LocalPlayerContext.OnLocalPlayerCleared -= HandleLocalPlayerCleared;

        CancelInvoke(nameof(UpdateWeaponDisplay));
    }
}
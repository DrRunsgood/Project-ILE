using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class FocusedIFFPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image healthBack;
    [SerializeField] private Image healthFill;
    [SerializeField] private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (!canvasGroup)
            canvasGroup = GetComponent<CanvasGroup>();

        if (!canvasGroup)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        ConfigureHealthFill();

        SetVisible(false);
    }

    private void OnEnable()
    {
        ConfigureHealthFill();
        SetVisible(false);
    }

    private void ConfigureHealthFill()
    {
        if (!healthFill)
            return;

        healthFill.type = Image.Type.Filled;
        healthFill.fillMethod = Image.FillMethod.Horizontal;
        healthFill.fillOrigin = (int)Image.OriginHorizontal.Left;
        healthFill.fillClockwise = true;
        healthFill.fillAmount = 1f;
        healthFill.gameObject.SetActive(true);
    }

    public void SetVisible(bool visible)
    {
        if (!canvasGroup)
            return;

        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void SetData(string displayName, float health01, Color color)
    {
        health01 = Mathf.Clamp01(health01);

        SetVisible(true);

        if (nameText)
        {
            nameText.text = string.IsNullOrWhiteSpace(displayName)
                ? "Player"
                : displayName;

            nameText.color = color;
        }

        if (healthBack)
            healthBack.gameObject.SetActive(true);

        if (healthFill)
        {
            healthFill.gameObject.SetActive(true);
            healthFill.color = color;
            healthFill.fillAmount = health01;
        }
    }
}
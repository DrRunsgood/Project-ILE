using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class IFFWidget : MonoBehaviour
{
    [SerializeField] TMP_Text marker;
    [SerializeField] TMP_Text nameText;
    [SerializeField] Image healthBack;
    [SerializeField] Image healthFill;
    [SerializeField] CanvasGroup canvasGroup;

    RectTransform _rect;

    void Awake()
    {
        _rect = (RectTransform)transform;
        if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();

        if (marker) marker.text = "v";

        if (healthFill)
        {
            healthFill.type = Image.Type.Filled;
            healthFill.fillMethod = Image.FillMethod.Horizontal;
            healthFill.fillOrigin = (int)Image.OriginHorizontal.Left;
        }
    }

    public void SetVisible(bool visible)
    {
        if (canvasGroup)
            canvasGroup.alpha = visible ? 1f : 0f;
    }

    public void SetScreenPosition(Vector2 pos)
    {
        _rect.anchoredPosition = pos;
    }

    public void SetData(string displayName, float health01, Color color, bool focused, float alpha)
    {
        if (canvasGroup)
            canvasGroup.alpha = alpha;

        if (marker)
        {
            marker.text = "v";
            marker.color = color;
            marker.gameObject.SetActive(true);
        }

        bool showDetail = focused;

        if (nameText)
        {
            nameText.gameObject.SetActive(showDetail);
            if (showDetail)
            {
                nameText.text = string.IsNullOrWhiteSpace(displayName)
                    ? "Player"
                    : displayName;
                nameText.color = color;
            }
        }

        if (healthBack)
            healthBack.gameObject.SetActive(showDetail);

        if (healthFill)
        {
            healthFill.gameObject.SetActive(showDetail);
            healthFill.color = color;
            healthFill.fillAmount = Mathf.Clamp01(health01);
        }
    }
}
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

    public void SetData(Color color, float alpha)
    {
        if (canvasGroup)
            canvasGroup.alpha = alpha;

        if (marker)
        {
            marker.text = "v";
            marker.color = color;
            marker.gameObject.SetActive(true);
        }

        if (nameText)
            nameText.gameObject.SetActive(false);

        if (healthBack)
            healthBack.gameObject.SetActive(false);

        if (healthFill)
            healthFill.gameObject.SetActive(false);
    }
}
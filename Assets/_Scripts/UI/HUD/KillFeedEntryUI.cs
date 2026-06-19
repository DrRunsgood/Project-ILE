using TMPro;
using UnityEngine;

namespace _Scripts.UI.HUD
{
    public sealed class KillFeedEntryUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;

        private void Awake()
        {
            if (text == null)
                text = GetComponentInChildren<TMP_Text>();
        }

        public void SetText(string value)
        {
            if (text != null)
                text.text = value;
        }
    }
}
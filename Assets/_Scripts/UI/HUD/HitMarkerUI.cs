using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI.HUD
{
    public sealed class HitMarkerUI : MonoBehaviour
    {
        public static HitMarkerUI Instance { get; private set; }

        [Header("Shape")]
        [SerializeField] float lineLength = 14f;
        [SerializeField] float lineThickness = 2f;
        [SerializeField] float centerGap = 8f;

        [Header("Animation")]
        [SerializeField] float fadeSeconds = 0.14f;
        [SerializeField] float popScale = 1.25f;

        CanvasGroup _group;
        RectTransform _root;
        float _timer;

        static Sprite _whiteSprite;

        void Awake()
        {
            Instance = this;
            BuildIfNeeded();
            HideImmediate();
        }

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        void Update()
        {
            if (_timer <= 0f)
                return;

            _timer -= Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(_timer / Mathf.Max(0.001f, fadeSeconds));

            _group.alpha = t;

            float scale = Mathf.Lerp(1f, popScale, t);
            _root.localScale = Vector3.one * scale;

            if (_timer <= 0f)
                HideImmediate();
        }

        public void ShowHit()
        {
            BuildIfNeeded();

            _timer = fadeSeconds;
            _group.alpha = 1f;
            _root.localScale = Vector3.one * popScale;
            _root.gameObject.SetActive(true);
        }

        void HideImmediate()
        {
            if (_group != null)
                _group.alpha = 0f;

            if (_root != null)
            {
                _root.localScale = Vector3.one;
                _root.gameObject.SetActive(false);
            }

            _timer = 0f;
        }

        void BuildIfNeeded()
        {
            if (_root != null)
                return;

            GameObject rootGo = new GameObject("HitMarker", typeof(RectTransform), typeof(CanvasGroup));
            rootGo.transform.SetParent(transform, false);

            _root = rootGo.GetComponent<RectTransform>();
            _root.anchorMin = new Vector2(0.5f, 0.5f);
            _root.anchorMax = new Vector2(0.5f, 0.5f);
            _root.pivot = new Vector2(0.5f, 0.5f);
            _root.anchoredPosition = Vector2.zero;
            _root.sizeDelta = Vector2.zero;

            _group = rootGo.GetComponent<CanvasGroup>();
            _group.blocksRaycasts = false;
            _group.interactable = false;

            float offset = centerGap + lineLength * 0.35f;

            CreateLine("TR", new Vector2(offset, offset), 45f);
            CreateLine("BL", new Vector2(-offset, -offset), 45f);
            CreateLine("TL", new Vector2(-offset, offset), -45f);
            CreateLine("BR", new Vector2(offset, -offset), -45f);
        }

        void CreateLine(string name, Vector2 pos, float rotation)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(_root, false);

            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = pos;
            rt.sizeDelta = new Vector2(lineLength, lineThickness);
            rt.localRotation = Quaternion.Euler(0f, 0f, rotation);

            Image img = go.GetComponent<Image>();
            img.raycastTarget = false;
            img.color = Color.white;
            img.sprite = GetWhiteSprite();
        }

        static Sprite GetWhiteSprite()
        {
            if (_whiteSprite != null)
                return _whiteSprite;

            _whiteSprite = Sprite.Create(
                Texture2D.whiteTexture,
                new Rect(0, 0, Texture2D.whiteTexture.width, Texture2D.whiteTexture.height),
                new Vector2(0.5f, 0.5f));

            return _whiteSprite;
        }
    }
}
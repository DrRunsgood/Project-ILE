using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.UI.HUD
{
    public sealed class KillFeedUI : MonoBehaviour
    {
        public static KillFeedUI Instance { get; private set; }

        [Header("References")]
        [SerializeField] private KillFeedEntryUI entryPrefab;
        [SerializeField] private Transform entryRoot;

        [Header("Settings")]
        [SerializeField] private int maxEntries = 6;
        [SerializeField] private float entryLifetime = 5f;

        private readonly List<KillFeedEntryUI> _entries = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (entryRoot == null)
                entryRoot = transform;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void Push(
            string killerName,
            string victimName,
            string sourceText,
            bool isSelfKill,
            bool isEnvironmental)
        {
            if (entryPrefab == null || entryRoot == null)
            {
                Debug.LogWarning("[KillFeedUI] Missing entry prefab or entry root.");
                return;
            }

            string message = BuildMessage(
                killerName,
                victimName,
                sourceText,
                isSelfKill,
                isEnvironmental);

            KillFeedEntryUI entry = Instantiate(entryPrefab, entryRoot);
            entry.SetText(message);

            _entries.Add(entry);

            while (_entries.Count > maxEntries)
                RemoveEntryAt(0);

            StartCoroutine(RemoveAfterDelay(entry, entryLifetime));
        }

        private static string BuildMessage(
            string killerName,
            string victimName,
            string sourceText,
            bool isSelfKill,
            bool isEnvironmental)
        {
            killerName = string.IsNullOrWhiteSpace(killerName) ? "Unknown" : killerName;
            victimName = string.IsNullOrWhiteSpace(victimName) ? "Unknown" : victimName;
            sourceText = string.IsNullOrWhiteSpace(sourceText) ? "eliminated" : sourceText;

            if (isEnvironmental)
                return $"{victimName} died";

            if (isSelfKill)
                return $"{victimName} eliminated themselves";

            return $"{killerName} [{sourceText}] {victimName}";
        }

        private IEnumerator RemoveAfterDelay(KillFeedEntryUI entry, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (entry == null)
                yield break;

            int index = _entries.IndexOf(entry);

            if (index >= 0)
                RemoveEntryAt(index);
        }

        private void RemoveEntryAt(int index)
        {
            if (index < 0 || index >= _entries.Count)
                return;

            KillFeedEntryUI entry = _entries[index];
            _entries.RemoveAt(index);

            if (entry != null)
                Destroy(entry.gameObject);
        }
    }
}
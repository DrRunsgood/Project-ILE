using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VfxPool
{
    /* one global pool per prefab path */
    static readonly Dictionary<string, Queue<GameObject>> _pools = new();

    /* ––––– public API ––––– */
    public static void Spawn(string path, Vector3 pos, Quaternion rot, float ttl = 3f, Transform parent = null)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            Debug.LogWarning("VfxPool: Spawn called with an empty Resources path.");
            return;
        }

        if (!_pools.TryGetValue(path, out Queue<GameObject> queue))
        {
            queue = new Queue<GameObject>();
            _pools[path] = queue;
        }

        GameObject instance = null;

        while (queue.Count > 0 && instance == null)
            instance = queue.Dequeue();

        if (instance == null)
        {
            GameObject prefab = Resources.Load<GameObject>(path);

            if (prefab == null)
            {
                Debug.LogWarning($"VfxPool: \"{path}\" was not found in Resources.");
                return;
            }

            instance = Object.Instantiate(prefab);
        }

        Transform instanceTransform = instance.transform;

        instanceTransform.SetParent(parent, true);
        instanceTransform.SetPositionAndRotation(pos, rot);

        instance.SetActive(true);

        Runner.StartCoroutine(DespawnAfter(instance, path, ttl));
    }

    /* ––––– private ––––– */
    static IEnumerator DespawnAfter(GameObject instance, string path, float ttl)
    {
        yield return new WaitForSeconds(Mathf.Max(0f, ttl));

        if (instance == null)
            yield break;

        instance.SetActive(false);

        Transform instanceTransform = instance.transform;

        instanceTransform.SetParent(Runner.transform, false);

        instanceTransform.localPosition = Vector3.zero;
        instanceTransform.localRotation = Quaternion.identity;

        if (!_pools.TryGetValue(path, out Queue<GameObject> queue))
        {
            queue = new Queue<GameObject>();
            _pools[path] = queue;
        }

        queue.Enqueue(instance);
    }

    /* one invisible MonoBehaviour that lives forever */
    static VfxRunner _runner;
    static VfxRunner Runner
    {
        get
        {
            if (_runner == null)
            {
                var go = new GameObject("VfxPool-Runner");
                Object.DontDestroyOnLoad(go);
                _runner = go.AddComponent<VfxRunner>();
            }
            return _runner;
        }
    }
}

/* just an empty shell to run coroutines */
class VfxRunner : MonoBehaviour { }
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VfxPool
{
    /* one global pool per prefab path */
    static readonly Dictionary<string, Queue<GameObject>> _pools = new();

    /* ––––– public API ––––– */
    public static void Spawn(string path, Vector3 pos, Quaternion rot,
        float ttl = 3f, Transform parent = null)
    {
        if (!_pools.TryGetValue(path, out var q))
            _pools[path] = q = new Queue<GameObject>();

        GameObject go = (q.Count > 0 && q.Peek() != null)
            ? q.Dequeue()
            : Resources.Load<GameObject>(path);

        if (go == null)
        {
            Debug.LogWarning($"VfxPool: “{path}” not found in Resources");
            return;
        }

        go = Object.Instantiate(go, pos, rot, parent);
        go.SetActive(true);

        /* start despawn coroutine on the hidden runner */
        Runner.StartCoroutine(DespawnAfter(go, path, ttl));
    }

    /* ––––– private ––––– */
    static IEnumerator DespawnAfter(GameObject go, string path, float ttl)
    {
        yield return new WaitForSeconds(ttl);

        go.SetActive(false);

        if (!_pools.TryGetValue(path, out var q))
            _pools[path] = q = new Queue<GameObject>();

        q.Enqueue(go);
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
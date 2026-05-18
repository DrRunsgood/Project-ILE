using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class PlayerSpawnPoint : MonoBehaviour
{
    [SerializeField] bool registerOnStart = true;

    IEnumerator Start()
    {
        if (!registerOnStart)
            yield break;

        while (SpawnManager.Instance == null)
            yield return null;

        SpawnManager.Instance.AddSpawnPoint(transform);
    }

    void OnDisable()
    {
        if (SpawnManager.Instance != null)
            SpawnManager.Instance.RemoveSpawnPoint(transform);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.DrawRay(transform.position, transform.forward * 1.5f);
    }
}
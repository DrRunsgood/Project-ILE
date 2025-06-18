using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

public class SpawnManager : NetworkBehaviour
{
    /* ───── singleton ───── */
    public static SpawnManager Instance { get; private set; }

    /* ───── inspector ───── */
    [SerializeField] List<Transform> spawnPoints = new List<Transform>();

    /* connection → player */
    readonly Dictionary<NetworkConnection, NetworkObject> spawnedPlayers = new ();

    /* ───────────────────── */
    #region Unity
    void Awake()  => Instance = this;

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
    #endregion
    /* ───────────────────── */

    /* ------------------------------------------------------------ */
    #region Spawn-point list helpers
    public void AddSpawnPoint   (Transform t) { if (!spawnPoints.Contains(t)) spawnPoints.Add(t); }
    public void RemoveSpawnPoint(Transform t) {               spawnPoints.Remove(t);              }

    public Transform GetRandomSpawn()
    {
        return spawnPoints.Count == 0
             ? null
             : spawnPoints[Random.Range(0, spawnPoints.Count)];
    }
    #endregion
    /* ------------------------------------------------------------ */

    #region Player life-cycle
    public void SpawnPlayer(NetworkConnection conn, GameObject prefab)
    {
        if (!IsServerStarted || prefab == null) return;

        Transform sp = GetRandomSpawn();
        Vector3   pos = sp ? sp.position : Vector3.zero;
        Quaternion rot = sp ? sp.rotation : Quaternion.identity;

        NetworkObject nob = Instantiate(prefab, pos, rot).GetComponent<NetworkObject>();
        Spawn(nob, conn);                       // Fish-Net call
        spawnedPlayers[conn] = nob;
    }

    public void DespawnPlayer(NetworkConnection conn)
    {
        if (!IsServerStarted || !spawnedPlayers.TryGetValue(conn, out var nob)) return;
        Despawn(nob);
        spawnedPlayers.Remove(conn);
    }

    public void DespawnAllPlayers()
    {
        if (!IsServerStarted) return;
        foreach (var nob in spawnedPlayers.Values)
            Despawn(nob);
        spawnedPlayers.Clear();
    }
    #endregion
}

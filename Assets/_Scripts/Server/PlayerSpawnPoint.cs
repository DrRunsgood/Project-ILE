using System.Collections;
using UnityEngine;
using _Scripts.Game;

public enum SpawnTeam : byte
{
    Any,
    TeamA,
    TeamB
}

[DisallowMultipleComponent]
public sealed class PlayerSpawnPoint : MonoBehaviour
{
    [Header("Registration")]
    [SerializeField] bool registerOnStart = true;

    [Header("Spawn Rules")]
    [SerializeField] bool allowAnyMode = true;
    [SerializeField] GameModeType mode = GameModeType.Deathmatch;

    [SerializeField] bool allowAnyTeam = true;
    [SerializeField] SpawnTeam team = SpawnTeam.Any;

    public bool AllowAnyMode => allowAnyMode;
    public GameModeType Mode => mode;

    public bool AllowAnyTeam => allowAnyTeam;
    public SpawnTeam Team => team;

    IEnumerator Start()
    {
        if (!registerOnStart)
            yield break;

        while (SpawnManager.Instance == null)
            yield return null;

        SpawnManager.Instance.AddSpawnPoint(this);
    }

    void OnDisable()
    {
        if (SpawnManager.Instance != null)
            SpawnManager.Instance.RemoveSpawnPoint(this);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = team switch
        {
            SpawnTeam.TeamA => Color.blue,
            SpawnTeam.TeamB => Color.red,
            _ => Color.green
        };

        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.DrawRay(transform.position, transform.forward * 1.5f);
    }
}
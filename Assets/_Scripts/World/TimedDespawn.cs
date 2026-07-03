using FishNet.Object;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class TimedDespawn : NetworkBehaviour
{
    [Header("Lifetime")]
    [SerializeField] float defaultLifetime = 60f;
    [SerializeField] bool startOnServerSpawn = false;

    int _ticksRemaining;
    bool _armed;

    public bool IsArmed => _armed;
    public float DefaultLifetime => defaultLifetime;

    public override void OnStartServer()
    {
        base.OnStartServer();

        if (TimeManager != null)
            TimeManager.OnTick += OnServerTick;

        if (startOnServerSpawn)
            Arm(defaultLifetime);
    }

    public override void OnStopServer()
    {
        if (TimeManager != null)
            TimeManager.OnTick -= OnServerTick;

        _armed = false;
        _ticksRemaining = 0;

        base.OnStopServer();
    }

    [Server]
    public void Arm(float lifetime)
    {
        float safeLifetime = Mathf.Max(0.01f, lifetime);

        float tickDelta = TimeManager != null
            ? (float)TimeManager.TickDelta
            : 1f / 60f;

        _ticksRemaining = Mathf.Max(1, Mathf.CeilToInt(safeLifetime / tickDelta));
        _armed = true;
    }

    [Server]
    public void ArmDefault()
    {
        Arm(defaultLifetime);
    }

    [Server]
    public void Cancel()
    {
        _armed = false;
        _ticksRemaining = 0;
    }

    [Server]
    void OnServerTick()
    {
        if (!_armed)
            return;

        _ticksRemaining--;

        if (_ticksRemaining > 0)
            return;

        _armed = false;
        _ticksRemaining = 0;

        if (IsSpawned)
            ServerManager.Despawn(gameObject, DespawnType.Pool);
    }
}
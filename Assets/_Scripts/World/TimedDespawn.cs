using FishNet.Object;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class TimedDespawn : NetworkBehaviour
{
    [Header("Lifetime")]
    [SerializeField] float defaultLifetime = 60f;
    [SerializeField] bool startOnServerSpawn = false;

    float _despawnAt;
    bool _armed;

    public bool IsArmed => _armed;

    public override void OnStartServer()
    {
        base.OnStartServer();

        if (startOnServerSpawn)
            Arm(defaultLifetime);
    }

    [Server]
    public void Arm(float lifetime)
    {
        _despawnAt = Time.time + Mathf.Max(0.01f, lifetime);
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
    }

    void Update()
    {
        if (!IsServer || !_armed)
            return;

        if (Time.time >= _despawnAt)
        {
            _armed = false;
            ServerManager.Despawn(gameObject, DespawnType.Pool);
        }
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        _armed = false;
    }
}
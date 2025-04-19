using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public sealed class LagCompensationManager : MonoBehaviour
{
    #region Singleton ---------------------------------------------------------
    public static LagCompensationManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    #endregion

    [Tooltip("How many server ticks of history to keep (≈ 2‑3s worth).")]
    [SerializeField] private int bufferTicks = 250;

    private readonly Dictionary<NetworkObject, CircularBuffer<FireSnapshot>> _buffers = new();

    public struct FireSnapshot
    {
        public Vector3 Position;  // Fire point position
        public Vector3 Direction; // Fire point forward
        public Vector3 Velocity;  // Player velocity
        public uint Tick;         // Tick this snapshot was taken

        public FireSnapshot(Vector3 pos, Vector3 dir, Vector3 vel, uint tick)
        {
            Position = pos;
            Direction = dir;
            Velocity = vel;
            Tick = tick;
        }
    }

    /// <summary>
    /// Called by each player's predicted controller once per server tick.
    /// </summary>
    public void RecordSnapshot(NetworkObject playerObj,
                               Vector3 firePointPos,
                               Vector3 firePointDir,
                               Vector3 playerVelocity,
                               uint tick)
    {
        if (!_buffers.TryGetValue(playerObj, out var buf))
        {
            buf = new CircularBuffer<FireSnapshot>(bufferTicks);
            _buffers[playerObj] = buf;
        }

        buf.PushBack(new FireSnapshot(firePointPos, firePointDir, playerVelocity, tick));
    }

    /// <summary>
    /// Attempts to get a snapshot for a player at a specific tick.
    /// </summary>
    public bool TryGetSnapshot(NetworkObject playerObj, uint tick, out FireSnapshot snap)
    {
        snap = default;

        if (!_buffers.TryGetValue(playerObj, out var buf))
        {
            return false;
        }

        foreach (var s in buf)
        {
            if (s.Tick == tick)
            {
                snap = s;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns the number of snapshots available for a given player.
    /// </summary>
    public int GetSnapshotCount(NetworkObject playerObj)
    {
        return _buffers.TryGetValue(playerObj, out var buf) ? buf.Count : 0;
    }
}

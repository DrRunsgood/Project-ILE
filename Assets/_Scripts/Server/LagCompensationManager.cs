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
        public Vector3 Position;
        public Vector3 Direction;
        public Vector3 Velocity;
        public uint Tick;

        public FireSnapshot(Vector3 pos, Vector3 dir, Vector3 vel, uint tick)
        {
            Position = pos;
            Direction = dir;
            Velocity = vel;
            Tick = tick;
        }
    }

    public void RecordSnapshot(NetworkObject playerObj, Vector3 firePointPos, Vector3 firePointDir, Vector3 playerVelocity, uint tick)
    {
        if (!_buffers.TryGetValue(playerObj, out var buf))
        {
            buf = new CircularBuffer<FireSnapshot>(bufferTicks);
            _buffers[playerObj] = buf;
        }
        buf.PushBack(new FireSnapshot(firePointPos, firePointDir, playerVelocity, tick));
    }

    public bool TryGetSnapshot(NetworkObject playerObj, uint targetTick, out FireSnapshot snap, uint tolerance = 0) // Added tolerance parameter
    {
        snap = default;
        if (!_buffers.TryGetValue(playerObj, out var buf))
        {
            return false;
        }

        FireSnapshot bestMatch = default;
        uint closestDiff = uint.MaxValue;
        bool foundMatch = false;

        foreach (var s in buf)
        {
            if (s.Tick == targetTick)
            {
                snap = s;
                return true; // Exact match is best
            }

            if (tolerance > 0)
            {
                uint diff = (s.Tick > targetTick) ? (s.Tick - targetTick) : (targetTick - s.Tick);
                if (diff <= tolerance && diff < closestDiff)
                {
                    closestDiff = diff;
                    bestMatch = s;
                    foundMatch = true;
                }
            }
        }

        if (foundMatch)
        {
            snap = bestMatch;
            return true;
        }

        return false;
    }

    public int GetSnapshotCount(NetworkObject playerObj)
    {
        return _buffers.TryGetValue(playerObj, out var buf) ? buf.Count : 0;
    }
}
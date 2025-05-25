// Network/InputCmd.cs
using UnityEngine;
using FishNet.Object.Prediction;

namespace _Scripts.Player                // use one consistent namespace
{
    /* ------------------------------------------------------------ *
     *  Bit-flags for all boolean inputs (one byte = eight buttons) *
     * ------------------------------------------------------------ */
    [System.Flags]
    public enum InputButtons : byte
    {
        None    = 0,
        Jump    = 1 << 0,
        Sprint  = 1 << 1,
        Crouch  = 1 << 2,
        Jetpack = 1 << 3,
        Ski     = 1 << 4,
        WallRun = 1 << 5,
        Fire    = 1 << 6,
        // add more as needed (keep ≤ 8 flags)
    }

    /* ------------------------------------------------------------ *
     *  One input sample for a single simulation-tick               *
     * ------------------------------------------------------------ */
    public struct InputCmd : IReplicateData
    {
        public uint         tick;      // which tick this belongs to
        public Vector2      move;      // WASD, already normalised
        public Vector2      look;      // raw mouse delta
        public InputButtons buttons;   // on/off bits

        /* ----- IReplicateData implementation ----- */
        public uint  GetTick()            => tick;
        public void  SetTick(uint value)  => tick = value;
        public void  Dispose()            { }
    }

    /* ------------------------------------------------------------ *
     *  Tiny ring-buffer (64 ≈ one second @ 60 Hz) for prediction   *
     * ------------------------------------------------------------ */
    public sealed class InputCmdRing
    {
        const int Size = 64;                        // > max RTT in ticks
        readonly InputCmd[] _buf = new InputCmd[Size];

        public void Push(in InputCmd cmd) => _buf[cmd.tick % Size] = cmd;
        public InputCmd Get(uint tick)    => _buf[tick % Size];
    }
}

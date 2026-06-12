using UnityEngine;

namespace _Scripts.Weapons
{
    public readonly struct FirePose
    {
        public readonly Vector3 Position;
        public readonly Vector3 Direction;
        public readonly Vector3 Velocity;
        public readonly uint Tick;

        public FirePose(Vector3 position, Vector3 direction, Vector3 velocity, uint tick)
        {
            Position = position;
            Direction = direction;
            Velocity = velocity;
            Tick = tick;
        }
    }
}
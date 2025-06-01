using UnityEngine;
using FishNet;
using FishNet.Object;
using _Scripts.Player;

namespace YourGameNamespace.Weapons
{
    public class ProjectileWeapon : BaseWeapon
    {
        [Header("Projectile Settings")]
        [SerializeField] private NetworkObject projectilePrefab;
        [SerializeField] private float projectileSpeed = 50f;
        [SerializeField, Range(0f,1f)]
        private float velocityInheritanceFactor = 0.5f;
        [SerializeField] private Transform firePoint;

        private InputHandler  _input;
        private NetworkObject _netObj;

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (IsOwner) _input = GetComponent<InputHandler>();
        }
        public override void OnStartServer()
        {
            base.OnStartServer();
            _netObj = GetComponent<NetworkObject>();
        }
        void Update()
        {
            if (!IsOwner || _input == null)
                return;
            
            var  cmd   = _input.CmdRing.Get(TimeManager.Tick);
            bool fire  = (cmd.buttons & InputButtons.Fire) != 0;

            if (fire && Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + 1f / fireRate;
                Vector3 dir  = firePoint.forward.normalized;

                /* one‑way latency (ms → s) */
                double halfRTTms   = TimeManager.HalfRoundTripTime;   // long → double
                double oneWaySec   = halfRTTms * 0.001;               // ms → seconds

                /* convert seconds to whole ticks (always round‑up) */
                double tickLen     = TimeManager.TickDelta;           // seconds per tick
                uint   lagTicks    = (uint)Mathf.CeilToInt((float)(oneWaySec / tickLen));
                
                /* tiny safety cushion so we never overshoot */
                const uint safety  = 1u;

                /* rewind, clamped so it never goes negative */
                uint rewind        = lagTicks + safety;
                Debug.Log($"Rewind:" + rewind);
                uint bufferedTick  = (rewind > TimeManager.Tick) ? 0u : TimeManager.Tick - rewind;

                Cmd_RequestSpawn(bufferedTick);
            }
        }
        
        [ServerRpc]
        private void Cmd_RequestSpawn(uint clientTick)
        {
            if (projectilePrefab == null || _netObj == null) return;
            if (!LagCompensationManager.Instance.TryGetSnapshot(_netObj, clientTick, out var snap))
                return;

            Vector3 finalVel = snap.Direction.normalized * projectileSpeed + snap.Velocity * velocityInheritanceFactor; //clientDir * projectileSpeed

            var nob = InstanceFinder.NetworkManager.GetPooledInstantiated(projectilePrefab, true);
            if (nob == null) return;

            if (nob.TryGetComponent(out BaseProjectile proj))
            {
                // use lag-compensated muzzle pos
                proj.Init(snap.Position, finalVel, TimeManager.Tick, _netObj);
                ServerManager.Spawn(nob);
            }
            else
                ServerManager.Despawn(nob, DespawnType.Pool);
        }

        public override void Fire() { }
    }
}

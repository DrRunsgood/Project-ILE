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
        [SerializeField, Range(0f,2f)]
        private float velocityInheritanceFactor = 0.5f;
        [SerializeField] private Transform firePoint;

        // cached refs
        private InputHandler  _input;
        private NetworkObject _netObj;

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (IsOwner)
                _input = GetComponent<InputHandler>();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            _netObj = GetComponent<NetworkObject>();
        }

        void Update()
        {
            if (!IsOwner || _input == null) return;
            if (_input.FireInput && Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + 1f / fireRate;
                var dir = firePoint.forward.normalized;
                var pos = firePoint.position;
                Cmd_RequestSpawn(dir, pos, TimeManager.Tick - 2u);
            }
        }

        [ServerRpc]
        private void Cmd_RequestSpawn(Vector3 dir, Vector3 pos, uint clientTick)
        {
            if (projectilePrefab == null || _netObj == null) return;
            if (!LagCompensationManager.Instance.TryGetSnapshot(_netObj, clientTick, out var snap))
                return;

            dir.Normalize();
            if (dir == Vector3.zero) return;

            var finalVel = dir * projectileSpeed
                         + snap.Velocity * velocityInheritanceFactor;

            var nob = InstanceFinder.NetworkManager
                             .GetPooledInstantiated(projectilePrefab, true);
            if (nob == null) return;

            if (nob.TryGetComponent(out BaseProjectile proj))
            {
                // pass shooter so projectile can ignore self‑hits
                proj.Init(pos, finalVel, TimeManager.Tick, _netObj);
                ServerManager.Spawn(nob);
            }
            else ServerManager.Despawn(nob, DespawnType.Pool);
        }

        public override void Fire() { }
    }
}

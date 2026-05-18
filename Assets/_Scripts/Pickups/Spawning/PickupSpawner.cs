using FishNet.Object;
using UnityEngine;

namespace _Scripts.Pickups.Spawning
{
    public enum PickupSpawnType : byte
    {
        Generic,
        Weapon,
        Ammo,
        Item,
        Pack
    }

    public struct PickupSpawnPayload
    {
        public PickupSpawnType Type;
        public int StartingAmmo;
        public int AmmoAmount;
        public int ItemCount;
    }

    public interface ISpawnInitialized
    {
        void ServerInitializeFromSpawner(PickupSpawnPayload payload);
    }

    [DisallowMultipleComponent]
    public sealed class PickupSpawner : NetworkBehaviour
    {
        [Header("Spawn")]
        [SerializeField] NetworkObject pickupPrefab;
        [SerializeField] Transform spawnPoint;

        [Header("Spawn Metadata")]
        [SerializeField] PickupSpawnType spawnType = PickupSpawnType.Generic;
        [SerializeField] int startingAmmo = -1; // -1 = prefab/default
        [SerializeField] int ammoAmount = 0;
        [SerializeField] int itemCount = 1;

        [Header("Respawn")]
        [SerializeField] bool spawnOnServerStart = true;
        [SerializeField] float respawnDelay = 30f;

        [Header("Pickup Arming")]
        [SerializeField] float pickupArmDelay = 0.15f;

        NetworkObject _currentSpawned;
        bool _respawnPending;
        float _respawnAt;

        Transform SpawnTransform => spawnPoint != null ? spawnPoint : transform;

        public override void OnStartServer()
        {
            base.OnStartServer();

            if (spawnOnServerStart)
                SpawnNow();
        }

        void Update()
        {
            if (!IsServerStarted || !_respawnPending)
                return;

            if (Time.time < _respawnAt)
                return;

            _respawnPending = false;
            SpawnNow();
        }

        [Server]
        void SpawnNow()
        {
            if (pickupPrefab == null)
                return;

            if (_currentSpawned != null)
                return;

            Transform t = SpawnTransform;

            NetworkObject nob = PoolUtil.TakeFromPool(pickupPrefab);
            if (nob == null)
                return;

            nob.transform.SetPositionAndRotation(t.position, t.rotation);

            var link = nob.GetComponent<SpawnedPickupLink>();
            if (link == null)
                link = nob.gameObject.AddComponent<SpawnedPickupLink>();

            link.Bind(this, nob);

            ServerManager.Spawn(nob);
            _currentSpawned = nob;

            var payload = new PickupSpawnPayload
            {
                Type = spawnType,
                StartingAmmo = startingAmmo,
                AmmoAmount = ammoAmount,
                ItemCount = itemCount
            };

            if (nob.TryGetComponent(out ISpawnInitialized initialized))
                initialized.ServerInitializeFromSpawner(payload);

            if (nob.TryGetComponent(out _Scripts.Weapons.WeaponPickup weaponPickup))
                weaponPickup.Arm(pickupArmDelay);

            if (nob.TryGetComponent(out _Scripts.Packs.PackPickup packPickup))
                packPickup.Arm(pickupArmDelay);
        }

        [Server]
        public void NotifyPickupDespawned(NetworkObject nob)
        {
            if (nob == null || _currentSpawned != nob)
                return;

            _currentSpawned = null;
            _respawnPending = true;
            _respawnAt = Time.time + Mathf.Max(0.01f, respawnDelay);
        }

        [Server]
        public void ForceRespawnNow()
        {
            if (_currentSpawned != null)
            {
                ServerManager.Despawn(_currentSpawned, DespawnType.Pool);
                _currentSpawned = null;
            }

            _respawnPending = false;
            SpawnNow();
        }
    }
}
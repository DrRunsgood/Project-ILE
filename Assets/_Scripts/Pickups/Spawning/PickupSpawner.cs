using FishNet.Object;
using UnityEngine;
using _Scripts.Data;
using _Scripts.FNPool;
using _Scripts.Weapons;

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

    public enum PickupRespawnMode : byte
    {
        Timed,
        RoundResetOnly,
        MatchStartOnly,
        Manual
    }

    public struct PickupSpawnPayload
    {
        public PickupSpawnType Type;
        public int StartingAmmo;
        public AmmoType AmmoType;
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
        [SerializeField] int startingAmmo = -1;
        [SerializeField] AmmoType ammoType = AmmoType.None;
        [SerializeField] int ammoAmount = 0;
        [SerializeField] int itemCount = 1;

        [Header("Respawn")]
        [SerializeField] PickupRespawnMode respawnMode = PickupRespawnMode.Timed;
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

            if (respawnMode != PickupRespawnMode.Timed)
                return;

            if (Time.time < _respawnAt)
                return;

            _respawnPending = false;
            SpawnNow();
        }

        [Server]
        void SpawnNow()
        {
            if (pickupPrefab == null || _currentSpawned != null)
                return;

            Transform t = SpawnTransform;

            NetworkObject nob = PoolUtil.TakeFromPool(pickupPrefab);
            if (nob == null)
                return;

            nob.transform.SetPositionAndRotation(t.position, t.rotation);

            SpawnedPickupLink link = nob.GetComponent<SpawnedPickupLink>();
            if (link == null)
                link = nob.gameObject.AddComponent<SpawnedPickupLink>();

            link.Bind(this, nob);

            ServerManager.Spawn(nob);
            _currentSpawned = nob;

            PickupSpawnPayload payload = new()
            {
                Type = spawnType,
                StartingAmmo = startingAmmo,
                AmmoType = ammoType,
                AmmoAmount = ammoAmount,
                ItemCount = itemCount
            };

            if (nob.TryGetComponent(out ISpawnInitialized initialized))
                initialized.ServerInitializeFromSpawner(payload);

            if (nob.TryGetComponent(out WeaponPickup weaponPickup))
                weaponPickup.Arm(pickupArmDelay);

            if (nob.TryGetComponent(out AmmoPickup ammoPickup))
                ammoPickup.Arm(pickupArmDelay);

            if (nob.TryGetComponent(out Packs.PackPickup packPickup))
                packPickup.Arm(pickupArmDelay);
        }

        [Server]
        public void NotifyPickupDespawned(NetworkObject nob)
        {
            if (nob == null || _currentSpawned != nob)
                return;

            _currentSpawned = null;

            switch (respawnMode)
            {
                case PickupRespawnMode.Timed:
                    _respawnPending = true;
                    _respawnAt = Time.time + Mathf.Max(0.01f, respawnDelay);
                    break;

                case PickupRespawnMode.RoundResetOnly:
                case PickupRespawnMode.MatchStartOnly:
                case PickupRespawnMode.Manual:
                    _respawnPending = false;
                    break;
            }
        }

        [Server]
        public void ForceRespawnNow()
        {
            DespawnCurrent();
            _respawnPending = false;
            SpawnNow();
        }

        [Server]
        public void DespawnCurrent()
        {
            if (_currentSpawned == null)
                return;

            NetworkObject current = _currentSpawned;
            _currentSpawned = null;

            ServerManager.Despawn(current, DespawnType.Pool);
        }

        [Server]
        public void ResetForRound()
        {
            if (respawnMode != PickupRespawnMode.RoundResetOnly)
                return;

            ForceRespawnNow();
        }
    }
}
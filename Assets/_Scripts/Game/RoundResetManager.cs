using FishNet.Object;
using UnityEngine;
using _Scripts.Pickups.Spawning;

namespace _Scripts.Game
{
    [DisallowMultipleComponent]
    public sealed class RoundResetManager : NetworkBehaviour
    {
        public static RoundResetManager Instance { get; private set; }

        PickupSpawner[] _pickupSpawners;

        void Awake()
        {
            Instance = this;
        }

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            RefreshCaches();
        }

        [Server]
        public void RefreshCaches()
        {
            _pickupSpawners = FindObjectsByType<PickupSpawner>(FindObjectsInactive.Exclude);
        }

        [Server]
        public void ResetCommonWorldState()
        {
            ClearRoundObjects();
        }

        [Server]
        public void ResetForArenaRound()
        {
            RefreshCaches();

            ResetCommonWorldState();

            SpawnManager.Instance?.ResetAllPlayerInventoriesForRound();

            ResetRoundPickupSpawners();
        }

        [Server]
        public void ResetForCTFMatchStart()
        {
            RefreshCaches();

            ResetCommonWorldState();

            // Intentionally not resetting inventories or pickup spawners yet.
            // CTF is continuous-play; add only if match-start design requires it.
        }

        [Server]
        void ClearRoundObjects()
        {
            RoundScopedObject[] objects =
                FindObjectsByType<RoundScopedObject>(FindObjectsInactive.Exclude);

            foreach (RoundScopedObject obj in objects)
            {
                if (obj == null)
                    continue;

                if (obj.Scope != RoundScopedObject.CleanupScope.Round)
                    continue;

                ServerManager.Despawn(obj.gameObject, DespawnType.Pool);
            }
        }

        [Server]
        void ResetRoundPickupSpawners()
        {
            if (_pickupSpawners == null)
                return;

            foreach (PickupSpawner spawner in _pickupSpawners)
            {
                if (spawner == null)
                    continue;

                spawner.ResetForRound();
            }
        }
        
        [Server]
        public void PrepareForMapUnload()
        {
            RefreshCaches();

            if (_pickupSpawners != null)
            {
                foreach (PickupSpawner spawner in _pickupSpawners)
                {
                    if (spawner == null)
                        continue;

                    spawner.ServerPrepareForMapUnload();
                }
            }

            ClearRoundObjects();
        }
    }
}
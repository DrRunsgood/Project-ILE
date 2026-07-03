// _Scripts/Weapons/Ground/WeaponPickup.cs
using FishNet.Object;
using UnityEngine;
using _Scripts.Data;
using _Scripts.Packs;
using _Scripts.Pickups.Spawning;

namespace _Scripts.Weapons
{
    [RequireComponent(typeof(Collider))]
    public sealed class WeaponPickup : NetworkBehaviour, ISpawnInitialized
    {
        [Header("Pickup")]
        [SerializeField] float defaultArmDelay = 0.15f;
        [SerializeField] WeaponDefinition definition;

        [Header("Runtime Ammo")]
        [SerializeField] int runtimeAmmo = -1; // -1 = use definition.spawnAmmo

        double _pickupEnableTime;

        public WeaponDefinition Definition => definition;

        public int RuntimeAmmo =>
            definition != null && definition.usesAmmo
                ? ResolveAmmo(runtimeAmmo)
                : 0;

        static int ResolveAmmo(int value) => Mathf.Max(0, value);
        
        [Server]
        public void ServerInitializeFromSpawner(PickupSpawnPayload payload)
        {
            ServerInitializeAmmoFromSpawner(payload.StartingAmmo);
        }

        [Server]
        public void ServerSetRuntimeAmmo(int ammo)
        {
            runtimeAmmo = ammo;
        }

        [Server]
        public void ServerInitializeAmmoFromSpawner(int startingAmmo)
        {
            if (definition == null || !definition.usesAmmo)
            {
                runtimeAmmo = 0;
                return;
            }

            runtimeAmmo = startingAmmo >= 0
                ? Mathf.Clamp(startingAmmo, 0, definition.maxAmmo)
                : Mathf.Clamp(definition.spawnAmmo, 0, definition.maxAmmo);
        }

        [Server]
        public void Arm(float delay)
        {
            double enable = Time.timeAsDouble + delay;
            SetEnableTime(enable);
            RpcSetEnableTime(enable);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            if (definition != null && definition.usesAmmo && runtimeAmmo < 0)
                runtimeAmmo = Mathf.Clamp(definition.spawnAmmo, 0, definition.maxAmmo);

            if (_pickupEnableTime == 0)
            {
                double enable = Time.timeAsDouble + defaultArmDelay;
                SetEnableTime(enable);
                RpcSetEnableTime(enable);
            }
        }

        [ObserversRpc(BufferLast = true)]
        void RpcSetEnableTime(double enable)
        {
            if (IsServer)
                return;

            SetEnableTime(enable);
        }

        void SetEnableTime(double enable) => _pickupEnableTime = enable;

        void OnTriggerEnter(Collider other) => TryPickup(other);
        void OnTriggerStay(Collider other) => TryPickup(other);

        void TryPickup(Collider other)
        {
            if (Time.timeAsDouble < _pickupEnableTime)
                return;

            if (!IsServer && other.TryGetComponent(out NetworkObject nObj))
            {
                if (!nObj.IsOwner)
                    return;

                Server_RequestPickup(nObj);
                return;
            }

            if (!IsServer)
                return;

            if (!other.TryGetComponent(out WeaponManager wm))
                return;

            if (other.TryGetComponent(out PlayerHealth hp) && !hp.CanPickup)
                return;

            if (!CanPlayerPickup(other))
                return;

            if (wm.Server_AddOrMergeWeapon(definition, RuntimeAmmo))
                ServerManager.Despawn(gameObject, DespawnType.Pool);
        }

        [ServerRpc(RequireOwnership = false)]
        void Server_RequestPickup(NetworkObject playerObj)
        {
            if (!playerObj.TryGetComponent(out WeaponManager wm))
                return;

            if (playerObj.TryGetComponent(out PlayerHealth hp) && !hp.CanPickup)
                return;

            if (!CanPlayerPickup(playerObj.transform))
                return;

            if (wm.Server_AddOrMergeWeapon(definition, RuntimeAmmo))
                ServerManager.Despawn(gameObject, DespawnType.Pool);
        }

        bool CanPlayerPickup(Component player)
        {
            if (definition == null)
                return false;

            if (!definition.requiresEnergyPack)
                return true;

            return player.TryGetComponent(out PackManager pm) &&
                   pm.CurrentId == PackId.Energy;
        }
    }
}
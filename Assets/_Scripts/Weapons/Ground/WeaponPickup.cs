// _Scripts/Weapons/Ground/WeaponPickup.cs
using FishNet.Object;
using UnityEngine;
using _Scripts.Data;
using _Scripts.Packs;

namespace _Scripts.Weapons
{
    [RequireComponent(typeof(Collider))]
    public sealed class WeaponPickup : NetworkBehaviour
    {
        /* ───── Inspector ───── */
        [SerializeField] float            defaultArmDelay = 0.15f;   // scene‑placed fallback
        [SerializeField] WeaponDefinition definition;

        /* ───── Runtime ───── */
        double _pickupEnableTime;   // shared on all peers

        /* ════════════ Arming from WeaponManager ════════════ */
        [Server] public void Arm(float delay)               // called right after Spawn()
        {
            double enable = Time.timeAsDouble + delay;
            SetEnableTime(enable);                          // server local
            RpcSetEnableTime(enable);                       // every client (buffered)
        }

        /* ════════════ Scene‑placed objects ════════════ */
        public override void OnStartServer()
        {
            base.OnStartServer();
            if (_pickupEnableTime == 0)                     // was in scene
            {
                double enable = Time.timeAsDouble + defaultArmDelay;
                SetEnableTime(enable);
                RpcSetEnableTime(enable);
            }
        }

        /* ════════════ Tiny buffered RPC ════════════ */
        [ObserversRpc(BufferLast = true)]
        void RpcSetEnableTime(double enable)
        {
            if (IsServer) return;        // host already set locally
            SetEnableTime(enable);
        }

        void SetEnableTime(double enable) => _pickupEnableTime = enable;

        /* ════════════ Overlap events ════════════ */
        void OnTriggerEnter(Collider other) => TryPickup(other);
        void OnTriggerStay (Collider other) => TryPickup(other);
        
        void TryPickup(Collider other)
        {
            /* Grace period still active? */
            if (Time.timeAsDouble < _pickupEnableTime)
                return;

            /* ---------------- CLIENT PATH ---------------- */
            if (!IsServer && other.TryGetComponent(out NetworkObject nObj))
            {
                if (!nObj.IsOwner)          
                    return;
                // Let the server decide – still safe if we’re inside the trigger
                Server_RequestPickup(nObj);
                return;
            }

            /* ---------------- SERVER PATH ---------------- */
            if (!IsServer) return;
            if (!other.TryGetComponent(out WeaponManager wm)) return;

            /* Energy‑pack gate (if required) */
            if (definition.requiresEnergyPack)
            {
                if (!other.TryGetComponent(out PackManager pm) ||
                    pm.CurrentId != PackId.Energy)
                    return;
            }

            /* Attempt to add weapon, despawn if accepted */
            if (wm.Server_AddWeapon(definition))
                ServerManager.Despawn(gameObject, DespawnType.Pool);
        }

        /* ════════════ Bridge RPC (client → server) ════════════ */
        [ServerRpc(RequireOwnership = false)]
        void Server_RequestPickup(NetworkObject playerObj)
        {
            if (!playerObj.TryGetComponent(out WeaponManager wm)) return;

            if (definition.requiresEnergyPack)
            {
                if (!playerObj.TryGetComponent(out PackManager pm) ||
                    pm.CurrentId != PackId.Energy)
                    return;
            }

            if (wm.Server_AddWeapon(definition))
                ServerManager.Despawn(gameObject, DespawnType.Pool);
        }
    }
}
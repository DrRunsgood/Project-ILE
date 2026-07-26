// _Scripts/Player/PlayerCarriedDropCoordinator.cs

using FishNet.Object;
using UnityEngine;
using _Scripts.Game;
using _Scripts.Items;
using _Scripts.Packs;
using _Scripts.Weapons;

namespace _Scripts.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(WeaponManager))]
    [RequireComponent(typeof(PackManager))]
    [RequireComponent(typeof(ItemManager))]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class PlayerCarriedDropCoordinator : NetworkBehaviour
    {
        [Header("Terminal Scatter")]
        [SerializeField] private float dropOriginHeight = 0.75f;

        [SerializeField]
        [Range(0f, 45f)]
        private float angularJitter = 12f;

        private WeaponManager _weaponManager;
        private PackManager _packManager;
        private ItemManager _itemManager;
        private Rigidbody _rigidbody;

        private bool _terminalDropProcessed;

        private void Awake()
        {
            _weaponManager = GetComponent<WeaponManager>();
            _packManager = GetComponent<PackManager>();
            _itemManager = GetComponent<ItemManager>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            _terminalDropProcessed = false;
        }

        [Server]
        public void Server_DropForTerminalExit()
        {
            if (_terminalDropProcessed)
                return;

            _terminalDropProcessed = true;

            int weaponCount = _weaponManager != null ? _weaponManager.Server_GetTerminalDropCount() : 0;

            int packCount = _packManager != null ? _packManager.Server_GetTerminalDropCount() : 0;

            int itemStackCount = _itemManager != null ? _itemManager.Server_GetTerminalDropCount() : 0;

            int totalDropCount = weaponCount + packCount + itemStackCount;

            if (totalDropCount <= 0)
                return;

            Vector3 origin = transform.position + Vector3.up * dropOriginHeight;

            Vector3 playerVelocity = _rigidbody != null ? _rigidbody.linearVelocity : Vector3.zero;

            float baseAngle = Random.Range(0f, 360f);

            int ordinal = 0;

            for (int i = 0; i < weaponCount; i++)
            {
                WorldDropContext context = CreateContext(origin, playerVelocity, baseAngle, ordinal, totalDropCount);

                _weaponManager.Server_DropNextTerminalWeapon(context);

                ordinal++;
            }

            if (packCount > 0)
            {
                WorldDropContext context = CreateContext(origin, playerVelocity, baseAngle, ordinal, totalDropCount);

                _packManager.Server_DropTerminal(context);

                ordinal++;
            }

            for (int i = 0; i < itemStackCount; i++)
            {
                WorldDropContext context = CreateContext(origin, playerVelocity, baseAngle, ordinal, totalDropCount);

                _itemManager.Server_DropNextTerminalItemStack(context);

                ordinal++;
            }
        }

        [Server]
        public void Server_ResetForNewLife()
        {
            _terminalDropProcessed = false;
        }

        private WorldDropContext CreateContext(Vector3 origin, Vector3 playerVelocity, float baseAngle, int ordinal, int totalDropCount)
        {
            float spacing = 360f / Mathf.Max(1, totalDropCount);

            float jitter = angularJitter > 0f ? Random.Range(-angularJitter, angularJitter) : 0f;

            float angle = baseAngle + spacing * ordinal + jitter;

            Vector3 direction = Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward;

            return new WorldDropContext(origin, direction.normalized, playerVelocity);
        }
    }
}
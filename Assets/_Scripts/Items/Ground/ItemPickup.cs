// _Scripts/Items/Ground/ItemPickup.cs

using FishNet.Connection;
using FishNet.Object;
using UnityEngine;
using _Scripts.Items;
using _Scripts.Pickups.Spawning;

[RequireComponent(typeof(Collider))]
public sealed class ItemPickup : NetworkBehaviour, ISpawnInitialized
{
    [Header("Item")]
    [SerializeField] private ItemDefinition definition;

    [Header("Pickup")]
    [SerializeField] private float defaultArmDelay = 0.15f;

    private Collider _collider;

    /*
     * Authoritative quantity remaining in this world pickup.
     * This is server-side gameplay state; clients do not currently
     * need the value for presentation.
     */
    private int _runtimeCount;

    private double _pickupEnableTime;
    private bool _pickupClaimed;

    public int RuntimeCount => _runtimeCount;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        ResetServerRuntime();
        Arm(defaultArmDelay);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        /*
         * A host has already initialized the object through
         * OnStartServer. Do not overwrite authoritative state.
         */
        if (IsServer)
            return;

        if (_collider == null)
            _collider = GetComponent<Collider>();

        if (_collider != null)
            _collider.enabled = true;

        /*
         * Prevent local pickup requests until the server-provided
         * buffered arm time arrives.
         */
        _pickupEnableTime = double.PositiveInfinity;
    }

    /*
     * Called by PickupSpawner after the NetworkObject has spawned.
     */
    [Server]
    public void ServerInitializeFromSpawner(PickupSpawnPayload payload)
    {
        ServerSetRuntimeCount(payload.ItemCount);
    }

    /*
     * Used by runtime player drops to assign the exact stack count.
     */
    [Server]
    public void ServerSetRuntimeCount(int count)
    {
        _runtimeCount = Mathf.Max(1, count);
    }

    [Server]
    public void Arm(float delay)
    {
        double enableTime = Time.timeAsDouble + Mathf.Max(0f, delay);

        SetEnableTime(enableTime);
        RpcSetEnableTime(enableTime);
    }

    [ObserversRpc(BufferLast = true)]
    private void RpcSetEnableTime(double enableTime)
    {
        if (IsServer)
            return;

        SetEnableTime(enableTime);
    }

    private void SetEnableTime(double enableTime)
    {
        _pickupEnableTime = enableTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        TryPickup(other);
    }

    private void TryPickup(Collider other)
    {
        if (other == null)
            return;

        if (Time.timeAsDouble < _pickupEnableTime)
            return;

        /*
         * Remote client path: only the local player's collision may
         * generate a pickup request.
         */
        if (!IsServer)
        {
            ItemManager localItemManager = other.GetComponentInParent<ItemManager>();

            if (localItemManager == null || !localItemManager.IsOwner)
                return;

            Server_RequestPickup(localItemManager.NetworkObject);

            return;
        }

        /*
         * Server/host path.
         */
        ItemManager itemManager = other.GetComponentInParent<ItemManager>();

        if (itemManager == null)
            return;

        PlayerHealth health = other.GetComponentInParent<PlayerHealth>();

        if (health != null && !health.CanPickup)
            return;

        Server_TryGiveItems(itemManager);
    }

    [ServerRpc(RequireOwnership = false)]
    private void Server_RequestPickup(NetworkObject player, NetworkConnection sender = null)
    {
        if (_pickupClaimed)
            return;

        if (Time.timeAsDouble < _pickupEnableTime)
            return;

        /*
         * The requesting connection may act only on its own player
         * NetworkObject.
         */
        if (sender == null || !sender.IsValid || player == null || player.Owner != sender)
            return;
        

        if (!player.TryGetComponent(out ItemManager itemManager))
            return;

        if (player.TryGetComponent(out PlayerHealth health) && !health.CanPickup)
            return;

        Server_TryGiveItems(itemManager);
    }

    [Server]
    private void Server_TryGiveItems(ItemManager itemManager)
    {
        if (_pickupClaimed || itemManager == null || definition == null || _runtimeCount <= 0)
            return;

        /*
         * Prevent two server requests from processing the same count
         * concurrently.
         */
        _pickupClaimed = true;

        int accepted = itemManager.Server_AddItems(definition, _runtimeCount);

        if (accepted <= 0)
        {
            /*
             * The player had no available inventory capacity.
             * Leave the pickup in the world.
             */
            _pickupClaimed = false;
            return;
        }

        _runtimeCount -= accepted;

        if (_runtimeCount > 0)
        {
            /*
             * Partial pickup: the player accepted some items, while
             * the remainder stays in this world object.
             */
            _pickupClaimed = false;
            return;
        }

        if (_collider != null)
            _collider.enabled = false;

        ServerManager.Despawn(NetworkObject, DespawnType.Pool);
    }

    [Server]
    private void ResetServerRuntime()
    {
        _pickupClaimed = false;
        _pickupEnableTime = 0d;
        _runtimeCount = 1;

        if (_collider == null)
            _collider = GetComponent<Collider>();

        if (_collider != null)
            _collider.enabled = true;
    }
}
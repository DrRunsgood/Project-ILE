using UnityEngine;
using FishNet.Object;
using YourGameNamespace;

public class TestProjectileFirer : NetworkBehaviour
{
    [Header("Firing Settings")]
    [Tooltip("Projectile prefab with PredictedProjectile and NetworkObject components.")]
    [SerializeField]
    private PredictedProjectile projectilePrefab;

    [Tooltip("Transform representing the fire point (eg, muzzle).")]
    [SerializeField]
    private Transform firePoint;

    [Tooltip("Projectile lifetime in seconds.")]
    [SerializeField]
    private float projectileLifetime = 5f;

    // Maximum allowed passed time (lag compensation) in seconds.
    private const float MAX_PASSED_TIME = 0.3f;

    private void Update()
    {
        // Only the local (owning) client fires.
        if (!IsOwner)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("[TestProjectileFirer] Fire button pressed.");
            FireProjectile();
        }
    }

    // The owner immediately instantiates a predicted copy (which is not network spawned)
    // for immediate feedback and then notifies the server.
    private void FireProjectile()
    {
        Vector3 position = firePoint.position;
        Vector3 direction = firePoint.forward;

        // Spawn a local predicted copy. This copy is not network spawned.
        SpawnPredictedProjectile(position, direction, 0f);

        // Get the current tick from FishNet s TimeManager if available.
        uint tick = (base.TimeManager != null) ? base.TimeManager.Tick : 0;

        // Tell the server that we fired.
        ServerFire(position, direction, tick);
    }

    // Called on the server when a client fires. The server calculates passedTime,
    // instantiates a networked projectile, and calls Spawn() so that it replicates.
    [ServerRpc]
    private void ServerFire(Vector3 position, Vector3 direction, uint tick)
    {
        float passedTime = (float)base.TimeManager.TimePassed(tick, false);
        passedTime = Mathf.Min(MAX_PASSED_TIME / 2f, passedTime);
        Debug.Log("[TestProjectileFirer] ServerFire: pos " + position + ", passedTime " + passedTime);

        // On the server, instantiate and network spawn the projectile.
        SpawnNetworkedProjectile(position, direction, passedTime);
    }

    // Instantiates the local predicted projectile (owner only). This copy is not network spawned.
    private void SpawnPredictedProjectile(Vector3 position, Vector3 direction, float passedTime)
    {
        Debug.Log("[TestProjectileFirer] Spawning predicted projectile at " + position + " with passedTime " + passedTime);
        PredictedProjectile proj = Instantiate(projectilePrefab, position, Quaternion.identity);
        proj.Initialize(direction, passedTime, projectileLifetime);
    }

    // Instantiates a networked projectile on the server and calls Spawn() so it replicates to all clients.
    private void SpawnNetworkedProjectile(Vector3 position, Vector3 direction, float passedTime)
    {
        Debug.Log("[TestProjectileFirer] Spawning networked projectile at " + position + " with passedTime " + passedTime);
        PredictedProjectile proj = Instantiate(projectilePrefab, position, Quaternion.identity);
        proj.Initialize(direction, passedTime, projectileLifetime);

        NetworkObject netObj = proj.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            // Passing null for owner.
            Spawn(netObj, base.Owner);
            Debug.Log($"[TestProjectileFirer] NetworkObject.base.Owner ({base.Owner}) called on server.");
        }
        else
        {
            Debug.LogWarning("[TestProjectileFirer] No NetworkObject found on projectile prefab.");
        }
    }
}

using UnityEngine;
using FishNet.Object;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class DropPhysics : NetworkBehaviour
{
    [Header("Throw tuning")]
    [SerializeField] float forwardSpeed   =  6f;   // m/s away from player
    [SerializeField] float upwardSpeed    =  2f;   // initial loft
    [SerializeField] float spinStrength   = 12f;   // rad/s random tumble
    [SerializeField] float settleSpeed    =  0.05f;

    Rigidbody rb;
    Collider  col;
    PhysicsMaterial solidMat, triggerMat;

    void Awake()
    {
        rb  = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        // keep one solid and one trigger material reference if you need different friction
        solidMat   = col.sharedMaterial;
        triggerMat = solidMat; // same material is fine
    }

    /* called only on SERVER */
    public void ThrowFrom(Transform hand, Vector3 ownerVelocity)
    {
        // 1) place just in front of the hand so it starts in free space
        transform.position = hand.position + hand.forward * 0.35f + Vector3.up * 0.10f;
        transform.rotation = hand.rotation;

        // 2) activate physics
        rb.isKinematic            = false;
        rb.useGravity             = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation          = RigidbodyInterpolation.Interpolate;
        col.isTrigger             = false;
        col.sharedMaterial        = solidMat;

        // 3) give it velocity and spin
        rb.linearVelocity = ownerVelocity +
                      hand.forward * forwardSpeed +
                      Vector3.up   * upwardSpeed;

        rb.angularVelocity = Random.onUnitSphere * spinStrength;
    }

    void FixedUpdate()
    {
        if (!IsServer) return;   // authority only

        if (!rb.isKinematic &&
            rb.linearVelocity.sqrMagnitude < settleSpeed * settleSpeed)
        {
            // settle: turn into trigger so player can walk over it
            rb.isKinematic            = true;
            rb.useGravity             = false;
            col.isTrigger             = true;
            col.sharedMaterial        = triggerMat;
        }
    }
}

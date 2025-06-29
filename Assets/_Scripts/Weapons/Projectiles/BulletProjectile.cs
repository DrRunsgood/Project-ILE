// _Scripts/Weapons/Projectiles/BulletProjectile.cs
using UnityEngine;

public class BulletProjectile : BaseProjectile
{
    protected override void Awake()
    {
        base.Awake();
        _smoothVisual = false;      // disable interpolation
        gravAcc = Vector3.zero;     // no gravity drop
    }

    /* Ray-only sweep */
    protected override bool Sweep(Vector3 from, Vector3 to, out RaycastHit hit)
    {
        Vector3 dir = to - from;
        float len   = dir.magnitude;
        dir        /= len;
        return Physics.Raycast(from, dir, out hit, len, def.hitMask, QueryTriggerInteraction.Ignore);
    }

    /* Direct-hit only, no blast RPCs */
    protected override void Explode(Vector3 pos, Vector3 n, Collider col = null)
    {
        if (col != null) DealDamageAndKnockback(col, pos, n);
        DespawnSelf();
    }

    /* No LateUpdate cost */
    protected override void LateUpdate()
    {
        if (IsServer) return;
        transform.position = _next;
    }
}
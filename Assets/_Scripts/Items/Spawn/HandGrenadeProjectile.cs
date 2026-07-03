// _Scripts/Weapons/Projectiles/HandGrenadeProjectile.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public sealed class HandGrenadeProjectile : BaseProjectile
{
    /* ───── settings ─────────────────────────────────────────────── */
    [Header("Bounce")]
    [Range(0f, 1f)] [SerializeField] float bounciness  = 0.35f;   // 0 = no bounce
    [SerializeField] float stopSpeed   = 0.35f;                  // m/s ⇒ rest
    [SerializeField] float surfaceLift = 0.01f;                  // avoid z‑fighting

    [Header("Visual")]
    [SerializeField] float blastLift   = 0.08f;

    /* ───── runtime ──────────────────────────────────────────────── */
    private bool _sleeping;     // true once velocity drops below stopSpeed

    void OnEnable()          // runs every time the pooled object is spawned
    {
        _sleeping = false;   // reset sleep state
    }
    
    /* ───────────────────────────── */
    #region Server‑side simulation
    protected override void ServerTick()
    {
        if (_despawning || def == null) return;

        /* 1) fuse ---------------------------------------------------- */
        double age = (TimeManager.Tick - _spawnTick) * TimeManager.TickDelta;
        if (age >= def.lifeTime)
        {
            Explode(transform.position + Vector3.up * 0.05f, Vector3.up);
            return;
        }

        /* 2) no physics once asleep --------------------------------- */
        if (_sleeping) return;

        /* 3) integrate velocity ------------------------------------- */
        double dt = TimeManager.TickDelta;
        if (gravAcc != Vector3.zero)
            _velocity += gravAcc * (float)dt;

        Vector3 from  = transform.position;
        float   speed = _velocity.magnitude;

        if (speed <= 0f) return;

        Vector3 dir  = _velocity / speed;
        float   cast = speed * (float)dt + def.castRadius;

        if (Physics.SphereCast(from, def.castRadius, dir, out var hit, cast, def.hitMask,
                               QueryTriggerInteraction.Ignore))
        {
            /* bounce / slide / stop --------------------------------- */
            transform.position = hit.point + hit.normal * (def.castRadius + surfaceLift);

            _velocity = Vector3.Reflect(_velocity, hit.normal) * bounciness;

            if (_velocity.sqrMagnitude < stopSpeed * stopSpeed)
            {
                _velocity = Vector3.zero;
                _sleeping = true;           // grenade is at rest
            }
        }
        else
        {
            /* free flight ------------------------------------------- */
            transform.position = from + _velocity * (float)dt;
        }
    }
    #endregion

    /* ───────────────────────────── */
    #region Client‑side prediction
    protected override void ClientTick()
    {
        if (IsServer || _despawning)          return;
        if (TimeManager.Tick < _spawnTick)    return;   // RpcInit not yet received
        if (_sleeping)                        return;   // settled – no physics cost

        double dt = TimeManager.TickDelta;

        /* 0) gravity ------------------------------------------------ */
        if (gravAcc != Vector3.zero)
            _velocity += gravAcc * (float)dt;

        /* 1) kinematic sweep (mirrors ServerTick) ------------------- */
        Vector3 from  = transform.position;
        _prev = from;                         // interpolation start

        float   speed = _velocity.magnitude;
        if (speed > 0f)
        {
            Vector3 dir  = _velocity / speed;
            float   cast = speed * (float)dt + def.castRadius;

            if (Physics.SphereCast(from, def.castRadius, dir, out var hit, cast,
                                   def.hitMask, QueryTriggerInteraction.Ignore))
            {
                transform.position = hit.point + hit.normal * (def.castRadius + surfaceLift);
                _velocity = Vector3.Reflect(_velocity, hit.normal) * bounciness;

                if (_velocity.sqrMagnitude < stopSpeed * stopSpeed)
                {
                    _velocity = Vector3.zero;
                    _sleeping = true;
                }
            }
            else
            {
                transform.position = from + _velocity * (float)dt;
            }
        }

        /* 2) interpolation support --------------------------------- */
        _next   = transform.position;         // where we ended this tick
        _tickDt = (float)dt;
        _timer  = 0f;
    }
    #endregion

    /* ───────────────────────────── */
    #region Explosion overrides

    protected override void ApplyExplosion(Vector3 centre, Vector3 shotDir, Vector3 fallbackImpulseDir, Collider directHitCol)
    {
        /*
         * Grenade behavior differs from base projectile behavior:
         * direct-hit collider is allowed to receive damage/knockback even if
         * the overlap/LOS query would miss it.
         */
        if (directHitCol != null)
            DealDamageAndKnockback(directHitCol, centre, shotDir, fallbackImpulseDir);

        int cnt = Physics.OverlapSphereNonAlloc(centre, def.blastRadius, _buf, def.playerMask, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < cnt; ++i)
        {
            Collider c = _buf[i];

            if (c == null)
                continue;

            if (c == directHitCol)
                continue;

            if (!ClearLineOfSight(centre, c))
                continue;

            DealDamageAndKnockback(c, centre, shotDir, fallbackImpulseDir);
        }

        ClearBuffer(cnt);
    }

    protected override void Explode(Vector3 pos, Vector3 normal, Collider directHitCol = null)
    {
        Vector3 shotDir = _velocity.sqrMagnitude > 0.0001f
            ? _velocity.normalized
            : transform.forward;

        Vector3 fallbackImpulseDir = normal.sqrMagnitude > 0.0001f
            ? normal.normalized
            : -shotDir;

        if (fallbackImpulseDir.sqrMagnitude <= 0.0001f)
            fallbackImpulseDir = Vector3.up;

        ApplyExplosion(pos, shotDir, fallbackImpulseDir, directHitCol);

        RpcSpawnImpact(pos + fallbackImpulseDir * blastLift, fallbackImpulseDir);

        DespawnSelf();
    }

// Disable base sweep logic. Grenade bounces/settles instead of detonating on impact.
    protected override bool Sweep(Vector3 from, Vector3 to, out RaycastHit hit)
    {
        hit = default;
        return false;
    }

    #endregion
}

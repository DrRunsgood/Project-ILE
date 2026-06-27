// _Scripts/Player/PlayerHealth.cs
using System;
using System.Collections;
using _Scripts.Game;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using _Scripts.Player;    // AdvancedPredictedController
using _Scripts.Weapons;   // WeaponManager
using _Scripts.Packs;
using _Scripts.Game.CTF;
using _Scripts.Combat;
using _Scripts.Player.Sessions;

[DisallowMultipleComponent]
public sealed class PlayerHealth : NetworkBehaviour
{
    /* ───── designer knobs ───── */
    [SerializeField] int   maxHp        = 100;
    [SerializeField] float respawnDelay = 5f;

    /* ───── public read-only ─── */
    public int  Current => _hp.Value;
    public int  Max     => maxHp;
    public bool IsDead  => _hp.Value == 0;
    public bool IsAlive => !IsDead && _hp.Value > 0;
    
    public bool CanPickup => !IsDead;

    /* ───── gameplay events ──── */
    public event Action<int,int> OnHealthChanged;
    public event Action           OnDied;
    public event Action           OnRespawned;

    /* ───── authoritative HP ─── */
    readonly SyncVar<int> _hp = new();

    /* ───── cached refs ──────── */
    AdvancedPredictedController ctrl;
    WeaponManager               wm;
    PackManager                 pm;
    Rigidbody                   rb;
    Collider[]                  cols;
    Renderer[]                  rends;

    /* ───── Co-routines ──────── */
    Coroutine _healRoutine;
    
    /* ═════════════════════════ */
    #region Init
    void Awake()
    {
        _hp.OnChange += HpChanged;

        ctrl  = GetComponent<AdvancedPredictedController>();
        pm    = GetComponent<PackManager>();
        wm    = GetComponent<WeaponManager>();
        rb    = GetComponent<Rigidbody>();
        cols  = GetComponentsInChildren<Collider>(true);
        rends = GetComponentsInChildren<Renderer>(true);
    }
    void OnDestroy() => _hp.OnChange -= HpChanged;

    public override void OnStartServer()
    {
        base.OnStartServer();
        _hp.Value = maxHp;                     // first spawn
    }
    #endregion
    /* ═════════════════════════ */

    #region Server-side API
    [Server]
    public bool ApplyDamage(int dmg, NetworkObject instigator = null)
    {
        var info = new DamageInfo(
            amount: dmg,
            attacker: instigator,
            source: instigator,
            type: DamageType.Unknown,
            point: transform.position,
            normal: Vector3.up,
            impulse: Vector3.zero);

        return ApplyDamage(info).Applied;
    }

    [Server]
    public DamageResult ApplyDamage(in DamageInfo info)
    {
        int before = _hp.Value;

        if (IsDead)
            return DamageResult.Rejected(info, NetworkObject, before, DamageRejectReason.TargetDead);

        if (info.Amount <= 0)
            return DamageResult.Rejected(info, NetworkObject, before, DamageRejectReason.NonPositiveDamage);

        bool isSelfDamage =
            info.Attacker != null &&
            NetworkObject != null &&
            info.Attacker == NetworkObject;

        if (!isSelfDamage && GameModeManager.Instance != null && !GameModeManager.Instance.AllowTeamDamage
            && IsSameTeam(info.Attacker, NetworkObject))
        {
            return DamageResult.Rejected(info, NetworkObject, before, DamageRejectReason.BlockedByGameRules);
        }

        int rawDamage = info.Amount;
        int finalDamage = rawDamage;

        if (ctrl != null)
            finalDamage = ctrl.AbsorbDamageWithShield(rawDamage);

        int shieldAbsorbed = rawDamage - finalDamage;

        if (finalDamage <= 0)
        {
            return new DamageResult(
                applied: false,
                killed: false,
                rawDamage: rawDamage,
                finalDamage: 0,
                shieldAbsorbed: shieldAbsorbed,
                healthBefore: before,
                healthAfter: before,
                attacker: info.Attacker,
                victim: NetworkObject,
                type: info.Type,
                weaponId: info.WeaponId,
                rejectReason: DamageRejectReason.FullyAbsorbed);
        }

        int after = Mathf.Max(before - finalDamage, 0);
        _hp.Value = after;

        bool killed = after == 0;

        var result = new DamageResult(
            applied: true,
            killed: killed,
            rawDamage: rawDamage,
            finalDamage: finalDamage,
            shieldAbsorbed: shieldAbsorbed,
            healthBefore: before,
            healthAfter: after,
            attacker: info.Attacker,
            victim: NetworkObject,
            type: info.Type,
            weaponId: info.WeaponId,
            rejectReason: DamageRejectReason.None);

        if (killed)
            HandleDeath(info.Attacker, result);

        return result;
    }

    [Server] public void ApplyHeal(int amount) =>
        ApplyHealOverTime(amount, 0f);
    
    [Server] public void ApplyHealOverTime(int amount, float seconds)
    {
        if (IsDead || amount <= 0 || _hp.Value >= maxHp) return;

        /* cancel any previous HoT so the newest kit replaces it     */
        if (_healRoutine != null)
            StopCoroutine(_healRoutine);

        if (seconds <= 0f)
        {
            _hp.Value = Mathf.Min(_hp.Value + amount, maxHp);
        }
        else
        {
            _healRoutine = StartCoroutine(HealRoutine(amount, seconds));
        }
    }
    
    #endregion

    /* ───────────────────────── */
    void HpChanged(int prev, int next, bool asServer)
    {
        OnHealthChanged?.Invoke(next, maxHp);
        
        if (prev == 0 && next > 0)
            ctrl?.ResetEnergy();  
    }

    /* ─── death ───────────────── */
    [Server]
    void HandleDeath(NetworkObject killer)
    {
        HandleDeath(killer, default);
    }

    [Server]
    void HandleDeath(NetworkObject killer, DamageResult result)
    {
        SetPlayable(false);

        Debug.Log($"[PlayerHealth] {name} died. Checking for carried flag.");

        GetComponent<FlagCarrier>()?.Server_DropCarriedFlagOnDeath();

        wm?.DropAll();
        pm?.Server_Drop();

        ApplyAliveState(false);
        RpcSetAlive(false);

        OnDied?.Invoke();
        
        PlayerIdentity identity = GetComponent<PlayerIdentity>();

        if (PlayerSessionManager.Instance != null)
            PlayerSessionManager.Instance.ServerMarkDead(identity);

        if (result.Victim != null)
            GameModeManager.Instance?.NotifyPlayerDied(this, result);
        else
            GameModeManager.Instance?.NotifyPlayerDied(this, killer);

        float delay = GameModeManager.Instance != null
            ? GameModeManager.Instance.GetRespawnDelay(this)
            : respawnDelay;

        StartCoroutine(RespawnAfter(delay));
    }
    

    /* ─── respawn ─────────────── */
    [Server]
    IEnumerator RespawnAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Game mode decides if this player is allowed to respawn now.
        if (GameModeManager.Instance != null &&
            !GameModeManager.Instance.CanPlayerRespawn(this))
        {
            yield break;
        }

        RespawnNow();
    }
    
    [Server]
    public void RespawnNow()
    {
        SpawnManager.Instance?.TryMovePlayerToSpawn(NetworkObject);

        rb.isKinematic = false;

        ctrl?.ResetEnergy();

        _hp.Value = maxHp;

        SetPlayable(true);
        ApplyAliveState(true); // server-side physics/hitboxes
        RpcSetAlive(true);     // clients/observers

        GameModeManager.Instance?.NotifyPlayerRespawned(this);
        OnRespawned?.Invoke();
    }
    
    [Server] IEnumerator HealRoutine(int amount, float seconds)
    {
        float elapsed   = 0f;
        float perSecond = amount / seconds;
        float carry     = 0f;                    // fractional remainder

        while (elapsed < seconds && _hp.Value < maxHp && !IsDead)
        {
            yield return new WaitForSeconds((float)TimeManager.TickDelta);

            float dt = (float)TimeManager.TickDelta;
            elapsed += dt;

            carry += perSecond * dt;
            int inc = Mathf.FloorToInt(carry);
            if (inc > 0)
            {
                _hp.Value = Mathf.Min(_hp.Value + inc, maxHp);
                carry -= inc;
            }
        }
        _healRoutine = null;                     // finished / interrupted
    }

    /* ═════════ helpers ═════════ */
    void SetPlayable(bool yes)
    {
        if (ctrl) ctrl.IsFrozen = !yes;   // stops client-side prediction
        if (wm)   wm.enabled   = yes;     // block weapon input

        // colliders handled in RpcSetAlive for everyone
    }
    
    void ApplyAliveState(bool alive)
    {
        foreach (Renderer r in rends)
        {
            if (r)
                r.enabled = alive;
        }

        foreach (Collider c in cols)
        {
            if (c)
                c.enabled = alive;
        }
    }
    
    bool IsSameTeam(NetworkObject attacker, NetworkObject victim)
    {
        if (attacker == null || victim == null)
            return false;

        if (!attacker.TryGetComponent(out PlayerIdentity attackerIdentity))
            return false;

        if (!victim.TryGetComponent(out PlayerIdentity victimIdentity))
            return false;

        if (attackerIdentity.Team == _Scripts.Game.Teams.TeamId.None ||
            victimIdentity.Team == _Scripts.Game.Teams.TeamId.None)
        {
            return false;
        }

        return attackerIdentity.Team == victimIdentity.Team;
    }

    /* ---------- one tiny RPC toggles visuals & hitboxes everywhere ---------- */
    [ObserversRpc(BufferLast = false, ExcludeOwner = false)]
    void RpcSetAlive(bool alive)
    {
        ApplyAliveState(alive);

        if (IsOwner)
        {
            FpsCameraFollow cam = FindAnyObjectByType<FpsCameraFollow>();
            if (cam != null)
                cam.SetTargetAlive(alive);
        }
    }
}

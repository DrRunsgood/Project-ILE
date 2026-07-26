// _Scripts/Player/PlayerHealth.cs
using System;
using System.Collections;
using _Scripts.Game;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using _Scripts.Player;
using _Scripts.Weapons;
using _Scripts.Game.CTF;
using _Scripts.Combat;
using _Scripts.Player.Sessions;
using _Scripts.Game.Teams;

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
    
    public event Action<bool> OnClientAliveStateApplied;

    /* ───── authoritative HP ─── */
    readonly SyncVar<int> _hp = new();

    /* ───── cached refs ──────── */
    private AdvancedPredictedController ctrl;
    private WeaponManager wm;
    private Rigidbody rb;
    private FlagCarrier flagCarrier;
    private PlayerIdentity identity;
    private PlayerCarriedDropCoordinator carriedDropCoordinator;

    private Collider[] cols;
    private Renderer[] rends;

    /* ───── Co-routines ──────── */
    Coroutine _healRoutine;
    Coroutine _respawnRoutine;
    
    /* ═════════════════════════ */
    #region Init
    void Awake()
    {
        _hp.OnChange += HpChanged;

        ctrl = GetComponent<AdvancedPredictedController>();
        carriedDropCoordinator = GetComponent<PlayerCarriedDropCoordinator>();
        wm = GetComponent<WeaponManager>();
        rb = GetComponent<Rigidbody>();
        flagCarrier = GetComponent<FlagCarrier>();
        identity = GetComponent<PlayerIdentity>();
        cols = GetComponentsInChildren<Collider>(true);
        rends = GetComponentsInChildren<Renderer>(true);
    }
    void OnDestroy() => _hp.OnChange -= HpChanged;

    public override void OnStartServer()
    {
        base.OnStartServer();
        _hp.Value = maxHp;                     // first spawn
    }
    
    public override void OnStopServer()
    {
        CancelPendingRespawn();
        CancelPendingHeal();

        base.OnStopServer();
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

        if (!isSelfDamage && GameModeManager.Instance != null && !GameModeManager.Instance.AllowTeamDamage &&
            IsSameTeam(info.Attacker))
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
    }

    [Server]
    void HandleDeath(NetworkObject killer, DamageResult result)
    {
        SetPlayable(false);

        Debug.Log($"[PlayerHealth] {name} died. Checking for carried flag.");

        flagCarrier?.Server_DropCarriedFlagOnDeath();

        carriedDropCoordinator?.Server_DropForTerminalExit();

        ApplyAliveState(false);
        RpcSetAlive(false);

        OnDied?.Invoke();

        // Notify game mode first while the match is still Live.
        // This records K/D and kill feed before session eligibility ends the round.
        if (result.Victim != null)
            GameModeManager.Instance?.NotifyPlayerDied(this, result);
        else
            GameModeManager.Instance?.NotifyPlayerDied(this, killer);

        // Mark session dead after game mode has recorded the death.
        // This may trigger Arena elimination and move the round to PostRound.
        if (PlayerSessionManager.Instance != null)
        {
            PlayerSessionManager.Instance.ServerMarkDead(identity);
        }

        float delay = GameModeManager.Instance != null
            ? GameModeManager.Instance.GetRespawnDelay(this)
            : respawnDelay;

        CancelPendingHeal();
        CancelPendingRespawn();

        _respawnRoutine = StartCoroutine(RespawnAfter(delay));
    }
    

    /* ─── respawn ─────────────── */
    [Server]
    IEnumerator RespawnAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        /*
         * The coroutine has reached its terminal point. Clear the handle
         * before calling RespawnNow so RespawnNow does not try to stop the
         * coroutine currently executing.
         */
        _respawnRoutine = null;

        if (GameModeManager.Instance != null && !GameModeManager.Instance.CanPlayerRespawn(this))
            yield break;

        RespawnNow();
    }
    
    [Server]
    public void RespawnNow()
    {
        if (SpawnManager.Instance == null || !SpawnManager.Instance.TryMovePlayerToSpawn(NetworkObject))
        {
            Debug.LogError($"[PlayerHealth] Failed to respawn '{name}': no valid spawn was available.", this);
            return;
        }

        /*
         * A forced round/match respawn supersedes any previously scheduled
         * death respawn. Cancel it only after a valid spawn was obtained.
         */
        CancelPendingRespawn();

        rb.isKinematic = false;

        ctrl?.ResetEnergy();

        _hp.Value = maxHp;
        
        carriedDropCoordinator?.Server_ResetForNewLife();

        SetPlayable(true);
        ApplyAliveState(true);
        RpcSetAlive(true);

        if (PlayerSessionManager.Instance != null && Owner != null)
            PlayerSessionManager.Instance.ServerMarkSpawnedAlive(Owner);

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
    
    [Server]
    public void ServerSuicide()
    {
        if (IsDead)
            return;
        
        if (ctrl != null && ctrl.IsFrozen)
            return;

        int before = _hp.Value;
        int lethalDamage = Mathf.Max(before, maxHp);

        var info = new DamageInfo(
            amount: lethalDamage,
            attacker: NetworkObject,
            source: NetworkObject,
            type: DamageType.Suicide,
            point: transform.position,
            normal: Vector3.up,
            impulse: Vector3.zero);

        int after = 0;
        _hp.Value = after;

        var result = new DamageResult(
            applied: true,
            killed: true,
            rawDamage: lethalDamage,
            finalDamage: before,
            shieldAbsorbed: 0,
            healthBefore: before,
            healthAfter: after,
            attacker: NetworkObject,
            victim: NetworkObject,
            type: DamageType.Suicide,
            weaponId: 0,
            rejectReason: DamageRejectReason.None);

        HandleDeath(NetworkObject, result);
    }
    
    [Server]
    public void ServerApplyOutOfBoundsDamage(int amount)
    {
        if (IsDead)
            return;

        amount = Mathf.Max(0, amount);
        if (amount <= 0)
            return;

        int before = _hp.Value;
        int after = Mathf.Max(0, before - amount);

        _hp.Value = after;

        bool killed = after <= 0;

        var result = new DamageResult(
            applied: true,
            killed: killed,
            rawDamage: amount,
            finalDamage: before - after,
            shieldAbsorbed: 0,
            healthBefore: before,
            healthAfter: after,
            attacker: null,
            victim: NetworkObject,
            type: DamageType.OutOfBounds,
            weaponId: 0,
            rejectReason: DamageRejectReason.None);

        if (killed)
            HandleDeath(null, result);
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
    
    private bool IsSameTeam(NetworkObject attacker)
    {
        if (attacker == null || identity == null)
            return false;

        if (!attacker.TryGetComponent(out PlayerIdentity attackerIdentity))
            return false;

        TeamId attackerTeam = attackerIdentity.Team;

        TeamId victimTeam = identity.Team;

        if (attackerTeam == TeamId.None || victimTeam == TeamId.None)
            return false;

        return attackerTeam == victimTeam;
    }
    
    [Server]
    private void CancelPendingRespawn()
    {
        if (_respawnRoutine == null)
            return;

        StopCoroutine(_respawnRoutine);
        _respawnRoutine = null;
    }

    [Server]
    private void CancelPendingHeal()
    {
        if (_healRoutine == null)
            return;

        StopCoroutine(_healRoutine);
        _healRoutine = null;
    }

    /* ---------- one tiny RPC toggles visuals & hitboxes everywhere ---------- */
    [ObserversRpc(BufferLast = false, ExcludeOwner = false)]
    private void RpcSetAlive(bool alive)
    {
        ApplyAliveState(alive);

        OnClientAliveStateApplied?.Invoke(alive);
    }
}

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
    [Server] public void ApplyDamage(int dmg, NetworkObject instigator = null)
    {
        if (IsDead || dmg <= 0) return;
        
        // Check for shield absorb
        if (ctrl != null)
            dmg = ctrl.AbsorbDamageWithShield(dmg);  // may return 0
        if (dmg <= 0) return;                        // all soaked; early-out
        
        _hp.Value = Mathf.Max(_hp.Value - dmg, 0);
        if (_hp.Value == 0)
            HandleDeath(instigator);
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
    [Server] void HandleDeath(NetworkObject killer)
    {
        // Stop gameplay control, but do not freeze/stutter physics manually.
        SetPlayable(false);

        // Drop held weapons and pack.
        wm?.DropAll();
        pm?.Server_Drop();

        // Hide player / disable hitboxes.
        RpcSetAlive(false);

        // Later: play death explosion/VFX here.
        // RpcPlayDeathFx(transform.position);

        OnDied?.Invoke();

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
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        ctrl?.HardResetMovement();
        ctrl?.ResetEnergy();

        _hp.Value = maxHp;

        SetPlayable(true);
        RpcSetAlive(true);

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

    /* ---------- one tiny RPC toggles visuals & hitboxes everywhere ---------- */
    [ObserversRpc(BufferLast = false, ExcludeOwner = false)]
    void RpcSetAlive(bool alive)
    {
        foreach (var r in rends) r.enabled = alive;
        foreach (var c in cols)  c.enabled = alive;
        
        if (IsOwner)
        {
            FpsCameraFollow cam = FindFirstObjectByType<FpsCameraFollow>();
            if (cam != null)
                cam.SetTargetAlive(alive);
        }
    }
}

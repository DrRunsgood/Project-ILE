// _Scripts/Player/PlayerHealth.cs
using System;
using System.Collections;
using _Scripts.Packs;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using _Scripts.Player;    // AdvancedPredictedController
using _Scripts.Weapons;   // WeaponManager

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

    [Server] public void ApplyHeal(int amount)
    {
        if (IsDead || amount <= 0) return;
        _hp.Value = Mathf.Min(_hp.Value + amount, maxHp);
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
        // 1) freeze authoritative physics
        rb.isKinematic      = true;
        rb.linearVelocity   = Vector3.zero;
        rb.angularVelocity  = Vector3.zero;

        // 2) disable control + visuals + colliders for every peer
        RpcSetAlive(false);
        SetPlayable(false);          // owner logic only

        // 3) drop held weapons and pack
        wm?.DropAll();
        pm?.Server_Drop();

        OnDied?.Invoke();

        // 4) schedule respawn
        StartCoroutine(RespawnAfter(respawnDelay));
    }

    /* ─── respawn ─────────────── */
    [Server] IEnumerator RespawnAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        // teleport to spawn
        Transform sp = SpawnManager.Instance?.GetRandomSpawn();
        if (sp != null)
            transform.SetPositionAndRotation(sp.position, sp.rotation);

        // full physics + gameplay reset
        rb.isKinematic      = false;
        rb.linearVelocity   = Vector3.zero;
        rb.angularVelocity  = Vector3.zero;
        ctrl?.HardResetMovement();
        ctrl?.ResetEnergy();

        _hp.Value = maxHp;           // fires Ui callback
        
        SetPlayable(true);
        RpcSetAlive(true);
        OnRespawned?.Invoke();
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
    }
}

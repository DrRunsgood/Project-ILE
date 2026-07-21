using System.Collections.Generic;
using UnityEngine;
using _Scripts.Player;

[DisallowMultipleComponent]
public sealed class PlayerIFFTarget : MonoBehaviour
{
    private static readonly List<PlayerIFFTarget> _activeTargets = new();

    public static IReadOnlyList<PlayerIFFTarget> ActiveTargets =>
        _activeTargets;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        _activeTargets.Clear();
    }
    
    
    [SerializeField]
    private Transform iffAnchor;

    public Transform Anchor => iffAnchor != null ? iffAnchor : transform;

    public PlayerIdentity Identity { get; private set; }

    public PlayerHealth Health { get; private set; }

    public PlayerPresentation Presentation { get; private set; }

    /*
     * Start ready so normal initial player spawning does not depend
     * on receiving a respawn-only RPC.
     */
    private bool _clientAliveApplied = true;
    private bool _presentationPoseReady = true;

    public bool CanShowIFF => _clientAliveApplied && _presentationPoseReady && (Health == null || !Health.IsDead);

    private void Awake()
    {
        Identity = GetComponent<PlayerIdentity>();

        Health = GetComponent<PlayerHealth>();

        Presentation = GetComponent<PlayerPresentation>();
    }

    private void OnEnable()
    {
        if (!_activeTargets.Contains(this)) _activeTargets.Add(this);

        _clientAliveApplied = true;
        _presentationPoseReady = true;

        if (Health != null)
            Health.OnClientAliveStateApplied += HandleClientAliveStateApplied;
        

        if (Presentation != null)
        {
            Presentation.OnPresentationPoseResetStarted += HandlePresentationPoseResetStarted;

            Presentation.OnPresentationPoseResetApplied += HandlePresentationPoseResetApplied;
        }
    }

    private void OnDisable()
    {
        _activeTargets.Remove(this);

        if (Health != null)
            Health.OnClientAliveStateApplied -= HandleClientAliveStateApplied;
        

        if (Presentation != null)
        {
            Presentation.OnPresentationPoseResetStarted -= HandlePresentationPoseResetStarted;

            Presentation.OnPresentationPoseResetApplied -= HandlePresentationPoseResetApplied;
        }
    }

    private void HandleClientAliveStateApplied(bool alive)
    {
        _clientAliveApplied = alive;

        if (!alive)
        {
            /*
             * A future respawn must receive a fresh presentation reset
             * before its IFF may become visible again.
             */
            _presentationPoseReady = false;
        }
    }

    private void HandlePresentationPoseResetApplied()
    {
        _presentationPoseReady = true;
    }
    
    private void HandlePresentationPoseResetStarted()
    {
        _presentationPoseReady = false;
    }
}
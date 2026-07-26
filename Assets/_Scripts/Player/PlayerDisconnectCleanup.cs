using System;
using System.Collections;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;
using _Scripts.Game.CTF;
using _Scripts.Player.Sessions;

namespace _Scripts.Player
{
    [RequireComponent(typeof(PlayerCarriedDropCoordinator))]
    public sealed class PlayerDisconnectCleanup : NetworkBehaviour
    {
        [SerializeField]
        private float fallbackDisconnectDelay = 0.35f;

        [SerializeField]
        private float disconnectAfterCleanupDelay = 0.15f;

        private PlayerCarriedDropCoordinator _dropCoordinator;
        private FlagCarrier _flagCarrier;

        private Action _pendingClientCallback;
        private bool _cleanupRequested;
        private bool _serverCleanupProcessed;

        private void Awake()
        {
            _dropCoordinator = GetComponent<PlayerCarriedDropCoordinator>();

            _flagCarrier = GetComponent<FlagCarrier>();
        }
        
        public override void OnStartServer()
        {
            base.OnStartServer();

            _serverCleanupProcessed = false;
        }

        public bool TryBeginGracefulDisconnect(Action onCleanupComplete)
        {
            if (!IsOwner)
                return false;

            if (_cleanupRequested)
                return false;

            _cleanupRequested = true;
            _pendingClientCallback = onCleanupComplete;

            Debug.Log("[PlayerDisconnectCleanup] Requesting server disconnect cleanup.");

            Server_RequestDisconnectCleanup();
            StartCoroutine(ClientFallbackComplete());

            return true;
        }

        private IEnumerator ClientFallbackComplete()
        {
            yield return new WaitForSeconds(fallbackDisconnectDelay);

            if (!_cleanupRequested)
                yield break;

            Debug.LogWarning("[PlayerDisconnectCleanup] Cleanup ack fallback elapsed. Continuing disconnect.");

            CompleteClientCleanup();
        }

        [ServerRpc(RequireOwnership = true)]
        private void Server_RequestDisconnectCleanup(NetworkConnection conn = null)
        {
            ServerPrepareForDisconnect();

            if (conn != null)
                Target_DisconnectCleanupComplete(conn);
            
        }

        [Server]
        public void ServerPrepareForDisconnect()
        {
            if (_serverCleanupProcessed)
                return;

            _serverCleanupProcessed = true;

            Debug.Log($"[PlayerDisconnectCleanup] Server cleanup for {name}.");

            if (_dropCoordinator == null)
                _dropCoordinator = GetComponent<PlayerCarriedDropCoordinator>();
            
            if (_flagCarrier == null)
                _flagCarrier = GetComponent<FlagCarrier>();
            
            _flagCarrier?.Server_DropCarriedFlagOnDeath();

            _dropCoordinator?.Server_DropForTerminalExit();

            if (PlayerSessionManager.Instance != null && Owner != null)
                PlayerSessionManager.Instance.ServerMarkDead(Owner);
            
        }

        [TargetRpc]
        private void Target_DisconnectCleanupComplete(NetworkConnection conn)
        {
            Debug.Log("[PlayerDisconnectCleanup] Server cleanup ack received.");

            CompleteClientCleanup();
        }

        private void CompleteClientCleanup()
        {
            if (!_cleanupRequested)
                return;

            _cleanupRequested = false;

            StartCoroutine(CompleteClientCleanupDelayed());
        }

        private IEnumerator CompleteClientCleanupDelayed()
        {
            yield return new WaitForSeconds(disconnectAfterCleanupDelay);

            Action callback = _pendingClientCallback;
            _pendingClientCallback = null;

            callback?.Invoke();
        }
    }
}
using System;
using System.Collections;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;
using _Scripts.Weapons;
using _Scripts.Player.Sessions;

namespace _Scripts.Player
{
    [RequireComponent(typeof(WeaponManager))]
    public sealed class PlayerDisconnectCleanup : NetworkBehaviour
    {
        [SerializeField] private float fallbackDisconnectDelay = 0.35f;
        [SerializeField] private float disconnectAfterCleanupDelay = 0.15f;

        private WeaponManager _weaponManager;
        private Action _pendingClientCallback;
        private bool _cleanupRequested;

        private void Awake()
        {
            _weaponManager = GetComponent<WeaponManager>();
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
            Debug.Log($"[PlayerDisconnectCleanup] Server cleanup for {name}.");

            if (_weaponManager == null)
                _weaponManager = GetComponent<WeaponManager>();

            if (_weaponManager != null)
                _weaponManager.DropAll();
            
            if (PlayerSessionManager.Instance != null && Owner != null)
                PlayerSessionManager.Instance.ServerMarkDead(Owner);

            // TODO later:
            // - Drop carried flag.
            // - Clear active pack/shield state.
            // - Drop/destroy carried deployables or inventory items as design dictates.
            // - Route forced disconnect/death through a unified server player-exit cleanup path.
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
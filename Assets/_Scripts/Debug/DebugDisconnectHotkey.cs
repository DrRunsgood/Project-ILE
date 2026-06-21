using _Scripts.Networking;
using UnityEngine;

namespace _Scripts.DebugTools
{
    public sealed class DebugDisconnectHotkey : MonoBehaviour
    {
        [SerializeField] private NetworkSessionManager networkSessionManager;
        [SerializeField] private KeyCode disconnectKey = KeyCode.F10;

        private void Awake()
        {
            if (networkSessionManager == null)
                networkSessionManager = FindAnyObjectByType<NetworkSessionManager>();
        }

        private void Update()
        {
            if (!Input.GetKeyDown(disconnectKey))
                return;

            if (networkSessionManager == null)
            {
                networkSessionManager = FindAnyObjectByType<NetworkSessionManager>();

                if (networkSessionManager == null)
                {
                    Debug.LogError("[DebugDisconnectHotkey] Missing NetworkSessionManager.");
                    return;
                }
            }

            Debug.Log("[DebugDisconnectHotkey] F10 pressed. Disconnecting client.");
            networkSessionManager.DisconnectClient();
        }
    }
}
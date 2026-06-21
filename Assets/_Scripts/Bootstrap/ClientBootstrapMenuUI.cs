using _Scripts.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Bootstrap
{
    public sealed class ClientBootstrapMenuUI : MonoBehaviour
    {
        [Header("Session")]
        [SerializeField] private NetworkSessionManager networkSessionManager;

        [Header("UI")]
        [SerializeField] private TMP_InputField displayNameInput;
        [SerializeField] private TMP_InputField addressInput;
        [SerializeField] private TMP_InputField portInput;
        [SerializeField] private Button joinButton;
        [SerializeField] private TMP_Text statusText;

        private bool _subscribed;
        
        
        private void OnEnable()
        {
            if (networkSessionManager == null)
                networkSessionManager = FindAnyObjectByType<NetworkSessionManager>();

            if (networkSessionManager != null && !_subscribed)
            {
                networkSessionManager.OnStateChanged += HandleSessionStateChanged;
                _subscribed = true;

                RefreshForState(networkSessionManager.CurrentState);
            }
        }

        private void OnDisable()
        {
            if (networkSessionManager != null && _subscribed)
            {
                networkSessionManager.OnStateChanged -= HandleSessionStateChanged;
                _subscribed = false;
            }
        }

        private void HandleSessionStateChanged(NetworkSessionState previous, NetworkSessionState next)
        {
            RefreshForState(next);
        }

        private void RefreshForState(NetworkSessionState state)
        {
            switch (state)
            {
                case NetworkSessionState.Offline:
                case NetworkSessionState.ClientMenu:
                case NetworkSessionState.Failed:
                    if (joinButton != null)
                        joinButton.interactable = true;

                    if (state == NetworkSessionState.Failed)
                        SetStatus("Connection failed. Check address/port and try again.");
                    else
                        SetStatus("Enter name and server address.");
                    break;

                case NetworkSessionState.Connecting:
                    if (joinButton != null)
                        joinButton.interactable = false;

                    SetStatus("Connecting...");
                    break;

                case NetworkSessionState.Connected:
                case NetworkSessionState.LoadingGameplay:
                    if (joinButton != null)
                        joinButton.interactable = false;

                    SetStatus("Loading game...");
                    break;

                case NetworkSessionState.InGame:
                    if (joinButton != null)
                        joinButton.interactable = false;

                    SetStatus("In game.");
                    break;

                case NetworkSessionState.Disconnecting:
                    if (joinButton != null)
                        joinButton.interactable = false;

                    SetStatus("Disconnecting...");
                    break;
                // server states
                case NetworkSessionState.ServerStarting:
                case NetworkSessionState.ServerLoadingGameplay:
                case NetworkSessionState.ServerRunning:
                case NetworkSessionState.ServerStopping:
                    if (joinButton != null)
                        joinButton.interactable = false;

                    SetStatus("Server mode active.");
                    break;
            }
        }
        
        private void Awake()
        {
            if (networkSessionManager == null)
                networkSessionManager = FindAnyObjectByType<NetworkSessionManager>();

            if (displayNameInput != null)
                displayNameInput.text = ClientBootstrapSettings.DisplayName;

            if (addressInput != null)
                addressInput.text = ClientBootstrapSettings.ServerAddress;

            if (portInput != null)
                portInput.text = ClientBootstrapSettings.ServerPort.ToString();

            if (joinButton != null)
                joinButton.onClick.AddListener(JoinServer);

            SetStatus("Enter name and server address.");
        }

        private void OnDestroy()
        {
            if (joinButton != null)
                joinButton.onClick.RemoveListener(JoinServer);
        }

        public void JoinServer()
        {
            SaveInputs();

            if (networkSessionManager == null)
            {
                SetStatus("Missing NetworkSessionManager.");
                Debug.LogError("[ClientBootstrapMenuUI] Missing NetworkSessionManager.");
                return;
            }

            string address = ClientBootstrapSettings.ServerAddress;
            ushort port = ClientBootstrapSettings.ServerPort;

            SetStatus($"Connecting to {address}:{port}...");

            if (joinButton != null)
                joinButton.interactable = false;

            bool started = networkSessionManager.StartClient(address, port);

            if (!started)
            {
                SetStatus("Client start failed.");

                if (joinButton != null)
                    joinButton.interactable = true;

                return;
            }

            SetStatus($"Connecting to {address}:{port}...");
        }

        private void SaveInputs()
        {
            if (displayNameInput != null)
                ClientBootstrapSettings.DisplayName = displayNameInput.text;

            if (addressInput != null)
                ClientBootstrapSettings.ServerAddress = addressInput.text;

            if (portInput != null && ushort.TryParse(portInput.text, out ushort parsedPort))
            {
                ClientBootstrapSettings.ServerPort = parsedPort;
            }
            else if (portInput != null)
            {
                portInput.text = ClientBootstrapSettings.ServerPort.ToString();
            }
        }

        private void SetStatus(string value)
        {
            if (statusText != null)
                statusText.text = value;
        }
    }
}
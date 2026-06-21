using FishNet;
using FishNet.Managing;
using FishNet.Transporting.Tugboat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Bootstrap
{
    public sealed class ClientBootstrapMenuUI : MonoBehaviour
    {
        [Header("FishNet")]
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private Tugboat tugboat;

        [Header("UI")]
        [SerializeField] private TMP_InputField displayNameInput;
        [SerializeField] private TMP_InputField addressInput;
        [SerializeField] private TMP_InputField portInput;
        [SerializeField] private Button joinButton;
        [SerializeField] private TMP_Text statusText;

        [Header("Options")]
        [SerializeField] private bool hideAfterJoinClicked = false;

        private void Awake()
        {
            if (networkManager == null)
                networkManager = InstanceFinder.NetworkManager;

            if (tugboat == null && networkManager != null)
                tugboat = networkManager.GetComponent<Tugboat>();

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

            if (networkManager == null)
            {
                SetStatus("Missing NetworkManager.");
                Debug.LogError("[ClientBootstrapMenuUI] Missing NetworkManager.");
                return;
            }

            ApplyTugboatAddress();

            SetStatus($"Connecting to {ClientBootstrapSettings.ServerAddress}:{ClientBootstrapSettings.ServerPort}...");

            bool started = networkManager.ClientManager.StartConnection();

            if (!started)
            {
                SetStatus("Client start failed.");
                return;
            }

            if (hideAfterJoinClicked)
                gameObject.SetActive(false);
        }

        private void SaveInputs()
        {
            if (displayNameInput != null)
                ClientBootstrapSettings.DisplayName = displayNameInput.text;

            if (addressInput != null)
                ClientBootstrapSettings.ServerAddress = addressInput.text;

            if (portInput != null && ushort.TryParse(portInput.text, out ushort parsedPort))
                ClientBootstrapSettings.ServerPort = parsedPort;
        }

        private void ApplyTugboatAddress()
        {
            if (tugboat == null)
            {
                Debug.LogWarning("[ClientBootstrapMenuUI] Tugboat reference missing. Using transport defaults.");
                return;
            }

            tugboat.SetClientAddress(ClientBootstrapSettings.ServerAddress);
            tugboat.SetPort(ClientBootstrapSettings.ServerPort);
        }

        private void SetStatus(string value)
        {
            if (statusText != null)
                statusText.text = value;
        }
    }
}
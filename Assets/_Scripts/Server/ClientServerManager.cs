using System.Collections;
using FishNet;
using UnityEngine;

public class ClientServerManager : MonoBehaviour
{
    [Header("Mode")]
    [SerializeField] private bool isServer;

    [Header("Client Bootstrap")]
    [SerializeField] private GameObject clientBootstrapRoot;
    [SerializeField] private Camera clientBootstrapCamera;

    void Awake()
    {
        if (isServer)
        {
            Debug.Log("[ClientServerManager] Starting server mode.");

            SetClientBootstrapVisible(false);

            StripGraphicsOnce();
            InstanceFinder.ServerManager.StartConnection();
            StartCoroutine(StripGraphicsLatePass());

            return;
        }

        Debug.Log("[ClientServerManager] Starting client mode. Waiting for bootstrap join.");

        SetClientBootstrapVisible(true);
    }

    public void SetClientBootstrapVisible(bool visible)
    {
        if (clientBootstrapRoot == null)
        {
            Debug.LogWarning("[ClientServerManager] ClientBootstrapRoot is not assigned.");
            return;
        }

        if (clientBootstrapRoot.name == "_Scene")
        {
            Debug.LogError("[ClientServerManager] Refusing to toggle _Scene. ClientBootstrapRoot is assigned incorrectly.");
            return;
        }

        if (clientBootstrapRoot.transform.parent == null)
        {
            Debug.LogError($"[ClientServerManager] Refusing to toggle root object '{clientBootstrapRoot.name}'. Assign _Bootstrap/ClientBootstrap instead.");
            return;
        }

        clientBootstrapRoot.SetActive(visible);

        if (clientBootstrapCamera != null)
        {
            clientBootstrapCamera.gameObject.SetActive(visible);
            clientBootstrapCamera.enabled = visible;
        }
    }

    void StripGraphicsOnce()
    {
        Camera[] cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);

        foreach (Camera cam in cameras)
        {
            if (cam == clientBootstrapCamera)
                continue;

            cam.gameObject.SetActive(false);
        }

        AudioListener[] listeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);

        foreach (AudioListener listener in listeners)
            listener.enabled = false;
    }

    IEnumerator StripGraphicsLatePass()
    {
        yield return null;
        StripGraphicsOnce();
    }
}
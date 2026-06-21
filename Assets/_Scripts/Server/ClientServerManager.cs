using System.Collections;
using FishNet;
using FishNet.Managing.Scened;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-10000)]
public class ClientServerManager : MonoBehaviour
{
    [Header("Mode")]
    [SerializeField] private bool isServer;

    [Header("Server Startup")]
    [SerializeField] private bool loadDefaultGameplaySceneOnServerStart = true;
    [SerializeField] private string defaultGameplaySceneName = "Arena_TestMap_01";

    [Header("Client Bootstrap")]
    [SerializeField] private GameObject clientBootstrapRoot;
    [SerializeField] private Camera menuCamera;

    [Header("Server-only graphics strip")]
    [SerializeField] private int serverQualityLevel = 0;
    [SerializeField] private int serverTargetFps = 60;
    [SerializeField] private bool disableEnviro = true;
    [SerializeField] private bool disableVolumes = true;
    [SerializeField] private bool disableCameras = true;
    [SerializeField] private bool disableAudio = true;

    private bool _serverSceneLoadStarted;

    private void Awake()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += HandleUnitySceneLoaded;

        if (isServer)
        {
            Debug.Log("[ClientServerManager] Starting server mode from BootScene.");

            SetClientBootstrapVisible(false);

            StripGraphicsOnce();

            InstanceFinder.ServerManager.StartConnection();

            if (loadDefaultGameplaySceneOnServerStart)
                StartCoroutine(LoadDefaultGameplaySceneAfterServerStart());

            StartCoroutine(StripGraphicsLatePass());

            return;
        }

        Debug.Log("[ClientServerManager] Starting client mode from BootScene. Waiting for bootstrap join.");

        SetClientBootstrapVisible(true);
    }

    private void OnDestroy()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= HandleUnitySceneLoaded;
    }

    public void SetClientBootstrapVisible(bool visible)
    {
        if (clientBootstrapRoot != null)
        {
            if (IsDangerousRoot(clientBootstrapRoot))
            {
                Debug.LogError(
                    $"[ClientServerManager] Refusing to toggle dangerous root '{clientBootstrapRoot.name}'. " +
                    "Assign the specific ClientBootstrap object instead.");
            }
            else
            {
                clientBootstrapRoot.SetActive(visible);
            }
        }

        if (menuCamera != null)
        {
            menuCamera.gameObject.SetActive(visible);
            menuCamera.enabled = visible;
        }
    }

    private bool IsDangerousRoot(GameObject target)
    {
        if (target == null)
            return true;

        string targetName = target.name;

        return targetName == "_Scene" ||
               targetName == "NetworkManager" ||
               targetName == "AppRoot" ||
               targetName == "MapMagic";
    }

    private IEnumerator LoadDefaultGameplaySceneAfterServerStart()
    {
        if (_serverSceneLoadStarted)
            yield break;

        _serverSceneLoadStarted = true;

        yield return null;

        if (string.IsNullOrWhiteSpace(defaultGameplaySceneName))
        {
            Debug.LogError("[ClientServerManager] Default gameplay scene name is empty.");
            yield break;
        }

        Debug.Log($"[ClientServerManager] Loading server gameplay scene: {defaultGameplaySceneName}");

        SceneLoadData sceneLoadData = new SceneLoadData(defaultGameplaySceneName);
        InstanceFinder.SceneManager.LoadGlobalScenes(sceneLoadData);

        // Server graphics need another pass after the gameplay scene loads.
        StartCoroutine(StripGraphicsLatePass());
    }

    private void HandleUnitySceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!isServer)
            return;

        Debug.Log($"[ClientServerManager] Unity scene loaded on server: {scene.name}. Re-stripping graphics.");
        StartCoroutine(StripGraphicsLatePass());
    }

    private void StripGraphicsOnce()
    {
        QualitySettings.vSyncCount = 0;

        if (serverQualityLevel >= 0 && serverQualityLevel < QualitySettings.names.Length)
            QualitySettings.SetQualityLevel(serverQualityLevel, applyExpensiveChanges: true);

        Application.targetFrameRate = serverTargetFps;

        if (disableVolumes)
        {
            foreach (Volume vol in FindObjectsByType<Volume>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                vol.enabled = false;
        }

        if (disableEnviro)
        {
            GameObject enviroGO = GameObject.Find("Enviro 3");

            if (enviroGO != null)
            {
                enviroGO.SetActive(false);
                Debug.Log("[ClientServerManager] Enviro found - setting inactive.");
            }
        }

        if (disableCameras)
        {
            foreach (Camera cam in FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                UniversalAdditionalCameraData urp = cam.GetUniversalAdditionalCameraData();

                if (urp != null)
                {
                    urp.renderPostProcessing = false;
                    urp.antialiasing = AntialiasingMode.None;
                    urp.renderShadows = false;
                }

                cam.enabled = false;
                cam.gameObject.SetActive(false);
            }
        }

        if (disableAudio)
        {
            foreach (AudioListener listener in FindObjectsByType<AudioListener>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                listener.enabled = false;

            AudioListener.volume = 0f;
        }

        QualitySettings.shadowDistance = 0f;
    }

    private IEnumerator StripGraphicsLatePass()
    {
        yield return null;
        StripGraphicsOnce();

        yield return new WaitForSeconds(0.2f);
        StripGraphicsOnce();
    }
}
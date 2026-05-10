using System.Collections;
using FishNet;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Single toggle to run as Server or Client in-editor.
/// Also strips ALL rendering/post for the server instance so it behaves like headless.
/// </summary>
[DefaultExecutionOrder(-10000)] // run as early as possible
public class ClientServerManager : MonoBehaviour
{
    [SerializeField] private bool isServer;

    [Header("Server-only graphics strip")]
    [SerializeField] int  serverQualityLevel = 0;   // 0 = Very Low
    [SerializeField] int  serverTargetFps    = 60;  // cap editor server
    [SerializeField] bool disableEnviro      = true;
    [SerializeField] bool disableVolumes     = true;
    [SerializeField] bool disableCameras     = true;
    [SerializeField] bool disableAudio       = true;

    void Awake()
    {
        if (isServer)
        {
            StripGraphicsOnce();
            // Start as server AFTER we’ve cut graphics.
            InstanceFinder.ServerManager.StartConnection();
            // Catch any systems that re-enabled themselves in Start()
            StartCoroutine(StripGraphicsLatePass());
        }
        else
        {
            // Client
            InstanceFinder.ClientManager.StartConnection();
        }
    }

    void StripGraphicsOnce()
    {
        // Quality + timing
        QualitySettings.vSyncCount = 0;
        if (serverQualityLevel >= 0 && serverQualityLevel < QualitySettings.names.Length)
            QualitySettings.SetQualityLevel(serverQualityLevel, applyExpensiveChanges: true);
        Application.targetFrameRate = serverTargetFps;

        // Kill post-processing volumes (URP)
        if (disableVolumes)
        {
            foreach (var vol in FindObjectsOfType<Volume>(true))
                vol.enabled = false;
        }

        // Kill Enviro (adjust to your exact types/names if you know them)
        if (disableEnviro)
        {
            // If you know Enviro’s core component type, replace this with FindObjectOfType<EnviroSkyMgr>(true)
            var enviroGO = GameObject.Find("Enviro 3");
            if (enviroGO)
            {
                enviroGO.SetActive(false);
                Debug.Log("Enviro Found - Setting inactive");
            }
        }

        // Kill cameras (and URP extras)
        if (disableCameras)
        {
            foreach (var cam in FindObjectsOfType<Camera>(true))
            {
                var urp = cam.GetUniversalAdditionalCameraData();
                if (urp != null)
                {
                    urp.renderPostProcessing = false;
                    urp.antialiasing         = AntialiasingMode.None;
                    urp.renderShadows        = false;
                }
                cam.enabled = false;
            }
        }

        // Kill audio
        if (disableAudio)
        {
            foreach (var al in FindObjectsOfType<AudioListener>(true))
                al.enabled = false;
            AudioListener.volume = 0f;
        }

        // Extra safety: shadows off
        QualitySettings.shadowDistance = 0f;
    }

    IEnumerator StripGraphicsLatePass()
    {
        // Let any Start() initializers run, then re-strip.
        yield return null;          // next frame
        StripGraphicsOnce();
        yield return new WaitForSeconds(0.2f);
        StripGraphicsOnce();
    }
}

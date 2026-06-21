using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace _Scripts.Bootstrap
{
    public sealed class DedicatedServerGraphicsStripper : MonoBehaviour
    {
        [Header("Server-only graphics strip")]
        [SerializeField] private int serverQualityLevel = 0;
        [SerializeField] private int serverTargetFps = 60;
        [SerializeField] private bool disableEnviro = true;
        [SerializeField] private bool disableVolumes = true;
        [SerializeField] private bool disableCameras = true;
        [SerializeField] private bool disableAudio = true;
        [SerializeField] private bool stripAgainWhenSceneLoads = true;

        private bool _isActiveForServer;

        public void ActivateForServer()
        {
            if (_isActiveForServer)
                return;

            _isActiveForServer = true;

            if (stripAgainWhenSceneLoads)
                SceneManager.sceneLoaded += HandleSceneLoaded;

            StripGraphicsOnce();
            StartCoroutine(StripGraphicsLatePass());
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!_isActiveForServer)
                return;

            Debug.Log($"[DedicatedServerGraphicsStripper] Scene loaded: {scene.name}. Re-stripping graphics.");
            StartCoroutine(StripGraphicsLatePass());
        }

        public void StripGraphicsOnce()
        {
            QualitySettings.vSyncCount = 0;

            if (serverQualityLevel >= 0 && serverQualityLevel < QualitySettings.names.Length)
                QualitySettings.SetQualityLevel(serverQualityLevel, applyExpensiveChanges: true);

            Application.targetFrameRate = serverTargetFps;

            if (disableVolumes)
            {
                foreach (Volume volume in FindObjectsByType<Volume>(FindObjectsInactive.Include))
                    volume.enabled = false;
            }

            if (disableEnviro)
            {
                GameObject enviroGO = GameObject.Find("Enviro 3");

                if (enviroGO != null)
                {
                    enviroGO.SetActive(false);
                    Debug.Log("[DedicatedServerGraphicsStripper] Enviro found - setting inactive.");
                }
            }

            if (disableCameras)
            {
                foreach (Camera camera in FindObjectsByType<Camera>(FindObjectsInactive.Include))
                {
                    UniversalAdditionalCameraData urp = camera.GetUniversalAdditionalCameraData();

                    if (urp != null)
                    {
                        urp.renderPostProcessing = false;
                        urp.antialiasing = AntialiasingMode.None;
                        urp.renderShadows = false;
                    }

                    camera.enabled = false;
                    camera.gameObject.SetActive(false);
                }
            }

            if (disableAudio)
            {
                foreach (AudioListener listener in FindObjectsByType<AudioListener>(FindObjectsInactive.Include))
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
}
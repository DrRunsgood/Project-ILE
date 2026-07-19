using FishNet.Object;
using UnityEngine;
using FishNet.Component.Transforming.Beta;
using System;

namespace _Scripts.Player
{
    [DisallowMultipleComponent]
    public sealed class PlayerPresentation : NetworkBehaviour
    {
        [Header("Presentation Roots")]

        [Tooltip("Visible third-person body and its visual attachments.")]
        [SerializeField]
        private GameObject worldModelRoot;

        [Tooltip("Third-person aim pivot and its presentation-only weapon anchor.")]
        [SerializeField]
        private GameObject thirdPersonPresentationRoot;
        
        [Header("Render Smoothers")]

        [Tooltip("Render-position proxy followed by the local gameplay camera.")]
        [SerializeField]
        private NetworkTickSmoother cameraFollowSmoother;

        [Tooltip("Render-smoothed third-person body/presentation root.")]
        [SerializeField]
        private NetworkTickSmoother graphicsRootSmoother;

        [Header("Layers")]

        [SerializeField]
        private string worldPlayerLayerName = "Player";

        [SerializeField]
        private string ownerThirdPersonLayerName = "TP_Only";
        
        [Header("Observer Reset")]

        [Tooltip(
            "How close the remote gameplay root must be to the reset position " +
            "before presentation smoothing resumes.")]
        [SerializeField]
        private float observerResetArrivalTolerance = 2f;

        private bool _observerResetPending;
        private byte _pendingObserverResetSequence;
        private Vector3 _pendingObserverResetPosition;
        private Quaternion _pendingObserverResetRotation;

        private AdvancedPredictedController _controller;
        
        private int _worldPlayerLayer;
        private int _ownerThirdPersonLayer;
        
        public event Action OnPresentationPoseResetStarted;
        public event Action OnPresentationPoseResetApplied;

        public override void OnStartClient()
        {
            base.OnStartClient();

            CacheLayers();
            ApplyOwnershipPresentation();

            _controller = GetComponent<AdvancedPredictedController>();

            if (_controller == null)
                return;

            if (IsOwner)
                _controller.OnLocalPoseResetApplied += HandleLocalPoseResetApplied;
            
            else
                _controller.OnObserverPoseResetReceived += HandleObserverPoseResetReceived;
            
        }
        
        public override void OnStopClient()
        {
            if (_controller != null)
            {
                if (IsOwner)
                    _controller.OnLocalPoseResetApplied -= HandleLocalPoseResetApplied;
                
                else
                    _controller.OnObserverPoseResetReceived -= HandleObserverPoseResetReceived;
            }

            _controller = null;

            base.OnStopClient();
        }
        
        private void LateUpdate()
        {
            if (IsOwner || !_observerResetPending)
            {
                return;
            }

            float tolerance = Mathf.Max(0.01f, observerResetArrivalTolerance);

            float distanceSqr = (transform.position - _pendingObserverResetPosition).sqrMagnitude;

            if (distanceSqr > tolerance * tolerance)
                return;
            

            /*
             * The remotely replicated gameplay root has now reached the
             * authoritative reset area. Rebase the detached graphical proxy
             * and resume smoothing from the correct lifecycle.
             */
            if (graphicsRootSmoother != null)
                graphicsRootSmoother.transform.SetPositionAndRotation(transform.position, transform.rotation);
            

            RestartSmoother(graphicsRootSmoother);

            _observerResetPending = false;

            OnPresentationPoseResetApplied?.Invoke();
        }

        private void CacheLayers()
        {
            _worldPlayerLayer = LayerMask.NameToLayer(worldPlayerLayerName);

            _ownerThirdPersonLayer = LayerMask.NameToLayer(ownerThirdPersonLayerName);

            if (_worldPlayerLayer < 0)
            {
                Debug.LogError($"[{nameof(PlayerPresentation)}] Layer '{worldPlayerLayerName}' does not exist.", this);
            }

            if (_ownerThirdPersonLayer < 0)
            {
                Debug.LogError($"[{nameof(PlayerPresentation)}] Layer '{ownerThirdPersonLayerName}' does not exist.", this);
            }
        }

        private void ApplyOwnershipPresentation()
        {
            int targetLayer = IsOwner ? _ownerThirdPersonLayer : _worldPlayerLayer;

            if (targetLayer < 0)
                return;

            SetLayerRecursively(worldModelRoot, targetLayer);

            SetLayerRecursively(thirdPersonPresentationRoot, targetLayer);
        }

        private static void SetLayerRecursively(GameObject root, int layer)
        {
            if (root == null)
                return;

            Transform[] children = root.GetComponentsInChildren<Transform>(true);

            for (int i = 0; i < children.Length; i++)
            {
                Transform child = children[i];

                if (child != null)
                    child.gameObject.layer = layer;
            }
        }
        
        private void HandleLocalPoseResetApplied()
        {
            OnPresentationPoseResetStarted?.Invoke();

            RestartSmoother(cameraFollowSmoother);

            RestartSmoother(graphicsRootSmoother);

            OnPresentationPoseResetApplied?.Invoke();
        }
        
        private void HandleObserverPoseResetReceived(byte sequence, Vector3 position, Quaternion rotation)
        {
            _observerResetPending = true;

            _pendingObserverResetSequence = sequence;

            _pendingObserverResetPosition = position;

            _pendingObserverResetRotation = rotation;

            /*
             * Immediately close any presentation-dependent UI gate.
             */
            OnPresentationPoseResetStarted?.Invoke();

            /*
             * Freeze the detached graphical proxy instead of allowing it
             * to continue smoothing around the old death position.
             */
            StopSmoother(graphicsRootSmoother);

            if (graphicsRootSmoother != null)
                graphicsRootSmoother.transform.SetPositionAndRotation(position, rotation);
            
        }
        
        private static void StopSmoother(NetworkTickSmoother smoother)
        {
            if (smoother == null)
                return;

            TickSmootherController controller = smoother.SmootherController;

            if (controller == null)
                return;

            controller.StopSmoother();
        }
        
        private static void RestartSmoother(NetworkTickSmoother smoother)
        {
            if (smoother == null)
                return;

            TickSmootherController controller = smoother.SmootherController;

            if (controller == null)
                return;

            controller.StopSmoother();
            controller.StartSmoother();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (worldModelRoot == null)
            {
                Transform found = transform.Find("GraphicsRoot/WorldModel");

                if (found != null)
                    worldModelRoot = found.gameObject;
            }

            if (thirdPersonPresentationRoot == null)
            {
                Transform found = transform.Find("GraphicsRoot/ThirdPersonAimPivot");

                if (found != null)
                {
                    thirdPersonPresentationRoot = found.gameObject;
                }
            }
            
            if (cameraFollowSmoother == null)
            {
                Transform found = transform.Find("CameraFollowTarget");

                if (found != null)
                {
                    cameraFollowSmoother = found.GetComponent<NetworkTickSmoother>();
                }
            }

            if (graphicsRootSmoother == null)
            {
                Transform found = transform.Find("GraphicsRoot");

                if (found != null)
                {
                    graphicsRootSmoother = found.GetComponent<NetworkTickSmoother>();
                }
            }
        }
#endif
    }
}
using FishNet.Object;
using UnityEngine;

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

        [Header("Layers")]

        [SerializeField]
        private string worldPlayerLayerName = "Player";

        [SerializeField]
        private string ownerThirdPersonLayerName = "TP_Only";

        private int _worldPlayerLayer;
        private int _ownerThirdPersonLayer;

        public override void OnStartClient()
        {
            base.OnStartClient();

            CacheLayers();
            ApplyOwnershipPresentation();
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
        }
#endif
    }
}
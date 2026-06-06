using UnityEngine;

namespace _Scripts.Game.CTF
{
    [DisallowMultipleComponent]
    public sealed class FlagVisualSmoother : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] Transform visualRoot;

        [Header("Smoothing")]
        [SerializeField, Range(0.01f, 1f)] float positionSharpness = 0.35f;
        [SerializeField, Range(0.01f, 1f)] float rotationSharpness = 0.35f;

        Vector3 _targetLocalPosition;
        Quaternion _targetLocalRotation;

        Vector3 _smoothedWorldPosition;
        Quaternion _smoothedWorldRotation;

        bool _initialized;
        bool _smoothEnabled;

        void Awake()
        {
            if (visualRoot == null)
                return;

            _targetLocalPosition = visualRoot.localPosition;
            _targetLocalRotation = visualRoot.localRotation;

            Snap();
        }

        void LateUpdate()
        {
            if (visualRoot == null)
                return;

            if (!_smoothEnabled)
            {
                Snap();
                return;
            }

            if (!_initialized)
                Snap();

            Vector3 targetWorldPosition = transform.TransformPoint(_targetLocalPosition);
            Quaternion targetWorldRotation = transform.rotation * _targetLocalRotation;

            _smoothedWorldPosition = Vector3.Lerp(
                _smoothedWorldPosition,
                targetWorldPosition,
                positionSharpness);

            _smoothedWorldRotation = Quaternion.Slerp(
                _smoothedWorldRotation,
                targetWorldRotation,
                rotationSharpness);

            visualRoot.SetPositionAndRotation(
                _smoothedWorldPosition,
                _smoothedWorldRotation);
        }

        public void SetSmoothing(bool enabled)
        {
            _smoothEnabled = enabled;

            if (!enabled)
                Snap();
        }

        public void Snap()
        {
            if (visualRoot == null)
                return;

            visualRoot.localPosition = _targetLocalPosition;
            visualRoot.localRotation = _targetLocalRotation;

            _smoothedWorldPosition = visualRoot.position;
            _smoothedWorldRotation = visualRoot.rotation;

            _initialized = true;
        }
    }
}
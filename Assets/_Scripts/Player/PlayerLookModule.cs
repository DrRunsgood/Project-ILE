using UnityEngine;

namespace _Scripts.Player
{
    [System.Serializable]
    public sealed class PlayerLookModule
    {
        private readonly float _yawSensitivity;
        private readonly float _pitchSensitivity;
        private readonly float _minPitch;
        private readonly float _maxPitch;

        private float _yaw;
        private float _currentPitch;

        public float Yaw => _yaw;
        public float CurrentPitch => _currentPitch;

        public PlayerLookModule(float yawSensitivity, float pitchSensitivity, float minPitch,
            float maxPitch, float startingYaw, float startingPitch)
        {
            _yawSensitivity = yawSensitivity;
            _pitchSensitivity = pitchSensitivity;
            _minPitch = minPitch;
            _maxPitch = maxPitch;

            _yaw = startingYaw;
            _currentPitch = Mathf.Clamp(startingPitch, _minPitch, _maxPitch);
        }

        public void ApplyRotation(float yawDeltaRaw, float pitchDeltaRaw, Rigidbody rb, Transform headAnchor)
        {
            _yaw += yawDeltaRaw * _yawSensitivity;

            if (rb != null)
                rb.MoveRotation(Quaternion.Euler(0f, _yaw, 0f));

            _currentPitch = Mathf.Clamp(
                _currentPitch - pitchDeltaRaw * _pitchSensitivity,
                _minPitch,
                _maxPitch);

            if (headAnchor != null)
                headAnchor.localEulerAngles = new Vector3(_currentPitch, 0f, 0f);
        }

        public void ApplyLookState(float yaw, float pitch, Rigidbody rb, Transform headAnchor)
        {
            _yaw = yaw;
            _currentPitch = Mathf.Clamp(pitch, _minPitch, _maxPitch);

            if (rb != null)
                rb.MoveRotation(Quaternion.Euler(0f, _yaw, 0f));

            if (headAnchor != null)
                headAnchor.localEulerAngles = new Vector3(_currentPitch, 0f, 0f);
        }

        public void ResetLook(Rigidbody rb, Transform headAnchor, float yaw = 0f, float pitch = 0f)
        {
            ApplyLookState(yaw, pitch, rb, headAnchor);
        }
        
        public Quaternion GetPreviewRotation(Vector2 pendingLookDelta)
        {
            float previewYaw = _yaw + pendingLookDelta.x * _yawSensitivity;

            float previewPitch = Mathf.Clamp(_currentPitch - pendingLookDelta.y * _pitchSensitivity,
                    _minPitch, _maxPitch);

            return Quaternion.Euler(previewPitch, previewYaw, 0f);
        }
    }
}
using FishNet.Object;
using UnityEngine;
using _Scripts.Map;

namespace _Scripts.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerHealth))]
    public sealed class PlayerOutOfBoundsController : NetworkBehaviour
    {
        [Header("Out Of Bounds")]
        [SerializeField] private bool enableOutOfBounds = true;
        [SerializeField] private float graceSeconds = 3f;
        [SerializeField] private float damageTickInterval = 1f;
        [SerializeField] private int damagePerTick = 25;

        private PlayerHealth _health;
        private bool _isOutOfBounds;
        private float _outsideTime;
        private float _nextDamageTickTime;

        private void Awake()
        {
            _health = GetComponent<PlayerHealth>();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            _isOutOfBounds = false;
            _outsideTime = 0f;
            _nextDamageTickTime = 0f;
        }

        private void Update()
        {
            if (!IsServerInitialized)
                return;

            ServerTickOutOfBounds(Time.deltaTime);
        }
        
        [Server]
        private void ServerTickOutOfBounds(float deltaTime)
        {
            if (!enableOutOfBounds)
                return;

            if (_health == null)
                _health = GetComponent<PlayerHealth>();

            if (_health == null || _health.IsDead)
                return;

            MapBoundsManager bounds = MapBoundsManager.Instance;
            if (bounds == null)
                return;

            bool inside = bounds.IsInsideBounds(transform.position);

            if (inside)
            {
                if (_isOutOfBounds)
                    Debug.Log($"[PlayerOutOfBoundsController] {name} returned in bounds.");

                _isOutOfBounds = false;
                _outsideTime = 0f;
                _nextDamageTickTime = 0f;
                return;
            }

            if (!_isOutOfBounds)
            {
                _isOutOfBounds = true;
                _outsideTime = 0f;
                _nextDamageTickTime = graceSeconds;

                Debug.Log($"[PlayerOutOfBoundsController] {name} left bounds.");
            }

            _outsideTime += deltaTime;

            if (_outsideTime < graceSeconds)
                return;

            if (_outsideTime < _nextDamageTickTime)
                return;

            _nextDamageTickTime += damageTickInterval;

            Debug.Log($"[PlayerOutOfBoundsController] Applying OOB damage to {name}. Damage={damagePerTick}");
            _health.ServerApplyOutOfBoundsDamage(damagePerTick);
        }
    }
}
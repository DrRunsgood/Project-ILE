using UnityEngine;
using FishNet.Object;
using FishNet.Object.Prediction;

namespace YourGameNamespace
{
    // NOTE: Make sure this prefab also has a NetworkTransform component attached.
    public class PredictedProjectile : NetworkBehaviour
    {
        // Travel direction.
        private Vector3 _direction;
        // Extra lag compensation time.
        private float _passedTime;
        // Time when this projectile was initialized.
        private float _spawnTime;
        // Lifetime in seconds.
        private float _lifetime;
        
        private Rigidbody _rb;

        // Constant move speed (units per second).
        [SerializeField] private float moveRate = 40f;
        // Fraction of remaining passed time to consume each frame.
        private const float CATCH_UP_PERCENTAGE = 0.08f;

        // Initializes the projectile's movement parameters.
        public void Initialize(Vector3 direction, float passedTime, float lifetime)
        {
            _direction = direction;
            _passedTime = passedTime;
            _lifetime = lifetime;
            _spawnTime = Time.time;
            Debug.Log("[PredictedProjectile] Initialized at " + transform.position + " with direction " + _direction + ", passedTime " + _passedTime);
        }
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            // Always update movement.
            MoveProjectile();

            // Only the networked copy on the server handles lifetime.
            if (IsServer && (Time.time - _spawnTime) >= _lifetime)
            {
                Debug.Log("[PredictedProjectile] Lifetime expired at " + transform.position + ". Destroying.");
                Destroy(gameObject);
            }
            
            _rb.AddForce(Vector3.down * 30f);  // our regular gravity, a test
        }

        // Moves the projectile forward and applies lag compensation.
        private void MoveProjectile()
        {
            float delta = (float)TimeManager.TickDelta;
            float passedTimeDelta = 0f;

            if (_passedTime > 0f)
            {
                float step = _passedTime * CATCH_UP_PERCENTAGE;
                _passedTime -= step;
                if (_passedTime <= (delta / 2f))
                {
                    step += _passedTime;
                    _passedTime = 0f;
                }
                passedTimeDelta = step;
            }

            transform.position += _direction * (moveRate * (delta + passedTimeDelta));
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("[PredictedProjectile] Collision with " + collision.gameObject.name);

            Destroy(gameObject);
        }
    }
}

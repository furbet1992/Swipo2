using UnityEngine;

namespace TypingGameKit.AsteroidTyping
{
    /// <summary>
    /// An asteroid that threatens the player ship
    /// </summary>
    public class Asteroid : MonoBehaviour
    {
        [SerializeField] private float _maxRotation = 50f;
        [SerializeField] private GameObject _remains = null;
        private Rigidbody2D _rigidBody;

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
        }

        /// <summary>
        /// Makes the asteroid blow up.
        /// </summary>
        public void Explode()
        {
            Destroy(gameObject);
            SpawnRemains();
        }

        /// <summary>
        /// Pushes the asteroid toward a target.
        /// </summary>
        public void InitializeAsteroid(Vector3 target, float velocity)
        {
            // Randomize orientation
            transform.up = target - transform.position;

            // Set velocity
            Vector2 direction = (target - transform.position).normalized;
            _rigidBody.velocity = direction * velocity;
            _rigidBody.angularVelocity = Random.Range(-_maxRotation, _maxRotation);
        }

        private void SpawnRemains()
        {
            Instantiate(_remains, transform.position, transform.rotation);
        }
    }
}
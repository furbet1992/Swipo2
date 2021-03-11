using UnityEngine;

namespace TypingGameKit.AsteroidTyping
{
    /// <summary>
    /// Manages the 2D example game
    /// </summary>
    public class AsteroidSpawner : MonoBehaviour
    {
        [SerializeField] private PlayerShip _player = null;
        [SerializeField] private SequenceOverlay _sequenceOverlay = null;
        [SerializeField] private TextCollection _texts = null;

        [SerializeField] private Asteroid _asteroidPrefab = null;
        [SerializeField] private float _spawnRadius = 1f;
        [SerializeField] private float _asteroidsPerMinute = 20;
        [SerializeField] private float _asteroidVelocity = 0.1f;
        [SerializeField] private float _asteroidIncreasePerSecond = 0.5f;
        [SerializeField] private float _velocityIncreasePerSecond = 0.01f;

        private float lastSpawned = 0;

        private void Update()
        {
            if (AsteroidGameManager.Instance.IsRunning)
            {
                UpdateSpawning();
            }
        }

        private void UpdateSpawning()
        {
            // spawn asteroids at the current wpm pace
            if (Time.time - lastSpawned > 60 / _asteroidsPerMinute)
            {
                lastSpawned = Time.time;
                SpawnAsteroidWithSequence();
            }

            // increase sequences spawned per minute over time
            _asteroidsPerMinute += Time.deltaTime * _asteroidIncreasePerSecond;
            _asteroidVelocity += Time.deltaTime * _velocityIncreasePerSecond;
        }

        private void SpawnAsteroidWithSequence()
        {
            AttachSequenceToAsteroid(SpawnAsteroid());
        }

        private Asteroid SpawnAsteroid()
        {
            Asteroid asteroid = Instantiate(_asteroidPrefab, transform);
            asteroid.transform.position = RandomInitialAsteroidPosition();
            asteroid.InitializeAsteroid(_player.transform.position, _asteroidVelocity);
            return asteroid;
        }

        private void AttachSequenceToAsteroid(Asteroid asteroid)
        {
            string inputText = _texts.FindUniquelyTargetableText();
            InputSequence sequence = _sequenceOverlay.CreateSequence(inputText, asteroid.transform);

            sequence.OnInputAccepted += delegate { _player.FireAt(asteroid); };
            sequence.OnCompleted += delegate { asteroid.Explode(); };
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, _spawnRadius);
        }

        private Vector3 RandomInitialAsteroidPosition()
        {
            //float value = Random.Range(0, Mathf.PI * 2);
            //return transform.position + new Vector3(Mathf.Cos(value), Mathf.Sin(value)) * _spawnRadius;

                // Get a random position on the top of the viewport
                Vector3 topPosition = Camera.main.ViewportToWorldPoint(new Vector2(Random.Range(0f, 1f), 1));

                // Make sure its z height is at the same height as the player.
                return new Vector3(topPosition.x, topPosition.y, 0);
            
        }
    }
}
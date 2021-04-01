using UnityEngine;

namespace TypingGameKit.AsteroidTyping
{
    /// <summary>
    /// Player ship that shoots at asteroids.
    /// </summary>
    public class PlayerShip : MonoBehaviour
    {
        private AudioSource audioSource;

        [SerializeField] private float _laserDuration = 0.1f;
        [SerializeField] private float _laserRandomSpread = 1f;
        [SerializeField] private LineRenderer _laserRenderer = null;
        [SerializeField] private AudioClip _laserSound = null;

        [UnityEngine.Serialization.FormerlySerializedAs("remains")]
        [SerializeField] private GameObject _remains = null;

        private float lastShotTime;

        /// <summary>
        /// Blows up the ship.
        /// </summary>
        public void Explode()
        {
            gameObject.SetActive(false);
            Instantiate(_remains, transform.position, transform.rotation);
        }

        /// <summary>
        /// Shoots at the given asteroid
        /// </summary>
        public void FireAt(Asteroid asteroid)
        {
            lastShotTime = Time.time;
            Vector3 target = asteroid.transform.position;

            LookAtPosition(target);
            DisplayLaser(transform.position, target);
            audioSource.PlayOneShot(_laserSound);
        }

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            FindObjectOfType<AsteroidGameManager>().GameOver();   
            
        }

        private void DisplayLaser(Vector3 pos, Vector3 targetPos)
        {
            _laserRenderer.enabled = true;
            Vector3 randomSpread = Random.insideUnitCircle * _laserRandomSpread;
            _laserRenderer.SetPositions(new Vector3[] { pos, targetPos + randomSpread });
        }

        private void Update()
        {
            if (_laserRenderer.enabled && Time.time - lastShotTime > _laserDuration)
            {
                _laserRenderer.enabled = false;
            }
        }

        private void LookAtPosition(Vector3 target)
        {
            transform.up = target - transform.position;
        }
    }
}
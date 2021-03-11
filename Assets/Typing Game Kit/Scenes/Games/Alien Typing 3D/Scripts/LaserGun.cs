using UnityEngine;

namespace TypingGameKit.AlienTyping
{
    /// <summary>
    /// Manages the visuals and sound of firing the player's laser gun.
    /// </summary>
    public class LaserGun : MonoBehaviour
    {
        private AudioSource audioSource;

        [SerializeField] private AudioClip _fireSound = null;

        [SerializeField] private LineRenderer _laserRenderer = null;
        [SerializeField] private Light _laserLight = null;
        [SerializeField] private float _laserDuration = 0.1f;
        [SerializeField] private float _randomSpread = 0.1f;

        private float _lastShot;

        public void FireAt(Vector3 targetPos)
        {
            _lastShot = Time.time;
            DisplayLaser(targetPos);
            PlayFireSound();
        }

        private void DisplayLaser(Vector3 targetPos)
        {
            _laserRenderer.enabled = true;
            _laserLight.enabled = true;
            var spread = Random.insideUnitSphere * _randomSpread;
            _laserRenderer.SetPositions(new Vector3[] { transform.position, targetPos + spread });
        }

        private void PlayFireSound()
        {
            audioSource.clip = _fireSound;
            audioSource.Play();
        }

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (_laserRenderer.enabled && Time.time - _lastShot > _laserDuration)
            {
                _laserRenderer.enabled = false;
                _laserLight.enabled = false;
            }
        }
    }
}
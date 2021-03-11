using System.Collections;
using UnityEngine;

namespace TypingGameKit.AsteroidTyping
{
    /// <summary>
    /// Manages a 2D example game
    /// </summary>
    public class AsteroidGameManager : MonoBehaviour
    {
        [SerializeField] private PlayerShip _player = null;
        [SerializeField] private InputSequenceStatistics _statistics = null;

        [SerializeField] private ResultsPanel _resultsMenu = null;
        [SerializeField] private float _resultsMenuDelay = 2f;

        [SerializeField] private AudioSource _audioSource = null;
        [SerializeField] private AudioClip _inputErrorSound = null;

        public static AsteroidGameManager Instance { get; private set; }
        public bool IsRunning { get; private set; }

        public int Score
        {
            get { return (int)(_statistics.AcceptedInputs * _statistics.Accuracy); }
        }
        private void Awake()
        {
            Instance = this;
            IsRunning = true;
            Unpause();
            InputSequenceManager.OnInputRejected += PlayErrorSound;
        }

        private void OnDestroy()
        {
            InputSequenceManager.OnInputRejected -= PlayErrorSound;
        }

        /// <summary>
        /// To be called when the game is over.
        /// </summary>
        public void GameOver()
        {
            InputSequenceManager.RemoveAllSequences();
            _player.Explode();
            IsRunning = false;
            StartCoroutine(ShowResultsMenu());
        }


        private void PlayErrorSound()
        {
            _audioSource.PlayOneShot(_inputErrorSound);
        }

        private IEnumerator ShowResultsMenu()
        {
            yield return new WaitForSeconds(_resultsMenuDelay);
            Pause();
            _resultsMenu.gameObject.SetActive(true);
        }

        private void Unpause()
        {
            Time.timeScale = 1;
        }

        private void Pause()
        {
            Time.timeScale = 0;
        }
    }
}
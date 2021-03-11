using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TypingGameKit.AlienTyping
{
    /// <summary>
    /// Manages the game.
    /// </summary>
    public partial class AlienGameManager : MonoBehaviour
    {
        [SerializeField] private SequenceOverlay _sequenceOverlay = null;
        [SerializeField] private InputSequenceStatistics _typingStatistics = null;
        [SerializeField] private TextCollection _texts = null;

        [SerializeField] private AudioClip _inputErrorSound = null;

        private AudioSource _audioSource;
        private int _completedSequences = 0;

        public event System.Action OnGameOver = delegate () { };

        public event System.Action OnLevelCompleted = delegate () { };

        public static AlienGameManager Instance { get; private set; }

        public float Accuracy
        {
            get
            {
                return _typingStatistics.Accuracy;
            }
        }

        public int Score
        {
            get
            {
                return (int)((_completedSequences - Player.Instance.HitsTaken) * 10 * Accuracy);
            }
        }

        public void RestartGame()
        {
            Scene loadedLevel = SceneManager.GetActiveScene();
            SceneManager.LoadScene(loadedLevel.buildIndex);
        }

        /// <summary>
        /// Create a sequence that is attached to the given transform with.
        /// </summary>
        public InputSequence CreateSequence(Transform transform, int difficulty)
        {
            string inputText = GenerateSequenceText(difficulty);
            InputSequence sequence = _sequenceOverlay.CreateSequence(inputText, transform);
            return sequence;
        }

        /// <summary>
        /// Generates a text with a given difficulty.
        /// </summary>
        private string GenerateSequenceText(int difficulty)
        {
            List<string> texts = new List<string>();
            texts.Add(_texts.FindUniquelyTargetableText());
            for (int i = 1; i < difficulty; i++)
            {
                texts.Add(_texts.PickRandomText());
            }
            return string.Join(" ", texts);
        }

        private void Awake()
        {
            // Set instance
            Instance = this;

            // Fetch
            _audioSource = GetComponent<AudioSource>();

            // Add listeners to sequence manager events
            InputSequenceManager.OnInputRejected += HandleTypingFailure;
            InputSequenceManager.OnSequenceCompleted += HandleSequenceCompletion;
        }

        private void OnDestroy()
        {
            // remove event listeners to sequence manager events
            InputSequenceManager.OnInputRejected -= HandleTypingFailure;
            InputSequenceManager.OnSequenceCompleted -= HandleSequenceCompletion;
        }

        private void Start()
        {
            InitializeLevel();
        }

        private void InitializeLevel()
        {
            Unpause();

            // listen to player events
            Player.Instance.OnHealthChanged += HandlePlayerHealthChange;

            // listen to player path events
            FindObjectOfType<PlayerPath>().OnCompletion += OnLevelCompleted;
        }

        private void HandlePlayerHealthChange(float health)
        {
            if (health <= 0)
            {
                OnGameOver();
            }
        }

        private void HandleSequenceCompletion()
        {
            _completedSequences += 1;
        }

        private void HandleTypingFailure()
        {
            _audioSource.PlayOneShot(_inputErrorSound);
        }

        public void Pause()
        {
            foreach (var sequence in FindObjectsOfType<InputSequence>())
            {
                InputSequenceManager.UnregisterSequence(sequence);
            }
            Time.timeScale = 0f;
        }

        public void Unpause()
        {
            foreach (var sequence in FindObjectsOfType<InputSequence>())
            {
                InputSequenceManager.RegisterSequence(sequence);
            }
            Time.timeScale = 1f;
        }
    }
}
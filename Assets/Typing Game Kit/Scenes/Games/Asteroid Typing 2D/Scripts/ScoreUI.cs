using UnityEngine;

namespace TypingGameKit.AsteroidTyping
{
    /// <summary>
    /// Keeps track of the score during the game session.
    /// </summary>
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Text _highScoreDisplay = null;
        [SerializeField] private UnityEngine.UI.Text _scoreDisplay = null;

        private void Start()
        {
            UpdateHighScoreDisplay();
        }

        private void Update()
        {
            UpdateScoreDisplay();
        }

        private void UpdateHighScoreDisplay()
        {
            _highScoreDisplay.text = string.Format("Hi-Score: {0}", HighScore.GetHighScore());
        }

        private void UpdateScoreDisplay()
        {
            _scoreDisplay.text = string.Format("Score: {0}", AsteroidGameManager.Instance.Score);
        }
    }
}
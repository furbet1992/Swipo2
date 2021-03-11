using UnityEngine;

namespace TypingGameKit.AlienTyping
{
    /// <summary>
    /// Displays the result panel.
    /// </summary>
    public class ResultsPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _content = null;

        [SerializeField] private UnityEngine.UI.Text _titleText = null;
        [SerializeField] private UnityEngine.UI.Text _scoreText = null;
        [SerializeField] private UnityEngine.UI.Text _highScoreText = null;

        private void Awake()
        {
            _content.SetActive(false);
        }

        private void GameOver()
        {
            AlienGameManager.Instance.Pause();
            _titleText.text = "Game Over";
            _content.SetActive(true);
            UpdateScore();
        }

        private void LevelCompleted()
        {
            AlienGameManager.Instance.Pause();
            _titleText.text = "Level Completed";
            _content.SetActive(true);
            UpdateScore();
        }

        private void Start()
        {
            AlienGameManager.Instance.OnGameOver += GameOver;
            AlienGameManager.Instance.OnLevelCompleted += LevelCompleted;
        }

        private void UpdateScore()
        {
            var currentScore = AlienGameManager.Instance.Score;
            var highestScore = HighScore.GetHighScore();

            if (currentScore > highestScore)
            {
                HighScore.SetHighScore(currentScore);
                _scoreText.text = currentScore.ToString();
                _highScoreText.text = "-";
            }
            else
            {
                _scoreText.text = $"{currentScore}";
                _highScoreText.text = $"{highestScore}";
            }
        }
    }
}
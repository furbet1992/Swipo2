using UnityEngine;

namespace TypingGameKit.AlienTyping
{
    /// <summary>
    /// Tracks the player score through the level.
    /// </summary>
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Text _accuracyDisplay = null;
        [SerializeField] private UnityEngine.UI.Text _healthDisplay = null;
        [SerializeField] private UnityEngine.UI.Text _scoreDisplay = null;

        private void Update()
        {
            _accuracyDisplay.text = $"Accuracy: {AlienGameManager.Instance.Accuracy:P0}";
            _healthDisplay.text = $"HP: {Player.Instance.Health}";
            _scoreDisplay.text = $"Score: {AlienGameManager.Instance.Score}";
        }
    }
}
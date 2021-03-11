using UnityEngine;

namespace TypingGameKit.AsteroidTyping
{
    /// <summary>
    /// Manages the score persistence.
    /// </summary>
    public static class HighScore
    {
        private const string _highScoreKey = "Asteroid Typing HighScore";

        public static int GetHighScore()
        {
            return PlayerPrefs.GetInt(_highScoreKey, 0);
        }

        public static void SetHighScore(int value)
        {
            PlayerPrefs.SetInt(_highScoreKey, value);
        }
    }
}
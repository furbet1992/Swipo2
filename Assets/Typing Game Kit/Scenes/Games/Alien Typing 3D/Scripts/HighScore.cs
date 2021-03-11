using UnityEngine;

namespace TypingGameKit.AlienTyping
{
    /// <summary>
    /// Handles score persistence.
    /// </summary>
    public static class HighScore
    {
        private const string _highScoreKey = "Alien Typing HighScore";

        public static int GetHighScore()
        {
            return PlayerPrefs.GetInt(_highScoreKey, 0);
        }

        public static void SetHighScore(int value)
        {
            PlayerPrefs.SetFloat(_highScoreKey, value);
        }
    }
}
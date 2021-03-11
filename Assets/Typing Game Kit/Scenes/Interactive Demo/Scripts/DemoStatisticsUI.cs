using UnityEngine;
using UnityEngine.UI;

namespace TypingGameKit.Demo
{
    public class DemoStatisticsUI : MonoBehaviour
    {
        [SerializeField] private InputSequenceWPM _wpmTimer = null;
        [SerializeField] private Text _wpmText = null;

        private void Update()
        {
            if (_wpmText != null)
            {
                _wpmText.text = $"{_wpmTimer.WPM:F0} wpm";
            }
        }
    }
}
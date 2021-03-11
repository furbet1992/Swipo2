using UnityEngine;

namespace TypingGameKit
{
    /// <summary>
    /// A script that will change the text of a sequence once it has been completed.
    /// </summary>
    public class TextChanger : MonoBehaviour
    {
        [SerializeField] private string[] _texts = null;
        [SerializeField] private InputSequence _sequence = null;

        private int _index = -1;

        private void Start()
        {
            SetNextText();
            _sequence.OnCompleted += delegate { SetNextText(); };
        }

        private void SetNextText()
        {
            _index = (_index + 1) % _texts.Length;
            string chosenText = _texts[_index];
            _sequence.Text = chosenText;
        }
    }
}
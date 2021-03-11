using UnityEngine;

namespace TypingGameKit
{
    /// <summary>
    /// Displays the current state of an input sequence.
    /// </summary>
    [System.Serializable]
    [ExecuteInEditMode]
    public class InputSequenceDisplay : MonoBehaviour
    {
        private const string NOSPACE_BREAKABLE = "\u200B";
        private const string NO_BREAK_SPACE = "\u00A0";

        private enum PositionIndicator { None, Unicode_032D = '\u032D' };

        private enum SpaceReplacement { None, Interpunct = '\u00B7' };

        [Tooltip("A reference to the sequence who's text will be displayed.")]
        [SerializeField] private InputSequence _sequence = null;

        [Tooltip("Text component that display text output.")]
        [SerializeField] private TMPro.TMP_Text _text = null;

        [Tooltip("Color to use when showing the sequence's completed text.")]
        [SerializeField] private Color _completedColor = Color.grey;

        [Tooltip("Color to use when showing the sequence's remaining text.")]
        [SerializeField] private Color _remainingColor = Color.white;

        [Tooltip("This setting determines completed portion of the sequence is displayed or not.")]
        [SerializeField] private bool _hideCompletedText = false;

        [Tooltip("A symbol will be drawn highlighting the next input.")]
        [SerializeField] private PositionIndicator _positionIndicator = PositionIndicator.None;

        [Tooltip("If this setting is toggled a character will replace the upcoming space character in the sequence.")]
        [SerializeField] private SpaceReplacement _spaceReplacement = SpaceReplacement.Interpunct;

        private string _lastUsedText = "";
        private int _lastUsedIndex = 0;

        private void Awake()
        {
            Debug.Assert(_text != null, this);
            Debug.Assert(_sequence != null, this);

            _sequence.OnTextUpdated += delegate (InputSequence s) { UpdateText(s.Text, s.Progress); };
        }

        private void OnValidate()
        {
            if (_sequence == null)
            {
                UpdateText("Example Text", 4);
            }
            else
            {
                UpdateText(_sequence.Text, _sequence.Progress);
            }
        }

        private string ColorizeText(string text, Color color)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{text}</color>";
        }

        private string FormatAsTargeted(string originalText)
        {
            string formatedText = originalText;
            if (_positionIndicator != PositionIndicator.None)
            {
                // Make sure that the position indicator and the targeted text stay together.;
                formatedText = $"<nobr>{formatedText}{(char)_positionIndicator}</nobr>";

                if (string.IsNullOrWhiteSpace(originalText))
                {
                    // Try to keep the line wrapping consistent
                    formatedText += NOSPACE_BREAKABLE;
                }
            }

            if (_spaceReplacement != SpaceReplacement.None)
            {
                formatedText = formatedText.Replace(" ", $"{(char)_spaceReplacement}{NOSPACE_BREAKABLE}");
            }

            return ColorizeText(formatedText, _remainingColor);
        }

        private string FormatAsCompleted(string text)
        {
            if (_hideCompletedText)
            {
                return "";
            }

            return ColorizeText(ReplaceSpace(text), _completedColor);
        }

        private string FormatAsRemaining(string text)
        {
            return ColorizeText(ReplaceSpace(text), _remainingColor); ;
        }

        private string ReplaceSpace(string text)
        {
            // In order to make line wrapping consistent if want to replace the space with a nonbreakable character.
            return text.Replace(" ", NO_BREAK_SPACE + NOSPACE_BREAKABLE);
        }

        private void UpdateText(string text, int startIndex)
        {
            _lastUsedText = text;
            _lastUsedIndex = startIndex;

            string completedText = FormatAsCompleted(text.Substring(0, startIndex));
            string targetedText = startIndex >= text.Length ? "" : FormatAsTargeted(text.Substring(startIndex, 1));
            string remainingText = startIndex + 1 >= text.Length ? "" : FormatAsRemaining(text.Substring(startIndex + 1));

            _text.text = $"{completedText}{targetedText}{remainingText}";
            ;
        }
    }
}
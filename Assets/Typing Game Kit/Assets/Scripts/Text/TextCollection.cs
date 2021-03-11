using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TypingGameKit
{
    /// <summary>
    /// Extract texts from text files and index the texts by their initial character.
    /// </summary>
    [CreateAssetMenu(fileName = "Text Collection", menuName = "Typing Game Kit/Text Collection")]
    public class TextCollection : ScriptableObject
    {
        [Tooltip("If enabled the statistics view will differentiate initials with different case")]
        [SerializeField, HideInInspector] private bool _initialBreakdownByCase = true;

        [Tooltip("A number of extractor that will extract text from text files.")]
        [SerializeField] private TextExtractor[] _textExtractors = new TextExtractor[] { };

        private string[] _texts;
        private Dictionary<char, HashSet<string>> _initialToTexts;
        private Dictionary<char, HashSet<string>> _upperInitialToTexts;

        /// <summary>
        /// If true the statistics separate the initials with different case.
        /// </summary>
        public bool InitialsBreakdownByCase
        {
            get { return _initialBreakdownByCase; }
            set { _initialBreakdownByCase = value; }
        }

        /// <summary>
        /// All available text in the collection.
        /// </summary>
        public string[] Texts
        {
            get { return _texts; }
        }

        /// <summary>
        /// Reevaluates the extracted strings.
        /// </summary>
        public void EvaluateTexts()
        {
            HashSet<string> textSetSet = new HashSet<string>();
            foreach (var source in _textExtractors)
            {
                textSetSet.UnionWith(source.Texts);
            }
            _texts = textSetSet.ToArray();

            EvaluateInitialMappings();
        }

        /// <summary>
        /// Attempts to return a text with a unique initial among the other registered input sequences' texts.
        /// </summary>
        public string FindUniquelyTargetableText()
        {
            var sequences = InputSequenceManager.Sequences;
            bool isCaseSensitive = sequences.Any(s => s.IsCaseSensitive);

            string input = UniqueInitialInput(sequences.Select(s => s.RemainingText), isCaseSensitive);
            if (input == null)
            {
                Debug.LogWarning($"Failed to find a sequence with unique initial for sequence manager and the collection {this}!");
                input = PickRandomText();
            }
            return input;
        }

        /// <summary>
        /// Grabs all the initials. If 'caseSensitive' is true all initials will be represented by its upper case.
        /// </summary>
        public IEnumerable<char> Initials(bool caseSensitive)
        {
            if (caseSensitive)
            {
                return _initialToTexts.Keys;
            }
            else
            {
                return _upperInitialToTexts.Keys;
            }
        }

        /// <summary>
        /// Picks a random text from the collection.
        /// </summary>
        public string PickRandomText()
        {
            return _texts.OrderBy(_ => Random.Range(0f, 1f)).FirstOrDefault();
        }

        /// <summary>
        /// Returns all the text with the specified initial.
        /// </summary>
        public IEnumerable<string> TextsByInitial(char initial, bool caseSensitive)
        {
            if (caseSensitive)
            {
                if (_initialToTexts.ContainsKey(initial))
                {
                    return _initialToTexts[initial];
                }
            }
            else
            {
                char upper = char.ToUpper(initial);
                if (_upperInitialToTexts.ContainsKey(upper))
                {
                    return _upperInitialToTexts[upper];
                }
            }
            return new string[] { };
        }

        /// <summary>
        /// Returns a text that has a unique initial among the other provided texts. Returns null if no such text exists.
        /// </summary>
        public string UniqueInitialInput(IEnumerable<string> otherTexts, bool isCaseSensitive)
        {
            var otherChars = otherTexts.Where(s => s.Length > 0).Select(s => isCaseSensitive ? s[0] : s.ToUpper()[0]);
            var availableInitials = Initials(isCaseSensitive);
            char[] availableUniqueInitials = availableInitials.Except(otherChars).ToArray();
            if (availableUniqueInitials.Length == 0)
            {
                return null;
            }

            int availableUniqueInitialsTotal = availableUniqueInitials.Select(c => TextsByInitial(c, isCaseSensitive).Count()).Sum();
            float randomValue = Random.Range(0f, 1f);
            float currentValue = 0;

            foreach (char initial in availableUniqueInitials)
            {
                currentValue += TextsByInitial(initial, isCaseSensitive).Count() / (float)availableUniqueInitialsTotal;
                if (randomValue <= currentValue)
                {
                    return TextsByInitial(initial, isCaseSensitive)
                        .OrderBy(_ => Random.Range(0f, 1f)).FirstOrDefault();
                }
            }

            return null;
        }

        private void EvaluateInitialMappings()
        {
            _initialToTexts = new Dictionary<char, HashSet<string>>();
            _upperInitialToTexts = new Dictionary<char, HashSet<string>>();
            foreach (var str in _texts)
            {
                char key = str[0];
                char upperCaseKey = char.ToUpper(str[0]);

                // add to case-sensitive dict
                if (_initialToTexts.ContainsKey(key) == false)
                {
                    _initialToTexts[key] = new HashSet<string>() { str };
                }
                _initialToTexts[key].Add(str);

                // add to case-insensitive dict
                if (_upperInitialToTexts.ContainsKey(upperCaseKey) == false)
                {
                    _upperInitialToTexts[upperCaseKey] = new HashSet<string>() { str };
                }
                _upperInitialToTexts[upperCaseKey].Add(str);
            }
        }

        private void OnEnable()
        {
            EvaluateTexts();
        }

        private void OnValidate()
        {
            EvaluateTexts();
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TypingGameKit
{
    /// <summary>
    /// Extracts text snippets from a given text source.
    /// </summary>
    [System.Serializable]
    public class TextExtractor
    {
        private enum Separator { Newline, AnyWhitespace }

        [Tooltip("Text source from which texts will be extracted from.")]
        [SerializeField] private TextAsset _sourceFile = null;

        [Tooltip("Separator to use when splitting texts from the source file.")]
        [SerializeField] private Separator _separator = Separator.Newline;

        [Tooltip("If enabled whitespace will be removed beginning and end of the extracted texts.")]
        [SerializeField] private bool _trimWhitespace = true;

        [Tooltip("Characters to filter out from the texts.")]
        [SerializeField] private string _characterFilter = ".,`\"\'";

        private string FilterCharacters(string str)
        {
            foreach (char c in System.Text.RegularExpressions.Regex.Unescape(_characterFilter))
            {
                str = str.Replace(c.ToString(), "");
            }
            return str;
        }

        /// <summary>
        /// The extracted texts.
        /// </summary>
        public IEnumerable<string> Texts
        {
            get
            {
                if (_sourceFile == null)
                {
                    return new string[] { };
                }

                char[] separators = null;
                switch (_separator)
                {
                    case Separator.Newline:
                        separators = new char[] { '\n' };
                        break;

                    case Separator.AnyWhitespace:
                        separators = new char[0];
                        break;

                    default:
                        Debug.LogWarning($"Undefined separator selection: {_separator}");
                        break;
                }

                return _sourceFile.text.Split(separators)
                .Select(w => FilterCharacters(w))
                .Select(w => _trimWhitespace ? w.Trim() : w)
                .Where(s => s.Length > 0);
            }
        }
    }
}
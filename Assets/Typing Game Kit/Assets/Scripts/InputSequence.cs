using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TypingGameKit
{
    /// <summary>
    /// Represents a sequence of texts that may be typed.
    /// </summary>
    public sealed class InputSequence : MonoBehaviour
    {
        [Tooltip("The sequence's text.")]
        [SerializeField] private string _text = "Example Text!";

        [Tooltip("The amount of characters that has been accepted by the sequence.")]
        [SerializeField] private int _progress = 0;

        [Tooltip("Determines if the case of input is regarded when accepting/rejecting input")]
        [SerializeField] private bool _isCaseSensitive = false;

        [Tooltip("If enabled whitespace is optional and the next non whitespace character is acceptable input.")]
        [SerializeField] private bool _optionalWhiteSpace = false;

        private HashSet<string> _acceptableInputs = new HashSet<string>();
        private int _progressForAcceptableInputs = -1;
        private string _previousText;

        /// <summary>
        /// Is raised when the sequence has been completed.
        /// </summary>
        public event Action<InputSequence> OnCompleted = delegate { };

        /// <summary>
        /// Is raised when the sequence receives matching input.
        /// </summary>
        public event Action<InputSequence> OnInputAccepted = delegate { };

        /// <summary>
        /// Is raised when the sequence receives input that does not match.
        /// </summary>
        public event Action<InputSequence> OnInputRejected = delegate { };

        /// <summary>
        /// Is raised when a sequence is set for removal.
        /// </summary>
        public event Action<InputSequence> OnRemoval = delegate { };

        /// <summary>
        /// Is raised when the sequence has been targeted.
        /// </summary>
        public event Action<InputSequence> OnTargeted = delegate { };

        /// <summary>
        /// Is raised when the sequence's text has been altered.
        /// </summary>
        public event Action<InputSequence> OnTextUpdated = delegate { };

        /// <summary>
        /// Is raised when the sequence has been untargeted.
        /// </summary>
        public event Action<InputSequence> OnUntargeted = delegate { };

        /// <summary>
        /// Returns the inputs that can currently be accepted by the sequence.
        /// </summary>
        public IEnumerable<string> AcceptableInputs
        {
            get
            {
                if (_progressForAcceptableInputs == _progress)
                {
                    return _acceptableInputs;
                }

                EvaluateAcceptableInputs();

                return _acceptableInputs;
            }
        }

        /// <summary>
        /// Returns the portion of the text that has been accepted by the sequence so far.
        /// </summary>
        public string CompletedText
        {
            get { return _text.Substring(0, _progress); }
        }

        /// <summary>
        /// Determines if the case of input is regarded when accepting/rejecting input.
        /// </summary>
        public bool IsCaseSensitive
        {
            get { return _isCaseSensitive; }
            set { _isCaseSensitive = value; }
        }

        /// <summary>
        /// Returns true if the sequence has been typed to completion.
        /// </summary>
        public bool IsCompleted
        {
            get { return (_progress >= _text.Length); }
        }

        /// <summary>
        /// Returns `true` if the sequence is targeted else `false`.
        /// </summary>
        public bool IsTargeted { get; private set; }

        /// <summary>
        /// The last input fed into the sequence.
        /// </summary>
        public string LastProcessedInput { get; internal set; }

        /// <summary>
        /// The number of characters that have been accepted by the sequence.
        /// </summary>
        public int Progress
        {
            get { return _progress; }
        }

        /// <summary>
        /// Returns the portion of the text that is remaining of the sequence.
        /// </summary>
        public string RemainingText
        {
            get { return _text.Substring(_progress); }
        }

        /// <summary>
        /// The sequence's text content.
        /// </summary>
        public string Text
        {
            get { return _text; }
            set { SetText(value); }
        }

        /// <summary>
        /// If enabled whitespace is optional and the next non-whitespace character is acceptable input.
        /// </summary>
        public bool OptionalWhiteSpace
        {
            get { return _optionalWhiteSpace; }
            set { _optionalWhiteSpace = value; }
        }

        /// <summary>
        /// Returns true if the sequence would accept the given input.
        /// </summary>
        public bool DoesAcceptInput(string input)
        {
            foreach (var requiredInput in AcceptableInputs)
            {
                if (TGK_Input.AreInputEqual(input, requiredInput, _isCaseSensitive))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Makes the input sequence receive and either 'accept' or 'reject' the provided input.
        /// </summary>
        public bool ReceiveInput(string input)
        {
            LastProcessedInput = input;

            int investigatedIndex = _progress;
            if (_optionalWhiteSpace && DoesMatchAtIndex(input, investigatedIndex) == false)
            {
                investigatedIndex = NextNonWhitespaceIndex();
            }

            if (DoesMatchAtIndex(input, investigatedIndex))
            {
                _progress = investigatedIndex + 1;
                OnTextUpdated(this);
                OnInputAccepted(this);
                if (IsCompleted)
                {
                    OnCompleted(this);
                }
                return true;
            }
            else
            {
                OnInputRejected(this);
                return false;
            }
        }

        /// <summary>
        /// Destroys the sequence.
        /// </summary>
        public void Remove()
        {
            OnRemoval(this);
            Destroy(gameObject);
        }

        /// <summary>
        /// Resets the progress of the sequence.
        /// </summary>
        public void ResetProgress()
        {
            SetText(_text);
        }

        /// <summary>
        /// Targets the sequence.
        /// </summary>
        public void Target()
        {
            InputSequenceManager.TargetSequence(this);
            IsTargeted = true;
            OnTargeted(this);
        }

        /// <summary>
        /// Untargets the InputSequence.
        /// </summary>
        public void Untarget()
        {
            InputSequenceManager.UntargetSequence(this);
            IsTargeted = false;
            OnUntargeted(this);
        }

        private void Awake()
        {
            InputSequenceManager.RegisterSequence(this);
        }

        private void OnDestroy()
        {
            InputSequenceManager.UnregisterSequence(this);
        }

        private void Update()
        {
            InputSequenceManager.ProcessInput();
        }

        private void OnValidate()
        {
            int tempIndex = _progress;
            if (_previousText != _text)
            {
                SetText(_text);
            }
            _progress = Mathf.Clamp(tempIndex, 0, _text.Length - 1);
            OnTextUpdated(this);
        }

        private bool DoesMatchAtIndex(string input, int investigatedIndex)
        {
            return TGK_Input.AreInputEqual(RequiredInputByIndex(investigatedIndex), input, _isCaseSensitive);
        }

        private int NextNonWhitespaceIndex()
        {
            int index = _progress;

            while (TGK_Input.IsWhitespace(RequiredInputByIndex(index)) && index < _text.Length)
            {
                index += 1;
            }

            return index;
        }

        private string RequiredInputByIndex(int index)
        {
            return _text.Substring(index, 1);
        }

        private void SetText(string text)
        {
            _previousText = _text;
            if (_optionalWhiteSpace)
            {
                _text = text.Trim();
            }
            else
            {
                _text = text;
            }
            if (text != _text)
            {
                _progressForAcceptableInputs = -1;
            }

            _progress = 0;
            OnTextUpdated(this);
        }

        private void EvaluateAcceptableInputs()
        {
            _progressForAcceptableInputs = _progress;

            _acceptableInputs.Clear();
            _acceptableInputs.Add(RequiredInputByIndex(_progress));

            if (_optionalWhiteSpace)
            {
                _acceptableInputs.Add(RequiredInputByIndex(NextNonWhitespaceIndex()));
            }

            if (_isCaseSensitive == false)
            {
                foreach (var input in _acceptableInputs.ToArray())
                {
                    _acceptableInputs.Add(input.ToLower());
                    _acceptableInputs.Add(input.ToUpper());
                }
            }
        }
    }
}
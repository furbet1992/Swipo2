using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace TypingGameKit
{
    /// <summary>
    /// A key in the virtual keyboard.
    /// </summary>
    [SelectionBase]
    public class VirtualKeyboardKey : MonoBehaviour
    {
        private enum DisplayState { Normal, Pressed, }

        [SerializeField] private DisplayState _displayState = DisplayState.Normal;

        private VirtualKeyboard _keyboard = null;

        [SerializeField] private Image _backgroundImage = null;
        [SerializeField] private Text _keyText = null;

        [SerializeField] private KeyColor _normalColor = new KeyColor() { BackgroundColor = Color.black, TextColor = Color.white };
        [SerializeField] private KeyColor _pressedColor = new KeyColor() { BackgroundColor = Color.grey, TextColor = Color.white };

        private float _pressDuration = 0f;
        [SerializeField] private float _maxPressDuration = 0.20f;

        private KeyLayerContent _currentContent = null;
        [SerializeField] private KeyLayerContent[] _layerContents = new KeyLayerContent[3];

        public string InputByLayer(VirtualKeyboardLayer layer)
        {
            return ContentByLayer(layer)?.Input ?? null;
        }

        private KeyLayerContent ContentByLayer(VirtualKeyboardLayer layer)
        {
            return (int)layer < _layerContents.Length ? _layerContents[(int)layer] : null;
        }

        public string Input { get { return CurrentContent.Input; } }

        public VirtualKeyboardLayer NextLayer { get { return CurrentContent.NextLayer; } }

        private KeyLayerContent CurrentContent
        {
            get
            {
                if (_currentContent == null)
                {
                    _currentContent = _layerContents.First();
                }
                return _currentContent;
            }
        }

        /// <summary>
        /// Sets the current layer.
        /// </summary>
        public void SetLayer(VirtualKeyboardLayer layer)
        {
            _currentContent = ContentByLayer(layer) ?? CurrentContent;
            UpdateText();
        }

        /// <summary>
        /// Initializes the key with the supplied keyboard.
        /// </summary>
        public void Initialize(VirtualKeyboard keyboard)
        {
            _keyboard = keyboard;
        }

        private void Awake()
        {
            _displayState = DisplayState.Normal;
        }

        private void Start()
        {
            Debug.Assert(_keyboard != null, $"Key not assigned to keyboard ({this})", this);
        }

        public void HandleClick()
        {
            DisplayAsPressed();
            _keyboard.HandleKeyPress(this);
        }

        private void UpdateText()
        {
            if (Application.IsPlaying(gameObject))
            {
                gameObject.SetActive(CurrentContent.Hide == false);
            }
            _keyText.text = CurrentContent.KeyDisplay;
        }

        private KeyColor ColorByState(DisplayState state)
        {
            switch (state)
            {
                case DisplayState.Normal:
                    return _normalColor;

                case DisplayState.Pressed:
                    return _pressedColor;

                default:
                    Debug.LogWarning($"Undefined color for {_displayState}");
                    return _normalColor;
            }
        }

        private void DisplayAsPressed()
        {
            SetDisplayState(DisplayState.Pressed);
            _pressDuration = _maxPressDuration;
        }

        private void OnEnable()
        {
            SetDisplayState(DisplayState.Normal);
        }

        private void OnValidate()
        {
            SetDisplayState(_displayState, true);
            foreach (var content in _layerContents)
            {
                content.OnValidate();
            }
            UpdateText();
        }

        private void SetColor(KeyColor color)
        {
            _keyText.color = color.TextColor;
            _backgroundImage.color = color.BackgroundColor;
        }

        private void SetDisplayState(DisplayState newState, bool forceUpdate = false)
        {
            if (newState != _displayState || forceUpdate)
            {
                SetColor(ColorByState(newState));
            }
            _displayState = newState;
        }

        private void Update()
        {
            UpdatePressDuration();
        }

        private void UpdatePressDuration()
        {
            if (_pressDuration > 0)
            {
                _pressDuration -= Time.unscaledDeltaTime;
            }
            if (_pressDuration <= 0 && _displayState == DisplayState.Pressed)
            {
                SetDisplayState(DisplayState.Normal);
            }
        }

        [System.Serializable]
        private class KeyLayerContent
        {
            [SerializeField] private string _input = "";
            [SerializeField] private string _display = "";
            [SerializeField] private VirtualKeyboardLayer _nextLayer = VirtualKeyboardLayer.First;
            [SerializeField] private bool _hide = false;

            public string Input
            {
                get { return _input; }
            }

            public string KeyDisplay { get { return _display; } }

            public VirtualKeyboardLayer NextLayer { get { return _nextLayer; } }

            public bool Hide
            {
                get { return _hide; }
            }

            public void OnValidate()
            {
                _input = System.Text.RegularExpressions.Regex.Unescape(_input);
                if (_display == "")
                {
                    _display = _input;
                }
            }
        }

        [System.Serializable]
        public class KeyColor
        {
            public Color BackgroundColor = Color.black;
            public Color TextColor = Color.white;
        }
    }
}
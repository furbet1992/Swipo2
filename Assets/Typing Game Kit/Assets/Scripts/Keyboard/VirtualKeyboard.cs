using System;
using System.Collections.Generic;
using UnityEngine;

namespace TypingGameKit
{
    /// <summary>
    /// A virtual keyboard that feeds key input into the TypingInput
    /// </summary>
    public partial class VirtualKeyboard : MonoBehaviour
    {
        [Tooltip("The layer that will be shown initially and in the editor.")]
        [SerializeField] private VirtualKeyboardLayer _currentLayer = VirtualKeyboardLayer.First;

        private VirtualKeyboardKey[] _keys = null;

        public VirtualKeyboardLayer CurrentLayer
        {
            get
            {
                return _currentLayer;
            }

            set
            {
                _currentLayer = value;
            }
        }

        private void OnValidate()
        {
            InitializeKeys();
            SetLayer(_currentLayer);
        }

        private void InitializeKeys()
        {
            _keys = FindObjectsOfType<VirtualKeyboardKey>();
            foreach (var key in _keys)
            {
                key.Initialize(this);
            }
        }

        /// <summary>
        /// Returns the keys of the keyboard.
        /// </summary>
        public IEnumerable<VirtualKeyboardKey> Keys
        {
            get { return _keys; }
        }

        public event Action<VirtualKeyboardKey> OnKeyPressed = delegate { };

        private void Awake()
        {
            InitializeKeys();
            SetLayer(_currentLayer);
        }

        /// <summary>
        /// Adds input from the specified key.
        /// </summary>
        public void HandleKeyPress(VirtualKeyboardKey key)
        {
            OnKeyPressed(key);
            TGK_Input.AddInput(key.Input);
            SetLayer(key.NextLayer);
        }

        private void SetLayer(VirtualKeyboardLayer layer)
        {
            _currentLayer = layer;
            foreach (var key in _keys)
            {
                key.SetLayer(layer);
            }
        }
    }
}
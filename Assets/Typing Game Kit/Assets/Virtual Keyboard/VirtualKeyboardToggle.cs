using UnityEngine;

namespace TypingGameKit

{
    public class VirtualKeyboardToggle : MonoBehaviour
    {
        private const string PREFERENCE_KEY = "_show_keyboard";
        [SerializeField] private GameObject _keyboardObject = null;

        private void Awake()
        {
            _keyboardObject.SetActive(GetPreference());
        }

        public void Toggle()
        {
            DeselectUI();
            _keyboardObject.SetActive(!_keyboardObject.activeInHierarchy);
            SavePreference();
        }

        public void DeselectUI()
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }

        private bool GetPreference()
        {
            return PlayerPrefs.GetInt(PREFERENCE_KEY, 0) != 0;
        }

        private void SavePreference()
        {
            PlayerPrefs.SetInt(PREFERENCE_KEY, (_keyboardObject.activeInHierarchy ? 1 : 0));
        }
    }
}
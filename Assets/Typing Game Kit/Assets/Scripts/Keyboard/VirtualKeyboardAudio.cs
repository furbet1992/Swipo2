using System.Linq;
using UnityEngine;

namespace TypingGameKit
{
    public class VirtualKeyboardAudio : MonoBehaviour
    {
        [SerializeField] private VirtualKeyboard _keyboard = null;
        [SerializeField] private AudioSource _source = null;
        [SerializeField] private AudioClip[] _keyPressCilps = null;

        private void Awake()
        {
            _keyboard.OnKeyPressed += HandleKeyPress;
        }

        public void HandleKeyPress(VirtualKeyboardKey _)
        {
            AudioClip clip = _keyPressCilps.OrderBy(__ => Random.Range(0f, 1f)).FirstOrDefault();
            _source.PlayOneShot(clip);
        }
    }
}
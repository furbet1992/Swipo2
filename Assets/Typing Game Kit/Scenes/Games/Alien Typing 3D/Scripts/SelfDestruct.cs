using UnityEngine;

namespace TypingGameKit.AlienTyping
{
    /// <summary>
    /// Destroys the game object after a specified amount of time.
    /// </summary>
    public class SelfDestruct : MonoBehaviour
    {
        [SerializeField]
        private float _secondsToExist = 1f;

        private void Start()
        {
            Destroy(gameObject, _secondsToExist);
        }
    }
}
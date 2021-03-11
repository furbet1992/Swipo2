using UnityEngine;

namespace TypingGameKit.AsteroidTyping
{
    /// <summary>
    /// Destroys the gameObject after a specified amount of time.
    /// </summary>
    public class SelfDestruct : MonoBehaviour
    {
        [SerializeField] private float _secondsToExist = 1f;

        private void Start()
        {
            Destroy(gameObject, _secondsToExist);
        }
    }
}
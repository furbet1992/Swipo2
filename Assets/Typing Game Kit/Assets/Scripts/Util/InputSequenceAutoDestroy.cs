using UnityEngine;

namespace TypingGameKit
{
    /// <summary>
    /// Used to automatically destroy the object after an input sequence has been completed.
    /// </summary>
    public class InputSequenceAutoDestroy : MonoBehaviour
    {
        [SerializeField] private InputSequence _inputSeqeunce = null;
        [SerializeField] private float _timeToRemain = 1;

        private void Awake()
        {
            _inputSeqeunce.OnCompleted += delegate
            {
                Destroy(gameObject, _timeToRemain);
            };
        }
    }
}
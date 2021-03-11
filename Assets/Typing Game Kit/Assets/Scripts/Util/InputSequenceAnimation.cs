using UnityEngine;

namespace TypingGameKit
{
    /// <summary>
    /// Triggers animations for an input sequence.
    /// </summary>
    public class InputSequenceAnimation : MonoBehaviour
    {
        [SerializeField] private Animator _animator = null;

        [SerializeField] protected InputSequence _sequence = null;

        private void Awake()
        {
            Debug.Assert(_animator != null, this);
            Debug.Assert(_sequence != null, this);

            _sequence.OnCompleted += OnCompleted;
            _sequence.OnInputRejected += OnInputFailed;
            _sequence.OnInputAccepted += OnInputSucceeded;

            _animator.Update(0);
        }

        private void OnCompleted(InputSequence _)
        {
            _animator.SetTrigger("Completed");
        }

        private void OnInputFailed(InputSequence _)
        {
            _animator.SetTrigger("Failed");
        }

        private void OnInputSucceeded(InputSequence sequence)
        {
            if (sequence.IsCompleted)
            {
                return;
            }

            _animator.SetTrigger("Succeeded");
        }
    }
}
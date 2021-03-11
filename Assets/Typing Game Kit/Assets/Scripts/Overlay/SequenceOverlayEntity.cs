using UnityEngine;

namespace TypingGameKit
{
    /// <summary>
    /// Defines a Sequence Overlay Entity.
    /// </summary>
    [RequireComponent(typeof(InputSequence))]
    public sealed class SequenceOverlayEntity : OverlayEntity
    {
        /// <summary>
        /// The associated input sequence.
        /// </summary>
        public InputSequence Sequence { get; private set; }

        private void Awake()
        {
            Sequence = GetComponent<InputSequence>();
            Debug.Assert(Sequence != null);
        }
    }
}
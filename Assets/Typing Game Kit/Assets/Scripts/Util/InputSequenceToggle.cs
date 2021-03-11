using UnityEngine;

namespace TypingGameKit
{
    /// <summary>
    /// Toggles the active status of two objects depending on if an object is targeted or not.
    /// </summary>
    public class InputSequenceToggle : MonoBehaviour
    {
        [SerializeField] private InputSequence _sequence = null;
        [SerializeField] private GameObject _targetObject = null;
        [SerializeField] private GameObject _untargetedObject = null;

        private void Awake()
        {
            Debug.Assert(_sequence != null, this);
            Debug.Assert(_targetObject != null, this);
            Debug.Assert(_untargetedObject != null, this);

            _sequence.OnTargeted += ShowAsTargeted;
            _sequence.OnUntargeted += ShowAsUntargeted;
            ShowAsUntargeted(_sequence);
        }

        /// <summary>
        /// Display as targeted.
        /// </summary>
        protected void ShowAsTargeted(InputSequence _)
        {
            _targetObject.SetActive(true);
            _untargetedObject.SetActive(false);
        }

        /// <summary>
        /// Display as untargeted;
        /// </summary>
        protected void ShowAsUntargeted(InputSequence _)
        {
            _targetObject.gameObject.SetActive(false);
            _untargetedObject.gameObject.SetActive(true);
        }
    }
}
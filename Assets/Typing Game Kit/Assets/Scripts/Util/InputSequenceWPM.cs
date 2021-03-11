using System.Collections.Generic;
using UnityEngine;

namespace TypingGameKit
{
    /// <summary>
    /// Tracks how fast the input sequences are typed.
    /// </summary>
    public class InputSequenceWPM : MonoBehaviour
    {
        [Tooltip("The time frame in seconds in which to measure the WPM.")]
        [SerializeField] private float _wpmTimeWindow = 5f;

        [Tooltip("Time to smooth out the change in wpm over.")]
        [SerializeField] private float _smoothTime = 2f;

        [Tooltip("If enabled erroneous input will be counted towards the wpm.")]
        [SerializeField] private bool _includeRejectedInput = false;

        private float _kpmVelocity = 0.0f;
        private float _smoothKPM = 0;

        // Queue containing time stamps of successful inputs
        private Queue<float> _wpmTimeQueue = new Queue<float>();

        /// <summary>
        /// The words per minute.
        /// </summary>
        public float WPM
        {
            get
            {
                return KPM / 5f;
            }
        }

        /// <summary>
        /// The keys per minute.
        /// </summary>
        public float KPM
        {
            get
            {
                float targetValue = _wpmTimeQueue.Count * (60 / _wpmTimeWindow);
                _smoothKPM = Mathf.SmoothDamp(_smoothKPM, targetValue, ref _kpmVelocity, _smoothTime);
                return _smoothKPM;
            }
        }

        private void Awake()
        {
            InputSequenceManager.OnInputAccepted += OnInputAccepted;
            InputSequenceManager.OnInputRejected += OnInputRejected;
        }

        private void OnDestroy()
        {
            InputSequenceManager.OnInputAccepted -= OnInputAccepted;
            InputSequenceManager.OnInputRejected -= OnInputRejected;
        }

        private void OnInputAccepted()
        {
            AddInputToQueue();
        }

        private void OnInputRejected()
        {
            if (_includeRejectedInput)
            {
                AddInputToQueue();
            }
        }

        private void AddInputToQueue()
        {
            _wpmTimeQueue.Enqueue(Time.time);
        }

        private void Update()
        {
            UpdateWPMQueue();
        }

        private void UpdateWPMQueue()
        {
            while (_wpmTimeQueue.Count > 0 && Time.time - _wpmTimeQueue.Peek() > _wpmTimeWindow)
            {
                _wpmTimeQueue.Dequeue();
            }
        }
    }
}
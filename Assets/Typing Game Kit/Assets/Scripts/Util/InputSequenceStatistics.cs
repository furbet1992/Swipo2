using System.Collections.Generic;
using UnityEngine;

namespace TypingGameKit
{
    /// <summary>
    ///  Gathers typing statistics from input sequences.
    /// </summary>
    public class InputSequenceStatistics : MonoBehaviour
    {
        // Mapping each char to a statistics object
        private Dictionary<string, CharStatistics> _statisticsDict = new Dictionary<string, CharStatistics>();

        /// <summary>
        /// The amount of inputs received that was not accepted by a sequence.
        /// </summary>
        public int RejectedInputs { get; private set; }

        /// <summary>
        /// The amount of inputs received that was a accepted by a sequence.
        /// </summary>
        public int AcceptedInputs { get; private set; }

        /// <summary>
        /// The total amount of inputs received.
        /// </summary>
        public int TotalInputs { get { return RejectedInputs + AcceptedInputs; } }

        public float Accuracy
        {
            get
            {
                return TotalInputs == 0 ? 1 : AcceptedInputs / (float)TotalInputs;
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

        //Adding int to the Score
        private void OnInputAccepted()
        {
            var input = InputSequenceManager.LatestProcessedInput;
            GetInputStats(input).RecordAcceptedInput();
            AcceptedInputs += 2;
        }

        private void OnInputRejected()
        {
            var input = InputSequenceManager.LatestProcessedInput;
            GetInputStats(input).RecordRejectedInput();
            RejectedInputs += 1;
        }

        /// <summary>
        /// Returns the statistics for a given string `input`.
        /// </summary>
        public CharStatistics GetInputStats(string input)
        {
            if (_statisticsDict.ContainsKey(input) == false)
            {
                _statisticsDict[input] = new CharStatistics();
            }
            return _statisticsDict[input];
        }

        /// <summary>
        /// Resets the statistics.
        /// </summary>
        public void Reset()
        {
            RejectedInputs = 0;
            AcceptedInputs = 0;
            _statisticsDict.Clear();
        }

        public class CharStatistics
        {
            public int AcceptedCount { get; private set; }
            public int RejectedCount { get; private set; }

            public int TotalReceivedCount { get { return AcceptedCount + RejectedCount; } }

            public void RecordAcceptedInput()
            {
                AcceptedCount += 1;
            }

            public void RecordRejectedInput()
            {
                RejectedCount += 1;
            }
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TypingGameKit
{
    /// <summary>
    /// Controls what and how input is fed into the input sequences.
    /// </summary>
    public static class InputSequenceManager
    {

        private static int _lastUpdateFrame;
        private static bool _useMultiTargeting = false;

        private static List<InputSequence> _sequences = new List<InputSequence>();
        private static List<InputSequence> _targetedSequences = new List<InputSequence>();

        /// <summary>
        /// Is raised when input successfully matched any sequence.
        /// </summary>
        public static event System.Action OnInputAccepted = delegate { };

        /// <summary>
        /// Is raised when input failed to match any sequence.
        /// </summary>
        public static event System.Action OnInputRejected = delegate { };

        /// <summary>
        /// Is raised when any sequence was completed.
        /// </summary>
        public static event System.Action OnSequenceCompleted = delegate { };

        /// <summary>
        /// Returns all the inputs that would be accepted by at least one sequence.
        /// </summary>
        public static IEnumerable<string> AcceptableInputs
        {
            get
            {
                return ReceivingSequences.SelectMany(s => s.AcceptableInputs);
            }
        }

        /// <summary>
        /// The latest processed input.
        /// </summary>
        public static string LatestProcessedInput { get; private set; }

        /// <summary>
        /// Returns the number of registered input sequences.
        /// </summary>
        public static int SequenceCount
        {
            get { return _sequences.Count(); }
        }

        /// <summary>
        /// Returns all the registered input sequences.
        /// </summary>
        public static InputSequence[] Sequences
        {
            get { return _sequences.ToArray(); }
        }

        /// <summary>
        /// Returns the targeted sequences.
        /// </summary>
        public static IEnumerable<InputSequence> TargetedSequences
        {
            get { return _targetedSequences; }
        }

        /// <summary>
        /// Sequences that currently may receive input.
        /// </summary>
        private static IEnumerable<InputSequence> ReceivingSequences
        {
            get { return _targetedSequences.Count > 0 ? _targetedSequences : _sequences; }
        }

        /// <summary>
        /// Clears the targeted sequences.
        /// </summary>
        public static void ClearTargetedSequences()
        {
            foreach (var _sequence in _targetedSequences)
            {
                _sequence.Untarget();
            }
        }

        /// <summary>
        /// Registers a sequence with the manager and enable it to receive keyboard input.
        /// </summary>
        public static void RegisterSequence(InputSequence sequence)
        {
            if (_sequences.Contains(sequence))
            {
                return;
            }
            _sequences.Add(sequence);
            sequence.OnRemoval += UnregisterSequence;
            sequence.OnTargeted += TargetSequence;
            sequence.OnUntargeted += UntargetSequence;
        }

        /// <summary>
        /// Marks registered sequences for removal.
        /// </summary>
        public static void RemoveAllSequences()
        {
            foreach (var sequence in _sequences.ToArray())
            {
                sequence.Remove();
            }
            _sequences.Clear();
            _targetedSequences.Clear();
        }

        /// <summary>
        /// Targets the given input sequence.
        /// </summary>
        public static void TargetSequence(InputSequence sequence)
        {
            if (_targetedSequences.Contains(sequence) == false)
            {
                if (_useMultiTargeting == false)
                {
                    ClearTargetedSequences();
                }
                _targetedSequences.Add(sequence);
            }
        }

        /// <summary>
        /// Untargets the given input sequence.
        /// </summary>
        public static void UntargetSequence(InputSequence sequence)
        {
            if (_targetedSequences.Contains(sequence))
            {
                _targetedSequences.Remove(sequence);
            }
        }

        /// <summary>
        /// The sequence will no longer receive input.
        /// </summary>
        public static void UnregisterSequence(InputSequence sequence)
        {
            _sequences.Remove(sequence);
            _targetedSequences.Remove(sequence);
            sequence.OnRemoval -= UnregisterSequence;
        }

        /// <summary>
        /// Forces the manager to process input unless it has already done so this frame.
        /// </summary>
        public static void ProcessInput()
        {
            if (_lastUpdateFrame != Time.frameCount)
            {
                _lastUpdateFrame = Time.frameCount;
                UpdateInput();
            }
        }

        private static void AttemptToTargetASequence(string input)
        {
            foreach (InputSequence sequence in _sequences)
            {
                if (sequence.DoesAcceptInput(input))
                {
                    sequence.Target();
                    if (_useMultiTargeting == false)
                    {
                        return;
                    }
                }
            }
            OnInputRejected();
        }

        private static void CompleteSequence(InputSequence sequence)
        {
            OnSequenceCompleted();
            UnregisterSequence(sequence);
            _targetedSequences.Remove(sequence);
            Debug.Log("Destroy"); 
        }

        private static bool SendInputToSequence(string input, InputSequence sequence)
        {
            bool successful = sequence.ReceiveInput(input);

            if (successful)
            {
                OnInputAccepted();

                if (sequence.IsCompleted)
                {
                    CompleteSequence(sequence);
                }
            }
            else
            {
                OnInputRejected();
            }

            return successful;
        }

        private static void UpdateInput()
        {
            // Ignore input if there are no sequences
            if (SequenceCount == 0)
            {
                return;
            }

            // Go through each input and try to match it to a sequence
            foreach (string input in TGK_Input.ReceivedInputs)
            {
                LatestProcessedInput = input;
                if (_targetedSequences.Any() == false)
                {
                    AttemptToTargetASequence(input);
                }

                foreach (var sequence in _targetedSequences.ToArray())
                {
                    if (sequence.IsCompleted == false)
                    {
                        bool successful = SendInputToSequence(input, sequence);
                        if (successful == false && _useMultiTargeting == false)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}
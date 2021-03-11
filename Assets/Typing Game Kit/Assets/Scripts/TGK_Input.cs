using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TypingGameKit
{
    /// <summary>
    /// Managed how input is received and processed in the Typing Game Kit
    /// </summary>
    public static class TGK_Input
    {
        private static string[] _inputs = null;
        private static int _lastInputFrame = -1;
        private static char _latestRecievedInput;
        private static string _virtualInput = "";

        /// <summary>
        /// Returns all inputs received this frame.
        /// </summary>
        public static string[] ReceivedInputs
        {
            get
            {
                UpdateInputsForThisFrame();
                return _inputs;
            }
        }

        /// <summary>
        /// Adds input manually.
        /// </summary>
        public static void AddInput(string input)
        {
            _virtualInput += input;
            UpdateInputsForThisFrame();
        }

        /// <summary>
        /// Returns true if the two given input are equal.
        /// </summary>
        public static bool AreInputEqual(string stringA, string stringB, bool isCaseSensitive)
        {
            if (IsWhitespace(stringA))
            {
                return IsWhitespace(stringB);
            }
            else if (isCaseSensitive)
            {
                return stringA == stringB;
            }
            else
            {
                return stringA.ToLower() == stringB.ToLower();
            }
        }

        /// <summary>
        /// Returns true if the given `input` has been received this frame.
        /// </summary>
        public static bool ContainsInput(string input, bool isCaseSensitive)
        {
            return ReceivedInputs.Any((i) => AreInputEqual(i, input, isCaseSensitive));
        }

        /// <summary>
        /// Returns true if the given `input` is white space.
        /// </summary>
        public static bool IsWhitespace(string input)
        {
            return input.Length > 0 && string.IsNullOrWhiteSpace(input);
        }

        private static IEnumerable<string> EvaluateCurrentInputs()
        {
            string input = UnityEngine.Input.inputString + _virtualInput;
            _virtualInput = "";

            // Add a tab character if tab was pressed
            if (Input.GetKey(KeyCode.Tab))
            {
                input += "\t";
            }

            // Return if there is no input to handle.
            if (input == "")
            {
                yield break;
            }

            // Ignore input if key is being held down.
            if (UnityEngine.Input.anyKey && UnityEngine.Input.anyKeyDown == false && _latestRecievedInput == input[0])
            {
                yield break;
            }

            _latestRecievedInput = input[input.Length - 1];

            foreach (char c in input)
            {
                yield return c.ToString();
            }
        }

        private static void UpdateInputsForThisFrame()
        {
            if (_lastInputFrame != Time.frameCount)
            {
                _lastInputFrame = Time.frameCount;
                _inputs = EvaluateCurrentInputs().ToArray();
            }
        }
    }
}
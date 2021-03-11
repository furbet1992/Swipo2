using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TypingGameKit
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TextCollection))]
    public class TextCollectionEditor : Editor
    {
        private TextCollection _texts;
        private string[] _randomSamples = new string[] { };
        private int _sampleLength = 8;
        private char[] _initials = new char[] { };

        private GUIStyle _labelStyle;
        private GUIStyle _boxStyle;
        private Vector2 _initialScroll;

        private void OnEnable()
        {
            _texts = (TextCollection)target;
            _labelStyle = EditorStyles.boldLabel;
            _boxStyle = EditorStyles.textArea;
            UpdateAnalysis();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Statistics", _labelStyle);

            EditorGUILayout.LabelField($"The collection contains {_texts.Texts.Length} texts.");

            EditorGUILayout.Separator();

            DrawInitialStatistics();

            EditorGUILayout.Separator();

            DrawTextSamples();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawInitialToggle()
        {
            EditorGUI.BeginChangeCheck();
            _texts.InitialsBreakdownByCase = EditorGUILayout.Toggle("case-sensitive", _texts.InitialsBreakdownByCase);
            if (EditorGUI.EndChangeCheck())
            {
                UpdateAnalysis();
            }
        }

        private void DrawInitialStatistics()
        {
            _initialScroll = EditorGUILayout.BeginScrollView(_initialScroll, _boxStyle);
            EditorGUILayout.LabelField($"Initial breakdown", _labelStyle);
            DrawInitialToggle();
            EditorGUILayout.LabelField($"The text {_initials.Length} distinguishable initials.");
            EditorGUILayout.Separator();
            DrawInitialBreakdown();
            EditorGUILayout.EndScrollView();
        }

        private void DrawInitialBreakdown()
        {
            EditorGUIUtility.labelWidth = 1;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Initial");
            EditorGUILayout.LabelField($"Count");
            EditorGUILayout.LabelField($"Percent");
            EditorGUILayout.EndHorizontal();

            foreach (var c in _initials.OrderByDescending(c => _texts.TextsByInitial(c, _texts.InitialsBreakdownByCase).Count()))
            {
                int count = _texts.TextsByInitial(c, _texts.InitialsBreakdownByCase).Count();
                float percent = 100 * count / (float)_texts.Texts.Length;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"{c}");
                EditorGUILayout.LabelField($"{count}");
                EditorGUILayout.LabelField($"({percent:#.##}%)");
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawTextSamples()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Random Samples", _labelStyle);
            if (GUILayout.Button("Generate"))
            {
                UpdateSample();
            }
            EditorGUILayout.EndHorizontal();
            foreach (string sample in _randomSamples)
            {
                EditorGUILayout.LabelField(sample, _boxStyle);
            }
        }

        private void UpdateAnalysis()
        {
            _texts.EvaluateTexts();
            UpdateLeadingChars();
            UpdateSample();
        }

        private void UpdateLeadingChars()
        {
            _initials = _texts.Initials(_texts.InitialsBreakdownByCase).ToArray();
        }

        private void UpdateSample()
        {
            var strings = new List<string>();

            for (int i = 0; i < _sampleLength; i++)
            {
                strings.Add(_texts.PickRandomText());
            }

            _randomSamples = strings.ToArray();
        }
    }
}
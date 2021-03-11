using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TypingGameKit
{
    /// <summary>
    /// Instantiates and manages the Sequence Overlay.
    /// </summary>
    public class SequenceOverlay : OverlayManager
    {
        [Tooltip("Specifies the prefab for the initialized overlay entities.")]
        [SerializeField] private SequenceOverlayEntity _sequenceEntityPrefab = null;

        private List<SequenceOverlayEntity> _spawnedSequences = new List<SequenceOverlayEntity>();

        /// <summary>
        /// The prefab to use when instantiating the overlay entities.
        /// </summary>
        public SequenceOverlayEntity SequenceEntityPrefab
        {
            get { return _sequenceEntityPrefab; }
            set { _sequenceEntityPrefab = value; }
        }

        /// <summary>
        /// Instantiates a new `SequenceOverlayEntity` prefab and returns its corresponding `InputSequence`
        /// </summary>
        public InputSequence CreateSequence(string text, Transform worldTarget)
        {
            SequenceOverlayEntity entity = Instantiate(_sequenceEntityPrefab, transform);
            _spawnedSequences.Add(entity);

            // Keep name updated.
            entity.Sequence.OnTextUpdated += delegate
            {
                entity.name = $"Sequence: {entity.Sequence.Text}";
            };

            entity.Target = worldTarget;
            entity.MoveToTargetPosition();
            entity.Sequence.Text = text;
            entity.Sequence.OnCompleted += delegate
            {
                InputSequenceManager.UnregisterSequence(entity.Sequence);
                _spawnedSequences.Remove(entity);
            };
            entity.Sequence.OnRemoval += delegate
            {
                _spawnedSequences.Remove(entity);
                Destroy(entity.gameObject);
            };

            return entity.Sequence;
        }

        private void UpdateDrawOrder()
        {
            // set draw order based on distance to camera and if targeted or not
            int order = 0;
            var cameraPosition = Camera.main.transform.position;
            var orderedSequences = _spawnedSequences.OrderByDescending(s => (s.Sequence.IsTargeted ? -1 : 0, s.TargetDistanceFromCamera()));
            foreach (var sequence in orderedSequences)
            {
                sequence.transform.SetSiblingIndex(order);
                order += 1;
            }
        }

        private void Awake()
        {
            Debug.Assert(GetComponentInParent<Canvas>().renderMode == RenderMode.ScreenSpaceOverlay);
            Debug.Assert(_sequenceEntityPrefab != null, this);
        }

        private void LateUpdate()
        {
            UpdateOverlaySystem();
            UpdateDrawOrder();
        }
    }
}
using UnityEngine;

namespace TypingGameKit
{
    /// <summary>
    /// Settings defining sequence display.
    /// </summary>
    public partial class OverlaySettings : MonoBehaviour
    {
        [Tooltip("If toggled the overlay entities will remain inside the overlay manager's bounds.")]
        [SerializeField] private bool _remainWithinBounds = true;

        [Tooltip("Speed at which the entities are following their targets.")]
        [Range(0, 5)]
        [SerializeField] private float _movementSmoothness = 0.5f;

        [Tooltip("Determines the strengths of the repulsion force when sequences overlap")]
        [SerializeField] private float _avoidanceStrength = 50f;

        [Space]
        [Tooltip("If toggled the overlay entities scale will be determined by their distance from the camera.")]
        [SerializeField] private bool _useDistanceScaling = false;

        [SerializeField] private DistanceScalingSettings _distanceScaleSettings = new DistanceScalingSettings();

        public float AvoidanceStrength { get => _avoidanceStrength; set => _avoidanceStrength = value; }

        public float MovementSmoothness { get => _movementSmoothness; set => _movementSmoothness = value; }

        /// <summary>
        /// The distance scaling settings!
        /// </summary>
        public DistanceScalingSettings DistanceScaleSettings
        {
            get { return _distanceScaleSettings; }
        }

        /// <summary>
        /// If true the entities will be forced to remain within the overlay manager's bounds.
        /// </summary>
        public bool RemainWithinBounds
        {
            get { return _remainWithinBounds; }
            set { _remainWithinBounds = value; }
        }

        /// <summary>
        /// Sets the distance scaling setting.
        /// </summary>
        public bool UseDistanceScaling
        {
            get { return _useDistanceScaling; }
            set { _useDistanceScaling = value; }
        }

        private void OnValidate()
        {
            _avoidanceStrength = Mathf.Max(0, _avoidanceStrength);
            if (_distanceScaleSettings != null)
            {
                _distanceScaleSettings.Validate();
            }
        }

        /// <summary>
        /// Class containing the distance scale settings.
        /// </summary>
        [System.Serializable]
        public class DistanceScalingSettings
        {
            [Tooltip("Defines the distance camera that the closest distance scale will be applied.")]
            [SerializeField] private float _closestDistance = 2;

            [Tooltip("The scale at the closest distance.")]
            [SerializeField] private float _closestDistanceScale = 1;

            [Space]
            [Tooltip("Defines the distance camera that the furthest distance scale will be applied.")]
            [SerializeField] private float _furthestDistance = 10;

            [Tooltip("Scale at the furthest distance.")]
            [SerializeField] private float _furthestDistanceScale = 0.5f;

            public float Closest { get => _closestDistance; set => _closestDistance = value; }
            public float Furthest { get => _furthestDistance; set => _furthestDistance = value; }
            public float FurthestScale { get => _furthestDistanceScale; set => _furthestDistanceScale = value; }
            public float ClosestScale { get => _closestDistanceScale; set => _closestDistanceScale = value; }

            public void Validate()
            {
                _closestDistance = Mathf.Max(0, _closestDistance);
                _furthestDistance = Mathf.Max(_closestDistance, _furthestDistance);

                _furthestDistanceScale = Mathf.Max(0, _furthestDistanceScale);
                _closestDistanceScale = Mathf.Max(_furthestDistanceScale, _closestDistanceScale);
            }
        }
    }
}
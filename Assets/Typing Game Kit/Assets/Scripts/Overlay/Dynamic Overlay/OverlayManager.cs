using System.Collections.Generic;
using UnityEngine;

namespace TypingGameKit
{
    /// <summary>
    /// Manages the dynamic overlay entities.
    /// </summary>
    public class OverlayManager : MonoBehaviour
    {
        [Tooltip("Specifies the settings to use for associated entities.")]
        [SerializeField] private OverlaySettings _settings = null;

        [Tooltip("Defines the bounds of the overlay entities if the restrict to bounds setting is enabled.")]
        [SerializeField] private RectTransform _entityBounds = null;

        [Tooltip("Camera to be used. If undefined the default camera will be used")]
        [SerializeField] private Camera _camera = null;

        private Vector3[] _cornerArray = new Vector3[4];
        private List<OverlayEntity> _entities = new List<OverlayEntity>();

        private Vector2 previousResolution;

        /// <summary>
        /// The settings used for the dynamic overlay system.
        /// </summary>
        public OverlaySettings Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }

        /// <summary>
        /// Camera to be used for determining the position of the sequences..
        /// </summary>
        private Camera UsedCamera
        {
            get { return _camera == null ? Camera.main : _camera; }
        }

        private void Awake()
        {
            Debug.Assert(_settings != null, this);
            Debug.Assert(_entityBounds != null, this);
        }

        /// <summary>
        /// Adds the entity to the system.
        /// </summary>
        public void SubscribeEntity(OverlayEntity entity)
        {
            _entities.Add(entity);
            entity.PrepareForPositionUpdate(_cornerArray, UsedCamera, _settings);
        }

        /// <summary>
        /// Removes an entity from the system.
        /// </summary>
        public void UnsubscribeEntity(OverlayEntity entity)
        {
            _entities.Remove(entity);
        }

        private void UpdateDistanceScaling()
        {
            foreach (var entity in _entities)
            {
                ApplyDistanceScaling(entity);
            }
        }

        private void UpdateSequencePositions()
        {
            foreach (var entity in _entities)
            {
                entity.UpdateMovement(_entities.ToArray());
            }
        }

        private void PrepareForUpdate()
        {
            Camera camera = UsedCamera;
            _entityBounds.GetWorldCorners(_cornerArray);
            foreach (var entity in _entities)
            {
                entity.PrepareForPositionUpdate(_cornerArray, camera, _settings);
            }
        }

        private void LateUpdate()
        {
            UpdateOverlaySystem();
        }

        protected void UpdateOverlaySystem()
        {
            CheckForResolutionChange();
            PrepareForUpdate();
            UpdateDistanceScaling();
            UpdateSequencePositions();
        }

        private void CheckForResolutionChange()
        {
            if (previousResolution.x != Screen.width || previousResolution.y != Screen.height)
            {
                foreach (var entity in _entities)
                {
                    entity.MoveToTargetPosition();
                }

                previousResolution.x = Screen.width;
                previousResolution.y = Screen.height;
            }
        }

        private void ApplyDistanceScaling(OverlayEntity entity)
        {
            if (Settings.UseDistanceScaling == false || entity.Target == null)
            {
                return;
            }

            var settings = Settings.DistanceScaleSettings;
            if (settings.Furthest == settings.Closest)
            {
                return;
            }

            // Calculate the scale with a linear equation.
            float x = Mathf.Clamp(entity.TargetDistanceFromCamera(), settings.Closest, settings.Furthest);
            float k = (settings.FurthestScale - settings.ClosestScale) / (settings.Furthest - settings.Closest);
            float m = settings.FurthestScale - k * settings.Furthest;
            float scale = k * x + m;

            entity.transform.localScale = Vector3.one * scale;
        }
    }
}
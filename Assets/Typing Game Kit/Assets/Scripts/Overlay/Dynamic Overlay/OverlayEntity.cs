using UnityEngine;

namespace TypingGameKit
{
    /// <summary>
    /// Moves a canvas element so that it overlays an object in the scene.
    /// </summary>
    public class OverlayEntity : MonoBehaviour
    {
        [Tooltip("The RectTransform that defines the bounds of the entity.")]
        [SerializeField] private RectTransform _rect = null;

        [Tooltip("The target in the scene that the entity will follow.")]
        [SerializeField] private Transform _target = null;

        private Camera _camera;
        private OverlayManager _manager;

        private Vector2 _avoidanceOffset;
        private Vector2 _desiredAvoidanceOffset;

        private Vector3[] _parentCorners;
        private Vector3[] _cornerArray = new Vector3[4];

        private OverlaySettings _settings;
        private Vector2 _targetPosition;
        private Vector2 _velocity;
        private Vector2 _virtualPosition;

        /// <summary>
        /// Array containing the corners of the canvas entity.
        /// </summary>
        public Vector3[] CornerArray
        {
            get { return _cornerArray; }
        }

        /// <summary>
        /// The transform of the object the entity will try to overlay.
        /// </summary>
        public Transform Target
        {
            get { return _target; }
            set { _target = value; }
        }

        private Vector2 ParentCenter
        {
            get
            {
                return _parentCorners[0] + (_parentCorners[2] - _parentCorners[0]) / 2;
            }
        }

        private Vector2 RectCenter
        {
            get
            {
                return _cornerArray[0] + (_cornerArray[2] - _cornerArray[0]) / 2;
            }
        }

        /// <summary>
        /// Sets the position of the entity directly on top of its target.
        /// </summary>
        public void MoveToTargetPosition()
        {
            _virtualPosition = GetTargetPosition();
            transform.position = _virtualPosition;
        }

        /// <summary>
        /// Should be called before updating the overlay entities.
        /// </summary>
        public void PrepareForPositionUpdate(Vector3[] parentCorners, Camera camera, OverlaySettings settings)
        {
            _camera = camera;
            _parentCorners = parentCorners;
            _settings = settings;
            _rect.GetWorldCorners(_cornerArray);
            _targetPosition = GetTargetPosition();
        }

        /// <summary>
        /// The distance from the used camera.
        /// </summary>
        public float TargetDistanceFromCamera()
        {
            if (_camera == null || _target == null)
            {
                return 0f;
            }
            else
            {
                return Vector3.Distance(_camera.transform.position, _target.transform.position);
            }
        }

        /// <summary>
        /// Updates the position of the entity.
        /// </summary>
        public void UpdateMovement(OverlayEntity[] movers)
        {
            if (_target == null)
            {
                return;
            }

            if (_settings.MovementSmoothness > 0)
            {
                _virtualPosition = SmoothBaseMovement(_virtualPosition, _targetPosition);
            }
            else
            {
                _virtualPosition = _targetPosition;
            }

            UpdateAvoidanceOffset(movers, _targetPosition);

            Vector2 updatedPosition = _virtualPosition + _avoidanceOffset;
            if (_settings.RemainWithinBounds)
            {
                updatedPosition = GetParentRestrictedPosition(updatedPosition);
            }

            transform.position = updatedPosition;
        }

        private static Vector2 GetRandomDirection()
        {
            return new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
        }

        private static bool Overlaps(Vector3[] corners1, Vector3[] corners2)
        {
            return corners2[2].x >= corners1[0].x &&
                   corners2[0].x <= corners1[2].x &&
                   corners2[2].y >= corners1[0].y &&
                   corners2[0].y <= corners1[2].y;
        }

        private void Awake()
        {
            Debug.Assert(_rect != null, this);
        }

        private Vector2 CalculateCollisionEscapeOffset(OverlayEntity[] movers)
        {
            Vector2 escapeOffset = Vector2.zero;
            for (int index = 0; index < movers.Length; index++)
            {
                OverlayEntity other = movers[index];
                if (other != this && Overlaps(CornerArray, other.CornerArray))
                {
                    if (Vector2.Distance(RectCenter, other.RectCenter) <= 0)
                    {
                        escapeOffset += GetRandomDirection();
                    }
                    else
                    {
                        escapeOffset += RectCenter - other.RectCenter;
                    }
                    if (_settings.RemainWithinBounds && DistanceFromParentCenter() < other.DistanceFromParentCenter())
                    {
                        escapeOffset += ParentCenter - RectCenter;
                    }
                }
            }
            return escapeOffset;
        }

        private float DistanceFromParentCenter()
        {
            return Vector2.Distance(RectCenter, ParentCenter);
        }

        private Vector2 GetParentRestrictedPosition(Vector2 desiredPosition)
        {
            Vector2 currentPosition = transform.position;
            Vector2 parentBottomLeft = _parentCorners[0];
            Vector2 parentTopRight = _parentCorners[2];
            Vector2 bottomLeft = _cornerArray[0];
            Vector2 topRight = _cornerArray[2];

            float leftOffset = bottomLeft.x - currentPosition.x;
            float rightOffset = topRight.x - currentPosition.x;
            float bottomOffset = bottomLeft.y - currentPosition.y;
            float topOffset = topRight.y - currentPosition.y;

            float x = desiredPosition.x;
            if (parentBottomLeft.x > x + leftOffset)
            {
                x = parentBottomLeft.x - leftOffset;
            }
            else if (parentTopRight.x < x + rightOffset)
            {
                x = parentTopRight.x - rightOffset;
            }

            float y = desiredPosition.y;
            if (parentBottomLeft.y > y + bottomOffset)
            {
                y = parentBottomLeft.y - bottomOffset;
            }
            else if (parentTopRight.y < y + topOffset)
            {
                y = parentTopRight.y - topOffset;
            }

            return new Vector2(x, y);
        }

        private Vector3 GetTargetPosition()
        {
            if (Target == null)
            {
                return transform.position;
            }
            Vector3 worldPosition = Target.transform.position;
            Transform cameraTransform = _camera.transform;
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 cameraPosition = cameraTransform.position;
            Vector3 cameraToPositionOffset = worldPosition - cameraPosition;

            float product = Vector3.Dot(cameraForward, cameraToPositionOffset);
            bool isBehindCamera = product <= 0;

            if (isBehindCamera)
            {
                Vector3 forwardPushedPosition = worldPosition - (1.01f * product * cameraForward);
                return _camera.WorldToScreenPoint(forwardPushedPosition);
            }
            else
            {
                return _camera.WorldToScreenPoint(worldPosition);
            }
        }

        private void OnDisable()
        {
            _manager.UnsubscribeEntity(this);
        }

        private void OnEnable()
        {
            _manager = GetComponentInParent<OverlayManager>();
            Debug.Assert(_manager != null, this);
            _manager.SubscribeEntity(this);
        }

        private Vector2 SmoothBaseMovement(Vector2 origin, Vector2 target)
        {
            return Vector2.SmoothDamp(origin, target, ref _velocity, _settings.MovementSmoothness, (target - origin).magnitude, Time.deltaTime);
        }

        private void UpdateAvoidanceOffset(OverlayEntity[] movers, Vector2 targetPosition)
        {
            if (_settings.AvoidanceStrength <= 0)
            {
                _avoidanceOffset = Vector2.zero;
                return;
            }

            Vector2 escapeOffset = CalculateCollisionEscapeOffset(movers);
            float targetFollowPiority = (targetPosition - (Vector2)transform.position).sqrMagnitude;
            float overlapEscapePriority = escapeOffset.sqrMagnitude;

            if (targetFollowPiority < overlapEscapePriority)
            {
                // increase desired velocity
                Vector2 velocityIncrease = escapeOffset.normalized * _settings.AvoidanceStrength;
                _desiredAvoidanceOffset += velocityIncrease * Time.deltaTime;
            }
            else
            {
                // let desired velocity fade over time
                _desiredAvoidanceOffset -= _desiredAvoidanceOffset * Time.deltaTime;
            }

            // smoothly increase/decrease actual velocity.
            _avoidanceOffset = Vector2.Lerp(_avoidanceOffset, _desiredAvoidanceOffset, Time.fixedDeltaTime);
        }
    }
}
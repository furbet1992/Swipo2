using UnityEngine;

namespace TypingGameKit.Demo
{
    /// <summary>
    /// Handles the demo UI.
    /// </summary>
    public class DemoUI : MonoBehaviour
    {
        private DemoManager _demoManager;

        [SerializeField] private UnityEngine.UI.Toggle _restrictToParentToggle = null;
        [SerializeField] private UnityEngine.UI.Toggle _useDistanceScalingToggle = null;
        [SerializeField] private UnityEngine.UI.Toggle _useOverlapAvoidanceToggle = null;
        [SerializeField] private UnityEngine.UI.Toggle _useSoftMovementToggle = null;
        [SerializeField] private UnityEngine.UI.Toggle _sequenceRespawnToggle = null;

        [SerializeField] private AudioClip _interactionClip = null;

        public void ClearSequences()
        {
            PlayClip(_interactionClip);
            InputSequenceManager.RemoveAllSequences();
        }

        public void AddSequence()
        {
            PlayClip(_interactionClip);
            _demoManager.AddSequenceObject();
        }

        private void PlayClip(AudioClip clip)
        {
            GetComponent<AudioSource>().PlayOneShot(clip);
        }

        public void DeselectUI()
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }

        public void SetCamerHorizontalAngle(float angle)
        {
            var oldEuler = Camera.main.transform.eulerAngles;
            Camera.main.transform.eulerAngles = new Vector3(oldEuler.x, angle, oldEuler.z);
        }

        public void SetRotationSpeed(float angle)
        {
            FindObjectOfType<DemoAnchorParent>().AngleRotatedPerSecond = angle;
        }

        private void Awake()
        {
            _demoManager = FindObjectOfType<DemoManager>();

            InitializeToggle(_useSoftMovementToggle, SetSoftPositioning);
            InitializeToggle(_useOverlapAvoidanceToggle, SetOverlapAvoidance);
            InitializeToggle(_useDistanceScalingToggle, SetDistanceScaling);
            InitializeToggle(_restrictToParentToggle, SetParentRestriction);
            InitializeToggle(_sequenceRespawnToggle, SetRespawnBool);
        }

        private void SetRespawnBool(bool value)
        {
            _demoManager.SetRespawn(value);
        }

        private void InitializeToggle(UnityEngine.UI.Toggle toggle, UnityEngine.Events.UnityAction<bool> valueChangedHandler)
        {
            // Set the current settings
            valueChangedHandler(toggle.isOn);
            // Add listeners
            toggle.onValueChanged.AddListener(valueChangedHandler);
            toggle.onValueChanged.AddListener(PlayToggleSound);
        }

        private void PlayToggleSound(bool _)
        {
            PlayClip(_interactionClip);
        }

        public void SetOverlapAvoidance(bool value)
        {
            _demoManager.SequenceSettings.AvoidanceStrength = value ? 25f : 0f;
        }

        public void SetSoftPositioning(bool value)
        {
            _demoManager.SequenceSettings.MovementSmoothness = value ? 0.5f : 0f;
        }

        private void SetDistanceScaling(bool value)
        {
            _demoManager.SequenceSettings.UseDistanceScaling = value;
            if (value)
            {
                return;
            }
            // reset the scale of the sequences
            foreach (var sequence in FindObjectsOfType<SequenceOverlayEntity>())
            {
                sequence.transform.localScale = Vector3.one;
            }
        }

        private void SetParentRestriction(bool value)
        {
            _demoManager.SequenceSettings.RemainWithinBounds = value;
        }
    }
}
using UnityEngine;

namespace TypingGameKit.Demo
{
    /// <summary>
    /// Sets up and manages the interactive demo.
    /// </summary>
    public class DemoManager : MonoBehaviour
    {
        private AudioSource _audioSource;

        [SerializeField] private int _initualSequenceCount = 10;
        [SerializeField] private GameObject _anchorObject = null;
        [SerializeField] private SequenceOverlay _sequenceOverlay = null;
        [SerializeField] private OverlaySettings _overlaySettings = null;
        [SerializeField] private TextCollection _texts = null;
        [SerializeField] private float _positionRange = 10;
        [SerializeField] private Transform _anchorParent = null;

        [SerializeField] private AudioClip _inputFailedClip = null;
        [SerializeField] private AudioClip _inputSucceededClip = null;

        private bool _respawn = true;

        public SequenceOverlay SequenceOverlay
        {
            get { return _sequenceOverlay; }
        }

        public OverlaySettings SequenceSettings
        {
            get { return _overlaySettings; }
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            SpawnInitialSequences();

            InputSequenceManager.OnInputRejected += PlayInputFailed;
            InputSequenceManager.OnInputAccepted += PlayInputSuccessfull;
        }

        private void OnDestroy()
        {
            InputSequenceManager.OnInputRejected -= PlayInputFailed;
            InputSequenceManager.OnInputAccepted -= PlayInputSuccessfull;
        }

        private void PlayInputSuccessfull()
        {
            PlayClip(_inputSucceededClip);
        }

        private void PlayInputFailed()
        {
            PlayClip(_inputFailedClip);
        }

        public void SetRespawn(bool value)
        {
            _respawn = value;
        }

        private Vector3 RandomPosition()
        {
            float x = Random.Range(-_positionRange, _positionRange);
            float y = Random.Range(-_positionRange, _positionRange);
            float z = Random.Range(-_positionRange, _positionRange);
            return new Vector3(x, y, z);
        }

        private void SpawnInitialSequences()
        {
            for (int i = 0; i < _initualSequenceCount; i++)
            {
                AddSequenceObject();
            }
        }

        public void AddSequenceObject()
        {
            GameObject anchor = Instantiate(_anchorObject, _anchorParent, true);
            anchor.transform.position = RandomPosition();
            AttachSequence(anchor);
        }

        private void AttachSequence(GameObject obj)
        {
            string textSequence = _texts.FindUniquelyTargetableText();
            InputSequence sequence = _sequenceOverlay.CreateSequence(textSequence, obj.transform);
            sequence.OnCompleted += delegate
            {
                Destroy(obj);
                if (_respawn)
                {
                    AddSequenceObject();
                }
            };
            sequence.OnRemoval += delegate
            {
                Destroy(obj);
            };
        }

        private void PlayClip(AudioClip clip)
        {
            _audioSource.PlayOneShot(clip);
        }
    }
}
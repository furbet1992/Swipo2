using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace TypingGameKit.AlienTyping
{
    /// <summary>
    /// The enemy component manages the enemy AI, sound, and animation.
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private Animator _animator = null;
        [SerializeField] private GameObject _remains = null;

        [SerializeField] private float _attackRate = 5f;
        [SerializeField] private int _attackStrength = 1;

        [SerializeField] private Transform _sequenceAnchor = null;

        [Range(0, 10)]
        [SerializeField] private int _sequenceDifficulty = 1;

        private NavMeshAgent _agent;
        private AudioSource _audioSource;

        private bool _hasBeenDiscovered = false;
        private float _lastAttackTime;

        /// <summary>
        /// Raised when the enemy has been killed.
        /// </summary>
        public event System.Action<Enemy> OnDeath;

        private Vector3 CenterPosition
        {
            get { return transform.position + 0.5f * _agent.height * Vector3.up; }
        }

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _audioSource = GetComponent<AudioSource>();

            enabled = false;
            _hasBeenDiscovered = false;
        }

        private void Update()
        {
            CreateSequenceIfDiscoveredByPlayer();
            UpdateMovement();
            AttackIfPossible();
        }

        private void CreateSequenceIfDiscoveredByPlayer()
        {
            if (_hasBeenDiscovered == false && CanBeSeenByPlayer())
            {
                _hasBeenDiscovered = true;
                CreateSequence();
            }
        }

        private void UpdateMovement()
        {
            // keep playing movement animation until velocity is zero
            _animator.SetBool("IsMoving", _agent.velocity != Vector3.zero);

            // keep destination updated
            _agent.SetDestination(Player.Instance.transform.position);
        }

        private void AttackIfPossible()
        {
            if (DestinationReached() && Time.time - _lastAttackTime > _attackRate)
            {
                Attack();
            }
        }

        private void Attack()
        {
            _animator.SetTrigger("Attack");
            _lastAttackTime = Time.time;
        }

        private bool CanBeSeenByPlayer()
        {
            Vector3 origin = CenterPosition;
            Vector3 target = Player.Instance.transform.position + Vector3.up;
            Vector3 direction = target - origin;

            RaycastHit hitInfo;
            return Physics.Raycast(origin, direction, out hitInfo) && hitInfo.transform.CompareTag("Player");
        }

        private void CreateSequence()
        {
            var sequence = AlienGameManager.Instance.CreateSequence(_sequenceAnchor, _sequenceDifficulty);
            sequence.OnInputAccepted += delegate { Player.Instance.LaserGun.FireAt(CenterPosition); };
            sequence.OnCompleted += delegate { Die(); };
        }

        private bool DestinationReached()
        {
            return _agent.pathPending == false &&
                (_agent.remainingDistance <= _agent.stoppingDistance) &&
                (_agent.hasPath == false || _agent.velocity == Vector3.zero);
        }

        private void Die()
        {
            OnDeath(this);
            Instantiate(_remains, CenterPosition, transform.rotation);
            Destroy(gameObject);
        }

        private void PlaySound(AudioClip clip)
        {
            if (clip && _audioSource.isPlaying == false)
            {
                _audioSource.clip = clip;
                _audioSource.Play();
            }
        }

        private void RegisterAttackHit()
        {
            Player.Instance.TakeDamage(_attackStrength);
        }
    }
}
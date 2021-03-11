using UnityEngine;

namespace TypingGameKit.AlienTyping
{
    /// <summary>
    /// Manges the player on the path through the level.
    /// </summary>
    public class PlayerPath : MonoBehaviour
    {
        [SerializeField] private Animator _animator = null;
        [SerializeField] private int _startSection = 0;
        [SerializeField] private EnemySection[] _sections = null;

        private int _sectionIndex = 0;

        public event System.Action OnCompletion = delegate { };

        public void Start()
        {
            SetPathSpeed(1);
        }

        // called by path animation
        private void PathCompleted()
        {
            OnCompletion();
        }

        // called by path animation
        private void SectionReached()
        {
            SetPathSpeed(0);
            if (_sectionIndex < _startSection)
            {
                // skip section
                SectionCompleted();
            }
            else
            {
                _sections[_sectionIndex].Begin(SectionCompleted);
            }
        }

        private void SectionCompleted()
        {
            SetPathSpeed(1);
            _sectionIndex += 1;
        }

        private void SetPathSpeed(float value)
        {
            Player.Instance.SetWalk(value > 0);

            _animator.speed = value;
        }
    }
}
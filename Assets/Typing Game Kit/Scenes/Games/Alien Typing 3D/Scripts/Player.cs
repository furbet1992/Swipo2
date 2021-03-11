using UnityEngine;

namespace TypingGameKit.AlienTyping
{
    /// <summary>
    /// Handles player's the health, animations, and actions.
    /// </summary>
    public class Player : MonoBehaviour
    {
        [SerializeField] private Animator _animator = null;
        [SerializeField] private LaserGun _gun = null;
        [SerializeField] private int _health = 10;
        [SerializeField] private CreatureSound _sound = null;

        public event System.Action<float> OnHealthChanged = delegate { };

        public static Player Instance { get; private set; }

        public int Health
        {
            get { return _health; }
        }

        public LaserGun LaserGun
        {
            get { return _gun; }
        }

        public int HitsTaken { get; private set; }

        public void TakeDamage(int amount)
        {
            HitsTaken += 1;
            _health -= amount;
            OnHealthChanged(_health);
            _animator.SetTrigger("OnHit");
            _sound.PlayHurt();
        }

        private void Awake()
        {
            Instance = this;
        }

        public void SetWalk(bool value)
        {
            _animator.SetBool("IsWalking", value);
        }
    }
}
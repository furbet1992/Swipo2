using System;
using UnityEngine;

namespace TypingGameKit.AlienTyping
{
    /// <summary>
    /// A section in the game designates a group of enemies that has to be
    /// disposed of before the player can proceed.
    /// </summary>
    public class EnemySection : MonoBehaviour
    {
        [SerializeField] private float _completionDelay = 1f;

        private Action _onSectionCompleted;
        private Enemy[] _enemies;
        private int _enemiesDownCount;

        /// <summary>
        /// Begins the section. The passed callback will be called when the section is complete.
        /// </summary>
        public void Begin(System.Action sectionCompletedCallback)
        {
            _onSectionCompleted = sectionCompletedCallback;
            _enemies = GetComponentsInChildren<Enemy>();

            if (_enemies.Length == 0)
            {
                // skip section if no enemies
                CompleteSection();
                return;
            }

            foreach (var enemy in _enemies)
            {
                enemy.enabled = true;
                enemy.OnDeath += HandleEnemyDeath;
            }
        }

        private void CompleteSection()
        {
            _onSectionCompleted();
        }

        private void HandleEnemyDeath(Enemy enemy)
        {
            _enemiesDownCount += 1;

            if (_enemies.Length == _enemiesDownCount)
            {
                Invoke("CompleteSection", _completionDelay);
            }
        }
    }
}
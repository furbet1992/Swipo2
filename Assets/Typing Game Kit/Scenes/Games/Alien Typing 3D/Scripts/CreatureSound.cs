using System.Linq;
using UnityEngine;

namespace TypingGameKit.AlienTyping
{
    /// <summary>
    /// Handles a creatures sound
    /// </summary>
    public class CreatureSound : MonoBehaviour
    {
        [SerializeField] private AudioSource _source = null;

        [SerializeField] private AudioClip[] _hurtClips = null;
        [SerializeField] private AudioClip[] _stepClips = null;
        [SerializeField] private AudioClip[] _attackSounds = null;

        public AudioClip GetRandom(AudioClip[] _clips)
        {
            return _clips.OrderBy(_ => Random.Range(0, 1)).FirstOrDefault();
        }

        public void PlayStep()
        {
            _source.PlayOneShot(GetRandom(_stepClips));
        }

        public void PlayAttack()
        {
            _source.PlayOneShot(GetRandom(_attackSounds));
        }

        public void PlayHurt()
        {
            _source.PlayOneShot(GetRandom(_hurtClips));
        }
    }
}
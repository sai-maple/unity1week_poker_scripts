using System.Collections.Generic;
using System.Linq;
using Laughter.Poker.Domain.Enum;
using UnityEngine;

namespace Laughter.Poker.View.UI.Audio
{
    public class AudioView : MonoBehaviour
    {
        public static AudioView Instance;

        [SerializeField] private AudioSource _bgm;
        private readonly List<AudioSource> _audioSources = new();
        private float _volume;
        private readonly Dictionary<Sounds, AudioClip> _clips = new();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }


        public void PlayOneShot(Sounds sound)
        {
            var source = _audioSources.FirstOrDefault(a => !a.isPlaying);
            if (source == null)
            {
                source = AddAudioSource(false);
                _audioSources.Add(source);
            }

            var clip = GetClip(sound);

            source.clip = clip;
            source.PlayOneShot(clip);
        }

        public void StopOneShot(Sounds sound)
        {
            var clip = GetClip(sound);
            var source = _audioSources.FirstOrDefault(a => a.isPlaying && a.clip == clip);
            if (source != null)
            {
                source.Stop();
            }
        }

        public void SetVolume(float volume)
        {
            _volume = volume;
            _bgm.volume = volume;
            foreach (var audioSource in _audioSources)
            {
                audioSource.volume = volume;
            }
        }

        private AudioClip GetClip(Sounds sound)
        {
            if (_clips.TryGetValue(sound, out var clip))
            {
                return clip;
            }

            clip = Resources.Load($"Sounds/{sound}") as AudioClip;
            _clips[sound] = clip;
            return clip;
        }

        private AudioSource AddAudioSource(bool isLoop)
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = isLoop;
            source.volume = _volume;
            return source;
        }
    }
}

using System;
using System.Collections.Generic;
using _root.Scripts.Managers.Sound;
using _root.Scripts.SingleTon;
using UnityEngine;

namespace _root.Scripts.Managers
{
    public class SoundManager : SingleMono<SoundManager>
    {
        [SerializeField] public List<SoundResource> sounds;

        private Dictionary<SoundKey, AudioSource> _soundDictionary;

        protected override void Awake()
        {
            base.Awake();
            _soundDictionary = new Dictionary<SoundKey, AudioSource>();
            var root = new GameObject("@Sounds");
            DontDestroyOnLoad(root);
            foreach (var sound in sounds)
            {
                if (_soundDictionary.ContainsKey(sound.key)) continue;
                var soundObject = new GameObject(sound.key.ToString());
                var audioSource = soundObject.AddComponent<AudioSource>();

                audioSource.playOnAwake = false;
                audioSource.clip = sound.audio;
                audioSource.pitch = sound.pitch;
                audioSource.volume = sound.volume;
                audioSource.loop = sound.loop;

                soundObject.transform.SetParent(root.transform);
                _soundDictionary.Add(sound.key, audioSource);
            }
        }

        public void PlayEffectSound(SoundKey key)
        {
            if (_soundDictionary.TryGetValue(key, out var audioSource))
                audioSource.PlayOneShot(audioSource.clip, audioSource.volume);
        }

        public void PlaySound(SoundKey key)
        {
            if (!_soundDictionary.TryGetValue(key, out var audioSource)) return;
            if (audioSource.isPlaying) audioSource.Stop();
            audioSource.Play();
        }

        public void StopSound(SoundKey key)
        {
            if (!_soundDictionary.TryGetValue(key, out var audioSource)) return;
            if (audioSource.isPlaying) audioSource.Stop();
        }

        public void StopAllLoopSound()
        {
            foreach (var (_, value) in _soundDictionary)
                if (value.loop && value.isPlaying)
                    value.Stop();
        }

        public void StopAllSound()
        {
            foreach (var (_, value) in _soundDictionary)
                if (value.isPlaying)
                    value.Stop();
        }

        [Serializable]
        public struct SoundResource
        {
            [SerializeField] public SoundKey key;
            [SerializeField] public AudioClip audio;
            [SerializeField] [Range(0f, 1f)] public float volume;
            [SerializeField] [Range(-3f, 3f)] public float pitch;
            [SerializeField] public bool loop;
        }
    }
}
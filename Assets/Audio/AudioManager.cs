using System;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        public Sound[] sounds;

        private Dictionary<string, Sound> _soundDict;

        private void Awake()
        {
            _soundDict = new Dictionary<string, Sound>();
            foreach (var sound in sounds)
            {
                sound.source = gameObject.AddComponent<AudioSource>();
                sound.source.clip = sound.audioClip;
                
                _soundDict.Add(sound.name, sound);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void Play(string name)
        {
            Sound sound;
            if(!_soundDict.TryGetValue(name, out sound))
                return;
            
            sound.source.PlayOneShot(sound.audioClip);
        }
    }
}

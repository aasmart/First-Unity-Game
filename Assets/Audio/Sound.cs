using System;
using UnityEngine;

namespace Audio
{
    [Serializable]
    public class Sound
    {
        public string name;
        public AudioClip audioClip;

        [HideInInspector] public AudioSource source;
    }
}

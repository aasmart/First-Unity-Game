using UnityEngine;

namespace Audio
{
    public class DestroyAfterPlaying : MonoBehaviour
    {
        public AudioSource audioSource;
        private void Awake()
        {
            Destroy(gameObject, audioSource.clip.length);
        }
    }
}

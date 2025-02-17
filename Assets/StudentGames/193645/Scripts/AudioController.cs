using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _193645
{
    public class AudioController : MonoBehaviour
    {
        private AudioSource audioSource;
        [SerializeField] private AudioClip cherryPickUpSound;
        [SerializeField] private AudioClip heartPickUpSound;
        [SerializeField] private AudioClip gemPickUpSound;
        [SerializeField] private AudioClip fallSound;
        [SerializeField] private AudioClip enemyDeathSound;
        [SerializeField] private AudioClip jumpSound;
        [SerializeField] private AudioClip dashSound;
        [SerializeField] private AudioClip checkpointSound;

        [SerializeField] private Slider volumeSlider;

        private void Awake()
        {
            audioSource = GetComponentInChildren<AudioSource>();
            GameManager.instance.SetVolumeSlider(volumeSlider);
        }
        public void playBonusSound()
        {
            audioSource.PlayOneShot(cherryPickUpSound, AudioListener.volume);
        }
        public void playHeartSound()
        {
            audioSource.PlayOneShot(heartPickUpSound, AudioListener.volume);
        }
        public void playGemSound()
        {
            audioSource.PlayOneShot(gemPickUpSound, AudioListener.volume);
        }
        public void playFallSound()
        {
            audioSource.PlayOneShot(fallSound, AudioListener.volume);
        }
        public void playEnemySound()
        {
            audioSource.PlayOneShot(enemyDeathSound, AudioListener.volume);
        }
        public void playJumpSound()
        {
            audioSource.PlayOneShot(jumpSound, AudioListener.volume);
        }
        public void playDashSound()
        {
            audioSource.PlayOneShot(dashSound, AudioListener.volume);
        }
        public void playCheckpointSound()
        {
            audioSource.PlayOneShot(checkpointSound, AudioListener.volume);
        }
        private void changeMasterVolume()
        {
            AudioListener.volume = volumeSlider.value;
            PlayerPrefs.SetFloat("MasterVolume", AudioListener.volume);
        }

        private void Update()
        {
            if (AudioListener.volume != volumeSlider.value) changeMasterVolume();
        }
    }
}
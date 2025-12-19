using System;
using UnityEngine;

namespace EndlessPlane.Core
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField]
        AudioSource _audioSource = null;

        [Header("BG music")]
        [SerializeField]
        AudioClip _bgMusic = null;

        [Header("SFX")]
        [SerializeField]
        AudioData _obstacleHitSFX = null;
        [SerializeField]
        AudioData _buttonSFX = null;

        public static SoundManager Instance => s_instance;
        static SoundManager s_instance;

        void Awake()
        {
            s_instance = this;
        }

        public void PlayBGMusic()
        {
            _audioSource.clip = _bgMusic;
            _audioSource.Play();
        }

        public void StopAudios()
        {
            _audioSource.Stop();
        }

        public void Mute()
        {
            _audioSource.mute = true;
        }

        public void UnMute()
        {
            _audioSource.mute = false;
        }

        public void PlayObstacleHitSFX()
        {
            if (_audioSource.isPlaying)
                _audioSource.PlayOneShot(_obstacleHitSFX.AudioClipRef, _obstacleHitSFX.VolumeScale);
        }

        public void PlayButtonSFX()
        {
            _audioSource.PlayOneShot(_buttonSFX.AudioClipRef, _buttonSFX.VolumeScale);
        }
    }

    [Serializable]
    public class AudioData
    {
        [SerializeField]
        AudioClip _audioClip = null;
        [SerializeField]
        float _volumeScale = 1.0f;

        public AudioClip AudioClipRef => _audioClip;
        public float VolumeScale => _volumeScale;
    }
}

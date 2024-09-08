using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace MyGame.Script.CoreGame
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] public AudioSource bgAudioSource;
        [SerializeField] public AudioSource effAudioSource;
        [SerializeField] private List<AudioClip> backgroundAudioClip;
        [SerializeField] private List<AudioClip> effectAudioClip;

        private List<AudioSource> _audioSourceArr;
        [Button("Audio Init")]
        public void OnAudioInit()
        {
            _audioSourceArr = new List<AudioSource>
            {
                bgAudioSource,
                effAudioSource
            };
            
            backgroundAudioClip = new List<AudioClip>();
            backgroundAudioClip.AddRange(Resources.LoadAll("Audio/BackgroundAudio",typeof(AudioClip)));
        
            effectAudioClip = new List<AudioClip>();
            effectAudioClip.AddRange(Resources.LoadAll("Audio/EffectsAudio",typeof(AudioClip)));
        }
    }
}

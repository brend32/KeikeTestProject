using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

namespace Game
{
    public class GameStartSound : MonoBehaviour
    {
        [SerializeField] private LocalizeAudioClipEvent _clipEvent;
        [SerializeField] private AudioSource _audioSource;

        public void SetAudio(LocalizedAudioClip clip)
        {
            _clipEvent.AssetReference = clip;
        }

        public void Play()
        {
            _audioSource.Play();
        }
        
        public async UniTask PlayAndWait()
        {
            var token = destroyCancellationToken;

            if (_clipEvent.AssetReference != null)
            {
                await UniTask.WaitUntil(() => _audioSource.clip != null, cancellationToken: token);

                _audioSource.Play();
                await UniTask.Delay(TimeSpan.FromSeconds(_audioSource.clip.length), cancellationToken: token);
            }
        }
    }
}
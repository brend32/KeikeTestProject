using System;
using Cysharp.Threading.Tasks;
using Services;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using Zenject;

namespace Game
{
    public class GameEndSound : MonoBehaviour
    {
        [SerializeField] private LocalizeAudioClipEvent _clipEvent;
        [SerializeField] private AudioSource _audioSource;
        
        private AudiosService _audiosService;

        [Inject]
        public void Construct(AudiosService audiosService)
        {
            _audiosService = audiosService;
        }
        
        public async UniTask PlayAndWait()
        {
            var token = destroyCancellationToken;
            _clipEvent.AssetReference = _audiosService.Praises.GetRandomClip();

            if (_clipEvent.AssetReference != null)
            {
                await UniTask.WaitUntil(() => _audioSource.clip != null, cancellationToken: token);

                _audioSource.Play();
                await UniTask.Delay(TimeSpan.FromSeconds(_audioSource.clip.length), cancellationToken: token);
            }
        }
    }
}
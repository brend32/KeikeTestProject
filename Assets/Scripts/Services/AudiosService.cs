using System;
using System.Threading;
using Configurations;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using Utils;
using Zenject;

namespace Services
{
    public class AudiosService : IDisposable
    {
        public AudiosCatalogue Praises => _settings.PraisesRef.GetAsset();
        public AudiosCatalogue GameStarters => _settings.GameStartersRef.GetAsset();
        
        private readonly Settings _settings;

        [Inject]
        public AudiosService(Settings settings)
        {
            _settings = settings;
        }

        [Serializable]
        public class Settings
        {
            public AssetReferenceT<AudiosCatalogue> PraisesRef;
            public AssetReferenceT<AudiosCatalogue> GameStartersRef;
        }

        public async UniTask Download(CancellationToken cancellationToken)
        {
            var handle = Addressables.DownloadDependenciesAsync(new[]
            {
                _settings.PraisesRef,
                _settings.GameStartersRef
            }, Addressables.MergeMode.UseFirst, true);

            await handle.WithCancellation(cancellationToken);
        }

        public async UniTask Load(CancellationToken cancellationToken)
        {
            await UniTask.WhenAll(
                _settings.PraisesRef.LoadAssetAsync().WithCancellation(cancellationToken),
                _settings.GameStartersRef.LoadAssetAsync().WithCancellation(cancellationToken)
                );
        }
        
        public void Dispose()
        {
            _settings.PraisesRef.Release();
            _settings.GameStartersRef.Release();
        }
    }
}
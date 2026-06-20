using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Configurations;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using Utils;

namespace Services
{
    public class LevelsService : IDisposable
    {
        public IReadOnlyList<LevelsCategoryCatalogue> Categories { get; private set; }
        
        private readonly Settings _settings;

        public LevelsService(Settings settings)
        {
            _settings = settings;
        }

        [Serializable]
        public class Settings
        {
            public AssetReferenceT<LevelsCategoryCatalogue>[] Categories;
        }
        
        public async UniTask Download(CancellationToken cancellationToken)
        {
            await Addressables.DownloadDependenciesAsync(_settings.Categories, Addressables.MergeMode.UseFirst, true).WithCancellation(cancellationToken);
        }

        public async UniTask Load(CancellationToken cancellationToken)
        {
            await UniTask.WhenAll(
                _settings.Categories.Select(a => a.LoadAssetAsync().WithCancellation(cancellationToken))
            );

            Categories = _settings.Categories.Select(a => a.GetAsset()).ToList();
        }

        public void Dispose()
        {
            foreach (var reference in _settings.Categories)
            {
                reference.Release();
            }

            Categories = null;
        }
    }
}
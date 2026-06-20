using System;
using System.Threading;
using Configurations;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Utils;
using Zenject;

namespace Services
{
    public class ShapesService : IDisposable
    {
        private ShapesCatalogue ShapesCatalogue => _settings.ShapesRef.GetAsset();
        
        private readonly Settings _settings;

        [Inject]
        public ShapesService(Settings settings)
        {
            _settings = settings;
        }

        [Serializable]
        public class Settings
        {
            public AssetReferenceT<ShapesCatalogue> ShapesRef;
            public AssetReferenceSprite FallbackShapeRef;
        }

        public async UniTask Download(CancellationToken cancellationToken)
        {
            await Addressables.DownloadDependenciesAsync(new object[]
            {
                _settings.ShapesRef,
                _settings.FallbackShapeRef
            }, Addressables.MergeMode.UseFirst, true).WithCancellation(cancellationToken);
        }

        public async UniTask Load(CancellationToken cancellationToken)
        {
            await UniTask.WhenAll(
                _settings.ShapesRef.LoadAssetAsync().WithCancellation(cancellationToken),
                _settings.FallbackShapeRef.LoadAssetAsync().WithCancellation(cancellationToken)
            );
        }

        public AssetReferenceSprite GetShape(string key)
        {
            if (ShapesCatalogue.Shapes.TryGetValue(key, out var reference))
            {
                return reference.Copy();
            }
            
            Debug.LogError($"Shape with key {key} is not present in the catalogue. Fallback shape is returned instead");
            return _settings.FallbackShapeRef.Copy();
        }

        public void Dispose()
        {
            _settings.ShapesRef.Release();
            _settings.FallbackShapeRef.Release();
        }
    }
}
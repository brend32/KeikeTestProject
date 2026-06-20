using System;
using System.Threading;
using Configurations.JSON;
using Cysharp.Threading.Tasks;
using Models;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Utils;
using Views;
using Zenject;

namespace ViewModels
{
    public class LevelViewModel : IDisposable
    {
        public event Action<LevelViewModel> OnLevelOpenRequested; 
        
        public LevelModel Model { get; private set; }
        public LevelView View { get; }

        private AssetReferenceSprite _image;
        private readonly AssetReferenceT<TextAsset> _levelAsset;
        private readonly LevelModel.Factory _factory;

        [Inject]
        public LevelViewModel(AssetReferenceT<TextAsset> levelAsset, LevelModel.Factory factory, LevelView.Pool pool)
        {
            _levelAsset = levelAsset;
            _factory = factory;
            View = pool.Spawn();
            
            View.Bind(this);
        }

        public async UniTask DownloadAndLoad(CancellationToken cancellationToken)
        {
            View.DisplayLoading();
            await Addressables.DownloadDependenciesAsync(_levelAsset, true).WithCancellation(cancellationToken);

            Model = await _factory.Create(_levelAsset, cancellationToken);

            _image = Model.Shape.Shape;
            var sprite = await _image.LoadAssetAsync().WithCancellation(cancellationToken);
            
            View.DisplayLoaded(sprite, Model.Color);
        }

        public void OpenLevel()
        {
            OnLevelOpenRequested?.Invoke(this);
        }

        public void Dispose()
        {
            _levelAsset.Release();
            _image?.Release();
            _image = null;
            
            Model = null;
        }

        public class Factory : PlaceholderFactory<AssetReferenceT<TextAsset>, LevelViewModel>
        {
            
        }
    }
}
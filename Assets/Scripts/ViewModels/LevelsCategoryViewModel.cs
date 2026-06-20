using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using EntryPoints;
using Models;
using Services.SceneLoader;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Views;
using Zenject;

namespace ViewModels
{
    public class LevelsCategoryViewModel : IDisposable
    {
        public LevelsCategoryModel Model { get; private set; }
        public LevelsCategoryView View { get; }
        
        private List<LevelViewModel> _levels;
        private readonly LevelViewModel.Factory _levelFactory;
        private readonly GameEntryPoint.Args.Factory _gameArgsFactory;
        private readonly SceneLoaderService _sceneLoaderService;

        [Inject]
        public LevelsCategoryViewModel(LevelViewModel.Factory levelFactory, 
            LevelsCategoryModel model, 
            LevelsCategoryView.Pool pool, 
            GameEntryPoint.Args.Factory gameArgsFactory, 
            SceneLoaderService sceneLoaderService)
        {
            _levelFactory = levelFactory;
            Model = model;
            _gameArgsFactory = gameArgsFactory;
            _sceneLoaderService = sceneLoaderService;
            View = pool.Spawn();
            
            View.Bind(this);
        }

        public async UniTask DownloadAndLoad(CancellationToken cancellationToken)
        {
            _levels = Model.Asset.Levels.Select(asset =>
            {
                var level = _levelFactory.Create(asset);
                level.OnLevelOpenRequested += OnOpenLevelRequested;
                
                return level;
            }).ToList();
            
            View.DisplayHostedViews(_levels.Select(l => l.View), Model.Title);
            
            await UniTask.WhenAll(_levels.Select(l => l.DownloadAndLoad(cancellationToken)));
        }

        private async void OnOpenLevelRequested(LevelViewModel levelViewModel)
        {
            try
            {
                var index = _levels.IndexOf(levelViewModel);

                var args = await _gameArgsFactory.Create(Model, index, Application.exitCancellationToken);

                await _sceneLoaderService.LoadWithTransition(View.gameObject.scene, async () =>
                {
                    await _sceneLoaderService.LoadSceneAsync(SceneLoaderService.Game, args, LoadSceneMode.Additive);
                });
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        public void Dispose()
        {
            if (_levels != null)
            {
                foreach (var levelViewModel in _levels)
                {
                    levelViewModel.Dispose();
                    levelViewModel.OnLevelOpenRequested -= OnOpenLevelRequested;
                }
            }
            _levels = null;

            Model = null;
            View.Dispose();
        }
        
        public class Factory : PlaceholderFactory<LevelsCategoryModel, LevelsCategoryViewModel>
        {
            
        }
    }
}
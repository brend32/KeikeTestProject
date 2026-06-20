using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Configurations.JSON;
using Cysharp.Threading.Tasks;
using Models;
using Newtonsoft.Json;
using Services;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Utils;
using Views;
using Zenject;

namespace ViewModels
{
    public class LevelsBlockViewModel : IDisposable
    {
        public LevelsBlockView View { get; }

        private List<LevelsCategoryViewModel> _categories;
        private readonly LevelsService _levelsService;
        private readonly LevelsCategoryViewModel.Factory _categoryFactory;
        private readonly LevelsCategoryModel.Factory _categoryModelFactory;

        [Inject]
        public LevelsBlockViewModel(LevelsService levelsService, 
            LevelsBlockView view, 
            LevelsCategoryViewModel.Factory categoryFactory, 
            LevelsCategoryModel.Factory categoryModelFactory)
        {
            _levelsService = levelsService;
            View = view;
            _categoryFactory = categoryFactory;
            _categoryModelFactory = categoryModelFactory;
            
            view.Bind(this);
        }

        public async UniTask DownloadAndLoad(CancellationToken cancellationToken)
        {
            View.DisplayLoading();
            _categories = _levelsService.Categories
                .Select(asset => 
                    _categoryFactory.Create(_categoryModelFactory.Create(asset)))
                .ToList();
            
            View.DisplayHostedViews(_categories.Select(c => c.View));
            
            await UniTask.WhenAll(_categories.Select(c => c.DownloadAndLoad(cancellationToken)));
        }

        public void Dispose()
        {
            foreach (var category in _categories)
            {
                category.Dispose();
            }
            
            _categories.Clear();
            _categories = null;
            View.Dispose();
        }
    }
}
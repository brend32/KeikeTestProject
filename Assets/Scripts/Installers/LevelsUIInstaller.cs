using Models;
using UnityEngine;
using UnityEngine.AddressableAssets;
using ViewModels;
using Views;
using Zenject;

namespace Installers
{
    public class LevelsUIInstaller : MonoInstaller
    {
        public LevelsCategoryView LevelsCategoryPrefab;
        public LevelView LevelPrefab;

        public override void InstallBindings()
        {
            BindViewModels();
            BindViews();
        }

        private void BindViewModels()
        {
            Container.BindFactory<AssetReferenceT<TextAsset>, LevelViewModel, LevelViewModel.Factory>();
            Container.BindFactory<LevelsCategoryModel, LevelsCategoryViewModel, LevelsCategoryViewModel.Factory>();
            Container.Bind<LevelsBlockViewModel>().FromNew().AsSingle();
        }

        private void BindViews()
        {
            Container.BindMemoryPool<LevelView, LevelView.Pool>()
                .WithInitialSize(3)
                .FromComponentInNewPrefab(LevelPrefab)
                .UnderTransformGroup(nameof(LevelView));
            
            Container.BindMemoryPool<LevelsCategoryView, LevelsCategoryView.Pool>()
                .WithInitialSize(3)
                .FromComponentInNewPrefab(LevelsCategoryPrefab)
                .UnderTransformGroup(nameof(LevelsCategoryView));
        }
    }
}
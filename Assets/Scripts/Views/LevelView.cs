using System;
using UnityEngine;
using UnityEngine.UI;
using ViewModels;
using Zenject;

namespace Views
{
    public class LevelView : MonoBehaviour, IDisposable, IPoolable<LevelView.Pool>
    {
        public LevelViewModel ViewModel { get; private set; }

        [SerializeField] private LoadingView _loadingView;
        [SerializeField] private Image _image;

        private IMemoryPool _pool;

        public void Bind(LevelViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public void DisplayLoaded(Sprite sprite, Color color)
        {
            _loadingView.Hide();
            _image.sprite = sprite;
            _image.color = color;
        }
        
        public void DisplayLoading()
        {
            _loadingView.Show();
        }

        public void Action()
        {
            ViewModel.OpenLevel();
        }
        
        public void OnDespawned()
        {
            _pool = null;
            ViewModel = null;
        }

        public void OnSpawned(Pool pool)
        {
            _pool = pool;
        }

        public void Dispose()
        {
            _pool?.Despawn(this);
            ViewModel = null;
        }
        
        public class Pool : MonoMemoryPool<LevelView> 
        {
            
        }
    }
}
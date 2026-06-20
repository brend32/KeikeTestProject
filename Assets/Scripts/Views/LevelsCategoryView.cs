using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using ViewModels;
using Zenject;

namespace Views
{
    public class LevelsCategoryView : MonoBehaviour, IDisposable, IPoolable<LevelsCategoryView.Pool>
    {
        public LevelsCategoryViewModel ViewModel { get; private set; }

        [SerializeField] private Transform _content;
        [SerializeField] private LocalizeStringEvent _title;

        private readonly List<LevelView> _hostedViews = new();
        private IMemoryPool _pool;

        public void Bind(LevelsCategoryViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public void DisplayHostedViews(IEnumerable<LevelView> views, LocalizedString title)
        {
            _title.StringReference = title;
            
            ClearContent();
            _hostedViews.AddRange(views);
            
            foreach (var levelView in _hostedViews)
            {
                levelView.transform.SetParent(_content, false);
            }
        }

        public void ClearContent()
        {
            foreach (var levelView in _hostedViews)
            {
                levelView.transform.SetParent(null, false);
                levelView.Dispose();
            }
            
            _hostedViews.Clear();
        }

        public void Dispose()
        {
            ClearContent();
            _pool?.Despawn(this);
            ViewModel = null;
        }
        
        public void OnSpawned(Pool pool)
        {
            _pool = pool;
        }

        public void OnDespawned()
        {
            _pool = null;
            ViewModel = null;
        }
        
        public class Pool : MemoryPool<LevelsCategoryView>
        {
            
        }
    }
}
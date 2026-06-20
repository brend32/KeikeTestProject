using System;
using System.Collections.Generic;
using UnityEngine;
using ViewModels;
using Zenject;

namespace Views
{
    public class LevelsBlockView : MonoBehaviour, IDisposable
    {
        public LevelsBlockViewModel ViewModel { get; private set; }

        [SerializeField] private LoadingView _loadingView;
        [SerializeField] private Transform _content;

        private readonly List<LevelsCategoryView> _hostedViews = new();

        public void Bind(LevelsBlockViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public void DisplayLoading()
        {
            _loadingView.Show();
        }
        
        public void DisplayHostedViews(IEnumerable<LevelsCategoryView> views)
        {
            _loadingView.Hide();
            ClearContent();
            _hostedViews.AddRange(views);
            
            foreach (var view in _hostedViews)
            {
                view.transform.SetParent(_content, false);
            }
        }

        public void ClearContent()
        {
            foreach (var view in _hostedViews)
            {
                view.transform.SetParent(null, false);
                view.Dispose();
            }
            
            _hostedViews.Clear();
        }

        public void Dispose()
        {
            ClearContent();
            ViewModel = null;
        }
    }
}
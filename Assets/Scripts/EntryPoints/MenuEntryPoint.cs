using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game;
using UnityEngine;
using Utils;
using ViewModels;
using Zenject;

namespace EntryPoints
{
    public class MenuEntryPoint : MonoBehaviour, IDisposable
    {
        private LevelsBlockViewModel _levelsBlockViewModel;
        private readonly CancellationTokenSource _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(Application.exitCancellationToken);
        
        [Inject]
        public void Construct(LevelsBlockViewModel levelsBlockViewModel)
        {
            _levelsBlockViewModel = levelsBlockViewModel;
        }
        
        private void Start()
        {
            var token = _tokenSource.Token;
            
            _levelsBlockViewModel.DownloadAndLoad(token).Forget();
        }
        
        public void Dispose()
        {
            _levelsBlockViewModel.Dispose();
            _tokenSource.Cancel();
            _tokenSource.Dispose();
        }
    }
}
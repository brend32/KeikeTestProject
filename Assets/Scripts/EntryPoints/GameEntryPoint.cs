using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game;
using Models;
using Newtonsoft.Json;
using Services.SceneLoader;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using Zenject;

namespace EntryPoints
{
    public class GameEntryPoint : MonoBehaviour, IDisposable
    {
        public class Args : SceneArgs, IDisposable 
        {
            public int Level { get; set; }
            public LevelsCategoryModel Category { get; set; }
            public LevelModel LevelModel { get; set; }
            
            public Sprite ShapeImage => LevelModel.Shape.Shape.GetAsset();

            public void Dispose()
            {
                LevelModel.Shape.Shape.Release();
            }

            public class Factory
            {
                private readonly LevelModel.Factory _levelModelFactory;

                [Inject]
                public Factory(LevelModel.Factory levelModelFactory)
                {
                    _levelModelFactory = levelModelFactory;
                }

                public async UniTask<Args> Create(LevelsCategoryModel category, int level, CancellationToken cancellationToken)
                {
                    var levelReference = category.Asset.Levels[level];
                    var levelModel = await _levelModelFactory.Create(levelReference, cancellationToken);
                    await levelModel.Shape.Shape.LoadAssetAsync().WithCancellation(cancellationToken);

                    return new Args()
                    {
                        Level = level,
                        Category = category,
                        LevelModel = levelModel
                    };
                }

                public async UniTask<Args> CreateNextLevel(Args args, CancellationToken cancellationToken)
                {
                    var nextLevel = args.Level + 1;
                    if (nextLevel >= args.Category.Levels)
                    {
                        nextLevel = 0;
                    }

                    return await Create(args.Category, nextLevel, cancellationToken);
                }
            }
        }

        private Args _args;
        private DrawGameController _gameController;
        private GameStyleSetter _gameStyleSetter;
        private SceneLoaderService _sceneLoaderService;
        private GameStartSound _startSound;
        private Args.Factory _argsFactory;

        [Inject]
        public void Construct(
            DrawGameController gameController, 
            GameStyleSetter gameStyleSetter,
            SceneLoaderService sceneLoaderService, 
            GameStartSound startSound,
            Args.Factory argsFactory,
            [InjectOptional] SceneArgs args)
        {
            _gameController = gameController;
            _gameStyleSetter = gameStyleSetter;
            _sceneLoaderService = sceneLoaderService;
            _startSound = startSound;
            _argsFactory = argsFactory;
            _args = (Args)args;
        }
        
        private void Start()
        {
            if (_args != null)
            {
                var model = _args.LevelModel;
                
                _startSound.SetAudio(model.StartAudio);
                _gameController.LoadSplines(model.Shape.Paths.Select(m => m.Path).ToList());
                _gameStyleSetter.SetImage(_args.ShapeImage, model.Color);
                _gameStyleSetter.SetWidth(model.Shape.Width);
            }
            _gameController.StartGame().Forget();
            
            _gameController.OnFinishedGame.AddListener(async () =>
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: destroyCancellationToken);
                NextLevel();
            });
        }

        public void Dispose()
        {
            _args?.Dispose();
            _args = null;
        }

        [EasyButtons.Button]
        public void NextLevel()
        {
            var args = _args;
            
            _sceneLoaderService.LoadWithTransition(gameObject.scene, async () =>
            {
                if (args != null)
                {
                    args = await _argsFactory.CreateNextLevel(args, Application.exitCancellationToken);
                }

                await _sceneLoaderService.LoadSceneAsync(SceneLoaderService.Game, args, LoadSceneMode.Additive);
            });
        }

        [EasyButtons.Button]
        public void GoToMenu()
        {
            _sceneLoaderService.LoadWithTransition(gameObject.scene, async () =>
            {
                await _sceneLoaderService.LoadSceneAsync(SceneLoaderService.Menu, LoadSceneMode.Additive);
            }).ToUniTask().Forget();
        }
    }
}
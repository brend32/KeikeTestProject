using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Services.SceneLoader
{
    public class SceneLoaderService
    {
        public const string Menu = "Menu";
        public const string Game = "Game";
        public const string Loading = "Loading";
        
        private readonly ZenjectSceneLoader _sceneLoader;

        [Inject]
        public SceneLoaderService(ZenjectSceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }
        
        public AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, Action<DiContainer> extraBindings = null)
        {
            return LoadSceneAsync(sceneName, null, mode, extraBindings);
        }

        public AsyncOperation LoadSceneAsync(string sceneName, SceneArgs args, LoadSceneMode mode = LoadSceneMode.Single, Action<DiContainer> extraBindings = null)
        {
            return _sceneLoader.LoadSceneAsync(sceneName, mode, di =>
            {
                di.Bind<SceneArgs>().FromInstance(args);
                extraBindings?.Invoke(di);
            });
        }
    }
}
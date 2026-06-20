using Cysharp.Threading.Tasks;
using Services;
using Services.SceneLoader;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Zenject;

namespace EntryPoints
{
    public class BootEntryPoint : MonoBehaviour
    {
        public string SceneToLoad = SceneLoaderService.Menu;
        
        [Inject] private AudiosService _audiosService;
        [Inject] private LevelsService _levelsService;
        [Inject] private ShapesService _shapesService;
        [Inject] private ZenjectSceneLoader _sceneLoader;

        private void Start()
        {
            LoadAndStartMenu().Forget();
        }

        public async UniTask LoadAndStartMenu()
        {
            var token = Application.exitCancellationToken;
            
            await Addressables.InitializeAsync(true).WithCancellation(token);

            await UniTask.WhenAll(
                _audiosService.Download(token),
                _levelsService.Download(token),
                _shapesService.Download(token)
            );

            await UniTask.WhenAll(
                _audiosService.Load(token),
                _levelsService.Load(token),
                _shapesService.Load(token)
            );
            
            await _sceneLoader.LoadSceneAsync(SceneToLoad).WithCancellation(token);
        }
    }
}
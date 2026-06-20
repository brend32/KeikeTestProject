using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using Services.SceneLoader;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using Zenject;

namespace EntryPoints
{
    public class LoadingEntryPoint : MonoBehaviour
    {
        public class Args : SceneArgs
        {
            public Func<UniTask> OnLoadingFullyVisible { get; }
            public Func<UniTask> OnLoadDestinationScene { get; }

            public Args(Func<UniTask> onLoadingFullyVisible, Func<UniTask> onLoadDestinationScene)
            {
                OnLoadingFullyVisible = onLoadingFullyVisible;
                OnLoadDestinationScene = onLoadDestinationScene;
            }
        }

        [SerializeField] private float _duration = 1f;
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private GameObject _shared;
        private Args _args;
        
        [Inject]
        public void Construct(SceneArgs args)
        {
            _args = (Args)args;
        }

        private IEnumerator Start()
        {
            _shared.SetActive(false);
            yield return Fade(0, 1);
            yield return _args.OnLoadingFullyVisible();
            _shared.SetActive(true);
            yield return _args.OnLoadDestinationScene();
            _shared.SetActive(false);
            yield return Fade(1, 0);
            yield return SceneManager.UnloadSceneAsync(gameObject.scene);
        }

        private IEnumerator Fade(float a, float b)
        {
            yield return AnimationUtils.Animate(_duration, t =>
            {
                _group.alpha = Mathf.LerpUnclamped(a, b, t);
            });
        }
    }

    public static class LoadingEntryPointExtensions
    {
        public static AsyncOperation LoadWithTransition(this SceneLoaderService service, Func<UniTask> onFullyVisible, Func<UniTask> onLoadDestinationScene)
        {
            return service.LoadSceneAsync(SceneLoaderService.Loading, new LoadingEntryPoint.Args(onFullyVisible, onLoadDestinationScene), LoadSceneMode.Additive);
        }

        public static AsyncOperation LoadWithTransition(this SceneLoaderService service, Scene toUnload, Func<UniTask> onLoadDestinationScene)
        {
            return service.LoadWithTransition(() => SceneManager.UnloadSceneAsync(toUnload).ToUniTask(), onLoadDestinationScene);
        }
    }
}
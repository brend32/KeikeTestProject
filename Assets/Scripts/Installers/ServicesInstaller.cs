using Services;
using Services.SceneLoader;
using Zenject;

namespace Installers
{
    public class ServicesInstaller : MonoInstaller
    {
        public AudiosService.Settings Audios;
        public LevelsService.Settings Levels;
        public ShapesService.Settings Shapes;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<AudiosService>().AsSingle().WithArguments(Audios);
            Container.BindInterfacesAndSelfTo<LevelsService>().AsSingle().WithArguments(Levels);
            Container.BindInterfacesAndSelfTo<ShapesService>().AsSingle().WithArguments(Shapes);
            Container.Bind<SceneLoaderService>().AsSingle();
        }
    }
}
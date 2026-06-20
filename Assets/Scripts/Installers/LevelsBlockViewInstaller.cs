using Views;
using Zenject;

namespace Installers
{
    public class LevelsBlockViewInstaller : MonoInstaller
    {
        public LevelsBlockView View;

        public override void InstallBindings()
        {
            Container.Bind<LevelsBlockView>().FromInstance(View).AsSingle();
        }
    }
}
using Game;
using Zenject;

namespace Installers
{
    public class DrawingGameInstaller : MonoInstaller
    {
        public DrawDraggingZone DraggingZone;
        public DrawGameController GameController;
        public GameStartSound StartSound;
        public GameEndSound EndSound;
        
        public override void InstallBindings()
        {
            Container.Bind<DrawDraggingZone>().FromInstance(DraggingZone).AsSingle();
            Container.Bind<DrawGameController>().FromInstance(GameController).AsSingle();
            Container.Bind<GameStartSound>().FromInstance(StartSound).AsSingle();
            Container.Bind<GameEndSound>().FromInstance(EndSound).AsSingle();
        }
    }
}
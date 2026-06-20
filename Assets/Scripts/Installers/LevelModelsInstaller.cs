using Configurations;
using Configurations.JSON;
using EntryPoints;
using Models;
using Zenject;

namespace Installers
{
    public class LevelModelsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindGameArgs();
            BindModels();
        }
        
        private void BindGameArgs()
        {
            Container.Bind<GameEntryPoint.Args.Factory>().AsSingle();
        }

        private void BindModels()
        {
            Container.Bind<LevelModel.Factory>().AsSingle();
            Container.Bind<IFactory<LevelJson, LevelModel>>().To<LevelModel.Factory>().FromResolve();
            
            Container.BindFactory<LevelsCategoryCatalogue, LevelsCategoryModel, LevelsCategoryModel.Factory>();
            
            Container.Bind<DrawingShapeModel.Factory>().AsSingle();
            Container.Bind<IFactory<DrawingShapeJson, DrawingShapeModel>>().To<DrawingShapeModel.Factory>().FromResolve();
            
            Container.Bind<DrawingPathModel.Factory>().AsSingle();
            Container.Bind<IFactory<DrawingPathJson, DrawingPathModel>>().To<DrawingPathModel.Factory>().FromResolve();
        }
    }
}
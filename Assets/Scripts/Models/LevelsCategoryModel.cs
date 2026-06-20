using Configurations;
using UnityEngine.Localization;
using Zenject;

namespace Models
{
    public class LevelsCategoryModel
    {
        public LocalizedString Title => Asset.Title;
        public int Levels => Asset.Levels.Count;
        
        public LevelsCategoryCatalogue Asset { get; }

        [Inject]
        public LevelsCategoryModel(LevelsCategoryCatalogue asset)
        {
            Asset = asset;
        }
        
        public class Factory : PlaceholderFactory<LevelsCategoryCatalogue, LevelsCategoryModel>
        {
            
        }
    }
}
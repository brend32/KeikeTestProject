using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;

namespace Configurations
{
    [CreateAssetMenu(menuName = "Configuration/Levels Category Catalogue")]
    public class LevelsCategoryCatalogue : ScriptableObject
    {
        public LocalizedString Title => _title;
        public IReadOnlyList<AssetReferenceT<TextAsset>> Levels => _levels;
        
        [SerializeField] private LocalizedString _title;
        [SerializeField] private AssetReferenceT<TextAsset>[] _levels;
    }
}
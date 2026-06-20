using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Configurations
{
    [CreateAssetMenu(menuName = "Configuration/Shapes Catalogue")]
    public class ShapesCatalogue : ScriptableObject
    {
        public IReadOnlyDictionary<string, AssetReferenceSprite> Shapes => _map ??= _shapes.ToDictionary(t => t.Name, t => t.Shape);

        private Dictionary<string, AssetReferenceSprite> _map;
        
        [SerializeField] private Entry[] _shapes;

        public void ClearMap()
        {
            _map = null;
        }
        
        [Serializable]
        private class Entry : ISerializationCallbackReceiver
        {
            public string Name;
            public AssetReferenceSprite Shape;
            
            public void OnBeforeSerialize()
            {
#if UNITY_EDITOR
                if (string.IsNullOrEmpty(Name) && Shape != null && Shape.editorAsset != null)
                {
                    Name = Shape.editorAsset.name;
                }
#endif
            }

            public void OnAfterDeserialize()
            {
                
            }
        }
    }
}
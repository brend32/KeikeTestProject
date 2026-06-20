using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using Utils;

namespace Configurations
{
    [CreateAssetMenu(menuName = "Configuration/Audios Catalogue")]
    public class AudiosCatalogue : ScriptableObject
    {
        public IReadOnlyDictionary<string, LocalizedAudioClip> Clips => _map ??= _clips.ToDictionary(t => t.Name, t => t.Clip);

        private Dictionary<string, LocalizedAudioClip> _map;
        
        [SerializeField] private Entry[] _clips;
        
        [Serializable]
        private class Entry
        {
            public string Name;
            public LocalizedAudioClip Clip;
        }

        public LocalizedAudioClip GetClipOrDefault(string key)
        {
            if (Clips.TryGetValue(key, out var clip))
                return clip;
            
            Debug.LogError($"Clip with key {key} is not present in the catalogue. Empty clip is returned instead");
            return new LocalizedAudioClip();
        }
        
        public LocalizedAudioClip GetRandomClip()
        {
            return _clips.RandomEntry().Clip;
        }
    }
}
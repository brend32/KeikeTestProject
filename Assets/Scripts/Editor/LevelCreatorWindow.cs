using System;
using System.IO;
using System.Linq;
using Configurations;
using Configurations.JSON;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Utils;

namespace Editor
{
    public class LevelCreatorWindow : EditorWindow
    {
        private TextAsset _asset;
        private AudiosCatalogue _audiosCatalogue;
        private string _audioStarterId;
        private TextAsset _shapeAsset;
        private string _shapeJson;
        private Color _color;

        private bool _pretify;
        
        [MenuItem("Window/Level Creator")]
        public static void ShowWindow()
        {
            GetWindow<LevelCreatorWindow>();
        }

        public void OnGUI()
        {
            _asset = EditorGUILayout.ObjectField("Level Asset", _asset, typeof(TextAsset), false) as TextAsset;
            EditorGUILayout.Space();
            
            _audiosCatalogue = (AudiosCatalogue) EditorGUILayout.ObjectField("Audios Catalogue", _audiosCatalogue, typeof(AudiosCatalogue), false);
            
            EditorGUILayout.Space();

            if (_audiosCatalogue)
            {
                var keys = _audiosCatalogue.Clips.Keys.ToArray();
                var index = Array.IndexOf(keys, _audioStarterId);
                if (index == -1)
                    index = 0;

                index = EditorGUILayout.Popup(index, keys);
                _audioStarterId = keys[index];
            }
            else
            {
                _audioStarterId = EditorGUILayout.TextField("Audio Starter Id", _audioStarterId);
            }
            
            _color = EditorGUILayout.ColorField("Color", _color);
            _shapeAsset = EditorGUILayout.ObjectField("Shape Asset", _shapeAsset, typeof(TextAsset), false) as TextAsset;
            if (_shapeAsset == null)
            {
                _shapeJson = EditorGUILayout.TextArea(_shapeJson, GUILayout.Height(200));
            }
            
            EditorGUILayout.Space();
            _pretify = EditorGUILayout.Toggle("Pretify", _pretify);

            if (_asset && GUILayout.Button("Load"))
            {
                Load();
            }

            if (_asset && GUILayout.Button("Save"))
            {
                SaveAt(AssetDatabase.GetAssetPath(_asset));
            }

            if (GUILayout.Button("Save As"))
            {
                var path = EditorUtility.SaveFilePanel("Select file", "", "Level.json", "json");
                if (string.IsNullOrEmpty(path))
                    return;
                
                SaveAt(path);
            }
        }

        private void Load()
        {
            UnityTypeSerializationSupport.Use();

            var json = JsonConvert.DeserializeObject<LevelJson>(_asset.text);
            
            _color = json.Color;
            _shapeAsset = null;
            _shapeJson = JsonConvert.SerializeObject(json.Shape, Formatting.Indented);
            _audioStarterId = json.StartAudioId;
        }

        private void SaveAt(string path)
        {
            path = Path.GetFullPath(path);
            path = FileUtil.GetProjectRelativePath(path);
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError($"Failed to get relative path {path}");
                return;
            }
            
            UnityTypeSerializationSupport.Use();

            var dto = new LevelJson()
            {
                Color = _color,
                Shape = JsonConvert.DeserializeObject<DrawingShapeJson>(_shapeAsset == null ? _shapeJson : _shapeAsset.text)!,
                StartAudioId = _audioStarterId
            };
            
            var json = JsonConvert.SerializeObject(dto, _pretify ? Formatting.Indented : Formatting.None);
            
            Debug.Log($"JSON: {json}");
            
            File.WriteAllText(path, json);
            AssetDatabase.ImportAsset(path);
        }
    }
}
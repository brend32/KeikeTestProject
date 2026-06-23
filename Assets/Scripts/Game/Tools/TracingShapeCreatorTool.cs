using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Configurations;
using Configurations.JSON;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Splines;
using Utils;

namespace Game.Tools
{
#if UNITY_EDITOR
    using UnityEditor;
#endif
    
    public class TracingShapeCreatorTool : MonoBehaviour
    {
#if UNITY_EDITOR
        public TextAsset Asset;
        
        [Space]
        public ShapesCatalogue ShapesCatalogue;
        public Sprite Shape;
        public string ShapeId;
        public float Width;
        public Color Color;

        [Space] 
        public int SplineIndex;
        [Range(0, 1)] public float Progress;
        public bool Preview;
        public bool Pretify;
        
        [Space]
        public DrawGameController GameController;
        public GameStyleSetter ShapeSetter;
        public HelpPath HelpPath;

        [EasyButtons.Button]
        public void Load()
        {
            if (Asset == null)
            {
                Debug.LogError($"Populate {nameof(Asset)} first!");
                return;
            }

            try
            {
                UnityTypeSerializationSupport.Use();
                var json = JsonConvert.DeserializeObject<DrawingShapeJson>(Asset.text);
                
                Load(json);
            }
            catch (JsonException e)
            {
                Debug.LogError("Failed to deserialize asset");
                Debug.LogException(e);
            }
        }

        public void Load(DrawingShapeJson json)
        {
            Undo.RecordObject(this, "Load shape");
            Undo.RecordObject(GameController.Container, "Load shape");
            GameController.LoadSplines(json.Paths.Select(p => new Spline(p.Points, p.Closed)).ToList());
            Shape = null;
            ShapeId = json.ShapeAssetId;
            Width = json.Width;
            EditorUtility.SetDirty(this);
            EditorApplication.QueuePlayerLoopUpdate();
            SceneView.RepaintAll();
            HelpPath._ClearPreview();
            OnValidate(); 
        }

        [EasyButtons.Button]
        public void Save()
        {
            if (Asset == null)
            {
                Debug.LogError($"Populate {nameof(Asset)} first!");
                return;
            }
            
            SaveAt(AssetDatabase.GetAssetPath(Asset));
        }

        [EasyButtons.Button]
        public void SaveAs()
        {
            var path = EditorUtility.SaveFilePanel("Select file", "", "Shape.json", "json");
            if (string.IsNullOrEmpty(path))
                return;
            
            SaveAt(path);
        }

        [EasyButtons.Button]
        public void ResetImageId()
        {
            ShapeId = FindShapeId();
            OnValidate();
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
            
            var dto = new DrawingShapeJson()
            {
                Width = Width,
                ShapeAssetId = ShapeId,
                Paths = GameController.Container.Splines.Select(spline => new DrawingPathJson()
                {
                    Points = spline.Knots.ToList(),
                    Closed = spline.Closed
                }).ToList()
            };
            
            UnityTypeSerializationSupport.Use();
            var json = JsonConvert.SerializeObject(dto, Pretify ? Formatting.Indented : Formatting.None);
            Debug.Log($"JSON: {json}");
            
            File.WriteAllText(path, json);
            AssetDatabase.ImportAsset(path);
            
            Debug.Log("Saved");
        }

        private string FindShapeId()
        {
            if (Shape == null || ShapesCatalogue == null)
                return null;

            ShapesCatalogue.ClearMap();
            return ShapesCatalogue.Shapes
                .Where(m => m.Value.editorAsset != null)
                .FirstOrDefault(m => m.Value.editorAsset == Shape || m.Value.editorAsset == Shape.texture).Key;
        }

        private void SyncShapeAndId()
        {
            if (ShapesCatalogue)
            {
                if (Shape == null && string.IsNullOrWhiteSpace(ShapeId) == false)
                {
                    var asset = ShapesCatalogue.Shapes.GetValueOrDefault(ShapeId);
                    if (asset == null)
                         return;
                    
                    if (asset.editorAsset is Texture2D)
                    {
                        Shape = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(asset.editorAsset)).OfType<Sprite>().FirstOrDefault(s => s.name == asset.SubObjectName);
                    }
                    else if (asset.editorAsset is Sprite sprite)
                    {
                        Shape = sprite;
                    }
                }
                else if (string.IsNullOrWhiteSpace(ShapeId) && Shape)
                {
                    ShapeId = FindShapeId();
                }
            }
        }

        public void OnValidate()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
                return;
            
            if (GameController == null || ShapeSetter == null || HelpPath == null)
                return;
            
            HelpPath.__preview = Preview;
            HelpPath.OnValidate();

            SyncShapeAndId(); 

            if (Preview == false)
                return;
            
            Invoke(nameof(SyncLogic), 0.1f);
        }

        private void SyncLogic()
        {
            HelpPath._SyncPreviewInstant();
            
            ShapeSetter.SetImage(Shape, Color);
            ShapeSetter.SetWidth(Width);
            GameController.SetIndex(SplineIndex);
            GameController.SetPathProgress(Progress);
        }
#endif
    }
}
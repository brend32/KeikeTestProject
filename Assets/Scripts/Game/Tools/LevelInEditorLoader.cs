using Configurations.JSON;
using Newtonsoft.Json;
using UnityEngine;
using Utils;

namespace Game.Tools
{
    public class LevelInEditorLoader : MonoBehaviour
    {
#if UNITY_EDITOR
        public TextAsset Asset;
        
        [Space]
        public TracingShapeCreatorTool Tool;

        [EasyButtons.Button]
        public void Load()
        {
            UnityTypeSerializationSupport.Use();
            
            var json = JsonConvert.DeserializeObject<LevelJson>(Asset.text);

            Tool.Color = json.Color;
            
            Tool.Preview = true;
           
            Tool.Load(json.Shape);
        }
#endif
    }
}
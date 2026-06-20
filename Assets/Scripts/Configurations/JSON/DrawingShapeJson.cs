using System.Collections.Generic;
using Newtonsoft.Json;

namespace Configurations.JSON
{
    public class DrawingShapeJson
    {
        [JsonProperty(PropertyName = "paths")]
        public List<DrawingPathJson> Paths { get; set; }
        [JsonProperty(PropertyName = "shapeAssetId")]
        public string ShapeAssetId { get; set; }
        [JsonProperty(PropertyName = "width")]
        public float Width { get; set; }
    }
}
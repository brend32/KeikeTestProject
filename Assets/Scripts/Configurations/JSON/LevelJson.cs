using System.Collections.Generic;
using Newtonsoft.Json;

namespace Configurations.JSON
{
    public class LevelJson
    {
        [JsonProperty(PropertyName = "shape")]
        public DrawingShapeJson Shape { get; set; }
        [JsonProperty(PropertyName = "color")]
        public HexColor Color { get; set; }
        [JsonProperty(PropertyName = "startAudioId")]
        public string StartAudioId { get; set; }
    }
}
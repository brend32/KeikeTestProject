using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Mathematics;
using UnityEngine.Splines;

namespace Configurations.JSON
{
    public class DrawingPathJson
    {
        [JsonProperty(PropertyName = "points")]
        public List<BezierKnot> Points { get; set; }
        [JsonProperty(PropertyName = "closed")]
        public bool Closed { get; set; }
    }
}
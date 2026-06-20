using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Configurations.JSON
{
    [JsonConverter(typeof(Converter))]
    public struct HexColor
    {
        public float R;
        public float G;
        public float B;
        public float A;

        public HexColor(Color color)
        {
            R = color.r;
            G = color.g;
            B = color.b;
            A = color.a;
        }
        
        public static implicit operator HexColor(Color color) => new(color);
        public static implicit operator Color(HexColor hexColor) => new(hexColor.R, hexColor.G, hexColor.B, hexColor.A);

        public static HexColor FromHex(string hex)
        {
            if (string.IsNullOrEmpty(hex))
                return default;
            
            if (hex.StartsWith("#") == false)
            {
                hex = $"#{hex}";
            }
            
            if (ColorUtility.TryParseHtmlString(hex, out var color))
            {
                return color;
            }
            
            return default;
        }

        public override string ToString()
        {
            if (Mathf.Approximately(A, 1))
            {
                return ColorUtility.ToHtmlStringRGB(new Color(R, G, B));
            }
            
            return ColorUtility.ToHtmlStringRGBA(new Color(R, G, B, A));
        }
        
        public class Converter : JsonConverter<HexColor>
        {
            public override void WriteJson(JsonWriter writer, HexColor value, JsonSerializer serializer)
            {
                writer.WriteValue(value.ToString());
            }

            public override HexColor ReadJson(JsonReader reader, Type objectType, HexColor existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var str = reader.Value as string;
                return FromHex(str);
            }
        }
    }
}
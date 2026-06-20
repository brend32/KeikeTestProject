using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace Utils
{
    public static class UnityTypeSerializationSupport
    {
        [RuntimeInitializeOnLoadMethod]
#if UNITY_EDITOR 
        [UnityEditor.InitializeOnLoadMethod]
#endif
        public static void Use()
        {
            JsonConvert.DefaultSettings = GetJsonSerializerSettings;
        }

        public static JsonSerializerSettings GetJsonSerializerSettings()
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new UnityTypeConverter());
            return settings;
        }

        private class UnityTypeConverter : JsonConverter
        {
            private static readonly HashSet<Type> UnityEngineTypes;

            static UnityTypeConverter()
            {
                UnityEngineTypes = new HashSet<Type>(typeof(UnityEngine.Object).Assembly.GetTypes().Concat(typeof(float2).Assembly.GetTypes()))
                {
                    typeof(Color?)
                };
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                try
                {
                    writer.WriteRawValue(JsonUtility.ToJson(value));
                }
                catch (Exception)
                {
                    writer.WriteRawValue($"\"Unsupported {value?.GetType().ToString() ?? "null"}\"");
                }
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (typeof(ScriptableObject).IsAssignableFrom(objectType))
                {
                    JsonUtility.FromJsonOverwrite(JObject.Load(reader).ToString(), existingValue);
                    return existingValue;
                }

                if (typeof(Color?) == objectType)
                {
                    var read = JObject.Load(reader).ToString();
                    return JsonUtility.FromJson(read, typeof(Color));
                }

                return JsonUtility.FromJson(JObject.Load(reader).ToString(), objectType);
            }

            public override bool CanConvert(Type objectType)
            {
                return IsUnityEngineType(objectType);
            }

            private static bool IsUnityEngineType(Type objectType)
            {
                return UnityEngineTypes.Contains(objectType);
            }
        }
    }
}
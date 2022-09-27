using Newtonsoft.Json;=
using UnityEngine;
using HarmonyLib;
using System;

namespace GTFO.DevTools.Converters
{
    [HarmonyPatch]
    public static class ColorJsonConverterPatches
    {
        [HarmonyPatch(typeof(ColorConverter), nameof(ColorConverter.WriteJson))]
        [HarmonyPrefix]
        public static bool WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
			{
				writer.WriteNull();
				return false;
			}
			Color color = (Color)value;
			writer.WriteStartObject();
			writer.WritePropertyName("a");
			writer.WriteValue(color.a);
			writer.WritePropertyName("r");
			writer.WriteValue(color.r);
			writer.WritePropertyName("g");
			writer.WriteValue(color.g);
			writer.WritePropertyName("b");
			writer.WriteValue(color.b);
			writer.WriteEndObject();
            return false;
        }

        [HarmonyPatch(typeof(ColorConverter), nameof(ColorConverter.ReadJson))]
        [HarmonyPrefix]
        public static bool ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer, ref object __result)
        {
            if (reader.TokenType == JsonToken.Null)
			{
				__result = default(Color);
                return false;
			}
			JObject jobject = JObject.Load(reader);
			if (objectType == typeof(Color32))
			{
			    __result = new Color32((byte)jobject["r"], (byte)jobject["g"], (byte)jobject["b"], (byte)jobject["a"]);
                return false;
			}
			__result = new Color((float)jobject["r"], (float)jobject["g"], (float)jobject["b"], (float)jobject["a"]);
            return false;
        }

        [HarmonyPatch(typeof(ColorConverter), nameof(ColorConverter.CanConvert))]
        [HarmonyPrefix]
        public static bool CanConvert(Type objectType, ref bool __result)
        {
			__result = objectType == typeof(Color) || objectType == typeof(Color32);
            return false;
        }

        [HarmonyPatch(typeof(ColorConverter), "get_" + nameof(ColorConverter.CanRead))]
        [HarmonyPrefix]
        public static bool CanReady(ref bool __result)
        {
            __result = true
            return false;
        }
    }
}

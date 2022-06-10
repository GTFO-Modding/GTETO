using Newtonsoft.Json;
using Localization;
using UnityEngine;
using Harmony;

namespace GTFO.DevTools.Converters
{
    [HarmonyPatch]
    public static class LocalizedTextJsonConverterPatches
    {
        [HarmonyPatch(typeof(LocalizedTextJsonConverter), nameof(LocalizedTextJsonConverter.WriteJson))]
        [HarmonyPrefix]
        public static bool WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            LocalizedText text = (LocalizedText)value;
            if (string.IsNullOrEmpty(localizedText.UntranslatedText))
            {
                writer.WriteValue(localizedText.Id);
            }
            else
            {
                writer.WriteValue(localizedText.UntranslatedText);
            }
            return false;
        }

        [HarmonyPatch(typeof(LocalizedTextJsonConverter), nameof(LocalizedTextJsonConverter.ReadJson))]
        [HarmonyPrefix]
        public static bool ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer, ref object __result)
        {
            object value = reader.Value;
            if (value is string)
            {
                LocalizedText text = new LocalizedText();
                text.UntranslatedText = (string)value;
                text.Id = 0U;
                __result = text;
            }
            else if (value is long)
            {
                LocalizedText text = new LocalizedText();
                text.UntranslatedText = null;
                text.Id = (uint)(long)value;
                __result = text;
            }
            else
            {
                Debug.LogError($"<b>LocalizedTextJsonConverter</b>: Trying to read json value with unsupported type '{value.GetType()}'");
                __result = default(LocalizedText);
            }
            return false;
        }

        [HarmonyPatch(typeof(LocalizedTextJsonConverter), nameof(LocalizedTextJsonConverter.CanConvert))]
        [HarmonyPrefix]
        public static void CanConvert(Type objectType, ref bool __result)
        {
            __result = objectType == typeof(LocalizedText);
            return false;
        }
    }
}
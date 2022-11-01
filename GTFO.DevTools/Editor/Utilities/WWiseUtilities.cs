using GTFO.DevTools.WWise;
using Newtonsoft.Json;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GTFO.DevTools.Utilities
{
    [InitializeOnLoad]
    public static class WWiseUtilities
    {
        private static readonly WWiseInfoJSON s_infoJSON;

        public static WWiseEventInfo[] GetEvents() => s_infoJSON.Events.ToArray();

        static WWiseUtilities()
        {
            s_infoJSON = JsonConvert.DeserializeObject<WWiseInfoJSON>(File.ReadAllText(Path.Combine(Application.dataPath, "GTFO.DevTools", "Data", "wwise.json")));
        }
    }
}

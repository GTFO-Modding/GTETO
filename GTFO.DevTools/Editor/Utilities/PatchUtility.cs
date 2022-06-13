using HarmonyLib;
using UnityEditor;

namespace GTFO.DevTools.Utilities
{
    [InitializeOnLoad]
    public static class PatchUtility
    {
        static PatchUtility()
        {
            Harmony unpatcher = new Harmony("dev.flaff.GTFO.DevTools.Unpatcher");
            unpatcher.UnpatchAll("dev.flaff.GTFO.DevTools.Patcher");

            Harmony patcher = new Harmony("dev.flaff.GTFO.DevTools.Patcher");
            patcher.PatchAll();
        }
    }
}

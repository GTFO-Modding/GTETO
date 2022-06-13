using HarmonyLib;

namespace GTFO.DevTools.Patches
{
    [HarmonyPatch]
    public static class SoundEventAttributePatches
    {
        [HarmonyPatch(typeof(SoundEventAttribute), MethodType.Constructor)]
        [HarmonyPostfix]
        public static void InitializeSoundEvent(SoundEventAttribute __instance, string filter)
        {
            __instance.Filter = filter;
        }
    }
}

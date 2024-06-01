using HarmonyLib;

namespace MTFO.Patches
{
    [HarmonyPatch(typeof(DiscordManager), "UpdateDiscordDetails")]
    static class Patch_DiscordManager
    {
        [HarmonyPrefix]
        private static bool Pre_UpdateDiscordDetails()
        {
            return false;
        }
    }
}

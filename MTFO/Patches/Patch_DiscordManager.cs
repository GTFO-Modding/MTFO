using MTFO.Utilities;
using HarmonyLib;

namespace MTFO.Patches
{
    [HarmonyPatch(typeof(DiscordManager), nameof(DiscordManager.UpdateDiscordDetails))]
    static class Patch_DiscordManager
    {
        [HarmonyPrefix]
        private static bool Pre_UpdateDiscordDetails(eDiscordDetailsDisplay details)
        {
            if (details == null) return false;

            String detailName = details.ToString();
            if (detailName.Equals("OnSuccess") || detailName.Equals("OnFail"))
            {
                Log.Debug($"DiscordManager.UpdateDiscordDetails {detailName} patched out");
                return false;
            }
            return true;
        }
    }
}

using HarmonyLib;

namespace MTFO.Patches
{
    [HarmonyPatch(typeof(DiscordManager), nameof(DiscordManager.UpdateDiscordDetails))]
    static class Patch_DiscordManager
    {
        [HarmonyPostfix]
        private static void Post_UpdateDiscordDetails(eDiscordDetailsDisplay details)
        {
            // this empty postfix fixes the problem with "return all players to lobby" button.
        }
    }
}

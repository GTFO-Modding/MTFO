using HarmonyLib;
using ChainedPuzzles;
using MTFO.CustomCP;

namespace MTFO.CustomCP.Patches
{
    [HarmonyPatch(typeof(ChainedPuzzleManager), nameof(ChainedPuzzleManager.OnAssetsLoaded))]
    static class Patch_PuzzleManager
    {
        internal static void Postfix(ChainedPuzzleManager __instance)
        {
            CustomPuzzleManager.Initialize(__instance);
        }
    }

    [HarmonyPatch(typeof(CP_Bioscan_Core), nameof(CP_Bioscan_Core.Update))]
    static class FixNullRefSpam
    {
        internal static bool Prefix(CP_Bioscan_Core __instance)
        {
            if (__instance.gameObject.transform.position.x > 9000 && __instance.gameObject.transform.position.y > 9000 && __instance.gameObject.transform.position.z > 9000)
            {
                return false;
            }
            return true;
        }
    }
}

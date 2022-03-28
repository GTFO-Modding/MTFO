using HarmonyLib;
using MTFO.Managers;
using ChainedPuzzles;

namespace MTFO.Patches
{
    [HarmonyPatch(typeof(ChainedPuzzleManager), nameof(ChainedPuzzleManager.OnAssetsLoaded))]
    class Patch_PuzzleManager
    {
        public static void Postfix(ChainedPuzzleManager __instance)
        {
            CustomPuzzleManager.Initialize(__instance);
        }

        
    }

    [HarmonyPatch(typeof(CP_Bioscan_Core), nameof(CP_Bioscan_Core.Update))]
    class FixNullRefSpam
    {
        static bool Prefix(CP_Bioscan_Core __instance)
        {
            if (__instance.gameObject.transform.position.x > 9000 && __instance.gameObject.transform.position.y > 9000 && __instance.gameObject.transform.position.z > 9000)
            {
                return false;
            }
            return true;
        }
    }
}

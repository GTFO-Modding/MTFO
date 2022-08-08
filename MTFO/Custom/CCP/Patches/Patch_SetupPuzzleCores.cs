using ChainedPuzzles;
using HarmonyLib;
using MTFO.Custom;
using MTFO.Custom.CCP.Components;
using MTFO.Managers;

namespace MTFO.CustomCP.Patches
{
    [HarmonyPatch]
    static class Patch_SetupPuzzleCores
    {
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(typeof(CP_Bioscan_Core), nameof(CP_Bioscan_Core.Setup))]
        public static void SetupSpline(CP_Bioscan_Core __instance)
        {
            var coreData = __instance.GetComponent<CorePuzzleData>();
            if (coreData == null)
                return;

            if (!CustomPuzzleManager.TryGetScanByID(coreData.PersistentID.Value, out var scan))
                return;

            var spline = __instance.m_spline.TryCast<CP_Holopath_Spline>();
            if (spline != null)
            {
                scan.ApplySplineRevealSpeed(spline);
            }
        }

        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(typeof(CP_Cluster_Core), nameof(CP_Cluster_Core.Setup))]
        public static void SetupSpline(CP_Cluster_Core __instance)
        {
            var clusterData = __instance.GetComponent<ClusterPuzzleData>();
            if (clusterData == null)
                return;

            if (!CustomPuzzleManager.TryGetClusterByID(clusterData.PersistentID.Value, out var scan))
                return;

            var spline = __instance.m_spline.TryCast<CP_Holopath_Spline>();
            if (spline != null)
            {
                scan.ApplySplineRevealSpeed(spline);
            }
        }
    }
}

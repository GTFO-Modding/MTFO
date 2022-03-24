using ChainedPuzzles;
using HarmonyLib;
using MTFO.Custom;
using MTFO.Managers;

namespace MTFO.Patches
{
    [HarmonyPatch]
    static class Patch_SetupPuzzleCores
    {
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(typeof(CP_Bioscan_Core), nameof(CP_Bioscan_Core.Setup))]
        public static void SetupSpline(CP_Bioscan_Core __instance, int puzzleIndex, iChainedPuzzleOwner owner)
        {
            // right now owner can be either ChainedPuzzleInstance, or CP_Cluster_Core.
            // we will only use ChainedPuzzleInstance, as the patch for CP_Cluster_Core will go in
            // and setup the splines of it's children.

            if (owner == null)
                return;

            ChainedPuzzleInstance instanceOwner = owner.TryCast<ChainedPuzzleInstance>();
            if (instanceOwner == null)
                return;

            var puzzleType = instanceOwner.Data.ChainedPuzzle[puzzleIndex];
            if (!CustomPuzzleManager.TryGetScanByID(puzzleType.PuzzleType, out var scan) ||
                (scan.RevealTime <= 0f && scan.RevealTimeDistanceMode == RevealDistanceMode.Distance))
                return;

            var spline = __instance.m_spline.TryCast<CP_Holopath_Spline>();
            if (spline == null)
                return;

            if (scan.RevealTime > 0f)
                spline.m_revealSpeed = scan.SplineRevealSpeed;
            if (scan.RevealTimeDistanceMode == RevealDistanceMode.Time)
                spline.m_splineLength = 1f;
        }

        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(typeof(CP_Cluster_Core), nameof(CP_Cluster_Core.Setup))]
        public static void SetupSpline(CP_Cluster_Core __instance, int puzzleIndex, iChainedPuzzleOwner owner)
        {
            if (owner == null)
                return;

            // should only be chained puzzle instance.
            // game breaks when having nested clusters...
            ChainedPuzzleInstance instanceOwner = owner.TryCast<ChainedPuzzleInstance>();
            if (instanceOwner == null)
                return;

            var puzzleType = instanceOwner.Data.ChainedPuzzle[puzzleIndex];

            if (!CustomPuzzleManager.TryGetClusterByID(puzzleType.PuzzleType, out var cluster))
                return;

            // check children first for valid reveal speed before checking cluster
            if (CustomPuzzleManager.TryGetScanByID(cluster.BioscanID, out var scan) &&
                (scan.RevealTime > 0f || scan.RevealTimeDistanceMode != RevealDistanceMode.Distance))
            {
                foreach (var childPuzzle in __instance.m_childCores)
                {
                    var child = childPuzzle.TryCast<CP_Bioscan_Core>();
                    if (child == null) continue;

                    var childSpline = child.m_spline.TryCast<CP_Holopath_Spline>();
                    if (childSpline == null) continue;

                    if (scan.RevealTime > 0f)
                        childSpline.m_revealSpeed = scan.SplineRevealSpeed;
                    if (scan.RevealTimeDistanceMode == RevealDistanceMode.Time)
                        childSpline.m_splineLength = 1f; // set it to one so constant distance
                }
            }

            if (cluster.RevealTime <= 0f)
                return;

            var spline = __instance.m_spline.TryCast<CP_Holopath_Spline>();
            if (spline == null)
                return;

            spline.m_revealSpeed = cluster.RevealTime;
        }
    }
}

﻿using ChainedPuzzles;
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

            //TODO: change owner fetching method -> add custom component that includes their puzzle persistentID once Field inject has merged.
            ChainedPuzzleInstance instanceOwner = owner.TryCast<ChainedPuzzleInstance>();
            if (instanceOwner == null)
                return;

            var puzzleType = instanceOwner.Data.ChainedPuzzle[puzzleIndex];
            if (!CustomPuzzleManager.TryGetScanByID(puzzleType.PuzzleType, out var scan))
                return;

            var spline = __instance.m_spline.TryCast<CP_Holopath_Spline>();
            if (spline != null)
            {
                scan.ApplySplineRevealSpeed(spline);
            }

            var graphic = __instance.m_graphics.TryCast<CP_Bioscan_Graphics>();
            if (graphic != null)
            {
                if (!string.IsNullOrEmpty(scan.BioScanGraphics.ScanText))
                {
                    graphic.SetText(scan.BioScanGraphics.ScanText);
                }
            }
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
                (scan.RevealTime > 0f || scan.RevealMode != RevealMode.ScaleByDistance))
            {
                foreach (var childPuzzle in __instance.m_childCores)
                {
                    var child = childPuzzle.TryCast<CP_Bioscan_Core>();
                    if (child == null) continue;

                    var childSpline = child.m_spline.TryCast<CP_Holopath_Spline>();
                    if (childSpline != null)
                    {
                        scan.ApplySplineRevealSpeed(childSpline);
                    }

                    var childGraphic = child.m_graphics.TryCast<CP_Bioscan_Graphics>();
                    if (childGraphic != null)
                    {
                        if (!string.IsNullOrEmpty(scan.BioScanGraphics.ScanText))
                        {
                            childGraphic.SetText(scan.BioScanGraphics.ScanText);
                        }
                    }
                }
            }

            var spline = __instance.m_spline.TryCast<CP_Holopath_Spline>();
            if (spline == null)
                return;

            scan.ApplySplineRevealSpeed(spline);
        }
    }
}
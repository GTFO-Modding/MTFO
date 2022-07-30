using ChainedPuzzles;
using HarmonyLib;
using MTFO.Custom.CCP.Components;
using MTFO.CustomCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTFO.Custom.CCP.Patches
{
    [HarmonyPatch(typeof(CP_Bioscan_Hud))]
    static class Patch_Bioscan_Hud
    {
        [HarmonyPatch(nameof(CP_Bioscan_Hud.Setup))]
        [HarmonyPostfix]
        internal static void Post_Setup(CP_Bioscan_Hud __instance)
        {
            UpdateText(__instance);
        }

        [HarmonyPatch(nameof(CP_Bioscan_Hud.UpdateText))]
        [HarmonyPostfix]
        internal static void Post_UpdateText(CP_Bioscan_Hud __instance)
        {
            UpdateText(__instance);
        }

        private static void UpdateText(CP_Bioscan_Hud hud)
        {
            var data = hud.GetComponent<CorePuzzleData>();
            if (data == null)
                return;

            if (!CustomPuzzleManager.TryGetScanByID(data.PersistentID.Value, out var scanData))
                return;

            if (scanData.BioScanGraphics.HideScanText)
            {
                hud.m_bioscanWorldText.SetText(string.Empty);
                return;
            }

            if (!string.IsNullOrWhiteSpace(scanData.BioScanGraphics.ScanText))
            {
                hud.m_bioscanWorldText.SetText(scanData.BioScanGraphics.ScanText);
            }
        }
    }
}

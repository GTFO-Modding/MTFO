using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;

namespace MTFO.Patches
{
    [HarmonyPatch(typeof(PUI_Watermark), "UpdateWatermark")]
    class Patch_WatermarkUpdateWatermark
    {
        public static void Postfix(ref PUI_Watermark __instance)
        {
            __instance.m_watermarkText.SetText($"<color=red>MODDED</color>\n<color=orange>{VersionInfo.Version}</color>\n<b>REV_{CellBuildData.GetRevision()}</b>");
        }
    }
}

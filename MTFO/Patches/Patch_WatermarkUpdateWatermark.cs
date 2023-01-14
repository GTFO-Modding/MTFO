using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;

namespace MTFO.Patches
{
    [HarmonyPatch(typeof(PUI_Watermark), nameof(PUI_Watermark.UpdateWatermark))]
    static class Patch_WatermarkUpdateWatermark
    {
        public static void Postfix(PUI_Watermark __instance)
        {
            __instance.m_watermarkText.SetText("<color=red>MODDED</color> <color=orange>" + VersionInfo.Version + "</color>");
        }
    }
}

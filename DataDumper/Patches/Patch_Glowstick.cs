using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameData;
using Harmony;
using UnityEngine;
using DataDumper.Managers;
using DataDumper.Utilities;
using FX_EffectSystem;
using SNetwork;
using DataDumper.Custom;

namespace DataDumper.Patches
{
    [HarmonyPatch(typeof(GlowstickInstance), "Update")]
    class Patch_Glowstick
    {
        //This could be written more efficently if you re-make the glowstick logic and then just overwrite the original
        //
        //but im lazy
        public static void Postfix(ref GlowstickInstance __instance)
        {
            if (ConfigManager.CustomContent.GlowstickHolder == null) return; 
            if (__instance == null) return;
            if (__instance.m_state == eFadeState.FadeOut || __instance.m_state == eFadeState.Done) return;
            if (!ConfigManager.CustomContent.GlowstickHolder.GlowstickLookup.TryGetValue(__instance.PublicName, out CustomGlowstick customGlowstick)) return;

            __instance.m_light.SetRange(customGlowstick.Range);
            __instance.m_light.SetColor(customGlowstick.Color * __instance.m_progression);
            __instance.m_light.UpdateData();
        }
    }
}

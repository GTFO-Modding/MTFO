using AK;
using ChainedPuzzles;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTFO.Patches
{
    [HarmonyPatch]
    static class Patch_InstantRevealPuzzlePath
    {
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch(typeof(CP_Holopath_Spline), nameof(CP_Holopath_Spline.Reveal))]
        public static bool Reveal(CP_Holopath_Spline __instance)
        {
            if (__instance.m_revealSpeed < 0)
            {
                if (!__instance.m_isSetup)
                    return false;
                __instance.SetSplineProgress(1f);
                __instance.SetVisible(true);
                __instance.m_sound.UpdatePosition(__instance.TipPos);
                if (!__instance.m_didSetColor)
                {
                    __instance.m_didSetColor = __instance.TrySetColor();
                }
                __instance.m_sound.Post(EVENTS.BIOSCAN_TUBE_EMITTER_STOP, true);
                __instance.OnRevealDone?.Invoke();
                return false;
            }
            return true;
        }
    }
}

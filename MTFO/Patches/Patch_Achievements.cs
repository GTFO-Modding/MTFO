using System;
using GameEvent;
using SNetwork;
using HarmonyLib;
using MTFO.Utilities;
using MTFO.Managers;

namespace MTFO.Patches
{
    [HarmonyPatch(typeof(AchievementManager), nameof(AchievementManager.Setup))]
    static class Patch_Achievements
    {
        public static void Prefix(AchievementManager __instance)
        {
            if (!ConfigManager.DisableAchievements) return; 
            __instance.m_allAchievements.Clear();
            __instance.enabled = false;
            Log.Debug($"Cleared achievements {__instance.m_allAchievements.Count}");
        }
    }

    [HarmonyPatch(typeof(AchievementManager), nameof(AchievementManager.CanSkipAchievement))]
    static class Patch_Achievements2
    {
        public static bool Prefix(ref bool __result)
        {
            if (!ConfigManager.DisableAchievements) return false;
            __result = true;
            Log.Debug("Skipped achievement");
            return false;
        }
    }
}

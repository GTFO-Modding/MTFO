using System;
using GameEvent;
using SNetwork;
using HarmonyLib;
using MTFO.Utilities;
using MTFO.Managers;
using Steamworks;

namespace MTFO.Patches
{
    [HarmonyPatch(typeof(SteamUserStats), nameof(SteamUserStats.SetAchievement))]
    internal static class Patch_SteamAPI_Achievement
    {
        static bool Prefix(string pchName)
        {
            if (!ConfigManager.DisableAchievements) return true;
            Log.Error($"Achievement Completion Blocked: {pchName}");
            return false;
        }
    }
}

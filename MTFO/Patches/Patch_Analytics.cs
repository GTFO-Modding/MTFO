using System;
using GameEvent;
using SNetwork;
using HarmonyLib;
using MTFO.Utilities;

namespace MTFO.Patches
{
    [HarmonyPatch(typeof(AnalyticsManager), nameof(AnalyticsManager.OnGameEvent), typeof(GameEventData))]
    static class Patch_Analytics
    {
        public static bool Prefix(GameEventData data)
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(SNet_Core_STEAM), nameof(SNet_Core_STEAM.SetFriendsData), typeof(FriendsDataType), typeof(string))]
    static class Patch_RichPresence2
    {
        public static void Prefix(FriendsDataType type, ref string data, SNet_Core_STEAM __instance)
        {
            switch(type)
            {
                case FriendsDataType.ExpeditionName:
                    data = $"MODDED - {data}";
                    break;
                case FriendsDataType.Revision:
                    data = "0";
                    break;
                default:
                    Log.Verbose($"Lobby data\nType: {type} Data: {data}");
                    break;
            }
        }
    }
}

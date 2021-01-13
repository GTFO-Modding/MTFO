using System;
using GameEvent;
using SNetwork;
using HarmonyLib;

namespace DataDumper.Patches
{
    [HarmonyPatch(typeof(AnalyticsManager), "OnGameEvent", typeof(GameEventData))]
    class Patch_Analytics
    {
        public static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(SNet_Core_STEAM), "SetFriendsData", new Type[] { typeof(string), typeof(string) })]
    class Patch_RichPresence1
    {
        public static void Prefix(ref string data)
        {
            data = "";
        }
    }

    [HarmonyPatch(typeof(SNet_Core_STEAM), "SetFriendsData", new Type[] { typeof(FriendsDataType), typeof(string) })]
    class Patch_RichPresence2
    {
        public static void Prefix(ref string data)
        {
            data = "";
        }
    }
}

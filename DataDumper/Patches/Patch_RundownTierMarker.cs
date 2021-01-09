using CellMenu;
using DataDumper.Managers;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDumper.Patches
{
    [HarmonyPatch(typeof(CM_PageRundown_New), "PlaceRundown")]
    class Patch_RundownTierMarker
    {
        public static void Postfix(CM_PageRundown_New __instance)
        {
            ContentManager contentManager = ConfigManager.CustomContent;
            if (contentManager == null) return;

            __instance.m_tierMarker1.m_name = contentManager.TierNames.Tier1;
            __instance.m_tierMarker2.m_name = contentManager.TierNames.Tier2;
            __instance.m_tierMarker3.m_name = contentManager.TierNames.Tier3;
            __instance.m_tierMarker4.m_name = contentManager.TierNames.Tier4;
            __instance.m_tierMarker5.m_name = contentManager.TierNames.Tier5;

            __instance.m_tierMarker1.UpdateHeader();
            __instance.m_tierMarker2.UpdateHeader();
            __instance.m_tierMarker3.UpdateHeader();
            __instance.m_tierMarker4.UpdateHeader();
            __instance.m_tierMarker5.UpdateHeader();
        }
    }
}

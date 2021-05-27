using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using CellMenu;
using TMPro;

namespace MTFO.Patches
{
    [HarmonyPatch(typeof(CM_PageRundown_New), "Update")]
    class Patch_PageRundownUpdate
    {
        public static void Postfix(ref CM_PageRundown_New __instance)
        {
            //Replace discord button text and link
            __instance.m_discordButton.SetText("Mod Server");
            __instance.m_discordButton.OnBtnPressCallback = (id) => Application.OpenURL("https://discord.com/invite/rRMPtv4FAh");

            //Disable and hide the matchmake button on the left
            __instance.m_matchmakeAllButton.SetVisible(false);
            __instance.m_matchmakeAllButton.SetText("MATCHMAKE DISABLED");
            __instance.m_matchmakeAllButton.OnBtnPressCallback = null;

            //Hide the matchmake button when selecting an expedition
            __instance.m_expeditionWindow.m_matchButton.SetVisible(false);
        }
    }
}

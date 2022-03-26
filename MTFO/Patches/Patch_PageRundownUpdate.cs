using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using CellMenu;
using TMPro;
using UnityEngine;
using System;

namespace MTFO.Patches
{
    [HarmonyPatch(typeof(CM_PageRundown_New), nameof(CM_PageRundown_New.Intro_RevealRundown))]
    class Patch_PageRundownNew
    {
        public static void Postfix(ref CM_PageRundown_New __instance)
        {
            //Replace discord button text and link
            __instance.m_discordButton.SetText("MOD SERVER");
            __instance.m_discordButton.OnBtnPressCallback = (Action<int>)((id) => Application.OpenURL("https://discord.com/invite/rRMPtv4FAh"));


            //Disable and hide the matchmake button on the left
            __instance.m_matchmakeAllButton.SetVisible(false);
            __instance.m_matchmakeAllButton.SetText("MATCHMAKE DISABLED");
            __instance.m_matchmakeAllButton.OnBtnPressCallback = null;

            __instance.m_aboutTheRundownButton.SetText("THUNDERSTORE");
            __instance.m_aboutTheRundownButton.OnBtnPressCallback = (Action<int>)((id) => Application.OpenURL("https://gtfo.thunderstore.io/"));

            //Hide the matchmake button when selecting an expedition
            __instance.m_expeditionWindow.m_matchButton.SetText("MATCHMAKE DISABLED");
            __instance.m_expeditionWindow.m_matchButton.m_visible = false;
            __instance.m_expeditionWindow.m_matchButton.OnBtnPressCallback = null;
        }
    }
}

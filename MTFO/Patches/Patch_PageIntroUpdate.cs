using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using CellMenu;
using UnityEngine;

namespace MTFO.Patches
{
    [HarmonyPatch(typeof(CM_PageIntro), "Update")]
    class Patch_PageIntroUpdate
    {
        public static void Postfix(ref CM_PageIntro __instance)
        {
            //Replace report bug button text and link
            __instance.m_startupScreen.m_btnBug.SetText("MTFO Wiki");
            __instance.m_startupScreen.m_btnBug.OnBtnPressCallback = (Action<int>)((id) => Application.OpenURL("https://wiki.mtfo.dev/"));

            //Replace discord button text and link
            __instance.m_startupScreen.m_btnDiscord.SetText("Mod Server");
            __instance.m_startupScreen.m_btnDiscord.OnBtnPressCallback = (Action<int>)((id) => Application.OpenURL("https://discord.com/invite/rRMPtv4FAh"));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using CellMenu;
using TMPro;

namespace MTFO.Patches
{
    [HarmonyPatch(typeof(CM_PageLoadout), "UpdatePageData")]
    class Patch_PageLoadoutUpdatePageData
    {
        public static void Prefix(ref CM_PageLoadout __instance)
        {
            //Replace text blurb on the top right in lobby
            __instance.m_movingContentHolder.Find("ShareServerId/ShareText").gameObject.GetComponent<TextMeshPro>()
                .SetText("Do not play modded content on the official GTFO server or online matchmake lobbies.\n\nFeel free to join the unofficial discord server linked below and ask people to play.");

            //Replace discord button text and link
            __instance.m_discordButton.SetText("Mod Server");
            __instance.m_discordButton.OnBtnPressCallback = (id) => Application.OpenURL("https://discord.com/invite/rRMPtv4FAh");
        }
    }
}

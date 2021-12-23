using System;
using MTFO.Utilities;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using CellMenu;
using GameData;
using TMPro;
using UnityEngine;

namespace MTFO.Patches
{
    [HarmonyPatch(typeof(CM_PageLoadout), "UpdatePlayerBars")]
    class Patch_PageLoadoutUpdatePlayerBars
    {
        public static void Prefix(ref CM_PageLoadout __instance)
        {
            //Disable matchmake buttons behind players
            foreach(CM_PlayerLobbyBar playerLobbyBar in __instance.m_playerLobbyBars)
            { 
                playerLobbyBar.m_matchmakeButton.SetOnBtnPressCallback(null);
                playerLobbyBar.m_matchmakeButton.SetText("DISABLED");
            }
        }
    }
}
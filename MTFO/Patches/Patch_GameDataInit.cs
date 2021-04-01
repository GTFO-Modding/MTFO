using GameData;
using HarmonyLib;
using MTFO.Managers;
using MTFO.PartialData;
using MTFO.Utilities;
using MTFO.Utilities.JsonConverters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MTFO.Patches
{
    [HarmonyPatch(typeof(GameDataInit), "Initialize")]
    class Patch_GameDataInit
    {
        private static bool Once = false;

        public static void Postfix()
        {
            if (Once)
                return;

            Once = true;

            Log.Verbose("Loading PartialData Files");

            PartialDataManager.UpdatePartialData();

            Log.Verbose("Loaded PartialData Files");
        }
    }
}

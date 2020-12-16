using CellMenu;
using Harmony;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataDumper.Managers;

namespace DataDumper.Patches
{
    [HarmonyPatch(typeof(CM_StartupScreen), "SetText")]
    class Patch_IntroText
    {
        public static void Prefix(ref string txt)
        {
            if (ConfigManager.MenuText != null)
            {
                txt = ConfigManager.MenuText;
            }
        }
    }
}

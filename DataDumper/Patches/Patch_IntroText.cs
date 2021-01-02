using CellMenu;
using HarmonyLib;
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

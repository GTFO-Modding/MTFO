using CellMenu;
using HarmonyLib;
using MTFO.Managers;

namespace MTFO.Patches
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

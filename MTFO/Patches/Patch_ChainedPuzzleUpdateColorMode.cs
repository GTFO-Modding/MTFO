using ChainedPuzzles;
using HarmonyLib;
using System.Text.RegularExpressions;

namespace MTFO.Patches
{
    [HarmonyPatch(typeof(CP_Bioscan_Graphics), nameof(CP_Bioscan_Graphics.SetColorMode))]
    class Patch_ChainedPuzzleUpdateColorMode
    {
        public static Regex s_replacerRegex = new Regex(@" \[[0-9]+%\]<size=0>Show-Percent$", RegexOptions.Compiled);

        private static string GetProgressString(CP_Bioscan_Graphics graphics)
        {
            var core = graphics.GetComponent<CP_Bioscan_Core>();

            return core.m_sync.GetCurrentState().progress
                .ToString() + "%";
        }

        public static void Postfix(CP_Bioscan_Graphics __instance)
        {
            string text = __instance.m_textMeshRenderer.text;

            if (text.EndsWith("<size=0>Show-Percent"))
            {
                text = s_replacerRegex.Replace(text, "");
                text += $" [{GetProgressString(__instance)}]<size=0>Show-Percent";
            }

            __instance.SetText(text);
        }
    }
}

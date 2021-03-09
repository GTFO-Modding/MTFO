using CellMenu;
using MTFO.Managers;
using MTFO.HotReload;
using UnhollowerRuntimeLib;
using BepInEx.IL2CPP;
using BepInEx;
using HarmonyLib;


namespace MTFO
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class MTFO : BasePlugin
    {
        public const string
            MODNAME = "MTFO",
            AUTHOR = "dak",
            GUID = "com." + AUTHOR + "." + MODNAME,
            VERSION = VersionInfo.SemVer;


        public override void Load()
        {
            ClassInjector.RegisterTypeInIl2Cpp<HotReloader>();


            var harmony = new Harmony(GUID);
            if (ConfigManager.IsHotReloadEnabled)
            {
                var hotReloadInjectPoint = typeof(CM_PageRundown_New).GetMethod("OnEnable");
                var hotReloadPatch = typeof(HotReloadInjector).GetMethod("PostFix");
                harmony.Patch(hotReloadInjectPoint, null, new HarmonyMethod(hotReloadPatch));
            }
            harmony.PatchAll();
        }
    }
}

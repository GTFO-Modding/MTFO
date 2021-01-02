using CellMenu;
using DataDumper.Managers;
using DataDumper.HotReload;
using UnhollowerRuntimeLib;
using BepInEx.IL2CPP;
using BepInEx;
using HarmonyLib;


namespace DataDumper
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class DataDumperMain : BasePlugin
    {
        public const string
            MODNAME = "Data-Dumper",
            AUTHOR = "Dak",
            GUID = "com." + AUTHOR + "." + MODNAME,
            VERSION = "3.0.0";


        public override void Load()
        {
            ClassInjector.RegisterTypeInIl2Cpp<HotReloader>();


            var harmony = new Harmony(GUID);
            if (ConfigManager.IsHotReloadEnabled)
            {
                var hotReloadInjectPoint = typeof(CM_PageIntro).GetMethod("EXT_PressInject");
                var hotReloadPatch = typeof(HotReloadInjector).GetMethod("PostFix");
                harmony.Patch(hotReloadInjectPoint, null, new HarmonyMethod(hotReloadPatch));
            }
            harmony.PatchAll();
        }
    }
}

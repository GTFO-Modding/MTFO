using CellMenu;
using MTFO.Managers;
using MTFO.HotReload;
using UnhollowerRuntimeLib;
using BepInEx.IL2CPP;
using BepInEx;
using HarmonyLib;
using UnityEngine.Analytics;


namespace MTFO
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInDependency("dev.gtfomodding.gtfo-api", BepInDependency.DependencyFlags.HardDependency)]
    public class MTFO : BasePlugin
    {
        public const string
            MODNAME = "MTFO",
            AUTHOR = "dak",
            GUID = "com." + AUTHOR + "." + MODNAME,
            VERSION = VersionInfo.SemVer;


        public override void Load()
        {
            Analytics.enabled = false;

            var harmony = new Harmony(GUID);

            if (ConfigManager.IsHotReloadEnabled)
            {
                ClassInjector.RegisterTypeInIl2Cpp<HotReloaderBehaviour>();
                harmony.PatchAll(typeof(HotReloadInjector));
            }
                
            harmony.PatchAll();
        }
    }
}

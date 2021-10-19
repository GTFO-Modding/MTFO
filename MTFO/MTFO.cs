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
    /// <summary>
    /// Main MTFO plugin
    /// </summary>
    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInDependency("dev.gtfomodding.gtfo-api", BepInDependency.DependencyFlags.HardDependency)]
    public class MTFO : BasePlugin
    {
        /// <summary>
        /// Descriptors of MTFO
        /// </summary>
        public const string
            MODNAME = "MTFO",
            AUTHOR = "dak",
            GUID = "com." + AUTHOR + "." + MODNAME,
            VERSION = VersionInfo.SemVer;


        /// <inheritdoc/>
        public override void Load()
        {
            Analytics.enabled = false;

            var harmony = new Harmony(GUID);

            if (ConfigManager.IsHotReloadEnabled)
            {
                ClassInjector.RegisterTypeInIl2Cpp<HotReloader>();
                harmony.PatchAll(typeof(HotReloadInjector));
            }
                
            harmony.PatchAll();
        }
    }
}

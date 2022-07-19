using MTFO.Managers;
using MTFO.HotReload;
using BepInEx.IL2CPP;
using BepInEx;
using HarmonyLib;
using UnityEngine.Analytics;
using Il2CppInterop.Runtime.Injection;
using GTFO.API;
using MTFO.NativeDetours;

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

            AssetAPI.OnImplReady += () =>
            {
                if (ConfigManager.IsHotReloadEnabled)
                {
                    ClassInjector.RegisterTypeInIl2Cpp<HotReloader>();
                    harmony.PatchAll(typeof(HotReloadInjector));
                }
            };
                
            harmony.PatchAll();
            Detour_DataBlockBase.Patch();
        }
    }
}

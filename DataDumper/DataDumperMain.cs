using CellMenu;
using DataDumper.Managers;
using DataDumper.HotReload;
using DataDumper.Utilities;
using Harmony;
using MelonLoader;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Text;
using UnhollowerRuntimeLib;
using Newtonsoft.Json;
using System.IO;

namespace DataDumper
{
    public class DataDumperMain : MelonMod
    {
        public const string
            MODNAME = "Data-Dumper",
            AUTHOR = "Dak",
            GUID = "com." + AUTHOR + "." + MODNAME,
            VERSION = "2.2.0";


        public override void OnApplicationStart()
        {
            //Inject hot reloader
            ClassInjector.RegisterTypeInIl2Cpp<HotReloader>();

            MelonLogger.Log($"Game Version: {ConfigManager.GAME_VERSION}");

            //Setup hotreload if enabled
            var harmony = HarmonyInstance.Create(GUID);
            if (ConfigManager.IsHotReloadEnabled)
            {
                var hotReloadInjectPoint = typeof(CM_PageIntro).GetMethod("EXT_PressInject");
                var hotReloadPatch = typeof(HotReloadInjector).GetMethod("PostFix");
                harmony.Patch(hotReloadInjectPoint, null, new HarmonyMethod(hotReloadPatch));
            }
        }
    }
}

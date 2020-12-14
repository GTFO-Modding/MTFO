using CellMenu;
using GTFO_DataDumper.HotReload;
using Harmony;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace DataDumper
{
    public class DataDumperMain : MelonMod
    {
        public int GAME_VERSION;
        public static Dictionary<int, string> gameDataLookup;
        //hardcoded
        public static string GameDataSavePath = Path.Combine(MelonLoaderBase.UserDataPath, "GameData");


        public override void OnApplicationStart()
        {
            //Inject hot reloader
            ClassInjector.RegisterTypeInIl2Cpp<HotReloader>();

            MelonLogger.Log("Reading game version...");
            
            if (!Directory.Exists(GameDataSavePath))
            {
                Directory.CreateDirectory(GameDataSavePath);
            }

            //This is really backwards because I was getting werid crashes when just trying to read the text
            //and couldn't be bothered to figure out why
            string gameVersionPath = Path.Combine(Imports.GetGameDirectory(), "revision.txt");
            string gameVersion = File.ReadAllText(gameVersionPath);
            int.TryParse(gameVersion, out int result);
            GAME_VERSION = result;
            MelonLogger.Log($"Game Version: {GAME_VERSION}");


            var harmony = HarmonyInstance.Create("com.dakkhuza.offshoot");
            if (true) //Temp force on until meeelon loader config is setup
            {
                var hotReloadInjectPoint = typeof(CM_PageIntro).GetMethod("EXT_PressInject");
                var hotReloadPatch = typeof(HotReloadInjector).GetMethod("PostFix");
                harmony.Patch(hotReloadInjectPoint, null, new HarmonyMethod(hotReloadPatch));
            }
            harmony.PatchAll();
            /*
            *  Hash local game data for comparing
            */
            //Create gamedata lookup
            MelonLogger.Log("Hashing GameData...");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            gameDataLookup = new Dictionary<int, string>();
            ResourceSet gameData = GameData.ResourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, true);
            IDictionaryEnumerator gameDataCollection = gameData.GetEnumerator();

            while (gameDataCollection.MoveNext())
            {
                byte[] byteKey = gameDataCollection.Value as byte[];
                string key = Encoding.UTF8.GetString(byteKey);
                int hash = key.GetHashCode();
                gameDataLookup.Add(hash, gameDataCollection.Key as string);
            }
            sw.Stop();
            MelonLogger.Log("Hash done!");
            MelonLogger.Log("Time elapsed: " + sw.Elapsed);
        }

        [HarmonyPatch(typeof(BinaryEncoder), "Decode")]
        class Patch_BinaryDecoder
        {
            public static void Postfix(ref string __result)
            {
                //Ensure the file is game data related
                if (__result.Contains("Headers"))
                {
                    int hash = __result.GetHashCode();
                    gameDataLookup.TryGetValue(hash, out string name);

                    if (name != null)
                    {
                        MelonLogger.Log("Found " + name);
                        try
                        {
                            string filePath = Path.Combine(GameDataSavePath, name + ".json");
                            if (File.Exists(filePath))
                            {
                                MelonLogger.Log("Reading [" + name + "] from disk...");
                                __result = File.ReadAllText(filePath);
                                return;
                            }
                            else
                            {
                                MelonLogger.Log("No file found at [" + filePath + "], writing file to disk...");
                                File.WriteAllText(filePath, __result);
                            }
                        }
                        catch
                        {
                            MelonLogger.LogError("Failed to write " + name + " to disk!!");
                        }
                    }
                    else
                    {
                        string errorPath = Path.Combine(GameDataSavePath, "UNKNOWN");
                        if (!Directory.Exists(errorPath))
                        {
                            Directory.CreateDirectory(errorPath);
                        }
                        string errorFilePath = Path.Combine(errorPath, hash + ".json");
                        MelonLogger.LogError("Failed to find match for hash [" + hash + "]!");
                        if (File.Exists(errorFilePath))
                        {
                            MelonLogger.LogWarning("-- FILE FOUND IN DUMP FOLDER WITH MATCHING HASH FILE NAME, LOADING INSTEAD --");
                            __result = File.ReadAllText(errorFilePath);
                            return;
                        }

                        MelonLogger.LogError("----- FILE CONTENT DUMP START -----");
                        MelonLogger.LogError(__result);
                        MelonLogger.LogError("----- FILE CONTENT DUMP END -----");
                        MelonLogger.LogError("DUMPING FILE CONTENTS TO [" + errorFilePath + "]");
                        File.WriteAllText(errorFilePath, __result);
                    }
                }
            }
        }

        public class HotReloadInjector
        {
            //[HarmonyPostfix]
            public static void PostFix()
            {
                GameObject gameObject = new GameObject();
                gameObject.AddComponent<HotReloader>();
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
            }
        }
    }
}

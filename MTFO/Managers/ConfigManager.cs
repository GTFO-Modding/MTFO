using MTFO.Utilities;
using BepInEx.Configuration;
using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

namespace MTFO.Managers
{
    public static class ConfigManager
    {
        private const string
            CUSTOM_FOLDER = "Custom",
            GAMEDATA_LOOKUP = @"https://lookup.gtfomodding.dev/lookup/";

        // Anylitics Manager

        static ConfigManager()
        {            
            //Setup Config
            var CONFIG_PATH = Path.Combine(Paths.ConfigPath, "MTFO.cfg");
            var OLD_CONFIG_PATH = Path.Combine(Paths.ConfigPath, "DataDumper.cfg");
            //Handle old config
            if (File.Exists(OLD_CONFIG_PATH))
            {
                Log.Debug("Updating old config");
                if (File.Exists(CONFIG_PATH)) File.Delete(CONFIG_PATH);
                File.Move(OLD_CONFIG_PATH, CONFIG_PATH);
            }


            ConfigFile config = new(CONFIG_PATH, true);

            _enableHotReload = config.Bind(ConfigStrings.SECTION_DEV, ConfigStrings.SETTING_HOTRELOAD, false, ConfigStrings.SETTING_HOTRELOAD_DESC);
            _dumpGameData = config.Bind(ConfigStrings.SECTION_DEV, ConfigStrings.SETTING_DUMPDATA, false, ConfigStrings.SETTING_DUMPDATA_DESC);
            _isVerbose = config.Bind(ConfigStrings.SECTION_DEBUG, ConfigStrings.SETTING_VERBOSE, false, ConfigStrings.SETTING_VERBOSE_DESC);
            _useLegacyLoading = config.Bind(ConfigStrings.SECTION_GENERAL, ConfigStrings.SETTING_USE_LEGACY_PATH, false, ConfigStrings.SETTING_USE_LEGACY_PATH_DESC);

            //Obsolte
            _rundownFolder = config.Bind(ConfigStrings.SECTION_GENERAL, ConfigStrings.SETTING_RUNDOWNPACKAGE, "default", ConfigStrings.SETTING_RUNDOWNPACKAGE_DESC);

            //Get game version
            GAME_VERSION = GetGameVersion();

            //Setup Paths
            GameDataLookupPath = PathUtil.MakeRelativeDirectory(Paths.ConfigPath, "_gamedatalookup");
            GameDataLookupPath = Path.Combine(GameDataLookupPath, $"{GAME_VERSION}.json");
            if (File.Exists(GameDataLookupPath))
            {
                string content = File.ReadAllText(GameDataLookupPath);
                Log.Verbose(content);
                var json = new JsonSerializer();
                gameDataLookup = json.Deserialize<Dictionary<int, string>>(content);
            }

            
            if (UseLegacyLoading)
            {
                string path = _rundownFolder.Value;
                if (path.Equals("default", StringComparison.InvariantCultureIgnoreCase))
                {
                    GameDataPath = PathUtil.MakeRelativeDirectory("GameData_" + GAME_VERSION, createPath: false);
                }
                else
                {
                    GameDataPath = PathUtil.MakeRelativeDirectory(path, createPath: false);
                }
                HasGameDataPath = Directory.Exists(GameDataPath);
            }
            else
            {
                string[] files = Directory.GetFiles(Paths.PluginPath, "GameData_*.json", SearchOption.AllDirectories);
                foreach (string file in files.OrderBy(Path.GetDirectoryName))
                {
                    if (Path.GetDirectoryName(file) != GameDataPath)
                    {
                        HasGameDataPath = true;
                        GameDataPath = Path.GetDirectoryName(file);
                        break;
                    }
                }
            }

            if (HasGameDataPath)
            {
                CustomPath = Path.Combine(GameDataPath, CUSTOM_FOLDER);
                HasCustomContent = Directory.Exists(CustomPath);
            }
            else
            {
                GameDataPath = string.Empty;
                CustomPath = string.Empty;
                HasCustomContent = false;
            }

            //Setup Managers
            try
            {
                CustomContent = new ContentManager();
            } catch (Exception err)
            {
                HasCustomContent = false;
                Log.Error($"Failed to init custom content!\nIs your JSON valid?\n---- ERROR MESSAGE ---- {err} ---- END ERROR MESSAGE ----");
            }

            //Setup GameData Lookup
            //GetGameDataLookupV2();

            //Debug
            Log.Debug("---- DEBUG INFO ----");

            Log.Debug($"Time: {DateTime.Now}");
            Log.Debug($"Game Version: {GAME_VERSION}");

            Log.Debug("---- PATHS ----");

            Log.Debug($"Path to rundown: {GameDataPath}");
            Log.Debug($"Path to custom content: {CustomPath}");

            Log.Debug("---- FLAGS ----");

            Log.Debug($"Has GameData Path? {HasGameDataPath}");
            Log.Debug($"Has Custom Content? {HasCustomContent}");
            Log.Debug($"Hot Reload Enabled? {IsHotReloadEnabled}");
            Log.Debug($"Verbose Logging? {IsVerbose}");
            Log.Debug($"Using Legacy Loading? {UseLegacyLoading}");
            Log.Debug($"Dump Game Data? {DumpGameData}");

            Log.Debug($"---- DEBUG END ----");
        }

        private static readonly ConfigEntry<bool> _enableHotReload;
        private static readonly ConfigEntry<string> _rundownFolder;
        private static readonly ConfigEntry<bool> _dumpGameData;
        private static readonly ConfigEntry<bool> _isVerbose;
        private static readonly ConfigEntry<bool> _useLegacyLoading;

        public static int GAME_VERSION;

        //GameData Lookup
        public static Dictionary<int, string> gameDataLookup;

        //Managers
        public static ContentManager CustomContent;

        //Paths
        public static readonly string GameDataPath;
        public static readonly string CustomPath;
        public static readonly string GameDataLookupPath;

        //Flags
        public static bool HasGameDataPath = false;
        public static bool HasCustomContent = false;
        public static bool IsModded = false;
        public static bool IsVerbose
        {
            get
            {
                return _isVerbose.Value;
            }
        }

        //Dev Tools
        public static bool IsHotReloadEnabled 
        { 
            get
            {
                return _enableHotReload.Value;
            } 
        }

        public static bool DumpGameData
        {
            get
            {
                return _dumpGameData.Value;
            }
        }

        //Legacy
        public static bool UseLegacyLoading
        {
            get
            {
                return _useLegacyLoading.Value;
            }
        }

        private static void GetGameDataLookupV2()
        {
            var textList = Resources.LoadAll<TextAsset>("GameData");
            Log.Debug($"Text list: {textList.Count}");

            foreach (var text in textList)
            {
                if (text.name.EndsWith("DataBlock_bin"))
                {
                    
                }
            }

            //foreach (var text in textList)
            //{
            //    if (text.name.EndsWith("DataBlock_bin"))
            //    { 
            //        Log.Debug(string.Format("Found DataBlock: {0} hash: {1}", text.name, text.bytes.GetHashCode()));
            //        gameDataLookup.Add(text.bytes, text.name);
            //        
            //    } else
            //    {
            //        Log.Debug($"Skipping {text.name}, does not end with 'DataBlock_bin...'");
            //    }
            //}
        }

        private static int GetGameVersion()
        {
            return CellBuildData.GetRevision();
        }
    }
}

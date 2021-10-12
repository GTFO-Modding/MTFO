using MTFO.Utilities;
using BepInEx.Configuration;
using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text.Json;
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
            _dumpFiles = config.Bind(ConfigStrings.SECTION_DEV, ConfigStrings.SETTING_DUMPFILE, false, ConfigStrings.SETTING_DUMPFILE_DESC);
            _isVerbose = config.Bind(ConfigStrings.SECTION_DEBUG, ConfigStrings.SETTING_VERBOSE, false, ConfigStrings.SETTING_VERBOSE_DESC);
            _useLegacyLoading = config.Bind(ConfigStrings.SECTION_GENERAL, ConfigStrings.SETTING_USE_LEGACY_PATH, false, ConfigStrings.SETTING_USE_LEGACY_PATH_DESC);

            //Obsolte
            _rundownFolder = config.Bind(ConfigStrings.SECTION_GENERAL, ConfigStrings.SETTING_RUNDOWNPACKAGE, "default", ConfigStrings.SETTING_RUNDOWNPACKAGE_DESC);

            //Get game version
            GAME_VERSION = GetGameVersion();

            //Setup Paths
            GameDataLookupPath = GamePath.Create(PathUtil.MakeRelativeDirectory(Paths.ConfigPath, "_gamedatalookup"))
                .GetCurrentDirectory();
            if (GameDataLookupPath
                .GetPath().PathFile.Exists($"{GAME_VERSION}.json"))
            {
                string content = GameDataLookupPath.GetPath()
                    .PathFile.ReadAllText($"{GAME_VERSION}.json");
                Log.Verbose(content);
                gameDataLookup = JsonSerializer.Deserialize<Dictionary<int, string>>(content, ContentManager.s_SerializerOptions);
            }

            string path = _rundownFolder.Value;
            if (UseLegacyLoading)
            {
                GameDataPath = GamePath.Create(path != "default" ? PathUtil.MakeRelativeDirectory(path) : PathUtil.MakeRelativeDirectory("GameData_" + GAME_VERSION))
                    .GetCurrentDirectory();
            } else
            {
                var rawGameDataPath = PathUtil.MakeRelativeDirectory(Paths.PluginPath, "GameData_" + GAME_VERSION);
                GameDataPath = GamePath.Create(rawGameDataPath)
                    .GetCurrentDirectory();

                var searchPath = GamePath.Create(Paths.PluginPath);

                var files = searchPath.GetCurrentDirectory().GetFiles("GameData_*.json", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    if (file.DirectoryName != rawGameDataPath)
                    {
                        GameDataPath = GamePath.Create(file.DirectoryName)
                            .GetCurrentDirectory();
                        break;
                    }
                }
            }

            CustomPath = GameDataPath
                .GetDirectories()
                .FirstOrDefault((dir) => dir.Name == CUSTOM_FOLDER);

            //Setup flags 
            HasCustomContent = CustomPath.Exists();

            //Setup folders
            if (!GameDataPath.Exists())
            {
                GameDataPath.Create();
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
            Log.Debug($"Loading Rundown: {path}");

            Log.Debug("---- PATHS ----");

            Log.Debug($"Path to rundown: {GameDataPath}");
            Log.Debug($"Path to custom content: {CustomPath}");

            Log.Debug("---- FLAGS ----");

            Log.Debug($"Has Custom Content? {HasCustomContent}");
            Log.Debug($"Hot Reload Enabled? {IsHotReloadEnabled}");
            Log.Debug($"Dump Unknown Files? {DumpUnknownFiles}");
            Log.Debug($"Verbose Logging? {IsVerbose}");
            Log.Debug($"Using Legacy Loading? {UseLegacyLoading}");

            Log.Debug($"---- DEBUG END ----");
        }

        private static readonly ConfigEntry<bool> _enableHotReload;
        private static readonly ConfigEntry<string> _rundownFolder;
        private static readonly ConfigEntry<bool> _dumpFiles;
        private static readonly ConfigEntry<bool> _isVerbose;
        private static readonly ConfigEntry<bool> _useLegacyLoading;

        public static int GAME_VERSION;

        //GameData Lookup
        public static Dictionary<int, string> gameDataLookup;

        //Managers
        public static ContentManager CustomContent;

        //Strings
        public static string MenuText;

        //Paths
        public static readonly IDirectory GameDataPath;
        public static readonly IDirectory CustomPath;
        public static readonly IDirectory GameDataLookupPath;

        //Flags
        public static bool HasCustomContent;
        public static bool IsModded;
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

        public static bool DumpUnknownFiles
        {
            get
            {
                return _dumpFiles.Value;
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

        public static string ReadAllTextInGameData(string path)
        {
            return GameDataPath.GetPath()
                .PathFile.ReadAllText(path);
        }

        private static int GetGameVersion()
        {
            return CellBuildData.GetRevision();
        }
    }
}

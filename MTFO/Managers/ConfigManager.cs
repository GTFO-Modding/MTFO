using MTFO.Utilities;
using BepInEx.Configuration;
using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using HarmonyLib;

namespace MTFO.Managers
{
    using CText = ConfigStrings;

    public enum DumpGameDataMode
    {
        Single,
        PartialData,
        FullPartialData
    }

    public static class ConfigManager
    {
        private const string CUSTOM_FOLDER = "Custom";

        private static string ResolveGameDataPath(string rootPath)
        {
            string[] files = Directory.GetFiles(rootPath, "GameData_*.json", SearchOption.AllDirectories);
            foreach (string file in files.OrderBy(Path.GetDirectoryName))
            {
                if (Path.GetDirectoryName(file) != GameDataPath)
                {
                    HasGameDataPath = true;
                    return Path.GetDirectoryName(file);
                }
            }
            return null;
        }

        static ConfigManager()
        {            
            //Setup Config
            var CONFIG_PATH = Path.Combine(Paths.ConfigPath, "MTFO.cfg");

            ConfigFile config = new(CONFIG_PATH, true);

            _enableHotReload = config.Bind(CText.SECTION_DEV, CText.SETTING_HOTRELOAD, false, CText.SETTING_HOTRELOAD_DESC);
            _dumpGameData = config.Bind(CText.SECTION_DEV, CText.SETTING_DUMPDATA, false, CText.SETTING_DUMPDATA_DESC);
            _dumpGameDataMode = config.Bind(CText.SECTION_DEV, CText.SETTING_DUMPDATA_MODE, DumpGameDataMode.Single, CText.SETTING_DUMPDATA_MODE_DESC);
            _isVerbose = config.Bind(CText.SECTION_DEBUG, CText.SETTING_VERBOSE, false, CText.SETTING_VERBOSE_DESC);

            //Get game version
            GAME_VERSION = GetGameVersion();

            GameDataPath = ResolveGameDataPath(Path.Combine(Paths.BepInExRootPath, "GameData"));
            IsPluginGameDataPath = string.IsNullOrEmpty(GameDataPath);

            if (IsPluginGameDataPath)
            {
                GameDataPath = ResolveGameDataPath(Paths.PluginPath);
                Log.Warn("Plugin paths for gamedata are under legacy support and will be removed in the future. Considering migrating to the '\\BepInEx\\GameData' folder.");
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
                IsPluginGameDataPath = false;
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

            //Debug
            Log.Debug("---- DEBUG INFO ----");

            Log.Debug($"Time: {DateTime.Now}");
            Log.Debug($"Game Version: {GAME_VERSION}");

            Log.Debug("---- PATHS ----");

            Log.Debug($"Path to rundown: {GameDataPath}");
            Log.Debug($"Path to custom content: {CustomPath}");

            Log.Debug("---- FLAGS ----");

            Log.Debug($"Has GameData Path? {HasGameDataPath}");
            Log.Debug($"Using plugin GameData path? {IsPluginGameDataPath}");
            Log.Debug($"Has Custom Content? {HasCustomContent}");
            Log.Debug($"Hot Reload Enabled? {IsHotReloadEnabled}");
            Log.Debug($"Verbose Logging? {IsVerbose}");
            Log.Debug($"Dump Game Data? {DumpGameData}");

            Log.Debug($"---- DEBUG END ----");
        }

        private static readonly ConfigEntry<bool> _enableHotReload;
        private static readonly ConfigEntry<bool> _dumpGameData;
        private static readonly ConfigEntry<bool> _isVerbose;
        private static readonly ConfigEntry<DumpGameDataMode> _dumpGameDataMode;

        public static int GAME_VERSION;

        //Managers
        public static ContentManager CustomContent;

        //Paths
        public static readonly string GameDataPath;
        public static readonly string CustomPath;

        //Flags
        public static bool HasGameDataPath = false;
        public static bool HasCustomContent = false;
        public static bool IsModded = false;
        public static bool IsPluginGameDataPath = false;
        public static bool IsVerbose
        {
            get => _isVerbose.Value;
        }

        //Dev Tools
        public static bool IsHotReloadEnabled 
        { 
            get => _enableHotReload.Value;
        }

        public static bool DumpGameData
        {
            get => _dumpGameData.Value;
        }

        public static DumpGameDataMode DumpMode
        {
            get => _dumpGameDataMode.Value;
        }

        private static int GetGameVersion()
        {
            return CellBuildData.GetRevision();
        }
    }
}

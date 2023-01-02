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

            _enableHotReload = config.Bind(ConfigStrings.SECTION_DEV, ConfigStrings.SETTING_HOTRELOAD, false, ConfigStrings.SETTING_HOTRELOAD_DESC);
            _dumpGameData = config.Bind(ConfigStrings.SECTION_DEV, ConfigStrings.SETTING_DUMPDATA, false, ConfigStrings.SETTING_DUMPDATA_DESC);
            _isVerbose = config.Bind(ConfigStrings.SECTION_DEBUG, ConfigStrings.SETTING_VERBOSE, false, ConfigStrings.SETTING_VERBOSE_DESC);

            //Get game version
            GAME_VERSION = GetGameVersion();

            GameDataPath = ResolveGameDataPath(Path.Combine(Paths.BepInExRootPath, "GameData", "MTFO"));
            if (string.IsNullOrEmpty(GameDataPath)) GameDataPath = ResolveGameDataPath(Paths.PluginPath);

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
            Log.Debug($"Dump Game Data? {DumpGameData}");

            Log.Debug($"---- DEBUG END ----");
        }

        private static readonly ConfigEntry<bool> _enableHotReload;
        private static readonly ConfigEntry<bool> _dumpGameData;
        private static readonly ConfigEntry<bool> _isVerbose;

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

        private static int GetGameVersion()
        {
            return CellBuildData.GetRevision();
        }
    }
}

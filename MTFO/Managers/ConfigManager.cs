using MTFO.Utilities;
using MTFO;
using Newtonsoft.Json;
using BepInEx.Configuration;
using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace MTFO.Managers
{
    public static class ConfigManager
    {
        private const string
            CUSTOM_FOLDER = "Custom",
            GAMEDATA_LOOKUP = @"https://lookup.gtfomodding.dev/lookup/";
        
        private static readonly WebClient webClient = new WebClient();
        

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
                File.Move(OLD_CONFIG_PATH, CONFIG_PATH);
            }


            ConfigFile config = new ConfigFile(CONFIG_PATH, true);

            _enableHotReload = config.Bind(ConfigStrings.SECTION_DEV, ConfigStrings.SETTING_HOTRELOAD, false, ConfigStrings.SETTING_HOTRELOAD_DESC);
            _dumpFiles = config.Bind(ConfigStrings.SECTION_DEV, ConfigStrings.SETTING_DUMPFILE, false, ConfigStrings.SETTING_DUMPFILE_DESC);
            _isVerbose = config.Bind(ConfigStrings.SECTION_DEBUG, ConfigStrings.SETTING_VERBOSE, false, ConfigStrings.SETTING_VERBOSE_DESC);
            _useLegacyLoading = config.Bind(ConfigStrings.SECTION_GENERAL, ConfigStrings.SETTING_USE_LEGACY_PATH, false, ConfigStrings.SETTING_USE_LEGACY_PATH_DESC);

            //Obsolte
            _rundownFolder = config.Bind(ConfigStrings.SECTION_GENERAL, ConfigStrings.SETTING_RUNDOWNPACKAGE, "default", ConfigStrings.SETTING_RUNDOWNPACKAGE_DESC);

            //Get game version
            GAME_VERSION = GetGameVersion();

            //Setup Paths
            GameDataLookupPath = PathUtil.MakeRelativeDirectory(Paths.ConfigPath, "_gamedatalookup");

            string path = _rundownFolder.Value;
            if (UseLegacyLoading)
            {
                GameDataPath = path != "default" ? PathUtil.MakeRelativeDirectory(path) : PathUtil.MakeRelativeDirectory("GameData_" + GAME_VERSION);
            } else
            {
                GameDataPath = PathUtil.MakeRelativeDirectory(Paths.PluginPath, "GameData_" + GAME_VERSION);

                string[] files = Directory.GetFiles(Paths.PluginPath, "*.json", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    if (Path.GetDirectoryName(file) != GameDataPath)
                    {
                        GameDataPath = Path.GetDirectoryName(file);
                        break;
                    }
                }
            }

            CustomPath = Path.Combine(GameDataPath, CUSTOM_FOLDER);

            //Setup flags 
            HasCustomContent = Directory.Exists(CustomPath);

            //Setup folders
            if (!Directory.Exists(GameDataPath))
            {
                Directory.CreateDirectory(GameDataPath);
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
            GetGameDataLookup();

            //Debug
            Log.Debug("---- DEBUG INFO ----");

            Log.Debug($"Time: {DateTime.Now}");
            Log.Debug($"Game Version: {GAME_VERSION}");
            Log.Debug($"Loading Rundown: {path}");
            Log.Debug($"GDLU Length: {gameDataLookup.Count}");


            Log.Debug("---- PATHS ----");

            Log.Debug($"Path to rundown: {GameDataPath}");
            Log.Debug($"Path to custom content: {CustomPath}");
            Log.Debug($"Path to GDLU: {GameDataLookupPath}");

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
        public static Dictionary<int, string> gameDataLookup = new Dictionary<int, string>();

        //Managers
        public static ContentManager CustomContent;

        //Strings
        public static string MenuText;

        //Paths
        public static string GameDataPath;
        public static string CustomPath;
        private static readonly string GameDataLookupPath;

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

        private static void GetGameDataLookup()
        {
            string versionLookupPath = Path.Combine(GameDataLookupPath, GAME_VERSION + ".json");
            if (PathUtil.CheckFile(versionLookupPath))
            {
                Log.Message("Found game data lookup locally!");
                gameDataLookup = JsonConvert.DeserializeObject<Dictionary<int, string>>(File.ReadAllText(versionLookupPath));
            }
            else
            {
                Log.Message($"No local game data lookup found matching version {GAME_VERSION}!");
                Log.Message($"Trying to download game data lookup for version {GAME_VERSION}...");
                try
                {
                    string downloadURL = GAMEDATA_LOOKUP + GAME_VERSION + ".json";
                    string gameDataLookupString = webClient.DownloadString(downloadURL);
                    gameDataLookup = JsonConvert.DeserializeObject<Dictionary<int, string>>(gameDataLookupString);
                    Log.Message("Caching game data lookup to disk...");
                    File.WriteAllText(versionLookupPath, gameDataLookupString);
                }
                catch
                {
                    Log.Warn("Failed to download game data lookup for this version!");
                    Log.Message("Checking for previous version to fallback too...");
                    //This could be better, make it grab all the files in the lookup folder, order by name, pick latest / highest numbered
                    for (int version = GAME_VERSION; version > GAME_VERSION - 500; version--)
                    {
                        string localVerPath = Path.Combine(GameDataLookupPath, version + ".json");
                        if (PathUtil.CheckFile(localVerPath))
                        {
                            Log.Message("Found older lookup table!");
                            gameDataLookup = JsonConvert.DeserializeObject<Dictionary<int, string>>(File.ReadAllText(localVerPath));
                            Log.Warn($"USING OLD LOOKUP TABLE - Version {version}, SOME GAME DATA BLOCKS MAY BE MISSNAMED AND CUSTOM DATA BLOCKS MAY NOT LOAD!!");
                            return;
                        }
                    }
                    Log.Error("Failed to find any lookup tables! Cannot load custom mods and game data blocks will be named incorrectly!");
                }
            }
        }

        private static int GetGameVersion()
        {
            return CellBuildData.GetRevision();
        }
    }
}

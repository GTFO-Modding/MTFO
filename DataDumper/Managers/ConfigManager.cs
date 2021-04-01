using DataDumper.Utilities;
using DataDumper;
using Newtonsoft.Json;
using BepInEx.Configuration;
using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace DataDumper.Managers
{
    public static class ConfigManager
    {
        private const string
            CUSTOM_FOLDER = "Custom",
            GAMEDATA_LOOKUP = @"https://www.dakkhuza.com/projects/GTFO/gdlookup/";
        
        private static readonly WebClient webClient = new WebClient();
        

        // Anylitics Manager

        static ConfigManager()
        {            
            //Setup Config
            var CONFIG_PATH = Path.Combine(Paths.ConfigPath, "DataDumper.cfg");

            ConfigFile config = new ConfigFile(CONFIG_PATH, true);

            _enableHotReload = config.Bind(ConfigStrings.SECTION_DEV, ConfigStrings.SETTING_HOTRELOAD, false, ConfigStrings.SETTING_HOTRELOAD_DESC);
            _rundownFolder = config.Bind(ConfigStrings.SECTION_GENERAL, ConfigStrings.SETTING_RUNDOWNPACKAGE, "default", ConfigStrings.SETTING_RUNDOWNPACKAGE_DESC);
            _dumpFiles = config.Bind(ConfigStrings.SECTION_DEV, ConfigStrings.SETTING_DUMPFILE, false, ConfigStrings.SETTING_DUMPFILE_DESC);

            //Get game version
            GAME_VERSION = GetGameVersion();

            //Setup Paths
            GameDataLookupPath = PathUtil.MakeRelativeDirectory("_gamedatalookup");

            string path = _rundownFolder.Value;
            GameDataPath = path != "default" ? PathUtil.MakeRelativeDirectory(path) : PathUtil.MakeRelativeDirectory("GameData_" + GAME_VERSION);
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
                Log.Error(err.ToString());
                Log.Error("Failed to init custom content!\nIs the JSON in your PuzzleTypes.json file valid?");
            }

            //Setup GameData Lookup
            GetGameDataLookup();

            //Debug
            Log.Debug("HasCustomContent: " + HasCustomContent);
        }

        private static readonly ConfigEntry<bool> _enableHotReload;
        private static readonly ConfigEntry<string> _rundownFolder;
        private static readonly ConfigEntry<bool> _dumpFiles;

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

        private static void GetGameDataLookup()
        {
            string versionLookupPath = Path.Combine(GameDataLookupPath, GAME_VERSION + ".json");
            if (PathUtil.CheckFile(versionLookupPath))
            {
                Log.Message("Found game data lookup locally!");
                gameDataLookup = JsonConvert.DeserializeObject<Dictionary<int, string>>(File.ReadAllText(versionLookupPath));
                Log.Debug(gameDataLookup.Count);
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
            //This is really backwards because I was getting werid crashes when just trying to read the text
            //and couldn't be bothered to figure out why, probably some EOL shit or smthing
            string gameVersionPath = Path.Combine(Paths.GameRootPath, "revision.txt");
            string gameVersion = File.ReadAllText(gameVersionPath);
            int.TryParse(gameVersion, out int result);
            return result;
        }
    }
}

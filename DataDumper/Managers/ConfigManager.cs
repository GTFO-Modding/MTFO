using DataDumper.Utilities;
using MelonLoader;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace DataDumper.Managers
{
    public static class ConfigManager
    {
        private const string 
            SECTION = "Data Dumper",
            RUNDOWNPACKAGE = "RundownPackage",
            HOTRELOAD = "EnableHotReload",
            VERBOSE = "Verbose",
            DEBUG = "Debug",
            CUSTOM_FOLDER = "Custom",
            GAMEDATA_LOOKUP = @"https://www.dakkhuza.com/projects/GTFO/gdlookup/";

        private static readonly WebClient webClient = new WebClient();

        // Anylitics Manager

        static ConfigManager()
        {
            //Setup Config
            MelonPrefs.RegisterString(SECTION, RUNDOWNPACKAGE, "default"); 
            MelonPrefs.RegisterBool(SECTION, HOTRELOAD, false);
            MelonPrefs.RegisterBool(SECTION, VERBOSE, false);
            MelonPrefs.RegisterBool(SECTION, DEBUG, false);
            MelonPrefs.SaveConfig();

            //Setup Hotreload
            IsHotReloadEnabled = MelonPrefs.GetBool(SECTION, HOTRELOAD);

            //Get game version
            GAME_VERSION = GetGameVersion();

            //Setup Paths
            GameDataLookupPath = PathUtil.MakeRelativeDirectory("_gamedatalookup");

            string path = MelonPrefs.GetString(SECTION, RUNDOWNPACKAGE);
            GameDataPath = path != "default" ? PathUtil.MakeRelativeDirectory(path) : PathUtil.MakeRelativeDirectory("GameData_" + GAME_VERSION);

            //if (path != "default")
            //{
            //    GameDataPath = PathUtil.MakeRelativeDirectory(path);
            //} else
            //{
            //    GameDataPath = PathUtil.MakeRelativeDirectory("GameData_" + GAME_VERSION);
            //}

            CustomPath = Path.Combine(GameDataPath, CUSTOM_FOLDER);

            //Setup flags 
            HasCustomContent = Directory.Exists(CustomPath);
            IsVerbose = MelonPrefs.GetBool(SECTION, VERBOSE);
            IsDebug = MelonPrefs.GetBool(SECTION, DEBUG);

            //Setup folders
            if (!Directory.Exists(GameDataPath))
            {
                Directory.CreateDirectory(GameDataPath);
            }

            //Setup Managers
            try
            {
                CustomContent = new ContentManager();
            } catch
            {
                HasCustomContent = false;
                Log.Error("Failed to init custom content!\nIs the JSON in your PuzzleTypes.json file valid?");
            }

            //Setup GameData Lookup
            GetGameDataLookup();

            //Debug
            Log.Debug("HasCustomContent: " + HasCustomContent);
        }

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
        private static string GameDataLookupPath;

        //Flags
        public static bool HasCustomContent;
        public static bool IsVerbose;
        public static bool IsDebug;
        public static bool IsModded;

        //Dev Tools
        public static bool IsHotReloadEnabled;

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
                Log.Message("No local game data lookup found!");
                Log.Message("Downloading game data lookup...");
                try
                {
                    string downloadURL = GAMEDATA_LOOKUP + GAME_VERSION + ".json";
                    string gameDataLookupString = webClient.DownloadString(downloadURL);
                    gameDataLookup = JsonConvert.DeserializeObject<Dictionary<int, string>>(gameDataLookupString);
                    Log.Message("Writing game data lookup to disk...");
                    File.WriteAllText(versionLookupPath, gameDataLookupString);
                }
                catch
                {
                    Log.Error("Failed to download the gamedata lookup table! Cannot load custom mods and game data blocks will be named incorrectly!");
                }
                Log.Message("Done!");
            }
        }


        private static int GetGameVersion()
        {
            //This is really backwards because I was getting werid crashes when just trying to read the text
            //and couldn't be bothered to figure out why, probably some EOL shit or smthing
            string gameVersionPath = Path.Combine(Imports.GetGameDirectory(), "revision.txt");
            string gameVersion = File.ReadAllText(gameVersionPath);
            int.TryParse(gameVersion, out int result);
            return result;
        }
    }
}

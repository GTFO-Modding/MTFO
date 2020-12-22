using DataDumper.Utilities;
using MelonLoader;
using System.IO;

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
            CUSTOM_FOLDER = "Custom";

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
            GameDataPath = Path.Combine(MelonLoaderBase.UserDataPath, "GameData_" + GAME_VERSION);

            string path = MelonPrefs.GetString(SECTION, RUNDOWNPACKAGE);
            if (path != "default")
            {
                GameDataPath = Path.Combine(MelonLoaderBase.UserDataPath, path);
            }

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


            //Debug
            Log.Debug("HasCustomContent: " + HasCustomContent);
        }

        public static int GAME_VERSION;

        //Managers
        public static ContentManager CustomContent;

        //Strings
        public static string MenuText;

        //Paths
        public static string GameDataPath;
        public static string CustomPath;

        //Flags
        public static bool HasCustomContent;
        public static bool IsVerbose;
        public static bool IsDebug;

        //Dev Tools
        public static bool IsHotReloadEnabled;


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

using MelonLoader;
using System.IO;

namespace DataDumper.Managers
{
    public static class ConfigManager
    {
        public static readonly string SECTION = "Data Dumper";
        static ConfigManager()
        {
            //Setup Config
            MelonPrefs.RegisterString(SECTION, "RundownPackage", "default");
            MelonPrefs.RegisterBool(SECTION, "EnableHotReload", false);
            MelonPrefs.RegisterBool(SECTION, "Verbose", false);
            MelonPrefs.SaveConfig();

            //Setup Hotreload
            IsHotReloadEnabled = MelonPrefs.GetBool(SECTION, "EnableHotReload");


            //Get game version
            //This is really backwards because I was getting werid crashes when just trying to read the text
            //and couldn't be bothered to figure out why

            string gameVersionPath = Path.Combine(Imports.GetGameDirectory(), "revision.txt");
            string gameVersion = File.ReadAllText(gameVersionPath);
            int.TryParse(gameVersion, out int result);
            GAME_VERSION = result;

            //Setup Paths
            GameDataPath = Path.Combine(MelonLoaderBase.UserDataPath, "GameData_" + GAME_VERSION);

            string path = MelonPrefs.GetString(SECTION, "RundownPackage");
            if (path != "default")
            {
                GameDataPath = Path.Combine(MelonLoaderBase.UserDataPath, path);
            }
            CustomPath = Path.Combine(GameDataPath, "Custom");

            //Setup flags 
            HasCustomContent = Directory.Exists(CustomPath);
            IsVerbose = MelonPrefs.GetBool(SECTION, "Verbose");

            //Setup folders
            if (!Directory.Exists(GameDataPath))
            {
                Directory.CreateDirectory(GameDataPath);
            }
        }

        public static int GAME_VERSION;

        //Strings
        public static string MenuText;

        //Paths
        public static string GameDataPath;
        public static string CustomPath;

        //Flags
        public static bool HasCustomContent;
        public static bool IsVerbose;

        //Dev Tools
        public static bool IsHotReloadEnabled;
    }
}

using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDumper
{
    public static class ConfigManager
    {
        static ConfigManager()
        {
            //Setup Config
            MelonPrefs.RegisterString("Data Dumper", "RundownPackage", "default");
            MelonPrefs.RegisterBool("Data Dumper", "EnableHotReload", false);
            MelonPrefs.SaveConfig();

            //Setup Hotreload
            IsHotReloadEnabled = MelonPrefs.GetBool("Data Dumper", "EnableHotReload");


            //Get game version
            //This is really backwards because I was getting werid crashes when just trying to read the text
            //and couldn't be bothered to figure out why

            string gameVersionPath = Path.Combine(Imports.GetGameDirectory(), "revision.txt");
            string gameVersion = File.ReadAllText(gameVersionPath);
            int.TryParse(gameVersion, out int result);
            GAME_VERSION = result;

            //Setup game data path
            GameDataPath = Path.Combine(MelonLoaderBase.UserDataPath, "GameData_" + GAME_VERSION);

            string path = MelonPrefs.GetString("Data Dumper", "RundownPackage");
            if (path != "default")
            {
                GameDataPath = Path.Combine(MelonLoaderBase.UserDataPath, path);
            }

            //Setup folders
            if (!Directory.Exists(GameDataPath))
            {
                Directory.CreateDirectory(GameDataPath);
            }
        }

        public static int GAME_VERSION;
        public static string GameDataPath;
        public static bool IsHotReloadEnabled;
    }
}

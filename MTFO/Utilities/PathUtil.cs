using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MTFO.Managers;
using BepInEx;

namespace MTFO.Utilities
{
    public enum BaseDirectory
    {
        BepInEx,
        Plugins,
        GameData,
    }

    public static class PathUtil
    {
        public static string Prepare(BaseDirectory baseDir, params string[] subDirs)
        {
            var baseDirPath = baseDir switch
            {
                BaseDirectory.BepInEx => Paths.BepInExRootPath,
                BaseDirectory.Plugins => Paths.PluginPath,
                BaseDirectory.GameData => Path.Combine(Paths.BepInExRootPath, "GameData"),
                _ => throw new ArgumentOutOfRangeException(nameof(baseDir), $"{baseDir} is not a valid value for Argument: {nameof(baseDir)}")
            };

            string path = (subDirs == null || subDirs.Length <= 0) ? baseDirPath : Path.Combine(baseDirPath, Path.Combine(subDirs));
            Directory.CreateDirectory(path);
            return path;
        }

        public static bool CheckCustomFile(string file, out string CombinedPath)
        {
            CombinedPath = Path.Combine(ConfigManager.CustomPath, file);
            if (File.Exists(CombinedPath)) return true;
            return false;
        }

        public static bool CheckFile(string pathToFile)
        {
            if (File.Exists(pathToFile)) return true;
            return false;
        }

        [Obsolete]
        public static string MakeRelativeDirectory(string path, bool createPath = true)
        {
            string OldPath = Path.Combine(Path.Combine(Paths.ConfigPath, "Rundowns"), path);
            if (createPath && !Directory.Exists(OldPath))
            {
                Directory.CreateDirectory(OldPath);
            }
            return OldPath;
        }

        public static string MakeRelativeDirectory(string path, string folder)
        {
            string dir = Path.Combine(path, folder);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return dir;
        }

        public static void PrepareEmptyDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            Directory.CreateDirectory(path);
        }

        public static Stream OpenUtf8Stream(string filePath)
        {
            return new StreamReader(filePath, Encoding.UTF8).BaseStream;
        }
    }
}

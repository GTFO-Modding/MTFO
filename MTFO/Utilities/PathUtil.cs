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
    public static class PathUtil
    {
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

        public static string MakeRelativeDirectory(string path)
        {
            string CombinedPath = Path.Combine(Path.Combine(Paths.ConfigPath, "Rundowns"), path);
            if (!Directory.Exists(CombinedPath))
            {
                Directory.CreateDirectory(CombinedPath);
            }
            return CombinedPath;
        }
    }
}

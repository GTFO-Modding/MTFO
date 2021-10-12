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
        public static bool CheckCustomFile(string file, out IDirectoryFile CustomFile)
        {
            CustomFile = ConfigManager.CustomPath
                .GetFile(file);
            return CustomFile.Exists();
        }

        public static bool CheckFile(string pathToFile)
        {
            if (File.Exists(pathToFile)) return true;
            return false;
        }

        [Obsolete]
        public static string MakeRelativeDirectory(string path)
        {
            string OldPath = Path.Combine(Path.Combine(Paths.ConfigPath, "Rundowns"), path);
            if (!Directory.Exists(OldPath))
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
    }
}

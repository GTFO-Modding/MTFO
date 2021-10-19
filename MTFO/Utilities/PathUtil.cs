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
    /// <summary>
    /// Some utilities for parhs
    /// </summary>
    public static class PathUtil
    {
        /// <summary>
        /// Returns whether or not the file relative to the custom path exists, and outputs
        /// the resolved path to the custom file.
        /// </summary>
        /// <param name="file">The name of the file to check</param>
        /// <param name="CombinedPath">The outputted combined path</param>
        /// <returns>Whether or not the custom file exists</returns>
        public static bool CheckCustomFile(string file, out string CombinedPath)
        {
            CombinedPath = Path.Combine(ConfigManager.CustomPath, file);
            if (File.Exists(CombinedPath)) return true;
            return false;
        }

        /// <summary>
        /// Returns whether or not the file at the given path exists.
        /// </summary>
        /// <param name="pathToFile">The path to the file.</param>
        /// <returns>True if it exists, otherwise false.</returns>
        public static bool CheckFile(string pathToFile)
        {
            if (File.Exists(pathToFile)) return true;
            return false;
        }

        /// <summary>
        /// Creates a directory specified by path in the /Rundowns folder
        /// if it doesn't already exist.
        /// </summary>
        /// <param name="path">The folder</param>
        /// <returns>The path to the folder</returns>
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

        /// <summary>
        /// Combines the given path and folder into a directory path, and creates
        /// that directory if needed
        /// </summary>
        /// <param name="path">The path</param>
        /// <param name="folder">The folder</param>
        /// <returns>The path to the folder</returns>
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

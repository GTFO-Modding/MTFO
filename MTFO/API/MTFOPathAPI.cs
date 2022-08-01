using MTFO.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTFO.API
{
    public static class MTFOPathAPI
    {
        public static string RundownPath => ConfigManager.GameDataPath;
        public static bool HasRundownPath => ConfigManager.HasGameDataPath;
        public static string CustomPath => ConfigManager.CustomPath;
        public static bool HasCustomPath => ConfigManager.HasCustomContent;
    }
}

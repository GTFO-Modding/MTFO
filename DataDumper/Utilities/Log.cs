using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using DataDumper.Managers;

namespace DataDumper.Utilities
{
    public static class Log
    {
        public static void Verbose(string msg)
        {
            if (!ConfigManager.IsVerbose) return;
            MelonLogger.Log("VERBOSE: " + msg);
        }

        public static void Debug(string msg)
        {
            if (!ConfigManager.IsDebug) return;
            MelonLogger.Log("DEBUG: " + msg);
        }

        public static void Error(string msg)
        {
            MelonLogger.LogError(msg);
        }
    }
}

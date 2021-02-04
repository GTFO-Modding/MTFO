using BepInEx.Logging;
using MTFO.Managers;

namespace MTFO.Utilities
{
    public static class Log
    {
        private static readonly ManualLogSource logger;
        static Log()
        {
            logger = new ManualLogSource(MTFO.MODNAME);
            Logger.Sources.Add(logger);
        }
        public static void Verbose(object msg)
        {
            if (!ConfigManager.IsVerbose) return;
            logger.LogInfo(msg);
        }

        public static void Debug(object msg)
        {
            logger.LogDebug(msg);
        }

        public static void Message(object msg)
        {
            logger.LogMessage(msg);
        }

        public static void Error(object msg)
        {
            logger.LogError(msg);
        }

        public static void Warn(object msg)
        {
            logger.LogWarning(msg);
        }
    }
}

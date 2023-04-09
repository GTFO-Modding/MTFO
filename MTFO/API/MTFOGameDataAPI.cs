using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTFO.API
{
    public delegate void GameDataContentLoadEvent(string datablockName, string jsonContent, in List<string> jsonItemsToInject);
    public delegate void GameDataContentLoadedDelegate(string datablockName, string jsonContent);

    public static class MTFOGameDataAPI
    {
        public static event GameDataContentLoadEvent OnGameDataContentLoad;
        public static event GameDataContentLoadedDelegate OnGameDataContentLoaded;

        internal static void Invoke_OnGameDataContentLoad(string datablockName, string jsonContent, in List<string> jsonItemsToInject)
        {
            OnGameDataContentLoad?.Invoke(datablockName, jsonContent, in jsonItemsToInject);
        }

        internal static void Invoke_OnGameDataContentLoaded(string datablockName, string jsonContent)
        {
            OnGameDataContentLoaded?.Invoke(datablockName, jsonContent);
        }
    }
}

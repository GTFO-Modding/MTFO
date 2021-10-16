using System;
using GameData;
using MTFO.Utilities;

namespace MTFO.HotReload
{
    class HotGameDataManager : IHotManager
    {
        public void Reload(int id)
        {
            GameDataInit.ReInitialize(); // refresh game data
            Log.Message("Reinitialized GameData");
        }
    }
}

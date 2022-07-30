using MTFO.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTFO.API
{
    public static class MTFOHotReloadAPI
    {
        public static bool HotReloadEnabled => ConfigManager.IsHotReloadEnabled;
        public static event Action OnHotReload;

        internal static void HotReloaded()
        {
            OnHotReload?.Invoke();
        }
    }
}

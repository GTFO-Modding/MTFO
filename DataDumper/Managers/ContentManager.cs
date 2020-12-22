using System;
using System.Collections.Generic;
using MelonLoader;
using System.IO;
using DataDumper.Custom.Scans;
using Newtonsoft.Json;
using DataDumper.Utilities;

namespace DataDumper.Managers
{
    public class ContentManager
    {
        private readonly Dictionary<string, Action<string>> Handlers;
        public ScanHolder ScanHolder;

        public ContentManager()
        {
            Handlers = new Dictionary<string, Action<string>>
            {
                { "welcome.txt", SetupWelcomeText },
                { "puzzletypes.json", SetupChainedPuzzles }
            };
            Init();
        }

        private void Init()
        {
            if (!ConfigManager.HasCustomContent) return;

            foreach(string key in Handlers.Keys)
            {
                if (PathUtil.CheckCustomFile(key, out string path))
                {
                    Handlers.TryGetValue(key, out Action<string> value);
                    value?.Invoke(path);
                    MelonLogger.Log(path);
                }
            }
        }

        public void SetupWelcomeText(string path)
        {
            Log.Debug("Welcome text found");
            ConfigManager.MenuText = File.ReadAllText(path);
        }

        public void SetupChainedPuzzles(string path)
        {
            Log.Debug("Custom puzzles found");
            ScanHolder = JsonConvert.DeserializeObject<ScanHolder>(File.ReadAllText(path));
        }
    }
}

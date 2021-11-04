using System;
using System.Collections.Generic;
using System.IO;
using MTFO.Utilities;
using MTFO.Custom;

namespace MTFO.Managers
{
    public class ContentManager
    {
        private readonly JsonSerializer json = new();
        private readonly Dictionary<string, Action<string>> Handlers;
        public ScanHolder ScanHolder;
        public GlowstickHolder GlowstickHolder;
        public TierNames TierNames;

        public ContentManager()
        {
            Handlers = new Dictionary<string, Action<string>>
            {
                { "welcome.txt", SetupWelcomeText },
                { "puzzletypes.json", SetupChainedPuzzles },
                { "glowsticks.json", SetupGlowsticks },
                { "tiernames.json", SetupTierNames }
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
                    try
                    {
                        value?.Invoke(path);
                    } catch (Exception err)
                    {
                        Log.Error(err);
                    }
                    
                    Log.Debug(path);
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
            ScanHolder = json.Deserialize<ScanHolder>(File.ReadAllText(path));
        }

        public void SetupGlowsticks(string path)
        {
            Log.Debug("Custom glowsticks found");
            GlowstickHolder = json.Deserialize<GlowstickHolder>(File.ReadAllText(path));
            GlowstickHolder.Setup();
        }

        public void SetupTierNames(string path)
        {
            TierNames = json.Deserialize<TierNames>(File.ReadAllText(path));
        }
    }
}

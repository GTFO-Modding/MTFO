using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using DataDumper.Utilities;
using DataDumper.Custom;

namespace DataDumper.Managers
{
    public class ContentManager
    {
        private readonly Dictionary<string, Action<string>> Handlers;
        public ScanHolder ScanHolder;
        public GlowstickHolder GlowstickHolder;
        public TierNames TierNames;
        public FogRepellerConfig FogRepellerConfig;

        public ContentManager()
        {
            Handlers = new Dictionary<string, Action<string>>
            {
                { "welcome.txt", SetupWelcomeText },
                { "puzzletypes.json", SetupChainedPuzzles },
                { "glowsticks.json", SetupGlowsticks },
                { "tiernames.json", SetupTierNames },
                { "fogrep_config.json", SetupFogRep }
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
            ScanHolder = JsonConvert.DeserializeObject<ScanHolder>(File.ReadAllText(path));
        }

        public void SetupGlowsticks(string path)
        {
            Log.Debug("Custom glowsticks found");
            GlowstickHolder = JsonConvert.DeserializeObject<GlowstickHolder>(File.ReadAllText(path));
            GlowstickHolder.Setup();
        }

        public void SetupTierNames(string path)
        {
            TierNames = JsonConvert.DeserializeObject<TierNames>(File.ReadAllText(path));
        }

        public void SetupFogRep(string path)
        {
            FogRepellerConfig = JsonConvert.DeserializeObject<FogRepellerConfig>(File.ReadAllText(path));
        }
    }
}

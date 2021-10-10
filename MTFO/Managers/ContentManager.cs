using System;
using System.Collections.Generic;
using System.IO;
using MTFO.Utilities;
using MTFO.Custom;
using System.Text.Json;

namespace MTFO.Managers
{
    public class ContentManager
    {
        internal static readonly JsonSerializerOptions s_SerializerOptions = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            PropertyNameCaseInsensitive = true
        };
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
            ScanHolder = JsonSerializer.Deserialize<ScanHolder>(File.ReadAllText(path), s_SerializerOptions);
        }

        public void SetupGlowsticks(string path)
        {
            Log.Debug("Custom glowsticks found");
            GlowstickHolder = JsonSerializer.Deserialize<GlowstickHolder>(File.ReadAllText(path), s_SerializerOptions);
            GlowstickHolder.Setup();
        }

        public void SetupTierNames(string path)
        {
            TierNames = JsonSerializer.Deserialize<TierNames>(File.ReadAllText(path), s_SerializerOptions);
        }
    }
}

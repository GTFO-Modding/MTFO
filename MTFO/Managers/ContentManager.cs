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
        private readonly Dictionary<string, Action<IDirectoryFile>> Handlers;
        public ScanHolder ScanHolder;
        public GlowstickHolder GlowstickHolder;
        public TierNames TierNames;

        public ContentManager()
        {
            Handlers = new Dictionary<string, Action<IDirectoryFile>>
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
                if (PathUtil.CheckCustomFile(key, out IDirectoryFile customFile))
                {
                    Handlers.TryGetValue(key, out Action<IDirectoryFile> value);
                    try
                    {
                        value?.Invoke(customFile);
                    } catch (Exception err)
                    {
                        Log.Error(err);
                    }
                    
                    Log.Debug(customFile);
                }
            }
        }

        public void SetupWelcomeText(IDirectoryFile path)
        {
            Log.Debug("Welcome text found");
            ConfigManager.MenuText = path.ReadAllText();
        }

        public void SetupChainedPuzzles(IDirectoryFile path)
        {
            Log.Debug("Custom puzzles found");
            ScanHolder = JsonSerializer.Deserialize<ScanHolder>(path.ReadAllText(), s_SerializerOptions);
        }

        public void SetupGlowsticks(IDirectoryFile path)
        {
            Log.Debug("Custom glowsticks found");
            GlowstickHolder = JsonSerializer.Deserialize<GlowstickHolder>(path.ReadAllText(), s_SerializerOptions);
            GlowstickHolder.Setup();
        }

        public void SetupTierNames(IDirectoryFile path)
        {
            TierNames = JsonSerializer.Deserialize<TierNames>(path.ReadAllText(), s_SerializerOptions);
        }
    }
}

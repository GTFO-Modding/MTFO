using System;
using System.Collections.Generic;
using System.IO;
using MTFO.Utilities;
using MTFO.Custom;

namespace MTFO.Managers
{
    /// <summary>
    /// Handles custom content
    /// </summary>
    public class ContentManager
    {
        private readonly JsonSerializer json = new();
        private readonly Dictionary<string, Action<string>> Handlers;
        /// <summary>
        /// Custom Scans
        /// </summary>
        public ScanHolder ScanHolder;
        /// <summary>
        /// Custom Glowsticks
        /// </summary>
        public GlowstickHolder GlowstickHolder;

        /// <summary>
        /// Custom tier names
        /// </summary>
        public TierNames TierNames;

        /// <summary>
        /// Creates a new instance of the content manager
        /// </summary>
        public ContentManager()
        {
            Handlers = new Dictionary<string, Action<string>>
            {
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

        /// <summary>
        /// Sets up the welcome text
        /// </summary>
        /// <param name="path">The path to the welcome text file</param>
        public void SetupWelcomeText(string path)
        {
            Log.Debug("Welcome text found");
            ConfigManager.MenuText = File.ReadAllText(path);
        }

        /// <summary>
        /// Sets up the custom chained puzzles
        /// </summary>
        /// <param name="path">The path of the file for chained puzzles</param>
        public void SetupChainedPuzzles(string path)
        {
            Log.Debug("Custom puzzles found");
            ScanHolder = json.Deserialize<ScanHolder>(File.ReadAllText(path));
        }

        /// <summary>
        /// Sets up the custom glow sticks
        /// </summary>
        /// <param name="path">The path of the file for the custom glowsticks</param>
        public void SetupGlowsticks(string path)
        {
            Log.Debug("Custom glowsticks found");
            GlowstickHolder = json.Deserialize<GlowstickHolder>(File.ReadAllText(path));
            GlowstickHolder.Setup();
        }

        /// <summary>
        /// Sets up the custom tier names.
        /// </summary>
        /// <param name="path">The path to the custom tier names</param>
        public void SetupTierNames(string path)
        {
            TierNames = json.Deserialize<TierNames>(File.ReadAllText(path));
        }
    }
}

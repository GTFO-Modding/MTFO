using System;
using System.Collections.Generic;
using MelonLoader;
using System.IO;

namespace DataDumper.Managers
{
    public class CustomManager
    {
        private readonly Dictionary<string, Action<string>> Handlers;

        public CustomManager()
        {
            Handlers = new Dictionary<string, Action<string>>
            {
                { "welcometext", SetupWelcomeText },
                { "chainedpuzzle", SetupChainedPuzzles }
            };
            Init();
        }

        private void Init()
        {
            foreach(string key in Handlers.Keys)
            {
                if (PathUtil.CheckPath(key, out string path))
                {
                    Handlers.TryGetValue(key, out Action<string> value);
                    value?.Invoke(path);
                }
            }
        }

        private void SetupWelcomeText(string path)
        {
            string text = File.ReadAllText(path);
            ConfigManager.MenuText = text;
        }

        private void SetupChainedPuzzles(string path)
        {
            MelonLogger.Log(path);
        }
    }
}

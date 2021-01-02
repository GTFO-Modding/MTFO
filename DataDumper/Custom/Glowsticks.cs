using System.Collections.Generic;
using UnityEngine;
using DataDumper.Utilities;
using Newtonsoft.Json;

namespace DataDumper.Custom
{
    public class GlowstickHolder
    {
        public void Setup()
        {
            GlowstickLookup = new Dictionary<string, CustomGlowstick>();
            foreach (var glowstick in Glowsticks)
            {
                if (GlowstickLookup.TryGetValue(glowstick.Name, out _))
                {
                    Log.Warn($"Custom glowstick with name {glowstick.Name} already exists in the lookup! Skipping...");
                    continue;
                }
                CustomGlowstick newGlowstick = new CustomGlowstick()
                {
                    Color = new Color(glowstick.r, glowstick.g, glowstick.b, glowstick.a == 0f ? 1 : glowstick.a),
                    Range = glowstick.Range == 0 ? 15 : glowstick.Range
                };
                GlowstickLookup.Add(glowstick.Name, newGlowstick);
            }
        }

        public List<GlowstickConfig> Glowsticks;

        [JsonIgnore]
        public Dictionary<string, CustomGlowstick> GlowstickLookup;
    }

    public struct CustomGlowstick
    {
        public Color Color;
        public float Range;
    }

    public struct GlowstickConfig
    {
        public string Name;
        public float Range;
        public float r;
        public float g;
        public float b;
        public float a;
    }
}

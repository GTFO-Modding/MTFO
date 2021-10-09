using System.Collections.Generic;
using UnityEngine;
using MTFO.Utilities;
using System.Text.Json.Serialization;

namespace MTFO.Custom
{
    public class GlowstickHolder
    {
        public void Setup()
        {
            GlowstickLookup = new Dictionary<string, CustomGlowstick>();
            foreach (var glowstick in Glowsticks)
            {
                Log.Verbose(glowstick);
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

        public List<GlowstickConfig> Glowsticks { get; set; }

        [JsonIgnore]
        public Dictionary<string, CustomGlowstick> GlowstickLookup;
    }

    public struct CustomGlowstick
    {
        public Color Color { get; set; }
        public float Range { get; set; }
    }

    public struct GlowstickConfig
    {
        public string Name { get; set; }
        public float Range { get; set; }
        public float r { get; set; }
        public float g { get; set; }
        public float b { get; set; }
        public float a { get; set; }
        public override string ToString()
        {
            return $"{Name},{Range},{r},{g},{b},{a}";
        }
    }
}

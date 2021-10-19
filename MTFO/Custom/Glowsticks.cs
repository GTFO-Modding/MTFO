using System.Collections.Generic;
using UnityEngine;
using MTFO.Utilities;
using System.Text.Json.Serialization;

namespace MTFO.Custom
{
    /// <summary>
    /// Custom Glowsticks
    /// </summary>
    public class GlowstickHolder
    {
        /// <summary>
        /// Sets up the glowstick handler
        /// </summary>
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

        /// <summary>
        /// List of all glowsticks
        /// </summary>
        public List<GlowstickConfig> Glowsticks { get; set; }

        /// <summary>
        /// Glowstick Lookup
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, CustomGlowstick> GlowstickLookup;
    }

    /// <summary>
    /// Custom glow stick
    /// </summary>
    public struct CustomGlowstick
    {
        /// <summary>
        /// The color of tjhe glowstick
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// The range of the glowstick
        /// </summary>
        public float Range { get; set; }
    }

    /// <summary>
    /// The config for a glowstick
    /// </summary>
    public struct GlowstickConfig
    {
        /// <summary>
        /// The glowstick name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The glowstick range
        /// </summary>
        public float Range { get; set; }

        /// <summary>
        /// The red color component
        /// </summary>
        public float r { get; set; }
        /// <summary>
        /// The green color component
        /// </summary>
        public float g { get; set; }
        /// <summary>
        /// The blue color component
        /// </summary>
        public float b { get; set; }
        /// <summary>
        /// The alpha color component
        /// </summary>
        public float a { get; set; }


        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Name},{Range},{r},{g},{b},{a}";
        }
    }
}

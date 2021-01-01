using Harmony;
using DataDumper.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataDumper.Custom;

namespace DataDumper.Patches
{
    [HarmonyPatch(typeof(FogRepellerInstance), "Start")]
    class Patch_FogRepellerInstance
    {
        public static void Postfix(FogRepellerInstance __instance)
        {
            FogInstanceConfig config = ConfigManager.CustomContent?.FogRepellerConfig?.FogInstanceConfig;
            if (config == null) return;
            __instance.m_repellerSphere.Range = config.Range;
            __instance.m_repellerSphere.InfiniteDuration = config.InfiniteDuration;
            __instance.m_repellerSphere.ShrinkDuration = config.ShrinkDuration;
            __instance.m_repellerSphere.GrowDuration = config.GrowDuration;
            __instance.m_repellerSphere.LifeDuration = config.LifeInSeconds;
            __instance.m_repellerSphere.Density = config.Density;
        }
    }

    [HarmonyPatch(typeof(FogRepellerInstance), "Setup")]
    class Patch_HeavyFogRepeller
    {
        public static void Postfix(FogRepellerInstance __instance)
        { 
            HeavyFogTurbineConfig config = ConfigManager.CustomContent?.FogRepellerConfig?.HeavyFogTurbineConfig;
            if (config == null) return;
            __instance.m_repellerSphere.InfiniteDuration = config.InfiniteDuration;
            __instance.m_repellerSphere.Range = config.Range;
            __instance.m_repellerSphere.ShrinkDuration = config.ShrinkDuration;
            __instance.m_repellerSphere.GrowDuration = config.GrowDuration;
            __instance.m_repellerSphere.Density = config.Density;
            if (config.LifeInSeconds != 0)
            {
                __instance.m_repellerSphere.LifeDuration = config.LifeInSeconds;
            }
        }
    }
}

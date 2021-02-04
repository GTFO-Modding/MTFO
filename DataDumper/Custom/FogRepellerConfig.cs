using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTFO.Custom
{
    public class FogRepellerConfig
    {
        public FogInstanceConfig FogInstanceConfig;
        public HeavyFogTurbineConfig HeavyFogTurbineConfig;
    }

    public class FogInstanceConfig
    {
        public float Range = 11f;
        public bool InfiniteDuration = false;
        public float ShrinkDuration = 10f;
        public float GrowDuration = 8f;
        public float LifeInSeconds = 60f;
        public float Density = -7;
    }

    public class HeavyFogTurbineConfig
    {
        public bool InfiniteDuration = true;
        public float Range = 7f;
        public float ShrinkDuration = 20f;
        public float GrowDuration = 12f;
        public float Density = -7;
        public float LifeInSeconds;
    }

}

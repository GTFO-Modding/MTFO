using ChainedPuzzles;
using System.Collections.Generic;

namespace MTFO.Custom
{
    public class ScanHolder
    {
        public List<CustomBioScan> Scans { get; set; }
        public List<CustomClusterScan> Clusters { get; set; }
    }

    public struct CustomBioScan
    {
        public uint BaseScan { get; set; }
        public uint PersistentID { get; set; }
        public PlayerRequirement PlayerRequirement { get; set; }
        public float ScanRadius { get; set; }
        public float[] PlayersInScanMulti { get; set; }
        public float ReduceSpeed { get; set; }
        public bool ReduceWhenNoPlayer { get; set; }
        public float RevealSpeed { get; set; }
        public BioScanGx BioScanGraphics { get; set; }


        public struct BioScanGx
        {
            public string ScanText { get; set; }
            public float Radius { get; set; }
            public BioScanColorByMode[] colorModeColor { get; set; }
        }

        public class BioScanColorByMode
        {
            // fix dak's skill issue :)
#pragma warning disable IDE1006 // Naming Styles
            public eChainedPuzzleGraphicsColorMode mode { get; set; }
            public float r { get; set; }
            public float g { get; set; }
            public float b { get; set; }
            public float a { get; set; }
#pragma warning restore IDE1006 // Naming Styles
        }
    }

    public struct CustomClusterScan
    {
        public uint BaseCluster { get; set; }
        public uint PersistentID { get; set; }
        public int ClusterCount { get; set; }
        public uint BioscanID { get; set; }
        public float DistanceBetweenScans { get; set; }
        public float RevealSpeed { get; set; }
        public bool RevealWithHoloPath { get; set; }
    }
}

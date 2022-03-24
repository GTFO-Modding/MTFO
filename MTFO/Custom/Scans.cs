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
            public BioScanColorByMode[] ColorModeColor { get; set; }
        }

        public class BioScanColorByMode
        {
            public eChainedPuzzleGraphicsColorMode Mode { get; set; }
            public float R { get; set; }
            public float G { get; set; }
            public float B { get; set; }
            public float A { get; set; }
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

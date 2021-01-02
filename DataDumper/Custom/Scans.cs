using ChainedPuzzles;
using System.Collections.Generic;

namespace DataDumper.Custom
{
    public class ScanHolder
    {
        public List<CustomBioScan> Scans;
        public List<CustomClusterScan> Clusters;
    }

    public struct CustomBioScan
    {
        public uint BaseScan;
        public uint PersistentID;
        public bool RequireAll;
        public float ScanRadius;
        public float[] PlayersInScanMulti;
        public float ReduceSpeed;
        public bool ReduceWhenNoPlayer;
        public float ScanSpeedDecline;
        public BioScanGx BioScanGraphics;


        public struct BioScanGx
        {
            public float Radius;
            public BioScanColorByMode[] colorModeColor;
        }

        public class BioScanColorByMode
        {
            public eChainedPuzzleGraphicsColorMode mode;
            public float r;
            public float g;
            public float b;
            public float a = 1;
        }
    }

    public struct CustomClusterScan
    {
        public uint BaseCluster;
        public uint PersistentID;
        public int ClusterCount;
        public uint BioscanID;
        public float DistanceBetweenScans;
        public bool RevealWithHoloPath;
    }
}

using ChainedPuzzles;
using MTFO.Utilities;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTFO.Custom
{
    public class ScanHolder
    {
        public List<CustomBioScan> Scans { get; set; }
        public List<CustomClusterScan> Clusters { get; set; }
    }

    public enum RevealMode
    {
        ScaleByDistance,
        ConstantTime,
        Instant
    }

    public struct CustomBioScan : IRevealableScanConfig
    {
        public uint BaseScan { get; set; }
        public uint PersistentID { get; set; }
        public PlayerRequirement PlayerRequirement { get; set; }
        public float ScanRadius { get; set; }
        public float[] PlayersInScanMulti { get; set; }
        public float ReduceSpeed { get; set; }
        public bool ReduceWhenNoPlayer { get; set; }
        public float RevealTime { get; set; }
        public RevealMode RevealMode { get; set; }
        public BioScanGx BioScanGraphics { get; set; }


        public struct BioScanGx
        {
            public bool HideScanText { get; set; }
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

    public struct CustomClusterScan : IRevealableScanConfig
    {
        public uint BaseCluster { get; set; }
        public uint PersistentID { get; set; }
        public int ClusterCount { get; set; }
        public uint BioscanID { get; set; }
        public float DistanceBetweenScans { get; set; }
        public float RevealTime { get; set; }
        public RevealMode RevealMode { get; set; }
        public bool RevealWithHoloPath { get; set; }
    }

    interface IRevealableScanConfig
    {
        uint PersistentID { get; }
        float RevealTime { get; }
        RevealMode RevealMode { get; }
            
    }

    static class IRevealibleScanConfigExtensions
    {
        public static void ApplySplineRevealSpeed(this IRevealableScanConfig scan, CP_Holopath_Spline spline)
        {
            float revealTime = scan.RevealTime;

            switch (scan.RevealMode)
            {
                case RevealMode.ConstantTime:
                    if (revealTime <= 0f)
                    {
                        Log.Warn($"Attempted to set a custom scan with persistent id '{scan.PersistentID}' to reveal in less than or equal to 0 seconds. This is not supported. Instead, use RevealMode \"{RevealMode.Instant}\" or integer value {(int)RevealMode.Instant}");
                        return;
                    }
                    // ensure constant distance.
                    spline.m_splineLength = 1f;
                    spline.m_revealSpeed = revealTime;
                    break;
                case RevealMode.ScaleByDistance:
                    if (revealTime < 0f)
                    {
                        Log.Warn($"Attempted to set a custom scan with persistent id '{scan.PersistentID}' to reveal in less than 0 seconds. This is not supported.");
                        return;
                    }
                    else if (revealTime == 0f)
                        return;

                    spline.m_revealSpeed = revealTime;
                    break;
                case RevealMode.Instant:
                    spline.m_revealSpeed = -1f;
                    break;
            }
        }
    }
}

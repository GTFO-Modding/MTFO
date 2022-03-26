using ChainedPuzzles;
using System.Collections.Generic;

namespace MTFO.Custom
{
    /// <summary>
    /// Holds all defined scans and clusters
    /// </summary>
    public class ScanHolder
    {
        /// <summary>
        /// Bio scans
        /// </summary>
        public List<CustomBioScan> Scans { get; set; }

        /// <summary>
        /// Cluster scans
        /// </summary>
        public List<CustomClusterScan> Clusters { get; set; }
    }

    /// <summary>
    /// A custom bio scan, or individual circle 
    /// </summary>
    public struct CustomBioScan
    {
        /// <summary>
        /// The base scan from ChainedPuzzleTypesDataBlock
        /// </summary>
        public uint BaseScan { get; set; }
        /// <summary>
        /// The persistent ID of this bio scan
        /// </summary>
        public uint PersistentID { get; set; }
        /// <summary>
        /// Whether or not all members are required to progress the scan
        /// </summary>
        public bool RequireAll { get; set; }
        /// <summary>
        /// The radius at which players will be counted in the scan
        /// </summary>
        public float ScanRadius { get; set; }
        public int PlayerRequirement { get; set; }
        /// <summary>
        /// Multipliers for scan speed for players in scan
        /// </summary>
        public float[] PlayersInScanMulti { get; set; }
        /// <summary>
        /// Speed at which the scan is completed
        /// </summary>
        public float ReduceSpeed { get; set; }
        /// <summary>
        /// Whether or not the scan should behave like an S1 scan and go down in progress when no
        /// player stands in it.
        /// </summary>
        public bool ReduceWhenNoPlayer { get; set; }
        /// <summary>
        /// The speed at which the scan goes down.
        /// </summary>
        public float ScanSpeedDecline { get; set; }

        /// <summary>
        /// The graphics for the bioscan
        /// </summary>
        public BioScanGx BioScanGraphics { get; set; }


        /// <summary>
        /// Bio scan graphics
        /// </summary>
        public struct BioScanGx
        {
            /// <summary>
            /// Custom scan text
            /// </summary>
            public string ScanText { get; set; }

            /// <summary>
            /// Whether or not to show the scan percent
            /// </summary>
            public bool ShowScanPercent { get; set; }
            /// <summary>
            /// The radius of the scan
            /// </summary>
            public float Radius { get; set; }

            /// <summary>
            /// The color modes of the scan
            /// </summary>
            public BioScanColorByMode[] colorModeColor { get; set; }
        }

        /// <summary>
        /// A bio-scan color mode
        /// </summary>
        public class BioScanColorByMode
        {
            /// <summary>
            /// The mode this color is for
            /// </summary>
            public eChainedPuzzleGraphicsColorMode mode { get; set; }

            /// <summary>
            /// Red color component
            /// </summary>
            public float r { get; set; }
            /// <summary>
            /// Green color component
            /// </summary>
            public float g { get; set; }
            /// <summary>
            /// Blue color component
            /// </summary>
            public float b { get; set; }
            /// <summary>
            /// Alpha color component
            /// </summary>
            public float a { get; set; }
        }
    }

    /// <summary>
    /// A custom cluster scan
    /// </summary>
    public struct CustomClusterScan
    {
        /// <summary>
        /// The base cluster from ChainedPuzzleTypesDataBlock
        /// </summary>
        public uint BaseCluster { get; set; }
        /// <summary>
        /// The persistent ID of this cluster
        /// </summary>
        public uint PersistentID { get; set; }
        /// <summary>
        /// The number of bio-scans to spawn with this cluster
        /// </summary>
        public int ClusterCount { get; set; }
        /// <summary>
        /// The ChainedPuzzleTypeDataBlock ID or Custom Bioscan ID of the bioscan to use for the cluster.
        /// </summary>
        public uint BioscanID { get; set; }
        /// <summary>
        /// The distance between each scan
        /// </summary>
        public float DistanceBetweenScans { get; set; }
        /// <summary>
        /// Whether or not to reveal with a holo-path
        /// </summary>
        public bool RevealWithHoloPath { get; set; }
    }
}

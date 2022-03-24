using ChainedPuzzles;
using UnityEngine;
using static MTFO.Custom.CustomBioScan;
using MTFO.Custom;
using MTFO.Utilities;
using System;
using System.Collections.Generic;

namespace MTFO.Managers
{
    /// <summary>
    /// Handles all custom puzzle related stuff so it
    /// isn't in a single patch like c'mon Dak you
    /// had one job.
    /// </summary>
    public static class CustomPuzzleManager
    {
        private readonly static Dictionary<uint, CustomBioScan> s_scanMap = new();
        private readonly static Dictionary<uint, CustomClusterScan> s_clusterMap = new();

        public static bool TryGetScanByID(uint id, out CustomBioScan scan)
            => s_scanMap.TryGetValue(id, out scan);

        public static bool TryGetClusterByID(uint id, out CustomClusterScan cluster)
            => s_clusterMap.TryGetValue(id, out cluster);

        #region Initialization

        private static bool s_initialized = false;

        internal static void Initialize(ChainedPuzzleManager manager)
        {
            if (s_initialized) return;
            s_initialized = true;

            if (ConfigManager.CustomContent.ScanHolder != null)
            {
                InitScans(manager);
                InitClusters(manager);
            }
        }

        private static void InitScans(ChainedPuzzleManager manager)
        {
            if (manager == null)
            {
                Log.Error("Attempted to initialize custom bioscans with a null ChainedPuzzleManager. This should not happen!");
                throw new NullReferenceException("ChainedPuzzleManager was null");
            }

            foreach (var scan in ConfigManager.CustomContent.ScanHolder.Scans)
            {
                Log.Debug($"Adding scan with ID of [{scan.PersistentID}]");
                if (!manager.m_puzzleComponentPrefabs.ContainsKey(scan.BaseScan))
                {
                    Log.Error($"Custom scan with persistent ID {scan.PersistentID} references a non-existing base scan of persistent ID {scan.BaseScan}");
                    continue;
                }

                //Base instance
                var scanBase = manager.m_puzzleComponentPrefabs[scan.BaseScan];
                var scanPrefab = GameObject.Instantiate(scanBase);
                // why
                scanPrefab.transform.position = new(10000, 10000, 10000);

                //Setup Scanner
                var playerScanner = scanPrefab.GetComponent<CP_PlayerScanner>();
                playerScanner.m_reduceSpeed = scan.ReduceSpeed;
                playerScanner.m_reduceWhenNoPlayer = scan.ReduceWhenNoPlayer;
                playerScanner.m_scanRadius = scan.ScanRadius;
                playerScanner.m_scanSpeeds = scan.PlayersInScanMulti;
                playerScanner.m_playerRequirement = scan.PlayerRequirement;

                //Setup Graphics
                var scanGx = scanPrefab.GetComponent<CP_Bioscan_Graphics>();
                scanGx.m_radius = scan.BioScanGraphics.Radius;
                scanGx.m_colors = ConvertToColorMode(scan.BioScanGraphics.ColorModeColor);
                scanGx.SetText(scan.BioScanGraphics.ScanText);

                var core = scanPrefab.GetComponent<CP_Bioscan_Core>();
                core.m_playerAgents = new();

                s_scanMap.Add(scan.PersistentID, scan); // if this breaks it's your problem, not mine.
                manager.m_puzzleComponentPrefabs.Add(scan.PersistentID, scanPrefab);
            }
        }

        private static void InitClusters(ChainedPuzzleManager manager)
        {
            if (manager == null)
            {
                Log.Error("Attempted to initialize custom clusters with a null ChainedPuzzleManager. This should not happen!");
                throw new NullReferenceException("ChainedPuzzleManager was null");
            }

            foreach (CustomClusterScan cluster in ConfigManager.CustomContent.ScanHolder.Clusters)
            {
                Log.Debug($"Adding cluster with ID of [{cluster.PersistentID}]");

                if (!manager.m_puzzleComponentPrefabs.ContainsKey(cluster.BaseCluster))
                {
                    Log.Error($"Custom cluster scan with persistent ID {cluster.PersistentID} references a non-existing base cluster of persistent ID {cluster.BaseCluster}");
                    continue;
                }
                if (!manager.m_puzzleComponentPrefabs.ContainsKey(cluster.BioscanID))
                {
                    Log.Error($"Custom cluster scan with persistent ID {cluster.PersistentID} references a non-existing biscan of persistent ID {cluster.BioscanID}");
                    continue;
                }

                //Base Instance
                var clusterBase = manager.m_puzzleComponentPrefabs[cluster.BaseCluster];
                var clusterPrefab = GameObject.Instantiate(clusterBase);
                // why
                clusterPrefab.transform.position = new(1000, 1000, 1000);

                //Get references
                var clusterCore = clusterPrefab.GetComponent<CP_Cluster_Core>();
                var scanPrefab = manager.m_puzzleComponentPrefabs[cluster.BioscanID];

                //Set values
                clusterCore.m_amountOfPuzzles = cluster.ClusterCount;
                clusterCore.m_childPuzzlePrefab = scanPrefab;
                clusterCore.m_distanceBetween = cluster.DistanceBetweenScans;
                clusterCore.m_revealWithHoloPath = cluster.RevealWithHoloPath;

                s_clusterMap.Add(cluster.PersistentID, cluster); // if this breaks it's your problem, not mine.
                manager.m_puzzleComponentPrefabs.Add(cluster.PersistentID, clusterPrefab);
            }
        }

        #endregion

        #region Utils
        private static ColorModeColor[] ConvertToColorMode(BioScanColorByMode[] bioScanColorByModes)
        {
            var colorModes = new ColorModeColor[bioScanColorByModes.Length];

            for (int i = 0; i < bioScanColorByModes.Length; i++)
            {
                var bsMode = bioScanColorByModes[i];
                colorModes[i] = new() { col = new(bsMode.R, bsMode.G, bsMode.B, bsMode.A), mode = bsMode.Mode };
            }

            return colorModes;
        }

        #endregion
    }
}

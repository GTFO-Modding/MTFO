using ChainedPuzzles;
using UnityEngine;
using static MTFO.Custom.CustomBioScan;
using MTFO.Custom;
using MTFO.Utilities;
using System;
using System.Collections.Generic;
using MTFO.Managers;
using MTFO.Custom.CCP.Components;
using Il2CppInterop.Runtime.Injection;

namespace MTFO.CustomCP
{
    /// <summary>
    /// Handles all custom puzzle related content.
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

            ClassInjector.RegisterTypeInIl2Cpp<CorePuzzleData>();
            ClassInjector.RegisterTypeInIl2Cpp<ClusterPuzzleData>();

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

            foreach (var scanData in ConfigManager.CustomContent.ScanHolder.Scans)
            {
                Log.Debug($"Adding scan with ID of [{scanData.PersistentID}]");
                if (!manager.m_puzzleComponentPrefabs.ContainsKey(scanData.BaseScan))
                {
                    Log.Error($"Custom scan with persistent ID {scanData.PersistentID} references a non-existing base scan of persistent ID {scanData.BaseScan}");
                    continue;
                }

                //Base instance
                var scanBasePrefab = manager.m_puzzleComponentPrefabs[scanData.BaseScan];
                if (scanBasePrefab.GetComponent<CP_Bioscan_Core>() == null)
                {
                    Log.Error($"BaseScan id: {scanData.PersistentID} does not have {nameof(CP_Bioscan_Core)} component!");
                    continue;
                }

                var scanPrefab = UnityEngine.Object.Instantiate(scanBasePrefab);
                // hide scan. Really far away so null ref patch works.
                scanPrefab.transform.position = new(10000, 10000, 10000);

                //Setup Scanner
                var scanner = scanPrefab.GetComponent<CP_PlayerScanner>();
                if (scanner != null)
                {
                    scanner.m_reduceSpeed = scanData.ReduceSpeed;
                    scanner.m_reduceWhenNoPlayer = scanData.ReduceWhenNoPlayer;
                    scanner.m_scanRadius = scanData.ScanRadius;
                    scanner.m_scanSpeeds = scanData.PlayersInScanMulti;
                    scanner.m_playerRequirement = scanData.PlayerRequirement;
                }
                else
                {
                    Log.Warn($"BaseScan id: {scanData.BaseScan} does not have {nameof(CP_PlayerScanner)} component! This will make following setting won't work!");
                    Log.Warn($" - {nameof(scanData.ReduceSpeed)}");
                    Log.Warn($" - {nameof(scanData.ReduceWhenNoPlayer)}");
                    Log.Warn($" - {nameof(scanData.ScanRadius)}");
                    Log.Warn($" - {nameof(scanData.PlayersInScanMulti)}");
                    Log.Warn($" - {nameof(scanData.PlayerRequirement)}");
                }
                

                //Setup Graphics
                var scanGx = scanPrefab.GetComponent<CP_Bioscan_Graphics>();
                if (scanGx != null)
                {
                    scanGx.m_radius = scanData.BioScanGraphics.Radius;
                    scanGx.m_colors = ConvertToColorMode(scanData.BioScanGraphics.ColorModeColor);
                    scanGx.SetText(scanData.BioScanGraphics.ScanText);
                }
                else
                {
                    Log.Warn($"BaseScan id: {scanData.BaseScan} does not have {nameof(CP_Bioscan_Graphics)} component! This will make {nameof(scanData.BioScanGraphics)} setting won't work!");
                }


                var core = scanPrefab.GetComponent<CP_Bioscan_Core>();
                core.m_playerAgents = new();

                var data = scanPrefab.AddComponent<CorePuzzleData>();
                data.PersistentID.Set(scanData.PersistentID);

                s_scanMap.Add(scanData.PersistentID, scanData); // if this breaks it's your problem, not mine.
                manager.m_puzzleComponentPrefabs.Add(scanData.PersistentID, scanPrefab);
            }
        }

        private static void InitClusters(ChainedPuzzleManager manager)
        {
            if (manager == null)
            {
                Log.Error("Attempted to initialize custom clusters with a null ChainedPuzzleManager. This should not happen!");
                throw new NullReferenceException("ChainedPuzzleManager was null");
            }

            foreach (var clusterData in ConfigManager.CustomContent.ScanHolder.Clusters)
            {
                Log.Debug($"Adding cluster with ID of [{clusterData.PersistentID}]");

                if (!manager.m_puzzleComponentPrefabs.ContainsKey(clusterData.BaseCluster))
                {
                    Log.Error($"Custom cluster scan with persistent ID {clusterData.PersistentID} references a non-existing base cluster of persistent ID {clusterData.BaseCluster}");
                    continue;
                }
                if (!manager.m_puzzleComponentPrefabs.ContainsKey(clusterData.BioscanID))
                {
                    Log.Error($"Custom cluster scan with persistent ID {clusterData.PersistentID} references a non-existing bioscan of persistent ID {clusterData.BioscanID}");
                    continue;
                }

                //Base Instance
                var clusterBasePrefab = manager.m_puzzleComponentPrefabs[clusterData.BaseCluster];
                if (clusterBasePrefab.GetComponent<CP_Cluster_Core>() == null)
                {
                    Log.Error($"BaseScan id: {clusterData.PersistentID} does not have {nameof(CP_Cluster_Core)} component!");
                    continue;
                }
                var clusterPrefab = UnityEngine.Object.Instantiate(clusterBasePrefab);
                // hide scan
                clusterPrefab.transform.position = new(1000, 1000, 1000);

                //Get references
                var clusterCore = clusterPrefab.GetComponent<CP_Cluster_Core>();
                var scanPrefab = manager.m_puzzleComponentPrefabs[clusterData.BioscanID];

                //Set values
                clusterCore.m_amountOfPuzzles = clusterData.ClusterCount;
                clusterCore.m_childPuzzlePrefab = scanPrefab;
                clusterCore.m_distanceBetween = clusterData.DistanceBetweenScans;
                clusterCore.m_revealWithHoloPath = clusterData.RevealWithHoloPath;

                var data = clusterPrefab.AddComponent<ClusterPuzzleData>();
                data.PersistentID.Set(clusterData.PersistentID);

                s_clusterMap.Add(clusterData.PersistentID, clusterData); // if this breaks it's your problem, not mine.
                manager.m_puzzleComponentPrefabs.Add(clusterData.PersistentID, clusterPrefab);
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

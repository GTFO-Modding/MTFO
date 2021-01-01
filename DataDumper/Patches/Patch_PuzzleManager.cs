using Harmony;
using DataDumper.Managers;
using DataDumper.Utilities;
using ChainedPuzzles;
using UnityEngine;
using static DataDumper.Custom.CustomBioScan;
using DataDumper.Custom;

namespace DataDumper.Patches
{
    [HarmonyPatch(typeof(ChainedPuzzleManager), "OnAssetsLoaded")]
    class Patch_PuzzleManager
    {
        public static void Postfix(ChainedPuzzleManager __instance)
        {
            if (ConfigManager.CustomContent.ScanHolder != null)
            {
                InitScans(__instance);
                InitClusters(__instance);
            }
        }

        //TODO: null checks
        private static void InitScans(ChainedPuzzleManager instance)
        {
            foreach(CustomBioScan scan in ConfigManager.CustomContent.ScanHolder.Scans)
            {
                Log.Debug($"Adding scan with ID of [{scan.PersistentID}]");
                //Base instance
                GameObject scanBase = instance.m_puzzleComponentPrefabs[scan.BaseScan];
                GameObject scanPrefab = UnityEngine.Object.Instantiate(scanBase);
                scanPrefab.transform.position = new Vector3(1000, 1000, 1000);

                //Setup Scanner
                CP_PlayerScanner playerScanner = scanPrefab.GetComponent<CP_PlayerScanner>();
                playerScanner.m_reduceSpeed = scan.ReduceSpeed;
                playerScanner.m_reduceWhenNoPlayer = scan.ReduceWhenNoPlayer;
                playerScanner.m_scanSpeedDecline = scan.ScanSpeedDecline;
                playerScanner.m_scanRadius = scan.ScanRadius;
                playerScanner.m_scanSpeeds = scan.PlayersInScanMulti;
                playerScanner.m_requireAllPlayers = scan.RequireAll;

                //Setup Graphics
                CP_Bioscan_Graphics scanGx = scanPrefab.GetComponent<CP_Bioscan_Graphics>();
                scanGx.m_radius = scan.BioScanGraphics.Radius;
                scanGx.m_colors = ConvertToColorMode(scan.BioScanGraphics.colorModeColor);


                instance.m_puzzleComponentPrefabs.Add(scan.PersistentID, scanPrefab);
            }
        }

        private static void InitClusters(ChainedPuzzleManager instance)
        {
            foreach(CustomClusterScan cluster in ConfigManager.CustomContent.ScanHolder.Clusters)
            {
                Log.Debug($"Adding cluster with ID of [{cluster.PersistentID}]");

                //Base Instance
                GameObject clusterBase = instance.m_puzzleComponentPrefabs[cluster.BaseCluster];
                GameObject clusterPrefab = UnityEngine.Object.Instantiate(clusterBase);
                clusterPrefab.transform.position = new Vector3(1000, 1000, 1000);

                //Get references
                CP_Cluster_Core clusterCore = clusterPrefab.GetComponent<CP_Cluster_Core>();
                GameObject scanPrefab = instance.m_puzzleComponentPrefabs[cluster.BioscanID];

                //Set values
                clusterCore.m_amountOfPuzzles = cluster.ClusterCount;
                clusterCore.m_childPuzzlePrefab = scanPrefab;
                clusterCore.m_distanceBetween = cluster.DistanceBetweenScans;
                clusterCore.m_revealWithHoloPath = cluster.RevealWithHoloPath;




                instance.m_puzzleComponentPrefabs.Add(cluster.PersistentID, clusterPrefab);
            }
        }

        private static ColorModeColor[] ConvertToColorMode(BioScanColorByMode[] bioScanColorByModes)
        {
            ColorModeColor[] colorModes = new ColorModeColor[bioScanColorByModes.Length];

            for (int i = 0; i < bioScanColorByModes.Length; i++)
            {
                BioScanColorByMode bsMode = bioScanColorByModes[i];
                colorModes[i] = new ColorModeColor() { col = new Color(bsMode.r, bsMode.g, bsMode.b, bsMode.a), mode = bsMode.mode };
            }

            return colorModes;
        }
    }
}

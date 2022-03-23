using HarmonyLib;
using MTFO.Managers;
using MTFO.Utilities;
using ChainedPuzzles;
using UnityEngine;
using static MTFO.Custom.CustomBioScan;
using MTFO.Custom;
using LevelGeneration;
using Player;

namespace MTFO.Patches
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
                scanPrefab.transform.position = new Vector3(10000, 10000, 10000);

                //Setup Scanner
                CP_PlayerScanner playerScanner = scanPrefab.GetComponent<CP_PlayerScanner>();
                playerScanner.m_reduceSpeed = scan.ReduceSpeed;
                playerScanner.m_reduceWhenNoPlayer = scan.ReduceWhenNoPlayer;
                playerScanner.m_scanRadius = scan.ScanRadius;
                playerScanner.m_scanSpeeds = scan.PlayersInScanMulti;
                playerScanner.m_playerRequirement = (PlayerRequirement)scan.PlayerRequirement;

                //Setup Graphics
                CP_Bioscan_Graphics scanGx = scanPrefab.GetComponent<CP_Bioscan_Graphics>();
                scanGx.m_radius = scan.BioScanGraphics.Radius;
                scanGx.m_colors = ConvertToColorMode(scan.BioScanGraphics.colorModeColor);
                scanGx.SetText(scan.BioScanGraphics.ScanText);

                CP_Bioscan_Core core = scanPrefab.GetComponent<CP_Bioscan_Core>();
                core.m_playerAgents = new Il2CppSystem.Collections.Generic.List<PlayerAgent>();

                // setup holopath
                CP_Holopath_Spline spline = scanPrefab.GetComponentInChildren<CP_Holopath_Spline>();
                if (spline != null && scan.RevealSpeed > 0f)
                    spline.m_revealSpeed = scan.RevealSpeed;

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
                CP_Holopath_Spline spline = clusterPrefab.GetComponentInChildren<CP_Holopath_Spline>();
                GameObject scanPrefab = instance.m_puzzleComponentPrefabs[cluster.BioscanID];

                //Set values
                clusterCore.m_amountOfPuzzles = cluster.ClusterCount;
                clusterCore.m_childPuzzlePrefab = scanPrefab;
                clusterCore.m_distanceBetween = cluster.DistanceBetweenScans;
                clusterCore.m_revealWithHoloPath = cluster.RevealWithHoloPath;
                if (spline != null && cluster.RevealSpeed > 0f)
                    spline.m_revealSpeed = cluster.RevealSpeed;

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

    [HarmonyPatch(typeof(CP_Bioscan_Core), "Update")]
    class FixNullRefSpam
    {
        static bool Prefix(CP_Bioscan_Core __instance)
        {
            if (__instance.gameObject.transform.position.x > 9000 && __instance.gameObject.transform.position.y > 9000 && __instance.gameObject.transform.position.z > 9000)
            {
                return false;
            }
            return true;
        }
    }
}

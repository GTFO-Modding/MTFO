using System;
using System.Collections.Generic;
using MelonLoader;
using System.IO;
using DataDumper.Custom.Scans;
using Newtonsoft.Json;
using DataDumper.Utilities;

namespace DataDumper.Managers
{
    public class ContentManager
    {
        private readonly Dictionary<string, Action<string>> Handlers;
        public ScanHolder ScanHolder;

        public ContentManager()
        {
            Handlers = new Dictionary<string, Action<string>>
            {
                { "welcome.txt", SetupWelcomeText },
                { "chainedpuzzle.json", SetupChainedPuzzles }
            };
            Init();
        }

        private void Init()
        {
            if (!ConfigManager.HasCustomContent) return;

            foreach(string key in Handlers.Keys)
            {
                if (PathUtil.CheckPath(key, out string path))
                {
                    Handlers.TryGetValue(key, out Action<string> value);
                    value?.Invoke(path);
                    MelonLogger.Log(path);
                }
            }
        }

        public void SetupWelcomeText(string path)
        {
            ConfigManager.MenuText = File.ReadAllText(path);
        }

        public void SetupChainedPuzzles(string path)
        {
            /*ScanHolder testing = new ScanHolder() { 
                Scans = new List<CustomBioScan> { 
                    new CustomBioScan() { 
                        BaseScan = 2, 
                        PersistentID = 100,
                        ScanProgression = 2f, 
                        ScanRadius = 10f, 
                        ReduceWhenNoPlayer = true,
                        ReduceSpeed = 10,
                        PlayersInScanMulti = new float[] { 1f, 1f, 1f, 1f }, 
                        BioScanGraphics = new CustomBioScan.BioScanGx() { 
                            Radius = 10f,
                            colorModeColor = new CustomBioScan.BioScanColorByMode[]
                            {
                                new CustomBioScan.BioScanColorByMode()
                                {
                                    mode = ChainedPuzzles.eChainedPuzzleGraphicsColorMode.Active,
                                    r = 0,
                                    b = 1,
                                    g = 0
                                },
                                new CustomBioScan.BioScanColorByMode()
                                {
                                    mode = ChainedPuzzles.eChainedPuzzleGraphicsColorMode.Alarm_Active,
                                    r = 0,
                                    b = 1,
                                    g = 1
                                },
                                new CustomBioScan.BioScanColorByMode()
                                {
                                    mode = ChainedPuzzles.eChainedPuzzleGraphicsColorMode.Waiting,
                                    r = 0,
                                    b = 0,
                                    g = 1
                                },
                                new CustomBioScan.BioScanColorByMode()
                                {
                                    mode = ChainedPuzzles.eChainedPuzzleGraphicsColorMode.Alarm_Waiting,
                                    r = 1,
                                    b = 1,
                                    g = 0
                                }
                            }
                        } 
                    } 
                },
                Clusters = new List<CustomClusterScan>
                {
                    new CustomClusterScan()
                    {
                        BaseCluster = 4,
                        PersistentID = 101,
                        ClusterCount = 16,
                        BioscanID = 100
                    }
                }
            };*/
            //File.WriteAllText(path, JsonConvert.SerializeObject(testing));

            ScanHolder = JsonConvert.DeserializeObject<ScanHolder>(File.ReadAllText(path));
        }
    }
}

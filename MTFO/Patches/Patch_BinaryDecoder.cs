using MTFO.Managers;
using MTFO.Utilities;
using HarmonyLib;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using GameData;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace MTFO.Patches
{
    [HarmonyPatch(typeof(BinaryEncoder), "Decode")]
    class Patch_BinaryDecoder
    {
        static JsonSerializer json = new();
        static Il2CppArrayBase<TextAsset> lookup;
        static List<TextAsset> gameData;
        static Dictionary<int, string> localGDL;
        static int lookupLength = 0;
        static int count = 0;

        public static void Prefix(ref string __result)
        {
            if (lookupLength == 0)
                localGDL = ConfigManager.gameDataLookup;

            if (lookup == null && ConfigManager.gameDataLookup == null)
            {
                Log.Debug("First cycle setup...");
                lookup = Resources.LoadAll<TextAsset>("GameData");
                gameData = new List<TextAsset>();
                localGDL = new Dictionary<int, string>();
                foreach (var item in lookup)
                {
                    if (item.name.EndsWith("DataBlock_bin"))
                    {
                        lookupLength++;
                        gameData.Add(item);
                        Log.Verbose($"Added {item.name} to game data");
                    }
                }
                Log.Debug($"Total length: {lookupLength}");
                if (localGDL == null)
                {
                    Log.Error("What????????");
                }
                foreach (var item in gameData)
                {
                    string text = BinaryEncoder.Decode(GameDataInit.m_binaryEncoder, item.bytes);
                    int hash = text.GetStableHashCode();
                    string name = item.name.Replace('.', '_');
                    Log.Verbose($"Adding {name} with has {hash}");
                    if (localGDL == null)
                    {
                        Log.Error("???????");
                    }
                    localGDL.Add(hash, name);
                    Log.Verbose($"Added");
                    count++;
                    Log.Verbose($"Added {name} to lookup table. {count} / {lookupLength}");
                }

                string serializedLookup = json.Serialize(localGDL);
                File.WriteAllText(ConfigManager.GameDataLookupPath, serializedLookup);
            }
        }

        public static void Postfix(ref string __result, Il2CppStructArray<byte> bytes)
        {
            //Prevent writing till we've decoded all the datablocks
            if (count < lookupLength)
            {
                return;
            }           

            //Ensure the file is game data related
            if (__result.Contains("Headers"))
            {
                int hash = bytes.GetHashCode();
                localGDL.TryGetValue(__result.GetStableHashCode(), out string name);

                if (name != null)
                {
                    Log.Verbose("Found " + name);
                    try
                    {
                        string filePath = Path.Combine(ConfigManager.GameDataPath, name + ".json");
                        if (File.Exists(filePath))
                        {
                            Log.Verbose("Reading [" + name + "] from disk...");
                            Log.Verbose(filePath);
                            __result = File.ReadAllText(filePath);

                            return;
                        }
                        else
                        {
                            Log.Verbose("No file found at [" + filePath + "], writing file to disk...");
                            File.WriteAllText(filePath, __result);
                        }
                    }
                    catch
                    {
                        Log.Error("Failed to write " + name + " to disk!!");
                    }
                }
                else
                {
                    string errorPath = Path.Combine(ConfigManager.GameDataPath, "UNKNOWN");
                    if (!Directory.Exists(errorPath))
                    {
                        Directory.CreateDirectory(errorPath);
                    }
                    string errorFilePath = Path.Combine(errorPath, hash + ".json");
                    Log.Error("Failed to find match for hash [" + hash + "]! Cannot load custom data for this block!");
                    if (File.Exists(errorFilePath))
                    {
                        Log.Warn("-- FILE FOUND IN DUMP FOLDER WITH MATCHING HASH FILE NAME, LOADING INSTEAD --");
                        __result = File.ReadAllText(errorFilePath);
                        return;
                    }

                    if (ConfigManager.DumpUnknownFiles)
                    {
                        Log.Debug("----- FILE CONTENT DUMP START -----");
                        Log.Debug(__result);
                        Log.Debug("----- FILE CONTENT DUMP END -----");
                    }
                    Log.Debug("DUMPING FILE CONTENTS TO [" + errorFilePath + "]");
                    File.WriteAllText(errorFilePath, __result);
                }
            }
        }
    }
}

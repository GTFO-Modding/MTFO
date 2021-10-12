using MTFO.Managers;
using MTFO.Utilities;
using HarmonyLib;
using System.IO;
using UnhollowerBaseLib;
using UnityEngine;
using System.Collections.Generic;
using GameData;
using System.Text.Json;

namespace MTFO.Patches
{
    [HarmonyPatch(typeof(BinaryEncoder), "Decode")]
    class Patch_BinaryDecoder
    {
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

                string serializedLookup = JsonSerializer.Serialize(localGDL);
                ConfigManager.GameDataLookupPath
                    .GetPath()
                    .PathFile.WriteAllText($"{ConfigManager.GAME_VERSION}.json", serializedLookup);
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
                        IDirectoryFile file = ConfigManager.GameDataPath.GetFile(name + ".json");
                        if (file.Exists())
                        {
                            Log.Verbose("Reading [" + name + "] from disk...");
                            Log.Verbose(file);
                            __result = file.ReadAllText();

                            return;
                        }
                        else
                        {
                            Log.Verbose("No file found at [" + file + "], writing file to disk...");
                            file.WriteAllText(__result);
                        }
                    }
                    catch
                    {
                        Log.Error("Failed to write " + name + " to disk!!");
                    }
                }
                else
                {
                    var errorPath = ConfigManager.GameDataPath
                        .GetDirectory("UNKNOWN");
                    if (!errorPath.Exists())
                    {
                        errorPath.Create();
                    }
                    var errorFilePath = errorPath.GetFile(hash + ".json");
                    Log.Error("Failed to find match for hash [" + hash + "]! Cannot load custom data for this block!");
                    if (errorFilePath.Exists())
                    {
                        Log.Warn("-- FILE FOUND IN DUMP FOLDER WITH MATCHING HASH FILE NAME, LOADING INSTEAD --");
                        __result = errorFilePath.ReadAllText();
                        return;
                    }

                    if (ConfigManager.DumpUnknownFiles)
                    {
                        Log.Debug("----- FILE CONTENT DUMP START -----");
                        Log.Debug(__result);
                        Log.Debug("----- FILE CONTENT DUMP END -----");
                    }
                    Log.Debug("DUMPING FILE CONTENTS TO [" + errorFilePath + "]");
                    errorFilePath.WriteAllText(__result);
                }
            }
        }
    }
}

using DataDumper.Managers;
using DataDumper.Utilities;
using Harmony;
using MelonLoader;
using System.IO;

namespace DataDumper.Patches
{

    [HarmonyPatch(typeof(BinaryEncoder), "Decode")]
    class Patch_BinaryDecoder
    {
        public static void Postfix(ref string __result)
        {
            //Ensure the file is game data related
            if (__result.Contains("Headers"))
            {
                int hash = __result.GetHashCode();
                ConfigManager.gameDataLookup.TryGetValue(hash, out string name);

                if (name != null)
                {
                    Log.Verbose("Found " + name);
                    try
                    {
                        string filePath = Path.Combine(ConfigManager.GameDataPath, name + ".json");
                        if (File.Exists(filePath))
                        {
                            Log.Verbose("Reading [" + name + "] from disk...");
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
                        MelonLogger.LogError("Failed to write " + name + " to disk!!");
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
                    MelonLogger.LogError("Failed to find match for hash [" + hash + "]!");
                    if (File.Exists(errorFilePath))
                    {
                        MelonLogger.LogWarning("-- FILE FOUND IN DUMP FOLDER WITH MATCHING HASH FILE NAME, LOADING INSTEAD --");
                        __result = File.ReadAllText(errorFilePath);
                        return;
                    }

                    MelonLogger.LogError("----- FILE CONTENT DUMP START -----");
                    MelonLogger.LogError(__result);
                    MelonLogger.LogError("----- FILE CONTENT DUMP END -----");
                    MelonLogger.LogError("DUMPING FILE CONTENTS TO [" + errorFilePath + "]");
                    File.WriteAllText(errorFilePath, __result);
                }
            }
        }
    }
}

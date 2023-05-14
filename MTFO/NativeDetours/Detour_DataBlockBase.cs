using BepInEx;
using BepInEx.Unity.IL2CPP.Hook;
using GameData;
using GTFO.API;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Runtime;
using MTFO.API;
using MTFO.Managers;
using MTFO.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace MTFO.NativeDetours
{
    using DBBase = GameDataBlockBase<EnemyDataBlock>;

    internal static class Detour_DataBlockBase
    {
        private unsafe delegate IntPtr GetFileContentsDel(Il2CppMethodInfo* methodInfo);

        private static string _BasePathToDump;
        private static INativeDetour _Detour; //To Prevent GC Error
        private static GetFileContentsDel _Original;

        public static unsafe void Patch()
        {
            _BasePathToDump = Path.Combine(Paths.BepInExRootPath, "GameData-Dump", CellBuildData.GetRevision().ToString());
            if (ConfigManager.DumpGameData)
            {
                PathUtil.PrepareEmptyDirectory(_BasePathToDump);
            }

            var method = Il2CppAPI.GetIl2CppMethod<DBBase>(
            nameof(DBBase.GetFileContents),
            typeof(string).FullName,
            isGeneric: false,
            Array.Empty<string>());

            _Detour = INativeDetour.CreateAndApply((nint)method, Dtor_GetFileContents, out _Original);
        }

        private static unsafe IntPtr Dtor_GetFileContents(Il2CppMethodInfo* methodInfo)
        {
            var originalResult = _Original.Invoke(methodInfo);

            try
            {
                var datablock = new DataBlockBaseWrapper(methodInfo);
                var datablockName = datablock.BinaryFileName;
                var jsonFileName = $"{datablockName}.json";

                //
                // Dump Vanilla Blocks
                //
                if (ConfigManager.DumpGameData)
                {
                    DumpContent(datablock, IL2CPP.Il2CppStringToManaged(originalResult));
                }

                Log.Verbose($"GetFileContents Call of {datablockName}");

                var jsonStream = GetContentStream(datablock, originalResult);
                var jsonNode = jsonStream.ToJsonNode();
                var blocks = jsonNode["Blocks"].AsArray();

                //
                // Read Partial Data JSONs
                //
                var pDataPath = Path.Combine(ConfigManager.GameDataPath, datablock.FileName);
                if (Directory.Exists(pDataPath))
                {
                    int count = 0;
                    foreach (var filePath in Directory.GetFiles(pDataPath, "*.json"))
                    {
                        Log.Verbose($" - Trying to add PartialData [{filePath}]");
                        if (!File.Exists(filePath))
                        {
                            continue;
                        }

                        if (PathUtil.OpenUtf8Stream(filePath).TryParseJsonNode(dispose: true, out var pDataNode))
                        {
                            blocks.Add(pDataNode);
                            count++;
                        }
                    }

                    Log.Verbose($" - Added {count} partial data of {datablockName}");
                }

                //
                // Custom API
                //
                var jsonItemsToInject = new List<string>();
                MTFOGameDataAPI.Invoke_OnGameDataContentLoad(datablock.FileName, jsonStream, in jsonItemsToInject);
                foreach(var injectJson in jsonItemsToInject)
                {
                    try
                    {
                        var newBlock = injectJson.ToJsonNode();
                        blocks.Add(newBlock);
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Exception were found while reading Injected Json Data!");
                        Log.Error(e.ToString());
                        Log.Error($"Full Json:\n{injectJson}");
                    }
                }

                //
                // Update LastPersistentID
                //
                var highestPresistentID = 0u;
                foreach (var block in jsonNode["Blocks"].AsArray())
                {
                    var id = (uint)block["persistentID"].AsValue();
                    if (id > highestPresistentID)
                    {
                        highestPresistentID = id;
                    }
                }
                jsonNode["LastPersistentID"] = highestPresistentID;

                //
                // Pass Result
                //
                var resultJson = jsonNode.ToJsonStringIndented();
                var resultJsonPtr = IL2CPP.ManagedStringToIl2Cpp(resultJson);
                MTFOGameDataAPI.Invoke_OnGameDataContentLoaded(datablock.FileName, resultJson);
                return resultJsonPtr;
            }
            catch (Exception e)
            {
                Log.Error($"Exception were found while handling Detour;Falling back to original content!");
                Log.Error(e.ToString());
            }
            return originalResult;
        }

        private static Stream GetContentStream(DataBlockBaseWrapper datablock, IntPtr originalContentPtr)
        {
            var fileName = datablock.BinaryFileName;
            var jsonFileName = $"{fileName}.json";
            var originalJson = IL2CPP.Il2CppStringToManaged(originalContentPtr);

            string filePath = Path.Combine(ConfigManager.GameDataPath, jsonFileName);
            if (File.Exists(filePath))
            {
                Log.Verbose($"Opening filestream of [{fileName}] from disk...");
                Log.Verbose(filePath);
                return PathUtil.OpenUtf8Stream(filePath);
            }
            else
            {
                Log.Verbose($"No file found at [{fileName}]");
                return new MemoryStream(Encoding.UTF8.GetBytes(originalJson));
            }
        }

        private static void DumpContent(DataBlockBaseWrapper datablock, string json)
        {
            if (!json.TryParseToJsonNode(out var jsonNode))
            {
                Log.Verbose($"Unable to dump {datablock.FileName}, Invalid Json Content!");
                return;
            }

            var shouldMakeItPartialData = false;
            switch (ConfigManager.DumpMode)
            {
                case DumpGameDataMode.Single:
                    shouldMakeItPartialData = false;
                    break;

                case DumpGameDataMode.PartialData:
                    shouldMakeItPartialData = datablock.PreferPartialBlockOnDump;
                    break;

                case DumpGameDataMode.FullPartialData:
                    shouldMakeItPartialData = true;
                    break;
            }

            if (shouldMakeItPartialData) //PartialData Dump Moment
            {
                var blocks = jsonNode["Blocks"].AsArray();
                var folderName = datablock.FileName;
                var baseFolder = Path.Combine(_BasePathToDump, folderName);
                PathUtil.PrepareEmptyDirectory(baseFolder);

                foreach (var block in blocks)
                {
                    var partialJsonFileName = $"{block["persistentID"]}__{block["name"]}.json";
                    var partialDataSavePath = Path.Combine(baseFolder, partialJsonFileName);
                    File.WriteAllText(partialDataSavePath, block.ToJsonStringIndented());
                    Log.Verbose($" - Save... {partialDataSavePath}");
                }
                jsonNode["Blocks"] = new JsonArray();
            }

            var fileName = datablock.BinaryFileName;
            var jsonFileName = $"{fileName}.json";

            var savePath = Path.Combine(_BasePathToDump, jsonFileName);
            File.WriteAllText(savePath, jsonNode.ToJsonStringIndented());
            Log.Verbose($"{datablock.FileName} has dumped to '{_BasePathToDump}'");
        }
    }
}

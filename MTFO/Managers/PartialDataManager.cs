using MTFO.PartialData;
using MTFO.Utilities;
using MTFO.Utilities.JsonConverters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MTFO.Managers
{
    public static class PartialDataManager
    {
        private static readonly List<string> _AddedFileList = new List<string>();
        private static readonly Dictionary<string, uint> _GUIDDict = new Dictionary<string, uint>();

        public static void UpdatePartialData()
        {
            if (!ConfigManager.HasPartialData)
                return;

            var configPath = Path.Combine(ConfigManager.PartialDataPath, "_config.json");
            if (!File.Exists(configPath))
                return;

            _AddedFileList.Clear();

            var configs = JsonConvert.DeserializeObject<List<DataBlockDefinition>>(File.ReadAllText(configPath));
            foreach (var def in configs)
            {
                if (!DataBlockTypeLoader.TryFindCache(def.TypeName, out var cache))
                    continue;

                var idBuffer = def.GuidConfig.StartFromID;
                var idConverter = new PersistentIDConverter
                {
                    SaveID = (string guid) =>
                    {
                        if (TryAssignID(guid, idBuffer))
                        {
                            if (def.GuidConfig.IncrementMode == MapperIncrementMode.Decrement)
                                return idBuffer--;
                            else
                                return idBuffer++;
                        }
                        return 0;
                    },

                    LoadID = (string guid) =>
                    {
                        return GetID(guid);
                    }
                };

                Log.Verbose($"Found Type Cache from TypeName!: {def.TypeName}");
                Log.Verbose($"Found Those files with filters:");

                foreach (var searchConfig in def.SearchConfigs)
                {
                    var searchOption = searchConfig.CheckSubDirectory ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                    var searchPath = Path.Combine(ConfigManager.PartialDataPath, searchConfig.BaseSubDirectory);
                    var files = Directory.GetFiles(searchPath, searchConfig.FileSearchPattern, searchOption).OrderBy(f => f);
                    foreach (var file in files)
                    {
                        if(_AddedFileList.Contains(file))
                        {
                            Log.Error($"File ({file}) has loaded multiple times!");
                            continue;
                        }

                        Log.Verbose($" - {file}");
                        cache.AddJsonBlock(File.ReadAllText(file), idConverter);
                        _AddedFileList.Add(file);
                    }
                }
            }
        }

        private static bool TryAssignID(string guid, uint id)
        {
            if (_GUIDDict.ContainsKey(guid))
            {
                Log.Error($"GUID is already used: {guid}");
                return false;
            }

            _GUIDDict.Add(guid, id);
            return true;
        }

        private static uint GetID(string guid)
        {
            if (!_GUIDDict.TryGetValue(guid, out var id))
            {
                Log.Error($"GUID is Missing: {guid}");
                return 0;
            }
            return id;
        }
    }
}

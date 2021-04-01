using System;
using System.Collections.Generic;
using System.Text;

namespace MTFO.PartialData
{
    public class DataBlockDefinition
    {
        public string TypeName { get; set; } = "LevelLayoutDataBlock";
        public GUIDMapperConfig GuidConfig { get; set; } = new GUIDMapperConfig();
        public SearchConfig[] SearchConfigs { get; set; } = new SearchConfig[0];
    }

    public class GUIDMapperConfig
    {
        public uint StartFromID { get; set; } = uint.MaxValue;
        public MapperIncrementMode IncrementMode { get; set; } = MapperIncrementMode.Decrement;
    }

    public class SearchConfig
    {
        public string BaseSubDirectory { get; set; } = "";
        public string FileSearchPattern { get; set; } = "layout.*.json";
        public bool CheckSubDirectory { get; set; } = false;
    }

    public enum MapperIncrementMode
    {
        Decrement, Increment
    }
}

using MTFO.Utilities;
using MTFO.Utilities.JsonConverters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MTFO.PartialData
{
    public class DataBlockTypeCache
    {
        private static readonly JsonConverter[] _CommonConverters;

        static DataBlockTypeCache()
        {
            _CommonConverters = new JsonConverter[]
            {
                new Il2CppListReadOnlyConverter()
            };
        }

        public string TypeName;
        public Type SerializeType;
        public MethodInfo AddBlockMethod;
        public MethodInfo HasBlockMethod;

        public void AddBlock(object block)
        {
            dynamic blockDyn = block;

            if (HasBlock(blockDyn.persistentID))
            {
                Log.Error($"Block's persistentID has conflicted with other!: {blockDyn.persistentID}, {blockDyn.name}");
                return;
            }

            AddBlockMethod.Invoke(null, new object[] { block, -1 });

            Log.Verbose($"Added Block: {blockDyn.persistentID}, {blockDyn.name}");
        }

        public bool HasBlock(uint id)
        {
            return (bool)HasBlockMethod.Invoke(null, new object[] { id });
        }

        public object Deserialize(string content, params JsonConverter[] converters)
        {
            SerializeType.MakeArrayType();

            var newArray = _CommonConverters.Concat(converters);
            return JsonConvert.DeserializeObject(content, SerializeType, newArray.ToArray());
        }

        public void AddJsonBlock(string json, params JsonConverter[] converters)
        {
            try
            {
                var convertersArray = _CommonConverters.Concat(converters).ToArray();
                var jObj = JToken.Parse(json);
                if (jObj.Type == JTokenType.Array)
                {
                    dynamic blocks = JsonConvert.DeserializeObject(json, SerializeType.MakeArrayType(), convertersArray);
                    foreach (var block in blocks)
                    {
                        AddBlock(block);
                    }
                }
                else if (jObj.Type == JTokenType.Object)
                {
                    var block = JsonConvert.DeserializeObject(json, SerializeType, convertersArray);
                    AddBlock(block);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error While Adding Block: {e}");
            }
        }
    }
}

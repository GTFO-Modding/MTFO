using GameData;
using MTFO.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTFO.PartialData
{
    public static class DataBlockTypeLoader
    {
        private static readonly List<DataBlockTypeCache> _DataBlockCache = new List<DataBlockTypeCache>();
        private static readonly Type[] _HasBlockTypeFilter = new Type[] { typeof(uint) };


        static DataBlockTypeLoader()
        {
            try
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var asm = assemblies.First(a => !a.IsDynamic && a.Location.EndsWith("Modules-ASM.dll"));

                var dataBlockNameCache = new List<string>();
                foreach (var cppType in GameDataInit.m_allDataBlockTypes)
                {
                    dataBlockNameCache.Add(cppType.Name);
                }

                var dataBlockTypes = new List<Type>();
                foreach (var type in asm.ExportedTypes)
                {
                    if (type == null)
                        continue;

                    if (string.IsNullOrEmpty(type.Namespace))
                        continue;

                    if (!type.Namespace.Equals("GameData"))
                        continue;

                    if (dataBlockNameCache.Contains(type.Name))
                        dataBlockTypes.Add(type);
                }

                var genericBaseType = typeof(GameDataBlockBase<>);
                foreach (var type in dataBlockTypes)
                {
                    var genericType = genericBaseType.MakeGenericType(type);
                    _DataBlockCache.Add(new DataBlockTypeCache()
                    {
                        TypeName = type.Name,
                        SerializeType = type,
                        AddBlockMethod = genericType.GetMethod("AddBlock"),
                        HasBlockMethod = genericType.GetMethod("HasBlock", _HasBlockTypeFilter)
                    });
                }
            }
            catch (Exception e)
            {
                Log.Error($"Can't make cache from Modules-ASM.dll!: {e}");
            }
        }

        public static bool TryFindCache(string blockTypeName, out DataBlockTypeCache cache)
        {
            var index = -1;
            if ((index = _DataBlockCache.FindIndex(x => x.TypeName.Equals(blockTypeName))) != -1)
            {
                cache = _DataBlockCache[index];
                return true;
            }

            cache = null;
            return false;
        }
    }
}

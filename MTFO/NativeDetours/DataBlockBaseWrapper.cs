using GameData;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTFO.NativeDetours
{
    internal class DataBlockBaseWrapper
    {
        private static readonly HashSet<string> _PreferPartialDumpBlocks = new()
        {
            nameof(RundownDataBlock).ToLower(),
            nameof(LightSettingsDataBlock).ToLower(),
            nameof(FogSettingsDataBlock).ToLower(),
            nameof(LevelLayoutDataBlock).ToLower(),
            nameof(WardenObjectiveDataBlock).ToLower(),
            nameof(DimensionDataBlock).ToLower(),
            nameof(EnemyDataBlock).ToLower(),
            nameof(EnemySFXDataBlock).ToLower(),
            nameof(EnemyBehaviorDataBlock).ToLower(),
            nameof(EnemyBalancingDataBlock).ToLower(),
            nameof(EnemyMovementDataBlock).ToLower(),
            nameof(ArchetypeDataBlock).ToLower(),
            nameof(PlayerOfflineGearDataBlock).ToLower(),
            nameof(ComplexResourceSetDataBlock).ToLower(),
            nameof(TextDataBlock).ToLower()
        };

        public IntPtr ClassPointer { get; private set; }
        public string FileName { get; private set; }
        public string BinaryFileName { get; private set; }
        public bool PreferPartialBlockOnDump { get; private set; } = false;

        private IntPtr Ptr__m_fileNameNoExt;

        public unsafe DataBlockBaseWrapper(Il2CppMethodInfo* methodInfo)
        {
            var method = UnityVersionHandler.Wrap(methodInfo);
            var klass = UnityVersionHandler.Wrap(method.Class);

            ClassPointer = klass.Pointer;
            Ptr__m_fileNameNoExt = IL2CPP.GetIl2CppField(klass.Pointer, "m_fileNameNoExt");

            var fileNamePtr = IntPtr.Zero;
            IL2CPP.il2cpp_field_static_get_value(Ptr__m_fileNameNoExt, &fileNamePtr);
            FileName = IL2CPP.Il2CppStringToManaged(fileNamePtr).Replace('.', '_');
            BinaryFileName = FileName + "_bin";

            if (_PreferPartialDumpBlocks.Contains(FileName.ToLower().Replace("gamedata_", "")))
            {
                PreferPartialBlockOnDump = true;
            }
        }
    }
}


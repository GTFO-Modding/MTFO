using BepInEx;
using BepInEx.IL2CPP.Hook;
using GameData;
using GTFO.API;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Runtime;
using MTFO.Managers;
using MTFO.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTFO.NativeDetours
{
    internal static class Detour_DataBlockBase
    {
        private unsafe delegate IntPtr GetFileContentsDel(Il2CppMethodInfo* methodInfo);

        private static string _BasePathToDump;
        private static INativeDetour _Detour; //To Prevent GC Error
        private static GetFileContentsDel _Original;

        public static unsafe void Patch()
        {
            _BasePathToDump = Path.Combine(Paths.BepInExRootPath, "gamedata", CellBuildData.GetRevision().ToString());
            if (ConfigManager.DumpGameData)
            {
                if (!Directory.Exists(_BasePathToDump))
                {
                    Directory.CreateDirectory(_BasePathToDump);
                }
            }

            _Detour = Detours.Create(
               typeof(GameDataBlockBase<EnemyDataBlock>),
               isGenericMethod: false,
               "GetFileContents",
               typeof(string).FullName,
               new string[] { },
               Dtor_DoLoadFromDisk,
               out _Original);
        }

        private static unsafe IntPtr Dtor_DoLoadFromDisk(Il2CppMethodInfo* methodInfo)
        {
            var originalResult = _Original.Invoke(methodInfo);

            try
            {
                var method = UnityVersionHandler.Wrap(methodInfo);
                var klass = UnityVersionHandler.Wrap(method.Class);

                var fileNameFieldPtr = IL2CPP.GetIl2CppField(klass.Pointer, "m_fileNameBytesNoExt");

                var fileNamePtr = IntPtr.Zero;
                IL2CPP.il2cpp_field_static_get_value(fileNameFieldPtr, &fileNamePtr);
                var fileName = IL2CPP.Il2CppStringToManaged(fileNamePtr).Replace('.', '_');

                Log.Verbose($"GetFileContents Call of {fileName}");

                if (ConfigManager.DumpGameData)
                {
                    File.WriteAllText(Path.Combine(_BasePathToDump, $"{fileName}.json"), IL2CPP.Il2CppStringToManaged(originalResult));
                    Log.Verbose($"{fileName} has dumped to '{_BasePathToDump}'");
                }

                string filePath = Path.Combine(ConfigManager.GameDataPath, $"{fileName}.json");
                if (File.Exists(filePath))
                {
                    Log.Verbose($"Reading [{fileName}] from disk...");
                    Log.Verbose(filePath);

                    var json = File.ReadAllText(filePath);
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        throw new InvalidDataException($"File Content for '{filePath}' was null or whitespace! This is not allowed!");
                    }

                    return IL2CPP.ManagedStringToIl2Cpp(json);
                }
                else
                {
                    Log.Verbose($"No file found at [{fileName}]");
                }
            }
            catch (Exception e)
            {
                Log.Error($"Exception were found while handling  Detour;Falling back to original content!");
                Log.Error(e.ToString());
            }
            return originalResult;
        }
    }
}

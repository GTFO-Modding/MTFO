using BepInEx;
using BepInEx.Unity.IL2CPP;
using BepInEx.Unity.IL2CPP.Hook;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.Runtime;
using Il2CppInterop.Runtime;
using System.IO;
using GameData;
using System;
using System.Text;
using BepInEx.Configuration;

namespace MTFO
{
    using DataBlockBase = GameDataBlockBase<ArchetypeDataBlock>;

    [BepInDependency("dev.gtfomodding.gtfo-api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        public override unsafe void Load()
        {
            _GameDataPath = Config.Bind("GameData", "Path", Path.Combine(Paths.BepInExRootPath, "GameData"), "Where gamedata json files are handled");
            _DumpGameData = Config.Bind("GameData", "Dump", false, "Deletes and then writes gamedata json files to the specified path");

            Directory.CreateDirectory(_GameDataPath.Value);

            var method = GetIl2CppMethod<DataBlockBase>(
                nameof(DataBlockBase.GetFileContents),
                typeof(string).FullName,
                isGeneric: false,
                Array.Empty<string>());

            _NativeDetour = INativeDetour.CreateAndApply((nint)method, GetFileContentsDetour, out _GetFileContentsNative);
        }

        private unsafe delegate IntPtr GetFileContentsDel(Il2CppMethodInfo* methodInfo);

        private unsafe void* GetIl2CppMethod<T>(string methodName, string returnTypeName, bool isGeneric, params string[] argTypes) where T : Il2CppObjectBase
        {
            void** ppMethod = (void**)IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<T>.NativeClassPtr, isGeneric, methodName, returnTypeName, argTypes).ToPointer();
            if ((long)ppMethod == 0) return ppMethod;

            return *ppMethod;
        }

        private unsafe IntPtr GetFileContentsDetour(Il2CppMethodInfo* methodInfo)
        {
            var originalResult = _GetFileContentsNative.Invoke(methodInfo);

            try
            {
                var method = UnityVersionHandler.Wrap(methodInfo);
                var klass = UnityVersionHandler.Wrap(method.Class);

                var fileNameFieldPtr = IL2CPP.GetIl2CppField(klass.Pointer, "m_fileNameBytesNoExt");

                var fileNamePtr = IntPtr.Zero;
                IL2CPP.il2cpp_field_static_get_value(fileNameFieldPtr, &fileNamePtr);
                var fileName = IL2CPP.Il2CppStringToManaged(fileNamePtr).Replace('.', '_');

                var jsonFile = new FileInfo(Path.Combine(_GameDataPath.Value, $"{fileName}.json"));
                FileStream fileStream;
                byte[] buffer;
                var encode = new UTF8Encoding(true);
                string content;

                if (_DumpGameData.Value)
                {
                    content = IL2CPP.Il2CppStringToManaged(originalResult);
                    buffer = encode.GetBytes(content);
                    if (jsonFile.Exists) jsonFile.Delete();
                    using (fileStream = jsonFile.Create())
                    {
                        fileStream.Write(buffer, 0, buffer.Length);
                    }
                    fileStream.Dispose();
                }
                else if (jsonFile.Exists)
                {
                    using (fileStream = jsonFile.OpenRead())
                    {
                        buffer = new byte[jsonFile.Length];
                        int bufferOffset = 0;
                        var bufferCount = (int)jsonFile.Length;
                        int bufferIndex;
                        while (bufferOffset < bufferCount)
                        {
                            bufferIndex = fileStream.Read(buffer, bufferOffset, bufferCount);
                            if (bufferIndex == 0) break;
                            bufferOffset += bufferIndex;
                            bufferCount -= bufferIndex;
                        }
                    }
                    fileStream.Dispose();

                    content = encode.GetString(buffer, 0, buffer.Length);
                    if (content.IsNullOrWhiteSpace())
                    {
                        throw new InvalidDataException($"File Content for '{jsonFile.Name}' was null or whitespace! This is not allowed!");
                    }
                    return IL2CPP.ManagedStringToIl2Cpp(content);
                }
            }
            catch (Exception e)
            {
                Log.LogWarning($"Exception were found while handling  Detour;Falling back to original content!");
                Log.LogWarning(e.ToString());
            }
            return originalResult;
        }

        private INativeDetour _NativeDetour; //To Prevent GC Error
        private GetFileContentsDel _GetFileContentsNative;
        private ConfigEntry<bool> _DumpGameData;
        private ConfigEntry<string> _GameDataPath;
    }
}
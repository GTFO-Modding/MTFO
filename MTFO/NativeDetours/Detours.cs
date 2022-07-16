using BepInEx.IL2CPP.Hook;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTFO.NativeDetours
{
    internal static class Detours
    {
        public static unsafe INativeDetour Create<D>(Type Class, bool isGenericMethod, string methodName, string returnType, string[] paramTypes, D del, out D originalDel)
            where D : Delegate
        {
            var classPtr = Il2CppClassPointerStore.GetNativeClassPointer(Class);
            var methodPtr = IL2CPP.GetIl2CppMethod(classPtr, isGenericMethod, methodName, returnType, paramTypes);
            var methodInfo = new Il2CppSystem.Reflection.MethodInfo(IL2CPP.il2cpp_method_get_object(methodPtr, classPtr));

            var il2cppMethodInfo = UnityVersionHandler.Wrap((Il2CppMethodInfo*)IL2CPP.il2cpp_method_get_from_reflection(
                IL2CPP.Il2CppObjectBaseToPtrNotNull(methodInfo)));

            return INativeDetour.CreateAndApply(il2cppMethodInfo.MethodPointer, del, out originalDel);
        }
    }
}

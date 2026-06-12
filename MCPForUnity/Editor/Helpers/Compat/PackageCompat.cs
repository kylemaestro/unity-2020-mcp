using System;
using System.Reflection;
using UnityEditor.PackageManager;

namespace MCPForUnity.Editor.Helpers.Compat
{
    public static class PackageCompat
    {
        public static PackageInfo[] GetAllRegisteredPackages()
        {
#if UNITY_2021_1_OR_NEWER
            return PackageInfo.GetAllRegisteredPackages();
#else
            // 2020.3: the public API does not exist; the internal static PackageInfo.GetAll()
            // returns the same data.
            var mi = typeof(PackageInfo).GetMethod("GetAll", BindingFlags.NonPublic | BindingFlags.Static);
            if (mi != null && mi.Invoke(null, null) is PackageInfo[] all)
            {
                return all;
            }
            return Array.Empty<PackageInfo>();
#endif
        }
    }
}

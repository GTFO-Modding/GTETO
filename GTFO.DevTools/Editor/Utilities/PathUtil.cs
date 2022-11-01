using System;
using System.IO;
using UnityEngine;

namespace GTFO.DevTools.Utilities
{
    public static class PathUtil
    {
        /// <summary>
        /// Ensures a specific directory path exists, creating new directories if needed.
        /// </summary>
        /// <param name="path">The path to ensure exists.</param>
        public static void EnsureDirectoryExists(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            EnsureDirectoryExists(Path.GetDirectoryName(path));

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public static string FullPathToAssetPath(string fullPath)
            => FullPathToAssetPath(fullPath, Application.dataPath);
        public static string FullPathToAssetPath(string fullPath, string assetsFolder)
        {
            return NiceRelativePath(Path.GetDirectoryName(assetsFolder), fullPath)
                .Replace('\\', '/')
                .Substring(1);
        }

        public static string AssetPathToFullPath(string assetPath)
            => AssetPathToFullPath(assetPath, Application.dataPath);
        public static string AssetPathToFullPath(string assetPath, string assetsFolder)
        {
            return Path.Combine(Path.GetDirectoryName(assetsFolder), assetPath.Replace('/', '\\'));
        }

        public static string NiceRelativePath(string relativeTo, string path)
        {
            path = path.Replace(Path.DirectorySeparatorChar, '/');
            string relativePath = Path.GetRelativePath(relativeTo.Replace('/', Path.DirectorySeparatorChar), path.EndsWith('/') ? path : (path + "/")).Replace(Path.DirectorySeparatorChar, '/');
            if (relativePath == ".")
            {
                relativePath = "";
            }
            relativePath = "/" + relativePath;
            if (relativePath.EndsWith("/"))
            {
                relativePath = relativePath.Substring(0, relativePath.Length - 1);
            }
            return relativePath;
        }
    }
}
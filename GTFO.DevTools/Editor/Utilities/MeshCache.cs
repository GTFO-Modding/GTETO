using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GTFO.DevTools.Utilities
{
    public static class MeshCache
    {
        private static readonly Dictionary<string, Mesh> s_cache = new Dictionary<string, Mesh>();

        public static Mesh Fetch(string baseMeshPath, bool force = false)
        {
            string meshPath = "Assets/Resources/" + baseMeshPath + ".prefab";
            if (!s_cache.TryGetValue(meshPath, out var mesh) || (mesh == null && force)) // fetch from dictionary cache
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(meshPath);
                if (obj.TryGetComponent(out MeshFilter meshFilter))
                {
                    mesh = meshFilter.sharedMesh;
                }
                else if (!force)
                {
                    mesh = null;
                }
                else
                {
                    throw new KeyNotFoundException("Failed to find mesh");
                }

                if (s_cache.ContainsKey(meshPath))
                    s_cache[meshPath] = mesh;
                else
                    s_cache.Add(meshPath, mesh);
            }
            return mesh;
        }

        public static void Clear() => s_cache.Clear();
    }
}

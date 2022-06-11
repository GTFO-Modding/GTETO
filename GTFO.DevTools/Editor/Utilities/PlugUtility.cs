using Expedition;
using GTFO.DevTools.Persistent;
using GTFO.DevTools.Plugs;
using LevelGeneration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GTFO.DevTools.Utilities
{
    [InitializeOnLoad]
    public static class PlugUtility
    {
        static PlugUtility()
        {
            PreviewUtility.DoPreview += Preview;
            PreviewUtility.DoClearPreview += ClearPreview;
        }

        public static void Preview(GameObject obj)
        {
            foreach (var plug in obj.GetComponentsInChildren<LG_Plug>())
            {
                Preview(plug);
            }
        }

        public static void Preview(LG_Plug plug, PlugType type = PlugType.Any)
        {
            var random = new System.Random();
            var prefabs = GetPlugPrefabs(type, new LG_Plug[1] { plug });
            if (prefabs.Length == 0) return;

            string prefab = prefabs[random.Next(prefabs.Length)];
            string fileName = Path.GetFileName(prefab);
            string actualPath = $"Assets/PrefabInstance/{fileName}";

            PreviewPlug(plug, actualPath);
        }

        public static void PreviewPlug(LG_Plug plug, string prefab)
        {
            var thePrefabUwU = AssetDatabase.LoadAssetAtPath<GameObject>(prefab);
            if (!thePrefabUwU)
            {
                Debug.LogWarning("Failed to load prefab at path " + prefab);
                return;
            }

            ClearPreview(plug);

            var obj = PreviewUtility.Instantiate(thePrefabUwU, plug.transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;

            PrefabSpawnerUtility.BuildPrefabSpawners(obj);
            MarkerUtility.SpawnRandomMarkers(obj);
        }

        public static void ClearPreview(GameObject obj)
        {
            foreach (var plug in obj.GetComponentsInChildren<LG_Plug>())
                ClearPreview(plug);
        }

        public static void ClearPreview(LG_Plug plug)
        {
            if (plug.transform.childCount > 0)
            {
                GameObject.DestroyImmediate(plug.transform.GetChild(0).gameObject);
            }
        }

        public static string[] GetPlugPrefabs(PlugType plugType, LG_Plug[] plugs)
        {
            var cache = CachedComplexResourceSet.Instance;
            IEnumerable<string> prefabs = new string[0];
            if (plugType == PlugType.Any || plugType == PlugType.StraightNoGate)
            {
                prefabs = ConcatPrefabs(cache.StraightPlugsNoGates, prefabs, plugs);
            }
            if (plugType == PlugType.Any || plugType == PlugType.StraightWithGate)
            {
                prefabs = ConcatPrefabs(cache.StraightPlugsWithGates, prefabs, plugs);
            }
            if (plugType == PlugType.Any || plugType == PlugType.SingleDropNoGate)
            {
                prefabs = ConcatPrefabs(cache.SingleDropPlugsNoGates, prefabs, plugs);
            }
            if (plugType == PlugType.Any || plugType == PlugType.SingleDropWithGate)
            {
                prefabs = ConcatPrefabs(cache.SingleDropPlugsWithGates, prefabs, plugs);
            }
            if (plugType == PlugType.Any || plugType == PlugType.DoubleDropNoGate)
            {
                prefabs = ConcatPrefabs(cache.DoubleDropPlugsNoGates, prefabs, plugs);
            }
            if (plugType == PlugType.Any || plugType == PlugType.DoubleDropWithGate)
            {
                prefabs = ConcatPrefabs(cache.DoubleDropPlugsWithGates, prefabs, plugs);
            }
            if (plugType == PlugType.Any || plugType == PlugType.PlugCap)
            {
                prefabs = ConcatPrefabs(cache.PlugCaps, prefabs, plugs);
            }
            var set = new HashSet<string>(prefabs);
            return set.ToArray();
        }
        private static IEnumerable<string> ConcatPrefabs(CachedComplexResourceSet.CachedResourceData cache, IEnumerable<string> currentPrefabs, LG_Plug[] plugs)
        {
            var subcomplexes = new List<SubComplex>();
            foreach (var plug in plugs)
            {
                if (!subcomplexes.Contains(plug.m_subComplex))
                {
                    currentPrefabs = currentPrefabs.Concat(cache.GetPrefabs(plug.m_subComplex));
                    subcomplexes.Add(plug.m_subComplex);
                }
            }
            return currentPrefabs;
        }
    }
}

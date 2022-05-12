using Expedition;
using GTFO.DevTools.Gates;
using GTFO.DevTools.Persistent;
using LevelGeneration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GTFO.DevTools.Utilities
{
    [InitializeOnLoad]
    public static class GateUtility
    {
        static GateUtility()
        {
            PreviewUtility.DoPreview += Preview;
            PreviewUtility.DoClearPreview += ClearPreview;
        }

        public static void Preview(GameObject obj)
        {
            foreach (var gate in obj.GetComponentsInChildren<LG_InternalGate>())
                Preview(gate);
        }

        public static void ClearPreview(GameObject obj)
        {
            foreach (var gate in obj.GetComponentsInChildren<LG_InternalGate>())
                ClearPreview(gate);
        }

        public static void Preview(LG_InternalGate gate, GateType type = GateType.Any)
        {
            var random = new System.Random();
            var prefabs = GetGatePrefabs(type, new LG_InternalGate[1] { gate });
            if (prefabs.Length == 0) return;

            string prefab = prefabs[random.Next(prefabs.Length)];
            string fileName = Path.GetFileName(prefab);
            string actualPath = $"Assets/PrefabInstance/{fileName}";

            PreviewGate(gate, actualPath);
        }

        public static void PreviewGate(LG_InternalGate gate, string prefab)
        {
            var thePrefabUwU = AssetDatabase.LoadAssetAtPath<GameObject>(prefab);
            if (!thePrefabUwU)
            {
                Debug.LogWarning("Failed to load prefab at path " + prefab);
                return;
            }

            ClearPreview(gate);

            var obj = PreviewUtility.Instantiate(thePrefabUwU, gate.transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;

            PrefabSpawnerUtility.BuildPrefabSpawners(obj);
            MarkerUtility.SpawnRandomMarkers(obj);
        }

        public static void ClearPreview(LG_InternalGate gate)
        {
            if (gate.transform.childCount > 0)
            {
                GameObject.DestroyImmediate(gate.transform.GetChild(0).gameObject);
            }
        }

        public static string[] GetGatePrefabs(GateType gateType, LG_InternalGate[] gates)
        {
            var cache = CachedComplexResourceSet.Instance;
            IEnumerable<string> prefabs = new string[0];
            if (gateType == GateType.Any || gateType == GateType.ApexGate)
            {
                prefabs = ConcatPrefabs(cache.SmallApexGates, cache.MediumApexGates, cache.LargeApexGates, prefabs, gates);
            }
            if (gateType == GateType.Any || gateType == GateType.SecurityGate)
            {
                prefabs = ConcatPrefabs(cache.SmallSecurityGates, cache.MediumSecurityGates, cache.LargeSecurityGates, prefabs, gates);
            }
            if (gateType == GateType.Any || gateType == GateType.BulkheadGate)
            {
                prefabs = ConcatPrefabs(cache.SmallBulkheadGates, cache.MediumBulkheadGates, cache.LargeBulkheadGates, prefabs, gates);
            }
            if (gateType == GateType.Any || gateType == GateType.MainPathBulkheadGate)
            {
                prefabs = ConcatPrefabs(cache.SmallMainPathBulkheadGates, cache.MediumMainPathBulkheadGates, cache.LargeMainPathBulkheadGates, prefabs, gates);
            }
            if (gateType == GateType.Any || gateType == GateType.WeakGate)
            {
                prefabs = ConcatPrefabs(cache.SmallWeakGates, cache.MediumWeakGates, cache.LargeWeakGates, prefabs, gates);
            }
            if (gateType == GateType.Any || gateType == GateType.WallCap)
            {
                prefabs = ConcatPrefabs(cache.SmallWallCaps, cache.MediumWallCaps, cache.LargeWallCaps, prefabs, gates);
            }
            if (gateType == GateType.Any || gateType == GateType.DestroyedCap)
            {
                prefabs = ConcatPrefabs(cache.SmallDestroyedCaps, cache.MediumDestroyedCaps, cache.LargeDestroyedCaps, prefabs, gates);
            }
            if (gateType == GateType.Any || gateType == GateType.WallAndDestroyedCap)
            {
                prefabs = ConcatPrefabs(cache.SmallWallAndDestroyedCaps, cache.MediumWallAndDestroyedCaps, cache.LargeWallAndDestroyedCaps, prefabs, gates);
            }
            var set = new HashSet<string>(prefabs);
            return set.ToArray();
        }
        private static IEnumerable<string> ConcatPrefabs(CachedComplexResourceSet.CachedResourceData small, CachedComplexResourceSet.CachedResourceData medium, CachedComplexResourceSet.CachedResourceData large, IEnumerable<string> currentPrefabs, LG_InternalGate[] gates)
        {
            var gateTypes = new List<LG_GateType>();
            foreach (var gate in gates)
            {
                if (!gateTypes.Contains(gate.m_type))
                {
                    switch (gate.m_type)
                    {
                        case LG_GateType.Small:
                            currentPrefabs = ConcatPrefabs(small, currentPrefabs, gates);
                            break;
                        case LG_GateType.Medium:
                            currentPrefabs = ConcatPrefabs(medium, currentPrefabs, gates);
                            break;
                        case LG_GateType.Large:
                            currentPrefabs = ConcatPrefabs(large, currentPrefabs, gates);
                            break;
                    }
                    gateTypes.Add(gate.m_type);
                }
            }
            return currentPrefabs;
        }
        private static IEnumerable<string> ConcatPrefabs(CachedComplexResourceSet.CachedResourceData cache, IEnumerable<string> currentPrefabs, LG_InternalGate[] gates)
        {
            var subcomplexes = new List<SubComplex>();
            foreach (var gate in gates)
            {
                if (!subcomplexes.Contains(gate.m_subComplex))
                {
                    currentPrefabs = currentPrefabs.Concat(cache.GetPrefabs(gate.m_subComplex));
                    subcomplexes.Add(gate.m_subComplex);
                }
            }
            return currentPrefabs;
        }
    }
}

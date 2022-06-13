using CullingSystem;
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
        public static GameObject CreatePlug(Transform parent, PlugHeight height, PlugElevation elevation, PlugPassageType passage, SubComplex subcomplex, int increment)
        {
            string name = "env_plug_";
            switch (height)
            {
                case PlugHeight.height_8m:
                    name += "8mheight_";
                    break;
            }


            if (passage != PlugPassageType.Cap)
            {
                name += elevation.ToString().ToLower() + "_";
            }


            switch (passage)
            {
                case PlugPassageType.SmallGate:
                case PlugPassageType.MediumGate:
                case PlugPassageType.SmallGateAlign:
                case PlugPassageType.MediumGateAlign:
                    name += "with_gate_";
                    break;
                case PlugPassageType.Cap:
                    name += "cap_";
                    break;
            }

            name += subcomplex.ToString().ToLower() + "_";
            name += increment.ToString().PadLeft(2, '0');

            return CreatePlug(parent, name, passage, subcomplex);
        }
        public static GameObject CreatePlug(Transform parent, string name, PlugPassageType passage, SubComplex subcomplex)
        {
            GameObject plugObj = new GameObject(name);
            plugObj.transform.SetParent(parent);

            if (passage == PlugPassageType.Free)
            {
                LG_FreePassageGate gate = plugObj.AddComponent<LG_FreePassageGate>();
                gate.m_subComplex = subcomplex;
            }

            GameObject crossingObj = null;
            GameObject infrontObj = null;

            GameObject behindObj = new GameObject("behind");
            behindObj.transform.SetParent(plugObj.transform);

            if (passage != PlugPassageType.Cap)
            {
                crossingObj = new GameObject("crossing");
                crossingObj.transform.SetParent(plugObj.transform);

                if (passage.IsGate())
                {
                    LG_InternalGate gate = null; 
                    if (passage.IsGateAlign())
                    {
                        GameObject plugGateAlignObj = new GameObject("PlugDoorAlign");
                        plugGateAlignObj.transform.SetParent(crossingObj.transform);
                        gate = plugGateAlignObj.AddComponent<LG_PlugDoorAlign>();
                    }

                    gate.m_subComplex = subcomplex;
                    if (passage.IsSmallGate())
                    {
                        gate.m_type = LG_GateType.Small;
                    }
                    if (passage.IsMediumGate())
                    {
                        gate.m_type = LG_GateType.Medium;
                    }
                }
                else
                {
                    GameObject portalHelperObj = new GameObject("Portal Helper");
                    portalHelperObj.transform.SetParent(crossingObj.transform);
                    portalHelperObj.AddComponent<C_PortalHelper>();
                }

                

                infrontObj = new GameObject("infront");
                infrontObj.transform.SetParent(plugObj.transform);
            }

            LG_PortalDivider portalDivider = plugObj.AddComponent<LG_PortalDivider>();
            portalDivider.m_behind = behindObj.transform;
            portalDivider.m_crossing = crossingObj == null ? null : crossingObj.transform;
            portalDivider.m_inFront = infrontObj == null ? null : infrontObj.transform;

            return plugObj;
        }
    }
}

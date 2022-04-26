using Expedition;
using GTFO.DevTools.Persistent;
using GTFO.DevTools.Utilities;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GTFO.DevTools.Plugs
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LG_Plug))]
    public class PlugInspector : Editor
    {
        private PlugType m_plugType;
        private PlugView m_plugView;
        private string m_search = "";
        private Vector2 m_scrollPosition;
        private string[] m_prefabs = Array.Empty<string>();

        public override void OnInspectorGUI()
        {
            switch (this.m_plugView)
            {
                case PlugView.Default:
                    base.OnInspectorGUI();
                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Preview"))
                    {
                        this.m_plugView = PlugView.BuildPlugType;
                    }
                    if (GUILayout.Button("Clear Preview"))
                    {
                        this.ClearPreview();
                    }
                    EditorGUILayout.EndHorizontal();
                    break;
                case PlugView.BuildPlugType:

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Preview for Plug Type", EditorStyles.whiteLargeLabel);
                    if (GUILayout.Button("Back", GUILayout.ExpandWidth(false)))
                    {
                        this.m_plugView = PlugView.Default;
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("Flat");
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("With Gate"))
                    {
                        this.m_plugType = PlugType.StraightWithGate;
                        this.m_plugView = PlugView.BuildSpecificPlug;
                        this.CachePrefabs();
                    }
                    if (GUILayout.Button("Without Gate"))
                    {
                        this.m_plugType = PlugType.StraightNoGate;
                        this.m_plugView = PlugView.BuildSpecificPlug;
                        this.CachePrefabs();
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.LabelField("Single Drop");
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("With Gate"))
                    {
                        this.m_plugType = PlugType.SingleDropWithGate;
                        this.m_plugView = PlugView.BuildSpecificPlug;
                        this.CachePrefabs();
                    }
                    if (GUILayout.Button("Without Gate"))
                    {
                        this.m_plugType = PlugType.SingleDropNoGate;
                        this.m_plugView = PlugView.BuildSpecificPlug;
                        this.CachePrefabs();
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.LabelField("Double Drop");
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("With Gate"))
                    {
                        this.m_plugType = PlugType.DoubleDropWithGate;
                        this.m_plugView = PlugView.BuildSpecificPlug;
                        this.CachePrefabs();
                    }
                    if (GUILayout.Button("Without Gate"))
                    {
                        this.m_plugType = PlugType.DoubleDropNoGate;
                        this.m_plugView = PlugView.BuildSpecificPlug;
                        this.CachePrefabs();
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Plug Cap"))
                    {
                        this.m_plugType = PlugType.PlugCap;
                        this.m_plugView = PlugView.BuildSpecificPlug;
                        this.CachePrefabs();
                    }
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Any"))
                    {
                        this.m_plugType = PlugType.Any;
                        this.m_plugView = PlugView.BuildSpecificPlug;
                        this.CachePrefabs();
                    }
                    EditorGUILayout.Space();
                    break;
                case PlugView.BuildSpecificPlug:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Preview Plug", EditorStyles.whiteLargeLabel);
                    if (GUILayout.Button("Back", GUILayout.ExpandWidth(false)))
                    {
                        this.m_plugView = PlugView.BuildPlugType;
                        break;
                    }
                    EditorGUILayout.EndHorizontal();

                    this.m_search = EditorGUILayout.TextField("Search", this.m_search);
                    this.m_scrollPosition = EditorGUILayout.BeginScrollView(this.m_scrollPosition);
                    foreach (var prefab in this.m_prefabs)
                    {
                        string fileName = Path.GetFileName(prefab);
                        string actualPath = $"Assets/PrefabInstance/{fileName}";

                        if (!string.IsNullOrEmpty(this.m_search) && !fileName.Contains(this.m_search))
                            continue;

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(fileName);
                        if (GUILayout.Button("Preview", GUILayout.ExpandWidth(false)))
                        {
                            this.PreviewPlug(actualPath);
                            this.m_search = "";
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndScrollView();
                    
                    break;
            }
        }

        private void PreviewPlug(string prefab)
        {
            var thePrefabUwU = AssetDatabase.LoadAssetAtPath<GameObject>(prefab);
            if (!thePrefabUwU)
            {
                Debug.LogWarning("Failed to load prefab at path " + prefab);
                return;
            }

            this.ClearPreview();

            foreach (var target in this.targets)
            {
                var plug = (LG_Plug)target;

                var obj = GameObject.Instantiate(thePrefabUwU, plug.transform);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;
                obj.transform.localScale = Vector3.one;

                PrefabSpawnerUtility.BuildPrefabSpawners(obj);
                MarkerUtility.SpawnRandomMarkers(obj);
            }
        }

        private void CachePrefabs()
        {
            var cache = CachedComplexResourceSet.Instance;
            IEnumerable<string> prefabs = new string[0];
            if (this.m_plugType == PlugType.Any || this.m_plugType == PlugType.StraightNoGate)
            {
                prefabs = this.ConcatCache(cache.StraightPlugsNoGates, prefabs);
            }
            if (this.m_plugType == PlugType.Any || this.m_plugType == PlugType.StraightWithGate)
            {
                prefabs = this.ConcatCache(cache.StraightPlugsWithGates, prefabs);
            }
            if (this.m_plugType == PlugType.Any || this.m_plugType == PlugType.SingleDropNoGate)
            {
                prefabs = this.ConcatCache(cache.SingleDropPlugsNoGates, prefabs);
            }
            if (this.m_plugType == PlugType.Any || this.m_plugType == PlugType.SingleDropWithGate)
            {
                prefabs = this.ConcatCache(cache.SingleDropPlugsWithGates, prefabs);
            }
            if (this.m_plugType == PlugType.Any || this.m_plugType == PlugType.DoubleDropNoGate)
            {
                prefabs = this.ConcatCache(cache.DoubleDropPlugsNoGates, prefabs);
            }
            if (this.m_plugType == PlugType.Any || this.m_plugType == PlugType.DoubleDropWithGate)
            {
                prefabs = this.ConcatCache(cache.DoubleDropPlugsWithGates, prefabs);
            }
            if (this.m_plugType == PlugType.Any || this.m_plugType == PlugType.PlugCap)
            {
                prefabs = this.ConcatCache(cache.PlugCaps, prefabs);
            }
            var set = new HashSet<string>(prefabs);
            this.m_prefabs = set.ToArray();
        }
        private IEnumerable<string> ConcatCache(CachedComplexResourceSet.CachedResourceData cache, IEnumerable<string> currentPrefabs)
        {
            var subcomplexes = new List<SubComplex>();
            foreach (var target in this.targets)
            {
                var plug = (LG_Plug)target;
                if (!subcomplexes.Contains(plug.m_subComplex))
                {
                    currentPrefabs = currentPrefabs.Concat(cache.GetPrefabs(plug.m_subComplex));
                    subcomplexes.Add(plug.m_subComplex);
                }
            }
            return currentPrefabs;
        }

        private void ClearPreview()
        {
            foreach (var target in this.targets)
                this.ClearPreview((LG_Plug)target);
        }
        private void ClearPreview(LG_Plug plug)
        {
            if (plug.transform.childCount > 0)
            {
                DestroyImmediate(plug.transform.GetChild(0).gameObject);
            }
        }

        private enum PlugType
        {
            Any,
            StraightNoGate,
            StraightWithGate,
            SingleDropNoGate,
            SingleDropWithGate,
            DoubleDropNoGate,
            DoubleDropWithGate,
            PlugCap
        }

        private enum PlugView
        {
            Default,
            BuildPlugType,
            BuildSpecificPlug
        }

    }
}

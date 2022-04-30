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

namespace GTFO.DevTools.Gates
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LG_InternalGate), true)]
    public class LG_GateInspector : Editor
    {
        private GateType m_gateType;
        private GateView m_gateView;
        private string m_search = "";
        private Vector2 m_scrollPosition;
        private string[] m_prefabs = Array.Empty<string>();

        public override void OnInspectorGUI()
        {
            switch (this.m_gateView)
            {
                case GateView.Default:
                    base.OnInspectorGUI();
                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Preview"))
                    {
                        this.m_gateView = GateView.BuildGateType;
                    }
                    if (GUILayout.Button("Clear Preview"))
                    {
                        this.ClearPreview();
                    }
                    EditorGUILayout.EndHorizontal();
                    break;
                case GateView.BuildGateType:

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Preview for Gate Type", EditorStyles.whiteLargeLabel);
                    if (GUILayout.Button("Back", GUILayout.ExpandWidth(false)))
                    {
                        this.m_gateView = GateView.Default;
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Security Gate"))
                    {
                        this.m_gateType = GateType.SecurityGate;
                        this.m_gateView = GateView.BuildSpecificGate;
                        this.CachePrefabs();
                    }
                    if (GUILayout.Button("Apex Gate"))
                    {
                        this.m_gateType = GateType.ApexGate;
                        this.m_gateView = GateView.BuildSpecificGate;
                        this.CachePrefabs();
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Bulkhead Gate"))
                    {
                        this.m_gateType = GateType.BulkheadGate;
                        this.m_gateView = GateView.BuildSpecificGate;
                        this.CachePrefabs();
                    }
                    if (GUILayout.Button("Main Bulkhead Gate"))
                    {
                        this.m_gateType = GateType.MainPathBulkheadGate;
                        this.m_gateView = GateView.BuildSpecificGate;
                        this.CachePrefabs();
                    }
                    EditorGUILayout.EndHorizontal();
                    if (GUILayout.Button("Weak Gate"))
                    {
                        this.m_gateType = GateType.WeakGate;
                        this.m_gateView = GateView.BuildSpecificGate;
                        this.CachePrefabs();
                    }
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Wall Cap"))
                    {
                        this.m_gateType = GateType.WallCap;
                        this.m_gateView = GateView.BuildSpecificGate;
                        this.CachePrefabs();
                    }
                    if (GUILayout.Button("Destroyed Cap"))
                    {
                        this.m_gateType = GateType.DestroyedCap;
                        this.m_gateView = GateView.BuildSpecificGate;
                        this.CachePrefabs();
                    }
                    if (GUILayout.Button("Wall and Destroyed Cap"))
                    {
                        this.m_gateType = GateType.WallAndDestroyedCap;
                        this.m_gateView = GateView.BuildSpecificGate;
                        this.CachePrefabs();
                    }
                    EditorGUILayout.Space();
                    if (GUILayout.Button("All"))
                    {
                        this.m_gateType = GateType.Any;
                        this.m_gateView = GateView.BuildSpecificGate;
                        this.CachePrefabs();
                    }
                    EditorGUILayout.Space();
                    break;
                case GateView.BuildSpecificGate:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Preview Gate", EditorStyles.whiteLargeLabel);
                    if (GUILayout.Button("Back", GUILayout.ExpandWidth(false)))
                    {
                        this.m_gateView = GateView.BuildGateType;
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
                            this.PreviewGate(actualPath);
                            this.m_search = "";
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndScrollView();
                    
                    break;
            }
        }

        private void PreviewGate(string prefab)
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
                var gate = (LG_InternalGate)target;

                var obj = PreviewUtility.Instantiate(thePrefabUwU, gate.transform);
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
            if (this.m_gateType == GateType.Any || this.m_gateType == GateType.ApexGate)
            {
                prefabs = this.ConcatCache(cache.SmallApexGates, cache.MediumApexGates, cache.LargeApexGates, prefabs);
            }
            if (this.m_gateType == GateType.Any || this.m_gateType == GateType.SecurityGate)
            {
                prefabs = this.ConcatCache(cache.SmallSecurityGates, cache.MediumSecurityGates, cache.LargeSecurityGates, prefabs);
            }
            if (this.m_gateType == GateType.Any || this.m_gateType == GateType.BulkheadGate)
            {
                prefabs = this.ConcatCache(cache.SmallBulkheadGates, cache.MediumBulkheadGates, cache.LargeBulkheadGates, prefabs);
            }
            if (this.m_gateType == GateType.Any || this.m_gateType == GateType.MainPathBulkheadGate)
            {
                prefabs = this.ConcatCache(cache.SmallMainPathBulkheadGates, cache.MediumMainPathBulkheadGates, cache.LargeMainPathBulkheadGates, prefabs);
            }
            if (this.m_gateType == GateType.Any || this.m_gateType == GateType.WeakGate)
            {
                prefabs = this.ConcatCache(cache.SmallWeakGates, cache.MediumWeakGates, cache.LargeWeakGates, prefabs);
            }
            if (this.m_gateType == GateType.Any || this.m_gateType == GateType.WallCap)
            {
                prefabs = this.ConcatCache(cache.SmallWallCaps, cache.MediumWallCaps, cache.LargeWallCaps, prefabs);
            }
            if (this.m_gateType == GateType.Any || this.m_gateType == GateType.DestroyedCap)
            {
                prefabs = this.ConcatCache(cache.SmallDestroyedCaps, cache.MediumDestroyedCaps, cache.LargeDestroyedCaps, prefabs);
            }
            if (this.m_gateType == GateType.Any || this.m_gateType == GateType.WallAndDestroyedCap)
            {
                prefabs = this.ConcatCache(cache.SmallWallAndDestroyedCaps, cache.MediumWallAndDestroyedCaps, cache.LargeWallAndDestroyedCaps, prefabs);
            }
            var set = new HashSet<string>(prefabs);
            this.m_prefabs = set.ToArray();
        }
        private IEnumerable<string> ConcatCache(CachedComplexResourceSet.CachedResourceData small, CachedComplexResourceSet.CachedResourceData medium, CachedComplexResourceSet.CachedResourceData large, IEnumerable<string> currentPrefabs)
        {
            var gateTypes = new List<LG_GateType>();
            foreach (var target in this.targets)
            {
                var gate = (LG_InternalGate)target;
                if (!gateTypes.Contains(gate.m_type))
                {
                    switch (gate.m_type)
                    {
                        case LG_GateType.Small:
                            currentPrefabs = this.ConcatCache(small, currentPrefabs);
                            break;
                        case LG_GateType.Medium:
                            currentPrefabs = this.ConcatCache(medium, currentPrefabs);
                            break;
                        case LG_GateType.Large:
                            currentPrefabs = this.ConcatCache(large, currentPrefabs);
                            break;
                    }
                    gateTypes.Add(gate.m_type);
                }
            }
            return currentPrefabs;
        }
        private IEnumerable<string> ConcatCache(CachedComplexResourceSet.CachedResourceData cache, IEnumerable<string> currentPrefabs)
        {
            var subcomplexes = new List<SubComplex>();
            foreach (var target in this.targets)
            {
                var gate = (LG_InternalGate)target;
                if (!subcomplexes.Contains(gate.m_subComplex))
                {
                    currentPrefabs = currentPrefabs.Concat(cache.GetPrefabs(gate.m_subComplex));
                    subcomplexes.Add(gate.m_subComplex);
                }
            }
            return currentPrefabs;
        }

        private void ClearPreview()
        {
            foreach (var target in this.targets)
                this.ClearPreview((LG_InternalGate)target);
        }
        private void ClearPreview(LG_InternalGate plug)
        {
            if (plug.transform.childCount > 0)
            {
                DestroyImmediate(plug.transform.GetChild(0).gameObject);
            }
        }

        [DrawGizmo(GizmoType.NotInSelectionHierarchy |
               GizmoType.InSelectionHierarchy |
               GizmoType.Selected |
               GizmoType.Active |
               GizmoType.Pickable)]
        private static void DrawMarkerGizmo(LG_InternalGate gate, GizmoType gizmoType)
        {
            if (!DevToolSettings.Instance.m_showGates) return;
            Transform gateTransform = gate.transform;

            Vector2 size = default;
            switch (gate.m_type)
            {
                case LG_GateType.Small:
                    size = new Vector2(4f, 4f);
                    break;
                case LG_GateType.Medium:
                    size = new Vector2(8f, 4f);
                    break;
                case LG_GateType.Large:
                    size = new Vector2(8f, 8f);
                    break;
                default:
                    return;
            }

            Vector3 position = gateTransform.position;
            Vector2 halfSize = size * 0.5f;

            Vector3 topLF = position + (gateTransform.up * size.y) + (-gateTransform.right * halfSize.x) + (gate.transform.forward * 0.5f);
            Vector3 topRF = position + (gateTransform.up * size.y) + (gateTransform.right * halfSize.x) + (gate.transform.forward * 0.5f);
            Vector3 bottomLF = position + (-gateTransform.right * halfSize.x) + (gate.transform.forward * 0.5f);
            Vector3 bottomRF = position + (gateTransform.right * halfSize.x) + (gate.transform.forward * 0.5f);
            Vector3 topLB = position + (gateTransform.up * size.y) + (-gateTransform.right * halfSize.x) + (-gate.transform.forward * 0.5f);
            Vector3 topRB = position + (gateTransform.up * size.y) + (gateTransform.right * halfSize.x) + (-gate.transform.forward * 0.5f);
            Vector3 bottomLB = position + (-gateTransform.right * halfSize.x) + (-gate.transform.forward * 0.5f);
            Vector3 bottomRB = position + (gateTransform.right * halfSize.x) + (-gate.transform.forward * 0.5f);

            Gizmos.color = new Color(0.75f, 0.75f, 0.75f);
            Gizmos.DrawLine(topLF, topRF);
            Gizmos.DrawLine(topRF, bottomRF);
            Gizmos.DrawLine(bottomRF, bottomLF);
            Gizmos.DrawLine(bottomLF, topLF);
            Gizmos.DrawLine(topLB, topRB);
            Gizmos.DrawLine(topRB, bottomRB);
            Gizmos.DrawLine(bottomRB, bottomLB);
            Gizmos.DrawLine(bottomLB, topLB);
            Gizmos.DrawLine(topLF, topLB);
            Gizmos.DrawLine(topRF, topRB);
            Gizmos.DrawLine(bottomRF, bottomRB);
            Gizmos.DrawLine(bottomLF, bottomLB);

            //if (gizmoType.HasFlag(GizmoType.Selected))
            //{
            //    Gizmos.DrawWireCube
            //    Vector3 center = plug.transform.position - (plug.transform.forward * 4);
            //    Gizmos.DrawWireCube(mesh, marker.transform.position, marker.transform.rotation, marker.transform.localScale);
            //}
            //else
            //{
            //    // dont draw mesh if inside inner marker.
            //    // e.g.
            //    // MarkerA
            //    //  |- Obj (clone)
            //    //      |- MarkerB
            //    //      |- MarkerC
            //    // Marker B is currently selected.
            //    //
            //    // MarkerB should have the wire mesh,
            //    // MarkerA should not draw any mesh (because overlapping)
            //    // MarkerC should be visible.

            //    var producerTrans = marker.transform;

            //    var currentTrans = Selection.activeTransform;
            //    while (currentTrans != null)
            //    {
            //        if (currentTrans == producerTrans)
            //            return;
            //        currentTrans = currentTrans.parent;
            //    }

            //    Gizmos.DrawMesh(mesh, marker.transform.position, marker.transform.rotation, marker.transform.localScale);
            //}
        }

        private enum GateType
        {
            Any,
            WeakGate,
            SecurityGate,
            ApexGate,
            BulkheadGate,
            MainPathBulkheadGate,
            WallCap,
            DestroyedCap,
            WallAndDestroyedCap
        }

        private enum GateView
        {
            Default,
            BuildGateType,
            BuildSpecificGate
        }
    }
}

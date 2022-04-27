using GameData;
using GTFO.DevTools.Extensions;
using GTFO.DevTools.Persistent;
using GTFO.DevTools.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GTFO.DevTools
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LG_MarkerProducer), true)]
    public class MarkerInspector : Editor
    {
        private IMarkerDataBlock m_datablock;

        private static Dictionary<string, Mesh> s_meshes = new Dictionary<string, Mesh>();
        public static void ResetMeshes()
        {
            s_meshes.Clear();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            if (this.targets.Length > 1)
            {
                var producers = this.targets.Select((t) => (LG_MarkerProducer)t).ToArray();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Build Random"))
                {
                    foreach (var marker in producers)
                    {
                        MarkerUtility.SpawnRandomMarkers(marker);
                    }
                }
                if (GUILayout.Button("Clear"))
                {
                    foreach (var marker in producers)
                    {
                        MarkerUtility.CleanupMarker(marker);
                    }
                }
                EditorGUILayout.EndHorizontal();
                return;
            }

            var producer = (LG_MarkerProducer)this.target;

            this.m_datablock = MarkerUtility.GetDataBlockForProducer(producer);

            GUI.enabled = this.m_datablock != null;

            if (this.m_datablock == null) return;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Build Random"))
            {
                MarkerUtility.SpawnRandomMarkers(producer);
            }
            if (GUILayout.Button("Clear"))
            {
                MarkerUtility.CleanupMarker(producer);
            }
            EditorGUILayout.EndHorizontal();

            foreach (var comp in this.m_datablock.GetCommonData().Compositions)
            {
                if (!string.IsNullOrWhiteSpace(comp.prefab))
                {
                    EditorGUILayout.BeginHorizontal();
                    Texture2D texture = null;
                    if (comp.function != ExpeditionFunction.None && !s_functionIconMap.TryGetValue(comp.function, out texture))
                    {
                        string texturePath = "Assets/GTFO.DevTools/Editor/Resources/DevToolMarkers/marker_" + comp.function.ToString().ToLower() + ".png";
                        texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
                        s_functionIconMap.Add(comp.function, texture);
                    }

                    if (texture)
                    {
                        EditorGUILayout.LabelField(new GUIContent(Path.GetFileName(comp.prefab), texture, "Marker Type: " + comp.function.ToString() + "\nWeight: " + comp.weight));
                    }
                    else if (comp.function != ExpeditionFunction.None)
                    {
                        EditorGUILayout.LabelField(new GUIContent($"[{comp.function}] {Path.GetFileName(comp.prefab)}", "Weight: " + comp.weight));
                    }
                    else
                    {
                        EditorGUILayout.LabelField(new GUIContent(Path.GetFileName(comp.prefab), "Marker Type: " + comp.function.ToString() + "\nWeight: " + comp.weight));
                    }
                    if (GUILayout.Button("Build", GUILayout.ExpandWidth(false)))
                    {
                        MarkerUtility.SpawnMarkerComposition(producer, comp);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }


        }

        private static readonly Dictionary<ExpeditionFunction, Texture2D> s_functionIconMap = new Dictionary<ExpeditionFunction, Texture2D>();

        public static void ClearFunctionIconCache()
        {
            s_functionIconMap.Clear();
        }

        private static Color markerCol = new Color(1, 1, 1, 0.5f);
        [DrawGizmo(GizmoType.NotInSelectionHierarchy |
               GizmoType.InSelectionHierarchy |
               GizmoType.Selected |
               GizmoType.Active |
               GizmoType.Pickable)]
        private static void DrawMarkerGizmo(LG_MarkerProducer marker, GizmoType gizmoType)
        {
            if (!DevToolSettings.Instance.m_showMarkers) return;
            IMarkerDataBlock datablock = null;
            switch (marker.MarkerDataBlockType)
            {
                case LG_MarkerDataBlockType.Service:
                    datablock = GTFOGameConfig.Rundown.DataBlocks.ServiceMarker.GetBlockByID(marker.MarkerDataBlockID);
                    break;
                case LG_MarkerDataBlockType.Mining:
                    datablock = GTFOGameConfig.Rundown.DataBlocks.MiningMarker.GetBlockByID(marker.MarkerDataBlockID);
                    break;
                case LG_MarkerDataBlockType.Tech:
                    datablock = GTFOGameConfig.Rundown.DataBlocks.TechMarker.GetBlockByID(marker.MarkerDataBlockID);
                    break;
            }

            if (datablock == null)
            {
                return;
            }

            // get group id from the marker. This isn't publicly visible and static.
            var groupID = (uint)marker.GetType().GetField("groupID", BindingFlags.Static | BindingFlags.NonPublic)
                .GetValue("groupID");
            var groupBlock = GTFOGameConfig.Rundown.DataBlocks.MarkerGroup.GetBlockByID(groupID);
            Color color = markerCol;

            string meshPath = "Assets/Resources/" + datablock.GetCommonData().EditorMesh + ".prefab";
            if (!s_meshes.TryGetValue(meshPath, out var mesh)) // fetch from dictionary cache
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(meshPath);
                if (obj != null && obj.TryGetComponent(out MeshFilter meshFilter))
                {
                    mesh = meshFilter.sharedMesh;
                }
                else
                {
                    mesh = null;
                }
                s_meshes.Add(meshPath, mesh);
            }

            if (mesh == null)
            {
                return;
            }

            Gizmos.color = groupBlock?.MeshColor ?? color;
            if (gizmoType.HasFlag(GizmoType.Selected))
            {
                Gizmos.DrawWireMesh(mesh, marker.transform.position, marker.transform.rotation, marker.transform.localScale);
            }
            else
            {
                // dont draw mesh if inside inner marker.
                // e.g.
                // MarkerA
                //  |- Obj (clone)
                //      |- MarkerB
                //      |- MarkerC
                // Marker B is currently selected.
                //
                // MarkerB should have the wire mesh,
                // MarkerA should not draw any mesh (because overlapping)
                // MarkerC should be visible.

                var producerTrans = marker.transform;

                var currentTrans = Selection.activeTransform;
                while (currentTrans != null)
                {
                    if (currentTrans == producerTrans)
                        return;
                    currentTrans = currentTrans.parent;
                }

                Gizmos.DrawMesh(mesh, marker.transform.position, marker.transform.rotation, marker.transform.localScale);
            }
        }
    }
}

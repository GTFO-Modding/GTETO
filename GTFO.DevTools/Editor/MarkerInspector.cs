using GameData;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GTFO.DevTools
{
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
            var producer = (LG_MarkerProducer)this.target;

            this.m_datablock = GetDataBlockForProducer(producer);

            GUI.enabled = this.m_datablock != null;

            if (this.m_datablock != null)
            {
                EditorGUILayout.Space();
                if (GUILayout.Button("Clear"))
                {
                    CleanupMarker(producer);
                }

                foreach (var comp in this.m_datablock.CommonData.Compositions)
                {
                    if (!string.IsNullOrWhiteSpace(comp.prefab))
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField($"[{comp.function}] {Path.GetFileName(comp.prefab)}");
                        if (GUILayout.Button("Build", GUILayout.ExpandWidth(false)))
                        {
                            CleanupMarker(producer);
                            var actualPath = Path.Combine("Assets/PrefabInstance", Path.GetFileName(comp.prefab));

                            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(actualPath);
                            if (prefab != null)
                            {
                                var copy = Instantiate(prefab, producer.transform);
                                copy.transform.localPosition = Vector3.zero;
                                copy.transform.localRotation = Quaternion.identity;
                                ContextMenuExtensions.BuildPrefabSpawners(copy);
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }

            
        }

        public static IMarkerDataBlock GetDataBlockForProducer(LG_MarkerProducer producer)
        {
            switch (producer.MarkerDataBlockType)
            {
                case LG_MarkerDataBlockType.Service:
                    return GTFOGameConfig.Rundown.DataBlocks.ServiceMarker.GetBlockByID(producer.MarkerDataBlockID);
                case LG_MarkerDataBlockType.Mining:
                    return GTFOGameConfig.Rundown.DataBlocks.MiningMarker.GetBlockByID(producer.MarkerDataBlockID);
                case LG_MarkerDataBlockType.Tech:
                    return GTFOGameConfig.Rundown.DataBlocks.TechMarker.GetBlockByID(producer.MarkerDataBlockID);
            }
            return null;
        }

        public static void SpawnRandomMarkers(GameObject obj)
            => SpawnRandomMarkers(obj, new System.Random());

        public static void CleanupMarker(LG_MarkerProducer producer)
        {
            if (producer != null && producer.transform.childCount > 0)
            {
                DestroyImmediate(producer.transform.GetChild(0).gameObject);
            }
        }

        private static void SpawnRandomMarkerComp(LG_MarkerProducer producer, IMarkerDataBlock block, System.Random random)
        {
            if (producer == null) return;

            CleanupMarker(producer);
            var prefabs = block.CommonData.Compositions
                .Where((c) => c.function != ExpeditionFunction.Strongbox && c.function != ExpeditionFunction.ResourceContainerSecure)
                .Select((c) => string.IsNullOrEmpty(c.prefab) ? null : Path.Combine("Assets/PrefabInstance", Path.GetFileName(c.prefab)))
                .Select((p) => p == null ? null : AssetDatabase.LoadAssetAtPath<GameObject>(p))
                .Where((asset) => asset != null)
                .ToArray();

            if (prefabs.Length == 0)
                return;


            var prefab = prefabs[random.Next(prefabs.Length)];
            if (prefab == null)
                return;

            var copy = Instantiate(prefab, producer.transform);
            copy.transform.localPosition = Vector3.zero;
            copy.transform.localRotation = Quaternion.identity;
            ContextMenuExtensions.BuildPrefabSpawners(copy);

            for (int childIndex = 0, childCount = copy.transform.childCount; childIndex < childCount; childIndex++)
            {
                SpawnRandomMarkers(copy.transform.GetChild(childIndex).gameObject, random);
            }
        }

        public static void SpawnRandomMarkers(GameObject obj, System.Random random)
        {
            foreach (var marker in obj.GetComponentsInChildren<LG_MarkerProducer>())
            {
                var block = GetDataBlockForProducer(marker);
                if (block != null)
                    SpawnRandomMarkerComp(marker, GetDataBlockForProducer(marker), random);
            }
        }

        private static Color markerCol = new Color(1, 1, 1, 0.5f);
        [DrawGizmo(GizmoType.NotInSelectionHierarchy |
               GizmoType.InSelectionHierarchy |
               GizmoType.Selected |
               GizmoType.Active |
               GizmoType.Pickable)]
        private static void DrawMarkerGizmo(LG_MarkerProducer marker, GizmoType gizmoType)
        {
            if (!GeomorphToolWindow.DrawMarkers) return;
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

            string meshPath = "Assets/Resources/" + datablock.CommonData.EditorMesh + ".prefab";
            if (!s_meshes.TryGetValue(meshPath, out var mesh)) // fetch from dictionary cache
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(meshPath);
                if (obj.TryGetComponent(out MeshFilter meshFilter))
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

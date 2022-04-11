

using LevelGeneration;
using UnityEditor;
using UnityEngine;

namespace GTFO.DevTools
{
    [CustomEditor(typeof(LG_Plug), true)]
    public class PlugInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }

        private static readonly Color BASE_COLOR = new Color(1f, 1f, 1f, 0.2f);
        private static readonly Color BASE_OUTLINE_COLOR = new Color(1f, 1f, 1f, 1f);
        private static readonly Color FLOOR_COLOR = new Color(0.5f, 0.5f, 1f, 0.3f);
        private static readonly Color WALL_COLOR = new Color(1f, 0.5f, 0.5f, 0.8f);

        [DrawGizmo(GizmoType.NotInSelectionHierarchy |
               GizmoType.InSelectionHierarchy |
               GizmoType.Selected |
               GizmoType.Active |
               GizmoType.Pickable)]
        private static void DrawGizmo(LG_Plug marker, GizmoType gizmoType)
        {
            if (!GeomorphToolWindow.DrawMarkers) return;
            Gizmos.color = BASE_COLOR;
            Vector3 center = marker.transform.position - (marker.transform.forward * 4);
            Gizmos.DrawCube(center, marker.transform.rotation * new Vector3(16f, 16f, 8f));
            Gizmos.color = BASE_OUTLINE_COLOR;
            Gizmos.DrawWireCube(center, marker.transform.rotation * new Vector3(16f, 16f, 8f));
            Gizmos.color = FLOOR_COLOR;
            Gizmos.DrawCube(center, marker.transform.rotation * new Vector3(16f, 0.01f, 8f));
            Gizmos.color = WALL_COLOR;
            Gizmos.DrawCube(marker.transform.position, marker.transform.rotation * new Vector3(16f, 16f, 0.01f));
        }
    }
}

using GTFO.DevTools.Persistent;
using LevelGeneration;
using UnityEditor;
using UnityEngine;

namespace GTFO.DevTools
{
    public class GeomorphInspector
    {

        private static readonly Color BOUNDS_COLOR = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        private static readonly Color BOUNDS_INNER_COLOR = new Color(0f, 0f, 0f, 0.05f);
        private static readonly Color SELECTED_BOUNDS_COLOR = new Color(0.5f, 0.5f, 0.5f);
        private static readonly Color SELECTED_BOUNDS_INNER_COLOR = new Color(0f, 0f, 0f, 0.1f);
        private static readonly Color BASE_COLOR = new Color(1f, 1f, 1f, 0.2f);
        private static readonly Color BASE_OUTLINE_COLOR = new Color(1f, 1f, 1f, 1f);
        private static readonly Color FLOOR_COLOR = new Color(0.5f, 0.5f, 1f, 0.3f);
        private static readonly Color WALL_COLOR = new Color(1f, 0.5f, 0.5f, 0.8f);

        [DrawGizmo(GizmoType.NotInSelectionHierarchy |
               GizmoType.InSelectionHierarchy |
               GizmoType.Selected |
               GizmoType.Active |
               GizmoType.Pickable)]
        private static void DrawGeoGizmo(LG_Geomorph geomorph, GizmoType gizmoType)
        {
            bool geoSelected = (gizmoType & GizmoType.Selected) == GizmoType.Selected;
            if (geomorph.m_drawBounds || DevToolSettings.Instance.m_showGeoBounds)
            {
                Gizmos.color = geoSelected ? SELECTED_BOUNDS_INNER_COLOR : BOUNDS_INNER_COLOR;

                Vector3 size = default;
                switch (geomorph.m_goShapeType)
                {
                    case LG_GeomorphShapeType.devLevel:
                        size = new Vector3(256f, 64f, 256f);
                        break;
                    case LG_GeomorphShapeType.s2x2:
                        size = new Vector3(128f, 64f, 128f);
                        break;
                    case LG_GeomorphShapeType.s2x1:
                        size = new Vector3(64f, 64f, 128f);
                        break;
                    case LG_GeomorphShapeType.s1x1:
                        size = new Vector3(64f, 64f, 64f);
                        break;
                }

                //Vector3 size = new Vector3(((int)geomorph.m_goShapeType + 1) * 64f, 64f, (Mathf.Max(0, (int)geomorph.m_goShapeType - 1) + 2) * 64f);

                Gizmos.DrawCube(geomorph.transform.position, size);

                Gizmos.color = geoSelected ? SELECTED_BOUNDS_COLOR : BOUNDS_COLOR;
                Gizmos.DrawWireCube(geomorph.transform.position, size);
            }

            if (geomorph.m_drawPlugs || DevToolSettings.Instance.m_showPlugs)
            {
                foreach (var plug in geomorph.GetComponentsInChildren<LG_Plug>())
                {
                    Gizmos.color = BASE_COLOR;
                    Vector3 center = plug.transform.position - (plug.transform.forward * 4);
                    Gizmos.DrawCube(center, plug.transform.rotation * new Vector3(16f, 16f, 8f));
                    Gizmos.color = BASE_OUTLINE_COLOR;
                    Gizmos.DrawWireCube(center, plug.transform.rotation * new Vector3(16f, 16f, 8f));
                    Gizmos.color = FLOOR_COLOR;
                    Gizmos.DrawCube(center, plug.transform.rotation * new Vector3(16f, 0.01f, 8f));
                    Gizmos.color = WALL_COLOR;
                    Gizmos.DrawCube(plug.transform.position, plug.transform.rotation * new Vector3(16f, 16f, 0.01f));
                }
            }
        }
    }
}

using CullingSystem;
using UnityEditor;
using UnityEngine;

namespace GTFO.DevTools.Culling
{
    public class C_PortalHelperInspector
    {
        [DrawGizmo(GizmoType.Selected)]
        private static void DrawPortalHelperGizmo(C_PortalHelper cullingPortalHelper)
        {
            Color color = Gizmos.color;
            Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
            Matrix4x4 matrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(cullingPortalHelper.transform.position + (Vector3.up * cullingPortalHelper.transform.localScale.y), cullingPortalHelper.transform.rotation, cullingPortalHelper.transform.localScale);
            Gizmos.DrawCube(Vector3.zero, Vector3.one);
            Gizmos.matrix = matrix;
            Gizmos.color = color;
        }
    }
}
using GTFO.DevTools.Persistent;
using UnityEditor;
using UnityEngine;

namespace GTFO.DevTools.Geo
{
    public class SettingsComponent : GeoEditorComponent
    {
        public override bool NeedStyleRefresh => !Styles.HAS_GUI_CONSTANTS;
        public override void RefreshStyle()
        {
            Styles.RefreshGUIConstants();
        }

        protected override void OnHeaderGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(Styles.HEADER_TITLE, EditorStyles.whiteLargeLabel);
            if (GUILayout.Button(Styles.HEADER_VIEW_BUTTON_LABEL, GUILayout.ExpandWidth(false)))
            {
                EditorGUIUtility.PingObject(DevToolSettings.Instance);
            }
            if (GUILayout.Button(Styles.HEADER_BACK_BUTTON_LABEL, GUILayout.ExpandWidth(false)))
            {
                this.Reset();
                this.Window.ChangeToView(GeoToolView.Default);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        protected override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            DevToolSettings.Instance.m_rundownPath = EditorGUILayout.TextField(Styles.RUNDOWN_PATH_LABEL, DevToolSettings.Instance.m_rundownPath);
            DevToolSettings.Instance.m_authorInitials = EditorGUILayout.TextField(Styles.AUTHOR_INITIALS_LABEL, DevToolSettings.Instance.m_authorInitials);
            DevToolSettings.Instance.m_showMarkers = EditorGUILayout.Toggle(Styles.DRAW_MARKERS_LABEL, DevToolSettings.Instance.m_showMarkers);
            DevToolSettings.Instance.m_showGates = EditorGUILayout.Toggle(Styles.DRAW_GATES_LABEL, DevToolSettings.Instance.m_showGates);
            DevToolSettings.Instance.m_showPlugs = EditorGUILayout.Toggle(Styles.DRAW_PLUGS_LABEL, DevToolSettings.Instance.m_showPlugs);
            DevToolSettings.Instance.m_showGeoBounds = EditorGUILayout.Toggle(Styles.DRAW_GEO_BOUNDS_LABEL, DevToolSettings.Instance.m_showGeoBounds);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(DevToolSettings.Instance);
            }

            if (GUILayout.Button("Reset Meshes"))
            {
                MarkerInspector.ResetMeshes();
            }
            if (GUILayout.Button("Clear Marker Icon Cache"))
            {
                MarkerInspector.ClearFunctionIconCache();
            }
            if (GUILayout.Button("Refresh ComplexResourceSet Cache"))
            {
                CachedComplexResourceSet.Instance.Recache();
                EditorUtility.SetDirty(CachedComplexResourceSet.Instance);
            }
        }

        #region Constant GUI Contents
        private static class Styles
        {
            public static GUIContent HEADER_TITLE;
            public static GUIContent HEADER_BACK_BUTTON_LABEL;
            public static GUIContent HEADER_VIEW_BUTTON_LABEL;

            public static GUIContent DRAW_MARKERS_LABEL;
            public static GUIContent BACK_BUTTON_LABEL;
            public static GUIContent RUNDOWN_PATH_LABEL;
            public static GUIContent AUTHOR_INITIALS_LABEL;
            public static GUIContent DRAW_GATES_LABEL;
            public static GUIContent DRAW_PLUGS_LABEL;
            public static GUIContent DRAW_GEO_BOUNDS_LABEL;
            public static bool HAS_GUI_CONSTANTS = false;

            public static void RefreshGUIConstants()
            {
                HAS_GUI_CONSTANTS = true;

                HEADER_TITLE = new GUIContent("Settings");
                HEADER_BACK_BUTTON_LABEL = new GUIContent("Back");
                HEADER_VIEW_BUTTON_LABEL = new GUIContent("View");

                BACK_BUTTON_LABEL = new GUIContent("Back");
                DRAW_MARKERS_LABEL = new GUIContent("Draw Markers");
                RUNDOWN_PATH_LABEL = new GUIContent("Rundown Path", "The path to your rundown");
                AUTHOR_INITIALS_LABEL = new GUIContent("Author Initials", "Used for creating new geomorphs");
                DRAW_GATES_LABEL = new GUIContent("Draw Gates");
                DRAW_PLUGS_LABEL = new GUIContent("Draw Plugs");
                DRAW_GEO_BOUNDS_LABEL = new GUIContent("Draw Geo Bounds");
            }
        }
        #endregion
    }
}

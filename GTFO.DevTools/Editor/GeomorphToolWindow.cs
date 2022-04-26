using GTFO.DevTools.Persistent;
using LevelGeneration;
using System;
using UnityEditor;
using UnityEngine;

namespace GTFO.DevTools
{

    public class GeomorphToolWindow : EditorWindow
    {
        public static bool DrawMarkers { get; set; }

        private GameObject m_selectedGameObj;
        private LG_Geomorph m_currentGeomorph;
        private LG_Area m_currentArea;
        private bool m_showAreas;
        private LG_Area[] m_currentGeomorphAreas = new LG_Area[0];
        private bool m_isExitGeomorph;
        private ToolView m_view;

        private enum ToolView
        {
            Default,
            Settings
        }

        private void Awake()
        {
            if (!Styles.HAS_GUI_CONSTANTS)
                Styles.RefreshGUIConstants();
        }

        private void OnGUI()
        {
            this.RefreshSelectedGameObj();
            if (!Styles.HAS_GUI_CONSTANTS)
                Styles.RefreshGUIConstants();

            this.titleContent = Styles.TITLE;

            EditorGUILayout.BeginHorizontal();

            switch (this.m_view)
            {
                case ToolView.Default:
                    EditorGUILayout.LabelField(this.m_selectedGameObj == null ? "<No Object Selected>" : this.m_selectedGameObj.name, EditorStyles.whiteLargeLabel);
                    if (GUILayout.Button(Styles.SETTINGS_BUTTON_LABEL, GUILayout.ExpandWidth(false)))
                    {
                        this.m_view = ToolView.Settings;
                    }
                    break;
                case ToolView.Settings:
                    EditorGUILayout.LabelField(Styles.SETTINGS_VIEW_LABEL, EditorStyles.whiteLargeLabel);
                    if (GUILayout.Button(Styles.VIEW_BUTTON_LABEL, GUILayout.ExpandWidth(false)))
                    {
                        EditorGUIUtility.PingObject(DevToolSettings.Instance);
                    }
                    if (GUILayout.Button(Styles.BACK_BUTTON_LABEL, GUILayout.ExpandWidth(false)))
                    {
                        this.m_view = ToolView.Default;
                    }
                    break;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            switch (this.m_view)
            {
                case ToolView.Default:
                    this.DefaultViewGUI();
                    break;
                case ToolView.Settings:
                    this.SettingsViewGUI();
                    break;
            }
            

            
        }

        private void SettingsViewGUI()
        {
            DevToolSettings.Instance.m_rundownPath = EditorGUILayout.TextField(Styles.RUNDOWN_PATH_LABEL, DevToolSettings.Instance.m_rundownPath);
            DevToolSettings.Instance.m_showMarkers = EditorGUILayout.Toggle(Styles.DRAW_MARKERS_LABEL, DevToolSettings.Instance.m_showMarkers);
            DevToolSettings.Instance.m_showGates = EditorGUILayout.Toggle(Styles.DRAW_GATES_LABEL, DevToolSettings.Instance.m_showGates);
            DevToolSettings.Instance.m_showPlugs = EditorGUILayout.Toggle(Styles.DRAW_PLUGS_LABEL, DevToolSettings.Instance.m_showPlugs);
            DevToolSettings.Instance.m_showGeoBounds = EditorGUILayout.Toggle(Styles.DRAW_GEO_BOUNDS_LABEL, DevToolSettings.Instance.m_showGeoBounds);

            if (GUILayout.Button("Reset Meshes"))
            {
                MarkerInspector.ResetMeshes();
            }
        }

        private void DefaultViewGUI()
        {
            if (!this.m_selectedGameObj)
            {
                EditorGUILayout.HelpBox(Styles.ERROR_NO_SELECTION);
                return;
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            GUI.enabled = false;
            EditorGUILayout.ObjectField(Styles.GET_CURRENT_GEOMORPH_LABEL(this.m_isExitGeomorph), this.m_currentGeomorph, typeof(LG_Geomorph), true);
            GUI.enabled = this.m_currentGeomorph;
            if (GUILayout.Button(Styles.VIEW_BUTTON_LABEL, GUILayout.ExpandWidth(false)))
            {
                EditorGUIUtility.PingObject(this.m_currentGeomorph.gameObject);
                Selection.activeObject = this.m_currentGeomorph;
            }
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;

            if (!this.m_currentGeomorph)
            {
                EditorGUILayout.HelpBox(Styles.ERROR_NO_GEOMORPH);
                return;
            }

            if (this.m_currentGeomorphAreas.Length == 0)
            {
                EditorGUILayout.HelpBox(Styles.ERROR_NO_AREAS);
                return;
            }

            if (this.m_currentArea != null)
            {

            }
        }

        private bool NeedsRepaint()
        {
            var currentSelected = GetCurrentSelectedGameObj();
            if (currentSelected != this.m_selectedGameObj)
            {
                return true;
            }

            return false;
        }

        private void Update()
        {
            if (this.NeedsRepaint())
                this.Repaint();
        }

        private void RefreshSelectedGameObj()
        {

            this.m_selectedGameObj = GetCurrentSelectedGameObj();
            this.m_currentGeomorph = this.m_selectedGameObj == null ? null : this.m_selectedGameObj.GetComponentInParent<LG_Geomorph>();
            this.m_isExitGeomorph = this.m_currentGeomorph && this.m_currentGeomorph.GetComponent<LG_LevelExitGeo>();
            this.m_currentGeomorphAreas = this.m_currentGeomorph == null ? new LG_Area[0] : this.m_currentGeomorph.GetComponentsInChildren<LG_Area>();
            this.m_currentArea = this.m_selectedGameObj == null ? null : this.m_selectedGameObj.GetComponentInParent<LG_Area>();
        }

        [MenuItem("Window/Geomorph Tool")]
        private static void CreateToolWindowMenuItem()
            => CreateToolWindow();

        public static GeomorphToolWindow CreateToolWindow()
        {
            var window = GetWindow<GeomorphToolWindow>();
            window.Show();
            return window;
        }

        private static GameObject GetCurrentSelectedGameObj()
        {
            var activeTransform = Selection.activeTransform;
            return activeTransform == null ? null : activeTransform.gameObject;
        }

        #region Constant GUI Contents
        private static class Styles
        {
            public static GUIContent TITLE;
            public static GUIContent DRAW_MARKERS_LABEL;
            public static GUIContent CURRENT_GEOMORPH_LABEL;
            public static GUIContent CURRENT_GEOMORPH_EXIT_LABEL;
            public static GUIContent GET_CURRENT_GEOMORPH_LABEL(bool isExit)
                => isExit ? CURRENT_GEOMORPH_EXIT_LABEL : CURRENT_GEOMORPH_LABEL;
            public static GUIContent VIEW_BUTTON_LABEL;
            public static GUIContent SETTINGS_VIEW_LABEL;
            public static GUIContent SETTINGS_BUTTON_LABEL;
            public static GUIContent BACK_BUTTON_LABEL;
            public static GUIContent RUNDOWN_PATH_LABEL;
            public static GUIContent DRAW_GATES_LABEL;
            public static GUIContent DRAW_PLUGS_LABEL;
            public static GUIContent DRAW_GEO_BOUNDS_LABEL;
            public static GUIContent ERROR_NO_SELECTION;
            public static GUIContent ERROR_NO_GEOMORPH;
            public static GUIContent ERROR_NO_AREAS;
            public static GUIContent ERROR_ICON;
            public static bool HAS_GUI_CONSTANTS = false;

            public static void RefreshGUIConstants()
            {
                HAS_GUI_CONSTANTS = true;
                TITLE = new GUIContent("Geo Dev Tools", EditorGUIUtility.IconContent("Prefab Icon").image);
                CURRENT_GEOMORPH_LABEL = new GUIContent("Current Geomorph");
                CURRENT_GEOMORPH_EXIT_LABEL = new GUIContent("Current Geomorph [Exit]");
                VIEW_BUTTON_LABEL = new GUIContent("View");
                SETTINGS_VIEW_LABEL = new GUIContent("Settings");
                SETTINGS_BUTTON_LABEL = new GUIContent("Settings");
                BACK_BUTTON_LABEL = new GUIContent("Back");
                DRAW_MARKERS_LABEL = new GUIContent("Draw Markers");
                RUNDOWN_PATH_LABEL = new GUIContent("Rundown Path");
                DRAW_GATES_LABEL = new GUIContent("Draw Gates");
                DRAW_PLUGS_LABEL = new GUIContent("Draw Plugs");
                DRAW_GEO_BOUNDS_LABEL = new GUIContent("Draw Geo Bounds");
                ERROR_ICON = EditorGUIUtility.IconContent("console.erroricon");
                ERROR_NO_SELECTION = new GUIContent("No GameObject Selected", ERROR_ICON.image);
                ERROR_NO_GEOMORPH = new GUIContent("No Geomorph Found", ERROR_ICON.image);
                ERROR_NO_AREAS = new GUIContent("No Areas in Geomorph", ERROR_ICON.image);
            }
        }
        #endregion
    }
}
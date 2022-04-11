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

            if (!this.m_selectedGameObj)
            {
                EditorGUILayout.HelpBox(Styles.ERROR_NO_SELECTION);
                return;
            }

            EditorGUILayout.LabelField(this.m_selectedGameObj.name, EditorStyles.whiteLargeLabel);
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
            EditorGUILayout.Space();

            if (this.m_currentGeomorphAreas.Length == 0)
            {
                EditorGUILayout.HelpBox(Styles.ERROR_NO_AREAS);
                return;
            }

            DrawMarkers = EditorGUILayout.Toggle(Styles.DRAW_MARKERS_LABEL, DrawMarkers);
            if (GUILayout.Button("Reset Meshes"))
            {
                MarkerInspector.ResetMeshes();
            }

            this.m_showAreas = EditorGUILayout.Foldout(this.m_showAreas, Styles.AREAS_LABEL, true);
            if (this.m_showAreas)
            {
                using (var s = new EditorGUI.IndentLevelScope())
                {
                    foreach (var area in this.m_currentGeomorphAreas)
                    {
                        EditorGUILayout.LabelField(area.name, area == this.m_currentArea ? EditorStyles.boldLabel : EditorStyles.label);
                    }
                }
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
            public static GUIContent AREAS_LABEL;
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
                DRAW_MARKERS_LABEL = new GUIContent("Draw Markers");
                AREAS_LABEL = new GUIContent("AREAS");
                ERROR_ICON = EditorGUIUtility.IconContent("console.erroricon");
                ERROR_NO_SELECTION = new GUIContent("No GameObject Selected", ERROR_ICON.image);
                ERROR_NO_GEOMORPH = new GUIContent("No Geomorph Found", ERROR_ICON.image);
                ERROR_NO_AREAS = new GUIContent("No Areas in Geomorph", ERROR_ICON.image);
            }
        }
        #endregion
    }
}
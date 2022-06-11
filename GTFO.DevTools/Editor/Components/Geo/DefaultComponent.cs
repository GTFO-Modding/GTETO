using AIGraph;
using GTFO.DevTools.Utilities;
using LevelGeneration;
using UnityEditor;
using UnityEngine;

namespace GTFO.DevTools.Components.Geo
{
    public class DefaultComponent : GeoToolViewComponent
    {
        public DefaultComponent(GeoToolComponent parent) : base(parent, GeoToolComponent.View.Default)
        { }

        public override bool NeedStyleRefresh => !Styles.HAS_GUI_CONSTANTS;
        public override void RefreshStyle()
        {
            Styles.RefreshGUIConstants();
        }

        protected override void OnHeaderGUI()
        {
            EditorGUILayout.BeginHorizontal();
            var selectedGameObj = this.Window.SelectedGameObject;
            EditorGUILayout.LabelField(selectedGameObj == null ? "<No Object Selected>" : selectedGameObj.name, EditorStyles.whiteLargeLabel);
            if (GUILayout.Button(Styles.HEADER_SETTINGS_BUTTON_LABEL, GUILayout.ExpandWidth(false)))
            {
                this.Tool.ChangeToView(GeoToolComponent.View.Settings);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        protected override void OnInspectorGUI()
        {
            if (!this.Window.SelectedGameObject)
            {
                EditorGUILayout.HelpBox(Styles.WARN_NO_SELECTION);
                EditorGUILayout.Space();

                this.CreateGeomorphButtonGUI();
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            var selectedGeomorph = this.Window.SelectedGeomorph;

            GUI.enabled = false;
            EditorGUILayout.ObjectField(Styles.GET_CURRENT_GEOMORPH_LABEL(this.Window.IsSelectedGeomorphExit), selectedGeomorph, typeof(LG_Geomorph), true);
            GUI.enabled = selectedGeomorph;
            if (GUILayout.Button(Styles.VIEW_BUTTON_LABEL, GUILayout.ExpandWidth(false)))
            {
                EditorGUIUtility.PingObject(selectedGeomorph.gameObject);
                Selection.activeObject = selectedGeomorph;
            }
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;

            if (!selectedGeomorph)
            {
                EditorGUILayout.HelpBox(Styles.WARN_NO_GEOMORPH);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(Styles.SETUP_AS_GEOMORPH_BUTTON_LABEL))
                {
                    this.Tool.ChangeToView(GeoToolComponent.View.CreateGeomorph);
                    ((CreateNewGeomorphComponent)this.Tool.GetCurrentView()).SetGeomorphGameObject(this.Window.SelectedGameObject);
                }
                this.CreateGeomorphButtonGUI();
                EditorGUILayout.EndHorizontal();
                return;
            }

            var selectedGeomorphAreas = this.Window.SelectedGeomorphAreas;

            this.CreateGeomorphButtonGUI();
            EditorGUILayout.Space();

            if (selectedGeomorphAreas.Length == 0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(Styles.ERROR_NO_AREAS);
                if (GUILayout.Button(Styles.FIX_BUTTON_LABEL, GUILayout.ExpandWidth(false)))
                {
                    var area = CreateArea(selectedGeomorph, "Area A");
                    Selection.activeGameObject = area.gameObject;
                    EditorGUIUtility.PingObject(area);
                }
                EditorGUILayout.EndHorizontal();
            }

            if (selectedGeomorph.GetComponent<AIG_GeomorphNodeVolume>() == null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(Styles.ERROR_NO_NODEVOLUME);
                if (GUILayout.Button(Styles.FIX_BUTTON_LABEL, GUILayout.ExpandWidth(false)))
                {
                    selectedGeomorph.gameObject.AddComponent<AIG_GeomorphNodeVolume>();
                }
                EditorGUILayout.EndHorizontal();
            }

            foreach (var area in selectedGeomorphAreas)
            {
                this.ValidateArea(area);
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(Styles.PREVIEW_BUTTON_LABEL))
            {
                PreviewUtility.CreatePreview(selectedGeomorph.gameObject);
            }
            if (GUILayout.Button(Styles.CLEAR_PREVIEW_BUTTON_LABEL))
            {
                PreviewUtility.ClearPreview(selectedGeomorph.gameObject);
            }
            EditorGUILayout.EndHorizontal();
        }

        private static LG_Area CreateArea(LG_Geomorph geo, string areaName)
        {
            var geoObj = geo.gameObject;
            var areaObj = CreateGO(geoObj.transform, areaName);
            CreateGO(areaObj.transform, "EnvProps");
            CreateGO(areaObj.transform, "Markers");
            CreateGO(areaObj.transform, "Lights");
            CreateGO(areaObj.transform, "Invisible Walls");
            var graphObj = CreateGO(areaObj.transform, "AreaAiGraphSource");
            graphObj.AddComponent<LG_AreaAIGraphSource>();

            var area = areaObj.AddComponent<LG_Area>();
            return area;
        }

        private static GameObject CreateGO(Transform parent, string name)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.localRotation = Quaternion.identity;
            return go;
        }

        private void CreateGeomorphButtonGUI()
        {
            if (GUILayout.Button(Styles.CREATE_GEOMORPH_BUTTON_LABEL))
            {
                this.Tool.ChangeToView(GeoToolComponent.View.CreateGeomorph);
            }
        }

        private void ValidateArea(LG_Area area)
        {
            bool shownLabel = false;
            if (area.GetComponentInChildren<LG_AreaAIGraphSource>() == null)
            {
                this.ShowAreaLabel(area, ref shownLabel);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(Styles.ERROR_NO_AIGRAPH);
                if (GUILayout.Button(Styles.FIX_BUTTON_LABEL, GUILayout.ExpandWidth(false)))
                {
                    var gameObj = CreateGO(area.transform, "AreaAiGraphSource");
                    gameObj.AddComponent<LG_AreaAIGraphSource>();

                    Selection.activeGameObject = gameObj;
                    EditorGUIUtility.PingObject(gameObj);
                }
                EditorGUILayout.EndHorizontal();

            }

            if (shownLabel)
                EditorGUI.indentLevel--;
        }

        private void ShowAreaLabel(LG_Area area, ref bool shownLabel)
        {
            if (shownLabel)
                return;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(area.name, EditorStyles.boldLabel);
            if (GUILayout.Button(Styles.VIEW_BUTTON_LABEL, GUILayout.ExpandWidth(false)))
            {
                Selection.activeGameObject = area.gameObject;
                EditorGUIUtility.PingObject(area);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel++;
            shownLabel = true;
        }


        #region Constant GUI Contents
        private static class Styles
        {
            public static GUIContent HEADER_SETTINGS_BUTTON_LABEL;

            public static GUIContent CURRENT_GEOMORPH_LABEL;
            public static GUIContent CURRENT_GEOMORPH_EXIT_LABEL;
            public static GUIContent GET_CURRENT_GEOMORPH_LABEL(bool isExit)
                => isExit ? CURRENT_GEOMORPH_EXIT_LABEL : CURRENT_GEOMORPH_LABEL;
            public static GUIContent PREVIEW_BUTTON_LABEL;
            public static GUIContent CLEAR_PREVIEW_BUTTON_LABEL;
            public static GUIContent VIEW_BUTTON_LABEL;
            public static GUIContent FIX_BUTTON_LABEL;
            public static GUIContent SETUP_AS_GEOMORPH_BUTTON_LABEL;
            public static GUIContent CREATE_GEOMORPH_BUTTON_LABEL;
            public static GUIContent WARN_NO_SELECTION;
            public static GUIContent WARN_NO_GEOMORPH;
            public static GUIContent ERROR_NO_AREAS;
            public static GUIContent ERROR_NO_NODEVOLUME;
            public static GUIContent ERROR_NO_AIGRAPH;
            public static GUIContent ERROR_ICON;
            public static GUIContent WARN_ICON;

            public static bool HAS_GUI_CONSTANTS = false;

            public static void RefreshGUIConstants()
            {
                HAS_GUI_CONSTANTS = true;
                HEADER_SETTINGS_BUTTON_LABEL = new GUIContent("Settings");

                PREVIEW_BUTTON_LABEL = new GUIContent("Preview");
                CLEAR_PREVIEW_BUTTON_LABEL = new GUIContent("Clear Preview");

                CREATE_GEOMORPH_BUTTON_LABEL = new GUIContent("Create Geomorph");
                CURRENT_GEOMORPH_LABEL = new GUIContent("Current Geomorph");
                CURRENT_GEOMORPH_EXIT_LABEL = new GUIContent("Current Geomorph [Exit]");
                VIEW_BUTTON_LABEL = new GUIContent("View");
                SETUP_AS_GEOMORPH_BUTTON_LABEL = new GUIContent("Setup as Geomorph");
                FIX_BUTTON_LABEL = new GUIContent("Fix");

                ERROR_ICON = EditorGUIUtility.IconContent("console.erroricon");
                WARN_ICON = EditorGUIUtility.IconContent("console.warnicon");
                WARN_NO_SELECTION = new GUIContent("No GameObject Selected", WARN_ICON.image);
                WARN_NO_GEOMORPH = new GUIContent("No Geomorph Found", WARN_ICON.image);
                ERROR_NO_NODEVOLUME = new GUIContent("No Node Volume attached to Geo", ERROR_ICON.image);
                ERROR_NO_AIGRAPH = new GUIContent("Area doesn't have an AI Graph", ERROR_ICON.image);
                ERROR_NO_AREAS = new GUIContent("No Areas in Geomorph", ERROR_ICON.image);
            }
        }
        #endregion
    }
}

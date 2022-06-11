using GTFO.DevTools.Components.Geo;
using LevelGeneration;
using UnityEditor;
using UnityEngine;

namespace GTFO.DevTools.Geo
{

    public class GeomorphToolWindow : EditorWindow
    {
        private GameObject m_selectedGameObj;
        private LG_Geomorph m_currentGeomorph;
        private LG_Area m_currentArea;
        private LG_Area[] m_currentGeomorphAreas = new LG_Area[0];
        private bool m_currentIsExitGeomorph;
        private GeoToolComponent m_component;

        public GameObject SelectedGameObject => this.m_selectedGameObj;
        public LG_Geomorph SelectedGeomorph => this.m_currentGeomorph;
        public bool IsSelectedGeomorphExit => this.m_currentIsExitGeomorph;
        public LG_Area SelectedArea => this.m_currentArea;
        public LG_Area[] SelectedGeomorphAreas => this.m_currentGeomorphAreas;

        
        private void Awake()
        {
            if (!Styles.HAS_GUI_CONSTANTS)
                Styles.RefreshGUIConstants();

            if (this.m_component == null)
                this.m_component = new GeoToolComponent(this);

            this.m_component.OnShow();
        }

        private void OnGUI()
        {
            this.RefreshSelectedGameObj();
            if (!Styles.HAS_GUI_CONSTANTS)
                Styles.RefreshGUIConstants();

            this.titleContent = Styles.TITLE;
            this.m_component.Draw();
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
            this.m_currentIsExitGeomorph = this.m_currentGeomorph && this.m_currentGeomorph.GetComponent<LG_LevelExitGeo>();
            this.m_currentGeomorphAreas = this.m_currentGeomorph == null ? new LG_Area[0] : this.m_currentGeomorph.GetComponentsInChildren<LG_Area>();
            this.m_currentArea = this.m_selectedGameObj == null ? null : this.m_selectedGameObj.GetComponentInParent<LG_Area>();
        }

        [MenuItem("Window/GTFO/Geomorph Tool")]
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
            public static bool HAS_GUI_CONSTANTS = false;

            public static void RefreshGUIConstants()
            {
                HAS_GUI_CONSTANTS = true;
                TITLE = new GUIContent("Geo Dev Tools", EditorGUIUtility.IconContent("Prefab Icon").image);
            }
        }
        #endregion
    }
}
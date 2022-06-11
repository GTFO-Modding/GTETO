using AIGraph;
using GTFO.DevTools.Geo;
using GTFO.DevTools.Persistent;
using LevelGeneration;
using UnityEditor;
using UnityEngine;

namespace GTFO.DevTools.Components.Geo
{
    public class CreateNewGeomorphComponent : GeoToolViewComponent
    {
        private GameObject m_gameObject;
        private string m_name;
        private string m_suffix;
        private LG_GeomorphShapeType m_shape;
        private GeomorphType m_type;
        private GeomorphSubComplex m_subcomplex;
        private bool m_isExit;

        public CreateNewGeomorphComponent(GeoToolComponent parent) : base(parent, GeoToolComponent.View.CreateGeomorph)
        { }

        public override bool NeedStyleRefresh => !Styles.HAS_GUI_CONSTANTS;
        public override void RefreshStyle()
        {
            Styles.RefreshGUIConstants();
        }

        public override void Reset()
        {
            this.m_gameObject = null;
            this.m_suffix = "";
            this.m_name = "";
            this.m_shape = LG_GeomorphShapeType.s1x1;
            this.m_type = GeomorphType.None;
            this.m_subcomplex = GeomorphSubComplex.DigSite;
            this.m_isExit = false;
        }

        public void SetGeomorphGameObject(GameObject obj)
        {
            this.m_gameObject = obj;
        }

        protected override void OnHeaderGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(Styles.HEADER_TITLE, EditorStyles.whiteLargeLabel);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        protected override void OnInspectorGUI()
        {
            if (!this.m_gameObject)
            {
                this.m_name = EditorGUILayout.TextField(Styles.NAME_LABEL, this.m_name);
                this.m_suffix = EditorGUILayout.TextField(Styles.SUFFIX_LABEL, this.m_suffix);
            }
            this.m_shape = (LG_GeomorphShapeType)EditorGUILayout.EnumPopup(Styles.SHAPE_LABEL, this.m_shape);
            this.m_type = (GeomorphType)EditorGUILayout.EnumPopup(Styles.TYPE_LABEL, this.m_type);
            this.m_subcomplex = (GeomorphSubComplex)EditorGUILayout.EnumPopup(Styles.SUBCOMPLEX_LABEL, this.m_subcomplex);

            GUI.enabled = !this.m_type.IsFloorTransition();
            this.m_isExit = GUI.enabled && EditorGUILayout.Toggle(Styles.IS_EXIT_LABEL, this.m_isExit);
            GUI.enabled = true;
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            GUI.enabled = this.m_type != GeomorphType.None;
            if (GUILayout.Button("Create"))
            {
                LG_Geomorph geo = this.CreateGeomorph();
                Selection.activeGameObject = geo.gameObject;

                this.Reset();
                this.Tool.ChangeToView(GeoToolComponent.View.Default);
            }
            GUI.enabled = true;
            if (GUILayout.Button("Cancel"))
            {
                this.Reset();
                this.Tool.ChangeToView(GeoToolComponent.View.Default);
            }
            EditorGUILayout.EndHorizontal();
        }

        private LG_Geomorph CreateGeomorph()
        {
            var obj = this.m_gameObject;
            if (!obj)
            {
                string geoName = "geo_";
                switch (this.m_type)
                {
                    case GeomorphType.ElevatorShaft:
                        geoName += "32x32";
                        break;
                    default:
                        geoName += "64x64";
                        break;
                }
                switch (this.m_type)
                {
                    case GeomorphType.Service:
                        geoName += "_service";
                        break;
                    case GeomorphType.Mining:
                        geoName += "_mining";
                        break;
                    case GeomorphType.Tech:
                        geoName += "_tech";
                        break;
                    case GeomorphType.ElevatorShaft:
                        geoName += "_elevator_shaft";
                        break;
                }
                switch (this.m_subcomplex)
                {
                    case GeomorphSubComplex.Lab:
                        geoName += "_lab";
                        break;
                    case GeomorphSubComplex.DataCenter:
                        geoName += "_datacenter";
                        break;
                    case GeomorphSubComplex.DigSite:
                        geoName += "_digsite";
                        break;
                    case GeomorphSubComplex.Refinery:
                        geoName += "_refinery";
                        break;
                    case GeomorphSubComplex.Storage:
                        geoName += "_storage";
                        break;
                    case GeomorphSubComplex.Floodways:
                        geoName += "_floodways";
                        break;
                }

                string initials = DevToolSettings.Instance.m_authorInitials ?? "DEV";
                if (!string.IsNullOrWhiteSpace(this.m_name))
                {
                    geoName += "_" + this.m_name.Trim();
                }

                obj = new GameObject(geoName + "_" + initials + "_" + this.m_suffix);
            }
            if (!obj.GetComponent<AIG_GeomorphNodeVolume>())
            {
                obj.AddComponent<AIG_GeomorphNodeVolume>();
            }

            LG_Geomorph geo;
            if (this.m_type.IsFloorTransition())
            {
                var startTile = obj.AddComponent<LG_FloorTransition>();

                geo = startTile;

            }
            else
            {
                geo = obj.AddComponent<LG_Geomorph>();
            }
            geo.m_goShapeType = this.m_shape;
            if (this.m_isExit)
            {
                var exit = obj.AddComponent<LG_LevelExitGeo>();
            }
            this.SetupGeomorph(obj);
            return geo;
        }

        private void SetupGeomorph(GameObject obj)
        {
            var geomorph = obj.GetComponent<LG_Geomorph>();
            if (!geomorph)
            {
                return;
            }
        }

        #region Constant GUI Contents
        private static class Styles
        {
            public static GUIContent HEADER_TITLE;

            public static GUIContent SUFFIX_LABEL;
            public static GUIContent NAME_LABEL;
            public static GUIContent SHAPE_LABEL;
            public static GUIContent TYPE_LABEL;
            public static GUIContent SUBCOMPLEX_LABEL;
            public static GUIContent IS_EXIT_LABEL;
            public static GUIContent CREATE_BUTTON_LABEL;
            public static GUIContent CANCEL_BUTTON_LABEL;
            public static bool HAS_GUI_CONSTANTS = false;

            public static void RefreshGUIConstants()
            {
                HAS_GUI_CONSTANTS = true;
                HEADER_TITLE = new GUIContent("Create Geomorph");
                SUFFIX_LABEL = new GUIContent("Name Suffix");
                NAME_LABEL = new GUIContent("Name");
                SHAPE_LABEL = new GUIContent("Shape");
                TYPE_LABEL = new GUIContent("Type");
                SUBCOMPLEX_LABEL = new GUIContent("SubComplex");
                IS_EXIT_LABEL = new GUIContent("Is Exit?");
                CREATE_BUTTON_LABEL = new GUIContent("Create");
                CANCEL_BUTTON_LABEL = new GUIContent("Cancel");
            }
        }
        #endregion
    }
}

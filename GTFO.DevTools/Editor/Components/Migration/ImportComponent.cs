using UnityEngine;
using UnityEditor;
using static GTFO.DevTools.Components.Migration.MigrationToolComponent;
using System.IO;

namespace GTFO.DevTools.Components.Migration
{
    public class ImportComponent : MigrationToolViewComponent
    {
        public ImportComponent(MigrationToolComponent parent) : base(parent, View.Import)
        { }

        public override bool NeedStyleRefresh => !Styles.HAS_GUI_CONSTANTS;
        public override void RefreshStyle()
        {
            Styles.RefreshGUIConstants();
        }

        protected override void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(Styles.IMPORT_TITLE, EditorStyles.largeLabel);
            if (GUILayout.Button(Styles.BACK_BUTTON_LABEL, GUILayout.ExpandWidth(false)))
            {
                this.Tool.ChangeToView(View.None);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField(Styles.IMPORT_INFO_LABEL, EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(Styles.PROJECT_INFO_LOCATION_IMPORT_HINT_LABEL, EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField(Styles.IMPORT_FROM_FILE_INFO_LABEL, EditorStyles.wordWrappedLabel);
            if (GUILayout.Button(Styles.IMPORT_FROM_FILE_BUTTON_LABEL))
            {
                string basicPath = Path.GetDirectoryName(Path.GetDirectoryName(Application.dataPath));
                string folder = EditorUtility.OpenFilePanel("Import Project", basicPath, "projinfo");
                if (!string.IsNullOrWhiteSpace(folder))
                {
                    this.Tool.GetViewComponent<ImportingComponent>(View.Importing).SetImportPath(folder);
                    this.Tool.ChangeToView(View.Importing);
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(Styles.IMPORT_FROM_PATH_INFO_LABEL, EditorStyles.wordWrappedLabel);
            if (GUILayout.Button(Styles.IMPORT_FROM_PATH_BUTTON_LABEL))
            {
                this.Tool.ChangeToView(View.ImportFile);
            }
        }

        #region Constant GUI Contents
        private static class Styles
        {
            public static GUIContent IMPORT_TITLE;
            public static GUIContent BACK_BUTTON_LABEL;
            public static GUIContent IMPORT_INFO_LABEL;
            public static GUIContent PROJECT_INFO_LOCATION_IMPORT_HINT_LABEL;
            public static GUIContent IMPORT_FROM_FILE_BUTTON_LABEL;
            public static GUIContent IMPORT_FROM_FILE_INFO_LABEL;
            public static GUIContent IMPORT_FROM_PATH_INFO_LABEL;
            public static GUIContent IMPORT_FROM_PATH_BUTTON_LABEL;

            public static bool HAS_GUI_CONSTANTS = false;

            public static void RefreshGUIConstants()
            {
                HAS_GUI_CONSTANTS = true;

                IMPORT_TITLE = new GUIContent("Import");
                IMPORT_INFO_LABEL = new GUIContent("Imports prepared assets from another project.");
                PROJECT_INFO_LOCATION_IMPORT_HINT_LABEL = new GUIContent("A prepared project will have an 'information.projinfo' file outside of it's assets folder.");
                IMPORT_FROM_FILE_BUTTON_LABEL = new GUIContent("Import");
                IMPORT_FROM_FILE_INFO_LABEL = new GUIContent("Click Import below to select this file");
                IMPORT_FROM_PATH_INFO_LABEL = new GUIContent("You can also Import Project from a 'projinfo' file path. This should be used if you clicked 'Copy Path' for the project you setup for export.");
                IMPORT_FROM_PATH_BUTTON_LABEL = new GUIContent("Import from File Path");
                BACK_BUTTON_LABEL = new GUIContent("Back", "Go back");
            }
        }
        #endregion
    }
}

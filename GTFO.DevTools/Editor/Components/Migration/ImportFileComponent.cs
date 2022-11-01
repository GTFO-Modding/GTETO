using UnityEngine;
using UnityEditor;
using static GTFO.DevTools.Components.Migration.MigrationToolComponent;

namespace GTFO.DevTools.Components.Migration
{
    public class ImportFileComponent : MigrationToolViewComponent
    {
        public ImportFileComponent(MigrationToolComponent parent) : base(parent, View.ImportFile)
        { }

        private string m_importPath = "";

        public override bool NeedStyleRefresh => !Styles.HAS_GUI_CONSTANTS;
        public override void RefreshStyle()
        {
            Styles.RefreshGUIConstants();
        }

        protected override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField(Styles.IMPORT_FILE_PATH_LABEL);
            this.m_importPath = EditorGUILayout.TextField(this.m_importPath);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(Styles.IMPORT_FILE_PATH_IMPORT_BUTTON_LABEL))
            {
                this.Tool.GetViewComponent<ImportingComponent>(View.Importing).SetImportPath(this.m_importPath);
                this.Tool.ChangeToView(View.Importing);
                this.Reset();
            }
            if (GUILayout.Button(Styles.BACK_BUTTON_LABEL))
            {
                this.Tool.ChangeToView(View.Import);
                this.Reset();
            }
            EditorGUILayout.EndHorizontal();
        }

        public override void Reset()
        {
            base.Reset();
            this.m_importPath = "";
        }

        #region Constant GUI Contents
        private static class Styles
        {
            public static GUIContent BACK_BUTTON_LABEL;
            public static GUIContent IMPORT_FILE_PATH_LABEL;
            public static GUIContent IMPORT_FILE_PATH_IMPORT_BUTTON_LABEL;

            public static bool HAS_GUI_CONSTANTS = false;

            public static void RefreshGUIConstants()
            {
                HAS_GUI_CONSTANTS = true;

                BACK_BUTTON_LABEL = new GUIContent("Back", "Go back");
                IMPORT_FILE_PATH_LABEL = new GUIContent("File Path");
                IMPORT_FILE_PATH_IMPORT_BUTTON_LABEL = new GUIContent("Import");
            }
        }
        #endregion
    }
}

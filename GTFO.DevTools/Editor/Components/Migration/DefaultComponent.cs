using UnityEditor;
using UnityEngine;
using static GTFO.DevTools.Components.Migration.MigrationToolComponent;

namespace GTFO.DevTools.Components.Migration
{
    public class DefaultComponent : MigrationToolViewComponent
    {
        public DefaultComponent(MigrationToolComponent parent) : base(parent, View.None)
        { }

        public override bool NeedStyleRefresh => !Styles.HAS_GUI_CONSTANTS;
        public override void RefreshStyle()
        {
            Styles.RefreshGUIConstants();
        }

        protected override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField(Styles.MIGRATE_TOOL_TITLE, EditorStyles.largeLabel);
            EditorGUILayout.LabelField(Styles.MIGRATE_TOOL_INFO, EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(Styles.EXPORT_PROJECT_HINT, EditorStyles.wordWrappedLabel);
            if (GUILayout.Button(Styles.EXPORT_PROJECT_BUTTON))
            {
                this.Tool.ChangeToView(View.Export);
            }
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(Styles.IMPORT_PROJECT_HINT, EditorStyles.wordWrappedLabel);
            if (GUILayout.Button(Styles.IMPORT_PROJECT_BUTTON))
            {
                this.Tool.ChangeToView(View.Import);
            }
        }

        #region Constant GUI Contents
        private static class Styles
        {
            public static GUIContent MIGRATE_TOOL_TITLE;
            public static GUIContent MIGRATE_TOOL_INFO;
            public static GUIContent EXPORT_PROJECT_HINT;
            public static GUIContent IMPORT_PROJECT_HINT;
            public static GUIContent IMPORT_PROJECT_BUTTON;
            public static GUIContent EXPORT_PROJECT_BUTTON;

            public static bool HAS_GUI_CONSTANTS = false;

            public static void RefreshGUIConstants()
            {
                HAS_GUI_CONSTANTS = true;
                EXPORT_PROJECT_BUTTON = new GUIContent("Export");
                IMPORT_PROJECT_BUTTON = new GUIContent("Import/Transfer Project");
                IMPORT_PROJECT_HINT = new GUIContent("Import/Transfer will import a project into this project, transferring over your assets whilst updating them to match this project's structure. Choose this to transfer assets to this project.");
                EXPORT_PROJECT_HINT = new GUIContent("Export will prepare a project for export. Choose this for projects you want to transfer assets from.");
                MIGRATE_TOOL_INFO = new GUIContent("Migrate assets between projects to develop with newer versions of GTFO");
                MIGRATE_TOOL_TITLE = new GUIContent("Migrate Tool");
            }
        }
        #endregion
    }
}

using UnityEngine;
using UnityEditor;
using static GTFO.DevTools.Components.Migration.MigrationToolComponent;
using System.IO;

namespace GTFO.DevTools.Components.Migration
{
    public class ExportFinishedComponent : MigrationToolViewComponent
    {
        public ExportFinishedComponent(MigrationToolComponent parent) : base(parent, View.ExportFinished)
        { }

        public override bool NeedStyleRefresh => !Styles.HAS_GUI_CONSTANTS;
        public override void RefreshStyle()
        {
            Styles.RefreshGUIConstants();
        }

        protected override void OnInspectorGUI()
        {
            string projInfoPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "information.projinfo");
            EditorGUILayout.LabelField(Styles.PROJECT_EXPORT_SUCCESS_INFO, EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(Styles.PROJECT_INFO_LOCATION_HINT_LABEL, EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField(projInfoPath, EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField(Styles.COPY_PATH_INSTRUCTIONS_LABEL, EditorStyles.wordWrappedLabel);

            if (GUILayout.Button(Styles.COPY_PATH_BUTTON_LABEL, GUILayout.ExpandWidth(false)))
            {
                GUIUtility.systemCopyBuffer = projInfoPath;
            }
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(Styles.BACK_BUTTON_LABEL, GUILayout.ExpandWidth(false)))
            {
                this.Tool.ChangeToView(View.None);
            }
            if (GUILayout.Button(Styles.CLOSE_BUTTON_LABEL, GUILayout.ExpandWidth(false)))
            {
                this.DoClose();
            }
            EditorGUILayout.EndHorizontal();
        }

        #region Constant GUI Contents
        private static class Styles
        {
            public static GUIContent COPY_PATH_BUTTON_LABEL;
            public static GUIContent COPY_PATH_INSTRUCTIONS_LABEL;
            public static GUIContent PROJECT_INFO_LOCATION_HINT_LABEL;
            public static GUIContent PROJECT_EXPORT_SUCCESS_INFO;
            public static GUIContent BACK_BUTTON_LABEL;
            public static GUIContent CLOSE_BUTTON_LABEL;

            public static bool HAS_GUI_CONSTANTS = false;

            public static void RefreshGUIConstants()
            {
                HAS_GUI_CONSTANTS = true;

                BACK_BUTTON_LABEL = new GUIContent("Back", "Go back");
                CLOSE_BUTTON_LABEL = new GUIContent("Close", "Close this window.");
                COPY_PATH_INSTRUCTIONS_LABEL = new GUIContent("Click below to copy this path.");
                COPY_PATH_BUTTON_LABEL = new GUIContent("Copy Path");
                PROJECT_EXPORT_SUCCESS_INFO = new GUIContent("This project has been setup for export successfully.");
                PROJECT_INFO_LOCATION_HINT_LABEL = new GUIContent("For reference when importing and transferring assets from this project, this projects 'information.projinfo' file is at ");
            }
        }
        #endregion
    }
}

using UnityEngine;
using UnityEditor;
using static GTFO.DevTools.Components.Migration.MigrationToolComponent;

namespace GTFO.DevTools.Components.Migration
{
    public class ImportFailedComponent : MigrationToolViewComponent
    {
        public ImportFailedComponent(MigrationToolComponent parent) : base(parent, View.ImportFailed)
        { }

        private string m_importFailReason = "";

        public override bool NeedStyleRefresh => !Styles.HAS_GUI_CONSTANTS;
        public override void RefreshStyle()
        {
            Styles.RefreshGUIConstants();
        }

        public void SetFailReason(string reason)
        {
            this.m_importFailReason = reason;
        }

        protected override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField(Styles.IMPORT_FAILED_INFO_LABEL, EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(this.m_importFailReason, EditorStyles.wordWrappedLabel);

            EditorGUILayout.Space();
            if (GUILayout.Button(Styles.BACK_BUTTON_LABEL, GUILayout.ExpandWidth(false)))
            {
                this.Tool.ChangeToView(View.Import);
                this.Reset();
            }
        }

        public override void Reset()
        {
            base.Reset();
            this.m_importFailReason = "";
        }

        #region Constant GUI Contents
        private static class Styles
        {
            public static GUIContent IMPORT_FAILED_INFO_LABEL;
            public static GUIContent BACK_BUTTON_LABEL;

            public static bool HAS_GUI_CONSTANTS = false;

            public static void RefreshGUIConstants()
            {
                HAS_GUI_CONSTANTS = true;

                IMPORT_FAILED_INFO_LABEL = new GUIContent("A fatal error has occurred which resulted in failure to import project.");
                BACK_BUTTON_LABEL = new GUIContent("Back", "Go back");
            }
        }
        #endregion
    }
}

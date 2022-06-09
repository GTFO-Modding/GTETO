using UnityEngine;
using UnityEditor;
using static GTFO.DevTools.Components.Migration.MigrationToolComponent;
using System;

namespace GTFO.DevTools.Components.Migration
{
    public class ExportFailedComponent : MigrationToolViewComponent
    {
        private Exception m_failException;

        public ExportFailedComponent(MigrationToolComponent parent) : base(parent, View.ExportFailed)
        { }

        public void SetFailException(Exception exception)
        {
            this.m_failException = exception;
        }

        public override bool NeedStyleRefresh => !Styles.HAS_GUI_CONSTANTS;
        public override void RefreshStyle()
        {
            Styles.RefreshGUIConstants();
        }

        protected override void OnInspectorGUI()
        {
            if (this.m_failException == null)
            {
                this.Tool.ChangeToView(View.Export);
                return;
            }

            string failEx = this.m_failException.ToString();
            EditorGUILayout.LabelField(Styles.PROJECT_EXPORT_FAILURE_INFO, EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(failEx, EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField(Styles.COPY_EXCEPTION_HINT_LABEL, EditorStyles.wordWrappedLabel);

            if (GUILayout.Button(Styles.COPY_EXCEPTION_BUTTON_LABEL, GUILayout.ExpandWidth(false)))
            {
                GUIUtility.systemCopyBuffer = failEx;
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

        public override void Reset()
        {
            base.Reset();
            this.m_failException = null;
        }

        #region Constant GUI Contents
        private static class Styles
        {
            public static GUIContent COPY_EXCEPTION_HINT_LABEL;
            public static GUIContent PROJECT_EXPORT_FAILURE_INFO;
            public static GUIContent COPY_EXCEPTION_BUTTON_LABEL;
            public static GUIContent BACK_BUTTON_LABEL;
            public static GUIContent CLOSE_BUTTON_LABEL;

            public static bool HAS_GUI_CONSTANTS = false;

            public static void RefreshGUIConstants()
            {
                HAS_GUI_CONSTANTS = true;

                COPY_EXCEPTION_BUTTON_LABEL = new GUIContent("Click below to copy this for help troubleshooting in the #unity-development channel.");
                COPY_EXCEPTION_BUTTON_LABEL = new GUIContent("Copy Exception");
                PROJECT_EXPORT_FAILURE_INFO = new GUIContent("A fatal error has occurred which resulted in export failure.");
                BACK_BUTTON_LABEL = new GUIContent("Back", "Go back");
                CLOSE_BUTTON_LABEL = new GUIContent("Close", "Close this window.");
            }
        }
        #endregion
    }
}

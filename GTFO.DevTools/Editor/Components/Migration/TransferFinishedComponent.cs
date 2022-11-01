using UnityEngine;
using UnityEditor;
using static GTFO.DevTools.Components.Migration.MigrationToolComponent;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

namespace GTFO.DevTools.Components.Migration
{
    public class TransferFinishedComponent : MigrationToolViewComponent
    {
        public TransferFinishedComponent(MigrationToolComponent parent) : base(parent, View.TransferFinished)
        { }

        private List<TransferExceptionInfo> m_transferExceptions;
        private Vector2 m_scrollPos;

        public override bool NeedStyleRefresh => !Styles.HAS_GUI_CONSTANTS;
        public override void RefreshStyle()
        {
            Styles.RefreshGUIConstants();
        }

        public void ClearTransferExceptions()
        {
            if (this.m_transferExceptions == null)
            {
                this.m_transferExceptions = new List<TransferExceptionInfo>();
            }
            else
            {
                this.m_transferExceptions.Clear();
            }
        }

        public void AddTransferException(string userAsset, Exception exception)
        {
            this.m_transferExceptions.Add(new TransferExceptionInfo(userAsset, exception));
        }

        protected override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField(Styles.TRANSFER_COMPLETED_TITLE_LABEL, EditorStyles.largeLabel);
            EditorGUILayout.LabelField(Styles.TRANSFER_COMPLETED_INFO_LABEL, EditorStyles.wordWrappedLabel);
            if (this.m_transferExceptions.Count > 0)
            {
                EditorGUILayout.LabelField(Styles.TRANSFER_ERRORS_INFO_LABEL, EditorStyles.wordWrappedLabel);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField($"Errors ({this.m_transferExceptions.Count})");
                this.m_scrollPos = EditorGUILayout.BeginScrollView(this.m_scrollPos);
                foreach (TransferExceptionInfo error in this.m_transferExceptions)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(error.label, EditorStyles.wordWrappedLabel);
                    if (GUILayout.Button(Styles.COPY_SINGLE_EXCEPTION_BUTTON_LABEL, GUILayout.ExpandWidth(false)))
                    {
                        GUIUtility.systemCopyBuffer = error.ToString();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
                if (GUILayout.Button(Styles.COPY_ALL_EXCEPTIONS_BUTTON_LABEL, GUILayout.ExpandWidth(false)))
                {
                    StringBuilder builder = new StringBuilder();
                    foreach (TransferExceptionInfo error in this.m_transferExceptions)
                    {
                        builder.AppendLine(error.ToString());
                        builder.AppendLine();
                    }
                    GUIUtility.systemCopyBuffer = builder.ToString();
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(Styles.BACK_BUTTON_LABEL))
            {
                this.Tool.ChangeToView(View.Import);
                this.Reset();
            }
            if (GUILayout.Button(Styles.CLOSE_BUTTON_LABEL))
            {
                this.DoClose();
            }
            EditorGUILayout.EndHorizontal();
        }

        public override void Reset()
        {
            base.Reset();
            this.m_scrollPos = Vector2.zero;
            this.m_transferExceptions?.Clear();
        }

        private sealed class TransferExceptionInfo
        {
            public readonly string assetPath;
            public readonly Exception exception;
            public readonly GUIContent label;

            public TransferExceptionInfo(string assetPath, Exception ex)
            {
                this.assetPath = assetPath;
                this.exception = ex;
                this.label = new GUIContent($"[{Path.GetFileName(assetPath)}] {ex.Message}", $"Asset Path:\n{this.assetPath}\nFull Exception:\n{ex}");
            }

            public override string ToString()
            {
                return $"[{this.assetPath}] {this.exception}";
            }
        }

        #region Constant GUI Contents
        private static class Styles
        {
            public static GUIContent TRANSFER_COMPLETED_TITLE_LABEL;
            public static GUIContent TRANSFER_COMPLETED_INFO_LABEL;
            public static GUIContent TRANSFER_ERRORS_INFO_LABEL;
            public static GUIContent COPY_SINGLE_EXCEPTION_BUTTON_LABEL;
            public static GUIContent COPY_ALL_EXCEPTIONS_BUTTON_LABEL;
            public static GUIContent BACK_BUTTON_LABEL;
            public static GUIContent CLOSE_BUTTON_LABEL;

            public static bool HAS_GUI_CONSTANTS = false;

            public static void RefreshGUIConstants()
            {
                HAS_GUI_CONSTANTS = true;

                TRANSFER_COMPLETED_TITLE_LABEL = new GUIContent("Transfer Complete");
                TRANSFER_COMPLETED_INFO_LABEL = new GUIContent("Assets should have been transferred over to this new project!");
                TRANSFER_ERRORS_INFO_LABEL = new GUIContent("There were a couple of errors, though. Be sure to paste some of these errors into the #unity-development channel.");
                COPY_ALL_EXCEPTIONS_BUTTON_LABEL = new GUIContent("Copy All", "Copy all exception messages");
                COPY_SINGLE_EXCEPTION_BUTTON_LABEL = new GUIContent("Copy", "Copy the full exception message");
                BACK_BUTTON_LABEL = new GUIContent("Back", "Go back");
                CLOSE_BUTTON_LABEL = new GUIContent("Close", "Close this window.");
            }
        }
        #endregion
    }
}

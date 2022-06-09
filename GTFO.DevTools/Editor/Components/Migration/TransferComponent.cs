using UnityEngine;
using UnityEditor;
using static GTFO.DevTools.Components.Migration.MigrationToolComponent;
using GTFO.DevTools.Migration;
using System.Collections.Generic;
using System.IO;
using GTFO.DevTools.Utilities;
using System;

namespace GTFO.DevTools.Components.Migration
{
    public class TransferComponent : MigrationToolViewComponent
    {
        public TransferComponent(MigrationToolComponent parent) : base(parent, View.Transfer)
        { }

        private ProjectInformation m_project;
        private Dictionary<string, string> m_guidMap;
        private List<string> m_userAssets;
        private Vector2 m_scrollPos;

        public override bool NeedStyleRefresh => !Styles.HAS_GUI_CONSTANTS;
        public override void RefreshStyle()
        {
            Styles.RefreshGUIConstants();
        }

        public void SetProject(ProjectInformation project)
        {
            this.m_project = project;
        }

        public void SetGUIDMap(Dictionary<string, string> guidMap)
        {
            this.m_guidMap = guidMap;
        }

        public void SetUserAssets(List<string> userAssets)
        {
            this.m_userAssets = userAssets;
        }

        protected override void OnInspectorGUI()
        {
            if (this.m_project == null)
            {
                this.Tool.ChangeToView(View.Import);
                this.Reset();
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(Styles.TRANSFER_INFO_LABEL, EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(Styles.TRANSFER_FILES_TITLE_LABEL, EditorStyles.largeLabel);
            this.m_scrollPos = EditorGUILayout.BeginScrollView(this.m_scrollPos);

            foreach (string item in this.m_userAssets)
            {
                EditorGUILayout.LabelField(new GUIContent(Path.GetFileName(item), item));
            }
            EditorGUILayout.EndScrollView();


            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(Styles.CANCEL_BUTTON_LABEL, GUILayout.ExpandWidth(false)))
            {
                this.Tool.ChangeToView(View.Import);
                this.Reset();
            }
            if (GUILayout.Button(Styles.TRANSFER_BUTTON_LABEL, GUILayout.ExpandWidth(false)))
            {
                var transferFinishedView = this.Tool.GetViewComponent<TransferFinishedComponent>(View.TransferFinished);
                transferFinishedView.ClearTransferExceptions();
                for (int index = 0, userAssetCount = this.m_userAssets.Count; index < userAssetCount; index++)
                {
                    string userAsset = this.m_userAssets[index];
                    EditorUtility.DisplayProgressBar("Transfer", $"Transferring asset '{userAsset}'\n[{index + 1}/{userAssetCount}]", (float)(index + 1) / userAssetCount);

                    try
                    {
                        if (MigrationUtility.MapAndTransferAsset(userAsset, this.m_project.AssetsPath, this.m_guidMap))
                        {
                            Debug.Log($"Transferred asset '<color=orange>{userAsset}</color>' successfully!");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Failed transferring asset '<color=orange>{userAsset}</color>': {ex}");
                        transferFinishedView.AddTransferException(userAsset, ex);
                    }
                }
                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();
                this.Tool.ChangeToView(View.TransferFinished);
                this.Reset();
            }
            EditorGUILayout.EndHorizontal();
        }

        public override void Reset()
        {
            base.Reset();
            this.m_project = null;
            this.m_guidMap = null;
            this.m_userAssets = null;
            this.m_scrollPos = Vector2.zero;
        }

        #region Constant GUI Contents
        private static class Styles
        {
            public static GUIContent TRANSFER_INFO_LABEL;
            public static GUIContent TRANSFER_BUTTON_LABEL;
            public static GUIContent TRANSFER_FILES_TITLE_LABEL;
            public static GUIContent CANCEL_BUTTON_LABEL;

            public static bool HAS_GUI_CONSTANTS = false;

            public static void RefreshGUIConstants()
            {
                HAS_GUI_CONSTANTS = true;

                TRANSFER_BUTTON_LABEL = new GUIContent("Transfer");
                CANCEL_BUTTON_LABEL = new GUIContent("Cancel");
                TRANSFER_FILES_TITLE_LABEL = new GUIContent("Transferring Files:");
                TRANSFER_INFO_LABEL = new GUIContent("You are now closer to transferring over your assets to this project. View the files to transfer below. You can hover over them to view the full asset path.");
            }
        }
        #endregion
    }
}

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using GTFO.DevTools.Utilities;
using System.IO;
using System;
using static GTFO.DevTools.Components.Migration.MigrationToolComponent;
using Newtonsoft.Json;
using GTFO.DevTools.Migration;
using System.Linq;

namespace GTFO.DevTools.Components.Migration
{
    public class ExportComponent : MigrationToolViewComponent
    {
        private FolderInfo m_assetsFolder;
        private Vector2 m_scrollPos;

        public ExportComponent(MigrationToolComponent parent) : base(parent, View.Export)
        { }

        public override bool NeedStyleRefresh => !Styles.HAS_GUI_CONSTANTS;
        public override void RefreshStyle()
        {
            Styles.RefreshGUIConstants();
        }

        protected override void OnInspectorGUI()
        {
            if (this.m_assetsFolder == null)
            {
                this.m_assetsFolder = new FolderInfo(Application.dataPath);
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(Styles.EXPORT_LABEL, EditorStyles.largeLabel);
            if (GUILayout.Button(Styles.BACK_BUTTON_LABEL, GUILayout.ExpandWidth(false)))
            {
                this.Tool.ChangeToView(View.None);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField(Styles.EXPORT_INFO_LABEL, EditorStyles.wordWrappedLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(Styles.SELECT_USER_ASSETS_TITLE, EditorStyles.boldLabel);
            if (GUILayout.Button(Styles.REFRESH_ASSETS_BUTTON_LABEL, GUILayout.ExpandWidth(false)))
            {
                this.m_assetsFolder.Refresh(Application.dataPath);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField(Styles.SELECT_USER_ASSETS_HELP, EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField(Styles.ADD_REMOVE_ASSET_HELP, EditorStyles.wordWrappedLabel);
            this.m_scrollPos = EditorGUILayout.BeginScrollView(this.m_scrollPos);
            this.m_assetsFolder.OnGUI();
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button(Styles.BUILD_PROJECTINFO_BUTTON_LABEL))
            {
                try
                {
                    this.BuildProjectInformation();
                }
                catch (Exception ex)
                {
                    this.Tool.GetViewComponent<ExportFailedComponent>(View.ExportFailed).SetFailException(ex);
                    this.Tool.ChangeToView(View.ExportFailed);
                    return;
                }

                this.Tool.ChangeToView(View.ExportFinished);
            }
        }

        public void BuildProjectInformation()
        {
            IEnumerable<string> userFiles = this.m_assetsFolder.CollectFiles(Path.GetDirectoryName(Application.dataPath))
                .Select(file => PathUtil.FullPathToAssetPath(file, Application.dataPath));
            ProjectInformation information = MigrationUtility.CreateProjectInformation(Application.dataPath, userFiles);

            File.WriteAllText(Path.Combine(Path.GetDirectoryName(Application.dataPath), "information.projinfo"), JsonConvert.SerializeObject(information.ToRaw()));
        }

        private sealed class FolderInfo
        {
            public List<FileInfo> files = new List<FileInfo>();
            public List<FolderInfo> folders = new List<FolderInfo>();
            public bool foldout = false;
            public string name;
            public GUIContent nameContent;

            public FolderInfo(string path)
            {
                this.name = Path.GetFileName(path);
                this.nameContent = new GUIContent(this.name, PathUtil.NiceRelativePath(Path.GetDirectoryName(Application.dataPath), path));
                this.FetchFilesAndFolders(path);
            }

            public List<string> CollectFiles(string parentDirectory)
            {
                List<string> files = new List<string>();
                this.CollectFiles(files, parentDirectory);
                return files;
            }

            public void CollectFiles(List<string> files, string parentDirectory)
            {
                string directory = Path.Combine(parentDirectory, this.name);
                foreach (FileInfo file in this.files)
                {
                    if (!file.include)
                    {
                        continue;
                    }

                    files.Add(Path.Combine(directory, file.name));
                }

                foreach (FolderInfo folder in this.folders)
                {
                    folder.CollectFiles(files, directory);
                }
            }

            private void FetchFilesAndFolders(string path)
            {
                this.files.AddRange(Directory.GetFiles(path)
                    .Where(filePath => !filePath.EndsWith(".meta"))
                    .Select(filePath => new FileInfo(filePath)));
                this.folders.AddRange(Directory.GetDirectories(path)
                    .Select(folderPath => new FolderInfo(folderPath)));
            }

            public void OnGUI(bool exclude = false)
            {
                Color color = GUI.color;
                exclude |= !this.HasIncludedMembers;
                if (exclude)
                    GUI.color = Color.gray;
                EditorGUILayout.BeginHorizontal();
                this.foldout = EditorGUILayout.Foldout(this.foldout, this.nameContent, true);
                GUI.color = color;

                if (exclude && GUILayout.Button(Styles.INCLUDE_LABEL, GUILayout.ExpandWidth(false)))
                {
                    this.IncludeAll();
                }
                if (!exclude && GUILayout.Button(Styles.EXCLUDE_LABEL, GUILayout.ExpandWidth(false)))
                {
                    this.ExcludeAll();
                }
                EditorGUILayout.EndHorizontal();
                if (!this.foldout)
                    return;

                EditorGUI.indentLevel++;

                foreach (FolderInfo folder in this.folders)
                {
                    folder.OnGUI(exclude);
                }
                foreach (FileInfo file in this.files)
                {
                    file.OnGUI();
                }

                EditorGUI.indentLevel--;
            }

            public bool HasIncludedMembers
            {
                get
                {
                    return this.files.Any(file => file.include) ||
                        this.folders.Any(folder => folder.HasIncludedMembers);
                }
            }

            public void IncludeAll()
            {
                foreach (FolderInfo folder in this.folders)
                {
                    folder.IncludeAll();
                }
                foreach (FileInfo file in this.files)
                {
                    file.include = true;
                }
            }

            public void ExcludeAll()
            {
                foreach (FolderInfo folder in this.folders)
                {
                    folder.ExcludeAll();
                }
                foreach (FileInfo file in this.files)
                {
                    file.include = false;
                }
            }

            public void Refresh(string path)
            {
                this.files.Clear();
                this.folders.Clear();

                this.FetchFilesAndFolders(path);
            }
        }

        private sealed class FileInfo
        {
            public string name;
            public GUIContent nameContent;
            public bool include;

            public FileInfo(string path, bool include = false)
            {
                this.name = Path.GetFileName(path);
                this.include = include;
                this.nameContent = new GUIContent(this.name, PathUtil.NiceRelativePath(Path.GetDirectoryName(Application.dataPath), path));
            }

            public void OnGUI()
            {
                Color color = GUI.color;
                if (!this.include)
                    GUI.color = Color.gray;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(this.nameContent);
                GUI.color = color;
                if (GUILayout.Button(this.include ? Styles.EXCLUDE_LABEL : Styles.INCLUDE_LABEL, GUILayout.ExpandWidth(false)))
                {
                    this.include = !this.include;
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        #region Constant GUI Contents
        private static class Styles
        {
            public static GUIContent EXPORT_LABEL;
            public static GUIContent EXPORT_INFO_LABEL;
            public static GUIContent BACK_BUTTON_LABEL;
            public static GUIContent REFRESH_ASSETS_BUTTON_LABEL;
            public static GUIContent ADD_REMOVE_ASSET_HELP;
            public static GUIContent SELECT_USER_ASSETS_TITLE;
            public static GUIContent SELECT_USER_ASSETS_HELP;
            public static GUIContent BUILD_PROJECTINFO_BUTTON_LABEL;
            public static GUIContent INCLUDE_LABEL;
            public static GUIContent EXCLUDE_LABEL;

            public static bool HAS_GUI_CONSTANTS = false;

            public static void RefreshGUIConstants()
            {
                HAS_GUI_CONSTANTS = true;
                BACK_BUTTON_LABEL = new GUIContent("Back", "Go back");
                INCLUDE_LABEL = new GUIContent("+", "Include this file/folder");
                EXCLUDE_LABEL = new GUIContent("-", "Exclude this file/folder");
                EXPORT_LABEL = new GUIContent("Export");
                EXPORT_INFO_LABEL = new GUIContent("Prepares assets to be exported so you can transfer them over in the other project.");
                REFRESH_ASSETS_BUTTON_LABEL = new GUIContent("Refresh", "Refresh the list of assets. This will unselect and refold all items.");
                ADD_REMOVE_ASSET_HELP = new GUIContent("Click + to add, - to remove");
                SELECT_USER_ASSETS_TITLE = new GUIContent("Select User Assets");
                SELECT_USER_ASSETS_HELP = new GUIContent("Select all assets that you have created to be setup for export");
                BUILD_PROJECTINFO_BUTTON_LABEL = new GUIContent("Setup Project for Export");
            }
        }
        #endregion
    }
}

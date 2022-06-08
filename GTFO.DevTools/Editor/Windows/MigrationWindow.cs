using GTFO.DevTools.Migration;
using GTFO.DevTools.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using Unity.VisualScripting;
using System.Text;

namespace GTFO.DevTools.Windows
{
    public class MigrationWindow : EditorWindow
    {
        private FolderInfo m_assetsFolder;
        private Vector2 m_scrollPos;
        private Exception m_failException;
        private View m_view = View.None;
        private string m_importPath = "";
        private string m_importFailReason = "";
        private ProjectInformation m_project;
        private Dictionary<string, string> m_guidMap = new Dictionary<string, string>();
        private List<string> m_userAssets = new List<string>();
        private List<Exception> m_transferExceptions = new List<Exception>();

        private void OnGUI()
        {
            this.titleContent = Styles.TITLE;
            
            switch (this.m_view)
            {
                case View.None:
                    this.DefaultGUI();
                    break;
                case View.Export:
                    this.ExportGUI();
                    break;
                case View.ExportFailed:
                    this.ExportFailedGUI();
                    break;
                case View.ExportFinished:
                    this.ExportFinishedGUI();
                    break;
                case View.Import:
                    this.ImportGUI();
                    break;
                case View.ImportFile:
                    this.ImportFileGUI();
                    break;
                case View.Importing:
                    this.ImportingGUI();
                    break;
                case View.ImportFailed:
                    this.ImportFailedGUI();
                    break;
                case View.Transfer:
                    this.TransferGUI();
                    break;
                case View.TransferFinished:
                    this.TransferFinishedGUI();
                    break;
            }
        }

        private void TransferFinishedGUI()
        {
            EditorGUILayout.LabelField("Transfer Complete", EditorStyles.largeLabel);
            EditorGUILayout.LabelField("Assets should have been transferred over to this new project!", EditorStyles.wordWrappedLabel);
            if (this.m_transferExceptions.Count > 0)
            {
                EditorGUILayout.LabelField("There were a couple of errors, though. Be sure to paste some of these errors into the #unity-development channel.", EditorStyles.wordWrappedLabel);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField($"Errors ({this.m_transferExceptions.Count})");
                this.m_scrollPos = EditorGUILayout.BeginScrollView(this.m_scrollPos);
                foreach (Exception error in this.m_transferExceptions)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(new GUIContent(error.Message, error.ToString()), EditorStyles.wordWrappedLabel);
                    if (GUILayout.Button("Copy", GUILayout.ExpandWidth(false)))
                    {
                        GUIUtility.systemCopyBuffer = error.ToString();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
                if (GUILayout.Button("Copy All", GUILayout.ExpandWidth(false)))
                {
                    StringBuilder builder = new StringBuilder();
                    foreach (Exception error in this.m_transferExceptions)
                    {
                        builder.AppendLine(error.ToString());
                        builder.AppendLine();
                    }
                    GUIUtility.systemCopyBuffer = builder.ToString();
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Back"))
            {
                this.m_view = View.Transfer;
            }
            if (GUILayout.Button("Close"))
            {
                this.DoClose();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DoClose()
        {
            this.m_view = View.None;
            this.m_guidMap.Clear();
            this.m_importPath = "";
            this.m_project = null;
            this.m_userAssets.Clear();
            this.Close();
        }

        private void TransferGUI()
        {
            if (this.m_project == null)
            {
                this.m_view = View.Import;
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("You are now closer to transferring over your assets to this project. View the files to transfer below. You can hover over them to view the full asset path.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Transferring Files:", EditorStyles.largeLabel);
            this.m_scrollPos = EditorGUILayout.BeginScrollView(this.m_scrollPos);

            foreach (string item in this.m_project.GetUserAssets())
            {
                EditorGUILayout.LabelField(new GUIContent(Path.GetFileName(item), item));
            }
            EditorGUILayout.EndScrollView();


            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Cancel", GUILayout.ExpandWidth(false)))
            {
                this.m_view = View.Import;
            }
            if (GUILayout.Button("Transfer", GUILayout.ExpandWidth(false)))
            {
                this.m_transferExceptions.Clear();
                for (int index = 0, userAssetCount = this.m_userAssets.Count; index < userAssetCount; index++)
                {
                    string userAsset = this.m_userAssets[index];
                    EditorUtility.DisplayProgressBar("Transfer", "Transferring asset '{userAsset}'\n[{index + 1}/{userAssetCount}]", (float)(index + 1) / userAssetCount);

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
                        this.m_transferExceptions.Add(ex);
                    }
                }
                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();
                this.m_view = View.TransferFinished;
                this.m_scrollPos = Vector2.zero;
            }
            EditorGUILayout.EndHorizontal();
        }

        private void ImportingGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Importing...");

            if (!File.Exists(this.m_importPath))
            {
                this.m_importFailReason = "No such file at that path exists";
                this.m_view = View.ImportFailed;
                return;
            }

            string contents;
            try
            {
                contents = File.ReadAllText(this.m_importPath);
            }
            catch (Exception ex)
            {
                this.m_importFailReason = "Uncaught Exception whilst reading file: " + ex;
                this.m_view = View.ImportFailed;
                return;
            }

            ProjectInformation.Raw jsonProject;
            try
            {
                jsonProject = JsonConvert.DeserializeObject<ProjectInformation.Raw>(contents);
            }
            catch (Exception ex)
            {
                this.m_importFailReason = "Uncaught Exception whilst parsing json: " + ex;
                this.m_view = View.ImportFailed;
                return;
            }

            if (jsonProject == null)
            {
                this.m_importFailReason = "Imported Project File is empty!";
                this.m_view = View.ImportFailed;
                return;
            }

            this.m_project = jsonProject.ToInformation();
            this.m_scrollPos = Vector2.zero;
            this.m_guidMap.Clear();
            this.m_userAssets.Clear();
            this.m_userAssets.AddRange(this.m_project.GetUserAssets());
            foreach (ProjectAsset asset in this.m_project.GetAssets())
            {
                string assetPath = asset.AssetPath;
                if (this.m_guidMap.ContainsKey(asset.GUID))
                {
                    Debug.LogWarning($"An asset with GUID '<color=cyan>{asset.GUID}</color>' already has a mapping! Mapped to guid '<color=orange>{this.m_guidMap[asset.GUID]}</color>'. Asset path: '<color=green>{assetPath}</color>'");
                    continue;
                }

                if (assetPath.StartsWith("Assets/Scripts"))
                {
                    assetPath = "Assets/MonoScript" + assetPath.Substring("Assets/Scripts".Length);
                }
                if (assetPath.StartsWith("/Assets/Scripts"))
                {
                    assetPath = "/Assets/MonoScript" + assetPath.Substring("/Assets/Scripts".Length);
                }

                string guid = AssetDatabase.AssetPathToGUID(assetPath);
                if (string.IsNullOrWhiteSpace(guid))
                {
                    Debug.LogWarning($"No mapping asset path found at '<color=orange>{assetPath}</color>'. This asset wont be mapped to an asset in this project!");
                    this.m_guidMap.Add(asset.GUID, asset.GUID);
                    continue;
                }

                Debug.Log($"Mapping guid '<color=orange>{asset.GUID}</color>' to guid '<color=cyan>{guid}</color>'");
                this.m_guidMap.Add(asset.GUID, guid);
            }
            this.m_view = View.Transfer;
        }

        private void ImportFailedGUI()
        {
            EditorGUILayout.LabelField("A fatal error has occurred which resulted in failure to import project.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(this.m_importFailReason, EditorStyles.wordWrappedLabel);

            EditorGUILayout.Space();
            if (GUILayout.Button("Back", GUILayout.ExpandWidth(false)))
            {
                this.m_view = View.Import;
            }
        }

        private void ImportFileGUI()
        {
            this.m_importPath = EditorGUILayout.TextField(this.m_importPath);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Import"))
            {
                this.m_view = View.Importing;
            }
            if (GUILayout.Button("Back"))
            {
                this.m_view = View.Import;
            }
            EditorGUILayout.EndHorizontal();
        }

        private void ImportGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Import", EditorStyles.largeLabel);
            if (GUILayout.Button("Back", GUILayout.ExpandWidth(false)))
            {
                this.m_view = View.None;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("Imports prepared assets from another project.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("A prepared project will have an 'information.projinfo' file outside of it's assets folder.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField("Click Import below to select this file", EditorStyles.wordWrappedLabel);
            if (GUILayout.Button("Import"))
            {
                string basicPath = Path.GetDirectoryName(Path.GetDirectoryName(Application.dataPath));
                string folder = EditorUtility.OpenFilePanel("Import Project", basicPath, "projinfo");
                if (!string.IsNullOrWhiteSpace(folder))
                {
                    this.m_importPath = folder;
                    this.m_view = View.Importing;
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("You can also Import Project from a 'projinfo' file path. This should be used if you clicked 'Copy Path' for the project you setup for export.", EditorStyles.wordWrappedLabel);
            if (GUILayout.Button("Import from File Path"))
            {
                this.m_view = View.ImportFile;
            }
        }

        private void DefaultGUI()
        {
            EditorGUILayout.LabelField("Migration Tool", EditorStyles.largeLabel);
            EditorGUILayout.LabelField("Migrate assets between projects to develop with newer versions of GTFO", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Export will prepare a project for export. Choose this for projects you want to transfer assets from.", EditorStyles.wordWrappedLabel);
            if (GUILayout.Button("Export"))
            {
                this.m_view = View.Export;
            }
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Import/Transfer will import a project into this project, transferring over your assets whilst updating them to match this project's structure. Choose this to transfer assets to this project.", EditorStyles.wordWrappedLabel);
            if (GUILayout.Button("Import/Transfer Project"))
            {
                this.m_view = View.Import;
            }
        }
        private void ExportFailedGUI()
        {
            if (this.m_failException == null)
            {
                this.m_view = View.Export;
                return;
            }

            string failEx = this.m_failException.ToString();
            EditorGUILayout.LabelField("A fatal error has occurred which resulted in export failure.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(failEx, EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField("Click below to copy this for help troubleshooting in the #unity-development channel.", EditorStyles.wordWrappedLabel);

            if (GUILayout.Button("Copy Exception", GUILayout.ExpandWidth(false)))
            {
                GUIUtility.systemCopyBuffer = failEx;
            }
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Back", GUILayout.ExpandWidth(false)))
            {
                this.m_view = View.None;
            }
            if (GUILayout.Button("Close", GUILayout.ExpandWidth(false)))
            {
                this.DoClose();
            }
            EditorGUILayout.EndHorizontal();
        }
        private void ExportFinishedGUI()
        {
            string projInfoPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "information.projinfo");
            EditorGUILayout.LabelField("This project has been setup for export successfully.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("For reference when importing and transferring assets from this project, this projects 'information.projinfo' file is at " + projInfoPath, EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField("Click below to copy this path.", EditorStyles.wordWrappedLabel);

            if (GUILayout.Button("Copy Path", GUILayout.ExpandWidth(false)))
            {
                GUIUtility.systemCopyBuffer = projInfoPath;
            }
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Back", GUILayout.ExpandWidth(false)))
            {
                this.m_view = View.None;
            }
            if (GUILayout.Button("Close", GUILayout.ExpandWidth(false)))
            {
                this.DoClose();
            }
            EditorGUILayout.EndHorizontal();
        }
        private void ExportGUI()
        {
            if (this.m_assetsFolder == null)
            {
                this.m_assetsFolder = new FolderInfo(Application.dataPath);
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Export", EditorStyles.largeLabel);
            if (GUILayout.Button("Back", GUILayout.ExpandWidth(false)))
            {
                this.m_view = View.None;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("Prepares assets to be exported so you can transfer them over in the other project.", EditorStyles.wordWrappedLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Select User Assets", EditorStyles.boldLabel);
            if (GUILayout.Button("Refresh", GUILayout.ExpandWidth(false)))
            {
                this.m_assetsFolder.Refresh(Application.dataPath);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("Select all assets that you have created to be setup for export", EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField("Click + to add, - to remove", EditorStyles.wordWrappedLabel);
            this.m_scrollPos = EditorGUILayout.BeginScrollView(this.m_scrollPos);
            this.m_assetsFolder.OnGUI();
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Build"))
            {
                try
                {
                    this.BuildProjectInformation();
                }
                catch (Exception ex)
                {
                    this.m_failException = ex;
                    this.m_view = View.ExportFailed;
                    return;
                }

                this.m_view = View.ExportFinished;
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

        [MenuItem("Window/GTFO.DevTools/Migration Tool")]
        public static void Test()
        {
            GetWindow<MigrationWindow>().Show();
        }

        private enum View
        {
            None,
            Export,
            ExportFinished,
            ExportFailed,
            Import,
            ImportFile,
            Importing,
            ImportFailed,
            Transfer,
            TransferFinished
        }

        #region Styles
        private static class Styles
        {
            public static GUIContent TITLE;
            public static GUIContent ERROR_ICON;
            public static GUIContent INCLUDE_LABEL;
            public static GUIContent EXCLUDE_LABEL;

            static Styles()
            {
                TITLE = new GUIContent("Migration Window");
                ERROR_ICON = EditorGUIUtility.IconContent("console.erroricon");
                INCLUDE_LABEL = new GUIContent("+", "Include this file/folder");
                EXCLUDE_LABEL = new GUIContent("-", "Exclude this file/folder");
            }
        }
        #endregion
    }
}

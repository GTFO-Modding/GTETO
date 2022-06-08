using GTFO.DevTools.Migration;
using GTFO.DevTools.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
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
        private List<TransferExceptionInfo> m_transferExceptions = new List<TransferExceptionInfo>();

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
                this.m_view = View.Transfer;
            }
            if (GUILayout.Button(Styles.CLOSE_BUTTON_LABEL))
            {
                this.DoClose();
            }
            EditorGUILayout.EndHorizontal();
        }
        private void TransferGUI()
        {
            if (this.m_project == null)
            {
                this.m_view = View.Import;
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(Styles.TRANSFER_INFO_LABEL, EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(Styles.TRANSFER_FILES_TITLE_LABEL, EditorStyles.largeLabel);
            this.m_scrollPos = EditorGUILayout.BeginScrollView(this.m_scrollPos);

            foreach (string item in this.m_project.GetUserAssets())
            {
                EditorGUILayout.LabelField(new GUIContent(Path.GetFileName(item), item));
            }
            EditorGUILayout.EndScrollView();


            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(Styles.CANCEL_BUTTON_LABEL, GUILayout.ExpandWidth(false)))
            {
                this.m_view = View.Import;
            }
            if (GUILayout.Button(Styles.TRANSFER_BUTTON_LABEL, GUILayout.ExpandWidth(false)))
            {
                this.m_transferExceptions.Clear();
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
                        this.m_transferExceptions.Add(new TransferExceptionInfo(userAsset, ex));
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
            EditorGUILayout.LabelField(Styles.IMPORTING_LABEL);

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
            EditorGUILayout.LabelField(Styles.IMPORT_FAILED_INFO_LABEL, EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(this.m_importFailReason, EditorStyles.wordWrappedLabel);

            EditorGUILayout.Space();
            if (GUILayout.Button(Styles.BACK_BUTTON_LABEL, GUILayout.ExpandWidth(false)))
            {
                this.m_view = View.Import;
            }
        }
        private void ImportFileGUI()
        {
            EditorGUILayout.LabelField(Styles.IMPORT_FILE_PATH_LABEL);
            this.m_importPath = EditorGUILayout.TextField(this.m_importPath);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(Styles.IMPORT_FILE_PATH_IMPORT_BUTTON_LABEL))
            {
                this.m_view = View.Importing;
            }
            if (GUILayout.Button(Styles.BACK_BUTTON_LABEL))
            {
                this.m_view = View.Import;
            }
            EditorGUILayout.EndHorizontal();
        }
        private void ImportGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(Styles.IMPORT_TITLE, EditorStyles.largeLabel);
            if (GUILayout.Button(Styles.BACK_BUTTON_LABEL, GUILayout.ExpandWidth(false)))
            {
                this.m_view = View.None;
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
                    this.m_importPath = folder;
                    this.m_view = View.Importing;
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(Styles.IMPORT_FROM_PATH_INFO_LABEL, EditorStyles.wordWrappedLabel);
            if (GUILayout.Button(Styles.IMPORT_FROM_PATH_BUTTON_LABEL))
            {
                this.m_view = View.ImportFile;
            }
        }
        private void DefaultGUI()
        {
            EditorGUILayout.LabelField(Styles.MIGRATE_TOOL_TITLE, EditorStyles.largeLabel);
            EditorGUILayout.LabelField(Styles.MIGRATE_TOOL_INFO, EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(Styles.EXPORT_PROJECT_HINT, EditorStyles.wordWrappedLabel);
            if (GUILayout.Button(Styles.EXPORT_PROJECT_BUTTON))
            {
                this.m_view = View.Export;
            }
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(Styles.IMPORT_PROJECT_HINT, EditorStyles.wordWrappedLabel);
            if (GUILayout.Button(Styles.IMPORT_PROJECT_BUTTON))
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
                this.m_view = View.None;
            }
            if (GUILayout.Button(Styles.CLOSE_BUTTON_LABEL, GUILayout.ExpandWidth(false)))
            {
                this.DoClose();
            }
            EditorGUILayout.EndHorizontal();
        }
        private void ExportFinishedGUI()
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
                this.m_view = View.None;
            }
            if (GUILayout.Button(Styles.CLOSE_BUTTON_LABEL, GUILayout.ExpandWidth(false)))
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
            EditorGUILayout.LabelField(Styles.EXPORT_LABEL, EditorStyles.largeLabel);
            if (GUILayout.Button(Styles.BACK_BUTTON_LABEL, GUILayout.ExpandWidth(false)))
            {
                this.m_view = View.None;
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

        private void DoClose()
        {
            this.m_view = View.None;
            this.m_guidMap.Clear();
            this.m_importPath = "";
            this.m_project = null;
            this.m_userAssets.Clear();
            this.Close();
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

        [MenuItem("Window/GTFO.DevTools/Migration Tool")]
        public static void OpenWindow()
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
            public static GUIContent TRANSFER_COMPLETED_TITLE_LABEL;
            public static GUIContent TRANSFER_COMPLETED_INFO_LABEL;
            public static GUIContent TRANSFER_ERRORS_INFO_LABEL;
            public static GUIContent TRANSFER_FILES_TITLE_LABEL;
            public static GUIContent TRANSFER_INFO_LABEL;
            public static GUIContent TRANSFER_BUTTON_LABEL;
            public static GUIContent IMPORT_TITLE;
            public static GUIContent TITLE;
            public static GUIContent MIGRATE_TOOL_TITLE;
            public static GUIContent ERROR_ICON;
            public static GUIContent BACK_BUTTON_LABEL;
            public static GUIContent CLOSE_BUTTON_LABEL;
            public static GUIContent INCLUDE_LABEL;
            public static GUIContent EXCLUDE_LABEL;
            public static GUIContent EXPORT_LABEL;
            public static GUIContent EXPORT_INFO_LABEL;
            public static GUIContent REFRESH_ASSETS_BUTTON_LABEL;
            public static GUIContent ADD_REMOVE_ASSET_HELP;
            public static GUIContent SELECT_USER_ASSETS_TITLE;
            public static GUIContent SELECT_USER_ASSETS_HELP;
            public static GUIContent BUILD_PROJECTINFO_BUTTON_LABEL;
            public static GUIContent COPY_PATH_BUTTON_LABEL;
            public static GUIContent COPY_PATH_INSTRUCTIONS_LABEL;
            public static GUIContent PROJECT_INFO_LOCATION_HINT_LABEL;
            public static GUIContent PROJECT_EXPORT_SUCCESS_INFO;
            public static GUIContent PROJECT_EXPORT_FAILURE_INFO;
            public static GUIContent COPY_EXCEPTION_BUTTON_LABEL;
            public static GUIContent COPY_SINGLE_EXCEPTION_BUTTON_LABEL;
            public static GUIContent COPY_ALL_EXCEPTIONS_BUTTON_LABEL;
            public static GUIContent COPY_EXCEPTION_HINT_LABEL;
            public static GUIContent MIGRATE_TOOL_INFO;
            public static GUIContent EXPORT_PROJECT_HINT;
            public static GUIContent IMPORT_PROJECT_HINT;
            public static GUIContent IMPORT_PROJECT_BUTTON;
            public static GUIContent EXPORT_PROJECT_BUTTON;
            public static GUIContent IMPORT_INFO_LABEL;
            public static GUIContent PROJECT_INFO_LOCATION_IMPORT_HINT_LABEL;
            public static GUIContent IMPORT_FROM_FILE_BUTTON_LABEL;
            public static GUIContent IMPORT_FROM_FILE_INFO_LABEL;
            public static GUIContent IMPORT_FROM_PATH_INFO_LABEL;
            public static GUIContent IMPORT_FROM_PATH_BUTTON_LABEL;
            public static GUIContent IMPORT_FILE_PATH_LABEL;
            public static GUIContent IMPORT_FILE_PATH_IMPORT_BUTTON_LABEL;
            public static GUIContent IMPORT_FAILED_INFO_LABEL;
            public static GUIContent IMPORTING_LABEL;
            public static GUIContent CANCEL_BUTTON_LABEL;

            static Styles()
            {
                TRANSFER_COMPLETED_TITLE_LABEL = new GUIContent("Transfer Complete");
                TRANSFER_COMPLETED_INFO_LABEL = new GUIContent("Assets should have been transferred over to this new project!");
                TRANSFER_BUTTON_LABEL = new GUIContent("Transfer");
                TRANSFER_ERRORS_INFO_LABEL = new GUIContent("There were a couple of errors, though. Be sure to paste some of these errors into the #unity-development channel.");
                COPY_ALL_EXCEPTIONS_BUTTON_LABEL = new GUIContent("Copy All", "Copy all exception messages");
                COPY_SINGLE_EXCEPTION_BUTTON_LABEL = new GUIContent("Copy", "Copy the full exception message");
                CANCEL_BUTTON_LABEL = new GUIContent("Cancel");
                TRANSFER_FILES_TITLE_LABEL = new GUIContent("Transferring Files:");
                TRANSFER_INFO_LABEL = new GUIContent("You are now closer to transferring over your assets to this project. View the files to transfer below. You can hover over them to view the full asset path.");
                IMPORTING_LABEL = new GUIContent("Importing...");
                IMPORT_FAILED_INFO_LABEL = new GUIContent("A fatal error has occurred which resulted in failure to import project.");
                IMPORT_TITLE = new GUIContent("Import");
                IMPORT_INFO_LABEL = new GUIContent("Imports prepared assets from another project.");
                PROJECT_INFO_LOCATION_IMPORT_HINT_LABEL = new GUIContent("A prepared project will have an 'information.projinfo' file outside of it's assets folder.");
                IMPORT_FROM_FILE_BUTTON_LABEL = new GUIContent("Import");
                IMPORT_FROM_FILE_INFO_LABEL = new GUIContent("Click Import below to select this file");
                IMPORT_FROM_PATH_INFO_LABEL = new GUIContent("You can also Import Project from a 'projinfo' file path. This should be used if you clicked 'Copy Path' for the project you setup for export.");
                IMPORT_FROM_PATH_BUTTON_LABEL = new GUIContent("Import from File Path");
                IMPORT_FILE_PATH_LABEL = new GUIContent("File Path");
                IMPORT_FILE_PATH_IMPORT_BUTTON_LABEL = new GUIContent("Import");
                EXPORT_PROJECT_BUTTON = new GUIContent("Export");
                IMPORT_PROJECT_BUTTON = new GUIContent("Import/Transfer Project");
                IMPORT_PROJECT_HINT = new GUIContent("Import/Transfer will import a project into this project, transferring over your assets whilst updating them to match this project's structure. Choose this to transfer assets to this project.");
                EXPORT_PROJECT_HINT = new GUIContent("Export will prepare a project for export. Choose this for projects you want to transfer assets from.");
                MIGRATE_TOOL_INFO = new GUIContent("Migrate assets between projects to develop with newer versions of GTFO");
                MIGRATE_TOOL_TITLE = new GUIContent("Migrate Tool");
                COPY_EXCEPTION_BUTTON_LABEL = new GUIContent("Click below to copy this for help troubleshooting in the #unity-development channel.");
                COPY_EXCEPTION_BUTTON_LABEL = new GUIContent("Copy Exception");
                PROJECT_EXPORT_FAILURE_INFO = new GUIContent("A fatal error has occurred which resulted in export failure.");
                PROJECT_EXPORT_SUCCESS_INFO = new GUIContent("This project has been setup for export successfully.");
                PROJECT_INFO_LOCATION_HINT_LABEL = new GUIContent("For reference when importing and transferring assets from this project, this projects 'information.projinfo' file is at ");
                COPY_PATH_INSTRUCTIONS_LABEL = new GUIContent("Click below to copy this path.");
                COPY_PATH_BUTTON_LABEL = new GUIContent("Copy Path");
                TITLE = new GUIContent("Migration Window");
                ERROR_ICON = EditorGUIUtility.IconContent("console.erroricon");
                BACK_BUTTON_LABEL = new GUIContent("Back", "Go back");
                CLOSE_BUTTON_LABEL = new GUIContent("Close", "Close this window.");
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

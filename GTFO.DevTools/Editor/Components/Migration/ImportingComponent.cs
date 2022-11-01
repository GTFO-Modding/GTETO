using UnityEngine;
using UnityEditor;
using static GTFO.DevTools.Components.Migration.MigrationToolComponent;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GTFO.DevTools.Migration;
using System;
using System.Collections.Generic;

namespace GTFO.DevTools.Components.Migration
{
    public class ImportingComponent : MigrationToolViewComponent
    {
        public ImportingComponent(MigrationToolComponent parent) : base(parent, View.Importing)
        { }

        private ProjectInformation m_project;
        private State m_state;
        private Dictionary<string, string> m_guidMap;
        private List<string> m_userAssets;

        private string m_importPath;
        private string m_contents;
        private Task<string> m_readFileTask;

        public void SetImportPath(string path)
        {
            this.m_importPath = path;
        }

        public override bool NeedStyleRefresh => !Styles.HAS_GUI_CONSTANTS;
        public override void RefreshStyle()
        {
            Styles.RefreshGUIConstants();
        }

        protected override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField(Styles.IMPORTING_LABEL);

            switch (this.m_state)
            {
                case State.None:
                    this.m_state = State.CheckPath;
                    break;
                case State.CheckPath:
                    if (this.CheckPath())
                    {
                        this.m_state = State.ReadFile;
                    }
                    break;
                case State.ReadFile:
                    if (this.ReadFileTask())
                    {
                        this.m_state = State.ReadJSON;
                    }
                    break;
                case State.ReadJSON:
                    if (this.ReadJSON())
                    {
                        this.m_state = State.CreateMappings;
                    }
                    break;
                case State.CreateMappings:
                    if (this.CreateMappings())
                    {
                        this.m_state = State.Ready;
                    }
                    break;
                case State.Ready:
                    {
                        TransferComponent transfer = this.Tool.GetViewComponent<TransferComponent>(View.Transfer);
                        transfer.SetProject(this.m_project);
                        transfer.SetGUIDMap(this.m_guidMap);
                        transfer.SetUserAssets(this.m_userAssets);
                        this.Reset();
                        this.Tool.ChangeToView(View.Transfer);
                    }
                    break;
            }
        }

        private enum State
        {
            None,
            CheckPath,
            ReadFile,
            ReadJSON,
            CreateMappings,
            Ready
        }

        private bool CheckPath()
        {
            if (!File.Exists(this.m_importPath))
            {
                this.Fail("No such file at that path exists");
                return false;
            }
            return true;
        }

        private bool ReadFileTask()
        {
            if (this.m_readFileTask == null)
                this.m_readFileTask = File.ReadAllTextAsync(this.m_importPath);

            if (this.m_readFileTask.IsFaulted)
            {
                this.Fail("Uncaught Exception whilst reading file: " + this.m_readFileTask.Exception);
                return false;
            }

            if (this.m_readFileTask.IsCompleted)
            {
                this.m_contents = this.m_readFileTask.Result;
                this.m_readFileTask = null;
                return true;
            }

            return false;
        }

        private bool ReadJSON()
        {
            ProjectInformation.Raw jsonProject;
            try
            {
                jsonProject = JsonConvert.DeserializeObject<ProjectInformation.Raw>(this.m_contents);
            }
            catch (Exception ex)
            {
                this.Fail("Uncaught Exception whilst parsing json: " + ex);
                return false;
            }

            if (jsonProject == null)
            {
                this.Fail("Imported Project File is empty!");
                return false;
            }

            this.m_project = jsonProject.ToInformation();
            return true;
        }

        private bool CreateMappings()
        {
            this.m_guidMap = new Dictionary<string, string>();
            this.m_userAssets = new List<string>(this.m_project.GetUserAssets());
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
            return true;
        }

        private void Fail(string reason)
        {
            this.Tool.GetViewComponent<ImportFailedComponent>(View.ImportFailed).SetFailReason(reason);
            this.Tool.ChangeToView(View.ImportFailed);
            this.Reset();
        }

        public override void Reset()
        {
            base.Reset();
            this.m_project = null;
            this.m_contents = null;
            this.m_guidMap = null;
            this.m_importPath = null;
            this.m_readFileTask = null;
            this.m_userAssets = null;
            this.m_state = State.None;
        }

        #region Constant GUI Contents
        private static class Styles
        {
            public static GUIContent IMPORTING_LABEL;

            public static bool HAS_GUI_CONSTANTS = false;

            public static void RefreshGUIConstants()
            {
                HAS_GUI_CONSTANTS = true;

                IMPORTING_LABEL = new GUIContent("Importing...");
            }
        }
        #endregion
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GTFO.DevTools.Migration
{
    public sealed class ProjectInformation
    {
        private readonly string m_assetsPath;
        private readonly List<ProjectAsset> m_assets;
        private readonly List<string> m_userAssets;

        public ProjectInformation(string assetsPath)
        {
            this.m_assetsPath = assetsPath;
            this.m_assets = new List<ProjectAsset>();
            this.m_userAssets = new List<string>();
        }

        public string AssetsPath => this.m_assetsPath;

        public string[] GetUserAssets()
        {
            return this.m_userAssets.ToArray();
        }

        public ProjectAsset[] GetAssets()
        {
            return this.m_assets.ToArray();
        }

        public bool TryGetAssetGUID(string assetPath, out string guid)
        {
            foreach (ProjectAsset asset in this.m_assets)
            {
                if (asset.AssetPath == assetPath)
                {
                    guid = asset.GUID;
                    return true;
                }
            }

            guid = null;
            return false;
        }

        public void AddAsset(string assetPath)
        {
            string guid = AssetDatabase.AssetPathToGUID(assetPath.Replace('\\', '/'));
            if (string.IsNullOrWhiteSpace(guid))
            {
                Debug.LogError("Skipping '<color=orange>" + assetPath + "</color>', as there's no guid associated with it!");
                return;
            }

            this.m_assets.Add(new ProjectAsset(guid, assetPath, Path.Combine(Path.GetDirectoryName(this.AssetsPath), assetPath)));
        }
        public void AddUserAsset(string assetPath)
        {
            this.m_userAssets.Add(assetPath);
        }

        public Raw ToRaw() => new Raw(this);

        public sealed class Raw
        {
            public string assetsPath;
            public List<ProjectAsset.Raw> assets;
            public List<string> userAssets;

            public Raw()
            {
                this.assetsPath = "";
                this.assets = new List<ProjectAsset.Raw>();
                this.userAssets = new List<string>();
            }

            public Raw(ProjectInformation information)
            {
                if (information is null)
                    throw new ArgumentNullException(nameof(information));

                this.assetsPath = information.AssetsPath;
                this.assets = new List<ProjectAsset.Raw>(information.m_assets.Select(asset => asset.ToRaw()));
                this.userAssets = new List<string>(information.m_userAssets);
            }

            public ProjectInformation ToInformation()
            {
                ProjectInformation information = new ProjectInformation(this.assetsPath);
                information.m_assets.AddRange(this.assets.Select(rawAsset => rawAsset.ToAsset(information)));
                information.m_userAssets.AddRange(this.userAssets);
                return information;
            }
        }
    }
}
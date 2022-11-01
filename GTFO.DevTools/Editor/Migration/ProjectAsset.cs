using GTFO.DevTools.Utilities;
using System;
using UnityEditor;

namespace GTFO.DevTools.Migration
{
    public sealed class ProjectAsset
    {
        private readonly string m_guid;
        private readonly string m_assetPath;
        private readonly string m_fullPath;

        public ProjectAsset(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
                throw new ArgumentException("Invalid full path given! Was null or whitespace!", nameof(fullPath));

            string assetPath = PathUtil.FullPathToAssetPath(fullPath);
            this.m_guid = AssetDatabase.AssetPathToGUID(assetPath);
            this.m_assetPath = assetPath;
            this.m_fullPath = fullPath;
        }

        public ProjectAsset(string guid, string fullPath)
        {
            if (string.IsNullOrWhiteSpace(guid))
                throw new ArgumentException("Invalid guid given! Was null or whitespace!", nameof(guid));
            if (string.IsNullOrWhiteSpace(fullPath))
                throw new ArgumentException("Invalid full path given! Was null or whitespace!", nameof(fullPath));

            string assetPath = PathUtil.FullPathToAssetPath(fullPath);
            this.m_guid = guid;
            this.m_assetPath = assetPath;
            this.m_fullPath = fullPath;
        }

        public ProjectAsset(string guid, string assetPath, string fullPath)
        {
            if (string.IsNullOrWhiteSpace(guid))
                throw new ArgumentException("Invalid guid given! Was null or whitespace!", nameof(guid));
            if (string.IsNullOrWhiteSpace(assetPath))
                throw new ArgumentException("Invalid assetPath given! Was null or whitespace!", nameof(assetPath));
            if (string.IsNullOrWhiteSpace(fullPath))
                throw new ArgumentException("Invalid full path given! Was null or whitespace!", nameof(fullPath));

            this.m_guid = guid;
            this.m_assetPath = assetPath;
            this.m_fullPath = fullPath;
        }

        public string GUID => this.m_guid;
        public string AssetPath => this.m_assetPath;
        public string FullPath => this.m_fullPath;

        public Raw ToRaw() => new Raw(this);

        public sealed class Raw
        {
            public string guid;
            public string fullPath;

            public Raw()
            {
                this.guid = "";
                this.fullPath = "";
            }

            public Raw(ProjectAsset asset)
            {
                if (asset is null)
                    throw new ArgumentNullException(nameof(asset));

                this.guid = asset.GUID;
                this.fullPath = asset.FullPath;
            }

            public ProjectAsset ToAsset(ProjectInformation information)
            {
                return new ProjectAsset(this.guid, PathUtil.FullPathToAssetPath(this.fullPath, information.AssetsPath), this.fullPath);
            }
        }
    }
}

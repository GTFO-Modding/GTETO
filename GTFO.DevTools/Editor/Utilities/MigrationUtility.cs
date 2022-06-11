using GTFO.DevTools.Migration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace GTFO.DevTools.Utilities
{
    public static class MigrationUtility
    {
        public static ProjectInformation CreateProjectInformation(string assetsPath, IEnumerable<string> userFiles)
        {
            ProjectInformation information = new ProjectInformation(assetsPath);
            HashSet<string> userFileSet = new HashSet<string>(userFiles);
            IEnumerable<string> files = CollectAllFiles(assetsPath)
                .Select(file => PathUtil.FullPathToAssetPath(file, assetsPath));
                //.Where(file =>
                //{
                //    if (userFiles.Contains(file))
                //    {
                //        Debug.LogError($"File '<color=orange>{file}</color>' is a user file!");
                //        return false;
                //    }
                //    return true;
                //});

            foreach (string file in files)
            {
                information.AddAsset(file);
            }
            foreach (string userFile in userFiles)
            {
                information.AddUserAsset(userFile);
            }
            return information;
        }

        private static readonly Regex REFERENCE_REGEX = new Regex(@"\{fileID: (?<FileID>[0-9]+), guid: (?<GUID>[a-fA-F0-9]+), type: (?<Type>[0-9]+)\}");

        public static bool MapAndTransferAsset(string assetPath, string assetFolder, Dictionary<string, string> guidMap)
        {
            string path = PathUtil.AssetPathToFullPath(assetPath, assetFolder);
            string transferPath = PathUtil.AssetPathToFullPath(assetPath);

            string contents = File.ReadAllText(path);
            if (contents.StartsWith("%YAML "))
            {
                contents = REFERENCE_REGEX.Replace(contents, (match) =>
                {
                    string guid = match.Groups["GUID"].Value;
                    if (guidMap.TryGetValue(guid, out string mappedGUID))
                    {
                        guid = mappedGUID;
                    }
                    return $"{{fileID: {match.Groups["FileID"].Value}, guid: {guid}, type: {match.Groups["Type"].Value}}}";
                });
            }

            if (File.Exists(transferPath) &&
                !EditorUtility.DisplayDialog("Transfer", $"A file already exists at path '{assetPath}'. Do you want to replace it?", "Yes", "No"))
            {
                Debug.Log($"Skipping Asset '<color=orange>{assetPath}</color>'");
                return false;
            }

            PathUtil.EnsureDirectoryExists(Path.GetDirectoryName(transferPath));

            File.WriteAllText(transferPath, contents);
            return true;
        }
        public static List<string> CollectAllFiles()
            => CollectAllFiles(Application.dataPath);
        public static List<string> CollectAllFiles(string folder)
        {
            List<string> files = new List<string>();
            CollectAllFiles(files, folder);
            return files;
        }
        private static void CollectAllFiles(List<string> files, string folder)
        {
            files.AddRange(Directory.GetFiles(folder).Where(file => !file.EndsWith(".meta")));
            foreach (string directory in Directory.GetDirectories(folder))
            {
                CollectAllFiles(files, directory);
            }
        }
    }
}

using GTFO.DevTools.Utilities;
using UnityEditor;
using UnityEngine;

namespace GTFO.DevTools
{
    public class ContextMenuExtensions
    {
        [MenuItem("GameObject/GTFO/Prefab Spawners/Build Prefab Spawners", false, 0)]
        public static void BuildPrefabSpawners(MenuCommand cmd)
        {
            GameObject obj = cmd.context as GameObject;
            if (!obj) return;

            PrefabSpawnerUtility.BuildPrefabSpawners(obj);
        }

        [MenuItem("GameObject/GTFO/Prefab Spawners/Create Prefab Spawner", false, 0)]
        public static void CreatePrefabSpawner(MenuCommand cmd)
        {
            GameObject obj = cmd.context as GameObject;
            if (!obj) return;

            PrefabSpawnerUtility.ConvertToPrefabSpawner(obj);
        }

        [MenuItem("GameObject/GTFO/Prefab Spawners/Cleanup Prefab Spawners", false, 0)]
        public static void CleanupPrefabSpawners(MenuCommand cmd)
        {
            GameObject obj = cmd.context as GameObject;
            if (!obj) return;

            PrefabSpawnerUtility.CleanupPrefabSpawners(obj);
        }

        [MenuItem("GameObject/GTFO/Markers/Randomize Markers", false, 0)]
        public static void RandomizeMarkers(MenuCommand cmd)
        {
            var obj = cmd.context as GameObject;
            if (!obj) return;

            MarkerUtility.SpawnRandomMarkers(obj);
        }


        [MenuItem("GameObject/GTFO/Markers/Cleanup Markers", false, 0)]
        public static void CleanupMarkers(MenuCommand cmd)
        {
            var obj = cmd.context as GameObject;
            if (!obj) return;

            MarkerUtility.CleanupMarkers(obj);
        }

        [MenuItem("GameObject/GTFO/Preview/Create Preview", false, 0)]
        public static void CreatePreview(MenuCommand cmd)
        {
            var obj = cmd.context as GameObject;
            if (!obj) return;

            MarkerUtility.SpawnRandomMarkers(obj);
            PrefabSpawnerUtility.BuildPrefabSpawners(obj);
        }

        [MenuItem("GameObject/GTFO/Preview/Clear Preview", false, 0)]
        public static void ClearPreview(MenuCommand cmd)
        {
            var obj = cmd.context as GameObject;
            if (!obj) return;

            MarkerUtility.CleanupMarkers(obj);
            PrefabSpawnerUtility.CleanupPrefabSpawners(obj);
        }
    }
}
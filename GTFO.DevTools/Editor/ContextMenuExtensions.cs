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
            GameObject geomorph = cmd.context as GameObject;
            PrefabSpawnerUtility.BuildPrefabSpawners(geomorph);
        }
        [MenuItem("GameObject/GTFO/Prefab Spawners/Create Prefab Spawner", false, 0)]
        public static void CreatePrefabSpawner(MenuCommand cmd)
        {
            GameObject geomorph = cmd.context as GameObject;
            PrefabSpawnerUtility.ConvertToPrefabSpawner(geomorph);
        }

        [MenuItem("GameObject/GTFO/Prefab Spawners/Cleanup Prefab Spawners", false, 0)]
        public static void CleanupPrefabSpawners(MenuCommand cmd)
        {
            GameObject geomorph = cmd.context as GameObject;
            PrefabSpawnerUtility.CleanupPrefabSpawners(geomorph);
        }

        [MenuItem("GameObject/GTFO/Markers/Randomize Markers", false, 0)]
        public static void RandomizeMarkers(MenuCommand cmd)
        {
            var geomorph = cmd.context as GameObject;
            MarkerUtility.SpawnRandomMarkers(geomorph);
        }


        [MenuItem("GameObject/GTFO/Markers/Cleanup Markers", false, 0)]
        public static void CleanupMarkers(MenuCommand cmd)
        {
            var geomorph = cmd.context as GameObject;
            MarkerUtility.CleanupMarkers(geomorph);
        }


        [MenuItem("GameObject/GTFO/Preview/Create Preview", false, 0)]
        public static void CreatePreview(MenuCommand cmd)
        {
            var geomorph = cmd.context as GameObject;
            MarkerUtility.SpawnRandomMarkers(geomorph);
            PrefabSpawnerUtility.BuildPrefabSpawners(geomorph);
        }
        [MenuItem("GameObject/GTFO/Preview/Clear Preview", false, 0)]
        public static void ClearPreview(MenuCommand cmd)
        {
            var geomorph = cmd.context as GameObject;

            MarkerUtility.CleanupMarkers(geomorph);
            PrefabSpawnerUtility.CleanupPrefabSpawners(geomorph);
        }
    }
}
using GTFO.DevTools;
using GTFO.DevTools.Utilities;
using LevelGeneration;
using UnityEditor;
using UnityEngine;

public class ContextMenuExtensions
{

    [MenuItem("GameObject/GTFO/Prefab Spawners/Build Prefab Spawners", false, 0)]
    public static void TestItem(MenuCommand cmd)
    {
        GameObject geomorph = cmd.context as GameObject;
        PrefabSpawnerUtility.BuildPrefabSpawners(geomorph);
    }
    [MenuItem("GameObject/GTFO/Prefab Spawners/Create Prefab Spawner", false, 0)]
    public static void TestItem56(MenuCommand cmd)
    {
        GameObject geomorph = cmd.context as GameObject;
        PrefabSpawnerUtility.ConvertToPrefabSpawner(geomorph);
    }

    [MenuItem("GameObject/GTFO/Prefab Spawners/Cleanup Prefab Spawners", false, 0)]
    public static void TestItem2(MenuCommand cmd)
    {
        GameObject geomorph = cmd.context as GameObject;
        PrefabSpawnerUtility.CleanupPrefabSpawners(geomorph);
    }

    [MenuItem("GameObject/GTFO/Markers/Randomize Markers", false, 0)]
    public static void TestItem3(MenuCommand cmd)
    {
        var geomorph = cmd.context as GameObject;
        MarkerUtility.SpawnRandomMarkers(geomorph);
    }


    [MenuItem("GameObject/GTFO/Markers/Cleanup Markers", false, 0)]
    public static void TestItem4(MenuCommand cmd)
    {
        var geomorph = cmd.context as GameObject;
        MarkerUtility.CleanupMarkers(geomorph);
    }


    [MenuItem("GameObject/GTFO/Preview/Create Preview", false, 0)]
    public static void TestItem5(MenuCommand cmd)
    {
        var geomorph = cmd.context as GameObject;
        MarkerUtility.SpawnRandomMarkers(geomorph);
        PrefabSpawnerUtility.BuildPrefabSpawners(geomorph);
    }
    [MenuItem("GameObject/GTFO/Preview/Clear Preview", false, 0)]
    public static void TestItem6(MenuCommand cmd)
    {
        var geomorph = cmd.context as GameObject;

        MarkerUtility.CleanupMarkers(geomorph);
        PrefabSpawnerUtility.CleanupPrefabSpawners(geomorph);
    }
}

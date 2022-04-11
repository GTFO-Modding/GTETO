using GTFO.DevTools;
using LevelGeneration;
using UnityEditor;
using UnityEngine;

public class ContextMenuExtensions
{

    [MenuItem("GameObject/GTFO/Build Prefab Spawners", false, 0)]
    public static void TestItem(MenuCommand cmd)
    {
        GameObject geomorph = cmd.context as GameObject;
        BuildPrefabSpawners(geomorph);
    }

    [MenuItem("GameObject/GTFO/Cleanup Prefab Spawners", false, 0)]
    public static void TestItem2(MenuCommand cmd)
    {
        GameObject geomorph = cmd.context as GameObject;
        var spawners = geomorph.GetComponentsInChildren<LG_PrefabSpawner>();
        int spawnerCount = spawners.Length;
        for (int index = 0; index < spawnerCount; index++)
        {
            var prefabSpawner = spawners[index];
            EditorUtility.DisplayProgressBar("Cleanup Prefab Spawners", $"Resetting {prefabSpawner.m_prefab.name}", (index + 1f) / spawnerCount);
            if (prefabSpawner.transform.childCount > 0)
            {
                GameObject.DestroyImmediate(prefabSpawner.transform.GetChild(0).gameObject);
            }
        }

        EditorUtility.ClearProgressBar();
    }

    public static void BuildPrefabSpawners(GameObject obj)
    {
        if (PrefabUtility.IsPartOfAnyPrefab(obj))
        {
            PrefabUtility.UnpackPrefabInstance(obj, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
        }
        foreach (Transform child in obj.GetComponentsInChildren<Transform>())
        {
            if (PrefabUtility.GetPrefabInstanceStatus(child.gameObject) != PrefabInstanceStatus.NotAPrefab)
                PrefabUtility.UnpackPrefabInstance(child.gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
        }

        var spawners = obj.GetComponentsInChildren<LG_PrefabSpawner>();
        int spawnerCount = spawners.Length;
        for (int index = 0; index < spawnerCount; index++)
        {
            var prefabSpawner = spawners[index];
            EditorUtility.DisplayProgressBar("Build Prefab Spawners", $"Building {prefabSpawner.m_prefab.name}", (index + 1f) / spawnerCount);
            var initialScale = prefabSpawner.m_prefab.transform.lossyScale;
            GameObject prefab = GameObject.Instantiate(prefabSpawner.m_prefab, prefabSpawner.transform);
            prefab.transform.localRotation = Quaternion.identity;
            prefab.transform.localPosition = Vector3.zero;
            if (prefabSpawner.m_disableCollision)
            {
                Collider[] colliders = prefab.GetComponentsInChildren<Collider>();
                foreach (Collider collider in colliders)
                {
                    collider.enabled = false;
                }
            }
            if (!prefabSpawner.m_applyScale)
            {
                prefab.transform.localScale = initialScale;
            }
            Debug.Log($"Building {prefabSpawner.gameObject.name}");
        }

        EditorUtility.ClearProgressBar();
    }


    [MenuItem("GameObject/GTFO/Randomize Markers", false, 0)]
    public static void TestItem3(MenuCommand cmd)
    {
        var geomorph = cmd.context as GameObject;
        MarkerInspector.SpawnRandomMarkers(geomorph);
    }


    [MenuItem("GameObject/GTFO/Cleanup Markers", false, 0)]
    public static void TestItem4(MenuCommand cmd)
    {
        var geomorph = cmd.context as GameObject;
        foreach (var producer in geomorph.GetComponentsInChildren<LG_MarkerProducer>())
        {
            if (producer != null)
            {
                MarkerInspector.CleanupMarker(producer);
            }
        }
    }
}

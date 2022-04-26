﻿using LevelGeneration;
using UnityEditor;
using UnityEngine;

namespace GTFO.DevTools.Utilities
{
    public static class PrefabSpawnerUtility
    {
        public static void ConvertToPrefabSpawner(GameObject obj)
        {
            var root = PrefabUtility.GetOutermostPrefabInstanceRoot(obj).transform;
            int indexOverride = -1;
            if (root.parent != null)
            {
                indexOverride = root.GetSiblingIndex();
            }

            var correspondingRoot = PrefabUtility.GetCorrespondingObjectFromSource(root);
            Vector3 position = root.localPosition;
            Vector3 scale = new Vector3(root.localScale.x / correspondingRoot.localScale.x, root.localScale.y / correspondingRoot.localScale.y, root.localScale.z / correspondingRoot.localScale.z);
            Vector3 rotation = root.localEulerAngles - correspondingRoot.localEulerAngles;

            GameObject prefabSpawnerObj = new GameObject("LG_PrefabSpawner_" + obj.name);
            prefabSpawnerObj.transform.SetParent(root.parent);
            prefabSpawnerObj.transform.localPosition = position;
            prefabSpawnerObj.transform.localScale = scale;
            prefabSpawnerObj.transform.localEulerAngles = rotation;

            if (indexOverride > -1)
            {
                prefabSpawnerObj.transform.SetSiblingIndex(indexOverride);
            }

            var prefabSpawner = prefabSpawnerObj.AddComponent<LG_PrefabSpawner>();
            prefabSpawner.m_prefab = correspondingRoot.gameObject;
            prefabSpawner.m_applyScale = (Vector3.one - scale).magnitude < Mathf.Epsilon;

            GameObject.DestroyImmediate(obj);
        }

        public static void CleanupPrefabSpawners(GameObject obj, ProgressBarSettings progressBarSettings = default)
        {
            bool dirtySpawners;
            do
            {
                dirtySpawners = false;
                var spawners = obj.GetComponentsInChildren<LG_PrefabSpawner>();
                int spawnerCount = spawners.Length;
                for (int index = 0; index < spawnerCount; index++)
                {
                    var prefabSpawner = spawners[index];
                    if (prefabSpawner.transform.childCount > 0)
                    {
                        dirtySpawners = true;
                        progressBarSettings.Update("Cleanup Prefab Spawners", $"Resetting {prefabSpawner.m_prefab.name}", index, spawnerCount);
                        GameObject.DestroyImmediate(prefabSpawner.transform.GetChild(0).gameObject);
                    }
                    else
                    {
                        progressBarSettings.Update("Cleanup Prefab Spawners", "", index, spawnerCount);
                    }
                }
            }
            while (dirtySpawners);
            progressBarSettings.Clear();
        }

        public static void BuildPrefabSpawners(GameObject obj, ProgressBarSettings progressBarSettings = default)
        {
            if (PrefabUtility.IsPartOfAnyPrefab(obj))
            {
                PrefabUtility.UnpackPrefabInstance(obj, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
            }
            //foreach (Transform child in obj.GetComponentsInChildren<Transform>())
            //{
            //    if (PrefabUtility.GetPrefabInstanceStatus(child.gameObject) != PrefabInstanceStatus.NotAPrefab)
            //        PrefabUtility.UnpackPrefabInstance(child.gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            //}

            var spawners = obj.GetComponentsInChildren<LG_PrefabSpawner>();
        int spawnerCount = spawners.Length;
            for (int index = 0; index < spawnerCount; index++)
            {
                var prefabSpawner = spawners[index];
                progressBarSettings.Update("Build Prefab Spawners", $"Building {prefabSpawner.m_prefab.name}", index, spawnerCount);
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
            }

            progressBarSettings.Clear();
        }
    }
}

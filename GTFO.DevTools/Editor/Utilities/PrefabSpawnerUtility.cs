using LevelGeneration;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GTFO.DevTools.Utilities
{
    [InitializeOnLoad]
    public static class PrefabSpawnerUtility
    {
        private static FieldInfo m_disableCollisionField = typeof(LG_PrefabSpawner).GetField("m_disableCollisionField", BindingFlags.NonPublic | BindingFlags.Instance);

        static PrefabSpawnerUtility()
        {
            PreviewUtility.DoPreview += BuildPrefabSpawners;
            PreviewUtility.DoClearPreview += CleanupPrefabSpawners;
        }

        public static void ConvertToPrefabSpawner(GameObject obj)
        {
            var root = PrefabUtility.GetOutermostPrefabInstanceRoot(obj).transform;
            int indexOverride = -1;
            if (root.parent != null)
            {
                indexOverride = root.GetSiblingIndex();
            }

            var overrides = PrefabUtility.GetObjectOverrides(root.gameObject);
            overrides.RemoveAll((o) => o.instanceObject is Transform transform && transform == root);
            Transform correspondingRoot = PrefabUtility.GetCorrespondingObjectFromSource(root);
            if (overrides.Count > 0)
            {
                bool createOverride = EditorUtility.DisplayDialog("Prefab Changed", "Changes have been made to the prefab. Do you want to create a prefab override?", "Yes", "No, wipe changes");
                if (createOverride)
                {
                    string path = EditorUtility.SaveFilePanel("Create Prefab Variant", Application.dataPath, root.gameObject.name, "prefab");
                    if (path != null)
                    {
                        correspondingRoot = PrefabUtility.SaveAsPrefabAsset(root.gameObject, path).transform;
                    }
                }
            }

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

        public static void CleanupPrefabSpawners(GameObject obj)
            => CleanupPrefabSpawners(obj, default);

        public static void CleanupPrefabSpawners(GameObject obj, ProgressBarSettings progressBarSettings)
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

        public static void BuildPrefabSpawners(GameObject obj)
            => BuildPrefabSpawners(obj, default);

        public static void BuildPrefabSpawners(GameObject obj, ProgressBarSettings progressBarSettings)
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
                GameObject prefab = PreviewUtility.Instantiate(prefabSpawner.m_prefab, prefabSpawner.transform.position, prefabSpawner.transform.rotation, prefabSpawner.transform.parent);
                if ((bool)m_disableCollisionField.GetValue(prefabSpawner))
                {
                    Collider[] colliders = prefab.GetComponentsInChildren<Collider>();
                    foreach (Collider collider in colliders)
                    {
                        collider.enabled = false;
                    }
                }
                if (prefabSpawner.m_applyScale)
                {
                    prefab.transform.localScale = prefabSpawner.transform.localScale;
                }
                prefab.transform.SetParent(prefabSpawner.transform);
            }

            progressBarSettings.Clear();
        }
    }
}

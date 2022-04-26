using GameData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace GTFO.DevTools.Utilities
{
    public static class MarkerUtility
    {
        public static void SpawnRandomMarkers(GameObject obj)
            => SpawnRandomMarkers(obj, new System.Random());


        public static void CleanupMarkers(GameObject obj)
        {
            foreach (var marker in obj.GetComponentsInChildren<LG_MarkerProducer>())
            {
                if (marker != null)
                    CleanupMarker(marker);
            }
        }
        public static void CleanupMarker(LG_MarkerProducer producer)
        {
            if (producer != null && producer.transform.childCount > 0)
            {
                GameObject.DestroyImmediate(producer.transform.GetChild(0).gameObject);
            }
        }

        public static void SpawnRandomMarkers(LG_MarkerProducer marker)
        {
            var datablock = GetDataBlockForProducer(marker);
            if (datablock != null)
            {
                SpawnRandomMarkerComp(marker, datablock, new System.Random());
            }
        }

        public static void SpawnMarkerComposition(LG_MarkerProducer marker, MarkerComposition composition)
        {
            CleanupMarker(marker);
            var actualPath = Path.Combine("Assets/PrefabInstance", Path.GetFileName(composition.prefab));

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(actualPath);
            if (prefab != null)
            {
                var copy = GameObject.Instantiate(prefab, marker.transform);
                copy.transform.localPosition = Vector3.zero;
                copy.transform.localRotation = Quaternion.identity;
                PrefabSpawnerUtility.BuildPrefabSpawners(copy);

                for (int childIndex = 0, childCount = copy.transform.childCount; childIndex < childCount; childIndex++)
                {
                    SpawnRandomMarkers(copy.transform.GetChild(childIndex).gameObject);
                }
            }
        }

        private static void SpawnRandomMarkerComp(LG_MarkerProducer producer, IMarkerDataBlock block, System.Random random, bool firstPass = true)
        {
            if (producer == null) return;

            CleanupMarker(producer);
            var prefabs = block.CommonData.Compositions
                .Where((c) => c.function != ExpeditionFunction.Strongbox && c.function != ExpeditionFunction.ResourceContainerSecure)
                .Select((c) => string.IsNullOrEmpty(c.prefab) ? null : Path.Combine("Assets/PrefabInstance", Path.GetFileName(c.prefab)))
                .Select((p) => p == null ? null : AssetDatabase.LoadAssetAtPath<GameObject>(p))
                .Where((asset) => asset != null)
                .ToArray();

            if (prefabs.Length == 0)
                return;


            var prefab = prefabs[random.Next(prefabs.Length)];
            if (prefab == null)
                return;

            var copy = GameObject.Instantiate(prefab, producer.transform);
            copy.transform.localPosition = Vector3.zero;
            copy.transform.localRotation = Quaternion.identity;
            PrefabSpawnerUtility.BuildPrefabSpawners(copy, new ProgressBarSettings(false, "Build Marker", true));

            for (int childIndex = 0, childCount = copy.transform.childCount; childIndex < childCount; childIndex++)
            {
                SpawnRandomMarkers(copy.transform.GetChild(childIndex).gameObject, random, false);
            }

            if (firstPass)
                EditorUtility.ClearProgressBar();

        }

        public static void SpawnRandomMarkers(GameObject obj, System.Random random)
            => SpawnRandomMarkers(obj, random, true);

        private static void SpawnRandomMarkers(GameObject obj, System.Random random, bool firstPass)
        {
            foreach (var marker in obj.GetComponentsInChildren<LG_MarkerProducer>())
            {
                var block = GetDataBlockForProducer(marker);
                if (block != null)
                    SpawnRandomMarkerComp(marker, GetDataBlockForProducer(marker), random, firstPass);
            }
        }

        public static IMarkerDataBlock GetDataBlockForProducer(LG_MarkerProducer producer)
        {
            switch (producer.MarkerDataBlockType)
            {
                case LG_MarkerDataBlockType.Service:
                    return GTFOGameConfig.Rundown.DataBlocks.ServiceMarker.GetBlockByID(producer.MarkerDataBlockID);
                case LG_MarkerDataBlockType.Mining:
                    return GTFOGameConfig.Rundown.DataBlocks.MiningMarker.GetBlockByID(producer.MarkerDataBlockID);
                case LG_MarkerDataBlockType.Tech:
                    return GTFOGameConfig.Rundown.DataBlocks.TechMarker.GetBlockByID(producer.MarkerDataBlockID);
            }
            return null;
        }
    }
}

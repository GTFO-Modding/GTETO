using Expedition;
using GTFO.DevTools.Persistent;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GTFO.DevTools.Utilities
{
    [InitializeOnLoad]
    public static class LadderUtility
    {
        static LadderUtility()
        {
            PreviewUtility.DoPreview += Preview;
            PreviewUtility.DoClearPreview += ClearPreview;
        }
        

        // dont ask why its 0.6f and 0.16f. This is what the mono ref stats and it isn't assigned.
        public static readonly float m_offsetAboveTopFloor = 0.6f;
        public static readonly float m_topPieceThickness = 0.16f;

        public static void Preview(GameObject obj, SubComplex complex)
        {
            foreach (var ladder in obj.GetComponentsInChildren<LG_Ladder>())
            {
                BuildLadder(ladder, complex);
            }
        }
        public static void Preview(GameObject obj)
        {
            foreach (var ladder in obj.GetComponentsInChildren<LG_Ladder>())
            {
                Preview(ladder);
            }
        }
        public static void Preview(LG_Ladder ladder)
            => BuildLadder(ladder, SubComplex.DigSite);

        public static void ClearPreview(GameObject obj)
        {
            foreach (var ladder in obj.GetComponentsInChildren<LG_Ladder>())
            {
                ClearPreview(ladder);
            }
        }

        public static void ClearPreview(LG_Ladder ladder)
        {
            var ladderPreviewTrans = ladder.transform.Find("Preview");
            if (ladderPreviewTrans)
            {
                GameObject.DestroyImmediate(ladderPreviewTrans.gameObject);
            }
        }

        public static float CalculateLadderHeight(LG_Ladder ladder)
        {
            if (ladder == null) return 0f;
            Vector3 baseCenterPos = ladder.transform.position;
            Vector3 ladderTop = baseCenterPos + (ladder.m_topFloor.transform.localPosition.y + m_offsetAboveTopFloor) * ladder.transform.up;
            Vector3 ladderVec = ladderTop - baseCenterPos;
            return ladderVec.magnitude;
        }

        public static void BuildLadder(LG_Ladder ladder, SubComplex subcomplex)
        {
            float height = CalculateLadderHeight(ladder);
            if (!ladder.m_enemyClimbingOnly)
            {
                SpawnLadderGraphics(ladder, subcomplex, height);
            }
        }

        private static void SpawnLadderGraphics(LG_Ladder ladder, SubComplex subcomplex, float height)
        {
            float curHeight = height;
            SpawnLadderPiece(GetLadderPiece(subcomplex, LadderType.Bottom), curHeight, height, ladder);
            curHeight -= 0.4f;
            while (curHeight > 1f)
            {
                if (curHeight > 4f)
                {
                    SpawnLadderPiece(GetLadderPiece(subcomplex, LadderType.Length_4m), curHeight, height, ladder);
                    curHeight -= 4f;
                }
                else if (curHeight > 2f)
                {
                    SpawnLadderPiece(GetLadderPiece(subcomplex, LadderType.Length_2m), curHeight, height, ladder);
                    curHeight -= 2f;
                }
                else if (curHeight > 1f)
                {
                    SpawnLadderPiece(GetLadderPiece(subcomplex, LadderType.Length_1m), curHeight, height, ladder);
                    curHeight -= 1f;
                }
                else
                {
                    SpawnLadderPiece(GetLadderPiece(subcomplex, LadderType.Length_05m), curHeight, height, ladder);
                    curHeight -= 0.5f;
                }
            }
            if (height > 3f)
            {
                GameObject gameObject = SpawnLadderPiece(GetLadderPiece(subcomplex, LadderType.Top), curHeight, height, ladder);
                Vector3 position = gameObject.transform.position;
                gameObject.transform.position = new Vector3(position.x, ladder.m_topFloor.position.y + m_topPieceThickness, position.z);
            }
            curHeight -= 0.4f;
        }

        private enum LadderType
        {
            Top,
            Bottom,
            Length_4m,
            Length_2m,
            Length_1m,
            Length_05m
        }

        private static GameObject SpawnLadderPiece(GameObject obj, float height, float ladderHeight, LG_Ladder ladder)
        {
            var previewTrans = ladder.transform.Find("Preview");
            if (!previewTrans)
            {
                var previewObj = new GameObject("Preview");
                previewObj.transform.SetParent(ladder.transform);
                previewObj.transform.localScale = Vector3.one;
                previewObj.transform.localPosition = Vector3.zero;
                previewObj.transform.localRotation = Quaternion.identity;
                previewTrans = previewObj.transform;
            }
            GameObject piece = PreviewUtility.Instantiate(obj, ladder.transform.position + Vector3.up * (ladderHeight - height), ladder.transform.rotation, previewTrans);
            return piece;
        }

        private static GameObject GetLadderPiece(SubComplex subcomplex, LadderType type)
        {
            var random = new System.Random();
            var complexResourceSet = CachedComplexResourceSet.Instance;
            string[] prefabs = Array.Empty<string>();

            switch (type)
            {
                case LadderType.Top:
                    prefabs = complexResourceSet.Ladders_Top.GetPrefabs(subcomplex);
                    break;
                case LadderType.Bottom:
                    prefabs = complexResourceSet.Ladders_Bottom.GetPrefabs(subcomplex);
                    break;
                case LadderType.Length_4m:
                    prefabs = complexResourceSet.Ladders_4m.GetPrefabs(subcomplex);
                    break;
                case LadderType.Length_2m:
                    prefabs = complexResourceSet.Ladders_2m.GetPrefabs(subcomplex);
                    break;
                case LadderType.Length_1m:
                    prefabs = complexResourceSet.Ladders_1m.GetPrefabs(subcomplex);
                    break;
                case LadderType.Length_05m:
                    prefabs = complexResourceSet.Ladders_05m.GetPrefabs(subcomplex);
                    break;
            }

            if (prefabs.Length == 0)
                return null;
            string prefabPath = prefabs.Length == 1 ? prefabs[0] : prefabs[random.Next(prefabs.Length)];
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PrefabInstance/" + Path.GetFileName(prefabPath));
            return prefab;
        }
    }
}

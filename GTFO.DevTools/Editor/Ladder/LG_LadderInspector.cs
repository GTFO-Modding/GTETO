using Expedition;
using GTFO.DevTools.Persistent;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GTFO.DevTools.Ladder
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LG_Ladder))]
    public class LG_LadderInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Preview", EditorStyles.whiteLabel);
            if (GUILayout.Button("Clear", GUILayout.ExpandWidth(false)))
            {
                this.ClearPreviews();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel++;
            this.ShowLabelWithPreviewButton("Dig Site", SubComplex.DigSite);
            this.ShowLabelWithPreviewButton("Refinery", SubComplex.Refinery);
            this.ShowLabelWithPreviewButton("Storage", SubComplex.Storage);
            this.ShowLabelWithPreviewButton("Lab", SubComplex.Lab);
            this.ShowLabelWithPreviewButton("Data Center", SubComplex.DataCenter);
            this.ShowLabelWithPreviewButton("Floodways", SubComplex.Floodways);
            EditorGUI.indentLevel--;

        }

        private void ClearPreviews()
        {
            foreach (var target in this.targets)
            {
                var ladder = target as LG_Ladder;
                if (ladder == null)
                    continue;
                this.ClearPreview(ladder);
            }
        }

        private void ClearPreview(LG_Ladder ladder)
        {
            var ladderPreviewTrans = ladder.transform.Find("Preview");
            if (ladderPreviewTrans)
            {
                DestroyImmediate(ladderPreviewTrans.gameObject);
            }
        }

        private void ShowLabelWithPreviewButton(string label, SubComplex complex)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label);
            if (GUILayout.Button("Build", GUILayout.ExpandWidth(false)))
            {
                this.BuildLadders(complex);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void BuildLadders(SubComplex subcomplex)
        {
            foreach (var target in this.targets)
            {
                var ladder = target as LG_Ladder;
                if (ladder == null)
                    continue;

                this.ClearPreview(ladder);
                this.BuildLadder(ladder, subcomplex);
            }
        }

        // dont ask why its 0.6f and 0.16f. This is what the mono ref stats and it isn't assigned.
        private float m_offsetAboveTopFloor = 0.6f;
        private float m_topPieceThickness = 0.16f;

        private void BuildLadder(LG_Ladder ladder, SubComplex subcomplex)
        {
            var baseCenterPos = ladder.transform.position;
            var ladderTop = baseCenterPos + (ladder.m_topFloor.transform.localPosition.y + this.m_offsetAboveTopFloor) * ladder.transform.up;
            var ladderVec = ladderTop - baseCenterPos;
            var height = ladderVec.magnitude;
            if (!ladder.m_enemyClimbingOnly)
            {
                this.SpawnLadderGraphics(ladder, subcomplex, height);
            }
        }

        private void SpawnLadderGraphics(LG_Ladder ladder, SubComplex subcomplex, float height)
        {
            float curHeight = height;
            this.SpawnLadderPiece(this.GetLadderPiece(subcomplex, LadderType.Bottom), curHeight, height, ladder);
            curHeight -= 0.4f;
            while (curHeight > 1f)
            {
                if (curHeight > 4f)
                {
                    this.SpawnLadderPiece(this.GetLadderPiece(subcomplex, LadderType.Length_4m), curHeight, height, ladder);
                    curHeight -= 4f;
                }
                else if (curHeight > 2f)
                {
                    this.SpawnLadderPiece(this.GetLadderPiece(subcomplex, LadderType.Length_2m), curHeight, height, ladder);
                    curHeight -= 2f;
                }
                else if (curHeight > 1f)
                {
                    this.SpawnLadderPiece(this.GetLadderPiece(subcomplex, LadderType.Length_1m), curHeight, height, ladder);
                    curHeight -= 1f;
                }
                else
                {
                    this.SpawnLadderPiece(this.GetLadderPiece(subcomplex, LadderType.Length_05m), curHeight, height, ladder);
                    curHeight -= 0.5f;
                }
            }
            if (height > 3f)
            {
                GameObject gameObject = this.SpawnLadderPiece(this.GetLadderPiece(subcomplex, LadderType.Top), curHeight, height, ladder);
                Vector3 position = gameObject.transform.position;
                gameObject.transform.position = new Vector3(position.x, ladder.m_topFloor.position.y + this.m_topPieceThickness, position.z);
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

        private GameObject SpawnLadderPiece(GameObject obj, float height, float ladderHeight, LG_Ladder ladder)
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
            GameObject piece = Instantiate(obj, ladder.transform.position + Vector3.up * (ladderHeight - height), ladder.transform.rotation, previewTrans);
            return piece;
        }

        private GameObject GetLadderPiece(SubComplex subcomplex, LadderType type)
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

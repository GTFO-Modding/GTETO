using Expedition;
using GTFO.DevTools.Extensions;
using GTFO.DevTools.Persistent;
using GTFO.DevTools.Utilities;
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
        // random constant
        private float m_colliderHeightAdd = 0.7f;

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var target in this.targets)
                {
                    this.UpdateLadderCollider(target as LG_Ladder);

                }
                this.serializedObject.ApplyModifiedProperties();
            }

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

        private void UpdateLadderCollider(LG_Ladder ladder)
        {
            if (ladder == null) return;


            BoxCollider collider = ladder.GetCollider();
            if (collider == null)
            {
                ladder.SetCollider(collider = ladder.GetComponent<BoxCollider>());
            }

            float height = LadderUtility.CalculateLadderHeight(ladder);

            collider.center = this.CalculateBoxCenter(height);
            collider.size = this.CalculateBoxSize(ladder, height);
            collider.enabled = !ladder.m_enemyClimbingOnly;
        }

        private Vector3 CalculateBoxCenter(float height)
        {
            return new Vector3(0f, (height + this.m_colliderHeightAdd) * 0.5f, 0f);
        }

        private Vector3 CalculateBoxSize(LG_Ladder ladder, float height)
        {
            return new Vector3(ladder.m_width + 1.2f, height + this.m_colliderHeightAdd, ladder.m_thickness);
        }

        private void ClearPreviews()
        {
            foreach (var target in this.targets)
            {
                var ladder = target as LG_Ladder;
                if (ladder == null)
                    continue;
                LadderUtility.ClearPreview(ladder);
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

                LadderUtility.ClearPreview(ladder);
                LadderUtility.BuildLadder(ladder, subcomplex);
            }
        }
    }
}

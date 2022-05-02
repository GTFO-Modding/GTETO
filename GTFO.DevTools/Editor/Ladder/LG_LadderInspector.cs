using Expedition;
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

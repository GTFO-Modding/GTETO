using GameData;
using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace GTFO.DevTools.Windows
{
    public class SelectDataBlockWindow : EditorWindow
    {
        private uint m_selectedID;
        private string m_selectedName;
        private SerializedObject m_obj;
        private SerializedProperty m_property;
        private Vector2 m_scrollPosition;
        private Type m_type;
        private bool m_saveID;
        private string m_search = "";

        private void OnGUI()
        {
            this.titleContent = new GUIContent("Choose " + this.m_type.Name);

            var blockInfo = GTFOGameConfig.Rundown.DataBlocks.GetBlock(this.m_type);
            if (blockInfo == null)
                return;
            var blocks = blockInfo.GetBasicBlocks();

            var oldColor = GUI.contentColor;

            this.m_search = EditorGUILayout.TextField(this.m_type.Name, this.m_search);

            this.m_scrollPosition = EditorGUILayout.BeginScrollView(this.m_scrollPosition);

            foreach (var block in blocks)
            {
                string display = "[" + block.persistentID + "] " + block.name.ToLower();
                if (!display.Contains(this.m_search.ToLower()))
                    continue;

                bool selected = block.persistentID == this.m_selectedID ||
                    name == this.m_selectedName;
                Color baseColor = block.internalEnabled ? Color.white : new Color(0.5f, 0.5f, 0.5f);
                Color contentColor = selected ? new Color(0.5f, 0.5f, 1f) : oldColor;
                GUI.contentColor = baseColor * contentColor;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent(display, $"{block.blockTypeName}\nName: {block.name}\nPersistent ID: {block.persistentID}\nInternally Enabled: {block.internalEnabled}"));

                if (!selected && GUILayout.Button("Select", GUILayout.ExpandWidth(false)))
                {
                    this.m_selectedID = block.persistentID;
                    this.m_selectedName = block.name;

                    if (this.m_saveID)
                    {
                        this.m_property.intValue = (int)this.m_selectedID;
                    }
                    else
                    {
                        this.m_property.stringValue = this.m_selectedName;
                    }
                    this.m_obj.ApplyModifiedProperties();
                    this.Close();
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        public static void ShowWindow(uint value, SerializedProperty property, SerializedObject obj, Type type)
        {
            var window = GetWindow<SelectDataBlockWindow>();
            window.m_selectedID = value;
            window.m_property = property;
            window.m_type = type;
            window.m_obj = obj;
            window.m_saveID = true;
            window.Show();
        }
    }
}

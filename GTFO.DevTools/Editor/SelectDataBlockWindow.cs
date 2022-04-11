using GameData;
using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace GTFO.DevTools
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
            var blocks = (Array)blockInfo.GetType().GetMethod("GetBlocks").Invoke(blockInfo, new object[0]);

            var oldColor = GUI.contentColor;

            this.m_search = EditorGUILayout.TextField(this.m_type.Name, this.m_search);

            this.m_scrollPosition = EditorGUILayout.BeginScrollView(this.m_scrollPosition);

            foreach (var block in blocks)
            {
                uint persistentID = (uint)block.GetType().GetProperty("persistentID").GetValue(block);
                string name = (string)block.GetType().GetProperty("name").GetValue(block);
                string display = "[" + persistentID + "] " + name;
                if (!display.Contains(this.m_search))
                    continue;

                bool selected = persistentID == this.m_selectedID ||
                    name == this.m_selectedName;
                if (selected)
                {
                    GUI.contentColor = new Color(0.5f, 0.5f, 1f);
                }
                else
                {
                    GUI.contentColor = oldColor;
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(display);

                if (!selected && GUILayout.Button("Select", GUILayout.ExpandWidth(false)))
                {
                    this.m_selectedID = persistentID;
                    this.m_selectedName = name;

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
        }
    }
}

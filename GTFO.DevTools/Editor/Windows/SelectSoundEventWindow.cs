using GTFO.DevTools.Utilities;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GTFO.DevTools.Windows
{
    public class SelectSoundEventWindow : EditorWindow
    {
        private uint m_selectedID;
        private string m_selectedName;
        private SerializedObject m_obj;
        private SerializedProperty m_property;
        private Vector2 m_scrollPosition;
        private bool m_saveID;
        private string m_filter;
        private string m_search = "";

        private void OnGUI()
        {
            this.titleContent = new GUIContent("Choose Sound Event");

            var events = WWiseUtilities.GetEvents()
                .Where((ev) => string.IsNullOrEmpty(this.m_filter) || ev.EventName.Contains(this.m_filter));

            var oldColor = GUI.contentColor;

            this.m_search = EditorGUILayout.TextField("Search", this.m_search);

            this.m_scrollPosition = EditorGUILayout.BeginScrollView(this.m_scrollPosition); 
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("None");

            bool selected = this.m_selectedID == 0U ||
                string.IsNullOrEmpty(this.m_selectedName);

            if (!selected && GUILayout.Button("Select", GUILayout.ExpandWidth(false)))
            {
                this.m_selectedID = 0;
                this.m_selectedName = "";

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

            foreach (var ev in events)
            {
                string display = "[" + ev.EventID + "] " + ev.EventName.ToLower();
                if (!display.Contains(this.m_search.ToLower()))
                    continue;

                selected = ev.EventID == this.m_selectedID ||
                    ev.EventName == this.m_selectedName;
                Color baseColor = Color.white;
                Color contentColor = selected ? new Color(0.5f, 0.5f, 1f) : oldColor;
                GUI.contentColor = baseColor * contentColor;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent(display, $"Event Name: {ev.EventName}\nEvent ID: {ev.EventID}"));

                if (!selected && GUILayout.Button("Select", GUILayout.ExpandWidth(false)))
                {
                    this.m_selectedID = ev.EventID;
                    this.m_selectedName = ev.EventName;

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

        public static void ShowWindow(uint value, SerializedProperty property, SerializedObject obj, string filter)
        {
            var window = GetWindow<SelectSoundEventWindow>();
            window.m_selectedID = value;
            window.m_property = property;
            window.m_obj = obj;
            window.m_saveID = true;
            window.m_filter = filter ?? "";
            window.Show();
        }

        public static void ShowWindow(string value, SerializedProperty property, SerializedObject obj, string filter)
        {
            var window = GetWindow<SelectSoundEventWindow>();
            window.m_selectedID = WWiseUtilities.GetEvents()
                .Where(ev => ev.EventName == value)
                .Select(ev => ev.EventID)
                .FirstOrDefault();
            window.m_selectedName = value;
            window.m_property = property;
            window.m_obj = obj;
            window.m_saveID = true;
            window.m_filter = filter ?? "";
            window.Show();
        }
    }
}

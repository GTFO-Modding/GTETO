using GTFO.DevTools.Windows;
using UnityEditor;
using UnityEngine;

namespace GTFO.DevTools.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(SoundEventAttribute))]
    public class SoundEventPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Integer)
            {
                EditorGUI.BeginProperty(position, label, property);
                Rect indentedPosition = EditorGUI.IndentedRect(position);
                Rect itemRect = indentedPosition;
                itemRect.width -= 50f;
                Rect contentRect = indentedPosition;
                contentRect.width = 50f;
                contentRect.center += new Vector2(itemRect.width, 0f);

                EditorGUI.PropertyField(itemRect, property);
                if (GUI.Button(contentRect, "Select"))
                {
                    var prop = property.Copy();
                    var filter = ((SoundEventAttribute)this.attribute).Filter;

                    SelectSoundEventWindow.ShowWindow((uint)prop.intValue, prop, property.serializedObject, filter);
                }
                EditorGUI.EndProperty();
            }
            else if (property.propertyType == SerializedPropertyType.String)
            {
                EditorGUI.BeginProperty(position, label, property);
                Rect indentedPosition = EditorGUI.IndentedRect(position);
                Rect itemRect = indentedPosition;
                itemRect.width -= 50f;
                Rect contentRect = indentedPosition;
                contentRect.width = 50f;
                contentRect.center += new Vector2(itemRect.width, 0f);

                EditorGUI.PropertyField(itemRect, property);
                if (GUI.Button(contentRect, "Select"))
                {
                    var prop = property.Copy();
                    var filter = ((SoundEventAttribute)this.attribute).Filter;

                    SelectSoundEventWindow.ShowWindow(prop.stringValue, prop, property.serializedObject, filter);
                }
                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "SoundEventPropertyDrawer may only be used on int/uint fields");
            }
        }
    }
}

using UnityEditor;
using UnityEngine;

namespace GTFO.DevTools.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(GameDataBlockDropdownAttribute))]
    public class GameDataBlockDropdownPropertyDrawer : PropertyDrawer
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
                    // todo: select data block
                    var datablockType = ((GameDataBlockDropdownAttribute)this.attribute).BlockType;

                    SelectDataBlockWindow.ShowWindow((uint)prop.intValue, prop, property.serializedObject, datablockType);
                }
                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use TextAreaDrawer with string.");
            }
        }
    }
}

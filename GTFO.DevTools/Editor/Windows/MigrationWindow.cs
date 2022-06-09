using GTFO.DevTools.Components.Migration;
using UnityEditor;
using UnityEngine;

namespace GTFO.DevTools.Windows
{
    public class MigrationWindow : EditorWindow
    {
        private MigrationToolComponent m_component;

        private void OnGUI()
        {
            this.titleContent = Styles.TITLE;
            if (this.m_component == null)
            {
                this.m_component = new MigrationToolComponent(this);
            }
            this.m_component.Draw();
        }

        [MenuItem("Window/GTFO.DevTools/Migration Tool")]
        public static void OpenWindow()
        {
            GetWindow<MigrationWindow>().Show();
        }

        #region Styles
        private static class Styles
        {
            public static GUIContent TITLE;
            public static GUIContent ERROR_ICON;

            static Styles()
            {
                TITLE = new GUIContent("Migration Window");
                ERROR_ICON = EditorGUIUtility.IconContent("console.erroricon");
            }
        }
        #endregion
    }
}

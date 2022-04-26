using GTFO.DevTools.Persistent;
using UnityEditor;
using UnityEngine;

namespace GTFO.DevTools
{
    public class RundownToolsWindow : EditorWindow
    {
        private bool m_autoLoad;

        private void OnGUI()
        {
            this.titleContent = Styles.TITLE;

            if ((this.m_autoLoad || !string.IsNullOrEmpty(DevToolSettings.Instance.m_rundownPath)) && !GTFOGameConfig.Rundown.Valid)
            {
                GTFOGameConfig.Rundown.Folder = DevToolSettings.Instance.m_rundownPath;
                GTFOGameConfig.Rundown.Validate();
                if (!GTFOGameConfig.Rundown.Valid)
                {
                    this.m_autoLoad = false;

                }
                else
                {
                    GTFOGameConfig.Rundown.LoadBlocks();
                }
            }

            if (GTFOGameConfig.Rundown.ValidAndLoaded)
            {
                var rundown = GTFOGameConfig.Rundown.DataBlocks.GetRundown();
                EditorGUILayout.LabelField(rundown.StorytellingData.Title.TranslateText(GTFOGameConfig.Rundown.DataBlocks), EditorStyles.whiteLargeLabel);
            }

            if (!GTFOGameConfig.Rundown.Valid)
            {
                EditorGUILayout.HelpBox(Styles.ERROR_NO_RUNDOWN);
                if (GUILayout.Button(Styles.OPEN_RUNDOWN_BUTTON_LABEL))
                {
                    string path = EditorUtility.OpenFolderPanel("Select Rundown", DevToolSettings.Instance.m_rundownPath ?? Application.dataPath, "");
                    DevToolSettings.Instance.m_rundownPath = path;
                    this.m_autoLoad = true;

                    GTFOGameConfig.Rundown.Folder = path;
                    GTFOGameConfig.Rundown.Validate();
                }
                return;
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(Styles.CHANGE_RUNDOWN_BUTTON_LABEL))
            {
                string path = EditorUtility.OpenFolderPanel("Select Rundown", DevToolSettings.Instance.m_rundownPath ?? Application.dataPath, "");
                DevToolSettings.Instance.m_rundownPath = path;
                this.m_autoLoad = true;

                GTFOGameConfig.Rundown.Folder = path;
                GTFOGameConfig.Rundown.Validate();
                if (!GTFOGameConfig.Rundown.Valid)
                    return;
            }

            if (GUILayout.Button(Styles.LOAD_RUNDOWN_BUTTON_LABEL))
            {
                GTFOGameConfig.Rundown.LoadBlocks();
            }

            if (GUILayout.Button(Styles.SAVE_RUNDOWN_BUTTON_LABEL))
            {
                GTFOGameConfig.Rundown.SaveBlocks();
            }
            EditorGUILayout.EndHorizontal();

            if (!GTFOGameConfig.Rundown.DataBlocks.GameSetup.Loaded)
            {
                EditorGUILayout.HelpBox(Styles.ERROR_NOT_LOADED);
                return;
            }
            return;
        }

        [MenuItem("Window/Rundown Tools")]
        private static void CreateWindowMenuItem()
            => CreateWindow();

        public static RundownToolsWindow CreateWindow()
        {
            var window = GetWindow<RundownToolsWindow>();
            window.Show();
            return window;
        }

        #region Styles
        private static class Styles
        {
            public static GUIContent OPEN_RUNDOWN_BUTTON_LABEL;
            public static GUIContent CHANGE_RUNDOWN_BUTTON_LABEL;
            public static GUIContent LOAD_RUNDOWN_BUTTON_LABEL;
            public static GUIContent SAVE_RUNDOWN_BUTTON_LABEL;
            public static GUIContent TITLE;
            public static GUIContent ERROR_ICON;
            public static GUIContent ERROR_NO_RUNDOWN;
            public static GUIContent ERROR_NOT_LOADED;

            static Styles()
            {
                TITLE = new GUIContent("Rundown Tools");
                OPEN_RUNDOWN_BUTTON_LABEL = new GUIContent("Open Rundown");
                CHANGE_RUNDOWN_BUTTON_LABEL = new GUIContent("Change Rundown");
                LOAD_RUNDOWN_BUTTON_LABEL = new GUIContent("Load Rundown");
                SAVE_RUNDOWN_BUTTON_LABEL = new GUIContent("Save Rundown");
                ERROR_ICON = EditorGUIUtility.IconContent("console.erroricon");
                ERROR_NO_RUNDOWN = new GUIContent("No Rundown Loaded", ERROR_ICON.image);
                ERROR_NOT_LOADED = new GUIContent("Not Loaded", ERROR_ICON.image);
            }
        }
        #endregion
    }
}

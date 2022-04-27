using GTFO.DevTools.Persistent;
using UnityEditor;
using UnityEngine;

namespace GTFO.DevTools.Windows
{
    public class RundownToolsWindow : EditorWindow
    {
        private void OnAssemblyReloaded()
        {
            string currentFolder = GTFOGameConfig.Rundown.Folder;
            string settingsFolder = DevToolSettings.Instance.m_rundownPath;

            if (currentFolder != settingsFolder)
            {
                GTFOGameConfig.Rundown.Folder = settingsFolder;
            }
            GTFOGameConfig.Rundown.Validate();
            if (GTFOGameConfig.Rundown.Valid)
            {
                GTFOGameConfig.Rundown.LoadBlocks();
            }
        }

        private void OnEnable()
        {
            AssemblyReloadEvents.afterAssemblyReload += this.OnAssemblyReloaded;
        }

        private void OnDisable()
        {
            AssemblyReloadEvents.afterAssemblyReload -= this.OnAssemblyReloaded;
        }


        private void OnGUI()
        {
            this.titleContent = Styles.TITLE;

            if (GTFOGameConfig.Rundown.ValidAndLoaded)
            {
                var rundown = GTFOGameConfig.Rundown.DataBlocks.GetRundown();
                EditorGUILayout.LabelField(rundown.StorytellingData.Title.TranslateText(GTFOGameConfig.Rundown.DataBlocks), EditorStyles.whiteLargeLabel);
            }

            if (!GTFOGameConfig.Rundown.Valid)
            {
                EditorGUILayout.HelpBox(Styles.ERROR_NO_RUNDOWN);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(Styles.OPEN_RUNDOWN_BUTTON_LABEL) && this.ChangeRundown())
                {
                    GTFOGameConfig.Rundown.LoadBlocks();
                }
                if (GUILayout.Button(Styles.REFRESH_RUNDOWN_BUTTON_LABEL) && this.RefreshRundown())
                {
                    GTFOGameConfig.Rundown.LoadBlocks();
                }
                EditorGUILayout.EndHorizontal();
                return;
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(Styles.CHANGE_RUNDOWN_BUTTON_LABEL))
            {
                if (this.ChangeRundown())
                {
                    GTFOGameConfig.Rundown.LoadBlocks();
                }
                else
                {
                    return;
                }
            }

            GUI.enabled = false;
            if (GUILayout.Button(Styles.SAVE_RUNDOWN_BUTTON_LABEL))
            {
                GTFOGameConfig.Rundown.SaveBlocks();
            }
            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();

            if (!GTFOGameConfig.Rundown.DataBlocks.GameSetup.Loaded)
            {
                EditorGUILayout.HelpBox(Styles.ERROR_NOT_LOADED);
                return;
            }
            return;
        }

        private bool ChangeRundown()
        {
            string path = EditorUtility.OpenFolderPanel("Select Rundown", DevToolSettings.Instance.m_rundownPath ?? Application.dataPath, "");
            if (path == null)
                return false;

            DevToolSettings.Instance.m_rundownPath = path;
            return this.RefreshRundown();
        }

        private bool RefreshRundown()
        {
            GTFOGameConfig.Rundown.Folder = DevToolSettings.Instance.m_rundownPath;
            GTFOGameConfig.Rundown.Validate();

            return GTFOGameConfig.Rundown.Valid;
        }

        [MenuItem("Window/GTFO/Rundown Tools")]
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
            public static GUIContent REFRESH_RUNDOWN_BUTTON_LABEL;
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
                REFRESH_RUNDOWN_BUTTON_LABEL = new GUIContent("Refresh Rundown");
                CHANGE_RUNDOWN_BUTTON_LABEL = new GUIContent("Change Rundown");
                LOAD_RUNDOWN_BUTTON_LABEL = new GUIContent("Load Rundown");
                SAVE_RUNDOWN_BUTTON_LABEL = new GUIContent("Save Rundown", "Saving is not supported until Rundown 6.5 Scripts are exported.");
                ERROR_ICON = EditorGUIUtility.IconContent("console.erroricon");
                ERROR_NO_RUNDOWN = new GUIContent("No Rundown Loaded", ERROR_ICON.image);
                ERROR_NOT_LOADED = new GUIContent("Not Loaded", ERROR_ICON.image);
            }
        }
        #endregion
    }
}

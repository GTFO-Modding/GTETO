using UnityEditor;

namespace GTFO.DevTools.Utilities
{
    public struct ProgressBarSettings
    {
        public bool DontClearProgressBar { get; }
        public string TitlePrefix { get; }
        public bool DontShow { get; }

        public ProgressBarSettings(bool dontShow, string titlePrefix, bool dontClear)
        {
            this.DontShow = dontShow;
            this.TitlePrefix = titlePrefix;
            this.DontClearProgressBar = dontClear;
        }

        public void Update(string title, string content, int currentIndex, int amount)
        {
            this.Update(title, content, (currentIndex + 1f) / amount);
        }

        public void Update(string title, string content, float progress)
        {
            if (this.DontShow)
                return;

            if (!string.IsNullOrEmpty(this.TitlePrefix))
                title = this.TitlePrefix + " " + title;

            EditorUtility.DisplayProgressBar(title, content, progress);
        }

        public void Clear()
        {
            if (this.DontClearProgressBar || this.DontShow)
                return;

            EditorUtility.ClearProgressBar();
        }

        public static ProgressBarSettings DONT_SHOW => new ProgressBarSettings(true, default, default);
    }
}

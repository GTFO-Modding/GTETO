using System;
using UnityEditor;
using UnityEngine;

namespace GTFO.DevTools.Geo
{
    public abstract class GeoEditorComponent
    {
        private GeomorphToolWindow m_window;
        private bool m_isSetup;
        private Vector2 m_scrollPosition;

        public GeomorphToolWindow Window => this.m_window;

        public void Setup(GeomorphToolWindow window)
        {
            if (this.m_isSetup) return;

            this.m_isSetup = true;
            this.m_window = window;
            this.Setup();
        }

        public virtual bool NeedStyleRefresh => false;
        public virtual void RefreshStyle()
        { }

        public virtual void OnShow()
        { }

        public virtual void OnHide()
        { }

        public virtual void Reset()
        { }

        protected virtual void OnHeaderGUI()
        { }

        public void OnGUI()
        {
            if (this.NeedStyleRefresh)
                this.RefreshStyle();

            this.OnHeaderGUI();
            this.m_scrollPosition = EditorGUILayout.BeginScrollView(this.m_scrollPosition);
            try
            {
                this.OnInspectorGUI();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            EditorGUILayout.EndScrollView();
        }

        protected abstract void OnInspectorGUI();

        protected virtual void Setup()
        { }
    }
}

using GTFO.DevTools.Geo;
using System;
using UnityEditor;
using UnityEngine;

namespace GTFO.DevTools.Components.Geo
{
    public abstract class GeoToolViewComponent : EditorComponent<GeomorphToolWindow>
    {
        private Vector2 m_scrollPosition;
        private readonly GeoToolComponent.View m_view;

        protected GeoToolViewComponent(GeoToolComponent parent, GeoToolComponent.View view) : base(parent)
        { 
            this.m_view = view;
        }

        public GeoToolComponent.View View => this.m_view;
        public GeoToolComponent Tool => this.Parent as GeoToolComponent;

        protected virtual void OnHeaderGUI()
        { }

        protected abstract void OnInspectorGUI();

        protected override void OnGUI()
        {
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
    }
}

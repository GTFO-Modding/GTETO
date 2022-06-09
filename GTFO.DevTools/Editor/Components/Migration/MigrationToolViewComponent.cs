using GTFO.DevTools.Windows;
using System;
using UnityEngine;

namespace GTFO.DevTools.Components.Migration
{
    public abstract class MigrationToolViewComponent : EditorComponent<MigrationWindow>
    {
        private readonly MigrationToolComponent.View m_view;

        protected MigrationToolViewComponent(MigrationToolComponent parent, MigrationToolComponent.View view) : base(parent)
        {
            this.m_view = view;
        }

        public MigrationToolComponent.View View => this.m_view;
        public MigrationToolComponent Tool => this.Parent as MigrationToolComponent;

        protected virtual void OnHeaderGUI()
        { }

        protected abstract void OnInspectorGUI();

        protected void DoClose()
        {
            this.Tool.ChangeToView(MigrationToolComponent.View.None);
            this.Reset();
            this.Tool.Window.Close();
        }

        protected override void OnGUI()
        {
            this.OnHeaderGUI();
            try
            {
                this.OnInspectorGUI();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}

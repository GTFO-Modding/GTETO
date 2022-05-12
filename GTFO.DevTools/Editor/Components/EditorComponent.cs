using System;
using System.Collections.Generic;
using UnityEditor;

namespace GTFO.DevTools.Components
{
    public abstract class EditorComponent
    {
        private EditorComponent m_parent;
        private bool m_active;
        private readonly ChildCollection m_children;
        private EditorWindow m_window;

        protected EditorComponent(EditorWindow window) : this((EditorComponent)null)
        {
            this.m_window = window;
        }

        protected EditorComponent(EditorComponent parent)
        {
            this.m_parent = parent;
            this.m_children = new ChildCollection(this);
            this.m_window = parent?.m_window;
            this.m_active = true;
        }

        public EditorWindow Window => this.m_window;
        public EditorComponent Parent => this.m_parent;
        public ChildCollection Children => this.m_children;
        public bool ActiveSelf => this.m_active;
        public bool ActiveInHierarchy
        {
            get
            {
                return this.ActiveSelf &&
                    (this.Parent?.ActiveInHierarchy ?? true);
            }
        }

        private void SetParentImpl(EditorComponent parent)
        {
            this.m_parent = parent;
        }

        public void SetParent(EditorComponent parent)
        {
            if (this.m_parent != null)
            {
                this.m_parent.Children.RemoveChild(this);
            }

            if (parent != null)
            {
                parent.Children.AddChild(this);
            }
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

        public void SetActive(bool value)
        {
            if (this.m_active == value) return;

            this.m_active = value;

            if (value)
                this.OnShow();
            else
                this.OnHide();
        }

        public void Draw()
        {
            if (!this.ActiveSelf)
                return;

            if (this.NeedStyleRefresh)
                this.RefreshStyle();

            this.OnGUI();
            for (int index = 0; index < this.Children.ChildCount; index++)
            {
                var child = this.Children.GetChildAt(index);
                child.Draw();
            }
        }

        protected virtual void OnGUI()
        { }

        public sealed class ChildCollection
        {
            private readonly List<EditorComponent> m_children = new List<EditorComponent>();
            private readonly EditorComponent m_component;

            public ChildCollection(EditorComponent component)
            {
                this.m_component = component;
            }

            public EditorComponent Component => this.m_component;

            public int ChildCount => this.m_children.Count;

            public bool ContainsChild(EditorComponent child)
            {
                return this.m_children.Contains(child);
            }
            public void AddChild(EditorComponent child)
            {
                if (child == null || this.ContainsChild(child)) return;
                this.m_children.Add(child);
                child.SetParentImpl(this.Component);
            }
            public EditorComponent GetChildAt(int index)
            {
                if (index < 0 || index >= this.m_children.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return this.m_children[index];
            }
            public void InsertChild(int index, EditorComponent child)
            {
                if (child == null || this.ContainsChild(child)) return;

                this.m_children.Insert(index, child);
                child.SetParentImpl(this.Component);
            }
            public bool RemoveChild(EditorComponent child)
            {
                int index = this.m_children.IndexOf(child);
                if (index > -1)
                {
                    this.RemoveChildAt(index);
                    return true;
                }
                return false;
            }
            public void RemoveChildAt(int index)
            {
                if (index < 0 || index >= this.ChildCount)
                    return;

                var child = this.m_children[index];
                this.m_children.RemoveAt(index);
                child.SetParentImpl(null);
            }
        }
    }

    public abstract class EditorComponent<TWindow> : EditorComponent
        where TWindow : EditorWindow
    {
        protected EditorComponent(TWindow window) : base(window)
        { }

        protected EditorComponent(EditorComponent<TWindow> component) : base(component)
        { }

        public new TWindow Window => (TWindow)base.Window;
    }
}

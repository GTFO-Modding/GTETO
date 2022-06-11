using GTFO.DevTools.Windows;
using System;

namespace GTFO.DevTools.Components.Migration
{
    public class MigrationToolComponent : EditorComponent<MigrationWindow>
    {
        public enum View
        {
            None,
            Export,
            ExportFinished,
            ExportFailed,
            Import,
            ImportFile,
            Importing,
            ImportFailed,
            Transfer,
            TransferFinished
        }

        private View m_view;

        public MigrationToolComponent(MigrationWindow window) : base(window)
        {
            this.Children.AddChild(new DefaultComponent(this));
            this.Children.AddChild(new ExportComponent(this));
            this.Children.AddChild(new ExportFinishedComponent(this));
            this.Children.AddChild(new ExportFailedComponent(this));
            this.Children.AddChild(new ImportComponent(this));
            this.Children.AddChild(new ImportFileComponent(this));
            this.Children.AddChild(new ImportingComponent(this));
            this.Children.AddChild(new ImportFailedComponent(this));
            this.Children.AddChild(new TransferComponent(this));
            this.Children.AddChild(new TransferFinishedComponent(this));

            this.ChangeToView(View.None);
        }

        public bool TryGetViewComponent<TComponent>(View view, out TComponent component)
            where TComponent : MigrationToolViewComponent
        {
            if (this.TryGetViewComponent(view, out MigrationToolViewComponent comp) && comp is TComponent tcomp)
            {
                component = tcomp;
                return true;
            }

            component = null;
            return false;
        }

        public bool TryGetViewComponent(View view, out MigrationToolViewComponent component)
        {
            for (int index = 0; index < this.Children.ChildCount; index++)
            {
                if (this.Children.GetChildAt(index) is MigrationToolViewComponent child &&
                    child.View == view)
                {
                    component = child;
                    return true;
                }
            }

            component = null;
            return false;
        }

        public TComponent GetViewComponent<TComponent>(View view)
            where TComponent : MigrationToolViewComponent
        {
            return (TComponent)this.GetViewComponent(view);
        }

        public MigrationToolViewComponent GetViewComponent(View view)
        {
            if (this.TryGetViewComponent(view, out MigrationToolViewComponent component))
            {
                return component;
            }

            throw new ArgumentOutOfRangeException(nameof(view), $"No such component for view '{view}' exists!");
        }

        public override void OnShow()
        {
            base.OnShow();
            this.GetCurrentView()?.OnShow();
        }

        public void ChangeToView(View view)
        {
            for (int index = 0; index < this.Children.ChildCount; index++)
            {
                var child = (this.Children.GetChildAt(index) as MigrationToolViewComponent);
                if (child == null) continue;

                child.SetActive(child.View == view);
            }
            this.m_view = view;
        }

        public MigrationToolViewComponent GetCurrentView()
        {
            return this.TryGetViewComponent(this.m_view, out MigrationToolViewComponent component) ?
                component : null;
        }
    }
}

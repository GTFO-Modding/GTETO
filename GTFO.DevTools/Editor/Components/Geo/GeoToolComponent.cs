using GTFO.DevTools.Geo;

namespace GTFO.DevTools.Components.Geo
{
    public class GeoToolComponent : EditorComponent<GeomorphToolWindow>
    {
        public enum View
        {
            Default,
            Settings,
            CreateGeomorph
        }

        private View m_view;

        public GeoToolComponent(GeomorphToolWindow window) : base(window)
        {
            this.Children.AddChild(new DefaultComponent(this));
            this.Children.AddChild(new SettingsComponent(this));
            this.Children.AddChild(new CreateNewGeomorphComponent(this));

            this.ChangeToView(View.Default);
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
                var child = (this.Children.GetChildAt(index) as GeoToolViewComponent);
                if (child == null) continue;

                child.SetActive(child.View == view);
            }
            this.m_view = view;
        }

        public GeoToolViewComponent GetCurrentView()
        {

            for (int index = 0; index < this.Children.ChildCount; index++)
            {
                var child = (this.Children.GetChildAt(index) as GeoToolViewComponent);
                if (child != null && child.View == this.m_view)
                    return child;
            }
            return null;
        }
    }
}

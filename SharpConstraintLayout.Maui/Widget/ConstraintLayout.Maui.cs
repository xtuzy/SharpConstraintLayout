#if __MAUI__
using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIElement = Microsoft.Maui.Controls.View;
namespace SharpConstraintLayout.Maui.Widget
{
    public partial class ConstraintLayout
    {
        public ConstraintLayout()
        {
            init();
        }

        #region Add And Remove

        public UIElement GetChildAt(int index)
        {
            return Children[index] as UIElement;
        }

        public void AddElement(UIElement element)
        {
            Children?.Add(element);
            OnAddedView(element);
        }

        public void RemoveElement(UIElement element)
        {
            Children?.Remove(element);
            OnRemovedView(element);
        }

        public void RemoveAllElements()
        {
            foreach (var element in this.Children)
            {
                Children?.Remove(element);
                OnRemovedView(element as UIElement);
            }
        }

        public int ChildCount { get { return Children != null ? Children.Count : 0; } }

        #endregion

        #region Layout

        protected override ILayoutManager CreateLayoutManager()
        {
            return new ConstraintLayoutManager(this);
        }

        void LayoutChild(UIElement element, int x, int y, int w, int h)
        {
            element.Arrange(new Rect(x, y, w, h));
        }

        #endregion
    }

    internal class ConstraintLayoutManager : LayoutManager
    {
        WeakReference<ConstraintLayout> Layout;
        public ConstraintLayoutManager(Microsoft.Maui.ILayout layout) : base(layout)
        {
            Layout = new WeakReference<ConstraintLayout>(layout as ConstraintLayout);
        }

        public override Size Measure(double widthConstraint, double heightConstraint)
        {
            Layout.TryGetTarget(out var layout);
            return layout.Measure(new Size(widthConstraint, heightConstraint));
        }

        public override Size ArrangeChildren(Rect bounds)
        {
            Layout.TryGetTarget(out var layout);
            if (bounds.Width != layout.MLayoutWidget.Width || bounds.Height != layout.MLayoutWidget.Height)
            {
                // We haven't received our desired size. We need to refresh the rows.
                layout.Measure(bounds.Size);
            }

            layout.OnLayout();
            return bounds.Size;
        }
    }
}
#endif
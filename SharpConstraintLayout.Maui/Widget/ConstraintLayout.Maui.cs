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
    public partial class ConstraintLayout : IConstraintLayout
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
            this.Add(element);
            OnAddedView(element);
        }

        public void RemoveElement(UIElement element)
        {
            this.Remove(element);
            OnRemovedView(element);
        }

        public void RemoveAllElements()
        {
            foreach (var element in this.Children)
            {
                this.Remove(element);
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
            //AbsoluteLayout.SetLayoutBounds(element, new Rect(x, y, w, h));
        }
        #endregion

        public Size GetMLayoutWidgetSize() { return new Size(MLayoutWidget.Width, MLayoutWidget.Height); }
    }

    internal class ConstraintLayoutManager : LayoutManager
    {
        WeakReference<IConstraintLayout> Layout;

        public ConstraintLayoutManager(IConstraintLayout constraintLayout) : base(constraintLayout)
        {
            Layout = new WeakReference<IConstraintLayout>(constraintLayout);
        }

        public override Size Measure(double widthConstraint, double heightConstraint)
        {
            //base.Measure(widthConstraint, heightConstraint);
            Layout.TryGetTarget(out var layout);
            var size = layout.MeasureLayout(new Size(widthConstraint, heightConstraint));

            return size;
        }

        public override Size ArrangeChildren(Rect bounds)
        {
            Layout.TryGetTarget(out var layout);
            var layoutWidgwtSize = layout.GetMLayoutWidgetSize();
            if (bounds.Width != layoutWidgwtSize.Width || bounds.Height != layoutWidgwtSize.Height)
            {
                // We haven't received our desired size. We need to refresh the rows.
                layout.MeasureLayout(bounds.Size);
            }

            layout.ArrangeLayout();
            //base.ArrangeChildren(bounds);
            return bounds.Size;
        }
    }

    interface IConstraintLayout : Microsoft.Maui.ILayout
    {
        Size MeasureLayout(Size availableSize);
        void ArrangeLayout();

        Size GetMLayoutWidgetSize();
    }
}
#endif
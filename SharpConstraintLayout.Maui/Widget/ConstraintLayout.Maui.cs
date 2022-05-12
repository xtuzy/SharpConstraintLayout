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
    public partial class ConstraintLayout : IMauiConstraintLayout
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

        public (bool isInfinityAvailabelWidth, bool isInfinityAvailabelHeight) IsInfinitable(ConstraintLayout layout, int constrainWidth, int constrainHeight, Size availableSize)
        {
            bool isInfinityAvailabelWidth = false;
            bool isInfinityAvailabelHeight = false;
            if (double.IsPositiveInfinity(availableSize.Width))
            {
                isInfinityAvailabelWidth = true;
            }

            if (double.IsPositiveInfinity(availableSize.Height))
            {
                isInfinityAvailabelHeight = true;
            }
            return (isInfinityAvailabelWidth, isInfinityAvailabelHeight);
        }
        protected override ILayoutManager CreateLayoutManager()
        {
            return new ConstraintLayoutManager(this);
        }

        void LayoutChild(UIElement element, int x, int y, int w, int h)
        {
            //element.Arrange(new Rect(x, y, w, h));
            AbsoluteLayout.SetLayoutBounds(element, new Rect(x, y, w, h));
        }
        #endregion

        public Size GetLastMeasureSize() { return new Size(mLastMeasureWidth, mLastMeasureHeight); }
    }

    internal class ConstraintLayoutManager : AbsoluteLayoutManager
    {
        WeakReference<IMauiConstraintLayout> Layout;

        public ConstraintLayoutManager(IMauiConstraintLayout constraintLayout) : base(constraintLayout)
        {
            Layout = new WeakReference<IMauiConstraintLayout>(constraintLayout);
        }

        public override Size Measure(double widthConstraint, double heightConstraint)
        {

            Layout.TryGetTarget(out var layout);

            var availableSize = new Size(widthConstraint, heightConstraint);

            (int horizontalSpec, int verticalSpec) = layout.MakeSpec(layout as ConstraintLayout, availableSize);

            var finalSize = layout.MeasureLayout(availableSize, horizontalSpec, verticalSpec);
            base.Measure(widthConstraint, heightConstraint);

            return finalSize;
        }

        public override Size ArrangeChildren(Rect bounds)
        {
            Size finalSize = bounds.Size;
            Layout.TryGetTarget(out var layout);
            var lastMeasureSize = layout.GetLastMeasureSize();
            if (finalSize.Width != lastMeasureSize.Width || finalSize.Height != lastMeasureSize.Height)
            {
                // We haven't received our desired size. We need to refresh the rows.
                (int horizontalSpec, int verticalSpec) = layout.MakeSpec(layout as ConstraintLayout, finalSize);
                finalSize = layout.MeasureLayout(finalSize, horizontalSpec, verticalSpec);
            }

            layout.ArrangeLayout();
            base.ArrangeChildren(bounds);
            return finalSize;
        }
    }

    interface IMauiConstraintLayout : Microsoft.Maui.IAbsoluteLayout
    {
        Size MeasureLayout(Size availableSize, int horizontalSpec = 0, int verticalSpec = 0);
        void ArrangeLayout();

        Size GetLastMeasureSize();

        (int horizontalSpec, int verticalSpec) MakeSpec(ConstraintLayout layout, Size availableSize);
    }
}
#endif
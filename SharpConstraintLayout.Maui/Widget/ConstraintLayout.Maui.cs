#if __MAUI__
using Microsoft.Maui.Layouts;
using SharpConstraintLayout.Maui.Widget;
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
        public new void Add(IView child)
        {
            base.Add(child);
            OnAddedView(child as UIElement);
        }

        public UIElement GetChildAt(int index)
        {
            return Children[index] as UIElement;
        }

        public void AddElement(UIElement element)
        {
            this.Add(element);
        }

        public new void Remove(IView child)
        {
            base.Remove(child);
            OnRemovedView(child as UIElement);
        }

        public void RemoveElement(UIElement element)
        {
            this.Remove(element);
        }

        public void RemoveAllElements()
        {
            for (int i = this.Children.Count - 1; i >= 0; i--)
            {
                var element = Children[i];
                this.Remove(element);
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

        public void LayoutChild(UIElement element, int x, int y, int w, int h)
        {
            (element as IView).Arrange(new Rect(x, y, w, h));
        }
        #endregion

        public Size GetLastMeasureSize() { return new Size(mLastMeasureWidth, mLastMeasureHeight); }

        /// <summary>
        /// 获取ConstraintLayout所有子View信息,这是为动画特别提供的
        /// </summary>
        /// <param name="isNeedMeasure">没有测量过的可能需要测量才能获得到正确的信息</param>
        /// <returns></returns>
        public Dictionary<int, ViewInfo> CaptureLayoutTreeInfo(bool isNeedMeasure = false)
        {
            //Try强制Measure
            if (isNeedMeasure)
            {
                var availableSize = GetLastMeasureSize();
                (int horizontalSpec, int verticalSpec) = this.MakeSpec(this, availableSize);
                this.MeasureLayout(availableSize, horizontalSpec, verticalSpec);
            }

            var dic = new Dictionary<int, ViewInfo>();
            foreach (var item in idToViews)
            {
                var view = item.Value;
                var widget = GetOrAddWidgetById(item.Key);
                var info = new ViewInfo() { X = widget.X, Y = widget.Y, Size = new Size(widget.Width, widget.Height), TranlateX = view.TranslationX, TranlateY = view.TranslationY, ScaleX = view.ScaleX, ScaleY = view.ScaleY, Alpha = view.Opacity, RotationX = view.RotationX, RotationY = view.RotationY };
                dic.Add(item.Key, info);
            }
            return dic;
        }
    }

    public class ConstraintLayoutManager : LayoutManager
    {
        public ConstraintLayoutManager(IMauiConstraintLayout constraintLayout) : base(constraintLayout)
        {
        }

        public override Size Measure(double widthConstraint, double heightConstraint)
        {
            var availableSize = new Size(widthConstraint, heightConstraint);
            var layout = Layout as ConstraintLayout;
            (int horizontalSpec, int verticalSpec) = layout.MakeSpec(layout, availableSize);

            var finalSize = layout.MeasureLayout(availableSize, horizontalSpec, verticalSpec);

            return finalSize;
        }

        public override Size ArrangeChildren(Rect bounds)
        {
            Size finalSize = bounds.Size;
            var layout = Layout as ConstraintLayout;
            if (layout.mConstraintSet.IsForAnimation)
                return finalSize;
            var lastMeasureSize = layout.GetLastMeasureSize();
            if (finalSize.Width != lastMeasureSize.Width || finalSize.Height != lastMeasureSize.Height)
            {
                // We haven't received our desired size. We need to refresh the rows.
                (int horizontalSpec, int verticalSpec) = layout.MakeSpec(layout as ConstraintLayout, finalSize);
                finalSize = layout.MeasureLayout(finalSize, horizontalSpec, verticalSpec);
            }

            layout.ArrangeLayout();
            return finalSize;
        }
    }

    public interface IMauiConstraintLayout : Microsoft.Maui.ILayout
    {
        Size MeasureLayout(Size availableSize, int horizontalSpec = 0, int verticalSpec = 0);
        void ArrangeLayout();

        Size GetLastMeasureSize();

        (int horizontalSpec, int verticalSpec) MakeSpec(ConstraintLayout layout, Size availableSize);
    }
}
#endif
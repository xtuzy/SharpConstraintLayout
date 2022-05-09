#if WINDOWS && !__MAUI__
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

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
            return Children[index];
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
                OnRemovedView(element);
            }
        }

        public int ChildCount { get { return Children != null ? Children.Count : 0; } }

        #endregion

        protected override Size MeasureOverride(Size availableSize)
        {
            /*
             * Analysis available size
             */

            //sometimes no fixsize
            if (double.IsPositiveInfinity(availableSize.Width))
            {
                isInfinityAvailabelWidth = true;
            }

            if (double.IsPositiveInfinity(availableSize.Height))
            {
                isInfinityAvailabelHeight = true;
            }

            return MeasureLayout(availableSize);
        }

        #region Layout

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (DEBUG) Debug.WriteLine($"{nameof(ArrangeOverride)} {this} {finalSize}");

            //参考:https://github.com/CommunityToolkit/WindowsCommunityToolkit/blob/main/Microsoft.Toolkit.Uwp.UI.Controls.Primitives/WrapPanel/WrapPanel.cs
            if (finalSize.Width != MLayoutWidget.Width || finalSize.Height != MLayoutWidget.Height)
            {
                // We haven't received our desired size. We need to refresh the rows.
                finalSize = MeasureLayout(finalSize);
            }

            ArrangeLayout();

            //return new Size(MLayoutWidget.Width, MLayoutWidget.Height);//这里必须返回Widget的大小,因为返回值决定了layout的绘制范围?
            return finalSize;
        }

        void LayoutChild(UIElement element, int x, int y, int w, int h)
        {
            element.Arrange(new Rect(x, y, w, h));
        }

        #endregion
    }
}
#endif
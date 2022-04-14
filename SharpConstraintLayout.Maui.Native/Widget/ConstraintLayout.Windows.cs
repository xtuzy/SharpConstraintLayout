using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if WINDOWS
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
#elif __IOS__
using Panel = UIKit.UIView;
using UIElement = UIKit.UIView;
using CoreGraphics;
using Size = CoreGraphics.CGSize;
using Microsoft.Maui.Graphics;
#elif __ANDROID__
#endif
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

        public void AddView(UIElement element)
        {
            Children?.Add(element);
            OnAddedView(element);
        }

        public void RemoveView(UIElement element)
        {
            Children?.Remove(element);
            OnRemovedView(element);
        }

        public void RemoveAllViews()
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
            int availableWidth = 0;
            int availableHeight = 0;

            //sometimes no fixsize
            if (double.IsPositiveInfinity(availableSize.Width))
            {
                availableWidth = int.MaxValue; isInfinityAvailabelSize = true;
            }
            else
                availableWidth = (int)availableSize.Width;

            if (double.IsPositiveInfinity(availableSize.Height))
            {
                availableHeight = int.MaxValue;
                isInfinityAvailabelSize = true;
            }
            else
            {
                availableHeight = (int)availableSize.Height;
            }

            return Measure(new Size(availableWidth, availableHeight));
        }

        #region Layout

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (DEBUG) Debug.WriteLine($"{nameof(ArrangeOverride)} {this} {finalSize}");

            if (finalSize.Width != MLayoutWidget.Width || finalSize.Height != MLayoutWidget.Height)
            {
                // We haven't received our desired size. We need to refresh the rows.
                Measure(finalSize);
            }

            OnLayout();

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

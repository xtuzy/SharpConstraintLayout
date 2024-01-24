﻿#if WINDOWS && !__MAUI__
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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

        public ConstraintLayout(ILogger logger)
        {
            Logger = logger;
            init();
        }

        #region Add And Remove

        public UIElement GetChildAt(int index)
        {
            return Children[index];
        }

        public void AddElement(UIElement element)
        {
            OnAddedView(element);
            Children?.Add(element);
        }

        public void RemoveElement(UIElement element)
        {
            OnRemovedView(element);
            Children?.Remove(element);
        }

        public void RemoveAllElements()
        {
            for (int i = this.Children.Count - 1; i >= 0; i--)
            {
                var element = Children[i];
                OnRemovedView(element);
                Children?.Remove(element);
            }
        }

        public int ChildCount { get { return Children != null ? Children.Count : 0; } }

        #endregion

        protected override Size MeasureOverride(Size availableSize)
        {
            /*
             * Analysis available size
             */
            (int horizontalSpec, int verticalSpec) = MakeSpec(this, availableSize);

            var size = MeasureLayout(availableSize, horizontalSpec, verticalSpec);
            return new Size(size.Width, size.Height);
        }

        /// <summary>
        /// 判断是否可以无限大小
        /// </summary>
        /// <returns></returns>
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

        #region Layout

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (DEBUGCONSTRAINTLAYOUTPROCESS) Debug.WriteLine($"{nameof(ArrangeOverride)} {this} {finalSize}");

            //参考:https://github.com/CommunityToolkit/WindowsCommunityToolkit/blob/main/Microsoft.Toolkit.Uwp.UI.Controls.Primitives/WrapPanel/WrapPanel.cs
            if (finalSize.Width != mLastMeasureWidth || finalSize.Height != mLastMeasureHeight)
            {
                // We haven't received our desired size. We need to refresh the rows.
                (int horizontalSpec, int verticalSpec) = MakeSpec(this, finalSize);
                var size = MeasureLayout(finalSize, horizontalSpec, verticalSpec);
                finalSize = new Size(size.Width, size.Height);
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
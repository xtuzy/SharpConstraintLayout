// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MauiWrapPanel.Base;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
namespace MauiWrapPanel.WrapPanel
{
#if __ANDROID__
using ElementView = Android.Views.View;
#elif __IOS__
using ElementView = UIKit.UIView;
#elif WINDOWS
    using ElementView = Microsoft.UI.Xaml.UIElement;
#endif

    /// <summary>
    /// WrapPanel is a panel that position child control vertically or horizontally based on the orientation and when max width / max height is reached a new row (in case of horizontal) or column (in case of vertical) is created to fit new controls.
    /// </summary>
    public partial class WrapPanel : BaseLayout
    {
        public enum LayoutOrientation
        {
            Y,
            X,
        }

        /// <summary>
        /// Gets or sets a uniform Horizontal distance (in pixels) between items when <see cref="Orientation"/> is set to Horizontal,
        /// or between columns of items when <see cref="Orientation"/> is set to Vertical.
        /// </summary>
        public double HorizontalSpacing;
        /// <summary>
        /// Gets or sets a uniform Vertical distance (in pixels) between items when <see cref="Orientation"/> is set to Vertical,
        /// or between rows of items when <see cref="Orientation"/> is set to Horizontal.
        /// </summary>
        public double VerticalSpacing;

        /// <summary>
        /// Gets or sets the orientation of the WrapPanel.
        /// Horizontal means that child controls will be added horizontally until the width of the panel is reached, then a new row is added to add new child controls.
        /// Vertical means that children will be added vertically until the height of the panel is reached, then a new column is added.
        /// </summary>
        public LayoutOrientation Orientation;

        /// <summary>
        /// Gets or sets the distance between the border and its child object.
        /// </summary>
        /// <returns>
        /// The dimensions of the space between the border and its child as a Thickness value.
        /// Thickness is a structure that stores dimension values using pixel measures.
        /// </returns>
        public Microsoft.Maui.Graphics.Rect Padding;

        /// <summary>
        /// Gets or sets a value indicating how to arrange child items
        /// </summary>
        public StretchChild StretchChild;

        protected override Size MauiMeasure(Size availableSize)
        {
            var childAvailableSize = new Size(
                availableSize.Width - Padding.Left - Padding.Right,
                availableSize.Height - Padding.Top - Padding.Bottom);
            for (var index = 0; index < ChildCount; index++)
            {
                var child = GetChildAt(index);
                MeasureElement(child, (int)childAvailableSize.Width, (int)childAvailableSize.Height);
            }

            var requiredSize = UpdateRows(availableSize);
            return requiredSize;
        }

        protected override Size MauiLayout(Rect finalSize)
        {
            if ((Orientation == LayoutOrientation.X && finalSize.Width < GetElementSize(this).Width) ||
                (Orientation == LayoutOrientation.Y && finalSize.Height < GetElementSize(this).Height))
            {
                // We haven't received our desired size. We need to refresh the rows.
                UpdateRows(finalSize.Size);
            }

            if (_rows.Count > 0)
            {
                // Now that we have all the data, we do the actual arrange pass
                var childIndex = 0;
                foreach (var row in _rows)
                {
                    foreach (var rect in row.ChildrenRects)
                    {
                        var child = GetChildAt(childIndex++);
                        while (IsVisiable(child) == false)
                        {
                            // Collapsed children are not added into the rows,
                            // we skip them.
                            child = GetChildAt(childIndex++);
                        }

                        var arrangeRect = new UvRect
                        {
                            Position = rect.Position,
                            Size = new UvMeasure { U = rect.Size.U, V = row.Size.V },
                        };

                        var finalRect = arrangeRect.ToRect(Orientation);
                        LayoutElement(child, (int)finalRect.X, (int)finalRect.Y, (int)finalRect.Width, (int)finalRect.Height);
                    }
                }
            }

            return finalSize.Size;
        }

        private readonly List<Row> _rows = new List<Row>();

        private Size UpdateRows(Size availableSize)
        {
            _rows.Clear();

            var paddingStart = new UvMeasure(Orientation, Padding.Left, Padding.Top);
            var paddingEnd = new UvMeasure(Orientation, Padding.Right, Padding.Bottom);

            if (ChildCount == 0)
            {
                var emptySize = paddingStart.Add(paddingEnd).ToSize(Orientation);
                return emptySize;
            }

            var parentMeasure = new UvMeasure(Orientation, availableSize.Width, availableSize.Height);
            var spacingMeasure = new UvMeasure(Orientation, HorizontalSpacing, VerticalSpacing);
            var position = new UvMeasure(Orientation, Padding.Left, Padding.Top);

            var currentRow = new Row(new List<UvRect>(), default);
            var finalMeasure = new UvMeasure(Orientation, width: 0.0, height: 0.0);
            void Arrange(ElementView child, bool isLast = false)
            {
                if (IsVisiable(child) == false)
                {
                    return; // if an item is collapsed, avoid adding the spacing
                }

                var desiredMeasure = new UvMeasure(Orientation, new Size(GetElementSize(child).Width, GetElementSize(child).Height));
                if ((desiredMeasure.U + position.U + paddingEnd.U) > parentMeasure.U)
                {
                    // next row!
                    position.U = paddingStart.U;
                    position.V += currentRow.Size.V + spacingMeasure.V;

                    _rows.Add(currentRow);
                    currentRow = new Row(new List<UvRect>(), default);
                }

                // Stretch the last item to fill the available space
                if (isLast)
                {
                    desiredMeasure.U = parentMeasure.U - position.U;
                }

                currentRow.Add(position, desiredMeasure);

                // adjust the location for the next items
                position.U += desiredMeasure.U + spacingMeasure.U;
                finalMeasure.U = Math.Max(finalMeasure.U, position.U);
            }

            var lastIndex = ChildCount - 1;
            for (var i = 0; i < lastIndex; i++)
            {
                Arrange(GetChildAt(i));
            }

            Arrange(GetChildAt(lastIndex), StretchChild == StretchChild.Last);
            if (currentRow.ChildrenRects.Count > 0)
            {
                _rows.Add(currentRow);
            }

            if (_rows.Count == 0)
            {
                var emptySize = paddingStart.Add(paddingEnd).ToSize(Orientation);
                return emptySize;
            }

            // Get max V here before computing final rect
            var lastRowRect = _rows.Last().Rect;
            finalMeasure.V = lastRowRect.Position.V + lastRowRect.Size.V;
            var finalRect = finalMeasure.Add(paddingEnd).ToSize(Orientation);
            return finalRect;
        }
    }

}
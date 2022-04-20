using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpConstraintLayout.Maui.Native.Example
{
    using ViewElement = Microsoft.UI.Xaml.UIElement;

    public static class ViewPositionExtension
    {
        public static Size GetSize(this ViewElement view)
        {
            return new Size(view.ActualSize.X, view.ActualSize.Y);
        }

        public static Rect GetBounds(this ViewElement view)
        {
            return new Rect(view.ActualOffset.X, view.ActualOffset.Y, view.ActualSize.X, view.ActualSize.Y);
        }

        public static Rect GetBoundsOnScreen(this ViewElement view)
        {
            //when window is full screen.
            return GetBoundsOnWindow(view);
        }

        public static Rect GetBoundsOnWindow(this ViewElement view)
        {
            Microsoft.UI.Xaml.Window window = Microsoft.UI.Xaml.Window.Current;
            //https://cxyzjd.com/article/X___V/8655798
            var point = view.TransformToVisual(window.Content).TransformPoint(new Windows.Foundation.Point(0, 0));
            return new Rect(point.X, point.Y, view.ActualSize.X, view.ActualSize.Y);
        }

        public static Point GetCenter(this ViewElement view)
        {
            var rect = GetBounds(view);
            return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }
    }
}
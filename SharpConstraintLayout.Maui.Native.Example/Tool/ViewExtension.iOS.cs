using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpConstraintLayout.Maui.Native.Example.Tool
{
    using ViewElement = UIKit.UIView;

    public static class ViewPositionExtension
    {
        public static Size GetSize(this ViewElement view)
        {
            return new Size(view.Frame.Width, view.Frame.Height);
        }

        public static Rect GetBounds(this ViewElement view)
        {
            return new Rect(view.Frame.X, view.Frame.Y, view.Frame.Width, view.Frame.Height);
        }

        public static Rect GetBoundsOnScreen(this ViewElement view)
        {
            //https://www.jianshu.com/p/6d087d5bc0ca
            var rect = view.ConvertRectToView(view.Bounds, view.Window);
            return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static Rect GetBoundsOnWindow(this ViewElement view)
        {
            //when window is full screen.
            return view.GetBoundsOnScreen();
        }

        public static Point GetCenter(this ViewElement view)
        {
            var rect = view.GetBounds();
            return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpConstraintLayout.Maui.Example.Tool
{
    using ViewElement = Microsoft.Maui.Controls.View;

    public static class ViewPositionExtension
    {
        public static Size GetSize(this ViewElement view)
        {
            return new Size(view.Width, view.Height);
        }

        public static Rect GetBounds(this ViewElement view)
        {
            return view.Bounds;
        }

        public static Rect GetBoundsOnScreen(this ViewElement view)
        {
            //when window is full screen.
            return GetBoundsOnWindow(view);
        }

        public static Rect GetBoundsOnWindow(this ViewElement view)
        {
            return new Rect(view.X, view.Y, view.Width, view.Height);
        }

        public static Point GetCenter(this ViewElement view)
        {
            var rect = GetBounds(view);
            return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }
    }
}

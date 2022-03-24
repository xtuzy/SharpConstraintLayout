using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpConstraintLayout.Maui.Native.Example
{
    using ViewElement = Android.Views.View;

    public static class ViewPositionExtension
    {
        public static Size GetSize(this ViewElement view)
        {
            return new Size(view.Width, view.Height);
        }

        public static Rect GetBounds(this ViewElement view)
        {
            //https://www.jianshu.com/p/9c3a829abbfe
            // 注：要在onWindowFocusChanged（）里获取，即等window窗口发生变化后
            return new Rect(view.Left, view.Top, view.Width, view.Height);
        }

        public static Rect GetBoundsOnScreen(this ViewElement view)
        {
            //https://www.jianshu.com/p/9c3a829abbfe
            // 注：要在view.post（Runable）里获取，即等布局变化后
            int[] location = new int[2];
            view.GetLocationInWindow(location);
            return new Rect(location[0], location[1], view.Width, view.Height);
        }

        public static Rect GetBoundsOnWindow(this ViewElement view)
        {
            //https://www.jianshu.com/p/9c3a829abbfe
            int[] location = new int[2];
            view.GetLocationOnScreen(location);
            return new Rect(location[0], location[1], view.Width, view.Height);
        }

        public static Point GetCenter(this ViewElement view)
        {
            var rect = GetBounds(view);
            return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }
    }
}
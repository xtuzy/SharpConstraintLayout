using Android.Graphics;
using Android.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiWrapPanel.Base
{
    public static class UnitConverter
    {
        public static Microsoft.Maui.Graphics.Rect ToMauiRect(this Rect rect)
        {
            return new Microsoft.Maui.Graphics.Rect(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }

        public static Rect ToPlatformRect(this Microsoft.Maui.Graphics.Rect rect)
        {
            return new Rect((int)rect.Left, (int)rect.Top, (int)rect.Right, (int)rect.Bottom);
        }

        public static Microsoft.Maui.Graphics.Point ToMauiPoint(this Point point)
        {
            return new Microsoft.Maui.Graphics.Point(point.X, point.Y);
        }

        public static Point ToPlatformPoint(this Microsoft.Maui.Graphics.Point point)
        {
            return new Point((int)point.X, (int)point.Y);
        }

        public static Microsoft.Maui.Graphics.Size ToMauiSize(this Size size)
        {
            return new Microsoft.Maui.Graphics.Size(size.Width, size.Height);
        }

        public static Size ToPlatformSize(this Microsoft.Maui.Graphics.Size size)
        {
            return new Size((int)size.Width, (int)size.Height);
        }
    }
}

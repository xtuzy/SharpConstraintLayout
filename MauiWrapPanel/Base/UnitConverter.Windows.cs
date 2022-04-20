using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace MauiWrapPanel.Base
{
    public static class UnitConverter
    {
        public static Microsoft.Maui.Graphics.Rect ToMauiRect(this Rect rect)
        {
            return new Microsoft.Maui.Graphics.Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static Microsoft.Maui.Graphics.Size ToMauiSize(this Size size)
        {
            return new Microsoft.Maui.Graphics.Size(size.Width, size.Height);
        }

        public static Microsoft.Maui.Graphics.Point ToMauiPoint(this Point point)
        {
            return new Microsoft.Maui.Graphics.Point(point.X, point.Y);
        }

        public static Rect ToPlaformRect(this Microsoft.Maui.Graphics.Rect rect)
        {
            return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static Size ToPlaformSize(this Microsoft.Maui.Graphics.Size size)
        {
            return new Size(size.Width, size.Height);
        }

        public static Point ToPlaformPoint(this Microsoft.Maui.Graphics.Point point)
        {
            return new Point(point.X, point.Y);
        }
    }
}

using CoreGraphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiWrapPanel.Base
{
    public static class UnitConverter
    {
        public static Microsoft.Maui.Graphics.Rect ToMauiRect(this CGRect rect)
        {
            return new Microsoft.Maui.Graphics.Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static CGRect ToPlatformRect(this Microsoft.Maui.Graphics.Rect rect)
        {
            return new CGRect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static CGSize ToPlatformSize(this Microsoft.Maui.Graphics.Size size)
        {
            return new CGSize(size.Width, size.Height);
        }

        public static Microsoft.Maui.Graphics.Size ToMauiSize(this CGSize size)
        {
            return new Microsoft.Maui.Graphics.Size(size.Width, size.Height);
        }

        public static CGPoint ToPlatformPoint(this Microsoft.Maui.Graphics.Point point)
        {
            return new CGPoint(point.X, point.Y);
        }

        public static Microsoft.Maui.Graphics.Point ToMauiPoint(this CGPoint point)
        {
            return new Microsoft.Maui.Graphics.Point(point.X, point.Y);
        }
    }
}

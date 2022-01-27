using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FontBaseline.Wpf
{
    public static class FontBaselineHelper
    {
        /// <summary>
        /// In first line text,the distence from center of text to baseline of text.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="fontFamily"></param>
        /// <param name="fontSize"></param>
        /// <param name="pixelsPerDpi"></param>
        /// <returns></returns>
        public static double GetBaseline(string content,string fontFamily,double fontSize, double pixelsPerDpi)
        {
            //measure text see https://stackoverflow.com/a/52972071/13254773
            var typeface = new Typeface(fontFamily);
            var formattedText = new FormattedText(content, Thread.CurrentThread.CurrentCulture, FlowDirection.LeftToRight, typeface, fontSize, new VisualBrush(), pixelsPerDpi);
            var textBaselineHeight = formattedText.Baseline;//in first line text,the distence from top of text to baseline of text.
            return textBaselineHeight-fontSize/2;
        }
    }
}


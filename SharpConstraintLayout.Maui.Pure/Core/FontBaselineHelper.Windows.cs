namespace SharpConstraintLayout.Maui.Pure.Core
{
    using Microsoft.Maui.Essentials;
    using Microsoft.Maui.Controls.Internals;
    using SkiaSharp;

    // All the code in this file is only included on Windows.
    public partial class FontBaselineHelper
    {
        static SKPaint paint = new SKPaint();
        /// <summary>
        /// Baseline To Font Center Height
        /// </summary>
        /// <param name="fontSize"></param>
        /// <returns></returns>
        public static float GetBaseline(int fontSize)
        {
            paint.TextSize = fontSize;
            paint.Typeface = SKTypeface.Default;

            var fontMetrics = paint.FontMetrics;
            return (fontMetrics.Descent - fontMetrics.Ascent) / 2 - fontMetrics.Descent;
        }
    }
}
namespace FontBaseline.Maui.Skia
{
    using Microsoft.Maui.Essentials;
    using Microsoft.Maui.Controls.Internals;
    using SkiaSharp;

    // All the code in this file is only included on Windows.
    public partial class FontBaselineHelper
    {
        static SKPaint paint = new SKPaint();
        public (double fontHeight, float baselineToTextCenterHeight) GetBaseline(IFontElement button)
        {
            paint.TextSize = (float)button.FontSize;
            paint.Typeface = SKTypeface.Default;

            var fontMetrics = paint.FontMetrics;
            return (button.FontSize, (fontMetrics.Descent - fontMetrics.Ascent) / 2 - fontMetrics.Descent);
        }
    }
}
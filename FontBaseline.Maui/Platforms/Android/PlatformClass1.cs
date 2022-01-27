using static Android.Graphics.Paint;
using Paint = Android.Graphics.Paint;
using Microsoft.Maui.Controls.Internals;
using Android.Graphics;

namespace FontBaseline.Maui
{
    // All the code in this file is only included on Android.
    public partial class PlatformClass1
    {
        static Paint paint = new Paint();
        /// <summary>
        /// 参考资料
        /// iOS:https://stackoverflow.com/questions/9910766/how-to-align-baselines-of-text-in-uilabels-with-different-font-sizes-on-ios
        /// Android:https://www.jianshu.com/p/057ce6b81c52
        /// WPF:
        /// </summary>
        /// <param name="entry"></param>
        public (double fontHeight, float baselineToTextCenterHeight) GetBaseline(IFontElement entry)
        {
            var fontSize = entry.FontSize;
            paint.SetTypeface(Typeface.Create(entry.FontFamily, TypefaceStyle.Normal));
            paint.TextSize = (float)fontSize;
            FontMetrics fontMetrics = paint.GetFontMetrics();
            return (fontSize, (fontMetrics.Descent - fontMetrics.Ascent) / 2 - fontMetrics.Descent);
        }
    }
}
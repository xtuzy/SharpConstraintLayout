using static Android.Graphics.Paint;
using Paint = Android.Graphics.Paint;
namespace SharpConstraintLayout.Maui
{
    // All the code in this file is only included on Android.
    public class PlatformClass1
    {
        static Paint paint = new Paint();
        /// <summary>
        /// 参考资料
        /// iOS:https://stackoverflow.com/questions/9910766/how-to-align-baselines-of-text-in-uilabels-with-different-font-sizes-on-ios
        /// Android:https://www.jianshu.com/p/057ce6b81c52
        /// WPF:
        /// </summary>
        /// <param name="textBlock"></param>
        (double fontHeight, float baselineToTextCenterHeight) GetBaseline(TextBlock textBlock)
        {
            var fontSize = textBlock.FontSize;
            paint.SetTypeface(Android.Graphics.Typeface.Create(textBlock.FontFamily, Android.Graphics.TypefaceStyle.Normal));
            paint.TextSize = (float)fontSize;
            FontMetrics fontMetrics = paint.GetFontMetrics();
            return(fontSize, (fontMetrics.Descent - fontMetrics.Ascent) / 2 - fontMetrics.Descent);
        }
    }
}
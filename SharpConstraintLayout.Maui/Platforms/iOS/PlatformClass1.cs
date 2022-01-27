using UIKit;

namespace SharpConstraintLayout.Maui
{
    // All the code in this file is only included on iOS.
    public class PlatformClass1
    {
        /// <summary>
        /// 参考资料
        /// iOS:https://stackoverflow.com/questions/9910766/how-to-align-baselines-of-text-in-uilabels-with-different-font-sizes-on-ios
        /// Android:https://www.jianshu.com/p/057ce6b81c52
        /// WPF:
        /// </summary>
        /// <param name="textBlock"></param>
        (double fontHeight,float baselineToTextCenterHeight) GetBaseline(TextBlock textBlock)
        {
            var fontSize = textBlock.FontSize;
            var font = UIFont.FromName(textBlock.FontFamily, (float)fontSize);
            var descent = font.Descender > 0 ? font.Descender : -font.Descender;
            var ascent = font.Ascender > 0 ? font.Ascender : -font.Ascender;
          
            return (fontSize, (float)((descent - ascent) / 2 - descent));
        }
    }
}
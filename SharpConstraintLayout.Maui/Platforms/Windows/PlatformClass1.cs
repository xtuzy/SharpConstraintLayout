namespace SharpConstraintLayout.Maui
{
    using Microsoft.UI.Xaml.Media;
    // All the code in this file is only included on Windows.
    public class PlatformClass1
    {
        void GetBaseline(Button button)
        {
            //measure text see https://stackoverflow.com/a/52972071/13254773
            var typeface = new Typeface(button.FontFamily, button.FontStyle, button.FontWeight, button.FontStretch);
            var dpi = VisualTreeHelper.GetDpi(this).PixelsPerDip;//get dpi see https://stackoverflow.com/a/41556941/13254773
            var formattedText = new FormattedText(button.Content as string, Thread.CurrentThread.CurrentCulture, button.FlowDirection, typeface, button.FontSize, button.Foreground, dpi);
            var textBaselineHeight = formattedText.Baseline;//first line, textTop-textBaseline
           new Font().s
        }
    }
}
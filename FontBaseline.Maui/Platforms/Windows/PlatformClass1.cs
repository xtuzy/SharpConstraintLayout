namespace FontBaseline.Maui
{
    using Microsoft.Maui.Essentials;
    using Microsoft.Maui.Controls.Internals;
    // All the code in this file is only included on Windows.
    public partial class PlatformClass1
    {

        public  (double fontHeight, float baselineToTextCenterHeight) GetBaseline(IFontElement button)
        {
            var pixelsPerDpi = DeviceDisplay.MainDisplayInfo.Density * 96;
            var baseline = FontBaseline.Wpf.FontBaselineHelper.GetBaseline("agf史", button.FontFamily, button.FontSize, pixelsPerDpi);
            return (button.FontSize, (float)baseline);
        }
    }
}
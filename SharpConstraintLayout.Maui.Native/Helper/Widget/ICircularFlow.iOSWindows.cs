#if WINDOWS
    using View = Microsoft.UI.Xaml.FrameworkElement;
    using UIElement = Microsoft.UI.Xaml.UIElement;

    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Windows.Foundation;
#elif __IOS__
using View = UIKit.UIView;
using UIElement = UIKit.UIView;
#elif __ANDROID__
    using Android.Content;
    using View = Android.Views.View;
#endif
namespace SharpConstraintLayout.Maui.Helper.Widget
{
    public interface ICircularFlow
    {
        //JniPeerMembers JniPeerMembers { get; }

        void AddViewToCircularFlow(View view, int radius, float angle);
        float[] GetAngles();
        int[] GetRadius();
        bool IsUpdatable(View view);
        void SetDefaultAngle(float angle);
        void SetDefaultRadius(int radius);
        void UpdateAngle(View view, float angle);
        void UpdateRadius(View view, int radius);
        void UpdateReference(View view, int radius, float angle);
    }
}
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
namespace SharpConstraintLayout.Maui.Widget
{
    public interface IPlaceholder
    {
        View Content { get; }
        int EmptyVisibility { get; set; }
        //JniPeerMembers JniPeerMembers { get; }

        //void OnDraw(Canvas canvas);
        void SetContentId(int id);
        void UpdatePostMeasure(ConstraintLayout container);
        void UpdatePreLayout(ConstraintLayout container);
    }
}
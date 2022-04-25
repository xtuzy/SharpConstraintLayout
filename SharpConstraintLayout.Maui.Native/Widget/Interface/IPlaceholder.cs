#if __MAUI__
using FrameworkElement = Microsoft.Maui.Controls.View;
using UIElement = Microsoft.Maui.Controls.View;
#elif WINDOWS
using View = Microsoft.UI.Xaml.FrameworkElement;
using UIElement = Microsoft.UI.Xaml.UIElement;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
using SharpConstraintLayout;
using SharpConstraintLayout.Maui;
using SharpConstraintLayout.Maui.Widget;
using SharpConstraintLayout.Maui.Widget.Interface;
#elif __IOS__
using View = UIKit.UIView;
using UIElement = UIKit.UIView;
using SharpConstraintLayout;
using SharpConstraintLayout.Maui;
using SharpConstraintLayout.Maui.Widget;
using SharpConstraintLayout.Maui.Widget.Interface;
#elif __ANDROID__
using Android.Content;
using SharpConstraintLayout;
using SharpConstraintLayout.Maui;
using SharpConstraintLayout.Maui.Widget;
using SharpConstraintLayout.Maui.Widget.Interface;
using View = Android.Views.View;
#endif
namespace SharpConstraintLayout.Maui.Widget.Interface
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
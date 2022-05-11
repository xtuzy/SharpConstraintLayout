
using androidx.constraintlayout.core.widgets;
#if __MAUI__
using View = Microsoft.Maui.Controls.View;
using Size = Microsoft.Maui.Graphics.Size;
#elif WINDOWS 
using View = Microsoft.UI.Xaml.UIElement;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
using Size = Windows.Foundation.Size;
#elif __IOS__
using CoreGraphics;
using SharpConstraintLayout;
using SharpConstraintLayout.Maui;
using SharpConstraintLayout.Maui.Widget;
using SharpConstraintLayout.Maui.Widget.Interface;
using View = UIKit.UIView;
using Size = CoreGraphics.CGSize;
#elif __ANDROID__
using View = Android.Views.View;
using Size = Microsoft.Maui.Graphics.Size;
#endif
namespace SharpConstraintLayout.Maui.Widget.Interface
{
    public interface IConstraintLayout
    {
        //JniPeerMembers JniPeerMembers { get; }
        //int MaxHeight { get; set; }
        //int MaxWidth { get; set; }
        //int MinHeight { get; set; }
        //int MinWidth { get; set; }
        int OptimizationLevel { get; set; }

        //void FillMetrics(Metrics metrics);
        //Object GetDesignInformation(int type, Object value);
        View FindElementById(int id);
        ConstraintWidget GetWidgetByElement(View view);
        //void LoadLayoutDescription(int layoutDescription);
        //void SetConstraintSet(ConstraintSet set);
        //void SetDesignInformation(int type, Object value1, Object value2);
        //void SetOnConstraintsChanged(ConstraintsChangedListener constraintsChangedListener);
        //void SetState(int id, int screenWidth, int screenHeight);//?

        (bool isInfinityAvailabelWidth, bool isInfinityAvailabelHeight) IsInfinitable(ConstraintLayout layout, int constrainWidth, int constrainHeight, Size availableSize);
    }
}
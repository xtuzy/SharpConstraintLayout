
using androidx.constraintlayout.core.widgets;
#if WINDOWS
    using View = Microsoft.UI.Xaml.UIElement;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Windows.Foundation;
#elif __IOS__
using View = UIKit.UIView;
#endif
namespace SharpConstraintLayout.Maui.Widget
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
        View GetViewById(int id);
        ConstraintWidget GetViewWidget(View view);
        //void LoadLayoutDescription(int layoutDescription);
        //void SetConstraintSet(ConstraintSet set);
        //void SetDesignInformation(int type, Object value1, Object value2);
        //void SetOnConstraintsChanged(ConstraintsChangedListener constraintsChangedListener);
        //void SetState(int id, int screenWidth, int screenHeight);//?
    }
}
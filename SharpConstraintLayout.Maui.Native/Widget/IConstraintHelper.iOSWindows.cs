
using androidx.constraintlayout.core.widgets;
using SharpConstraintLayout.Maui.Widget;
using System;
using System.Collections.Generic;
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
    using Helper = androidx.constraintlayout.core.widgets.Helper;
    public interface IConstraintHelper
    {
        //JniPeerMembers JniPeerMembers { get; }

        void AddView(View view);
        bool ContainsId(int id);
        //int[] GetReferencedIds();
        int IndexFromId(int id);
        //void LoadParameters(ConstraintSet.Constraint constraint, HelperWidget child, ConstraintLayout.LayoutParams layoutParams ,Dictionary<int,ConstraintWidget> mapIdToWidget);
        void LoadParameters(ConstraintSet.Constraint constraint, HelperWidget child, Dictionary<int, ConstraintWidget> mapIdToWidget);
        //void OnDraw(Canvas canvas);
        int RemoveView(View view);
        void ResolveRtl(ConstraintWidget widget, bool isRtl);
        //void SetReferencedIds(int[] ids);
        int[] ReferencedIds { set; get; }
        void UpdatePostConstraints(ConstraintLayout container);
        void UpdatePostLayout(ConstraintLayout container);
        void UpdatePostMeasure(ConstraintLayout container);
        void UpdatePreDraw(ConstraintLayout container);
        void UpdatePreLayout(ConstraintLayout container);
        void UpdatePreLayout(ConstraintWidgetContainer container, Helper helper, Dictionary<int, ConstraintWidget> map);
        //void ValidateParams();
        void ValidateParams(Dictionary<int, ConstraintWidget> idsToConstraintWidgets = null);

        void OnAttachedToWindow();
    }
}
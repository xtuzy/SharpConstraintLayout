using System.Collections.Generic;
using System.Xml;
using static SharpConstraintLayout.Maui.Widget.FluentConstraintSet;
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
using AndroidX.ConstraintLayout.Widget;
#endif
using Element = SharpConstraintLayout.Maui.Widget.FluentConstraintSet.Element;
namespace SharpConstraintLayout.Maui.Widget.Interface
{
    public interface IElement
    {
        Element AddToXChain(View leftId, View rightId);
        Element AddToXChainRTL(View leftId, View rightId);
        Element AddToYChain(View topId, View bottomId);

        Element CenterTo(View firstID, int firstSide, int firstMargin, View secondId, int secondSide, int secondMargin, float bias);
        Element CenterXTo(View toView);
        Element CenterXTo(View leftId, int leftSide, int leftMargin, View rightId, int rightSide, int rightMargin, float bias);
        Element CenterXRtlTo(View toView);
        Element CenterXRtlTO(View startId, int startSide, int startMargin, View endId, int endSide, int endMargin, float bias);
        Element CenterYTo(View toView);
        Element CenterYTo(View topId, int topSide, int topMargin, View bottomId, int bottomSide, int bottomMargin, float bias);
        FluentConstraintSet Clear();
        FluentConstraintSet Clear(int anchor);

        Element Connect(int startSide, View endID, int endSide);
        Element Connect(int startSide, View endID, int endSide, int margin);
        Element ConstrainCircle(View id, int radius, float angle);
        Element DefaultHeight(int height);
        Element DefaultWidth(int width);
        Element ConstrainedHeight(bool constrained);
        Element ConstrainedWidth(bool constrained);
        Element Height(int height);
        Element MaxHeight(int height);
        Element MaxWidth(int width);
        Element MinHeight(int height);
        Element MinWidth(int width);
        Element PercentHeight(float percent);
        Element PercentWidth(float percent);
        Element Width(int width);
        Element CreateBarrier(int direction, int margin, params int[] referenced);
        Element CreateHorizontalChain(View leftId, int leftSide, View rightId, int rightSide, int[] chainIds, float[] weights, int style);
        Element CreateHorizontalChainRtl(View startId, int startSide, View endId, int endSide, int[] chainIds, float[] weights, int style);
        Element CreateVerticalChain(View topId, int topSide, View bottomId, int bottomSide, int[] chainIds, float[] weights, int style);

        Element RemoveFromHorizontalChain(View viewId);
        Element RemoveFromVerticalChain(View viewId);
        Element Alpha(float alpha);
        Element ApplyElevation(bool apply);
        Element BarrierType(int type);
        Element DimensionRatio(string ratio);
        Element EditorAbsoluteX(int position);
        Element EditorAbsoluteY(int position);
        Element Elevation(float elevation);

        Element GoneMargin(int anchor, int value);
        Element GuidelineBegin(int margin);
        Element GuidelineEnd(int margin);
        Element GuidelinePercent(float ratio);
        Element XBias(float bias);
        Element XChainStyle(int chainStyle);
        Element XWeight(float weight);
        Element LayoutWrapBehavior(int behavior);
        Element Margin(int anchor, int value);
        Element ReferencedIds(params int[] referenced);
        Element Rotation(float rotation);
        Element RotationX(float rotationX);
        Element RotationY(float rotationY);
        Element ScaleX(float scaleX);
        Element ScaleY(float scaleY);

        Element TransformPivot(float transformPivotX, float transformPivotY);
        Element TransformPivotX(float transformPivotX);
        Element TransformPivotY(float transformPivotY);
        Element Translation(float translationX, float translationY);
        Element TranslationX(float translationX);
        Element TranslationY(float translationY);
        Element TranslationZ(float translationZ);
        Element VerticalBias(float bias);
        Element VerticalChainStyle(int chainStyle);
        Element VerticalWeight(float weight);
        Element Visibility(int visibility);
        Element VisibilityMode(int visibilityMode);
    }
}
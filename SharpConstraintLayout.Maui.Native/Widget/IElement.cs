using System.Collections.Generic;
using System.Xml;
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
namespace SharpConstraintLayout.Maui.Widget
{
    public interface IElement
    {
        void AddToXChain(View leftId, View rightId);
        void AddToXChainRTL(View leftId, View rightId);
        void AddToYChain(View topId, View bottomId);

        void CenterTo(View centerID, View firstID, int firstSide, int firstMargin, View secondId, int secondSide, int secondMargin, float bias);
        void CenterXTo(View toView);
        void CenterXTo(View centerID, View leftId, int leftSide, int leftMargin, View rightId, int rightSide, int rightMargin, float bias);
        void CenterXRtlTo(View toView);
        void CenterXRtlTO(View centerID, View startId, int startSide, int startMargin, View endId, int endSide, int endMargin, float bias);
        void CenterYTo(View toView);
        void CenterYTo(View centerID, View topId, int topSide, int topMargin, View bottomId, int bottomSide, int bottomMargin, float bias);
        void Clear();
        void Clear(int anchor);

        void Connect(View startID, int startSide, View endID, int endSide);
        void Connect(View startID, int startSide, View endID, int endSide, int margin);
        void ConstrainCircle(View id, int radius, float angle);
        void DefaultHeight(int height);
        void DefaultWidth(int width);
        void ConstrainedHeight(bool constrained);
        void ConstrainedWidth(bool constrained);
        void Height(int height);
        void MaxHeight(int height);
        void MaxWidth(int width);
        void MinHeight(int height);
        void MinWidth(int width);
        void PercentHeight(float percent);
        void PercentWidth(float percent);
        void Width(int width);
        void Create(View guidelineID, int orientation);
        void CreateBarrier(View id, int direction, int margin, params int[] referenced);
        void CreateHorizontalChain(View leftId, int leftSide, View rightId, int rightSide, int[] chainIds, float[] weights, int style);
        void CreateHorizontalChainRtl(View startId, int startSide, View endId, int endSide, int[] chainIds, float[] weights, int style);
        void CreateVerticalChain(View topId, int topSide, View bottomId, int bottomSide, int[] chainIds, float[] weights, int style);

        void RemoveFromHorizontalChain(View viewId);
        void RemoveFromVerticalChain(View viewId);
        void Alpha(float alpha);
        void ApplyElevation(bool apply);
        void BarrierType(View id, int type);
        void DimensionRatio(string ratio);
        void EditorAbsoluteX(int position);
        void EditorAbsoluteY(int position);
        void Elevation(float elevation);

        void GoneMargin(int anchor, int value);
        void GuidelineBegin(View guidelineID, int margin);
        void GuidelineEnd(View guidelineID, int margin);
        void GuidelinePercent(View guidelineID, float ratio);
        void XBias(float bias);
        void XChainStyle(int chainStyle);
        void XWeight(float weight);
        void LayoutWrapBehavior(int behavior);
        void Margin(int anchor, int value);
        void ReferencedIds(View id, params int[] referenced);
        void Rotation(float rotation);
        void RotationX(float rotationX);
        void RotationY(float rotationY);
        void ScaleX(float scaleX);
        void ScaleY(float scaleY);

        void TransformPivot(float transformPivotX, float transformPivotY);
        void TransformPivotX(float transformPivotX);
        void TransformPivotY(float transformPivotY);
        void Translation(float translationX, float translationY);
        void TranslationX(float translationX);
        void TranslationY(float translationY);
        void TranslationZ(float translationZ);
        void VerticalBias(float bias);
        void VerticalChainStyle(int chainStyle);
        void VerticalWeight(float weight);
        void Visibility(int visibility);
        void VisibilityMode(int visibilityMode);
    }
}
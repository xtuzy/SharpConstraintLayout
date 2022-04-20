using androidx.constraintlayout.core.widgets;
using System.Collections.Generic;
using System.Xml;

namespace SharpConstraintLayout.Maui.Widget
{
    public interface IConstraintSet
    {
        //IDictionary<string, ConstraintAttribute> CustomAttributeSet { get; }
        //string DerivedState { get; set; }
        //bool ForceId { get; set; }
        //string MIdString { get; set; }
        //int MRotate { get; set; }

        //void AddColorAttributes(params string[] attributeName);
        //void AddFloatAttributes(params string[] attributeName);
        //void AddIntAttributes(params string[] attributeName);
        //void AddStringAttributes(params string[] attributeName);
        void AddToHorizontalChain(int viewId, int leftId, int rightId);
        void AddToHorizontalChainRTL(int viewId, int leftId, int rightId);
        void AddToVerticalChain(int viewId, int topId, int bottomId);
        //void ApplyCustomAttributes(ConstraintLayout constraintLayout);
        //void ApplyDeltaFrom(ConstraintSet cs);
        void ApplyTo(ConstraintLayout constraintLayout);
        //void ApplyToHelper(ConstraintHelper helper, ConstraintWidget child, ConstraintSet.Constraint layoutParams, Dictionary<int,ConstraintWidget> mapIdToWidget);
        //void ApplyToLayoutParams(int id, ConstraintLayout.LayoutParams layoutParams);
        //void ApplyToWithoutCustom(ConstraintLayout constraintLayout);
        void Center(int centerID, int firstID, int firstSide, int firstMargin, int secondId, int secondSide, int secondMargin, float bias);
        void CenterHorizontally(int viewId, int toView);
        void CenterHorizontally(int centerID, int leftId, int leftSide, int leftMargin, int rightId, int rightSide, int rightMargin, float bias);
        void CenterHorizontallyRtl(int viewId, int toView);
        void CenterHorizontallyRtl(int centerID, int startId, int startSide, int startMargin, int endId, int endSide, int endMargin, float bias);
        void CenterVertically(int viewId, int toView);
        void CenterVertically(int centerID, int topId, int topSide, int topMargin, int bottomId, int bottomSide, int bottomMargin, float bias);
        void Clear(int viewId);
        void Clear(int viewId, int anchor);
        void Clone(ConstraintLayout constraintLayout);
        //void Clone(Constraints constraints);
        //void Clone(ConstraintSet set);
        //void Clone(Context context, int constraintLayoutId);
        void Connect(int startID, int startSide, int endID, int endSide);
        void Connect(int startID, int startSide, int endID, int endSide, int margin);
        void ConstrainCircle(int viewId, int id, int radius, float angle);
        void ConstrainDefaultHeight(int viewId, int height);
        void ConstrainDefaultWidth(int viewId, int width);
        void ConstrainedHeight(int viewId, bool constrained);
        void ConstrainedWidth(int viewId, bool constrained);
        void ConstrainHeight(int viewId, int height);
        void ConstrainMaxHeight(int viewId, int height);
        void ConstrainMaxWidth(int viewId, int width);
        void ConstrainMinHeight(int viewId, int height);
        void ConstrainMinWidth(int viewId, int width);
        void ConstrainPercentHeight(int viewId, float percent);
        void ConstrainPercentWidth(int viewId, float percent);
        void ConstrainWidth(int viewId, int width);
        void Create(int guidelineID, int orientation);
        void CreateBarrier(int id, int direction, int margin, params int[] referenced);
        void CreateHorizontalChain(int leftId, int leftSide, int rightId, int rightSide, int[] chainIds, float[] weights, int style);
        void CreateHorizontalChainRtl(int startId, int startSide, int endId, int endSide, int[] chainIds, float[] weights, int style);
        void CreateVerticalChain(int topId, int topSide, int bottomId, int bottomSide, int[] chainIds, float[] weights, int style);
        //void Dump(MotionScene scene, params int[] ids);
        bool GetApplyElevation(int viewId);
        ConstraintSet.Constraint GetConstraint(int id);
        int GetHeight(int viewId);
        //int[] GetKnownIds();
        ConstraintSet.Constraint GetParameters(int mId);
        int[] GetReferencedIds(int id);
        int GetVisibility(int viewId);
        int GetVisibilityMode(int viewId);
        int GetWidth(int viewId);
        //void Load(Context context, int resourceId);
        //void Load(Context context, XmlReader parser);
        //void ParseColorAttributes(ConstraintSet.Constraint set, string attributes);
        //void ParseFloatAttributes(ConstraintSet.Constraint set, string attributes);
        //void ParseIntAttributes(ConstraintSet.Constraint set, string attributes);
        //void ParseStringAttributes(ConstraintSet.Constraint set, string attributes);
        //void ReadFallback(ConstraintLayout constraintLayout);
        //void ReadFallback(ConstraintSet set);
        //void RemoveAttribute(string attributeName);
        void RemoveFromHorizontalChain(int viewId);
        void RemoveFromVerticalChain(int viewId);
        void SetAlpha(int viewId, float alpha);
        void SetApplyElevation(int viewId, bool apply);
        void SetBarrierType(int id, int type);
        //void SetColorValue(int viewId, string attributeName, int value);
        void SetDimensionRatio(int viewId, string ratio);
        void SetEditorAbsoluteX(int viewId, int position);
        void SetEditorAbsoluteY(int viewId, int position);
        void SetElevation(int viewId, float elevation);
        //void SetFloatValue(int viewId, string attributeName, float value);
        void SetGoneMargin(int viewId, int anchor, int value);
        void SetGuidelineBegin(int guidelineID, int margin);
        void SetGuidelineEnd(int guidelineID, int margin);
        void SetGuidelinePercent(int guidelineID, float ratio);
        void SetHorizontalBias(int viewId, float bias);
        void SetHorizontalChainStyle(int viewId, int chainStyle);
        void SetHorizontalWeight(int viewId, float weight);
        //void SetIntValue(int viewId, string attributeName, int value);
        void SetLayoutWrapBehavior(int viewId, int behavior);
        void SetMargin(int viewId, int anchor, int value);
        void SetReferencedIds(int id, params int[] referenced);
        void SetRotation(int viewId, float rotation);
        void SetRotationX(int viewId, float rotationX);
        void SetRotationY(int viewId, float rotationY);
        void SetScaleX(int viewId, float scaleX);
        void SetScaleY(int viewId, float scaleY);
        //void SetStringValue(int viewId, string attributeName, string value);
        void SetTransformPivot(int viewId, float transformPivotX, float transformPivotY);
        void SetTransformPivotX(int viewId, float transformPivotX);
        void SetTransformPivotY(int viewId, float transformPivotY);
        void SetTranslation(int viewId, float translationX, float translationY);
        void SetTranslationX(int viewId, float translationX);
        void SetTranslationY(int viewId, float translationY);
        void SetTranslationZ(int viewId, float translationZ);
        //void SetValidateOnParse(bool validate);
        void SetVerticalBias(int viewId, float bias);
        void SetVerticalChainStyle(int viewId, int chainStyle);
        void SetVerticalWeight(int viewId, float weight);
        void SetVisibility(int viewId, int visibility);
        void SetVisibilityMode(int viewId, int visibilityMode);
        //void WriteState(Writer writer, ConstraintLayout layout, int flags);
    }
}
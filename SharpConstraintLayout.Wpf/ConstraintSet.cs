using androidx.constraintlayout.core.widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SharpConstraintLayout.Wpf
{
    
    public class ConstraintSet
    {
        /// <summary>
        /// Text orientation
        /// </summary>
        public enum TextOrientation
        {
            LeftToRight,
            RightToLeft
        }

        public enum Visibility
        {
            /*public const int VISIBLE = 0;
            public const int INVISIBLE = 4;
            public const int GONE = 8;*/
            Visible = 0,
            Invisible = 4,
            Gone = 8,
        }

        public enum SizeType
        {
            /*FIXED,
            WRAP_CONTENT,
            MATCH_CONSTRAINT,
            MATCH_PARENT*/
            Fixed,
            Wrap_Content,
            Match_Constraint,
            Match_Parent,
        }

        public enum LayoutStyle
        {
            /*public const int CHAIN_SPREAD = 0;
            public const int CHAIN_SPREAD_INSIDE = 1;
            public const int CHAIN_PACKED = 2;*/
            /// <summary>
            /// default style
            /// </summary>
            Chain_Spread,
            Chain_Spread_Inside,
            Chain_Packed
        }


        WeakReference<ConstraintLayout> Parent;//一个ConstraintSet基本只使用一次,需要时重新创建,所以直接弱引用避免内存泄漏

        public ConstraintSet(ConstraintLayout parent)
        {
            Parent = new WeakReference<ConstraintLayout>(parent);
        }

        public ConstraintSet AddConnect(UIElement fromView, ConstraintAnchor.Type fromSide, UIElement toView, ConstraintAnchor.Type toSide, int margin)
        {
            Parent.TryGetTarget(out var parent);
            if (parent == null) throw new NotImplementedException();
            var fromWidget = parent.GetWidget(fromView);
            var toWidget = parent.GetWidget(toView);
            fromWidget.connect(fromSide, toWidget, toSide, margin);
            return this;
        }

        public ConstraintSet AddConnect(UIElement view, ConstraintAnchor.Type firstSide, UIElement secondView, ConstraintAnchor.Type secondSide)
        {
            return AddConnect(view, firstSide, secondView, secondSide, 0);
        }

        /// <summary>
        /// Set view's width according to <br/>
        /// Constraint <see cref="ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT"></see> <br/>
        /// or Parent <see cref="ConstraintWidget.DimensionBehaviour.MATCH_PARENT"></see><br/>
        /// or FrameworkElement.Width <see cref="ConstraintWidget.DimensionBehaviour.WRAP_CONTENT"></see><br/>
        /// </summary>
        /// <param name="view"></param>
        /// <param name="behaviour">Set view's width according to Constraint or Parent or FrameworkElement.Width</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public ConstraintSet SetWidth(UIElement view, ConstraintWidget.DimensionBehaviour behaviour)
        {
            Parent.TryGetTarget(out var parent);
            if (parent == null) throw new NotImplementedException();
            var viewWidget = parent.GetWidget(view);
            viewWidget.HorizontalDimensionBehaviour = behaviour;

            return this;
        }

        public ConstraintSet SetWidth(UIElement view, int minWidth)
        {
            Parent.TryGetTarget(out var parent);
            if (parent == null) throw new NotImplementedException();
            var viewWidget = parent.GetWidget(view);
            viewWidget.MinWidth = minWidth;
            return this;
        }

        /// <summary>
        /// Set view's width according to <br/>
        /// Constraint <see cref="ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT"></see> <br/>
        /// or Parent <see cref="ConstraintWidget.DimensionBehaviour.MATCH_PARENT"></see><br/>
        /// or FrameworkElement.Width <see cref="ConstraintWidget.DimensionBehaviour.WRAP_CONTENT"></see><br/>
        /// </summary>
        /// <param name="view"></param>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public ConstraintSet SetHeight(UIElement view, ConstraintWidget.DimensionBehaviour behaviour)
        {
            Parent.TryGetTarget(out var parent);
            if (parent == null) throw new NotImplementedException();
            var viewWidget = parent.GetWidget(view);
            viewWidget.VerticalDimensionBehaviour = behaviour;

            //TODO:如果嵌套了ConstraintLayout,需要对子ConstraintLayout的Container也进行同步设置.
            //子ConstraintLayout一般需要指定宽高才能布局,
            //如果是Match_Parent,传递给子ConstraintLayout的应该是具体值,
            //如果是Warp_Content,那么应该指定子ConstraintLayout的Container为Match_Constraint
            //如果是Match_Constraint,那么父ConstraintLayout应该计算出具体值传递

            return this;
        }

        public ConstraintSet SetHeight(UIElement view, int minHeight)
        {
            Parent.TryGetTarget(out var parent);
            if (parent == null) throw new NotImplementedException();
            var viewWidget = parent.GetWidget(view);
            viewWidget.MinHeight = minHeight;
            return this;
        }

        /// <summary>
        /// 去掉某一个View的约束
        /// </summary>
        /// <param name="view"></param>
        public void Clear(UIElement view)
        {
            Parent.TryGetTarget(out var parent);
            if (parent == null) throw new NotImplementedException();
            var viewWidget = parent.GetWidget(view);
            viewWidget.resetAllConstraints();
        }
    }

    #region Better Api
    public static class ConstraintSetExtensions
    {

        #region Position

        /// <summary>
        /// fromView's center = toView's center
        /// </summary>
        /// <param name="fromView"></param>
        /// <param name="toView"></param>
        /// <param name="margin"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Parent of fromView is not ConstraintLayout</exception>
        public static FrameworkElement Center(this FrameworkElement fromView, FrameworkElement toView, int margin = 0)
        {
            var parent = fromView.Parent is ConstraintLayout ? fromView.Parent as ConstraintLayout : throw new ArgumentException($"Parent of {fromView} is not ConstraintLayout");
            var fromWidget = parent.GetWidget(fromView);
            var toWidget = parent.GetWidget(toView);
            fromWidget.connect(ConstraintAnchor.Type.CENTER, toWidget, ConstraintAnchor.Type.CENTER, margin);
            return fromView;
        }

        /// <summary>
        /// at X, fromView's center = toView's center
        /// </summary>
        /// <param name="fromView"></param>
        /// <param name="toView"></param>
        /// <param name="margin"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Parent of fromView is not ConstraintLayout</exception>
        public static FrameworkElement CenterX(this FrameworkElement fromView, FrameworkElement toView)
        {
            var parent = fromView.Parent is ConstraintLayout ? fromView.Parent as ConstraintLayout : throw new ArgumentException($"Parent of {fromView} is not ConstraintLayout");
            var fromWidget = parent.GetWidget(fromView);
            var toWidget = parent.GetWidget(toView);
            fromWidget.connect(ConstraintAnchor.Type.CENTER_X, toWidget, ConstraintAnchor.Type.CENTER_X, 0);
            return fromView;
        }

        /// <summary>
        /// at Y, fromView's center = toView's center
        /// </summary>
        /// <param name="fromView"></param>
        /// <param name="toView"></param>
        /// <param name="margin"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Parent of fromView is not ConstraintLayout</exception>
        public static FrameworkElement CenterY(this FrameworkElement fromView, FrameworkElement toView)
        {
            var parent = fromView.Parent is ConstraintLayout ? fromView.Parent as ConstraintLayout : throw new ArgumentException($"Parent of {fromView} is not ConstraintLayout");
            var fromWidget = parent.GetWidget(fromView);
            var toWidget = parent.GetWidget(toView);
            fromWidget.connect(ConstraintAnchor.Type.CENTER_Y, toWidget, ConstraintAnchor.Type.CENTER_Y, 0);
            return fromView;
        }
        /// <summary>
        /// fromView left side position relate to toview left side
        /// </summary>
        /// <param name="fromView"></param>
        /// <param name="toView"></param>
        /// <param name="margin">LeftToX, At toView Left is negative</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Parent of fromView is not ConstraintLayout</exception>
        public static FrameworkElement LeftToLeft(this FrameworkElement fromView, FrameworkElement toView, int margin = 0)
        {
            var parent = fromView.Parent is ConstraintLayout ? fromView.Parent as ConstraintLayout : throw new ArgumentException($"Parent of {fromView} is not ConstraintLayout");
            var fromWidget = parent.GetWidget(fromView);
            var toWidget = parent.GetWidget(toView);
            fromWidget.connect(ConstraintAnchor.Type.LEFT, toWidget, ConstraintAnchor.Type.LEFT, margin);
            return fromView;
        }

        /// <summary>
        /// fromView left side position relate to toview right side
        /// </summary>
        /// <param name="fromView"></param>
        /// <param name="toView"></param>
        /// <param name="margin">LeftToX, At toView Left is negative</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Parent of fromView is not ConstraintLayout</exception>
        public static FrameworkElement LeftToRight(this FrameworkElement fromView, FrameworkElement toView, int margin = 0)
        {
            var parent = fromView.Parent is ConstraintLayout ? fromView.Parent as ConstraintLayout : throw new ArgumentException($"Parent of {fromView} is not ConstraintLayout");
            var fromWidget = parent.GetWidget(fromView);
            var toWidget = parent.GetWidget(toView);
            fromWidget.connect(ConstraintAnchor.Type.LEFT, toWidget, ConstraintAnchor.Type.RIGHT, margin);
            return fromView;
        }
        /// <summary>
        /// fromView right side position relate to toview left side
        /// </summary>
        /// <param name="fromView"></param>
        /// <param name="toView"></param>
        /// <param name="margin">RightToX, At toView Right is negative</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Parent of fromView is not ConstraintLayout</exception>
        public static FrameworkElement RightToLeft(this FrameworkElement fromView, FrameworkElement toView, int margin = 0)
        {
            var parent = fromView.Parent is ConstraintLayout ? fromView.Parent as ConstraintLayout : throw new ArgumentException($"Parent of {fromView} is not ConstraintLayout");
            var fromWidget = parent.GetWidget(fromView);
            var toWidget = parent.GetWidget(toView);
            fromWidget.connect(ConstraintAnchor.Type.RIGHT, toWidget, ConstraintAnchor.Type.LEFT, margin);
            return fromView;
        }
        /// <summary>
        /// fromView right side position relate to toview right side
        /// </summary>
        /// <param name="fromView"></param>
        /// <param name="toView"></param>
        /// <param name="margin">RightToX, At toView Right is negative</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Parent of fromView is not ConstraintLayout</exception>
        public static FrameworkElement RightToRight(this FrameworkElement fromView, FrameworkElement toView, int margin = 0)
        {
            var parent = fromView.Parent is ConstraintLayout ? fromView.Parent as ConstraintLayout : throw new ArgumentException($"Parent of {fromView} is not ConstraintLayout");
            var fromWidget = parent.GetWidget(fromView);
            var toWidget = parent.GetWidget(toView);
            fromWidget.connect(ConstraintAnchor.Type.RIGHT, toWidget, ConstraintAnchor.Type.RIGHT, margin);
            return fromView;
        }

        /// <summary>
        /// fromView top side position relate to toview top side
        /// </summary>
        /// <param name="fromView"></param>
        /// <param name="toView"></param>
        /// <param name="margin">TopToX, At toView Top is negative</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Parent of fromView is not ConstraintLayout</exception>
        public static FrameworkElement TopToTop(this FrameworkElement fromView, FrameworkElement toView, int margin = 0)
        {
            var parent = fromView.Parent is ConstraintLayout ? fromView.Parent as ConstraintLayout : throw new ArgumentException($"Parent of {fromView} is not ConstraintLayout");
            var fromWidget = parent.GetWidget(fromView);
            var toWidget = parent.GetWidget(toView);
            fromWidget.connect(ConstraintAnchor.Type.TOP, toWidget, ConstraintAnchor.Type.TOP, margin);
            return fromView;
        }
        /// <summary>
        /// fromView top side position relate to toview bottom side
        /// </summary>
        /// <param name="fromView"></param>
        /// <param name="toView"></param>
        /// <param name="margin">TopToX, At toView Top is negative</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Parent of fromView is not ConstraintLayout</exception>
        public static FrameworkElement TopToBottom(this FrameworkElement fromView, FrameworkElement toView, int margin = 0)
        {
            var parent = fromView.Parent is ConstraintLayout ? fromView.Parent as ConstraintLayout : throw new ArgumentException($"Parent of {fromView} is not ConstraintLayout");
            var fromWidget = parent.GetWidget(fromView);
            var toWidget = parent.GetWidget(toView);
            fromWidget.connect(ConstraintAnchor.Type.TOP, toWidget, ConstraintAnchor.Type.BOTTOM, margin);
            return fromView;
        }

        /// <summary>
        /// fromView bottom side position relate to toview top side
        /// </summary>
        /// <param name="fromView"></param>
        /// <param name="toView"></param>
        /// <param name="margin">BottomToX, At toView Bottom is negative</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Parent of fromView is not ConstraintLayout</exception>
        public static FrameworkElement BottomToTop(this FrameworkElement fromView, FrameworkElement toView, int margin = 0)
        {
            var parent = fromView.Parent is ConstraintLayout ? fromView.Parent as ConstraintLayout : throw new ArgumentException($"Parent of {fromView} is not ConstraintLayout");
            var fromWidget = parent.GetWidget(fromView);
            var toWidget = parent.GetWidget(toView);
            fromWidget.connect(ConstraintAnchor.Type.BOTTOM, toWidget, ConstraintAnchor.Type.TOP, margin);
            return fromView;
        }
        /// <summary>
        /// fromView bottom side position relate to toview bottom side
        /// </summary>
        /// <param name="fromView"></param>
        /// <param name="toView"></param>
        /// <param name="margin">BottomToX, At toView Bottom is negative</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Parent of fromView is not ConstraintLayout</exception>
        public static FrameworkElement BottomToBottom(this FrameworkElement fromView, FrameworkElement toView, int margin = 0)
        {
            var parent = fromView.Parent is ConstraintLayout ? fromView.Parent as ConstraintLayout : throw new ArgumentException($"Parent of {fromView} is not ConstraintLayout");
            var fromWidget = parent.GetWidget(fromView);
            var toWidget = parent.GetWidget(toView);
            fromWidget.connect(ConstraintAnchor.Type.BOTTOM, toWidget, ConstraintAnchor.Type.BOTTOM, margin);
            return fromView;
        }

        /// <summary>
        /// When views' constraint in horizontal make a chain, this can arrange views regularly.<br/>
        /// Notice:<br/>
        /// 1.fromView need as the first View in the chain.otherwise this style may not work.
        /// </summary>
        /// <param name="fromView"></param>
        /// <param name="layoutStyle"><see href="https://constraintlayout.com/basics/create_chains.html">ChainStyle</see></param>
        /// <param name="bias">0~1,only when set <see cref="LayoutStyle.Chain_Packed"/>,it is effective</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Parent of fromView is not ConstraintLayout</exception>
        public static FrameworkElement SetHorizontalLayoutStyle(this FrameworkElement fromView, ConstraintSet.LayoutStyle layoutStyle, float bias = float.NaN)
        {
            var parent = fromView.Parent is ConstraintLayout ? fromView.Parent as ConstraintLayout : throw new ArgumentException($"Parent of {fromView} is not ConstraintLayout");
            var fromWidget = parent.GetWidget(fromView);
            fromWidget.HorizontalChainStyle = (int)layoutStyle;
            if (!float.IsNaN(bias))
                fromWidget.HorizontalBiasPercent = bias;
            return fromView;
        }

        /// <summary>
        /// When views' constraint in vertical make a chain, this can arrange views regularly.<br/>
        /// Notice:<br/>
        /// 1.fromView need as the first View in the chain.otherwise this style may not work.
        /// </summary>
        /// <param name="fromView"></param>
        /// <param name="layoutStyle"><see href="https://constraintlayout.com/basics/create_chains.html">ChainStyle</see></param>
        /// <param name="bias">0~1,only when set <see cref="LayoutStyle.Chain_Packed"/>,it is effective</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Parent of fromView is not ConstraintLayout</exception>
        public static FrameworkElement SetVerticalLayoutStyle(this FrameworkElement fromView, ConstraintSet.LayoutStyle layoutStyle, float bias = float.NaN)
        {
            var parent = fromView.Parent is ConstraintLayout ? fromView.Parent as ConstraintLayout : throw new ArgumentException($"Parent of {fromView} is not ConstraintLayout");
            var fromWidget = parent.GetWidget(fromView);
            fromWidget.VerticalChainStyle = (int)layoutStyle;
            if (!float.IsNaN(bias))
                fromWidget.VerticalBiasPercent = bias;
            return fromView;
        }

        /// <summary>
        /// Set a circular constraint
        /// </summary>
        /// <param name="fromView"></param>
        /// <param name="toView"> the target view we will use as the center of the circle</param>
        /// <param name="angle">  the angle (from 0 to 360) </param>
        /// <param name="radius"> the radius used </param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Parent of fromView is not ConstraintLayout</exception>
        public static FrameworkElement CenterToCircle(this FrameworkElement fromView, FrameworkElement toView, float angle, int radius)
        {
            var parent = fromView.Parent is ConstraintLayout ? fromView.Parent as ConstraintLayout : throw new ArgumentException($"Parent of {fromView} is not ConstraintLayout");
            var fromWidget = parent.GetWidget(fromView);
            var toWidget = parent.GetWidget(toView);
            fromWidget.connectCircularConstraint(toWidget, angle, radius);
            return fromView;
        }

        #endregion Position


        #region Size

        /// <summary>
        /// create constraint for child width.<br/>
        /// Notice:<br/>
        /// 1.if you set <see cref="SizeType.Match_Parent"/> or <see cref="SizeType.Match_Constraint"/>,must not set <see cref="FrameworkElement.Width"/>,they are conflict.<br/>
        /// 2.if you set <see cref="LayoutStyle"/> is <see cref="LayoutStyle.Chain_Packed"/>,must not set weight,they are conflict.
        /// </summary>
        /// <param name="fromView"></param>
        /// <param name="constraint"></param>
        /// <param name="weight">fromView's weight at horizontal when multiple view's constraint is a chain</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Parent of fromView is not ConstraintLayout</exception>
        public static FrameworkElement SetWidth(this FrameworkElement fromView, ConstraintSet.SizeType constraint, float weight = float.NaN)
        {
            var parent = fromView.Parent is ConstraintLayout ? fromView.Parent as ConstraintLayout : throw new ArgumentException($"Parent of {fromView} is not ConstraintLayout");
            var fromWidget = parent.GetWidget(fromView);
            fromWidget.HorizontalDimensionBehaviour = ConstraintSizeTypeDic[(int)constraint];
            if (!float.IsNaN(weight))
            {
                fromWidget.HorizontalWeight = weight;
            }
            return fromView;
        }
        /// <summary>
        /// Set width of ConstraintLayout self.
        /// </summary>
        /// <param name="fromView"></param>
        /// <param name="constraint"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static FrameworkElement SetWidth(this ConstraintLayout fromView, ConstraintSet.SizeType constraint)
        {
            var fromWidget = fromView.Root;
            fromWidget.HorizontalDimensionBehaviour = ConstraintSizeTypeDic[(int)constraint];
            return fromView;
        }

        /// <summary>
        /// Set child width is a fixed value.
        /// </summary>
        /// <param name="fromView"></param>
        /// <param name="constant"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Parent of fromView is not ConstraintLayout</exception>
        public static FrameworkElement SetWidth(this FrameworkElement fromView, int constant)
        {
            var parent = fromView.Parent is ConstraintLayout ? fromView.Parent as ConstraintLayout : throw new ArgumentException($"Parent of {fromView} is not ConstraintLayout");
            var fromWidget = parent.GetWidget(fromView);
            fromWidget.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
            fromWidget.Width = constant;
            fromView.Width = constant;
            return fromView;
        }

        /// <summary>
        /// create constraint for child height.<br/>
        /// 1.if you set <see cref="SizeType.Match_Parent"/> or <see cref="SizeType.Match_Constraint"/>,must not set <see cref="FrameworkElement.Height"/>,they are conflict.<br/>
        /// 2.if you set <see cref="LayoutStyle"/> is <see cref="LayoutStyle.Chain_Packed"/>,must not set weight,they are conflict.
        /// </summary>
        /// <param name="fromView"></param>
        /// <param name="constraint"></param>
        /// <param name="weight">fromview's weight at vertical when multiple view's constraint is a chain</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Parent of fromView is not ConstraintLayout</exception>
        public static FrameworkElement SetHeight(this FrameworkElement fromView, ConstraintSet.SizeType constraint, float weight = float.NaN)
        {
            var parent = fromView.Parent is ConstraintLayout ? fromView.Parent as ConstraintLayout : throw new ArgumentException($"Parent of {fromView} is not ConstraintLayout");
            var fromWidget = parent.GetWidget(fromView);
            fromWidget.VerticalDimensionBehaviour = ConstraintSizeTypeDic[(int)constraint];
            if (!float.IsNaN(weight))
            {
                fromWidget.VerticalWeight = weight;
            }
            return fromView;
        }

        /// <summary>
        /// Set height of ConstraintLayout self.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public static FrameworkElement SetHeight(this ConstraintLayout root, ConstraintSet.SizeType constraint)
        {
            var fromWidget = root.Root;
            fromWidget.VerticalDimensionBehaviour = ConstraintSizeTypeDic[(int)constraint];
            return root;
        }

        /// <summary>
        /// Set child height is a fixed value.
        /// </summary>
        /// <param name="fromView"></param>
        /// <param name="constant"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Parent of fromView is not ConstraintLayout</exception>
        public static FrameworkElement SetHeight(this FrameworkElement fromView, int constant)
        {
            var parent = fromView.Parent is ConstraintLayout ? fromView.Parent as ConstraintLayout : throw new ArgumentException($"Parent of {fromView} is not ConstraintLayout");
            var fromWidget = parent.GetWidget(fromView);
            fromWidget.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
            fromWidget.Height = constant;
            fromView.Height = constant;
            return fromView;
        }
        /// <summary>
        /// Create constraint for Width, Width = ratio * Height.
        /// </summary>
        /// <param name="fromView"></param>
        /// <param name="ratio"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Parent of fromView is not ConstraintLayout</exception>
        public static FrameworkElement SetWidthBaseOnHeight(this FrameworkElement fromView, float ratio)
        {
            var parent = fromView.Parent is ConstraintLayout ? fromView.Parent as ConstraintLayout : throw new ArgumentException($"Parent of {fromView} is not ConstraintLayout");
            var fromWidget = parent.GetWidget(fromView);
            fromView.Width = double.NaN;//if FrameworkElement.Width is set value,constraint not work.
            fromWidget.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            fromWidget.setDimensionRatio(ratio, ConstraintWidget.HORIZONTAL);
            return fromView;
        }

        /// <summary>
        /// Create constraint for Height, Height = ratio * Width.
        /// </summary>
        /// <param name="fromView"></param>
        /// <param name="ratio"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Parent of fromView is not ConstraintLayout</exception>
        public static FrameworkElement SetHeightBaseOnWidth(this FrameworkElement fromView, float ratio)
        {
            var parent = fromView.Parent is ConstraintLayout ? fromView.Parent as ConstraintLayout : throw new ArgumentException($"Parent of {fromView} is not ConstraintLayout");
            var fromWidget = parent.GetWidget(fromView);
            fromView.Height = double.NaN;//if FrameworkElement.Height is set value,constraint not work.
            fromWidget.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            fromWidget.setDimensionRatio(ratio, ConstraintWidget.VERTICAL);
            return fromView;
        }

        #endregion Size

        #region Visiable
        /// <summary>
        /// Set the visibility for this view.It will recalculate constraint.
        /// </summary>
        /// <param name="fromView"></param>
        /// <param name="visibility">either VISIBLE, INVISIBLE, or GONE</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Parent of fromView is not ConstraintLayout</exception>
        public static FrameworkElement SetVisibility(this FrameworkElement fromView, ConstraintSet.Visibility visibility)
        {
            var parent = fromView.Parent is ConstraintLayout ? fromView.Parent as ConstraintLayout : throw new ArgumentException($"Parent of {fromView} is not ConstraintLayout");
            var fromWidget = parent.GetWidget(fromView);
            fromWidget.Visibility = (int)visibility;
            return fromView;
        }


        #endregion
        static readonly ConstraintWidget.DimensionBehaviour[] ConstraintSizeTypeDic = new ConstraintWidget.DimensionBehaviour[]
        {
            ConstraintWidget.DimensionBehaviour.FIXED,
            ConstraintWidget.DimensionBehaviour.WRAP_CONTENT,
            ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT,
            ConstraintWidget.DimensionBehaviour.MATCH_PARENT,
        };
    }
    
    #endregion
}

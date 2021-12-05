using androidx.constraintlayout.core.widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SharpConstraintLayout.Wpf
{

    public class ConstraintSet
    {
        static readonly ConstraintAnchor.Type[] ConstraintAnchorTypeDic = new ConstraintAnchor.Type[]
        {
            ConstraintAnchor.Type.NONE,
            ConstraintAnchor.Type.LEFT,
            ConstraintAnchor.Type.TOP,
            ConstraintAnchor.Type.RIGHT,
            ConstraintAnchor.Type.BOTTOM,
            ConstraintAnchor.Type.BASELINE,
            ConstraintAnchor.Type.CENTER,
            ConstraintAnchor.Type.CENTER_X,
            ConstraintAnchor.Type.CENTER_Y
        };

        internal static readonly ConstraintWidget.DimensionBehaviour[] ConstraintSizeTypeDic = new ConstraintWidget.DimensionBehaviour[]
        {
            ConstraintWidget.DimensionBehaviour.FIXED,
            ConstraintWidget.DimensionBehaviour.WRAP_CONTENT,
            ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT,
            ConstraintWidget.DimensionBehaviour.MATCH_PARENT,
        };

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
            /// <summary>
            /// Display the element.
            /// </summary>
            Visible = 0,
            /// <summary>
            /// Do not display the element, but reserve space for the element in layout.
            /// </summary>
            Invisible = 4,
            /// <summary>
            ///  Do not display the element, and do not reserve space for it in layout.
            /// </summary>
            Gone = 8,
        }

        public enum SizeType
        {
            /*FIXED,
            WRAP_CONTENT,
            MATCH_CONSTRAINT,
            MATCH_PARENT*/
            Fixed,
            WrapContent,
            MatchConstraint,
            MatchParent,
        }

        public enum LayoutStyle
        {
            /*public const int CHAIN_SPREAD = 0;
            public const int CHAIN_SPREAD_INSIDE = 1;
            public const int CHAIN_PACKED = 2;*/
            /// <summary>
            /// default style
            /// </summary>
            ChainSpread,
            ChainSpreadInside,
            ChainPacked
        }

        public enum Side
        {
            None,
            Left,
            Top,
            Right,
            
            Bottom,
            /// <summary>
            /// Not use. please use BaselineToBaseline
            /// </summary>
            Baseline,
            Center,
            CenterX,
            CenterY,
            
            /// <summary>
            /// Not use.
            /// TODO for RTF
            /// </summary>
            Start,
            /// <summary>
            /// Not use.
            /// TODO for RTF
            /// </summary>
            End,
            
        }

        WeakReference<ConstraintLayout> Parent;//一个ConstraintSet基本只使用一次,需要时重新创建,所以直接弱引用避免内存泄漏

        public ConstraintSet(ConstraintLayout parent)
        {
            Parent = new WeakReference<ConstraintLayout>(parent);
        }

        public ConstraintSet AddConnect(UIElement fromView, Side fromSide, UIElement toView, Side toSide, int margin)
        {
            Parent.TryGetTarget(out var parent);
            if (parent == null) throw new NotImplementedException();
            var fromWidget = parent.GetWidget(fromView);
            var toWidget = parent.GetWidget(toView);
            fromWidget.connect(ConstraintAnchorTypeDic[(int)fromSide], toWidget, ConstraintAnchorTypeDic[(int)toSide], margin);
            return this;
        }

        public ConstraintSet AddConnect(UIElement view, Side firstSide, UIElement secondView, Side secondSide)
        {
            return AddConnect(view, firstSide, secondView, secondSide, 0);
        }

        /// <summary>
        /// Set child view's width type。<br/>
        /// Default is <see cref="ConstraintSet.SizeType.WrapContent"/>.<br/>
        /// </summary>
        /// <param name="view"></param>
        /// <param name="sizeType">Set view's width according to Constraint or Parent or FrameworkElement.Width</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public ConstraintSet SetWidth(UIElement view, SizeType sizeType)
        {
            Parent.TryGetTarget(out var parent);
            if (parent == null) throw new NotImplementedException();
            var viewWidget = parent.GetWidget(view);
            viewWidget.HorizontalDimensionBehaviour = ConstraintSizeTypeDic[(int)sizeType];

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
        /// Set child view's width type.<br/>
        /// Default is <see cref="ConstraintSet.SizeType.WrapContent"/>.<br/>
        /// </summary>
        /// <param name="view"></param>
        /// <param name="sizeType"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public ConstraintSet SetHeight(UIElement view, SizeType sizeType)
        {
            Parent.TryGetTarget(out var parent);
            if (parent == null) throw new NotImplementedException();
            var viewWidget = parent.GetWidget(view);
            viewWidget.VerticalDimensionBehaviour = ConstraintSizeTypeDic[(int)sizeType];

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
        /// Notice:<br/>
        /// 1. Only use in Button,TextBlock,TextBox,Lable.<br/>
        /// 2. Only use in single line text. multiple line will have conflict.<br/>
        /// 3. it have bug, please use simpel set for text control,i don't deal with complex content.
        /// </summary>
        /// <param name="fromTextView"></param>
        /// <param name="toTextView"></param>
        /// <returns></returns>
        public static FrameworkElement BaselineToBaseline(this FrameworkElement fromTextView,FrameworkElement toTextView)
        {
            var parent = fromTextView.Parent is ConstraintLayout ? fromTextView.Parent as ConstraintLayout : throw new ArgumentException($"Parent of {fromTextView} is not ConstraintLayout");
            var fromWidget = parent.GetWidget(fromTextView);
            var toWidget = parent.GetWidget(toTextView);
            if(fromTextView is TextBlock || fromTextView is TextBox || fromTextView is Button|| fromTextView is Label)
            {
                if (toTextView is TextBlock || toTextView is TextBox || toTextView is Button || toTextView is Label)
                {
                    fromWidget.BaselineDistance = 1;
                    toWidget.BaselineDistance = 1;
                    fromWidget.connect(ConstraintAnchor.Type.BASELINE, toWidget, ConstraintAnchor.Type.BASELINE);
                }
                else
                {
                    throw new ArgumentException($"{toTextView} no baseline");
                }
            }
            else
            {
                throw new ArgumentException($"{fromTextView} no baseline");
            }
            return fromTextView;
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
        /// <param name="angle">  the angle (from 0 to 360),0 at Top </param>
        /// <param name="radius"> the radius used </param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Parent of fromView is not ConstraintLayout</exception>
        public static FrameworkElement CircleToCenter(this FrameworkElement fromView, FrameworkElement toView, float angle, int radius)
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
        /// Create constraint for child width.<br/>
        /// Default is <see cref="ConstraintSet.SizeType.WrapContent"/>.<br/>
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
            fromWidget.HorizontalDimensionBehaviour = ConstraintSet.ConstraintSizeTypeDic[(int)constraint];
            if (!float.IsNaN(weight))
            {
                fromWidget.HorizontalWeight = weight;
            }
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
        /// Create constraint for child height.<br/>
        /// Default is <see cref="ConstraintSet.SizeType.WrapContent"/>.<br/>
        /// Notice:<br/>
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
            fromWidget.VerticalDimensionBehaviour = ConstraintSet.ConstraintSizeTypeDic[(int)constraint];
            if (!float.IsNaN(weight))
            {
                fromWidget.VerticalWeight = weight;
            }
            return fromView;
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
        /// Create constraint for Width base on height, Width = ratio * Height.
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
        /// Create constraint for Height base on width, Height = ratio * Width.
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
            switch (visibility)
            {
                case ConstraintSet.Visibility.Visible:
                    fromView.Visibility = System.Windows.Visibility.Visible;
                    break;
                case ConstraintSet.Visibility.Gone:
                    fromView.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case ConstraintSet.Visibility.Invisible:
                    fromView.Visibility = System.Windows.Visibility.Hidden;
                    break;
            }
            return fromView;
        }

        /// <summary>
        /// 去掉某一个View的所有约束,但不在移除控件树种移除
        /// </summary>
        /// <exception cref="ArgumentException">Parent of fromView is not ConstraintLayout</exception>
        public static FrameworkElement Clear(this FrameworkElement fromView)
        {
            var parent = fromView.Parent is ConstraintLayout ? fromView.Parent as ConstraintLayout : throw new ArgumentException($"Parent of {fromView} is not ConstraintLayout");
            var fromWidget = parent.GetWidget(fromView);
            fromWidget.resetAllConstraints();
            return fromView;
        }

        #endregion
    }

    #endregion
}

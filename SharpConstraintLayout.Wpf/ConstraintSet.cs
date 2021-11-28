using androidx.constraintlayout.core.widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SharpConstraintLayout.Wpf
{
    /// <summary>
    /// Text orientation
    /// </summary>
    public enum TextOrientation
    {
        LeftToRight,
        RightToLeft
    }
    public class ConstraintSet
    {
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

        //TODO:radtio public ConstraintSet Set
        
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
        /// <summary>
        /// fromView's center = toView's center
        /// </summary>
        /// <param name="fromView"></param>
        /// <param name="toView"></param>
        /// <param name="margin"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
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
        /// <exception cref="ArgumentException"></exception>
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
        /// <exception cref="ArgumentException"></exception>
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
        /// <exception cref="ArgumentException"></exception>
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
        /// <exception cref="ArgumentException"></exception>
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
        /// <exception cref="ArgumentException"></exception>
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
        /// <exception cref="ArgumentException"></exception>
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
        /// <exception cref="ArgumentException"></exception>
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
        /// <exception cref="ArgumentException"></exception>
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
        /// <exception cref="ArgumentException"></exception>
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
        /// <exception cref="ArgumentException"></exception>
        public static FrameworkElement BottomToBottom(this FrameworkElement fromView, FrameworkElement toView, int margin = 0)
        {
            var parent = fromView.Parent is ConstraintLayout ? fromView.Parent as ConstraintLayout : throw new ArgumentException($"Parent of {fromView} is not ConstraintLayout");
            var fromWidget = parent.GetWidget(fromView);
            var toWidget = parent.GetWidget(toView);
            fromWidget.connect(ConstraintAnchor.Type.BOTTOM, toWidget, ConstraintAnchor.Type.BOTTOM, margin);
            return fromView;
        }

        /// <summary>
        /// Set child width.
        /// </summary>
        /// <param name="fromView"></param>
        /// <param name="constraint"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static FrameworkElement SetWidth(this FrameworkElement fromView, ConstraintSizeType constraint)
        {
            var parent = fromView.Parent is ConstraintLayout ? fromView.Parent as ConstraintLayout : throw new ArgumentException($"Parent of {fromView} is not ConstraintLayout");
            var fromWidget = parent.GetWidget(fromView);
            fromWidget.HorizontalDimensionBehaviour = ConstraintSizeTypeDic[(int)constraint];
            return fromView;
        }
        /// <summary>
        /// Set width of ConstraintLayout self 
        /// </summary>
        /// <param name="fromView"></param>
        /// <param name="constraint"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static FrameworkElement SetWidth(this ConstraintLayout fromView, ConstraintSizeType constraint)
        {
            var fromWidget = fromView.Root;
            fromWidget.HorizontalDimensionBehaviour = ConstraintSizeTypeDic[(int)constraint];
            return fromView;
        }

        /// <summary>
        /// Set child
        /// </summary>
        /// <param name="fromView"></param>
        /// <param name="constant"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
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
        /// Set child
        /// </summary>
        /// <param name="fromView"></param>
        /// <param name="constraint"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static FrameworkElement SetHeight(this FrameworkElement fromView, ConstraintSizeType constraint)
        {
            var parent = fromView.Parent is ConstraintLayout ? fromView.Parent as ConstraintLayout : throw new ArgumentException($"Parent of {fromView} is not ConstraintLayout");
            var fromWidget = parent.GetWidget(fromView);
            fromWidget.VerticalDimensionBehaviour = ConstraintSizeTypeDic[(int)constraint];
            return fromView;
        }
        /// <summary>
        /// Set height of ConstraintLayout self 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="constraint"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static FrameworkElement SetHeight(this ConstraintLayout root, ConstraintSizeType constraint)
        {
            var fromWidget = root.Root;
            fromWidget.VerticalDimensionBehaviour = ConstraintSizeTypeDic[(int)constraint];
            return root;
        }

        /// <summary>
        /// Set child
        /// </summary>
        /// <param name="fromView"></param>
        /// <param name="constant"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static FrameworkElement SetHeight(this FrameworkElement fromView, int constant)
        {
            var parent = fromView.Parent is ConstraintLayout ? fromView.Parent as ConstraintLayout : throw new ArgumentException($"Parent of {fromView} is not ConstraintLayout");
            var fromWidget = parent.GetWidget(fromView);
            fromWidget.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
            fromWidget.Height = constant;
            fromView.Height = constant;
            return fromView;
        }

        static readonly ConstraintWidget.DimensionBehaviour[] ConstraintSizeTypeDic = new ConstraintWidget.DimensionBehaviour[]
        {
            ConstraintWidget.DimensionBehaviour.FIXED,
            ConstraintWidget.DimensionBehaviour.WRAP_CONTENT,
            ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT,
            ConstraintWidget.DimensionBehaviour.MATCH_PARENT,
        };
    }

    public enum ConstraintSizeType
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
    #endregion
}

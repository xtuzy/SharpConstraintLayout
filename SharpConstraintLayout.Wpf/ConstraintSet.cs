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
        WeakReference<ConstraintLayout> Parent;//一个ConstraintSet基本只使用一次,需要时重新创建,所以直接弱引用避免内存泄漏

        public ConstraintSet(ConstraintLayout parent)
        {
            Parent = new WeakReference<ConstraintLayout>(parent);
        }

        public ConstraintSet AddConnect(UIElement view, ConstraintAnchor.Type firstSide, UIElement secondView, ConstraintAnchor.Type secondSide, int margin)
        {
            Parent.TryGetTarget(out var parent);
            if (parent == null) throw new NotImplementedException();
            var viewWidget = parent.GetWidget(view);
            var secondViewWidget = parent.GetWidget(secondView);
            viewWidget.connect(firstSide, secondViewWidget, secondSide, margin);
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

        public ConstraintSet SetWidth(UIElement view,int minWidth)
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
}

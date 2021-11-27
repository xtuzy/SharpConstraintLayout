using androidx.constraintlayout.core.widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SharpConstraintLayout.Example.Reload
{
    public class ZConstraintSet
    {
        WeakReference<ZLayout> Parent;//一个ConstraintSet基本只使用一次,需要时重新创建,所以直接弱引用避免内存泄漏
        
        public ZConstraintSet(ZLayout parent)
        {
            Parent = new WeakReference<ZLayout>(parent);
        }

        public ZConstraintSet AddConnect(FrameworkElement view, ConstraintAnchor.Type firstSide, FrameworkElement secondView, ConstraintAnchor.Type secondSide, int margin)
        {
            Parent.TryGetTarget(out var parent);
            if (parent == null) throw new NotImplementedException();
            var viewWidget = parent.GetWidget(view);
            var secondViewWidget = parent.GetWidget(secondView);
            viewWidget.connect(firstSide, secondViewWidget, secondSide, margin);
            return this;
        }

        public ZConstraintSet AddConnect(FrameworkElement view, ConstraintAnchor.Type firstSide, FrameworkElement secondView, ConstraintAnchor.Type secondSide)
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
        public ZConstraintSet SetWidth(FrameworkElement view, ConstraintWidget.DimensionBehaviour behaviour)
        {
            Parent.TryGetTarget(out var parent);
            if (parent == null) throw new NotImplementedException();
            ConstraintWidget viewWidget =  parent.GetWidget(view);
            viewWidget.HorizontalDimensionBehaviour = behaviour;

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
        public ZConstraintSet SetHeight(FrameworkElement view, ConstraintWidget.DimensionBehaviour behaviour)
        {
            Parent.TryGetTarget(out var parent);
            if (parent == null) throw new NotImplementedException();
            var viewWidget = parent.GetWidget( view);
            viewWidget.VerticalDimensionBehaviour = behaviour;
            
            return this;
        }

        /// <summary>
        /// 去掉某一个View的约束
        /// </summary>
        /// <param name="view"></param>
        public void Clear(FrameworkElement view)
        {
            Parent.TryGetTarget(out var parent);
            if (parent == null) throw new NotImplementedException();
            var viewWidget = parent.GetWidget(view);
            viewWidget.resetAllConstraints();
        }
    }
}

#if __ANDROID__ && !__MAUI__
using Android.Content;
using Android.Views;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIElement = Android.Views.View;
namespace SharpConstraintLayout.Maui.Widget
{
    public partial class ConstraintLayout
    {
        public ConstraintLayout(Context context) : base(context)
        {
            init();
        }
        
        public ConstraintLayout(Context context, ILogger logger) : base(context)
        {
            Logger = logger;
            init();
        }

#region Add And Remove
        public override void AddView(UIElement element)
        {
            OnAddedView(element);//安卓添加View进视觉树时,会调用OnAttachedToWindow,导致调用C对应的Constraint,因此需要在之前生成
            base.AddView(element);
        }
        /// <summary>
        /// Same as <see cref="AddView(UIElement)"/>
        /// </summary>
        /// <param name="element"></param>
        public void AddElement(UIElement element)
        {
            this.AddView(element);
        }

        public override void RemoveView(UIElement element)
        {
            OnRemovedView(element);
            base.RemoveView(element);
        }

        /// <summary>
        /// Same as <see cref="RemoveView(UIElement)"/>
        /// </summary>
        /// <param name="element"></param>
        public void RemoveElement(UIElement element)
        {
            this.RemoveView(element);
        }

        public override void RemoveAllViews()
        {
            int childCount = ChildCount;
            for (var i = childCount - 1; i >= 0; i--)
            {
                View currentChild = this.GetChildAt(i);
                this.RemoveView(currentChild);
            }
        }

        /// <summary>
        /// Same as <see cref="RemoveAllViews()"/>
        /// </summary>
        public void RemoveAllElements()
        {
            this.RemoveAllViews();
        }

#endregion

#region Measure And Layout

        /// <summary>
        /// Android中不使用
        /// </summary>
        /// <param name="layout"></param>
        /// <param name="constrainWidth"></param>
        /// <param name="constrainHeight"></param>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public (bool isInfinityAvailabelWidth, bool isInfinityAvailabelHeight) IsInfinitable(ConstraintLayout layout, int constrainWidth, int constrainHeight, SizeI availableSize)
        {
            throw new NotImplementedException();
        }
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            var size = MeasureLayout(new SizeI(SharpConstraintLayout.Maui.Widget.MeasureSpec.GetSize(widthMeasureSpec), SharpConstraintLayout.Maui.Widget.MeasureSpec.GetSize(heightMeasureSpec)), widthMeasureSpec, heightMeasureSpec);
            SetMeasuredDimension(size.Width, size.Height);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            ArrangeLayout();
        }

        internal partial void LayoutChild(UIElement element, int x, int y, int w, int h)
        {
            element.Layout(x, y, x + w, y + h);
        }

#endregion
    }
}
#endif
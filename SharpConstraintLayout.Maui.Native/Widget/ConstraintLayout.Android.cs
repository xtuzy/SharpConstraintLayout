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
            bool isInfinityAvailabelWidth = false;
            bool isInfinityAvailabelHeight = false;
            if (layout.Parent is Android.Widget.ScrollView)//ScrollView的内容可以是无限值,ConstraintLayout作为其子View时,只有在WrapContent时才有无限值
            {
                if (constrainWidth == ConstraintSet.WrapContent)
                {
                    isInfinityAvailabelWidth = true;
                }
                if (constrainHeight == ConstraintSet.WrapContent)
                {
                    isInfinityAvailabelHeight = true;
                }
            }
            return (isInfinityAvailabelWidth, isInfinityAvailabelHeight);
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            var availableSize = new Size(UIElementExtension.PxToDp(SharpConstraintLayout.Maui.Widget.MeasureSpec.GetSize(widthMeasureSpec)), UIElementExtension.PxToDp(SharpConstraintLayout.Maui.Widget.MeasureSpec.GetSize(heightMeasureSpec)));
            var availableSizeI = new SizeI(UIElementExtension.DpToScaledPx(availableSize.Width), UIElementExtension.DpToScaledPx(availableSize.Height));

            (int horizontalSpec, int verticalSpec) = MakeSpec(this, availableSizeI);

            var size = MeasureLayout(availableSizeI, horizontalSpec, verticalSpec);
            var w = UIElementExtension.DpToPx(UIElementExtension.ScaledPxToDp(size.Width));
            var h = UIElementExtension.DpToPx(UIElementExtension.ScaledPxToDp(size.Height));
            SetMeasuredDimension(w, h);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            ArrangeLayout();
        }

        internal partial void LayoutChild(UIElement element, int x, int y, int w, int h)
        {
            var left = UIElementExtension.DpToPx(UIElementExtension.ScaledPxToDp(x));
            var top = UIElementExtension.DpToPx(UIElementExtension.ScaledPxToDp(y));
            var right = UIElementExtension.DpToPx(UIElementExtension.ScaledPxToDp(x + w));
            var bottom = UIElementExtension.DpToPx(UIElementExtension.ScaledPxToDp(y + h));
            element.Layout(left, top , right, bottom);
        }

#endregion
    }
}
#endif
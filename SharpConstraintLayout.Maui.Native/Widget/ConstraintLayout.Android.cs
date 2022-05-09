#if __ANDROID__ && !__MAUI__
using Android.Content;
using Android.Views;
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

        #region Add And Remove
        public override void AddView(UIElement element)
        {
            base.AddView(element);
            OnAddedView(element);
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
            base.RemoveView(element);
            OnRemovedView(element);
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
            for (var i = 0; i < childCount; i++)
            {
                View currentChild = this.GetChildAt(i);
                this.RemoveView(currentChild);
                break;
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
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            //base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            MeasureLayout(new Size(SharpConstraintLayout.Maui.Widget.MeasureSpec.GetSize(widthMeasureSpec), SharpConstraintLayout.Maui.Widget.MeasureSpec.GetSize(heightMeasureSpec)), widthMeasureSpec, heightMeasureSpec);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            ArrangeLayout();
        }

        void LayoutChild(UIElement element, int x, int y, int w, int h)
        {
            element.Layout(x, y, x + w, y + h);
        }

        #endregion
    }
}
#endif
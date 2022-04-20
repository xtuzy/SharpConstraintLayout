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

        public override void RemoveView(UIElement element)
        {
            base.RemoveView(element);
            OnRemovedView(element);
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

        #endregion
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            //base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            Measure(new Size(SharpConstraintLayout.Maui.Widget.MeasureSpec.GetSize(widthMeasureSpec), SharpConstraintLayout.Maui.Widget.MeasureSpec.GetSize(heightMeasureSpec)));
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            OnLayout();
        }

        void LayoutChild(UIElement element, int x, int y, int w, int h)
        {
            element.Layout(x, y, x + w, y + h);
        }
    }
}

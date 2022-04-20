using Android.Views;
using MauiWrapPanel.DebugTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MauiWrapPanel.Base
{
#if __ANDROID__
    using ElementView = Android.Views.View;
#elif __IOS__
using ElementView = UIKit.UIView;
#elif WINDOWS
    using ElementView = Microsoft.UI.Xaml.UIElement;
#endif
    public partial class BaseLayout : ViewGroup
    {
        public BaseLayout(Android.Content.Context context) : base(context)
        {
        }
        int widthMeasureSpec;
        int heightMeasureSpec;
        long measureTime;
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            if (MEASURE) measureTime = DateTimeHelperClass.CurrentUnixTimeMillis();
            this.widthMeasureSpec = widthMeasureSpec;
            this.heightMeasureSpec = heightMeasureSpec;
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            MauiMeasure(new Microsoft.Maui.Graphics.Size(MeasureSpec.GetSize(widthMeasureSpec), MeasureSpec.GetSize(heightMeasureSpec)));
            if (MEASURE) SimpleDebug.WriteLine($"{this.GetType().FullName} Measure time: {DateTimeHelperClass.CurrentUnixTimeMillis() - measureTime}ms");
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            MauiLayout(new Microsoft.Maui.Graphics.Rect(l, t, r - l, b - t));
            if (DEBUG) SimpleDebug.WriteLine($"{this.GetType().FullName} Layout info: {PrintLayoutInfo(this)}");
        }

        protected virtual void MeasureElement(ElementView element, int w, int h)
        {
            element.Measure(GetChildMeasureSpec(widthMeasureSpec, 0, element.LayoutParameters.Width), GetChildMeasureSpec(heightMeasureSpec, 0, element.LayoutParameters.Height));
        }

        protected virtual void LayoutElement(ElementView element, int x, int y, int w, int h)
        {
            element.Layout(x, y, x + w, y + h);
            if (DEBUG) SimpleDebug.WriteLine($"{this.GetType().FullName} Layout info: {PrintLayoutInfo(element)}");
        }

        public (int Width, int Height) GetElementSize(ElementView element)
        {
            return (element.MeasuredWidth, element.MeasuredHeight);
        }

        /// <summary>
        /// Call this when something has changed which has invalidated the layout of this view. This will schedule a layout pass of the view tree. This should not be called while the view hierarchy is currently in a layout pass (isInLayout(). If layout is happening, the request may be honored at the end of the current layout pass (and then layout will run again) or after the current frame is drawn and the next layout occurs.
        /// Subclasses which override this method should call the superclass method to handle possible request-during-layout errors correctly.
        /// </summary>
        public void RefreshLayout(ElementView element)
        {
            //According to https://stackoverflow.com/questions/13856180/usage-of-forcelayout-requestlayout-and-invalidate
            //At Android,this will let remeasure layout
            element.RequestLayout();
        }

        public string PrintLayoutInfo(ElementView element)
        {
            return $"{element.GetType().FullName} Visibility={element.Visibility} Position={element.GetX()}x{element.GetY()} Size={element.Width}x{element.Height} MeasuredSize={element.MeasuredWidth}x{element.MeasuredHeight}";
        }

        public ElementView GetParent(ElementView element)
        {
            return element.Parent as ElementView;
        }

        public bool IsVisiable(ElementView element)
        {
            return element.Visibility == ViewStates.Visible;
        }

        void SetVisiable(ElementView element, bool value)
        {
            if (value)
                element.Visibility = ViewStates.Visible;
            else
                element.Visibility = ViewStates.Gone;
        }

        public void AddElement(ElementView element)
        {
            AddView(element);
        }
    }
}

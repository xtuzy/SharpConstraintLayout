using CoreGraphics;
using MauiWrapPanel.DebugTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;
namespace MauiWrapPanel.Base
{
#if __ANDROID__
using ElementView = Android.Views.View;
#elif __IOS__
    using ElementView = UIKit.UIView;
#elif WINDOWS
    using ElementView = Microsoft.UI.Xaml.UIElement;
#endif
    public partial class BaseLayout : UIView
    {
        public ElementView GetChildAt(int index)
        {
            return Subviews[index];
        }
        public int ChildCount => Subviews.Length;
        //layout自身没有默认的大小,需要依靠布局时计算
        public override CGSize IntrinsicContentSize => measuredSize;

        long measureTime;
        CGSize measuredSize = new CGSize(-1, -1);
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            if (MEASURE) measureTime = DateTimeHelperClass.CurrentUnixTimeMillis();
            measuredSize = MauiMeasure(this.Frame.Size.ToMauiSize());
            if (MEASURE) SimpleDebug.WriteLine($"{this.GetType().FullName} Measure time: {DateTimeHelperClass.CurrentUnixTimeMillis() - measureTime}ms");
            MauiLayout(this.Frame.ToMauiRect());
            if (DEBUG) SimpleDebug.WriteLine($"{this.GetType().FullName} Layout info: {PrintLayoutInfo(this)}");
        }

        protected virtual void MeasureElement(ElementView element, int w, int h)
        {
        }

        protected virtual void LayoutElement(ElementView element, int x, int y, int w, int h)
        {
            element.Frame = new CoreGraphics.CGRect(x, y, w, h);
            if (DEBUG) SimpleDebug.WriteLine($"{this.GetType().FullName} Layout info: {PrintLayoutInfo(element)}");
        }

        public (int Width, int Height) GetElementSize(ElementView element)
        {
            return ((int)element.IntrinsicContentSize.Width, (int)element.IntrinsicContentSize.Height);
        }

        /// <summary>
        /// Call this when something has changed which has invalidated the layout of this view. This will schedule a layout pass of the view tree. This should not be called while the view hierarchy is currently in a layout pass (isInLayout(). If layout is happening, the request may be honored at the end of the current layout pass (and then layout will run again) or after the current frame is drawn and the next layout occurs.
        /// Subclasses which override this method should call the superclass method to handle possible request-during-layout errors correctly.
        /// </summary>
        public void RefreshLayout(ElementView element)
        {
            element.SetNeedsLayout();
        }

        public string PrintLayoutInfo(ElementView element)
        {
            return $"{element.GetType().FullName} IsHiden={element.Hidden} Frame={element.Frame} Bounds={element.Bounds} IntrinsicContentSize={element.IntrinsicContentSize}";
        }

        public ElementView GetParent(ElementView element)
        {
            return element.Superview;
        }

        public bool IsVisiable(ElementView element)
        {
            return !element.Hidden;
        }

        void SetVisiable(ElementView element, bool value)
        {
            element.Hidden = !value;
        }

        public void AddElement(ElementView element)
        {
            Add(element);
        }
    }
}

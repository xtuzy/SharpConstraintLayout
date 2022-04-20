using MauiWrapPanel.DebugTool;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace MauiWrapPanel.Base
{
    using ElementView = Microsoft.UI.Xaml.UIElement;
    public partial class BaseLayout : Panel
    {
        public ElementView GetChildAt(int index)
        {
            return Children[index];
        }

        public int ChildCount { get { return (Children?.Count) ?? 0; } }

        long measureTime;
        protected override Size MeasureOverride(Size availableSize)
        {
            if (MEASURE) measureTime = DateTimeHelperClass.CurrentUnixTimeMillis();
            var size = MauiMeasure(availableSize.ToMauiSize()).ToPlaformSize();
            if (MEASURE) SimpleDebug.WriteLine($"{this.GetType().FullName} Measure time: {DateTimeHelperClass.CurrentUnixTimeMillis() - measureTime}ms");
            return size;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (DEBUG) SimpleDebug.WriteLine($"{this.GetType().FullName} Layout info: {PrintLayoutInfo(this)}");
            return MauiLayout(new Microsoft.Maui.Graphics.Rect(0, 0, finalSize.Width, finalSize.Height)).ToPlaformSize();
        }

        protected virtual void MeasureElement(ElementView element, int w, int h)
        {
            element.Measure(new Windows.Foundation.Size(w, h));
        }

        protected virtual void LayoutElement(ElementView element, int x, int y, int w, int h)
        {
            element.Arrange(new Rect(x, y, w, h));
            if (DEBUG) SimpleDebug.WriteLine($"{this.GetType().FullName} Layout info: {PrintLayoutInfo(element)}");
        }

        public (int Width, int Height) GetElementSize(ElementView element)
        {
            return ((int)element.DesiredSize.Width, (int)element.DesiredSize.Height);
        }

        /// <summary>
        /// Call this when something has changed which has invalidated the layout of this view. This will schedule a layout pass of the view tree. This should not be called while the view hierarchy is currently in a layout pass (isInLayout(). If layout is happening, the request may be honored at the end of the current layout pass (and then layout will run again) or after the current frame is drawn and the next layout occurs.
        /// Subclasses which override this method should call the superclass method to handle possible request-during-layout errors correctly.
        /// </summary>
        public void RefreshLayout(ElementView element)
        {
            element.InvalidateMeasure();
        }

        public string PrintLayoutInfo(ElementView element)
        {
            return $"{element.GetType().FullName} Visibility={element.Visibility} Position={element.ActualOffset} DesiredSize={element.DesiredSize} ActualSize={element.ActualSize}";
        }

        public ElementView GetParent(ElementView element)
        {
            return (element as Microsoft.UI.Xaml.FrameworkElement).Parent as ElementView;
        }

        public bool IsVisiable(ElementView element)
        {
            return element.Visibility == Microsoft.UI.Xaml.Visibility.Visible;
        }

        void SetVisiable(ElementView element, bool value)
        {
            element.Visibility = value ? Microsoft.UI.Xaml.Visibility.Visible : Microsoft.UI.Xaml.Visibility.Collapsed;
        }

        public void AddElement(ElementView element)
        {
            Children.Add(element);
        }
    }
}
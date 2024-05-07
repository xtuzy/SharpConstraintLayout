#if __MAUI__
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Layouts;
using UIElement = Microsoft.Maui.Controls.View;
namespace SharpConstraintLayout.Maui.Widget
{
    public partial class ConstraintLayout : IMauiConstraintLayout
    {
        public ConstraintLayout()
        {
            init();
        }
        
        public ConstraintLayout(ILogger logger)
        {
            Logger = logger;
            init();
        }

        #region Add And Remove
        public new void Add(IView child)
        {
            OnAddedView(child as UIElement);
            base.Add(child);
        }

        public UIElement GetChildAt(int index)
        {
            return Children[index] as UIElement;
        }

        public void AddElement(UIElement element)
        {
            this.Add(element);
        }

        public new void Remove(IView child)
        {
            OnRemovedView(child as UIElement);
            base.Remove(child);
        }

        public void RemoveElement(UIElement element)
        {
            this.Remove(element);
        }

        public void RemoveAllElements()
        {
            for (int i = this.Children.Count - 1; i >= 0; i--)
            {
                var element = Children[i];
                this.Remove(element);
            }
        }

        public int ChildCount { get { return Children != null ? Children.Count : 0; } }

        #endregion

        #region Layout

        /// <summary>
        /// 平台给出的Size值可能是无限大小，ConstraintLayout中转换为像素时都使用int存储。
        /// </summary>
        /// <param name="layout"></param>
        /// <param name="constrainWidth"></param>
        /// <param name="constrainHeight"></param>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        public (bool isInfinityAvailabelWidth, bool isInfinityAvailabelHeight) IsInfinitable(ConstraintLayout layout, int constrainWidth, int constrainHeight, SizeI availableSize)
        {
            bool isInfinityAvailabelWidth = false;
            bool isInfinityAvailabelHeight = false;
            if (int.MaxValue == (availableSize.Width))
            {
                isInfinityAvailabelWidth = true;
            }

            if (int.MaxValue == (availableSize.Height))
            {
                isInfinityAvailabelHeight = true;
            }
            return (isInfinityAvailabelWidth, isInfinityAvailabelHeight);
        }

        protected override ILayoutManager CreateLayoutManager()
        {
            return new ConstraintLayoutManager(this);
        }

        internal partial void LayoutChild(UIElement element, int x, int y, int w, int h)
        {
            (element as IView).Arrange(new Rect(UIElementExtension.ScaledPxToDp(x), UIElementExtension.ScaledPxToDp(y), UIElementExtension.ScaledPxToDp(w), UIElementExtension.ScaledPxToDp(h)));//px to maui's dp
        }
        #endregion

        public SizeI GetLastMeasureSize() { return new SizeI(mLastMeasureWidth, mLastMeasureHeight); }

        /// <summary>
        /// 获取ConstraintLayout所有子View信息,这是为动画特别提供的
        /// </summary>
        /// <param name="views">if is null, will capture layout info for all view. if not null, only capture info for these view</param>
        /// <param name="isNeedMeasure">没有测量过的可能需要测量才能获得到正确的信息</param>
        /// <returns></returns>
        public Dictionary<int, ViewInfo> CaptureLayoutTreeInfo(List<View> views = null, bool isNeedMeasure = false)
        {
            //Try强制Measure
            if (isNeedMeasure)
            {
                var availableSize = GetLastMeasureSize();
                (int horizontalSpec, int verticalSpec) = this.MakeSpec(this, availableSize);
                this.MeasureLayout(availableSize, horizontalSpec, verticalSpec);
            }

            var dic = new Dictionary<int, ViewInfo>();
            if (views == null)
            {
                foreach (var item in idToViews)
                {
                    var view = item.Value;
                    var widget = GetOrAddWidgetById(item.Key);
                    var constraint = mConstraintSet.GetConstraint(item.Key);
                    var info = new ViewInfo()
                    {
                        X = widget.X,
                        Y = widget.Y,
                        Width = widget.Width,
                        Height = widget.Height,
                        TranlateX = constraint.transform.translationX,
                        TranlateY = constraint.transform.translationY,
                        //Scale = constraint.transform.scale., 
                        ScaleX = constraint.transform.scaleX,
                        ScaleY = constraint.transform.scaleY,
                        Rotation = constraint.transform.rotation,
                        RotationX = constraint.transform.rotationX,
                        RotationY = constraint.transform.rotationY
                    };
                    //这里处理Visibility与Alpha的冲突. 不可见时Alpha都是0, 可见时使用正确的Alpha
                    if (constraint.propertySet.visibility == ConstraintSet.Invisible || constraint.propertySet.visibility == ConstraintSet.Gone)
                        info.Alpha = 0;
                    else
                        info.Alpha = constraint.propertySet.alpha;
                    dic.Add(item.Key, info);
                }
            }
            else
            {
                foreach (var item in views)
                {
                    var id = item.GetId();
                    var widget = GetOrAddWidgetById(id);
                    var constraint = mConstraintSet.GetConstraint(id);
                    var info = new ViewInfo()
                    {
                        X = widget.X,
                        Y = widget.Y,
                        Width = widget.Width,
                        Height = widget.Height,
                        TranlateX = constraint.transform.translationX,
                        TranlateY = constraint.transform.translationY,
                        //Scale = constraint.transform.scale., 
                        ScaleX = constraint.transform.scaleX,
                        ScaleY = constraint.transform.scaleY,
                        Rotation = constraint.transform.rotation,
                        RotationX = constraint.transform.rotationX,
                        RotationY = constraint.transform.rotationY
                    };
                    //这里处理Visibility与Alpha的冲突. 不可见时Alpha都是0, 可见时使用正确的Alpha
                    if (constraint.propertySet.visibility == ConstraintSet.Invisible || constraint.propertySet.visibility == ConstraintSet.Gone)
                        info.Alpha = 0;
                    else
                        info.Alpha = constraint.propertySet.alpha;
                    dic.Add(id, info);
                }
            }
            return dic;
        }
    }

    public class ConstraintLayoutManager : LayoutManager
    {
        public ConstraintLayoutManager(IMauiConstraintLayout constraintLayout) : base(constraintLayout)
        {
        }

        public override Size Measure(double widthConstraint, double heightConstraint)
        {
            var layout = Layout as ConstraintLayout;
            var availableSize = new SizeI(UIElementExtension.DpToScaledPx(widthConstraint), UIElementExtension.DpToScaledPx(heightConstraint));//maui's dp to px
            (int horizontalSpec, int verticalSpec) = layout.MakeSpec(layout, availableSize);
            var finalSize = layout.MeasureLayout(availableSize, horizontalSpec, verticalSpec);
            var resultW = UIElementExtension.ScaledPxToDp(finalSize.Width);
            var resultH = UIElementExtension.ScaledPxToDp(finalSize.Height);
            return new Size(resultW, resultH);//px to maui's dp
        }

        public override Size ArrangeChildren(Rect bounds)
        {
            var layout = Layout as ConstraintLayout;
            var finalSize = new SizeI(UIElementExtension.DpToScaledPx(bounds.Size.Width), UIElementExtension.DpToScaledPx(bounds.Height));//maui's dp to px, when measure, MakeSpec will add 0.5
            if (layout.mConstraintSet.IsForAnimation)
            {
                //如果是动画,那么此时计算布局完毕,但我们不能让其布局,因此直接返回.另外如果给WidthRequest等赋固定值,会造成下次的控件显示大小为固定值,因此需要重置
                foreach (var child in layout.Children)
                {
                    var view = child as View;
                    if (view.WidthRequest > 0)
                        view.SetSizeAndMargin(width:ConstraintSet.Unset);
                    if (view.HeightRequest > 0)
                        view.SetSizeAndMargin(height:ConstraintSet.Unset);
                }
                return bounds.Size;
            }
            var lastMeasureSize = layout.GetLastMeasureSize();// will get int
            if (Math.Abs(finalSize.Width - lastMeasureSize.Width) > 1 || Math.Abs(finalSize.Height - lastMeasureSize.Height) > 1)//re measure when difference > 1 pixel
            {
                // We haven't received our desired size. We need to refresh the rows.
                (int horizontalSpec, int verticalSpec) = layout.MakeSpec(layout as ConstraintLayout, finalSize);
                finalSize = layout.MeasureLayout(finalSize, horizontalSpec, verticalSpec);
            }
            layout.ArrangeLayout();
            var resultW = UIElementExtension.ScaledPxToDp(finalSize.Width);
            var resultH = UIElementExtension.ScaledPxToDp(finalSize.Height);
            return new Size(resultW, resultH);//px to maui's dp
        }
    }

    public interface IMauiConstraintLayout : Microsoft.Maui.ILayout
    {
        SizeI MeasureLayout(SizeI availableSize, int horizontalSpec = 0, int verticalSpec = 0);
        void ArrangeLayout();

        SizeI GetLastMeasureSize();

        (int horizontalSpec, int verticalSpec) MakeSpec(ConstraintLayout layout, SizeI availableSize);
    }
}
#endif
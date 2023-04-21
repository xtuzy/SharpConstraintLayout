#if __IOS__ && !__MAUI__
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Panel = UIKit.UIView;
using UIElement = UIKit.UIView;
using CoreGraphics;
using Size = CoreGraphics.CGSize;
using Microsoft.Maui.Graphics;
using UIKit;
using androidx.constraintlayout.core.widgets;
using AndroidMeasureSpec = SharpConstraintLayout.Maui.Widget.MeasureSpec;
using Microsoft.Extensions.Logging;

namespace SharpConstraintLayout.Maui.Widget
{
    /// <summary>
    /// 不要使用Auto Layout为ConstraintLayout添加宽高约束.
    /// </summary>
    public partial class ConstraintLayout
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

        public UIElement GetChildAt(int index)
        {
            return Subviews[index];
        }

        public override void AddSubview(UIElement view)
        {
            OnAddedView(view);
            base.AddSubview(view);
        }

        /// <summary>
        /// Same as <see cref="AddSubview(UIElement)"/>
        /// </summary>
        /// <param name="element"></param>
        public void AddElement(UIElement element)
        {
            this.AddSubview(element);
        }

        public void RemoveView(UIElement element)
        {
            OnRemovedView(element);
            element.RemoveFromSuperview();
        }

        /// <summary>
        /// Same as <see cref="RemoveView(UIElement)"/>
        /// </summary>
        /// <param name="element"></param>
        public void RemoveElement(UIElement element)
        {
            this.RemoveView(element);
        }

        public void RemoveAllViews()
        {
            for (int i = this.Subviews.Length - 1; i >= 0; i--)
            {
                var element = this.Subviews[i];
                OnRemovedView(element);
                element.RemoveFromSuperview();
            }
        }

        /// <summary>
        /// Same as <see cref="RemoveAllViews"/>
        /// </summary>
        public void RemoveAllElements()
        {
            this.RemoveAllViews();
        }

        public int ChildCount => Subviews.Length;

#endregion

#region Measure And Layout
        /// <summary>
        /// DEBUG info will use IntrinsicContentSize, will have infinity loop, so set just use it at first time measure.
        /// </summary>
        bool isFirstTimeMeasure = true;
        /// <summary>
        /// Provide the size of the layout to parent
        /// </summary>
        public override Size IntrinsicContentSize
        {
            get
            {
                //父布局要调用IntrinsicContentSize作为子View大小的评判,这个发生在子View的LayoutSubviews之前,因此在此处先直接计算.
                //比如UIScrollView作为Parent时,需要此值去计算ContentSize
                if (this.Bounds.Size.Width <= 5 && isFirstTimeMeasure)
                {
                    isFirstTimeMeasure = false;
                    this.Bounds = new CGRect(this.Bounds.Location, MeasureSubviews());
                }
                if (DEBUG) Debug.WriteLine($"{Superview} Load ConstraintLayout IntrinsicContentSize: Bounds={this.Bounds} Widget={MLayoutWidget}");
                return this.Bounds.Size;
            }
        }

        public CGSize MeasureSubviews()
        {
            Size availableSize;
            if (this.Frame.Size.Width > 0)//取自身的大小作为availableSize
            {
                //作为ConstraintLayout的子ConstraintLayout是肯定有大小的,而作为根布局也有大小
                availableSize = this.Frame.Size;
                if (DEBUG) Debug.WriteLine($"{this.GetType().Name} MeasureSubviews: Use Frame Size={availableSize}");
            }
            else
            {
                //没有被父布局设置大小时,可能是使用了原生布局,也可能是使用了AutoLayout约束大小
                availableSize = this.SystemLayoutSizeFittingSize(UIView.UILayoutFittingCompressedSize); ;
                if (availableSize.Width <= 0 || availableSize.Height <= 0)//如果父布局未设置且未使用AutoLayout设置,那么取父布局的大小
                {
                    //search all superview, until superview's size is not zero
                    var superview = this.Superview;
                    while (superview != null)
                    {
                        if (superview.Frame.Size.Width > 1 || superview.Frame.Size.Height > 1)//float maybe is 0.00~, so i use 1, layout should't be 1dp or 1px
                        {
                            availableSize = superview.Frame.Size;
                            break;
                        }
                        superview = superview.Superview;
                    }
                    if (DEBUG) Debug.WriteLine($"{this.GetType().Name} MeasureSubviews: Use Parent={superview} Size={availableSize}");
                }
                else
                    if (DEBUG) Debug.WriteLine($"{this.GetType().Name} MeasureSubviews: Use SystemLayoutSizeFittingSize Size={availableSize}");

            }

            (int horizontalSpec, int verticalSpec) = MakeSpec(this, availableSize);

            return MeasureLayout(availableSize, horizontalSpec, verticalSpec);
        }

        /// <summary>
        /// 判断是否可以无限大小,作为UIScrollView的Child时可能.
        /// </summary>
        /// <returns></returns>
        public (bool isInfinityAvailabelWidth, bool isInfinityAvailabelHeight) IsInfinitable(ConstraintLayout layout, int constrainWidth, int constrainHeight, Size availableSize)
        {
            bool isInfinityAvailabelWidth = false;
            bool isInfinityAvailabelHeight = false;
            if (layout.Superview is UIScrollView)//UIScrollView的内容可以是无限值,ConstraintLayout作为其子View时,只有在WrapContent时才有无限值
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

        /*
        * layoutSubviews调用机制(链接：https://www.jianshu.com/p/915c7cc0e959)
        * 1 直接调用setLayoutSubviews。
        * 2 addSubview的时候触发layoutSubviews。
        * 3 当view的frame发生改变的时候触发layoutSubviews。
        * 4 第一次滑动UIScrollView的时候触发layoutSubviews。
        * 5 旋转Screen会触发父UIView上的layoutSubviews事件。
        * 6 改变一个UIView大小的时候也会触发父UIView上的layoutSubviews事件。
        */

        /// <summary>
        /// iOS don't have measure method, but at first show, it will load LayoutSubviews twice,
        /// that means we can get child size at second time layout.
        /// </summary>
        public override void LayoutSubviews()
        {
            if (DEBUG) Debug.WriteLine($"{this.GetType().Name} {nameof(LayoutSubviews)} Start: Frame={this.Frame} Parent={Superview.GetType().Name} Parents' Frame={this.Superview?.Frame}");

            base.LayoutSubviews();

            var finalSize = MeasureSubviews();

            if ((int)this.Frame.Width != (int)finalSize.Width || (int)this.Frame.Height != (int)finalSize.Height)//如果不一致代表父布局会获取到错误的大小,所以重新设置,这里不会刷新父布局
            {
                //更新layout的大小, Layout不指定自身位置
                this.Bounds = new CGRect(this.Bounds.Location, finalSize);
                InvalidateIntrinsicContentSize();//参考https://www.jianshu.com/p/9dd2cfa10ff9,IntrinsicContentSize变化时需要调用
            }

            ArrangeLayout();

            if (DEBUG) Debug.WriteLine($"{this.GetType().Name} {nameof(LayoutSubviews)} Finish: Frame={this.Frame} Bounds={this.Bounds} Widget={this.MLayoutWidget.ToString()}");
        }

        private void LayoutChild(UIElement element, int x, int y, int w, int h)
        {
            //参考https://github.com/youngsoft/MyLinearLayout/blob/master/MyLayout/Lib/MyBaseLayout.m,设置Bounds
            //element.Bounds = new CoreGraphics.CGRect(element.Bounds.X, element.Bounds.Y, w, h);
            //element.Center = new CoreGraphics.CGPoint((x + w / 2) + 0.5, y + h / 2);

            //探讨设置Frame还是Bounds
            //Frame和Bounds在旋转View时是不同的大小,我们使用IntrinsicContentSize得到控件大小,那么IntrinsicContentSize是谁的大小呢?
            //参考https://zhangbuhuai.com/post/auto-layout-part-1.html,这里好像暗示使用Frame去布局
            element.Frame = new CoreGraphics.CGRect(x, y, w, h);
        }

#endregion

    }
}
#endif
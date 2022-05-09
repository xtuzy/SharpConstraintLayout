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

namespace SharpConstraintLayout.Maui.Widget
{
    public partial class ConstraintLayout
    {
        public ConstraintLayout()
        {
            init();
        }

        #region Add And Remove

        public UIElement GetChildAt(int index)
        {
            return Subviews[index];
        }

        public override void AddSubview(UIElement view)
        {
            base.AddSubview(view);
            OnAddedView(view);
        }

        /// <summary>
        /// Same as <see cref="AddSubview(UIElement)"/>
        /// </summary>
        /// <param name="element"></param>
        public void AddElement(UIElement element)
        {
            base.AddSubview(element);
            OnAddedView(element);
        }

        public void RemoveView(UIElement element)
        {
            element.RemoveFromSuperview();
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

        public void RemoveAllViews()
        {
            foreach (var element in this.Subviews)
            {
                element.RemoveFromSuperview();
                OnRemovedView(element);
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
        public override void UpdateConstraints()
        {
            if (DEBUG) Debug.WriteLine($"{Superview} Load ConstraintLayout UpdateConstraints");

            base.UpdateConstraints();
        }

        public override CGRect Frame
        {
            get
            {
                return base.Frame;
            }
            set => base.Frame = value;
        }
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
                if (DEBUG) Debug.WriteLine($"{Superview} Load ConstraintLayout IntrinsicContentSize:Bounds {this.Bounds}");
                if (this.Bounds.Size.Width <= 5 && isFirstTimeMeasure)
                {
                    isFirstTimeMeasure = false;
                    this.Bounds = new CGRect(this.Bounds.Location, MeasureSubviews());
                }
                return this.Bounds.Size;
            }
        }

        public CGSize MeasureSubviews()
        {
            /*
            * layoutSubviews调用机制(链接：https://www.jianshu.com/p/915c7cc0e959)
            * 1 直接调用setLayoutSubviews。
            * 2 addSubview的时候触发layoutSubviews。
            * 3 当view的frame发生改变的时候触发layoutSubviews。
            * 4 第一次滑动UIScrollView的时候触发layoutSubviews。
            * 5 旋转Screen会触发父UIView上的layoutSubviews事件。
            * 6 改变一个UIView大小的时候也会触发父UIView上的layoutSubviews事件。
           */
            Size avaliableSize;
            if (this.Frame.Size.Width > 0)//取自身的大小作为availableSize
            {
                //作为ConstraintLayout的子ConstraintLayout是肯定有大小的,而作为根布局也有大小
                avaliableSize = this.Frame.Size;
                if (Superview is UIScrollView && avaliableSize.Height == 0)//如果是UIScrollView并且高度为0,则代表高度可以无限值
                {
                    isInfinityAvailabelHeight = true;
                    avaliableSize.Height = int.MaxValue;
                }
                if (DEBUG) Debug.WriteLine($"{this.GetType().Name} MeasureSubviews: Use Frame size={avaliableSize}");
            }
            else
            {
                //没有被父布局设置大小时,可能是使用了原生布局,也可能是使用了AutoLayout约束大小,如果都获取不到,那么取父布局的大小
                avaliableSize = base.SystemLayoutSizeFittingSize(UIView.UILayoutFittingCompressedSize); ;
                if (avaliableSize.Width <= 0 || avaliableSize.Height <= 0)
                {
                    //search all superview, until superview's size is not zero
                    var superview = this.Superview;
                    while (superview != null)
                    {
                        if (superview.Frame.Size.Width > 1 || superview.Frame.Size.Height > 1)//float maybe is 0.00~, so i use 1, layout should't be 1dp or 1px
                        {
                            avaliableSize = superview.Frame.Size;
                            break;
                        }
                        superview = superview.Superview;
                    }
                    if (DEBUG) Debug.WriteLine($"{this.GetType().Name} MeasureSubviews: Use {superview} size={avaliableSize}");
                }
                else
                    if (DEBUG) Debug.WriteLine($"{this.GetType().Name} MeasureSubviews: Use SystemLayoutSizeFittingSize size={avaliableSize}");

            }

            return MeasureLayout(avaliableSize);
        }

        /// <summary>
        /// iOS don't have measure method, but at first show, it will load LayoutSubviews twice,
        /// that means we can get child size at second time layout.
        /// </summary>
        public override void LayoutSubviews()
        {
            if (DEBUG) Debug.WriteLine($"{this.GetType().FullName} {nameof(LayoutSubviews)} Start: Frame={this.Frame} Parent={Superview.GetType().FullName} Parents' Frame={this.Superview?.Frame}");

            base.LayoutSubviews();

            //更新layout的大小, Layout不指定自身位置
            var finalSize = MeasureSubviews();
            if ((int)this.Frame.Width != (int)finalSize.Width || (int)this.Frame.Height != (int)finalSize.Height)//如果不一致代表父布局会获取到错误的大小,所以重新设置,这里不会刷新父布局
            {
                this.Bounds = new CGRect(this.Bounds.Location, finalSize);
            }
            ArrangeLayout();

            if (DEBUG) Debug.WriteLine($"{this.GetType().FullName} {nameof(LayoutSubviews)} Finish: Frame={this.Frame} Widget={this.MLayoutWidget.ToString()}");
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
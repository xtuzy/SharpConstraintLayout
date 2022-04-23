using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if WINDOWS
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
#elif __IOS__
using Panel = UIKit.UIView;
using UIElement = UIKit.UIView;
using CoreGraphics;
using Size = CoreGraphics.CGSize;
using Microsoft.Maui.Graphics;
#elif __ANDROID__
#endif
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

        public void AddView(UIElement element)
        {
            base.AddSubview(element);
            OnAddedView(element);
        }

        public void RemoveView(UIElement element)
        {
            element.RemoveFromSuperview();
            OnRemovedView(element);
        }

        public void RemoveAllViews()
        {
            foreach (var element in this.Subviews)
            {
                element.RemoveFromSuperview();
                OnRemovedView(element);
            }
        }

        public int ChildCount => Subviews.Length;

        #endregion

        #region Layout

        //public override Size IntrinsicContentSize => this.Frame.Size;

        /// <summary>
        /// iOS don't have measure method, but at first show, it will load LayoutSubviews twice,
        /// that means we can get child size at second time layout.
        /// </summary>
        public override void LayoutSubviews()
        {
            //base.LayoutSubviews();
            if (DEBUG) Debug.WriteLine($"{this.GetType().FullName} {nameof(LayoutSubviews)} Frame={this.Frame} Parent={Superview.GetType().FullName} Parents' Frame={this.Superview?.Frame}");

            /*
             * layoutSubviews调用机制(链接：https://www.jianshu.com/p/915c7cc0e959)
             * 1 直接调用setLayoutSubviews。
             * 2 addSubview的时候触发layoutSubviews。
             * 3 当view的frame发生改变的时候触发layoutSubviews。
             * 4 第一次滑动UIScrollView的时候触发layoutSubviews。
             * 5 旋转Screen会触发父UIView上的layoutSubviews事件。
             * 6 改变一个UIView大小的时候也会触发父UIView上的layoutSubviews事件。
            */

            if (this.Frame.Size.Width > 0)//取自身的大小作为availableSize
            {
                //作为ConstraintLayout的子ConstraintLayout是肯定有大小的,而作为根布局也有大小
                Measure(this.Frame.Size);
            }
            else
            {
                //没有被父布局设置大小时,可能是使用了原生布局,也可能是使用了AutoLayout约束大小,如果都获取不到,那么取父布局的大小
                (var w, var h) = this.GetWrapContentSize();
                if (w <= 0 || h <= 0)
                {
                    if (Superview != null)
                        Measure(this.Superview.Frame.Size);
                }
                else
                {
                    Measure(new Size(w, h));
                }
            }

            if (DEBUG) Debug.WriteLine($"{this.GetType().FullName} {nameof(LayoutSubviews)} Widget={this.MLayoutWidget.ToString()}");

            //更新layout的大小, Layout不指定自身位置
            if (this.Frame.Width != MLayoutWidget.Width || this.Frame.Height != MLayoutWidget.Height)
                this.Bounds = new CGRect(this.Bounds.X, this.Bounds.Y, MLayoutWidget.Width, MLayoutWidget.Height);

            OnLayout();
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

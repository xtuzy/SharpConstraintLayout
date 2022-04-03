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
        #region Add And Remove
        
        internal UIElement[] Children => Subviews;

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
        
        public override Size IntrinsicContentSize => this.Frame.Size;

        /// <summary>
        /// iOS don't have measure method, but at first show, it will load LayoutSubviews twice,
        /// that means we can get child size at second time layout.
        /// </summary>
        public override void LayoutSubviews()
        {
            //base.LayoutSubviews();
            if (DEBUG) Debug.WriteLine($"{nameof(LayoutSubviews)} {this} {this.Frame}");
            if (DEBUG) Debug.WriteLine($"{nameof(LayoutSubviews)} {Superview} {this.Superview?.Frame}");

            //得到自身或Superview的大小作为availableSize
            //if (this.Frame.Size.Width > 0)
            //{
            //    MeasureOverride(this.Frame.Size);
            //}
            //else
            {
                if (Superview != null)
                    Measure(this.Superview.Frame.Size);
            }

            if (DEBUG) Debug.WriteLine($"{nameof(LayoutSubviews)} RootWidget {this.MLayoutWidget.ToString()}");

            //更新layout的大小
            if (this.Frame.Size.Width > 0)
            {
                Frame = new CGRect(Frame.X, Frame.Y, MLayoutWidget.Width, MLayoutWidget.Height);
            }
            else
            {
                if (Superview != null)
                    Frame = new CGRect(this.Superview.Frame.X, this.Superview.Frame.Y, MLayoutWidget.Width, MLayoutWidget.Height);
            }

            OnLayout();
        }

        private void LayoutChild(UIElement element, int x, int y, int w, int h)
        {
            element.Frame = new CoreGraphics.CGRect(x, y, w, h);
        }

        #endregion
    }
}

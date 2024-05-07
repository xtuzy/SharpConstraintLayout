/*
 * Copyright (C) 2018 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
#if __MAUI__
using SharpConstraintLayout.Maui.Widget.Interface;
using FrameworkElement = Microsoft.Maui.Controls.View;
using UIElement = Microsoft.Maui.Controls.View;
#elif WINDOWS
using Microsoft.UI.Xaml;
using SharpConstraintLayout.Maui.Widget.Interface;
using System.Diagnostics;
using UIElement = Microsoft.UI.Xaml.UIElement;
#elif __IOS__
using SharpConstraintLayout.Maui.Widget.Interface;
using System;
using System.Diagnostics;
using UIElement = UIKit.UIView;
#elif __ANDROID__
using Android.Content;
using SharpConstraintLayout.Maui.Widget.Interface;
using UIElement = Android.Views.View;
#endif
using Microsoft.Extensions.Logging;
namespace SharpConstraintLayout.Maui.Widget
{

    /// <summary>
    ///  <b>Added in 2.0</b>
    ///  <para>
    ///  </para>
    ///  @suppress
    /// </summary>
    public abstract class VirtualLayout : ConstraintHelper, IVirtualLayout
    {
        protected ILogger Logger { get; set; }
        private bool mApplyVisibilityOnAttach;
        private bool mApplyElevationOnAttach;

#if __ANDROID__ && !__MAUI__
        protected VirtualLayout(Context context) : base(context)
#else
        protected VirtualLayout() : base()
#endif
        {
        }

        protected override void init()
        {
            base.init();
        }

#if __MAUI__
        /// <summary>
        /// store scaled pixel during measure.
        /// </summary>
        SizeI MeasuredSize;
#endif
        public virtual void onMeasure(androidx.constraintlayout.core.widgets.VirtualLayout layout, int widthMeasureSpec, int heightMeasureSpec)
        {
            // nothing
        }

        /// <summary>
        /// return scaled pixel.
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public SizeI GetMeasuredSize(UIElement child)
        {
#if __MAUI__
            return (child as VirtualLayout).MeasuredSize;
#elif __IOS__ && !__MAUI__
            return new SizeI(UIElementExtension.DpToScaledPx(child.Bounds.Width), UIElementExtension.DpToScaledPx(child.Bounds.Height));//我在iOS的Flow中,将测量值存储到了Bounds
#elif WINDOWS && !__MAUI__
            return new SizeI(UIElementExtension.DpToScaledPx((child as FrameworkElement).Width), UIElementExtension.DpToScaledPx((child as FrameworkElement).Height));
#elif __ANDROID__
            var size = child.GetWrapContentSize();
            return new SizeI(size.Width, size.Height);
#endif
        }

        /// <summary>
        /// params use scaled pixel. it be use to store measured size by platform way.
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public void SetMeasuredSize(int w, int h)
        {
#if __MAUI__
            // Setting WidthRequest and HeightRequest during measurement will causes a measurement loop.
            //this.WidthRequest = layout.Width;
            //this.HeightRequest = layout.Height;

            MeasuredSize = new SizeI(w, h);
#elif __ANDROID__ && !__MAUI__
            SetMeasuredDimension(UIElementExtension.DpToPx(UIElementExtension.ScaledPxToDp(w)), UIElementExtension.DpToPx(UIElementExtension.ScaledPxToDp(h)));//Android中这个的作用应该是设置flow的大小
#elif __IOS__ && !__MAUI__
            this.Bounds = new CoreGraphics.CGRect(this.Bounds.X, this.Bounds.Y, UIElementExtension.ScaledPxToDp(w), UIElementExtension.ScaledPxToDp(h));
#elif WINDOWS && !__MAUI__
            this.Width = UIElementExtension.ScaledPxToDp(w);
            this.Height = UIElementExtension.ScaledPxToDp(h);
#endif
        }

        /// <summary>
        /// @suppress
        /// </summary>
        protected override void WhenAttachedToWindow()
        {
            base.WhenAttachedToWindow();
            if (mApplyVisibilityOnAttach || mApplyElevationOnAttach)
            {
                var parent = this.GetParent();
                if (parent == null) Logger?.LogInformation(this.GetType().Name, "Parent is null, maybe something is false.");

                if (parent is ConstraintLayout)
                {
                    ConstraintLayout container = (ConstraintLayout)parent;
                    int visibility = ConstrainVisibility;
                    float elevation = 0;

                    for (int i = 0; i < mCount; i++)
                    {
                        int id = mIds[i];
                        var view = container.FindElementById(id);
                        if (view != null)
                        {
                            if (mApplyVisibilityOnAttach)
                            {
                                ConstrainVisibility = visibility;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ConstraintSet.VISIBLE,GONE,INVISIBLE
        /// </summary>
        int visible;
        /// <summary>
        /// @suppress
        /// </summary>
        public override int ConstrainVisibility
        {
            set
            {
                base.ConstrainVisibility = value;
                applyLayoutFeatures();
            }

            get
            {
                return visible;
            }
        }

        /// <summary>
        /// @suppress </summary>
        /// <param name="container"> </param>
        protected internal override void applyLayoutFeaturesInConstraintSet(ConstraintLayout container)
        {
            applyLayoutFeatures(container);
        }
    }
}
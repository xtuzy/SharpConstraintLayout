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
using Microsoft.Extensions.Logging;
using SharpConstraintLayout.Maui.Widget.Interface;
using FrameworkElement = Microsoft.Maui.Controls.View;
using UIElement = Microsoft.Maui.Controls.View;

#elif WINDOWS
using Microsoft.UI.Xaml;
using SharpConstraintLayout.Maui.Widget.Interface;
using System.Diagnostics;
using FrameworkElement = Microsoft.UI.Xaml.FrameworkElement;
#elif __IOS__
using SharpConstraintLayout.Maui.Widget.Interface;
using System;
using System.Diagnostics;
using FrameworkElement = UIKit.UIView;
#elif __ANDROID__
using Android.Content;
using SharpConstraintLayout.Maui.Widget.Interface;
using FrameworkElement = Android.Views.View;
#endif
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
        protected ILogger Logger {  get; set; }
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
        public Size MeasuredSize;//创建这个变量专门存储Widget测量值
#endif
        public virtual void onMeasure(androidx.constraintlayout.core.widgets.VirtualLayout layout, int widthMeasureSpec, int heightMeasureSpec)
        {
            // nothing
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
                        FrameworkElement view = (FrameworkElement)container.FindElementById(id);
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
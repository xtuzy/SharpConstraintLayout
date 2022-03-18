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
#if WINDOWS
using Microsoft.UI.Xaml;
using View = Microsoft.UI.Xaml.FrameworkElement;
#elif __IOS__
using System;
using View = UIKit.UIView;
#endif
namespace SharpConstraintLayout.Maui.Widget
{

    /// <summary>
    ///  <b>Added in 2.0</b>
    ///  <para>
    ///  </para>
    ///  @suppress
    /// </summary>
    public abstract class VirtualLayout : ConstraintHelper
    {
        private bool mApplyVisibilityOnAttach;
        private bool mApplyElevationOnAttach;

        public VirtualLayout() : base()
        {
        }

        protected internal  void init()
        {
            base.init();
        }

        public virtual void onMeasure(androidx.constraintlayout.core.widgets.VirtualLayout layout, int widthMeasureSpec, int heightMeasureSpec)
        {
            // nothing
        }

        /// <summary>
        /// @suppress
        /// </summary>
        public override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            if (mApplyVisibilityOnAttach || mApplyElevationOnAttach)
            {
#if WINDOWS
                object parent = Parent;
#elif __IOS__
                object parent = Superview;
#endif
                if (parent is ConstraintLayout)
                {
                    ConstraintLayout container = (ConstraintLayout) parent;
                    int visibility = Visible;
                    float elevation = 0;
#if __ANDROID__
                    if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.LOLLIPOP)
                    {
                        elevation = Elevation;
                    }
#endif
                    for (int i = 0; i < mCount; i++)
                    {
                        int id = mIds[i];
                        View view = (View)container.getViewById(id);
                        if (view != null)
                        {
                            if (mApplyVisibilityOnAttach)
                            {
                                Visible = visibility;
                            }
#if __ANDROID__
                            if (mApplyElevationOnAttach)
                            {
                                if (elevation > 0 && android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.LOLLIPOP)
                                {
                                    view.TranslationZ = view.TranslationZ + elevation;
                                }
                            }
#endif
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
        public override int Visible
        {
            set
            {
                base.Visible= value;
                applyLayoutFeatures();
            }

            get
            {
                 return visible;
            }
        }

#if __ANDROID__
        float elevation;
        /// <summary>
        /// @suppress
        /// </summary>
        public float Elevation
        {
            set
            {
                //base.Elevation = value;
                elevation = value;
                applyLayoutFeatures();
            }

            get
            {
                return elevation;
            }
        }
#endif

        /// <summary>
        /// @suppress </summary>
        /// <param name="container"> </param>
        protected internal override void applyLayoutFeaturesInConstraintSet(ConstraintLayout container)
        {
            applyLayoutFeatures(container);
        }
    }
}
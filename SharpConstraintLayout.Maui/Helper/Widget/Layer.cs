#if __TODO__
using SharpConstraintLayout.Maui.Widget;
using System;

/*
 * Copyright (C) 2021 The Android Open Source Project
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
    using View = Microsoft.UI.Xaml.FrameworkElement;
    using UIElement = Microsoft.UI.Xaml.UIElement;

    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Windows.Foundation;
#elif __IOS__
using View = UIKit.UIView;
using UIElement = UIKit.UIView;
#endif
namespace SharpConstraintLayout.Maui.Helper.Widget
{
    using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;

    /// <summary>
    /// Layer adds the ability to move and rotate a group of views as if they were contained in a viewGroup
    /// <b>Added in 2.0</b>
    /// Methods such as setRotation(float) rotate all views about a common center.
    /// For simple visibility manipulation use Group
    /// 
    /// </summary>
    public class Layer : ConstraintHelper
    {
        private const string TAG = "Layer";
        private float mRotationCenterX = float.NaN;
        private float mRotationCenterY = float.NaN;
        private float mGroupRotateAngle = float.NaN;
        internal ConstraintLayout mContainer;
        private float mScaleX = 1;
        private float mScaleY = 1;
        protected internal float mComputedCenterX = float.NaN;
        protected internal float mComputedCenterY = float.NaN;

        protected internal float mComputedMaxX = float.NaN;
        protected internal float mComputedMaxY = float.NaN;
        protected internal float mComputedMinX = float.NaN;
        protected internal float mComputedMinY = float.NaN;
        internal bool mNeedBounds = true;
        internal View[] mViews = null; // used to reduce the getViewById() cost
        private float mShiftX = 0;
        private float mShiftY = 0;

        private bool mApplyVisibilityOnAttach;
        private bool mApplyElevationOnAttach;

        public Layer() : base()
        {
        }

        protected internal override void init()
        {
            base.init();
            mUseViewMeasure = false;
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            mContainer = (ConstraintLayout)this.GetParent();
            if (mApplyVisibilityOnAttach || mApplyElevationOnAttach)
            {
                int visibility = Visibility;
                float elevation = 0;
                if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.LOLLIPOP)
                {
                    elevation = Elevation;
                }
                for (int i = 0; i < mCount; i++)
                {
                    int id = mIds[i];
                    View view = mContainer.FindViewById(id);
                    if (view != null)
                    {
                        if (mApplyVisibilityOnAttach)
                        {
                            view.Visibility = visibility;
                        }
                        if (mApplyElevationOnAttach)
                        {
                            if (elevation > 0 && android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.LOLLIPOP)
                            {
                                view.TranslationZ = view.TranslationZ + elevation;
                            }
                        }
                    }

                }
            }
        }

        /// <param name="container">
        /// @suppress </param>
        public override void UpdatePreDraw(ConstraintLayout container)
        {
            mContainer = container;
            float rotate = Rotation;
            if (rotate == 0)
            {
                if (!float.IsNaN(mGroupRotateAngle))
                {
                    mGroupRotateAngle = rotate;
                }
            }
            else
            {
                mGroupRotateAngle = rotate;
            }
        }

        /// <summary>
        /// Rotates all associated views around a single point post layout..
        /// The point is the middle of the bounding box or set by setPivotX,setPivotX; </summary>
        /// <param name="angle"> </param>
        public override float Rotation
        {
            set
            {
                mGroupRotateAngle = value;
                transform();
            }
        }
        /// <summary>
        /// Scales all associated views around a single point post layout..
        /// The point is the middle of the bounding box or set by setPivotX,setPivotX; </summary>
        /// <param name="scaleX"> The value to scale in X. </param>
        public override float ScaleX
        {
            set
            {
                mScaleX = value;
                transform();
            }
        }

        /// <summary>
        /// Scales all associated views around a single point post layout..
        /// The point is the middle of the bounding box or set by setPivotX,setPivotX; </summary>
        /// <param name="scaleY"> The value to scale in X. </param>
        public override float ScaleY
        {
            set
            {
                mScaleY = value;
                transform();
            }
        }

        /// <summary>
        /// Sets the pivot point for scale operations.
        /// Setting it to Float.NaN (default) results in the center of the group being used. </summary>
        /// <param name="pivotX"> The X location of the pivot point </param>
        public override float PivotX
        {
            set
            {
                mRotationCenterX = value;
                transform();
            }
        }

        /// <summary>
        /// Sets the pivot point for scale operations.
        /// Setting it to Float.NaN (default) results in the center of the group being used. </summary>
        /// <param name="pivotY"> The Y location of the pivot point </param>
        public override float PivotY
        {
            set
            {
                mRotationCenterY = value;
                transform();
            }
        }

        /// <summary>
        /// Shift all the views in the X direction post layout. </summary>
        /// <param name="dx"> number of pixes to shift </param>
        public override float TranslationX
        {
            set
            {
                mShiftX = value;
                transform();

            }
        }
        /// <summary>
        /// Shift all the views in the Y direction post layout. </summary>
        /// <param name="dy"> number of pixes to shift </param>
        public override float TranslationY
        {
            set
            {
                mShiftY = value;
                transform();
            }
        }

        /// <summary>
        /// @suppress
        /// </summary>
        public override int Visibility
        {
            set
            {
                base.Visibility = value;
                applyLayoutFeatures();
            }
        }

        /// <summary>
        /// @suppress
        /// </summary>
        public override float Elevation
        {
            set
            {
                base.Elevation = value;
                applyLayoutFeatures();
            }
        }

        /// <param name="container">
        /// @suppress </param>
        public override void UpdatePostLayout(ConstraintLayout container)
        {
            reCacheViews();

            mComputedCenterX = Float.NaN;
            mComputedCenterY = Float.NaN;
            ConstraintLayout.LayoutParams @params = (ConstraintLayout.LayoutParams)LayoutParams;
            ConstraintWidget widget = @params.ConstraintWidget;
            widget.Width = 0;
            widget.Height = 0;
            calcCenters();
            int left = (int)mComputedMinX - PaddingLeft;
            int top = (int)mComputedMinY - PaddingTop;
            int right = (int)mComputedMaxX + PaddingRight;
            int bottom = (int)mComputedMaxY + PaddingBottom;
            layout(left, top, right, bottom);
            transform();
        }

        private void reCacheViews()
        {
            if (mContainer == null)
            {
                return;
            }
            if (mCount == 0)
            {
                return;
            }

            if (mViews == null || mViews.Length != mCount)
            {
                mViews = new View[mCount];
            }
            for (int i = 0; i < mCount; i++)
            {
                int id = mIds[i];
                mViews[i] = mContainer.FindViewById(id);
            }
        }

        protected internal virtual void calcCenters()
        {
            if (mContainer == null)
            {
                return;
            }
            if (!mNeedBounds)
            {
                if (!(float.IsNaN(mComputedCenterX) || float.IsNaN(mComputedCenterY)))
                {
                    return;
                }
            }
            if (float.IsNaN(mRotationCenterX) || float.IsNaN(mRotationCenterY))
            {
                View[] views = getViews(mContainer);

                int minx = views[0].Left;
                int miny = views[0].Top;
                int maxx = views[0].Right;
                int maxy = views[0].Bottom;

                for (int i = 0; i < mCount; i++)
                {
                    View view = views[i];
                    minx = Math.Min(minx, view.Left);
                    miny = Math.Min(miny, view.Top);
                    maxx = Math.Max(maxx, view.Right);
                    maxy = Math.Max(maxy, view.Bottom);
                }

                mComputedMaxX = maxx;
                mComputedMaxY = maxy;
                mComputedMinX = minx;
                mComputedMinY = miny;

                if (float.IsNaN(mRotationCenterX))
                {
                    mComputedCenterX = (minx + maxx) / 2;
                }
                else
                {
                    mComputedCenterX = mRotationCenterX;
                }
                if (float.IsNaN(mRotationCenterY))
                {
                    mComputedCenterY = (miny + maxy) / 2;

                }
                else
                {
                    mComputedCenterY = mRotationCenterY;
                }

            }
            else
            {
                mComputedCenterY = mRotationCenterY;
                mComputedCenterX = mRotationCenterX;
            }

        }

        private void transform()
        {
            if (mContainer == null)
            {
                return;
            }
            if (mViews == null)
            {
                reCacheViews();
            }
            calcCenters();

            double rad = (float.IsNaN(mGroupRotateAngle)) ? 0.0 : Math.toRadians(mGroupRotateAngle);
            float sin = (float)Math.Sin(rad);
            float cos = (float)Math.Cos(rad);
            float m11 = mScaleX * cos;
            float m12 = -mScaleY * sin;
            float m21 = mScaleX * sin;
            float m22 = mScaleY * cos;

            for (int i = 0; i < mCount; i++)
            {
                View view = mViews[i];
                int x = (view.Left + view.Right) / 2;
                int y = (view.Top + view.Bottom) / 2;
                float dx = x - mComputedCenterX;
                float dy = y - mComputedCenterY;
                float shiftx = m11 * dx + m12 * dy - dx + mShiftX;
                float shifty = m21 * dx + m22 * dy - dy + mShiftY;

                view.TranslationX = shiftx;
                view.TranslationY = shifty;
                view.ScaleY = mScaleY;
                view.ScaleX = mScaleX;
                if (!float.IsNaN(mGroupRotateAngle))
                {
                    view.Rotation = mGroupRotateAngle;
                }
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
#endif
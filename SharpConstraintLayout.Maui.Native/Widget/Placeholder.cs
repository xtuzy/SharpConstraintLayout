/*
 * Copyright (C) 2017 The Android Open Source Project
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
using SharpConstraintLayout.Maui.Widget.Interface;
using System;
using System.Diagnostics;

namespace SharpConstraintLayout.Maui.Widget
{
#if WINDOWS
    using FrameworkElement = Microsoft.UI.Xaml.FrameworkElement;
    using UIElement = Microsoft.UI.Xaml.UIElement;
#elif __IOS__
    using FrameworkElement = UIKit.UIView;
    using UIElement = UIKit.UIView;
#elif __ANDROID__
    using Android.Content;
    using FrameworkElement = Android.Views.View;
    using UIElement = Android.Views.View;
#elif __MAUI__
    using FrameworkElement = Microsoft.Maui.Controls.View;
    using UIElement = Microsoft.Maui.Controls.View;
#endif
    using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;
    using IPlaceholder = Interface.IPlaceholder;

    /// <summary>
    /// <b>Added in 1.1</b>
    /// <para>
    /// A {@code Placeholder} provides a virtual object which can position an existing object.
    /// </para>
    /// <para>
    /// When the id of another view is set on a placeholder (using {@code setContent()}),
    /// the placeholder effectively becomes the content view. If the content view exist on the
    /// screen it is treated as gone from its original location.
    /// </para>
    /// <para>
    /// The content view is positioned using the layout of the parameters of the {@code Placeholder}  (the {@code Placeholder}
    /// is simply constrained in the layout like any other view).
    /// </para>
    /// 
    /// </summary>
    public partial class Placeholder : FrameworkElement, IPlaceholder
    {
        bool InEditMode = false;

        private int mContentId = -1;
        private FrameworkElement mContent = null;
        private int mEmptyVisibility = ConstraintSet.Invisible;
#if __ANDROID__
        public Placeholder(Context context) : base(context)
#else
        public Placeholder()
#endif
        {
            init();
        }

        private void init()
        {
            UIElementExtension.SetViewVisibility(this, mEmptyVisibility);
            mContentId = -1;
        }

        /// <summary>
        /// Sets the visibility of placeholder when not containing objects typically gone or invisible.
        /// This can be important as it affects behavior of surrounding components.
        /// </summary>
        /// <param name="visibility"> Either View.VISIBLE, View.INVISIBLE, View.GONE </param>
        public virtual int EmptyVisibility
        {
            set
            {
                mEmptyVisibility = value;
            }
            get
            {
                return mEmptyVisibility;
            }
        }

        /// <summary>
        /// Returns the content view </summary>
        /// <returns>return null if no content is set, otherwise the content view </returns>
        public virtual FrameworkElement Content
        {
            get
            {
                return mContent;
            }
        }

        /* /// <summary>
         /// Placeholder does not draw anything itself - therefore Paint and Rect allocations
         /// are fine to suppress and ignore.
         /// 
         /// @suppress </summary>
         /// <param name="canvas"> </param>
         public virtual void onDraw(Canvas canvas)
         {
             if (InEditMode)
             {
                 canvas.drawRGB(223, 223, 223);

                 //ORIGINAL LINE: @SuppressLint("DrawAllocation") android.graphics.Paint paint = new android.graphics.Paint();
                 Paint paint = new Paint();
                 paint.setARGB(255, 210, 210, 210);
                 paint.TextAlign = Paint.Align.CENTER;
                 paint.Typeface = Typeface.create(Typeface.DEFAULT, Typeface.NORMAL);

                 //ORIGINAL LINE: @SuppressLint("DrawAllocation") android.graphics.Rect r = new android.graphics.Rect();
                 Rect r = new Rect();
                 canvas.getClipBounds(r);
                 paint.TextSize = r.height();
                 int cHeight = r.height();
                 int cWidth = r.width();
                 paint.TextAlign = Paint.Align.LEFT;
                 string text = "?";
                 paint.getTextBounds(text, 0, text.Length, r);
                 float x = cWidth / 2f - r.width() / 2f - r.left;
                 float y = cHeight / 2f + r.height() / 2f - r.bottom;
                 canvas.drawText(text, x, y, paint);
             }
         }*/

        public virtual void UpdatePreLayout(ConstraintLayout container)
        {
            if (mContentId == -1)
            {
                if (!InEditMode)
                {
                    //Visibility = (Microsoft.UI.Xaml.Visibility)mEmptyVisibility;
                    UIElementExtension.SetViewVisibility(this, mEmptyVisibility);
                }
            }

            mContent = (FrameworkElement)container.FindElementById(mContentId);
            if (mContent != null)
            {
                //ConstraintLayout.LayoutParams layoutParamsContent = (ConstraintLayout.LayoutParams)mContent.LayoutParams;
                //layoutParamsContent.isInPlaceholder = true;
                container.mConstraintSet.Constraints[mContent.GetId()].layout.isInPlaceholder = true;

                UIElementExtension.SetViewVisibility(mContent, ConstraintSet.Visible);
                UIElementExtension.SetViewVisibility(this, ConstraintSet.Visible);
            }
        }

        /// <summary>
        /// Sets the content view id
        /// </summary>
        /// <param name="id"> the id of the content view we want to place in the Placeholder </param>
        public virtual void SetContentId(int value)
        {
            if (mContentId == value)
            {
                return;
            }
            if (mContent != null)
            {

                //ConstraintLayout.LayoutParams layoutParamsContent = (ConstraintLayout.LayoutParams)mContent.LayoutParams;
                //layoutParamsContent.isInPlaceholder = false;
                UIElementExtension.SetViewVisibility(mContent, ConstraintSet.Visible);
                ((mContent.GetParent()) as ConstraintLayout).mConstraintSet.Constraints[mContent.GetId()].layout.isInPlaceholder = true;

                mContent = null;
            }

            mContentId = value;
            if (value != ConstraintSet.Unset)
            {
                UIElement v = ((ConstraintLayout)this.GetParent()).FindElementById(value);
                if (v != null)
                {
                    UIElementExtension.SetViewVisibility(v, ConstraintSet.Gone);
                }
            }
        }

        public virtual void UpdatePostMeasure(ConstraintLayout container)
        {
            if (mContent == null)
            {
                return;
            }

            var layoutParams = container.GetWidgetByElement(this);
            var layoutParamsContent = container.GetWidgetByElement(mContent);

            layoutParamsContent.Visibility = ConstraintSet.Visible;

            if (layoutParams.HorizontalDimensionBehaviour != ConstraintWidget.DimensionBehaviour.FIXED)
            {
                layoutParams.Width = layoutParamsContent.Width;
            }
            if (layoutParams.VerticalDimensionBehaviour != ConstraintWidget.DimensionBehaviour.FIXED)
            {
                layoutParams.Height = layoutParamsContent.Height;
            }
            layoutParamsContent.Visibility = ConstraintSet.Gone;
        }
    }
}
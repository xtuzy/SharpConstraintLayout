/*
 * Copyright (C) 2015 The Android Open Source Project
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
#if WINDOWS
using FrameworkElement = Microsoft.UI.Xaml.FrameworkElement;
#elif __IOS__
using CoreGraphics;
using SharpConstraintLayout.Maui.Widget.Interface;
using FrameworkElement = UIKit.UIView;
#elif __ANDROID__
using Android.Content;
using Android.Graphics;
using SharpConstraintLayout.Maui.Widget.Interface;
using FrameworkElement = Android.Views.View;
#elif __MAUI__
using FrameworkElement = Microsoft.Maui.Controls.View;
#endif
namespace SharpConstraintLayout.Maui.Widget
{
    //using SuppressLint = android.annotation.SuppressLint;
    //using Context = android.content.Context;
    //using Canvas = android.graphics.Canvas;
    //using AttributeSet = android.util.AttributeSet;
    //using View = android.view.View;

    /// <summary>
    /// Utility class representing a Guideline helper object for <seealso cref="ConstraintLayout"/>.
    /// Helper objects are not displayed on device (they are marked as {@code View.GONE}) and are only used
    /// for layout purposes. They only work within a <seealso cref="ConstraintLayout"/>.
    /// <para>
    /// A Guideline can be either horizontal or vertical:
    /// <ul>
    ///     <li>Vertical Guidelines have a width of zero and the height of their <seealso cref="ConstraintLayout"/> parent</li>
    ///     <li>Horizontal Guidelines have a height of zero and the width of their <seealso cref="ConstraintLayout"/> parent</li>
    /// </ul>
    /// </para>
    /// <para>
    /// Positioning a Guideline is possible in three different ways:
    /// <ul>
    ///     <li>specifying a fixed distance from the left or the top of a layout ({@code layout_constraintGuide_begin})</li>
    ///     <li>specifying a fixed distance from the right or the bottom of a layout ({@code layout_constraintGuide_end})</li>
    ///     <li>specifying a percentage of the width or the height of a layout ({@code layout_constraintGuide_percent})</li>
    /// </ul>
    /// </para>
    /// <para>
    /// Widgets can then be constrained to a Guideline, allowing multiple widgets to be positioned easily from
    /// one Guideline, or allowing reactive layout behavior by using percent positioning.
    /// </para>
    /// <para>
    /// See the list of attributes in <seealso cref="androidx.constraintlayout.widget.ConstraintLayout.LayoutParams"/> to set a Guideline
    /// in XML, as well as the corresponding <seealso cref="ConstraintSet.setGuidelineBegin"/>, <seealso cref="ConstraintSet.setGuidelineEnd"/>
    /// and <seealso cref="ConstraintSet.SetGuidelinePercent"/> functions in <seealso cref="ConstraintSet"/>.
    /// </para>
    /// <para>
    /// Example of a {@code Button} constrained to a vertical {@code Guideline}:<br>
    /// {@sample resources/examples/Guideline.xml Guideline}
    /// </para>
    /// </summary>
    public class Guideline : FrameworkElement, IDisposable, IGuideline
    {
#if __ANDROID__
        public Guideline(Context context) : base(context)
#else
        public Guideline() : base()
#endif
        {
            UIElementExtension.SetViewVisibility(this, ConstraintSet.Gone);
        }

        internal androidx.constraintlayout.core.widgets.Guideline mGuideline = new androidx.constraintlayout.core.widgets.Guideline();

#if __ANDROID__
        /// <summary>
        /// @suppress
        /// </summary>
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            SetMeasuredDimension(0, 0);
        }
#endif

        /// <summary>
        /// Set the guideline's distance from the top or left edge.
        /// </summary>
        /// <param name="margin"> the distance to the top or left edge </param>
        public virtual void SetGuidelineBegin(int value)
        {
            mGuideline.GuideBegin = value;
        }

        /// <summary>
        /// Set a guideline's distance to end.
        /// </summary>
        /// <param name="margin"> the margin to the right or bottom side of container </param>
        public virtual void SetGuidelineEnd(int value)
        {
            mGuideline.GuideEnd = value;
        }

        /// <summary>
        /// Set a Guideline's percent. </summary>
        /// <param name="ratio"> the ratio between the gap on the left and right 0.0 is top/left 0.5 is middle </param>
        public virtual void SetGuidelinePercent(float value)
        {
            mGuideline.setGuidePercent(value);
        }

        [Obsolete("Android need set at layoutparams, please use constraintset.create to set orientation.")]
        public virtual int Orientation
        {
            set => mGuideline.Orientation = value;
            get => mGuideline.Orientation;
        }

        public void Dispose()
        {
            mGuideline = null;
        }
    }
}
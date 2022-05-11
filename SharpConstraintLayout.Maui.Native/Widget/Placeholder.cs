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
#if __MAUI__
    using Panel = Microsoft.Maui.Controls.Grid;
    using FrameworkElement = Microsoft.Maui.Controls.View;
    using UIElement = Microsoft.Maui.Controls.View;
#elif WINDOWS
    using Panel = Microsoft.UI.Xaml.Controls.Grid;
    using Microsoft.UI.Xaml.Controls;
    using FrameworkElement = Microsoft.UI.Xaml.FrameworkElement;
    using UIElement = Microsoft.UI.Xaml.UIElement;
#elif __IOS__
    using Panel = UIKit.UIView;
    using FrameworkElement = UIKit.UIView;
    using UIElement = UIKit.UIView;
    using UIKit;
    using CoreGraphics;
#elif __ANDROID__
    using Android.Content;
    using Panel = Android.Views.View;
    using FrameworkElement = Android.Views.View;
    using UIElement = Android.Views.View;
    using Android.Graphics;
#endif
    using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;
    using IPlaceholder = Interface.IPlaceholder;

    /// <summary>
    /// <b>Added in 1.1</b>
    /// <br/>
    /// A Placeholder provides a virtual object which can position an existing object.
    /// <br/>
    /// When the id of another view is set on a placeholder (using <see cref="Placeholder.SetContentId"/>}),
    /// the placeholder effectively becomes the content view. If the content view exist on the
    /// screen it is treated as gone from its original location.
    /// <br/>
    /// The content view is positioned using the layout of the parameters of the {@code Placeholder}  (the {@code Placeholder}
    /// is simply constrained in the layout like any other view).
    /// <br/>
    /// 
    /// <see href="https://developer.android.com/reference/androidx/constraintlayout/widget/Placeholder">	androidx.constraintlayout.widget.Placeholder</see>
    /// </summary>
    public partial class Placeholder : Panel, IPlaceholder
    {
        /// <summary>
        /// 指定是否作为编辑模式,由于ReloadPreview使用的是运行时预览,因此与设计器的设计模式不同,需要自己指定是否需要展示PlaceHolder.
        /// </summary>
        public static bool IsEditMode = false;

        private int mContentId = -1;
        private FrameworkElement mContent = null;
        private int mEmptyVisibility = ConstraintSet.Invisible;
#if __ANDROID__ && !__MAUI__
        public Placeholder(Context context) : base(context)
#else
        public Placeholder()
#endif
        {
            if (IsEditMode)
            {
#if __MAUI__
                this.BackgroundColor = Color.FromRgba(223, 223, 223, 255);
                this.Add(new Label() { Text = "?", TextColor = Colors.Black, FontSize = 18, HorizontalOptions = new LayoutOptions(LayoutAlignment.Center, false), VerticalOptions = new LayoutOptions(LayoutAlignment.Center, false), HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center });
#elif WINDOWS
                this.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 223, 223, 223));
                this.Children.Add(new TextBlock() { 
                    Text = "?", 
                    Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 0,0,0)), 
                    FontSize = 18,
                    HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center,
                    VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Center,
                    HorizontalTextAlignment = Microsoft.UI.Xaml.TextAlignment.Center,
                    Width = 20,
                    Height = 20
                });
#elif __IOS__
                this.BackgroundColor = UIColor.FromRGBA(223, 223, 223, 255);
                var label = new UILabel() { Text = "?", TextColor = UIColor.Black, Font = UIFont.SystemFontOfSize(18), TextAlignment = UITextAlignment.Center, };
                label.TranslatesAutoresizingMaskIntoConstraints = false;
                this.Add(label);
                label.CenterXAnchor.ConstraintEqualTo(this.CenterXAnchor).Active = true;
                label.CenterYAnchor.ConstraintEqualTo(this.CenterYAnchor).Active = true;
                label.WidthAnchor.ConstraintEqualTo(20).Active = true;
                label.HeightAnchor.ConstraintEqualTo(20).Active = true;
#endif
            }
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
#if __MAUI__
#elif __ANDROID__
        /// <summary>
        /// Placeholder does not draw anything itself - therefore Paint and Rect allocations
        /// are fine to suppress and ignore.
        /// 
        /// @suppress </summary>
        /// <param name="canvas"> </param>
        protected override void OnDraw(Canvas canvas)
        {
            if (IsEditMode)
            {
                canvas.DrawRGB(223, 223, 223);

                //ORIGINAL LINE: @SuppressLint("DrawAllocation") android.graphics.Paint paint = new android.graphics.Paint();
                Paint paint = new Paint();
                paint.SetARGB(255, 210, 210, 210);
                paint.TextAlign = Paint.Align.Center;
                paint.SetTypeface(Typeface.Create(Typeface.Default, TypefaceStyle.Normal));

                //ORIGINAL LINE: @SuppressLint("DrawAllocation") android.graphics.Rect r = new android.graphics.Rect();
                Rect r = new Rect();
                canvas.GetClipBounds(r);
                paint.TextSize = r.Height();
                int cHeight = r.Height();
                int cWidth = r.Width();
                paint.TextAlign = Paint.Align.Left;
                string text = "?";
                paint.GetTextBounds(text, 0, text.Length, r);
                float x = cWidth / 2f - r.Width() / 2f - r.Left;
                float y = cHeight / 2f + r.Height() / 2f - r.Bottom;
                canvas.DrawText(text, x, y, paint);
            }
        }

#elif WINDOWS

#elif __IOS__
        public override void Draw(CGRect rect)
        {
            if (IsEditMode)
            {
                base.Draw(rect);
                using var g = UIKit.UIGraphics.GetCurrentContext();
                g.SetFillColor(223, 223, 223, 255);
                g.FillRect(rect);
                g.SetLineWidth(1.0f);
                g.SetStrokeColor(210, 210, 210, 255);

                // set text specific graphics state
                g.SetTextDrawingMode(CGTextDrawingMode.FillStroke);
                g.SetFontSize(rect.Height);
                g.ShowTextAtPoint(rect.X / 2, rect.Y / 2, System.Text.Encoding.ASCII.GetBytes("?"));
            }
        }
#endif
        public virtual void UpdatePreLayout(ConstraintLayout container)
        {
            if (mContentId == -1)
            {
                if (!IsEditMode)
                {
                    //Visibility = (Microsoft.UI.Xaml.Visibility)mEmptyVisibility;
                    UIElementExtension.SetViewVisibility(this, mEmptyVisibility);
                }
            }

            mContent = container.FindElementById(mContentId) as FrameworkElement;
            if (mContent != null)
            {
                container.mConstraintSet.Constraints[mContent.GetId()].layout.isInPlaceholder = true;

                UIElementExtension.SetViewVisibility(mContent, ConstraintSet.Visible);
                UIElementExtension.SetViewVisibility(this, ConstraintSet.Visible);
            }
        }

        public void SetContent(UIElement element)
        {
            SetContentId(element.GetId());
        }

        /// <summary>
        /// Sets the content view id
        /// </summary>
        /// <param name="value"> the id of the content view we want to place in the Placeholder </param>
        public virtual void SetContentId(int value)
        {
            if (mContentId == value)
            {
                return;
            }
            if (mContent != null)
            {
                UIElementExtension.SetViewVisibility(mContent, ConstraintSet.Visible);
                if (mContent.GetParent() != null)
                    (mContent.GetParent() as ConstraintLayout).mConstraintSet.GetConstraint(mContent.GetId()).layout.isInPlaceholder = false;

                mContent = null;
            }

            mContentId = value;
            if (value != ConstraintSet.Unset)
            {
                if (this.GetParent() != null)
                {
                    UIElement v = (this.GetParent() as ConstraintLayout).FindElementById(value);
                    if (v != null)
                    {
                        UIElementExtension.SetViewVisibility(v, ConstraintSet.Gone);
                    }
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
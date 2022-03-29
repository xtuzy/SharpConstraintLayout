
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if WINDOWS
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
#elif __IOS__
using UIElement = UIKit.UIView;
using FrameworkElement = UIKit.UIView;
using Panel = UIKit.UIView;
using UIKit;
using CoreGraphics;
#elif __ANDROID__
using UIElement = Android.Views.View;
using FrameworkElement = Android.Views.View;
using AndroidX.ConstraintLayout.Widget;
using Android.Views;
#endif
namespace SharpConstraintLayout.Maui.Widget
{
    /// <summary>
    /// For deal with platform difference
    /// </summary>
    public static class ViewExtension
    {
#if WINDOWS ||__IOS__
        /// <summary>
        /// Baseline To Font Center Height
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public static float GetBaseline(this UIElement view)
        {
#if WINDOWS
            double? baselineOffset = 0;
            if (view is TextBlock || view is RichTextBlock)
            {
                baselineOffset = (view as TextBlock)?.BaselineOffset;
                if (baselineOffset == null)
                    baselineOffset = (view as RichTextBlock)?.BaselineOffset;
                return (float)baselineOffset;
            }
            else
            {
                return ConstraintSet.Unset;
            }
#elif __IOS__
            //https://stackoverflow.com/questions/35922215/how-to-calculate-uitextview-first-baseline-position-relatively-to-the-origin
            if (view is UITextField || view is UITextView || view is UILabel || view is UIButton)
            {
                var fontMetrics = (view as UITextField)?.Font;
                if (fontMetrics == null)
                    fontMetrics = (view as UITextView)?.Font;
                if (fontMetrics == null)
                    fontMetrics = (view as UIButton)?.TitleLabel?.Font;
                if (fontMetrics == null)
                    fontMetrics = (view as UILabel)?.Font;
                return (float)(view.IntrinsicContentSize.Height / 2 + (float)((fontMetrics.Descender - fontMetrics.Ascender) / 2 - fontMetrics.Descender));
            }
            else
                return ConstraintSet.Unset;
#endif
        }
#endif

        /// <summary>
        /// 每个Element的ID从该方法获取
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public static int GetId(this UIElement view)
        {
#if __ANDROID__
            if (view.Id == UIElement.NoId)
                view.Id = UIElement.GenerateViewId();
            return view.Id;
#else
            return view.GetHashCode();
#endif
        }

        public static void SetViewVisibility(this UIElement element, int ConstraintSetVisible)
        {
#if WINDOWS
            if (ConstraintSetVisible == ConstraintSet.Invisible)
                element.Opacity = 0;//https://stackoverflow.com/questions/28097153/workaround-for-visibilty-hidden-state-windows-phone-8-1-app-development
            else if (ConstraintSetVisible == ConstraintSet.Visible)
            {
                element.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
                if (element.Opacity == 0)
                    element.Opacity = 1;
            }
            else
            {
                element.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;//GONE
            }
#elif __IOS__
            if (ConstraintSetVisible == ConstraintSet.Invisible)
                element.Hidden = true;
            else if (ConstraintSetVisible == ConstraintSet.Visible)
                element.Hidden = false;
            else//Gone
            {
                //throw new NotImplementedException("iOS没有默认的Visibility = ConstraintSet.Gone");
                element.Hidden = true;
                Debug.WriteLine("iOS没有默认的Visibility = ConstraintSet.Gone");
            }
#endif
        }

#if WINDOWS || __IOS__
        public static (int Width, int Height) GetDefaultSize(this UIElement element)
        {
#if WINDOWS

            return ((int)element.DesiredSize.Width, (int)element.DesiredSize.Height);
#else
            return ((int)element.IntrinsicContentSize.Width, (int)element.IntrinsicContentSize.Height);
#endif
        }
#endif

#if WINDOWS || __IOS__
        public static (int Width, int Height) GetMeasuredSize(this UIElement element, androidx.constraintlayout.core.widgets.ConstraintWidget widget)
        {
            int w = 0;
            int h = 0;
#if WINDOWS
            (w, h) = ((int)element.DesiredSize.Width, (int)element.DesiredSize.Height);
#else
            (w, h) = ((int)element.IntrinsicContentSize.Width, (int)element.IntrinsicContentSize.Height);

            //@zhouyang add:for test why flow in iOS not correct
            if (ConstraintLayout.DEBUG && widget is androidx.constraintlayout.core.widgets.VirtualLayout)
            {
                Debug.WriteLine($"iOS IntrinsicContentSize:({w},{h})");
            }
#endif
            if (w <= 0) w = widget.Width;
            if (h <= 0) h = widget.Height;
            return (w, h);
        }
#endif

#if WINDOWS || __IOS__
        public static void MeasureSelf(this UIElement element, int w, int h)
        {
#if WINDOWS
            element.Measure(new Windows.Foundation.Size(w, h));
#endif
        }
#endif

#if WINDOWS || __IOS__
        public static UIElement GetParent(this FrameworkElement element)
        {
#if WINDOWS
            return element.Parent as UIElement;
#elif __IOS__
            return element.Superview;
#endif
        }
#endif
#if WINDOWS || __IOS__ || __ANDROID__
        public static void SetTransform(this UIElement element, ConstraintSet.Transform transform)
        {
            if (transform == null)
                return;
#if ANDROID
            //Copy from ConstraintSet
            var view = element;
            view.Rotation = transform.Rotation;
            view.RotationX = transform.RotationX;
            view.RotationY = transform.RotationY;
            view.ScaleX = transform.ScaleX;
            view.ScaleY = transform.ScaleY;
            if (transform.TransformPivotTarget != ConstraintSet.Unset)
            {
                View layout = (View)view.Parent;
                View center = layout.FindViewById(transform.TransformPivotTarget);
                if (center != null)
                {
                    float cy = (center.Top + center.Bottom) / 2.0f;
                    float cx = (center.Left + center.Right) / 2.0f;
                    if (view.Right - view.Left > 0 && view.Bottom - view.Top > 0)
                    {
                        float px = (cx - view.Left);
                        float py = (cy - view.Top);
                        view.PivotX = px;
                        view.PivotY = py;
                    }
                }
            }
            else
            {
                if (!float.IsNaN(transform.TransformPivotX))
                {
                    view.PivotX = transform.TransformPivotX;
                }
                if (!float.IsNaN(transform.TransformPivotY))
                {
                    view.PivotY = transform.TransformPivotY;
                }
            }
            view.TranslationX = transform.TranslationX;
            view.TranslationY = transform.TranslationY;
            //After Android5.0,have Z-index
            view.TranslationZ = transform.TranslationZ;
            if (transform.ApplyElevation)
            {
                view.Elevation = transform.Elevation;
            }
#elif WINDOWS
            var transformGroup = new TransformGroup();
            if (transform.rotation != 0)
            {
                var rotateTransform = new RotateTransform() { Angle = transform.rotation };
                transformGroup.Children.Add(rotateTransform);
            }
            if (transform.scaleX != 1)
            {
                var scaleTransform = new ScaleTransform() { ScaleX = transform.scaleX, ScaleY = transform.scaleY };
                transformGroup.Children.Add(scaleTransform);
            }
            if (transform.translationX != 0)
            {
                var translateTransform = new TranslateTransform() { X = transform.translationX, Y = transform.translationY };
                transformGroup.Children.Add(translateTransform);
            }
            element.RenderTransform = transformGroup;
#elif __IOS__

            //https://stackoverflow.com/questions/6813899/how-to-programmatically-rotate-the-view-by-180-degrees-on-ios
            var transformGroup = new CGAffineTransform();
            transformGroup.Rotate(transform.rotation);
            transformGroup.Scale(transform.scaleX, transform.scaleY);
            transformGroup.Translate(transform.translationX, transform.translationY);
            element.Transform = transformGroup;
#endif
        }
#endif
#if WINDOWS || __IOS__ || __ANDROID__
        public static void SetAlphaProperty(this UIElement element, ConstraintSet.PropertySet propertySet)
        {
            if (propertySet == null)
                return;
#if ANDROID
            //Copy from ConstraintSet
            element.Alpha = propertySet.Alpha;
#elif WINDOWS
            //https://docs.microsoft.com/en-us/dotnet/api/system.windows.uielement.opacity?view=windowsdesktop-6.0
            element.Opacity = propertySet.alpha;
#elif __IOS__
            //https://stackoverflow.com/questions/15381436/is-the-opacity-and-alpha-the-same-thing-for-uiview
            element.Alpha = propertySet.alpha;
#endif
        }
#endif
    }
}


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
#elif __MAUI__
using UIElement = Microsoft.Maui.Controls.View;
using FrameworkElement = Microsoft.Maui.Controls.View;
#endif
namespace SharpConstraintLayout.Maui.Widget
{
    /// <summary>
    /// For deal with platform difference
    /// </summary>
    internal static class UIElementExtension
    {

        /// <summary>
        /// Baseline To Font Center Height
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static float GetBaseline(this UIElement element)
        {
#if WINDOWS
            double? baselineOffset = 0;
            if (element is TextBlock || element is RichTextBlock)
            {
                baselineOffset = (element as TextBlock)?.BaselineOffset;
                if (baselineOffset == null)
                    baselineOffset = (element as RichTextBlock)?.BaselineOffset;
                return (float)baselineOffset;
            }
            else
            {
                return ConstraintSet.Unset;
            }
#elif __IOS__
            //https://stackoverflow.com/questions/35922215/how-to-calculate-uitextview-first-baseline-position-relatively-to-the-origin
            if (element is UITextField || element is UITextView || element is UILabel || element is UIButton)
            {
                var fontMetrics = (element as UITextField)?.Font;
                if (fontMetrics == null)
                    fontMetrics = (element as UITextView)?.Font;
                if (fontMetrics == null)
                    fontMetrics = (element as UIButton)?.TitleLabel?.Font;
                if (fontMetrics == null)
                    fontMetrics = (element as UILabel)?.Font;
                return (float)(element.IntrinsicContentSize.Height / 2 + (float)((fontMetrics.Descender - fontMetrics.Ascender) / 2 - fontMetrics.Descender));
            }
            else
                return ConstraintSet.Unset;
#elif __ANDROID__
            return element.Baseline;
#elif __MAUI__
            //TODO
            return ConstraintSet.Unset;
#endif
        }

        /// <summary>
        /// 每个Element的ID从该方法获取
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static int GetId(this UIElement element)
        {
            //#if __ANDROID__
            //            if (view.Id == UIElement.NoId)
            //                view.Id = UIElement.GenerateViewId();
            //            return view.Id;
            //#else
            return element.GetHashCode();
            //#endif
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
#elif __ANDROID__
            if (ConstraintSetVisible == ConstraintSet.Invisible)
                element.Visibility = ViewStates.Invisible;
            else if (ConstraintSetVisible == ConstraintSet.Visible)
                element.Visibility = ViewStates.Visible;
            else//Gone
                element.Visibility = ViewStates.Gone;
#elif __MAUI__
            if (ConstraintSetVisible == ConstraintSet.Invisible)
                (element as VisualElement).IsVisible = false;
            else if (ConstraintSetVisible == ConstraintSet.Visible)
                (element as VisualElement).IsVisible = true;
            else//Gone
            {
                //throw new NotImplementedException("Maui没有默认的Visibility = ConstraintSet.Gone");
                (element as VisualElement).IsVisible = false;
                Debug.WriteLine("Maui没有默认的Visibility = ConstraintSet.Gone");
            }
#endif
        }

        /// <summary>
        /// 获取控件自身测量的大小,这个大小是控件的内容大小或者平台的原生布局赋予的大小,由平台自身去计算
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static (int Width, int Height) GetWrapContentSize(this UIElement element)
        {
#if WINDOWS
            return ((int)element.DesiredSize.Width, (int)element.DesiredSize.Height);
#elif __IOS__
            //此处有各种Size的对比:https://zhangbuhuai.com/post/auto-layout-part-1.html
            var (w, h) = ((int)element.IntrinsicContentSize.Width, (int)element.IntrinsicContentSize.Height);//有固有大小的控件
            //iOS有些View的IntrinsicContentSize始终为-1,此时尝试使用SystemLayoutSizeFittingSize获得大小,但也可能获得0
            if (w <= 0 || h <= 0)
            {
                var size = element.SystemLayoutSizeFittingSize(UIView.UILayoutFittingCompressedSize);
                w = (int)size.Width;
                h = (int)size.Height;
                if (ConstraintLayout.DEBUG) Debug.WriteLine($"{element.GetType().FullName} SystemLayoutSizeFittingSize: {size}");
            }
            return (w, h);
#elif __ANDROID__
            return (element.MeasuredWidth, element.MeasuredHeight);
#elif __MAUI__
            return ((int)element.DesiredSize.Width, (int)element.DesiredSize.Height);
#endif
        }

        /// <summary>
        /// 获取控件测量的大小,这个大小结合了ConstraintWidget被设置的大小
        /// </summary>
        /// <param name="element"></param>
        /// <param name="widget"></param>
        /// <returns></returns>
        public static (int Width, int Height) GetMeasuredSize(this UIElement element, androidx.constraintlayout.core.widgets.ConstraintWidget widget)
        {
            var (w, h) = GetWrapContentSize(element);
            if (w <= 0) w = widget.Width;
            if (h <= 0) h = widget.Height;
            return (w, h);
        }

        public static void MeasureSelf(this UIElement element, int horizontalSpec, int verticalSpec)
        {

#if WINDOWS
            int w = MeasureSpec.GetSize(horizontalSpec);
            int h = MeasureSpec.GetSize(verticalSpec);
            element.Measure(new Windows.Foundation.Size(w, h));
#elif __ANDROID__
            element.Measure(horizontalSpec, verticalSpec);
#elif __MAUI__
            int w = MeasureSpec.GetSize(horizontalSpec);
            int h = MeasureSpec.GetSize(verticalSpec);
            element.Measure(w, h);
#endif
        }

        public static UIElement GetParent(this FrameworkElement element)
        {
#if WINDOWS
            return element.Parent as UIElement;
#elif __IOS__
            return element.Superview;
#elif __ANDROID__
            return element.Parent as UIElement;
#elif __MAUI__
            return element.Parent as UIElement;
#endif
        }

        public static void SetTransform(this UIElement element, ConstraintSet.Transform transform)
        {
            if (transform == null)
                return;
#if ANDROID
            //Copy from ConstraintSet
            var view = element;
            view.Rotation = transform.rotation;
            view.RotationX = transform.rotationX;
            view.RotationY = transform.rotationY;
            view.ScaleX = transform.scaleX;
            view.ScaleY = transform.scaleY;
            if (transform.transformPivotTarget != ConstraintSet.Unset)
            {
                View layout = (View)view.Parent;
                View center = layout.FindViewById(transform.transformPivotTarget);
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
                if (!float.IsNaN(transform.transformPivotX))
                {
                    view.PivotX = transform.transformPivotX;
                }
                if (!float.IsNaN(transform.transformPivotY))
                {
                    view.PivotY = transform.transformPivotY;
                }
            }
            view.TranslationX = transform.translationX;
            view.TranslationY = transform.translationY;
            //After Android5.0,have Z-index
            view.TranslationZ = transform.translationZ;
            if (transform.applyElevation)
            {
                view.Elevation = transform.elevation;
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
            //@zhouyang add at 2022/4/18:TODO:Use this ios will not show view, need leran how to use
            //https://stackoverflow.com/questions/6813899/how-to-programmatically-rotate-the-view-by-180-degrees-on-ios
            /*var transformGroup = new CGAffineTransform();
            transformGroup.Rotate(transform.rotation);
            transformGroup.Scale(transform.scaleX, transform.scaleY);
            transformGroup.Translate(transform.translationX, transform.translationY);
            element.Transform = transformGroup;*/
#elif __MAUI__
            //TODO
#endif
        }

        public static void SetAlphaProperty(this UIElement element, ConstraintSet.PropertySet propertySet)
        {
            if (propertySet == null)
                return;
#if __ANDROID__
            //Copy from ConstraintSet
            element.Alpha = propertySet.alpha;
#elif WINDOWS
            //https://docs.microsoft.com/en-us/dotnet/api/system.windows.uielement.opacity?view=windowsdesktop-6.0
            element.Opacity = propertySet.alpha;
#elif __IOS__
            //https://stackoverflow.com/questions/15381436/is-the-opacity-and-alpha-the-same-thing-for-uiview
            element.Alpha = propertySet.alpha;
#elif __MAUI__
            (element as VisualElement).Opacity = propertySet.alpha;
#endif
        }

        public static string GetViewLayoutInfo(this UIElement element)
        {
#if WINDOWS
            return $"{element.GetType().FullName} Visibility={element.Visibility} Position={element.ActualOffset} DesiredSize={element.DesiredSize} ActualSize={element.ActualSize}";
#elif __IOS__
            return $"{element.GetType().FullName} IsHiden={element.Hidden} Frame={element.Frame} Bounds={element.Bounds} IntrinsicContentSize={element.IntrinsicContentSize}";
#elif __ANDROID__
            return $"{element.GetType().FullName} Visibility={element.Visibility} Position={element.GetX()}x{element.GetY()} Size={element.Width}x{element.Height} MeasuredSize={element.MeasuredWidth}x{element.MeasuredHeight}";
#elif __MAUI__
            return $"{element.GetType().FullName} Visibility={element.IsVisible} Position={element.X},{element.Y} DesiredSize={element.DesiredSize}";
#endif
        }
    }
}

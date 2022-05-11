
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if __MAUI__
using UIElement = Microsoft.Maui.Controls.View;
using FrameworkElement = Microsoft.Maui.Controls.View;

#elif WINDOWS
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
    public static class UIElementExtension
    {

        /// <summary>
        /// Baseline To Font Center Height
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static int GetBaseline(this UIElement element, int elementHeight = 0)
        {
#if __MAUI__
            //TODO
            return ConstraintSet.Unset;
#elif WINDOWS
            double? baselineOffset = 0;
            if (element is TextBlock || element is RichTextBlock)
            {
                baselineOffset = (element as TextBlock)?.BaselineOffset;
                if (baselineOffset == null)
                    baselineOffset = (element as RichTextBlock)?.BaselineOffset;
                return (int)baselineOffset;
            }
            else
            {
                return ConstraintSet.Unset;
            }
#elif __IOS__
            //https://stackoverflow.com/questions/35922215/how-to-calculate-uitextview-first-baseline-position-relatively-to-the-origin
            if (element is UITextField || element is UITextView || element is UILabel /* || element is UIButton*/)
            {
                var fontMetrics = (element as UITextField)?.Font;
                if (fontMetrics == null)
                    fontMetrics = (element as UITextView)?.Font;
                /*if (fontMetrics == null)
                    fontMetrics = (element as UIButton)?.TitleLabel?.Font;*/
                if (fontMetrics == null)
                    fontMetrics = (element as UILabel)?.Font;
                return (int)(elementHeight / 2 + (float)((fontMetrics.Descender - fontMetrics.Ascender) / 2 - fontMetrics.Descender));
            }
            else
                return ConstraintSet.Unset;
#elif __ANDROID__
            return element.Baseline;
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
#if __MAUI__
            if (ConstraintSetVisible == ConstraintSet.Invisible)//TODO
            {
                (element as VisualElement).IsVisible = true;
                (element as VisualElement).Opacity = 0;
            }
            else if (ConstraintSetVisible == ConstraintSet.Visible)
            {
                (element as VisualElement).IsVisible = true;
                if (element.Opacity == 0)
                    element.Opacity = 1;
            }
            else//Gone
            {
                (element as VisualElement).IsVisible = false;
            }

#elif WINDOWS
            if (ConstraintSetVisible == ConstraintSet.Invisible)//TODO
            {
                element.Opacity = 0;//https://stackoverflow.com/questions/28097153/workaround-for-visibilty-hidden-state-windows-phone-8-1-app-development
            }
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
                element.Hidden = true;
            }
#elif __ANDROID__
            if (ConstraintSetVisible == ConstraintSet.Invisible)
                element.Visibility = ViewStates.Invisible;
            else if (ConstraintSetVisible == ConstraintSet.Visible)
                element.Visibility = ViewStates.Visible;
            else//Gone
                element.Visibility = ViewStates.Gone;
#endif
        }

        /// <summary>
        /// 获取控件自身测量的大小,这个大小是控件的内容大小或者平台的原生布局赋予的大小,由平台自身去计算
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static (int Width, int Height) GetWrapContentSize(this UIElement element)
        {
#if __MAUI__
            return ((int)element.DesiredSize.Width, (int)element.DesiredSize.Height);
#elif WINDOWS
            return ((int)element.DesiredSize.Width, (int)element.DesiredSize.Height);
#elif __IOS__
            //此处有各种Size的对比:https://zhangbuhuai.com/post/auto-layout-part-1.html
            //此处有AutoLayout中对各种Sizde的使用解释:https://www.jianshu.com/p/3a872a0bfe11

            CGSize size = CGSize.Empty;
            size = element.IntrinsicContentSize;//有固有大小的控件
            //iOS有些View的IntrinsicContentSize始终为-1,例如UIView,UIStackView,此时尝试使用SystemLayoutSizeFittingSize获得AutoLayout约束大小,但也可能获得不正确的值
            if (size.Width == UIView.NoIntrinsicMetric || size.Height == UIView.NoIntrinsicMetric)
            {
                var autoLayoutSize = element.SystemLayoutSizeFittingSize(UIView.UILayoutFittingCompressedSize);
                if (size.Width == UIView.NoIntrinsicMetric)
                    size.Width = autoLayoutSize.Width;
                if (size.Height == UIView.NoIntrinsicMetric)
                    size.Height = autoLayoutSize.Height;
            }
            //if (ConstraintLayout.DEBUG) Debug.WriteLine($"{element.GetType().FullName} IntrinsicContentSize: {element.IntrinsicContentSize}");
            //if (ConstraintLayout.DEBUG) Debug.WriteLine($"{element.GetType().FullName} SystemLayoutSizeFittingSize: {element.SystemLayoutSizeFittingSize(UIView.UILayoutFittingCompressedSize)}");
            //if (ConstraintLayout.DEBUG) Debug.WriteLine($"{element.GetType().FullName} Bounds: {element.Bounds}");
            //以上在对比UITextField时发现有IntrinsicContentSize时,SystemLayoutSizeFittingSize一样有,IntrinsicContentSize变化时,后者也会变,而Bounds不会

            if (size.Width < 5)//假定不正确的SystemLayoutSizeFittingSize都小于5,因为正常的控件大小至少比5dp要大
                size.Width = 0;
            if (size.Height < 5)
                size.Height = 0;
            return ((int)size.Width, (int)size.Height);
#elif __ANDROID__
            return (element.MeasuredWidth, element.MeasuredHeight);
#endif
        }

        public static (int measuredWidth, int measureWidth) MeasureSelf(this UIElement element, int horizontalSpec, int verticalSpec)
        {
            int w;
            int h;
#if __MAUI__
            w = MeasureSpec.GetSize(horizontalSpec);
            h = MeasureSpec.GetSize(verticalSpec);
            var sizeRequest = element.Measure(w, h);
            w = GetDefaultSize((int)sizeRequest.Request.Width, horizontalSpec);
            h = GetDefaultSize((int)sizeRequest.Request.Height, verticalSpec);
#elif WINDOWS
            w = MeasureSpec.GetSize(horizontalSpec);
            h = MeasureSpec.GetSize(verticalSpec);
            element.Measure(new Windows.Foundation.Size(w, h));
            (w, h) = element.GetWrapContentSize();
            w = GetDefaultSize(w, horizontalSpec);
            h = GetDefaultSize(h, verticalSpec);
#elif __IOS__
            (w, h) = element.GetWrapContentSize();
            w = GetDefaultSize(w, horizontalSpec);
            h = GetDefaultSize(h, verticalSpec);
#elif __ANDROID__
            element.Measure(horizontalSpec, verticalSpec);//Android中的measure会根据大小和MeasureSpec来测量,最后会存储测量值,其它平台不知道怎么存储,所以全返回
            w = element.MeasuredWidth;
            h = element.MeasuredHeight;
#endif
            return (w, h);
        }

        /// <summary>
        /// Utility to return a default size. Uses the supplied size if the
        /// MeasureSpec imposed no constraints. Will get larger if allowed
        /// by the MeasureSpec. <see href="https://developer.android.com/reference/android/view/View#getDefaultSize(int,%20int)">View.getDefaultSize</see>.<br/>
        /// 另外参考：<see href="https://www.jianshu.com/p/d16ec64181f2"/>
        /// 如果父控件传递给的MeasureSpec的mode是MeasureSpec.UNSPECIFIED，就说明，父控件对自己没有任何限制，那么尺寸就选择自己需要的尺寸size;<br/>
        /// 如果父控件传递给的MeasureSpec的mode是MeasureSpec.EXACTLY，就说明父控件有明确的要求，希望自己能用measureSpec中的尺寸，这时就推荐使用MeasureSpec.getSize(measureSpec);<br/>
        /// 如果父控件传递给的MeasureSpec的mode是MeasureSpec.AT_MOST，就说明父控件希望自己不要超出MeasureSpec.getSize(measureSpec)，如果超出了，就选择MeasureSpec.getSize(measureSpec)，否则用自己想要的尺寸就行了;<br/>
        /// </summary>
        /// <param name="size">Default size for this view</param>
        /// <param name="measureSpec">Constraints imposed by the parent</param>
        /// <returns>The size this view should be.</returns>

        internal static int GetDefaultSize(int size, int measureSpec)
        {
            int result = size;
            int specMode = MeasureSpec.GetMode(measureSpec);
            int specSize = MeasureSpec.GetSize(measureSpec);

            if (specMode == MeasureSpec.UNSPECIFIED)
            {
                result = size;
            }
            else if (specMode == MeasureSpec.AT_MOST)
            {
                if (specSize < size)
                {
                    result = specSize;
                }
                else
                {
                    result = size;
                }
            }
            if (specMode == MeasureSpec.EXACTLY)
            {
                result = specSize;
            }
            return result;
        }

        public static UIElement GetParent(this FrameworkElement element)
        {
#if __MAUI__
            return element.Parent as UIElement;
#elif WINDOWS
            return element.Parent as UIElement;
#elif __IOS__
            return element.Superview;
#elif __ANDROID__
            return element.Parent as UIElement;
#endif
        }

        internal static void SetSizeAndMargin(this UIElement element, int width, int height, int minWidth, int minHeight, int maxWidth, int maxHeight, int left, int top, int right, int bottom)
        {
#if __MAUI__
            if (width > 0)
                element.WidthRequest = width;
            if (height > 0)
                element.HeightRequest = height;
            if (minWidth > 0)
                element.MinimumWidthRequest = minWidth;
            if (minHeight > 0)
                element.MinimumHeightRequest = minHeight;
            if (maxWidth > 0)
                element.MaximumWidthRequest = maxWidth;
            if (maxHeight > 0)
                element.MaximumHeightRequest = maxHeight;
            element.Margin = new Thickness(left, top, right, bottom);
#elif WINDOWS
            var view = element as FrameworkElement;
            if (width > 0)
                view.Width = width;
            if (height > 0)
                view.Height = height;
            if (minWidth > 0)
                view.MinWidth = minWidth;
            if (minHeight > 0)
                view.MinHeight = minHeight;
            if (maxWidth > 0)
                view.MaxWidth = maxWidth;
            if (maxHeight > 0)
                view.MaxHeight = maxHeight;
            view.Margin = new Microsoft.UI.Xaml.Thickness(left, top, right, bottom);
#elif __IOS__
            //element.Frame = new CoreGraphics.CGRect(element.Frame.X, element.Frame.Y, width, height);
            if (width > 0 && height > 0)
                element.Frame = new CoreGraphics.CGRect(element.Frame.X, element.Frame.Y, width, height);
            else if (width > 0)
                element.Frame = new CoreGraphics.CGRect(element.Frame.X, element.Frame.Y, width, element.Frame.Height);
            else if (height > 0)
                element.Frame = new CoreGraphics.CGRect(element.Frame.X, element.Frame.Y, element.Frame.Width, height);
            element.LayoutMargins = new UIEdgeInsets(top, left, bottom, right);
#elif __ANDROID__
            if (element.LayoutParameters == null)
                element.LayoutParameters = new ViewGroup.MarginLayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            if (width > 0 || width == -1 || width == -2)//width==0是设置MatchConstraint,我没有在LayoutParams中实现,因此等于0作为不设置处理
                element.LayoutParameters.Width = width;
            if (height > 0 || height == -1 || height == -2)
                element.LayoutParameters.Height = height;
            if (minWidth > 0)
                element.SetMinimumWidth(minWidth);
            if (minHeight > 0)
                element.SetMinimumHeight(minHeight);
            var layoutParams = (element.LayoutParameters as ViewGroup.MarginLayoutParams);
            if (layoutParams != null)
            {
                layoutParams.LeftMargin = left;
                layoutParams.TopMargin = top;
                layoutParams.RightMargin = right;
                layoutParams.BottomMargin = bottom;
            }
#endif
        }

        internal static void SetTransform(this UIElement element, ConstraintSet.Transform transform)
        {
            if (transform == null)
                return;
#if __MAUI__
            //TODO
#elif ANDROID
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
#endif
        }

        internal static void SetAlphaProperty(this UIElement element, ConstraintSet.PropertySet propertySet)
        {
            if (propertySet == null)
                return;
#if __MAUI__
            (element as VisualElement).Opacity = propertySet.alpha;
#elif __ANDROID__
            //Copy from ConstraintSet
            element.Alpha = propertySet.alpha;
#elif WINDOWS
            //https://docs.microsoft.com/en-us/dotnet/api/system.windows.uielement.opacity?view=windowsdesktop-6.0
            element.Opacity = propertySet.alpha;
#elif __IOS__
            //https://stackoverflow.com/questions/15381436/is-the-opacity-and-alpha-the-same-thing-for-uiview
            element.Alpha = propertySet.alpha;
#endif
        }

        public static string GetViewLayoutInfo(this UIElement element)
        {
#if __MAUI__
            return $"{element.GetType().FullName} IsVisible={element.IsVisible} Position=({element.X},{element.Y}) DesiredSize={element.DesiredSize} WidthxHeight=({element.Width}x{element.Height})";

#elif WINDOWS
            return $"{element.GetType().FullName} Visibility={element.Visibility} Position={element.ActualOffset} DesiredSize={element.DesiredSize} ActualSize={element.ActualSize}";
#elif __IOS__
            return $"{element.GetType().FullName} IsHiden={element.Hidden} Frame={element.Frame} Bounds={element.Bounds} IntrinsicContentSize={element.IntrinsicContentSize}";
#elif __ANDROID__
            return $"{element.GetType().FullName} Visibility={element.Visibility} Position={element.GetX()}x{element.GetY()} Size={element.Width}x{element.Height} MeasuredSize={element.MeasuredWidth}x{element.MeasuredHeight}";
#endif
        }
    }
}

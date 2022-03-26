
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if WINDOWS
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#elif __IOS__
using UIElement = UIKit.UIView;
using UIKit;
#elif __ANDROID__
using UIElement = Android.Views.View;
#endif
namespace SharpConstraintLayout.Maui.Widget
{
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
                if(fontMetrics == null)
                    fontMetrics= (view as UIButton)?.TitleLabel?.Font;
                if(fontMetrics == null)
                    fontMetrics=(view as UILabel)?.Font;
                return (float)(view.IntrinsicContentSize.Height/2 + (float)((fontMetrics.Descender - fontMetrics.Ascender) / 2 - fontMetrics.Descender));
            }else
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
            if(view.Id == UIElement.NoId)
                view.Id = UIElement.GenerateViewId();
            return view.Id;
#else
            return view.GetHashCode();
#endif
        }

        public static void SetViewVisibility(UIElement element, int ConstraintSetVisible)
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
    }
}

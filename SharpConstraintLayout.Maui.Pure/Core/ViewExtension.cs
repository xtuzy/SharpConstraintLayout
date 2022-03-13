
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
#endif
namespace SharpConstraintLayout.Maui.Pure.Core
{
    public static class ViewExtension
    {
        /// <summary>
        /// Baseline To Font Center Height
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public static float GetBaseline(this UIElement view)
        {
#if WINDOWS
            double baselineToTopHeight = 0;
            double? fontSize = 0;
            if(view is TextBlock || view is TextBox || view is Button)
            {
                fontSize = (view as TextBlock)?.FontSize;
                if(fontSize == null)
                    fontSize = (view as TextBox)?.FontSize;
                if (fontSize == null)
                    fontSize = (view as Button)?.FontSize;
            }
            else
            {
                return ConstraintSet.UNSET;
            }
            return (float)FontBaselineHelper.GetBaseline((int)fontSize);
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
                return (float)((fontMetrics.Descender - fontMetrics.Ascender) / 2 - fontMetrics.Descender);
            }else
                return ConstraintSet.UNSET;
#endif
        }

        public static int Id(this UIElement view)
        {
            return view.GetHashCode();
        }
    }
}

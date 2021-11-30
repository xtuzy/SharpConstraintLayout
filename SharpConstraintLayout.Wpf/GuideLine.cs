using androidx.constraintlayout.core.widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SharpConstraintLayout.Wpf
{
    /// <summary>
    /// <see href="https://developer.android.com/reference/androidx/constraintlayout/widget/Guideline">Android Guideline</see><br/>
    /// Guideline is not displayed on device(they are marked as View.GONE) and are only used for layout purposes.They only work within a ConstraintLayout.
    /// A Guideline can be either horizontal or vertical:
    /// Vertical Guidelines have a width of zero and the height of their ConstraintLayout parent
    /// Horizontal Guidelines have a height of zero and the width of their ConstraintLayout parent
    /// Positioning a Guideline is possible in three different ways:
    /// specifying a fixed distance from the left or the top of a layout (layout_constraintGuide_begin)
    /// specifying a fixed distance from the right or the bottom of a layout (layout_constraintGuide_end)
    /// specifying a percentage of the width or the height of a layout (layout_constraintGuide_percent)
    /// Widgets can then be constrained to a Guideline, allowing multiple widgets to be positioned easily from one Guideline, or allowing reactive layout behavior by using percent positioning.
    /// </summary>
    public class GuideLine: FrameworkElement
    {
        public readonly Guideline Guideline = new Guideline();

        public enum OrientationType
        {
            Horizontal = 0,
            Vertical
        }

        public OrientationType Orientation
        {
            set
            {
                Guideline.Orientation = (int)value;
            }
            get
            {
                return (OrientationType)Guideline.Orientation;
            }
        }

        /// <summary>
        /// Set the guideline's distance from the top or left edge.
        /// </summary>
        public int GuideStart
        {
            set
            {
                Guideline.GuideBegin = value;
            }
        }

        /// <summary>
        /// Set a guideline's distance to end.
        /// </summary>
        public int GuideEnd
        {
            set
            {
                Guideline.GuideEnd = value;
            }
        }

        /// <summary>
        /// A line at position that percent(0-1) of parent view
        /// </summary>
        public float Percent
        {
            set
            {
                if(value < 0||value>1)
                    throw new ArgumentOutOfRangeException("Percent should >=0 and <=1");
                Guideline.setGuidePercent(value);
            }
        }
    }
}

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
    /// A line no size,just for layout more easy.
    /// It can layout view at position relative to parent view's percent.
    /// </summary>
    public class GuideLine: FrameworkElement
    {
        public Guideline Guideline = new Guideline();

        public enum Orientations
        {
            HORIZONTAL = 0,
            VERTICAL
        }
        public Orientations Orientation
        {
            set
            {
                Guideline.Orientation = (int)value;
            }
            get
            {
                return (Orientations)Guideline.Orientation;
            }
        }

        public int GuideBegin
        {
            set
            {
                Guideline.GuideBegin = value;
            }
        }

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

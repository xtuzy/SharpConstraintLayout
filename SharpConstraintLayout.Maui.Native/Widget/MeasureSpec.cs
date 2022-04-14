using androidx.constraintlayout.core.widgets.analyzer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpConstraintLayout.Maui.Widget
{
    /// <summary>
    /// A MeasureSpec encapsulates the layout requirements passed from parent to child.
    /// Each MeasureSpec represents a requirement for either the width or the height.
    /// A MeasureSpec is comprised of a size and a mode. There are three possible
    /// modes:
    /// <dl>
    /// <dt>UNSPECIFIED</dt>
    /// <dd>
    /// The parent has not imposed any constraint on the child. It can be whatever size
    /// it wants.
    /// </dd>
    /// 
    /// <dt>EXACTLY</dt>
    /// <dd>
    /// The parent has determined an exact size for the child. The child is going to be
    /// given those bounds regardless of how big it wants to be.
    /// </dd>
    /// 
    /// <dt>AT_MOST</dt>
    /// <dd>
    /// The child can be as large as it wants up to the specified size.
    /// </dd>
    /// </dl>
    /// 
    /// MeasureSpecs are implemented as ints to reduce object allocation. This class
    /// is provided to pack and unpack the &lt;size, mode&gt; tuple into the int.
    /// </summary>
    public static class MeasureSpec
    {
        private const int MODE_SHIFT = 30;
        private const int MODE_MASK = 0x3 << MODE_SHIFT;

        public static readonly int AT_MOST = BasicMeasure.AT_MOST;
        public static int EXACTLY = BasicMeasure.EXACTLY;
        public static int UNSPECIFIED = BasicMeasure.UNSPECIFIED;

        ///<summary>
        /// Extracts the mode from the supplied measure specification.
        /// 
        /// @param measureSpec the measure specification to extract the mode from
        /// @return {@link android.view.View.MeasureSpec#UNSPECIFIED},
        ///         {@link android.view.View.MeasureSpec#AT_MOST} or
        ///         {@link android.view.View.MeasureSpec#EXACTLY}
        /// </summary>
        public static int GetMode(int measureSpec)
        {
            //noinspection ResourceType
            return (measureSpec & MODE_MASK);
        }

        ///<summary>
        /// Extracts the size from the supplied measure specification.
        /// 
        /// @param measureSpec the measure specification to extract the size from
        /// @return the size in pixels defined in the supplied measure specification
        /// </summary>
        public static int GetSize(int measureSpec)
        {
            return (measureSpec & ~MODE_MASK);
        }

        public static string ToString(int measureSpec)
        {
            int mode = GetMode(measureSpec);
            int size = GetSize(measureSpec);

            StringBuilder sb = new StringBuilder("MeasureSpec: ");

            if (mode == MeasureSpec.UNSPECIFIED)
                sb.Append("UNSPECIFIED ");
            else if (mode == MeasureSpec.EXACTLY)
                sb.Append("EXACTLY ");
            else if (mode == MeasureSpec.AT_MOST)
                sb.Append("AT_MOST ");
            else
                sb.Append(mode).Append(" ");

            sb.Append(size);
            return sb.ToString();
        }

        ///<summary>
        ///  Does the hard part of measureChildren: figuring out the MeasureSpec to
        ///  pass to a particular child. This method figures out the right MeasureSpec
        ///  for one dimension (height or width) of one child view.
        /// 
        ///  The goal is to combine information from our MeasureSpec with the
        ///  LayoutParams of the child to get the best possible results. For example,
        ///  if the this view knows its size (because its MeasureSpec has a mode of
        ///  EXACTLY), and the child has indicated in its LayoutParams that it wants
        ///  to be the same size as the parent, the parent should ask the child to
        ///  layout given an exact size.
        /// 
        ///  @param spec The requirements for this view
        ///  @param padding The padding of this view for the current dimension and
        ///         margins, if applicable
        ///  @param childDimension How big the child wants to be in the current
        ///         dimension
        ///  @return a MeasureSpec integer for the child
        /// </summary>
        public static int getChildMeasureSpec(int spec, int padding, int childDimension)
        {
            int specMode = MeasureSpec.GetMode(spec);
            int specSize = MeasureSpec.GetSize(spec);

            int size = Math.Max(0, specSize - padding);

            int resultSize = 0;
            int resultMode = 0;

            if (specMode == MeasureSpec.EXACTLY)// Parent has imposed an exact size on us
            {
                if (childDimension >= 0)
                {
                    resultSize = childDimension;
                    resultMode = MeasureSpec.EXACTLY;
                }
                else if (childDimension == ConstraintSet.MatchParent)
                {
                    // Child wants to be our size. So be it.
                    resultSize = size;
                    resultMode = MeasureSpec.EXACTLY;
                }
                else if (childDimension == ConstraintSet.WrapContent)
                {
                    // Child wants to determine its own size. It can't be
                    // bigger than us.
                    resultSize = size;
                    resultMode = MeasureSpec.AT_MOST;
                }

            }
            else if (specMode == MeasureSpec.AT_MOST)// Parent has imposed a maximum size on us
            {
                if (childDimension >= 0)
                {
                    // Child wants a specific size... so be it
                    resultSize = childDimension;
                    resultMode = MeasureSpec.EXACTLY;
                }
                else if (childDimension == ConstraintSet.MatchParent)
                {
                    // Child wants to be our size, but our size is not fixed.
                    // Constrain child to not be bigger than us.
                    resultSize = size;
                    resultMode = MeasureSpec.AT_MOST;
                }
                else if (childDimension == ConstraintSet.WrapContent)
                {
                    // Child wants to determine its own size. It can't be
                    // bigger than us.
                    resultSize = size;
                    resultMode = MeasureSpec.AT_MOST;
                }
            }
            else if (specMode == MeasureSpec.UNSPECIFIED)// Parent asked to see how big we want to be
            {
                if (childDimension >= 0)
                {
                    // Child wants a specific size... let him have it
                    resultSize = childDimension;
                    resultMode = MeasureSpec.EXACTLY;
                }
                else if (childDimension == ConstraintSet.MatchParent)
                {
                    // Child wants to be our size... find out how big it should
                    // be
                    resultSize = sUseZeroUnspecifiedMeasureSpec ? 0 : size;
                    resultMode = MeasureSpec.UNSPECIFIED;
                }
                else if (childDimension == ConstraintSet.WrapContent)
                {
                    // Child wants to determine its own size.... find out how
                    // big it should be
                    resultSize = sUseZeroUnspecifiedMeasureSpec ? 0 : size;
                    resultMode = MeasureSpec.UNSPECIFIED;
                }
            }
            //noinspection ResourceType
            return MeasureSpec.MakeMeasureSpec(resultSize, resultMode);
        }

        /// <summary>
        ///  Use the old (broken) way of building MeasureSpecs.
        /// </summary>
        static bool sUseBrokenMakeMeasureSpec = false;

        /// <summary>
        /// Always return a size of 0 for MeasureSpec values with a mode of UNSPECIFIED
        /// </summary>
        static bool sUseZeroUnspecifiedMeasureSpec = false;

        ///<summary>
        /// Creates a measure specification based on the supplied size and mode.
        /// 
        /// The mode must always be one of the following:
        /// <ul>
        ///  <li>{@link android.view.View.MeasureSpec#UNSPECIFIED}</li>
        ///  <li>{@link android.view.View.MeasureSpec#EXACTLY}</li>
        ///  <li>{@link android.view.View.MeasureSpec#AT_MOST}</li>
        /// </ul>
        /// 
        /// <p><strong>Note:</strong> On API level 17 and lower, makeMeasureSpec's
        /// implementation was such that the order of arguments did not matter
        /// and overflow in either value could impact the resulting MeasureSpec.
        /// {@link android.widget.RelativeLayout} was affected by this bug.
        /// Apps targeting API levels greater than 17 will get the fixed, more strict
        /// behavior.</p>
        /// 
        /// @param size the size of the measure specification
        /// @param mode the mode of the measure specification
        /// @return the measure specification based on size and mode
        /// </summary>
        public static int MakeMeasureSpec(int size, int mode)
        {
            if (sUseBrokenMakeMeasureSpec)
            {
                return size + mode;
            }
            else
            {
                return (size & ~MODE_MASK) | (mode & MODE_MASK);
            }
        }

        ///<summary>
        /// Bits of {@link #getMeasuredWidthAndState()} and
        /// {@link #getMeasuredWidthAndState()} that provide the additional state bits.
        /// </summary>
        public const int MEASURED_STATE_MASK = unchecked((int)0xff000000);

        ///<summary>
        /// Bits of {@link #getMeasuredWidthAndState()} and
        /// {@link #getMeasuredWidthAndState()} that provide the actual measured size.
        ///</summary>
        public const int MEASURED_SIZE_MASK = 0x00ffffff;

        ///<summary>
        /// Bit shift of {@link #MEASURED_STATE_MASK} to get to the height bits
        /// for functions that combine both width and height into a single int,
        /// such as {@link #getMeasuredState()} and the childState argument of
        /// {@link #resolveSizeAndState(int, int, int)}.
        ///</summary>
        public const int MEASURED_HEIGHT_STATE_SHIFT = 16;

        ///<summary>
        /// Bit of {@link #getMeasuredWidthAndState()} and
        /// {@link #getMeasuredWidthAndState()} that indicates the measured size
        /// is smaller that the space the view would like to have.
        /// </summary>
        public const int MEASURED_STATE_TOO_SMALL = 0x01000000;

        ///<summary>
        /// Utility to reconcile a desired size and state, with constraints imposed
        /// by a MeasureSpec. Will take the desired size, unless a different size
        /// is imposed by the constraints. The returned value is a compound integer,
        /// with the resolved size in the {@link #MEASURED_SIZE_MASK} bits and
        /// optionally the bit {@link #MEASURED_STATE_TOO_SMALL} set if the
        /// resulting size is smaller than the size the view wants to be.
        /// 
        /// @param size How big the view wants to be.
        /// @param measureSpec Constraints imposed by the parent.
        /// @param childMeasuredState Size information bit mask for the view's
        ///                           children.
        /// @return Size information bit mask as defined by
        ///         {@link #MEASURED_SIZE_MASK} and
        ///         {@link #MEASURED_STATE_TOO_SMALL}.
        /// </summary>
        public static int resolveSizeAndState(int size, int measureSpec, int childMeasuredState)
        {
            int specMode = MeasureSpec.GetMode(measureSpec);
            int specSize = MeasureSpec.GetSize(measureSpec);
            int result;
            if (specMode == MeasureSpec.AT_MOST)
                if (specSize < size)
                {
                    result = specSize | MEASURED_STATE_TOO_SMALL;
                }
                else
                {
                    result = size;
                }
            else if (specMode == MeasureSpec.EXACTLY)
                result = specSize;
            //esle if (specMode == MeasureSpec.UNSPECIFIED)
            else
                result = size;
            return result | (childMeasuredState & MEASURED_STATE_MASK);
        }
    }
}

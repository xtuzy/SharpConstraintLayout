/*
* Copyright (C) 2021 The Android Open Source Project
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpConstraintLayout.Maui.Core
{
    public class ViewGroup
    {
        /// <summary>
        /// Per-child layout information for layouts that support margins.
        /// See
        /// <seealso cref="android.R.styleable#ViewGroup_MarginLayout ViewGroup Margin Layout Attributes"/>
        /// for a list of all child view attributes that this class supports.
        /// 
        /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_margin
        /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginHorizontal
        /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginVertical
        /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginLeft
        /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginTop
        /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginRight
        /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginBottom
        /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginStart
        /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginEnd
        /// </summary>
        public class MarginLayoutParams : ViewGroup.LayoutParams
        {
            /// <summary>
            /// The left margin in pixels of the child. Margin values should be positive.
            /// Call <seealso cref="ViewGroup#setLayoutParams(LayoutParams)"/> after reassigning a new value
            /// to this field.
            /// </summary>            
            ///ORIGINAL LINE: @ViewDebug.ExportedProperty(category = "layout") @InspectableProperty(name = "layout_marginLeft") public int leftMargin;
            public int leftMargin;

            /// <summary>
            /// The top margin in pixels of the child. Margin values should be positive.
            /// Call <seealso cref="ViewGroup#setLayoutParams(LayoutParams)"/> after reassigning a new value
            /// to this field.
            /// </summary>         
            ///ORIGINAL LINE: @ViewDebug.ExportedProperty(category = "layout") @InspectableProperty(name = "layout_marginTop") public int topMargin;
            public int topMargin;

            /// <summary>
            /// The right margin in pixels of the child. Margin values should be positive.
            /// Call <seealso cref="ViewGroup#setLayoutParams(LayoutParams)"/> after reassigning a new value
            /// to this field.
            /// </summary>            
            ///ORIGINAL LINE: @ViewDebug.ExportedProperty(category = "layout") @InspectableProperty(name = "layout_marginRight") public int rightMargin;
            public int rightMargin;

            /// <summary>
            /// The bottom margin in pixels of the child. Margin values should be positive.
            /// Call <seealso cref="ViewGroup#setLayoutParams(LayoutParams)"/> after reassigning a new value
            /// to this field.
            /// </summary>
            ///ORIGINAL LINE: @ViewDebug.ExportedProperty(category = "layout") @InspectableProperty(name = "layout_marginBottom") public int bottomMargin;
            public int bottomMargin;

            /// <summary>
            /// The start margin in pixels of the child. Margin values should be positive.
            /// Call <seealso cref="ViewGroup#setLayoutParams(LayoutParams)"/> after reassigning a new value
            /// to this field.
            /// </summary>
            ///ORIGINAL LINE: @ViewDebug.ExportedProperty(category = "layout") @UnsupportedAppUsage private int startMargin = DEFAULT_MARGIN_RELATIVE;
            internal int startMargin = DEFAULT_MARGIN_RELATIVE;

            /// <summary>
            /// The end margin in pixels of the child. Margin values should be positive.
            /// Call <seealso cref="ViewGroup#setLayoutParams(LayoutParams)"/> after reassigning a new value
            /// to this field.
            /// </summary>
            ///ORIGINAL LINE: @ViewDebug.ExportedProperty(category = "layout") @UnsupportedAppUsage private int endMargin = DEFAULT_MARGIN_RELATIVE;
            internal int endMargin = DEFAULT_MARGIN_RELATIVE;

            /// <summary>
            /// The default start and end margin.
            /// @hide
            /// </summary>
            public static readonly int DEFAULT_MARGIN_RELATIVE = int.MinValue;

            /// <summary>
            /// Bit  0: layout direction
            /// Bit  1: layout direction
            /// Bit  2: left margin undefined
            /// Bit  3: right margin undefined
            /// Bit  4: is RTL compatibility mode
            /// Bit  5: need resolution
            /// 
            /// Bit 6 to 7 not used
            /// 
            /// @hide
            /// </summary>
            ///ORIGINAL LINE: @ViewDebug.ExportedProperty(category = "layout", flagMapping = { @ViewDebug.FlagToString(mask = LAYOUT_DIRECTION_MASK, equals = LAYOUT_DIRECTION_MASK, name = "LAYOUT_DIRECTION"), @ViewDebug.FlagToString(mask = LEFT_MARGIN_UNDEFINED_MASK, equals = LEFT_MARGIN_UNDEFINED_MASK, name = "LEFT_MARGIN_UNDEFINED_MASK"), @ViewDebug.FlagToString(mask = RIGHT_MARGIN_UNDEFINED_MASK, equals = RIGHT_MARGIN_UNDEFINED_MASK, name = "RIGHT_MARGIN_UNDEFINED_MASK"), @ViewDebug.FlagToString(mask = RTL_COMPATIBILITY_MODE_MASK, equals = RTL_COMPATIBILITY_MODE_MASK, name = "RTL_COMPATIBILITY_MODE_MASK"), @ViewDebug.FlagToString(mask = NEED_RESOLUTION_MASK, equals = NEED_RESOLUTION_MASK, name = "NEED_RESOLUTION_MASK") }, formatToHexString = true) byte mMarginFlags;
            internal sbyte mMarginFlags;

            internal const int LAYOUT_DIRECTION_MASK = 0x00000003;
            internal const int LEFT_MARGIN_UNDEFINED_MASK = 0x00000004;
            internal const int RIGHT_MARGIN_UNDEFINED_MASK = 0x00000008;
            internal const int RTL_COMPATIBILITY_MODE_MASK = 0x00000010;
            internal const int NEED_RESOLUTION_MASK = 0x00000020;

            internal const int DEFAULT_MARGIN_RESOLVED = 0;
            internal static readonly int UNDEFINED_MARGIN = DEFAULT_MARGIN_RELATIVE;

            /// <summary>
            /// Creates a new set of layout parameters. The values are extracted from
            /// the supplied attributes set and context.
            /// </summary>
            /// <param name="c"> the application environment </param>
            /// <param name="attrs"> the set of attributes from which to extract the layout
            ///              parameters' values </param>
            /*public MarginLayoutParams(Context c, AttributeSet attrs) : base()
            {

                TypedArray a = c.obtainStyledAttributes(attrs, R.styleable.ViewGroup_MarginLayout);
                setBaseAttributes(a, R.styleable.ViewGroup_MarginLayout_layout_width, R.styleable.ViewGroup_MarginLayout_layout_height);

                int margin = a.getDimensionPixelSize(R.styleable.ViewGroup_MarginLayout_layout_margin, -1);
                if (margin >= 0)
                {
                    leftMargin = margin;
                    topMargin = margin;
                    rightMargin = margin;
                    bottomMargin = margin;
                }
                else
                {
                    int horizontalMargin = a.getDimensionPixelSize(R.styleable.ViewGroup_MarginLayout_layout_marginHorizontal, -1);
                    int verticalMargin = a.getDimensionPixelSize(R.styleable.ViewGroup_MarginLayout_layout_marginVertical, -1);

                    if (horizontalMargin >= 0)
                    {
                        leftMargin = horizontalMargin;
                        rightMargin = horizontalMargin;
                    }
                    else
                    {
                        leftMargin = a.getDimensionPixelSize(R.styleable.ViewGroup_MarginLayout_layout_marginLeft, UNDEFINED_MARGIN);
                        if (leftMargin == UNDEFINED_MARGIN)
                        {
                            mMarginFlags |= (sbyte)LEFT_MARGIN_UNDEFINED_MASK;
                            leftMargin = DEFAULT_MARGIN_RESOLVED;
                        }
                        rightMargin = a.getDimensionPixelSize(R.styleable.ViewGroup_MarginLayout_layout_marginRight, UNDEFINED_MARGIN);
                        if (rightMargin == UNDEFINED_MARGIN)
                        {
                            mMarginFlags |= (sbyte)RIGHT_MARGIN_UNDEFINED_MASK;
                            rightMargin = DEFAULT_MARGIN_RESOLVED;
                        }
                    }

                    startMargin = a.getDimensionPixelSize(R.styleable.ViewGroup_MarginLayout_layout_marginStart, DEFAULT_MARGIN_RELATIVE);
                    endMargin = a.getDimensionPixelSize(R.styleable.ViewGroup_MarginLayout_layout_marginEnd, DEFAULT_MARGIN_RELATIVE);

                    if (verticalMargin >= 0)
                    {
                        topMargin = verticalMargin;
                        bottomMargin = verticalMargin;
                    }
                    else
                    {
                        topMargin = a.getDimensionPixelSize(R.styleable.ViewGroup_MarginLayout_layout_marginTop, DEFAULT_MARGIN_RESOLVED);
                        bottomMargin = a.getDimensionPixelSize(R.styleable.ViewGroup_MarginLayout_layout_marginBottom, DEFAULT_MARGIN_RESOLVED);
                    }

                    if (MarginRelative)
                    {
                        mMarginFlags |= (sbyte)NEED_RESOLUTION_MASK;
                    }
                }

                //JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final boolean hasRtlSupport = c.getApplicationInfo().hasRtlSupport();
                bool hasRtlSupport = c.ApplicationInfo.hasRtlSupport();
                //JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final int targetSdkVersion = c.getApplicationInfo().targetSdkVersion;
                int targetSdkVersion = c.ApplicationInfo.targetSdkVersion;
                if (targetSdkVersion < JELLY_BEAN_MR1 || !hasRtlSupport)
                {
                    mMarginFlags |= (sbyte)RTL_COMPATIBILITY_MODE_MASK;
                }

                // Layout direction is LTR by default
                mMarginFlags |= LAYOUT_DIRECTION_LTR;

                a.recycle();
            }*/

            public MarginLayoutParams(int width, int height) : base(width, height)
            {

                mMarginFlags |= (sbyte)LEFT_MARGIN_UNDEFINED_MASK;
                mMarginFlags |= (sbyte)RIGHT_MARGIN_UNDEFINED_MASK;

                mMarginFlags &= (sbyte)(~NEED_RESOLUTION_MASK);
                mMarginFlags &= (sbyte)(~RTL_COMPATIBILITY_MODE_MASK);
            }

            /// <summary>
            /// Copy constructor. Clones the width, height and margin values of the source.
            /// </summary>
            /// <param name="source"> The layout params to copy from. </param>
            public MarginLayoutParams(MarginLayoutParams source)
            {
                this.width = source.width;
                this.height = source.height;

                this.leftMargin = source.leftMargin;
                this.topMargin = source.topMargin;
                this.rightMargin = source.rightMargin;
                this.bottomMargin = source.bottomMargin;
                this.startMargin = source.startMargin;
                this.endMargin = source.endMargin;

                this.mMarginFlags = source.mMarginFlags;
            }

            public MarginLayoutParams(LayoutParams source) : base(source)
            {

                mMarginFlags |= (sbyte)LEFT_MARGIN_UNDEFINED_MASK;
                mMarginFlags |= (sbyte)RIGHT_MARGIN_UNDEFINED_MASK;

                mMarginFlags &= (sbyte)(~NEED_RESOLUTION_MASK);
                mMarginFlags &= (sbyte)(~RTL_COMPATIBILITY_MODE_MASK);
            }

            /// <summary>
            /// @hide Used internally.
            /// </summary>
            public void copyMarginsFrom(MarginLayoutParams source)
            {
                this.leftMargin = source.leftMargin;
                this.topMargin = source.topMargin;
                this.rightMargin = source.rightMargin;
                this.bottomMargin = source.bottomMargin;
                this.startMargin = source.startMargin;
                this.endMargin = source.endMargin;

                this.mMarginFlags = source.mMarginFlags;
            }

            /// <summary>
            /// Sets the margins, in pixels. A call to <seealso cref="android.view.View#requestLayout()"/> needs
            /// to be done so that the new margins are taken into account. Left and right margins may be
            /// overridden by <seealso cref="android.view.View#requestLayout()"/> depending on layout direction.
            /// Margin values should be positive.
            /// </summary>
            /// <param name="left"> the left margin size </param>
            /// <param name="top"> the top margin size </param>
            /// <param name="right"> the right margin size </param>
            /// <param name="bottom"> the bottom margin size
            /// 
            /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginLeft
            /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginTop
            /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginRight
            /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginBottom </param>
            public virtual void setMargins(int left, int top, int right, int bottom)
            {
                leftMargin = left;
                topMargin = top;
                rightMargin = right;
                bottomMargin = bottom;
                mMarginFlags &= (sbyte)(~LEFT_MARGIN_UNDEFINED_MASK);
                mMarginFlags &= (sbyte)(~RIGHT_MARGIN_UNDEFINED_MASK);
                if (MarginRelative)
                {
                    mMarginFlags |= (sbyte)NEED_RESOLUTION_MASK;
                }
                else
                {
                    mMarginFlags &= (sbyte)(~NEED_RESOLUTION_MASK);
                }
            }

            /// <summary>
            /// Sets the relative margins, in pixels. A call to <seealso cref="android.view.View#requestLayout()"/>
            /// needs to be done so that the new relative margins are taken into account. Left and right
            /// margins may be overridden by <seealso cref="android.view.View#requestLayout()"/> depending on
            /// layout direction. Margin values should be positive.
            /// </summary>
            /// <param name="start"> the start margin size </param>
            /// <param name="top"> the top margin size </param>
            /// <param name="end"> the right margin size </param>
            /// <param name="bottom"> the bottom margin size
            /// 
            /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginStart
            /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginTop
            /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginEnd
            /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginBottom
            /// 
            /// @hide </param>
            ///ORIGINAL LINE: @UnsupportedAppUsage public void setMarginsRelative(int start, int top, int end, int bottom)
            public virtual void setMarginsRelative(int start, int top, int end, int bottom)
            {
                startMargin = start;
                topMargin = top;
                endMargin = end;
                bottomMargin = bottom;
                mMarginFlags |= (sbyte)NEED_RESOLUTION_MASK;
            }

            /// <summary>
            /// Sets the relative start margin. Margin values should be positive.
            /// </summary>
            /// <param name="start"> the start margin size
            /// 
            /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginStart </param>
            public virtual int MarginStart
            {
                set
                {
                    startMargin = value;
                    mMarginFlags |= (sbyte)NEED_RESOLUTION_MASK;
                }
                get
                {
                    if (startMargin != DEFAULT_MARGIN_RELATIVE)
                    {
                        return startMargin;
                    }
                    if ((mMarginFlags & NEED_RESOLUTION_MASK) == NEED_RESOLUTION_MASK)
                    {
                        doResolveMargins();
                    }
                    switch (mMarginFlags & LAYOUT_DIRECTION_MASK)
                    {
                        case AndroidView.LAYOUT_DIRECTION_RTL:
                            return rightMargin;
                        case AndroidView.LAYOUT_DIRECTION_LTR:
                        default:
                            return leftMargin;
                    }
                }
            }


            /// <summary>
            /// Sets the relative end margin. Margin values should be positive.
            /// </summary>
            /// <param name="end"> the end margin size
            /// 
            /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginEnd </param>
            public virtual int MarginEnd
            {
                set
                {
                    endMargin = value;
                    mMarginFlags |= (sbyte)NEED_RESOLUTION_MASK;
                }
                get
                {
                    if (endMargin != DEFAULT_MARGIN_RELATIVE)
                    {
                        return endMargin;
                    }
                    if ((mMarginFlags & NEED_RESOLUTION_MASK) == NEED_RESOLUTION_MASK)
                    {
                        doResolveMargins();
                    }
                    switch (mMarginFlags & LAYOUT_DIRECTION_MASK)
                    {
                        case AndroidView.LAYOUT_DIRECTION_RTL:
                            return leftMargin;
                        case AndroidView.LAYOUT_DIRECTION_LTR:
                        default:
                            return rightMargin;
                    }
                }
            }


            /// <summary>
            /// Check if margins are relative.
            /// 
            /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginStart
            /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginEnd
            /// </summary>
            /// <returns> true if either marginStart or marginEnd has been set. </returns>
            public virtual bool MarginRelative
            {
                get
                {
                    return (startMargin != DEFAULT_MARGIN_RELATIVE || endMargin != DEFAULT_MARGIN_RELATIVE);
                }
            }

            /// <summary>
            /// Set the layout direction </summary>
            /// <param name="layoutDirection"> the layout direction.
            ///        Should be either <seealso cref="View#LAYOUT_DIRECTION_LTR"/>
            ///                     or <seealso cref="View#LAYOUT_DIRECTION_RTL"/>. </param>
            public virtual int LayoutDirection
            {
                set
                {
                    if (value != AndroidView.LAYOUT_DIRECTION_LTR && value != AndroidView.LAYOUT_DIRECTION_RTL)
                    {
                        return;
                    }
                    if (value != (mMarginFlags & LAYOUT_DIRECTION_MASK))
                    {
                        mMarginFlags &= (sbyte)(~LAYOUT_DIRECTION_MASK);
                        mMarginFlags |= (sbyte)(value & LAYOUT_DIRECTION_MASK);
                        if (MarginRelative)
                        {
                            mMarginFlags |= (sbyte)NEED_RESOLUTION_MASK;
                        }
                        else
                        {
                            mMarginFlags &= (sbyte)(~NEED_RESOLUTION_MASK);
                        }
                    }
                }
                get
                {
                    return (mMarginFlags & LAYOUT_DIRECTION_MASK);
                }
            }


            /// <summary>
            /// This will be called by <seealso cref="android.view.View#requestLayout()"/>. Left and Right margins
            /// may be overridden depending on layout direction.
            /// </summary>
            public override void resolveLayoutDirection(int layoutDirection)
            {
                LayoutDirection = layoutDirection;

                // No relative margin or pre JB-MR1 case or no need to resolve, just dont do anything
                // Will use the left and right margins if no relative margin is defined.
                if (!MarginRelative || (mMarginFlags & NEED_RESOLUTION_MASK) != NEED_RESOLUTION_MASK)
                {
                    return;
                }

                // Proceed with resolution
                doResolveMargins();
            }

            internal virtual void doResolveMargins()
            {
                if ((mMarginFlags & RTL_COMPATIBILITY_MODE_MASK) == RTL_COMPATIBILITY_MODE_MASK)
                {
                    // if left or right margins are not defined and if we have some start or end margin
                    // defined then use those start and end margins.
                    if ((mMarginFlags & LEFT_MARGIN_UNDEFINED_MASK) == LEFT_MARGIN_UNDEFINED_MASK && startMargin > DEFAULT_MARGIN_RELATIVE)
                    {
                        leftMargin = startMargin;
                    }
                    if ((mMarginFlags & RIGHT_MARGIN_UNDEFINED_MASK) == RIGHT_MARGIN_UNDEFINED_MASK && endMargin > DEFAULT_MARGIN_RELATIVE)
                    {
                        rightMargin = endMargin;
                    }
                }
                else
                {
                    // We have some relative margins (either the start one or the end one or both). So use
                    // them and override what has been defined for left and right margins. If either start
                    // or end margin is not defined, just set it to default "0".
                    switch (mMarginFlags & LAYOUT_DIRECTION_MASK)
                    {
                        case AndroidView.LAYOUT_DIRECTION_RTL:
                            leftMargin = (endMargin > DEFAULT_MARGIN_RELATIVE) ? endMargin : DEFAULT_MARGIN_RESOLVED;
                            rightMargin = (startMargin > DEFAULT_MARGIN_RELATIVE) ? startMargin : DEFAULT_MARGIN_RESOLVED;
                            break;
                        case AndroidView.LAYOUT_DIRECTION_LTR:
                        default:
                            leftMargin = (startMargin > DEFAULT_MARGIN_RELATIVE) ? startMargin : DEFAULT_MARGIN_RESOLVED;
                            rightMargin = (endMargin > DEFAULT_MARGIN_RELATIVE) ? endMargin : DEFAULT_MARGIN_RESOLVED;
                            break;
                    }
                }
                mMarginFlags &= (sbyte)(~NEED_RESOLUTION_MASK);
            }

            /// <summary>
            /// @hide
            /// </summary>
            public virtual bool LayoutRtl
            {
                get
                {
                    return ((mMarginFlags & LAYOUT_DIRECTION_MASK) == AndroidView.LAYOUT_DIRECTION_RTL);
                }
            }
        }

        public class LayoutParams
        {
            /// <summary>
            /// Special value for the height or width requested by a View.
            /// FILL_PARENT means that the view wants to be as big as its parent,
            /// minus the parent's padding, if any. This value is deprecated
            /// starting in API Level 8 and replaced by <seealso cref="#MATCH_PARENT"/>.
            /// </summary>
            ///ORIGINAL LINE: @SuppressWarnings({"UnusedDeclaration"}) @Deprecated public static final int FILL_PARENT = -1;
            [Obsolete]
            public const int FILL_PARENT = -1;

            /// <summary>
            /// Special value for the height or width requested by a View.
            /// MATCH_PARENT means that the view wants to be as big as its parent,
            /// minus the parent's padding, if any. Introduced in API Level 8.
            /// </summary>
            public const int MATCH_PARENT = -1;

            /// <summary>
            /// Special value for the height or width requested by a View.
            /// WRAP_CONTENT means that the view wants to be just large enough to fit
            /// its own internal content, taking its own padding into account.
            /// </summary>
            public const int WRAP_CONTENT = -2;

            /// <summary>
            /// Information about how wide the view wants to be. Can be one of the
            /// constants FILL_PARENT (replaced by MATCH_PARENT
            /// in API Level 8) or WRAP_CONTENT, or an exact size.
            /// </summary>
            ///ORIGINAL LINE: @ViewDebug.ExportedProperty(category = "layout", mapping = { @ViewDebug.IntToString(from = MATCH_PARENT, to = "MATCH_PARENT"), @ViewDebug.IntToString(from = WRAP_CONTENT, to = "WRAP_CONTENT") }) @InspectableProperty(name = "layout_width", enumMapping = { @EnumEntry(name = "match_parent", value = MATCH_PARENT), @EnumEntry(name = "wrap_content", value = WRAP_CONTENT) }) public int width;
            public int width;

            /// <summary>
            /// Information about how tall the view wants to be. Can be one of the
            /// constants FILL_PARENT (replaced by MATCH_PARENT
            /// in API Level 8) or WRAP_CONTENT, or an exact size.
            /// </summary>
            ///ORIGINAL LINE: @ViewDebug.ExportedProperty(category = "layout", mapping = { @ViewDebug.IntToString(from = MATCH_PARENT, to = "MATCH_PARENT"), @ViewDebug.IntToString(from = WRAP_CONTENT, to = "WRAP_CONTENT") }) @InspectableProperty(name = "layout_height", enumMapping = { @EnumEntry(name = "match_parent", value = MATCH_PARENT), @EnumEntry(name = "wrap_content", value = WRAP_CONTENT) }) public int height;
            public int height;

            /// <summary>
            /// Used to animate layouts.
            /// </summary>
            //public LayoutAnimationController.AnimationParameters layoutAnimationParameters;

            /// <summary>
            /// Creates a new set of layout parameters. The values are extracted from
            /// the supplied attributes set and context. The XML attributes mapped
            /// to this set of layout parameters are:
            /// 
            /// <ul>
            ///   <li><code>layout_width</code>: the width, either an exact value,
            ///   <seealso cref="#WRAP_CONTENT"/>, or <seealso cref="#FILL_PARENT"/> (replaced by
            ///   <seealso cref="#MATCH_PARENT"/> in API Level 8)</li>
            ///   <li><code>layout_height</code>: the height, either an exact value,
            ///   <seealso cref="#WRAP_CONTENT"/>, or <seealso cref="#FILL_PARENT"/> (replaced by
            ///   <seealso cref="#MATCH_PARENT"/> in API Level 8)</li>
            /// </ul>
            /// </summary>
            /// <param name="c"> the application environment </param>
            /// <param name="attrs"> the set of attributes from which to extract the layout
            ///              parameters' values </param>
            //public LayoutParams(Context c, AttributeSet attrs)
            //{
            //    TypedArray a = c.obtainStyledAttributes(attrs, R.styleable.ViewGroup_Layout);
            //    setBaseAttributes(a, R.styleable.ViewGroup_Layout_layout_width, R.styleable.ViewGroup_Layout_layout_height);
            //    a.recycle();
            //}

            /// <summary>
            /// Creates a new set of layout parameters with the specified width
            /// and height.
            /// </summary>
            /// <param name="width"> the width, either <seealso cref="#WRAP_CONTENT"/>,
            ///        <seealso cref="#FILL_PARENT"/> (replaced by <seealso cref="#MATCH_PARENT"/> in
            ///        API Level 8), or a fixed size in pixels </param>
            /// <param name="height"> the height, either <seealso cref="#WRAP_CONTENT"/>,
            ///        <seealso cref="#FILL_PARENT"/> (replaced by <seealso cref="#MATCH_PARENT"/> in
            ///        API Level 8), or a fixed size in pixels </param>
            public LayoutParams(int width, int height)
            {
                this.width = width;
                this.height = height;
            }

            /// <summary>
            /// Copy constructor. Clones the width and height values of the source.
            /// </summary>
            /// <param name="source"> The layout params to copy from. </param>
            public LayoutParams(LayoutParams source)
            {
                this.width = source.width;
                this.height = source.height;
            }

            /// <summary>
            /// Used internally by MarginLayoutParams.
            /// @hide
            /// </summary>
            ///ORIGINAL LINE: @UnsupportedAppUsage LayoutParams()
            internal LayoutParams()
            {
            }

            /// <summary>
            /// Extracts the layout parameters from the supplied attributes.
            /// </summary>
            /// <param name="a"> the style attributes to extract the parameters from </param>
            /// <param name="widthAttr"> the identifier of the width attribute </param>
            /// <param name="heightAttr"> the identifier of the height attribute </param>
            //protected internal virtual void setBaseAttributes(TypedArray a, int widthAttr, int heightAttr)
            //{
            //    width = a.getLayoutDimension(widthAttr, "layout_width");
            //    height = a.getLayoutDimension(heightAttr, "layout_height");
            //}

            /// <summary>
            /// Resolve layout parameters depending on the layout direction. Subclasses that care about
            /// layoutDirection changes should override this method. The default implementation does
            /// nothing.
            /// </summary>
            /// <param name="layoutDirection"> the direction of the layout
            /// 
            /// <seealso cref="View#LAYOUT_DIRECTION_LTR"/>
            /// <seealso cref="View#LAYOUT_DIRECTION_RTL"/> </param>
            public virtual void resolveLayoutDirection(int layoutDirection)
            {
            }
        }
    }
}

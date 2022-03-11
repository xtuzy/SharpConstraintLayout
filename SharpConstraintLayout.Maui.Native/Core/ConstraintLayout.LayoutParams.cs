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
using androidx.constraintlayout.core.widgets;
namespace SharpConstraintLayout.Maui.Core
{
    public partial class ConstraintLayout
    {
        /// <summary>
        /// This class contains the different attributes specifying how a view want to be laid out inside
        /// a <seealso cref="ConstraintLayout"/>. For building up constraints at run time, using <seealso cref="ConstraintSet"/> is recommended.
        /// </summary>
        public class LayoutParams : ViewGroup.MarginLayoutParams
        {
            /// <summary>
            /// Dimension will be controlled by constraints.
            /// </summary>
            public const int MATCH_CONSTRAINT = 0;

            /// <summary>
            /// References the id of the parent.
            /// </summary>
            public const int PARENT_ID = 0;

            /// <summary>
            /// Defines an id that is not set.
            /// </summary>
            public const int UNSET = -1;


            /// <summary>
            /// Defines an id that is not set.
            /// </summary>
            public static readonly int GONE_UNSET = int.MinValue;


            /// <summary>
            /// The horizontal orientation.
            /// </summary>
            public const int HORIZONTAL = ConstraintWidget.HORIZONTAL;

            /// <summary>
            /// The vertical orientation.
            /// </summary>
            public const int VERTICAL = ConstraintWidget.VERTICAL;

            /// <summary>
            /// The left side of a view.
            /// </summary>
            public const int LEFT = 1;

            /// <summary>
            /// The right side of a view.
            /// </summary>
            public const int RIGHT = 2;

            /// <summary>
            /// The top of a view.
            /// </summary>
            public const int TOP = 3;

            /// <summary>
            /// The bottom side of a view.
            /// </summary>
            public const int BOTTOM = 4;

            /// <summary>
            /// The baseline of the text in a view.
            /// </summary>
            public const int BASELINE = 5;

            /// <summary>
            /// The left side of a view in left to right languages.
            /// In right to left languages it corresponds to the right side of the view
            /// </summary>
            public const int START = 6;

            /// <summary>
            /// The right side of a view in right to left languages.
            /// In right to left languages it corresponds to the left side of the view
            /// </summary>
            public const int END = 7;

            /// <summary>
            /// Circle reference from a view.
            /// </summary>
            public const int CIRCLE = 8;

            /// <summary>
            /// Set matchConstraintDefault* default to the wrap content size.
            /// Use to set the matchConstraintDefaultWidth and matchConstraintDefaultHeight
            /// </summary>
            public const int MATCH_CONSTRAINT_WRAP = ConstraintWidget.MATCH_CONSTRAINT_WRAP;

            /// <summary>
            /// Set matchConstraintDefault* spread as much as possible within its constraints.
            /// Use to set the matchConstraintDefaultWidth and matchConstraintDefaultHeight
            /// </summary>
            public const int MATCH_CONSTRAINT_SPREAD = ConstraintWidget.MATCH_CONSTRAINT_SPREAD;

            /// <summary>
            /// Set matchConstraintDefault* percent to be based on a percent of another dimension (by default, the parent)
            /// Use to set the matchConstraintDefaultWidth and matchConstraintDefaultHeight
            /// </summary>
            public const int MATCH_CONSTRAINT_PERCENT = ConstraintWidget.MATCH_CONSTRAINT_PERCENT;

            /// <summary>
            /// Chain spread style
            /// </summary>
            public const int CHAIN_SPREAD = ConstraintWidget.CHAIN_SPREAD;

            /// <summary>
            /// Chain spread inside style
            /// </summary>
            public const int CHAIN_SPREAD_INSIDE = ConstraintWidget.CHAIN_SPREAD_INSIDE;

            /// <summary>
            /// Chain packed style
            /// </summary>
            public const int CHAIN_PACKED = ConstraintWidget.CHAIN_PACKED;

            /// <summary>
            /// The distance of child (guideline) to the top or left edge of its parent.
            /// </summary>
            public int guideBegin = UNSET;

            /// <summary>
            /// The distance of child (guideline) to the bottom or right edge of its parent.
            /// </summary>
            public int guideEnd = UNSET;

            /// <summary>
            /// The ratio of the distance to the parent's sides
            /// </summary>
            public float guidePercent = UNSET;

            /// <summary>
            /// Constrains the left side of a child to the left side of a target child (contains the target child id).
            /// </summary>
            public int leftToLeft = UNSET;

            /// <summary>
            /// Constrains the left side of a child to the right side of a target child (contains the target child id).
            /// </summary>
            public int leftToRight = UNSET;

            /// <summary>
            /// Constrains the right side of a child to the left side of a target child (contains the target child id).
            /// </summary>
            public int rightToLeft = UNSET;

            /// <summary>
            /// Constrains the right side of a child to the right side of a target child (contains the target child id).
            /// </summary>
            public int rightToRight = UNSET;

            /// <summary>
            /// Constrains the top side of a child to the top side of a target child (contains the target child id).
            /// </summary>
            public int topToTop = UNSET;

            /// <summary>
            /// Constrains the top side of a child to the bottom side of a target child (contains the target child id).
            /// </summary>
            public int topToBottom = UNSET;

            /// <summary>
            /// Constrains the bottom side of a child to the top side of a target child (contains the target child id).
            /// </summary>
            public int bottomToTop = UNSET;

            /// <summary>
            /// Constrains the bottom side of a child to the bottom side of a target child (contains the target child id).
            /// </summary>
            public int bottomToBottom = UNSET;

            /// <summary>
            /// Constrains the baseline of a child to the baseline of a target child (contains the target child id).
            /// </summary>
            public int baselineToBaseline = UNSET;

            /// <summary>
            /// Constrains the baseline of a child to the top of a target child (contains the target child id).
            /// </summary>
            public int baselineToTop = UNSET;

            /// <summary>
            /// Constrains the baseline of a child to the bottom of a target child (contains the target child id).
            /// </summary>
            public int baselineToBottom = UNSET;

            /// <summary>
            /// Constrains the center of a child to the center of a target child (contains the target child id).
            /// </summary>
            public int circleConstraint = UNSET;

            /// <summary>
            /// The radius used for a circular constraint
            /// </summary>
            public int circleRadius = 0;

            /// <summary>
            /// The angle used for a circular constraint]
            /// </summary>
            public float circleAngle = 0;

            /// <summary>
            /// Constrains the start side of a child to the end side of a target child (contains the target child id).
            /// </summary>
            public int startToEnd = UNSET;

            /// <summary>
            /// Constrains the start side of a child to the start side of a target child (contains the target child id).
            /// </summary>
            public int startToStart = UNSET;

            /// <summary>
            /// Constrains the end side of a child to the start side of a target child (contains the target child id).
            /// </summary>
            public int endToStart = UNSET;

            /// <summary>
            /// Constrains the end side of a child to the end side of a target child (contains the target child id).
            /// </summary>
            public int endToEnd = UNSET;

            /// <summary>
            /// The left margin to use when the target is gone.
            /// </summary>
            public int goneLeftMargin = GONE_UNSET;

            /// <summary>
            /// The top margin to use when the target is gone.
            /// </summary>
            public int goneTopMargin = GONE_UNSET;

            /// <summary>
            /// The right margin to use when the target is gone
            /// </summary>
            public int goneRightMargin = GONE_UNSET;

            /// <summary>
            /// The bottom margin to use when the target is gone.
            /// </summary>
            public int goneBottomMargin = GONE_UNSET;

            /// <summary>
            /// The start margin to use when the target is gone.
            /// </summary>
            public int goneStartMargin = GONE_UNSET;

            /// <summary>
            /// The end margin to use when the target is gone.
            /// </summary>
            public int goneEndMargin = GONE_UNSET;

            /// <summary>
            /// The baseline margin to use when the target is gone.
            /// </summary>
            public int goneBaselineMargin = GONE_UNSET;

            /// <summary>
            /// The baseline margin.
            /// </summary>
            public int baselineMargin = 0;

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Layout margins handling TODO: re-activate in 3.0
            ///////////////////////////////////////////////////////////////////////////////////////////

            /// <summary>
            /// The left margin.
            /// </summary>
            // public int leftMargin = 0;

            /// <summary>
            /// The right margin.
            /// </summary>
            // public int rightMargin = 0;

            // int originalLeftMargin = 0;
            // int originalRightMargin = 0;

            /// <summary>
            /// The top margin.
            /// </summary>
            // public int topMargin = 0;

            /// <summary>
            /// The bottom margin.
            /// </summary>
            // public int bottomMargin = 0;

            /// <summary>
            /// The start margin.
            /// </summary>
            // public int startMargin = UNSET;

            /// <summary>
            /// The end margin.
            /// </summary>
            // public int endMargin = UNSET;

            // boolean isRtl = false;
            // int layoutDirection = ViewCompat.LAYOUT_DIRECTION_LTR;

            internal bool widthSet = true; // need to be set to false when we reactivate this in 3.0
            internal bool heightSet = true; // need to be set to false when we reactivate this in 3.0

            ///////////////////////////////////////////////////////////////////////////////////////////

            /// <summary>
            /// The ratio between two connections when the left and right (or start and end) sides are constrained.
            /// </summary>
            public float horizontalBias = 0.5f;

            /// <summary>
            /// The ratio between two connections when the top and bottom sides are constrained.
            /// </summary>
            public float verticalBias = 0.5f;

            /// <summary>
            /// The ratio information.
            /// </summary>
            public string dimensionRatio = null;

            /// <summary>
            /// The ratio between the width and height of the child.
            /// </summary>
            internal float dimensionRatioValue = 0;

            /// <summary>
            /// The child's side to constrain using dimensRatio.
            /// </summary>
            internal int dimensionRatioSide = VERTICAL;

            /// <summary>
            /// The child's weight that we can use to distribute the available horizontal space
            /// in a chain, if the dimension behaviour is set to MATCH_CONSTRAINT
            /// </summary>
            public float horizontalWeight = UNSET;

            /// <summary>
            /// The child's weight that we can use to distribute the available vertical space
            /// in a chain, if the dimension behaviour is set to MATCH_CONSTRAINT
            /// </summary>
            public float verticalWeight = UNSET;

            /// <summary>
            /// If the child is the start of a horizontal chain, this attribute will drive how
            /// the elements of the chain will be positioned. The possible values are:
            /// <ul>
            /// <li><seealso cref="#CHAIN_SPREAD"/> -- the elements will be spread out</li>
            /// <li><seealso cref="#CHAIN_SPREAD_INSIDE"/> -- similar, but the endpoints of the chain will not
            /// be spread out</li>
            /// <li><seealso cref="#CHAIN_PACKED"/> -- the elements of the chain will be packed together. The
            /// horizontal bias attribute of the child will then affect the positioning of the packed
            /// elements</li>
            /// </ul>
            /// </summary>
            public int horizontalChainStyle = CHAIN_SPREAD;

            /// <summary>
            /// If the child is the start of a vertical chain, this attribute will drive how
            /// the elements of the chain will be positioned. The possible values are:
            /// <ul>
            /// <li><seealso cref="#CHAIN_SPREAD"/> -- the elements will be spread out</li>
            /// <li><seealso cref="#CHAIN_SPREAD_INSIDE"/> -- similar, but the endpoints of the chain will not
            /// be spread out</li>
            /// <li><seealso cref="#CHAIN_PACKED"/> -- the elements of the chain will be packed together. The
            /// vertical bias attribute of the child will then affect the positioning of the packed
            /// elements</li>
            /// </ul>
            /// </summary>
            public int verticalChainStyle = CHAIN_SPREAD;

            /// <summary>
            /// Define how the widget horizontal dimension is handled when set to MATCH_CONSTRAINT
            /// <ul>
            /// <li><seealso cref="#MATCH_CONSTRAINT_SPREAD"/> -- the default. The dimension will expand up to
            /// the constraints, minus margins</li>
            /// <li><seealso cref="#MATCH_CONSTRAINT_WRAP"/> -- DEPRECATED -- use instead WRAP_CONTENT and
            /// constrainedWidth=true<br>
            /// The dimension will be the same as WRAP_CONTENT, unless the size ends
            /// up too large for the constraints; in that case the dimension will expand up to the constraints, minus margins
            /// This attribute may not be applied if the widget is part of a chain in that dimension.</li>
            /// <li><seealso cref="#MATCH_CONSTRAINT_PERCENT"/> -- The dimension will be a percent of another
            /// widget (by default, the parent)</li>
            /// </ul>
            /// </summary>
            public int matchConstraintDefaultWidth = MATCH_CONSTRAINT_SPREAD;

            /// <summary>
            /// Define how the widget vertical dimension is handled when set to MATCH_CONSTRAINT
            /// <ul>
            /// <li><seealso cref="#MATCH_CONSTRAINT_SPREAD"/> -- the default. The dimension will expand up to
            /// the constraints, minus margins</li>
            /// <li><seealso cref="#MATCH_CONSTRAINT_WRAP"/> -- DEPRECATED -- use instead WRAP_CONTENT and
            /// constrainedWidth=true<br>
            /// The dimension will be the same as WRAP_CONTENT, unless the size ends
            /// up too large for the constraints; in that case the dimension will expand up to the constraints, minus margins
            /// This attribute may not be applied if the widget is part of a chain in that dimension.</li>
            /// <li><seealso cref="#MATCH_CONSTRAINT_PERCENT"/> -- The dimension will be a percent of another
            /// widget (by default, the parent)</li>
            /// </ul>
            /// </summary>
            public int matchConstraintDefaultHeight = MATCH_CONSTRAINT_SPREAD;

            /// <summary>
            /// Specify a minimum width size for the widget. It will only apply if the size of the widget
            /// is set to MATCH_CONSTRAINT. Don't apply if the widget is part of a horizontal chain.
            /// </summary>
            public int matchConstraintMinWidth = 0;

            /// <summary>
            /// Specify a minimum height size for the widget. It will only apply if the size of the widget
            /// is set to MATCH_CONSTRAINT. Don't apply if the widget is part of a vertical chain.
            /// </summary>
            public int matchConstraintMinHeight = 0;

            /// <summary>
            /// Specify a maximum width size for the widget. It will only apply if the size of the widget
            /// is set to MATCH_CONSTRAINT. Don't apply if the widget is part of a horizontal chain.
            /// </summary>
            public int matchConstraintMaxWidth = 0;

            /// <summary>
            /// Specify a maximum height size for the widget. It will only apply if the size of the widget
            /// is set to MATCH_CONSTRAINT. Don't apply if the widget is part of a vertical chain.
            /// </summary>
            public int matchConstraintMaxHeight = 0;

            /// <summary>
            /// Specify the percentage when using the match constraint percent mode. From 0 to 1.
            /// </summary>
            public float matchConstraintPercentWidth = 1;

            /// <summary>
            /// Specify the percentage when using the match constraint percent mode. From 0 to 1.
            /// </summary>
            public float matchConstraintPercentHeight = 1;

            /// <summary>
            /// The design time location of the left side of the child.
            /// Used at design time for a horizontally unconstrained child.
            /// </summary>
            public int editorAbsoluteX = UNSET;

            /// <summary>
            /// The design time location of the right side of the child.
            /// Used at design time for a vertically unconstrained child.
            /// </summary>
            public int editorAbsoluteY = UNSET;

            public int orientation = UNSET;

            /// <summary>
            /// Specify if the horizontal dimension is constrained in case both left & right constraints are set
            /// and the widget dimension is not a fixed dimension. By default, if a widget is set to WRAP_CONTENT,
            /// we will treat that dimension as a fixed dimension, meaning the dimension will not change regardless
            /// of constraints. Setting this attribute to true allows the dimension to change in order to respect
            /// constraints.
            /// </summary>
            public bool constrainedWidth = false;

            /// <summary>
            /// Specify if the vertical dimension is constrained in case both top & bottom constraints are set
            /// and the widget dimension is not a fixed dimension. By default, if a widget is set to WRAP_CONTENT,
            /// we will treat that dimension as a fixed dimension, meaning the dimension will not change regardless
            /// of constraints. Setting this attribute to true allows the dimension to change in order to respect
            /// constraints.
            /// </summary>
            public bool constrainedHeight = false;

            /// <summary>
            /// Define a category of view to be used by helpers and motionLayout
            /// </summary>
            public string constraintTag = null;

            public const int WRAP_BEHAVIOR_INCLUDED = ConstraintWidget.WRAP_BEHAVIOR_INCLUDED;
            public const int WRAP_BEHAVIOR_HORIZONTAL_ONLY = ConstraintWidget.WRAP_BEHAVIOR_HORIZONTAL_ONLY;
            public const int WRAP_BEHAVIOR_VERTICAL_ONLY = ConstraintWidget.WRAP_BEHAVIOR_VERTICAL_ONLY;
            public const int WRAP_BEHAVIOR_SKIPPED = ConstraintWidget.WRAP_BEHAVIOR_SKIPPED;

            /// <summary>
            /// Specify how this view is taken in account during the parent's wrap computation
            /// 
            /// Can be either of:
            /// WRAP_BEHAVIOR_INCLUDED the widget is taken in account for the wrap (default)
            /// WRAP_BEHAVIOR_HORIZONTAL_ONLY the widget will be included in the wrap only horizontally
            /// WRAP_BEHAVIOR_VERTICAL_ONLY the widget will be included in the wrap only vertically
            /// WRAP_BEHAVIOR_SKIPPED the widget is not part of the wrap computation
            /// </summary>
            public int wrapBehaviorInParent = WRAP_BEHAVIOR_INCLUDED;

            // Internal use only
            internal bool horizontalDimensionFixed = true;
            internal bool verticalDimensionFixed = true;

            internal bool needsBaseline = false;
            internal bool isGuideline = false;
            internal bool isHelper = false;
            internal bool isInPlaceholder = false;
            internal bool isVirtualGroup = false;

            internal int resolvedLeftToLeft = UNSET;
            internal int resolvedLeftToRight = UNSET;
            internal int resolvedRightToLeft = UNSET;
            internal int resolvedRightToRight = UNSET;
            internal int resolveGoneLeftMargin = GONE_UNSET;
            internal int resolveGoneRightMargin = GONE_UNSET;
            internal float resolvedHorizontalBias = 0.5f;

            internal int resolvedGuideBegin;
            internal int resolvedGuideEnd;
            internal float resolvedGuidePercent;

            internal ConstraintWidget widget = new ConstraintWidget();

            /// <summary>
            /// @suppress
            /// </summary>
            public virtual ConstraintWidget ConstraintWidget
            {
                get
                {
                    return widget;
                }
            }

            /// <param name="text">
            /// @suppress </param>
            public virtual string WidgetDebugName
            {
                set
                {
                    widget.DebugName = value;
                }
            }

            public virtual void reset()
            {
                if (widget != null)
                {
                    widget.reset();
                }
            }

            public bool helped = false;

            /// <summary>
            /// Create a LayoutParams base on an existing layout Params
            /// </summary>
            /// <param name="source"> the Layout Params to be copied </param>
            public LayoutParams(LayoutParams source):base(source)
            {

                ///////////////////////////////////////////////////////////////////////////////////////////
                // Layout margins handling TODO: re-activate in 3.0
                ///////////////////////////////////////////////////////////////////////////////////////////
                // this.layoutDirection = source.layoutDirection;
                // this.isRtl = source.isRtl;
                // this.originalLeftMargin = source.originalLeftMargin;
                // this.originalRightMargin = source.originalRightMargin;
                // this.startMargin = source.startMargin;
                // this.endMargin = source.endMargin;
                // this.leftMargin = source.leftMargin;
                // this.rightMargin = source.rightMargin;
                // this.topMargin = source.topMargin;
                // this.bottomMargin = source.bottomMargin;
                ///////////////////////////////////////////////////////////////////////////////////////////

                this.guideBegin = source.guideBegin;
                this.guideEnd = source.guideEnd;
                this.guidePercent = source.guidePercent;
                this.leftToLeft = source.leftToLeft;
                this.leftToRight = source.leftToRight;
                this.rightToLeft = source.rightToLeft;
                this.rightToRight = source.rightToRight;
                this.topToTop = source.topToTop;
                this.topToBottom = source.topToBottom;
                this.bottomToTop = source.bottomToTop;
                this.bottomToBottom = source.bottomToBottom;
                this.baselineToBaseline = source.baselineToBaseline;
                this.baselineToTop = source.baselineToTop;
                this.baselineToBottom = source.baselineToBottom;
                this.circleConstraint = source.circleConstraint;
                this.circleRadius = source.circleRadius;
                this.circleAngle = source.circleAngle;
                this.startToEnd = source.startToEnd;
                this.startToStart = source.startToStart;
                this.endToStart = source.endToStart;
                this.endToEnd = source.endToEnd;
                this.goneLeftMargin = source.goneLeftMargin;
                this.goneTopMargin = source.goneTopMargin;
                this.goneRightMargin = source.goneRightMargin;
                this.goneBottomMargin = source.goneBottomMargin;
                this.goneStartMargin = source.goneStartMargin;
                this.goneEndMargin = source.goneEndMargin;
                this.goneBaselineMargin = source.goneBaselineMargin;
                this.baselineMargin = source.baselineMargin;
                this.horizontalBias = source.horizontalBias;
                this.verticalBias = source.verticalBias;
                this.dimensionRatio = source.dimensionRatio;
                this.dimensionRatioValue = source.dimensionRatioValue;
                this.dimensionRatioSide = source.dimensionRatioSide;
                this.horizontalWeight = source.horizontalWeight;
                this.verticalWeight = source.verticalWeight;
                this.horizontalChainStyle = source.horizontalChainStyle;
                this.verticalChainStyle = source.verticalChainStyle;
                this.constrainedWidth = source.constrainedWidth;
                this.constrainedHeight = source.constrainedHeight;
                this.matchConstraintDefaultWidth = source.matchConstraintDefaultWidth;
                this.matchConstraintDefaultHeight = source.matchConstraintDefaultHeight;
                this.matchConstraintMinWidth = source.matchConstraintMinWidth;
                this.matchConstraintMaxWidth = source.matchConstraintMaxWidth;
                this.matchConstraintMinHeight = source.matchConstraintMinHeight;
                this.matchConstraintMaxHeight = source.matchConstraintMaxHeight;
                this.matchConstraintPercentWidth = source.matchConstraintPercentWidth;
                this.matchConstraintPercentHeight = source.matchConstraintPercentHeight;
                this.editorAbsoluteX = source.editorAbsoluteX;
                this.editorAbsoluteY = source.editorAbsoluteY;
                this.orientation = source.orientation;
                this.horizontalDimensionFixed = source.horizontalDimensionFixed;
                this.verticalDimensionFixed = source.verticalDimensionFixed;
                this.needsBaseline = source.needsBaseline;
                this.isGuideline = source.isGuideline;
                this.resolvedLeftToLeft = source.resolvedLeftToLeft;
                this.resolvedLeftToRight = source.resolvedLeftToRight;
                this.resolvedRightToLeft = source.resolvedRightToLeft;
                this.resolvedRightToRight = source.resolvedRightToRight;
                this.resolveGoneLeftMargin = source.resolveGoneLeftMargin;
                this.resolveGoneRightMargin = source.resolveGoneRightMargin;
                this.resolvedHorizontalBias = source.resolvedHorizontalBias;
                this.constraintTag = source.constraintTag;
                this.wrapBehaviorInParent = source.wrapBehaviorInParent;
                this.widget = source.widget;
                this.widthSet = source.widthSet;
                this.heightSet = source.heightSet;
            }

            private class Table
            {
                public const int UNUSED = 0;
                public const int ANDROID_ORIENTATION = 1;
                public const int LAYOUT_CONSTRAINT_CIRCLE = 2;
                public const int LAYOUT_CONSTRAINT_CIRCLE_RADIUS = 3;
                public const int LAYOUT_CONSTRAINT_CIRCLE_ANGLE = 4;
                public const int LAYOUT_CONSTRAINT_GUIDE_BEGIN = 5;
                public const int LAYOUT_CONSTRAINT_GUIDE_END = 6;
                public const int LAYOUT_CONSTRAINT_GUIDE_PERCENT = 7;
                public const int LAYOUT_CONSTRAINT_LEFT_TO_LEFT_OF = 8;
                public const int LAYOUT_CONSTRAINT_LEFT_TO_RIGHT_OF = 9;
                public const int LAYOUT_CONSTRAINT_RIGHT_TO_LEFT_OF = 10;
                public const int LAYOUT_CONSTRAINT_RIGHT_TO_RIGHT_OF = 11;
                public const int LAYOUT_CONSTRAINT_TOP_TO_TOP_OF = 12;
                public const int LAYOUT_CONSTRAINT_TOP_TO_BOTTOM_OF = 13;
                public const int LAYOUT_CONSTRAINT_BOTTOM_TO_TOP_OF = 14;
                public const int LAYOUT_CONSTRAINT_BOTTOM_TO_BOTTOM_OF = 15;
                public const int LAYOUT_CONSTRAINT_BASELINE_TO_BASELINE_OF = 16;
                public const int LAYOUT_CONSTRAINT_START_TO_END_OF = 17;
                public const int LAYOUT_CONSTRAINT_START_TO_START_OF = 18;
                public const int LAYOUT_CONSTRAINT_END_TO_START_OF = 19;
                public const int LAYOUT_CONSTRAINT_END_TO_END_OF = 20;
                public const int LAYOUT_GONE_MARGIN_LEFT = 21;
                public const int LAYOUT_GONE_MARGIN_TOP = 22;
                public const int LAYOUT_GONE_MARGIN_RIGHT = 23;
                public const int LAYOUT_GONE_MARGIN_BOTTOM = 24;
                public const int LAYOUT_GONE_MARGIN_START = 25;
                public const int LAYOUT_GONE_MARGIN_END = 26;
                public const int LAYOUT_CONSTRAINED_WIDTH = 27;
                public const int LAYOUT_CONSTRAINED_HEIGHT = 28;
                public const int LAYOUT_CONSTRAINT_HORIZONTAL_BIAS = 29;
                public const int LAYOUT_CONSTRAINT_VERTICAL_BIAS = 30;
                public const int LAYOUT_CONSTRAINT_WIDTH_DEFAULT = 31;
                public const int LAYOUT_CONSTRAINT_HEIGHT_DEFAULT = 32;
                public const int LAYOUT_CONSTRAINT_WIDTH_MIN = 33;
                public const int LAYOUT_CONSTRAINT_WIDTH_MAX = 34;
                public const int LAYOUT_CONSTRAINT_WIDTH_PERCENT = 35;
                public const int LAYOUT_CONSTRAINT_HEIGHT_MIN = 36;
                public const int LAYOUT_CONSTRAINT_HEIGHT_MAX = 37;
                public const int LAYOUT_CONSTRAINT_HEIGHT_PERCENT = 38;
                public const int LAYOUT_CONSTRAINT_LEFT_CREATOR = 39;
                public const int LAYOUT_CONSTRAINT_TOP_CREATOR = 40;
                public const int LAYOUT_CONSTRAINT_RIGHT_CREATOR = 41;
                public const int LAYOUT_CONSTRAINT_BOTTOM_CREATOR = 42;
                public const int LAYOUT_CONSTRAINT_BASELINE_CREATOR = 43;
                public const int LAYOUT_CONSTRAINT_DIMENSION_RATIO = 44;
                public const int LAYOUT_CONSTRAINT_HORIZONTAL_WEIGHT = 45;
                public const int LAYOUT_CONSTRAINT_VERTICAL_WEIGHT = 46;
                public const int LAYOUT_CONSTRAINT_HORIZONTAL_CHAINSTYLE = 47;
                public const int LAYOUT_CONSTRAINT_VERTICAL_CHAINSTYLE = 48;
                public const int LAYOUT_EDITOR_ABSOLUTEX = 49;
                public const int LAYOUT_EDITOR_ABSOLUTEY = 50;
                public const int LAYOUT_CONSTRAINT_TAG = 51;
                public const int LAYOUT_CONSTRAINT_BASELINE_TO_TOP_OF = 52;
                public const int LAYOUT_CONSTRAINT_BASELINE_TO_BOTTOM_OF = 53;
                public const int LAYOUT_MARGIN_BASELINE = 54;
                public const int LAYOUT_GONE_MARGIN_BASELINE = 55;
                ///////////////////////////////////////////////////////////////////////////////////////////
                // Layout margins handling TODO: re-activate in 3.0
                ///////////////////////////////////////////////////////////////////////////////////////////
                // public static final int LAYOUT_MARGIN_LEFT = 56;
                // public static final int LAYOUT_MARGIN_RIGHT = 57;
                // public static final int LAYOUT_MARGIN_TOP = 58;
                // public static final int LAYOUT_MARGIN_BOTTOM = 59;
                // public static final int LAYOUT_MARGIN_START = 60;
                // public static final int LAYOUT_MARGIN_END = 61;
                // public static final int LAYOUT_WIDTH = 62;
                // public static final int LAYOUT_HEIGHT = 63;
                ///////////////////////////////////////////////////////////////////////////////////////////
                public const int LAYOUT_CONSTRAINT_WIDTH = 64;
                public const int LAYOUT_CONSTRAINT_HEIGHT = 65;
                public const int LAYOUT_WRAP_BEHAVIOR_IN_PARENT = 66;

                public static readonly Dictionary<int,int> map = new Dictionary<int, int>();

               /* static Table()
                {
                    ///////////////////////////////////////////////////////////////////////////////////////////
                    // Layout margins handling TODO: re-activate in 3.0
                    ///////////////////////////////////////////////////////////////////////////////////////////
                    // map.append(R.styleable.ConstraintLayout_Layout_android_layout_width, LAYOUT_WIDTH);
                    // map.append(R.styleable.ConstraintLayout_Layout_android_layout_height, LAYOUT_HEIGHT);
                    // map.append(R.styleable.ConstraintLayout_Layout_android_layout_marginLeft, LAYOUT_MARGIN_LEFT);
                    // map.append(R.styleable.ConstraintLayout_Layout_android_layout_marginRight, LAYOUT_MARGIN_RIGHT);
                    // map.append(R.styleable.ConstraintLayout_Layout_android_layout_marginTop, LAYOUT_MARGIN_TOP);
                    // map.append(R.styleable.ConstraintLayout_Layout_android_layout_marginBottom, LAYOUT_MARGIN_BOTTOM);
                    // map.append(R.styleable.ConstraintLayout_Layout_android_layout_marginStart, LAYOUT_MARGIN_START);
                    // map.append(R.styleable.ConstraintLayout_Layout_android_layout_marginEnd, LAYOUT_MARGIN_END);
                    ///////////////////////////////////////////////////////////////////////////////////////////
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintWidth, LAYOUT_CONSTRAINT_WIDTH);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintHeight, LAYOUT_CONSTRAINT_HEIGHT);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintLeft_toLeftOf, LAYOUT_CONSTRAINT_LEFT_TO_LEFT_OF);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintLeft_toRightOf, LAYOUT_CONSTRAINT_LEFT_TO_RIGHT_OF);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintRight_toLeftOf, LAYOUT_CONSTRAINT_RIGHT_TO_LEFT_OF);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintRight_toRightOf, LAYOUT_CONSTRAINT_RIGHT_TO_RIGHT_OF);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintTop_toTopOf, LAYOUT_CONSTRAINT_TOP_TO_TOP_OF);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintTop_toBottomOf, LAYOUT_CONSTRAINT_TOP_TO_BOTTOM_OF);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintBottom_toTopOf, LAYOUT_CONSTRAINT_BOTTOM_TO_TOP_OF);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintBottom_toBottomOf, LAYOUT_CONSTRAINT_BOTTOM_TO_BOTTOM_OF);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintBaseline_toBaselineOf, LAYOUT_CONSTRAINT_BASELINE_TO_BASELINE_OF);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintBaseline_toTopOf, LAYOUT_CONSTRAINT_BASELINE_TO_TOP_OF);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintBaseline_toBottomOf, LAYOUT_CONSTRAINT_BASELINE_TO_BOTTOM_OF);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintCircle, LAYOUT_CONSTRAINT_CIRCLE);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintCircleRadius, LAYOUT_CONSTRAINT_CIRCLE_RADIUS);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintCircleAngle, LAYOUT_CONSTRAINT_CIRCLE_ANGLE);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_editor_absoluteX, LAYOUT_EDITOR_ABSOLUTEX);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_editor_absoluteY, LAYOUT_EDITOR_ABSOLUTEY);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintGuide_begin, LAYOUT_CONSTRAINT_GUIDE_BEGIN);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintGuide_end, LAYOUT_CONSTRAINT_GUIDE_END);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintGuide_percent, LAYOUT_CONSTRAINT_GUIDE_PERCENT);
                    map.append(R.styleable.ConstraintLayout_Layout_android_orientation, ANDROID_ORIENTATION);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintStart_toEndOf, LAYOUT_CONSTRAINT_START_TO_END_OF);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintStart_toStartOf, LAYOUT_CONSTRAINT_START_TO_START_OF);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintEnd_toStartOf, LAYOUT_CONSTRAINT_END_TO_START_OF);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintEnd_toEndOf, LAYOUT_CONSTRAINT_END_TO_END_OF);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_goneMarginLeft, LAYOUT_GONE_MARGIN_LEFT);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_goneMarginTop, LAYOUT_GONE_MARGIN_TOP);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_goneMarginRight, LAYOUT_GONE_MARGIN_RIGHT);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_goneMarginBottom, LAYOUT_GONE_MARGIN_BOTTOM);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_goneMarginStart, LAYOUT_GONE_MARGIN_START);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_goneMarginEnd, LAYOUT_GONE_MARGIN_END);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_goneMarginBaseline, LAYOUT_GONE_MARGIN_BASELINE);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_marginBaseline, LAYOUT_MARGIN_BASELINE);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintHorizontal_bias, LAYOUT_CONSTRAINT_HORIZONTAL_BIAS);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintVertical_bias, LAYOUT_CONSTRAINT_VERTICAL_BIAS);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintDimensionRatio, LAYOUT_CONSTRAINT_DIMENSION_RATIO);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintHorizontal_weight, LAYOUT_CONSTRAINT_HORIZONTAL_WEIGHT);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintVertical_weight, LAYOUT_CONSTRAINT_VERTICAL_WEIGHT);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintHorizontal_chainStyle, LAYOUT_CONSTRAINT_HORIZONTAL_CHAINSTYLE);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintVertical_chainStyle, LAYOUT_CONSTRAINT_VERTICAL_CHAINSTYLE);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constrainedWidth, LAYOUT_CONSTRAINED_WIDTH);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constrainedHeight, LAYOUT_CONSTRAINED_HEIGHT);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintWidth_default, LAYOUT_CONSTRAINT_WIDTH_DEFAULT);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintHeight_default, LAYOUT_CONSTRAINT_HEIGHT_DEFAULT);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintWidth_min, LAYOUT_CONSTRAINT_WIDTH_MIN);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintWidth_max, LAYOUT_CONSTRAINT_WIDTH_MAX);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintWidth_percent, LAYOUT_CONSTRAINT_WIDTH_PERCENT);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintHeight_min, LAYOUT_CONSTRAINT_HEIGHT_MIN);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintHeight_max, LAYOUT_CONSTRAINT_HEIGHT_MAX);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintHeight_percent, LAYOUT_CONSTRAINT_HEIGHT_PERCENT);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintLeft_creator, LAYOUT_CONSTRAINT_LEFT_CREATOR);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintTop_creator, LAYOUT_CONSTRAINT_TOP_CREATOR);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintRight_creator, LAYOUT_CONSTRAINT_RIGHT_CREATOR);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintBottom_creator, LAYOUT_CONSTRAINT_BOTTOM_CREATOR);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintBaseline_creator, LAYOUT_CONSTRAINT_BASELINE_CREATOR);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintTag, LAYOUT_CONSTRAINT_TAG);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_wrapBehaviorInParent, LAYOUT_WRAP_BEHAVIOR_IN_PARENT);
                }*/
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Layout margins handling TODO: re-activate in 3.0
            ///////////////////////////////////////////////////////////////////////////////////////////
            /*
            public void setMarginStart(int start) {
                startMargin = start;
            }
    
            public void setMarginEnd(int end) {
                endMargin = end;
            }
    
            public int getMarginStart() {
                return startMargin;
            }
    
            public int getMarginEnd() {
                return endMargin;
            }
    
            public int getLayoutDirection() {
                return layoutDirection;
            }
            */
            ///////////////////////////////////////////////////////////////////////////////////////////


            //public LayoutParams(Context c, AttributeSet attrs) : base(c, attrs)
            //{

            //    TypedArray a = c.obtainStyledAttributes(attrs, R.styleable.ConstraintLayout_Layout);
            //    //JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
            //    //ORIGINAL LINE: final int N = a.getIndexCount();
            //    int N = a.IndexCount;

            //    ///////////////////////////////////////////////////////////////////////////////////////////
            //    // Layout margins handling TODO: re-activate in 3.0
            //    ///////////////////////////////////////////////////////////////////////////////////////////
            //    // super(WRAP_CONTENT, WRAP_CONTENT);
            //    /*
            //    if (N == 0) {
            //       // check if it's an include
            //       throw new IllegalArgumentException("Invalid LayoutParams supplied to " + this);
            //    }
    
            //    // let's first apply full margins if they are present.
            //    int margin = a.getDimensionPixelSize(R.styleable.ConstraintLayout_Layout_android_layout_margin, -1);
            //    int horizontalMargin = -1;
            //    int verticalMargin = -1;
            //    if (margin >= 0) {
            //        originalLeftMargin = margin;
            //        originalRightMargin = margin;
            //        topMargin = margin;
            //        bottomMargin = margin;
            //    } else {
            //        horizontalMargin = a.getDimensionPixelSize(R.styleable.ConstraintLayout_Layout_android_layout_marginHorizontal, -1);
            //        verticalMargin = a.getDimensionPixelSize(R.styleable.ConstraintLayout_Layout_android_layout_marginVertical, -1);
            //        if (horizontalMargin >= 0) {
            //            originalLeftMargin = horizontalMargin;
            //            originalRightMargin = horizontalMargin;
            //        }
            //        if (verticalMargin >= 0) {
            //            topMargin = verticalMargin;
            //            bottomMargin = verticalMargin;
            //        }
            //    }
            //    */
            //    ///////////////////////////////////////////////////////////////////////////////////////////

            //    for (int i = 0; i < N; i++)
            //    {
            //        int attr = a.getIndex(i);
            //        int look = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.map.get(attr);
            //        switch (look)
            //        {
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.UNUSED:
            //                {
            //                    // Skip
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_WIDTH:
            //                {
            //                    ConstraintSet.parseDimensionConstraints(this, a, attr, HORIZONTAL);
            //                    widthSet = true;
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_HEIGHT:
            //                {
            //                    ConstraintSet.parseDimensionConstraints(this, a, attr, VERTICAL);
            //                    heightSet = true;
            //                    break;
            //                }
            //            ///////////////////////////////////////////////////////////////////////////////////////////
            //            // Layout margins handling TODO: re-activate in 3.0
            //            ///////////////////////////////////////////////////////////////////////////////////////////
            //            /*
            //            case Table.LAYOUT_WIDTH: {
            //                width = a.getLayoutDimension(R.styleable.ConstraintLayout_Layout_android_layout_width, "layout_width");
            //                widthSet = true;
            //                break;
            //            }
            //            case Table.LAYOUT_HEIGHT: {
            //                height = a.getLayoutDimension(R.styleable.ConstraintLayout_Layout_android_layout_height, "layout_height");
            //                heightSet = true;
            //                break;
            //            }
            //            */
            //            ///////////////////////////////////////////////////////////////////////////////////////////
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_WRAP_BEHAVIOR_IN_PARENT:
            //                {
            //                    wrapBehaviorInParent = a.getInt(attr, wrapBehaviorInParent);
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_LEFT_TO_LEFT_OF:
            //                {
            //                    leftToLeft = a.getResourceId(attr, leftToLeft);
            //                    if (leftToLeft == UNSET)
            //                    {
            //                        leftToLeft = a.getInt(attr, UNSET);
            //                    }
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_LEFT_TO_RIGHT_OF:
            //                {
            //                    leftToRight = a.getResourceId(attr, leftToRight);
            //                    if (leftToRight == UNSET)
            //                    {
            //                        leftToRight = a.getInt(attr, UNSET);
            //                    }
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_RIGHT_TO_LEFT_OF:
            //                {
            //                    rightToLeft = a.getResourceId(attr, rightToLeft);
            //                    if (rightToLeft == UNSET)
            //                    {
            //                        rightToLeft = a.getInt(attr, UNSET);
            //                    }
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_RIGHT_TO_RIGHT_OF:
            //                {
            //                    rightToRight = a.getResourceId(attr, rightToRight);
            //                    if (rightToRight == UNSET)
            //                    {
            //                        rightToRight = a.getInt(attr, UNSET);
            //                    }
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_TOP_TO_TOP_OF:
            //                {
            //                    topToTop = a.getResourceId(attr, topToTop);
            //                    if (topToTop == UNSET)
            //                    {
            //                        topToTop = a.getInt(attr, UNSET);
            //                    }
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_TOP_TO_BOTTOM_OF:
            //                {
            //                    topToBottom = a.getResourceId(attr, topToBottom);
            //                    if (topToBottom == UNSET)
            //                    {
            //                        topToBottom = a.getInt(attr, UNSET);
            //                    }
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_BOTTOM_TO_TOP_OF:
            //                {
            //                    bottomToTop = a.getResourceId(attr, bottomToTop);
            //                    if (bottomToTop == UNSET)
            //                    {
            //                        bottomToTop = a.getInt(attr, UNSET);
            //                    }
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_BOTTOM_TO_BOTTOM_OF:
            //                {
            //                    bottomToBottom = a.getResourceId(attr, bottomToBottom);
            //                    if (bottomToBottom == UNSET)
            //                    {
            //                        bottomToBottom = a.getInt(attr, UNSET);
            //                    }
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_BASELINE_TO_BASELINE_OF:
            //                {
            //                    baselineToBaseline = a.getResourceId(attr, baselineToBaseline);
            //                    if (baselineToBaseline == UNSET)
            //                    {
            //                        baselineToBaseline = a.getInt(attr, UNSET);
            //                    }
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_BASELINE_TO_TOP_OF:
            //                {
            //                    baselineToTop = a.getResourceId(attr, baselineToTop);
            //                    if (baselineToTop == UNSET)
            //                    {
            //                        baselineToTop = a.getInt(attr, UNSET);
            //                    }
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_BASELINE_TO_BOTTOM_OF:
            //                {
            //                    baselineToBottom = a.getResourceId(attr, baselineToBottom);
            //                    if (baselineToBottom == UNSET)
            //                    {
            //                        baselineToBottom = a.getInt(attr, UNSET);
            //                    }
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_CIRCLE:
            //                {
            //                    circleConstraint = a.getResourceId(attr, circleConstraint);
            //                    if (circleConstraint == UNSET)
            //                    {
            //                        circleConstraint = a.getInt(attr, UNSET);
            //                    }
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_CIRCLE_RADIUS:
            //                {
            //                    circleRadius = a.getDimensionPixelSize(attr, circleRadius);
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_CIRCLE_ANGLE:
            //                {
            //                    circleAngle = a.getFloat(attr, circleAngle) % 360;
            //                    if (circleAngle < 0)
            //                    {
            //                        circleAngle = (360 - circleAngle) % 360;
            //                    }
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_EDITOR_ABSOLUTEX:
            //                {
            //                    editorAbsoluteX = a.getDimensionPixelOffset(attr, editorAbsoluteX);
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_EDITOR_ABSOLUTEY:
            //                {
            //                    editorAbsoluteY = a.getDimensionPixelOffset(attr, editorAbsoluteY);
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_GUIDE_BEGIN:
            //                {
            //                    guideBegin = a.getDimensionPixelOffset(attr, guideBegin);
            //                    break;
            //                }

            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_GUIDE_END:
            //                {
            //                    guideEnd = a.getDimensionPixelOffset(attr, guideEnd);
            //                    break;
            //                }

            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_GUIDE_PERCENT:
            //                {
            //                    guidePercent = a.getFloat(attr, guidePercent);
            //                    break;
            //                }

            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.ANDROID_ORIENTATION:
            //                {
            //                    orientation = a.getInt(attr, orientation);
            //                    break;
            //                }

            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_START_TO_END_OF:
            //                {
            //                    startToEnd = a.getResourceId(attr, startToEnd);
            //                    if (startToEnd == UNSET)
            //                    {
            //                        startToEnd = a.getInt(attr, UNSET);
            //                    }
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_START_TO_START_OF:
            //                {
            //                    startToStart = a.getResourceId(attr, startToStart);
            //                    if (startToStart == UNSET)
            //                    {
            //                        startToStart = a.getInt(attr, UNSET);
            //                    }
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_END_TO_START_OF:
            //                {
            //                    endToStart = a.getResourceId(attr, endToStart);
            //                    if (endToStart == UNSET)
            //                    {
            //                        endToStart = a.getInt(attr, UNSET);
            //                    }
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_END_TO_END_OF:
            //                {
            //                    endToEnd = a.getResourceId(attr, endToEnd);
            //                    if (endToEnd == UNSET)
            //                    {
            //                        endToEnd = a.getInt(attr, UNSET);
            //                    }
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_GONE_MARGIN_LEFT:
            //                {
            //                    goneLeftMargin = a.getDimensionPixelSize(attr, goneLeftMargin);
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_GONE_MARGIN_TOP:
            //                {
            //                    goneTopMargin = a.getDimensionPixelSize(attr, goneTopMargin);
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_GONE_MARGIN_RIGHT:
            //                {
            //                    goneRightMargin = a.getDimensionPixelSize(attr, goneRightMargin);
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_GONE_MARGIN_BOTTOM:
            //                {
            //                    goneBottomMargin = a.getDimensionPixelSize(attr, goneBottomMargin);
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_GONE_MARGIN_START:
            //                {
            //                    goneStartMargin = a.getDimensionPixelSize(attr, goneStartMargin);
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_GONE_MARGIN_END:
            //                {
            //                    goneEndMargin = a.getDimensionPixelSize(attr, goneEndMargin);
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_GONE_MARGIN_BASELINE:
            //                {
            //                    goneBaselineMargin = a.getDimensionPixelSize(attr, goneBaselineMargin);
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_MARGIN_BASELINE:
            //                {
            //                    baselineMargin = a.getDimensionPixelSize(attr, baselineMargin);
            //                    break;
            //                }
            //            ///////////////////////////////////////////////////////////////////////////////////////////
            //            // Layout margins handling TODO: re-activate in 3.0
            //            ///////////////////////////////////////////////////////////////////////////////////////////
            //            /*
            //            case Table.LAYOUT_MARGIN_LEFT: {
            //                if (margin == -1 && horizontalMargin == -1) {
            //                    originalLeftMargin = a.getDimensionPixelSize(attr, originalLeftMargin);
            //                }
            //                break;
            //            }
            //            case Table.LAYOUT_MARGIN_RIGHT: {
            //                if (margin == -1 && horizontalMargin == -1) {
            //                    originalRightMargin = a.getDimensionPixelSize(attr, originalRightMargin);
            //                }
            //                break;
            //            }
            //            case Table.LAYOUT_MARGIN_TOP: {
            //                if (margin == -1 && verticalMargin == -1) {
            //                    topMargin = a.getDimensionPixelSize(attr, topMargin);
            //                }
            //                break;
            //            }
            //            case Table.LAYOUT_MARGIN_BOTTOM: {
            //                if (margin == -1 && verticalMargin == -1) {
            //                    bottomMargin = a.getDimensionPixelSize(attr, bottomMargin);
            //                }
            //                break;
            //            }
            //            case Table.LAYOUT_MARGIN_START: {
            //                if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1) {
            //                    if (margin == -1 && horizontalMargin == -1) {
            //                        startMargin = a.getDimensionPixelSize(attr, startMargin);
            //                    }
            //                }
            //                break;
            //            }
            //            case Table.LAYOUT_MARGIN_END: {
            //                if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1) {
            //                    if (margin == -1 && horizontalMargin == -1) {
            //                        endMargin = a.getDimensionPixelSize(attr, endMargin);
            //                    }
            //                }
            //                break;
            //            }
            //            */
            //            ///////////////////////////////////////////////////////////////////////////////////////////
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_HORIZONTAL_BIAS:
            //                {
            //                    horizontalBias = a.getFloat(attr, horizontalBias);
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_VERTICAL_BIAS:
            //                {
            //                    verticalBias = a.getFloat(attr, verticalBias);
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_DIMENSION_RATIO:
            //                {
            //                    ConstraintSet.parseDimensionRatioString(this, a.getString(attr));
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_HORIZONTAL_WEIGHT:
            //                {
            //                    horizontalWeight = a.getFloat(attr, horizontalWeight);
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_VERTICAL_WEIGHT:
            //                {
            //                    verticalWeight = a.getFloat(attr, verticalWeight);
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_HORIZONTAL_CHAINSTYLE:
            //                {
            //                    horizontalChainStyle = a.getInt(attr, CHAIN_SPREAD);
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_VERTICAL_CHAINSTYLE:
            //                {
            //                    verticalChainStyle = a.getInt(attr, CHAIN_SPREAD);
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINED_WIDTH:
            //                {
            //                    constrainedWidth = a.getBoolean(attr, constrainedWidth);
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINED_HEIGHT:
            //                {
            //                    constrainedHeight = a.getBoolean(attr, constrainedHeight);
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_WIDTH_DEFAULT:
            //                {
            //                    matchConstraintDefaultWidth = a.getInt(attr, MATCH_CONSTRAINT_SPREAD);
            //                    if (matchConstraintDefaultWidth == MATCH_CONSTRAINT_WRAP)
            //                    {
            //                        Log.e(TAG, "layout_constraintWidth_default=\"wrap\" is deprecated." + "\nUse layout_width=\"WRAP_CONTENT\" and layout_constrainedWidth=\"true\" instead.");
            //                    }
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_HEIGHT_DEFAULT:
            //                {
            //                    matchConstraintDefaultHeight = a.getInt(attr, MATCH_CONSTRAINT_SPREAD);
            //                    if (matchConstraintDefaultHeight == MATCH_CONSTRAINT_WRAP)
            //                    {
            //                        Log.e(TAG, "layout_constraintHeight_default=\"wrap\" is deprecated." + "\nUse layout_height=\"WRAP_CONTENT\" and layout_constrainedHeight=\"true\" instead.");
            //                    }
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_WIDTH_MIN:
            //                {
            //                    try
            //                    {
            //                        matchConstraintMinWidth = a.getDimensionPixelSize(attr, matchConstraintMinWidth);
            //                    }
            //                    catch (Exception)
            //                    {
            //                        int value = a.getInt(attr, matchConstraintMinWidth);
            //                        if (value == WRAP_CONTENT)
            //                        {
            //                            matchConstraintMinWidth = WRAP_CONTENT;
            //                        }
            //                    }
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_WIDTH_MAX:
            //                {
            //                    try
            //                    {
            //                        matchConstraintMaxWidth = a.getDimensionPixelSize(attr, matchConstraintMaxWidth);
            //                    }
            //                    catch (Exception)
            //                    {
            //                        int value = a.getInt(attr, matchConstraintMaxWidth);
            //                        if (value == WRAP_CONTENT)
            //                        {
            //                            matchConstraintMaxWidth = WRAP_CONTENT;
            //                        }
            //                    }
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_WIDTH_PERCENT:
            //                {
            //                    matchConstraintPercentWidth = Math.Max(0, a.getFloat(attr, matchConstraintPercentWidth));
            //                    matchConstraintDefaultWidth = MATCH_CONSTRAINT_PERCENT;
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_HEIGHT_MIN:
            //                {
            //                    try
            //                    {
            //                        matchConstraintMinHeight = a.getDimensionPixelSize(attr, matchConstraintMinHeight);
            //                    }
            //                    catch (Exception)
            //                    {
            //                        int value = a.getInt(attr, matchConstraintMinHeight);
            //                        if (value == WRAP_CONTENT)
            //                        {
            //                            matchConstraintMinHeight = WRAP_CONTENT;
            //                        }
            //                    }
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_HEIGHT_MAX:
            //                {
            //                    try
            //                    {
            //                        matchConstraintMaxHeight = a.getDimensionPixelSize(attr, matchConstraintMaxHeight);
            //                    }
            //                    catch (Exception)
            //                    {
            //                        int value = a.getInt(attr, matchConstraintMaxHeight);
            //                        if (value == WRAP_CONTENT)
            //                        {
            //                            matchConstraintMaxHeight = WRAP_CONTENT;
            //                        }
            //                    }
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_HEIGHT_PERCENT:
            //                {
            //                    matchConstraintPercentHeight = Math.Max(0, a.getFloat(attr, matchConstraintPercentHeight));
            //                    matchConstraintDefaultHeight = MATCH_CONSTRAINT_PERCENT;
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_TAG:
            //                constraintTag = a.getString(attr);
            //                break;
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_LEFT_CREATOR:
            //                {
            //                    // Skip
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_TOP_CREATOR:
            //                {
            //                    // Skip
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_RIGHT_CREATOR:
            //                {
            //                    // Skip
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_BOTTOM_CREATOR:
            //                {
            //                    // Skip
            //                    break;
            //                }
            //            case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_BASELINE_CREATOR:
            //                {
            //                    // Skip
            //                    break;
            //                }
            //        }
            //    }

            //    ///////////////////////////////////////////////////////////////////////////////////////////
            //    // Layout margins handling TODO: re-activate in 3.0
            //    ///////////////////////////////////////////////////////////////////////////////////////////
            //    /*
            //    if (Build.VERSION.SDK_INT < Build.VERSION_CODES.JELLY_BEAN_MR1) {
            //        leftMargin = originalLeftMargin;
            //        rightMargin = originalRightMargin;
            //    }
            //    */
            //    ///////////////////////////////////////////////////////////////////////////////////////////

            //    a.recycle();
            //    validate();
            //}


            public virtual void validate()
            {
                isGuideline = false;
                horizontalDimensionFixed = true;
                verticalDimensionFixed = true;
                ///////////////////////////////////////////////////////////////////////////////////////////
                // Layout margins handling TODO: re-activate in 3.0
                ///////////////////////////////////////////////////////////////////////////////////////////
                /*
                if (dimensionRatio != null && !widthSet && !heightSet) {
                    width = MATCH_CONSTRAINT;
                    height = MATCH_CONSTRAINT;
                }
                */
                ///////////////////////////////////////////////////////////////////////////////////////////

                if (width == WRAP_CONTENT && constrainedWidth)
                {
                    horizontalDimensionFixed = false;
                    if (matchConstraintDefaultWidth == MATCH_CONSTRAINT_SPREAD)
                    {
                        matchConstraintDefaultWidth = MATCH_CONSTRAINT_WRAP;
                    }
                }
                if (height == WRAP_CONTENT && constrainedHeight)
                {
                    verticalDimensionFixed = false;
                    if (matchConstraintDefaultHeight == MATCH_CONSTRAINT_SPREAD)
                    {
                        matchConstraintDefaultHeight = MATCH_CONSTRAINT_WRAP;
                    }
                }
                if (width == MATCH_CONSTRAINT || width == MATCH_PARENT)
                {
                    horizontalDimensionFixed = false;
                    // We have to reset LayoutParams width/height to WRAP_CONTENT here, as some widgets like TextView
                    // will use the layout params directly as a hint to know if they need to request a layout
                    // when their content change (e.g. during setTextView)
                    if (width == MATCH_CONSTRAINT && matchConstraintDefaultWidth == MATCH_CONSTRAINT_WRAP)
                    {
                        width = WRAP_CONTENT;
                        constrainedWidth = true;
                    }
                }
                if (height == MATCH_CONSTRAINT || height == MATCH_PARENT)
                {
                    verticalDimensionFixed = false;
                    // We have to reset LayoutParams width/height to WRAP_CONTENT here, as some widgets like TextView
                    // will use the layout params directly as a hint to know if they need to request a layout
                    // when their content change (e.g. during setTextView)
                    if (height == MATCH_CONSTRAINT && matchConstraintDefaultHeight == MATCH_CONSTRAINT_WRAP)
                    {
                        height = WRAP_CONTENT;
                        constrainedHeight = true;
                    }
                }
                if (guidePercent != UNSET || guideBegin != UNSET || guideEnd != UNSET)
                {
                    isGuideline = true;
                    horizontalDimensionFixed = true;
                    verticalDimensionFixed = true;
                    if (!(widget is Guideline))
                    {
                        widget = new Guideline();
                    }
                    ((Guideline)widget).Orientation = orientation;
                }
            }

            public LayoutParams(int width, int height) : base(width, height)
            {
            }

            public LayoutParams(ViewGroup.LayoutParams source) : base(source)
            {
            }

            /// <summary>
            /// {@inheritDoc}
            /// </summary>
            //JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
            //ORIGINAL LINE: @Override @TargetApi(android.os.Build.VERSION_CODES.JELLY_BEAN_MR1) public void resolveLayoutDirection(int layoutDirection)
            public override void resolveLayoutDirection(int layoutDirection)
            {
                ///////////////////////////////////////////////////////////////////////////////////////////
                // Layout margins handling TODO: re-activate in 3.0
                ///////////////////////////////////////////////////////////////////////////////////////////
                /*
                if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1) {
                    this.layoutDirection = layoutDirection;
                    isRtl = (View.LAYOUT_DIRECTION_RTL == layoutDirection);
                }
    
                // First apply margins.
                leftMargin = originalLeftMargin;
                rightMargin = originalRightMargin;
    
                if (isRtl) {
                    leftMargin = originalRightMargin;
                    rightMargin = originalLeftMargin;
                    if (startMargin != UNSET) {
                        rightMargin = startMargin;
                    }
                    if (endMargin != UNSET) {
                        leftMargin = endMargin;
                    }
                } else {
                    if (startMargin != UNSET) {
                        leftMargin = startMargin;
                    }
                    if (endMargin != UNSET) {
                        rightMargin = endMargin;
                    }
                }
                */
                ///////////////////////////////////////////////////////////////////////////////////////////
                int originalLeftMargin = leftMargin;
                int originalRightMargin = rightMargin;

                bool isRtl = false;
                //if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1)
                //{
                    base.resolveLayoutDirection(layoutDirection);
                    isRtl = (AndroidView.LAYOUT_DIRECTION_RTL == LayoutDirection);
                //}
                ///////////////////////////////////////////////////////////////////////////////////////////

                resolvedRightToLeft = UNSET;
                resolvedRightToRight = UNSET;
                resolvedLeftToLeft = UNSET;
                resolvedLeftToRight = UNSET;

                resolveGoneLeftMargin = UNSET;
                resolveGoneRightMargin = UNSET;
                resolveGoneLeftMargin = goneLeftMargin;
                resolveGoneRightMargin = goneRightMargin;
                resolvedHorizontalBias = horizontalBias;

                resolvedGuideBegin = guideBegin;
                resolvedGuideEnd = guideEnd;
                resolvedGuidePercent = guidePercent;

                // Post JB MR1, if start/end are defined, they take precedence over left/right
                if (isRtl)
                {
                    bool startEndDefined = false;
                    if (startToEnd != UNSET)
                    {
                        resolvedRightToLeft = startToEnd;
                        startEndDefined = true;
                    }
                    else if (startToStart != UNSET)
                    {
                        resolvedRightToRight = startToStart;
                        startEndDefined = true;
                    }
                    if (endToStart != UNSET)
                    {
                        resolvedLeftToRight = endToStart;
                        startEndDefined = true;
                    }
                    if (endToEnd != UNSET)
                    {
                        resolvedLeftToLeft = endToEnd;
                        startEndDefined = true;
                    }
                    if (goneStartMargin != GONE_UNSET)
                    {
                        resolveGoneRightMargin = goneStartMargin;
                    }
                    if (goneEndMargin != GONE_UNSET)
                    {
                        resolveGoneLeftMargin = goneEndMargin;
                    }
                    if (startEndDefined)
                    {
                        resolvedHorizontalBias = 1 - horizontalBias;
                    }

                    // Only apply to vertical guidelines
                    if (isGuideline && orientation == Guideline.VERTICAL)
                    {
                        if (guidePercent != UNSET)
                        {
                            resolvedGuidePercent = 1 - guidePercent;
                            resolvedGuideBegin = UNSET;
                            resolvedGuideEnd = UNSET;
                        }
                        else if (guideBegin != UNSET)
                        {
                            resolvedGuideEnd = guideBegin;
                            resolvedGuideBegin = UNSET;
                            resolvedGuidePercent = UNSET;
                        }
                        else if (guideEnd != UNSET)
                        {
                            resolvedGuideBegin = guideEnd;
                            resolvedGuideEnd = UNSET;
                            resolvedGuidePercent = UNSET;
                        }
                    }
                }
                else
                {
                    if (startToEnd != UNSET)
                    {
                        resolvedLeftToRight = startToEnd;
                    }
                    if (startToStart != UNSET)
                    {
                        resolvedLeftToLeft = startToStart;
                    }
                    if (endToStart != UNSET)
                    {
                        resolvedRightToLeft = endToStart;
                    }
                    if (endToEnd != UNSET)
                    {
                        resolvedRightToRight = endToEnd;
                    }
                    if (goneStartMargin != GONE_UNSET)
                    {
                        resolveGoneLeftMargin = goneStartMargin;
                    }
                    if (goneEndMargin != GONE_UNSET)
                    {
                        resolveGoneRightMargin = goneEndMargin;
                    }
                }
                // if no constraint is defined via RTL attributes, use left/right if present
                if (endToStart == UNSET && endToEnd == UNSET && startToStart == UNSET && startToEnd == UNSET)
                {
                    if (rightToLeft != UNSET)
                    {
                        resolvedRightToLeft = rightToLeft;
                        if (rightMargin <= 0 && originalRightMargin > 0)
                        {
                            rightMargin = originalRightMargin;
                        }
                    }
                    else if (rightToRight != UNSET)
                    {
                        resolvedRightToRight = rightToRight;
                        if (rightMargin <= 0 && originalRightMargin > 0)
                        {
                            rightMargin = originalRightMargin;
                        }
                    }
                    if (leftToLeft != UNSET)
                    {
                        resolvedLeftToLeft = leftToLeft;
                        if (leftMargin <= 0 && originalLeftMargin > 0)
                        {
                            leftMargin = originalLeftMargin;
                        }
                    }
                    else if (leftToRight != UNSET)
                    {
                        resolvedLeftToRight = leftToRight;
                        if (leftMargin <= 0 && originalLeftMargin > 0)
                        {
                            leftMargin = originalLeftMargin;
                        }
                    }
                }
            }

            /// <summary>
            /// Tag that can be used to identify a view as being a member of a group.
            /// Which can be used for Helpers or in MotionLayout
            /// </summary>
            /// <returns> tag string or null if not defined </returns>
            public virtual string ConstraintTag
            {
                get
                {
                    return constraintTag;
                }
            }
        }
    }
}

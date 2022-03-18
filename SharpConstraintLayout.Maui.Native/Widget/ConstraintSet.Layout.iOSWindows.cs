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
using androidx.constraintlayout.core.widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SharpConstraintLayout.Maui.Widget
{
    public partial class ConstraintSet
    {
        public class Layout
        {
            public bool mIsGuideline = false;
            public bool mApply = false;
            public bool mOverride = false;
            public int mWidth;
            public int mHeight;
            public const int UNSET = ConstraintSet.Unset;
            public static readonly int UNSET_GONE_MARGIN = int.MinValue;
            
            
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
            public int leftToRight = UNSET;
            public int rightToLeft = UNSET;
            public int rightToRight = UNSET;
            public int topToTop = UNSET;
            public int topToBottom = UNSET;
            public int bottomToTop = UNSET;
            public int bottomToBottom = UNSET;
            public int baselineToBaseline = UNSET;
            public int baselineToTop = UNSET;
            public int baselineToBottom = UNSET;
            public int startToEnd = UNSET;
            /// <summary>
            /// Constrains the start side of a child to the start side of a target child (contains the target child id).
            /// </summary>
            public int startToStart = UNSET;
            public int endToStart = UNSET;
            public int endToEnd = UNSET;
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
            public int circleConstraint = UNSET;
            public int circleRadius = 0;
            public float circleAngle = 0;
            public int editorAbsoluteX = UNSET;
            public int editorAbsoluteY = UNSET;
            public int orientation = UNSET;
            public int leftMargin = 0;
            public int rightMargin = 0;
            public int topMargin = 0;
            public int bottomMargin = 0;
            public int endMargin = 0;
            public int startMargin = 0;
            public int baselineMargin = 0;
            /// <summary>
            /// The left margin to use when the target is gone.
            /// </summary>
            public int goneLeftMargin = UNSET_GONE_MARGIN;
            public int goneTopMargin = UNSET_GONE_MARGIN;
            public int goneRightMargin = UNSET_GONE_MARGIN;
            public int goneBottomMargin = UNSET_GONE_MARGIN;
            public int goneEndMargin = UNSET_GONE_MARGIN;
            public int goneStartMargin = UNSET_GONE_MARGIN;
            public int goneBaselineMargin = UNSET_GONE_MARGIN;
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
            /// <li><seealso cref="CHAIN_SPREAD"/> -- the elements will be spread out</li>
            /// <li><seealso cref="CHAIN_SPREAD_INSIDE"/> -- similar, but the endpoints of the chain will not
            /// be spread out</li>
            /// <li><seealso cref="CHAIN_PACKED"/> -- the elements of the chain will be packed together. The
            /// horizontal bias attribute of the child will then affect the positioning of the packed
            /// elements</li>
            /// </ul>
            /// </summary>
            public int horizontalChainStyle = CHAIN_SPREAD;
            /// <summary>
            /// If the child is the start of a vertical chain, this attribute will drive how
            /// the elements of the chain will be positioned. The possible values are:
            /// <ul>
            /// <li><seealso cref="CHAIN_SPREAD"/> -- the elements will be spread out</li>
            /// <li><seealso cref="CHAIN_SPREAD_INSIDE"/> -- similar, but the endpoints of the chain will not
            /// be spread out</li>
            /// <li><seealso cref="CHAIN_PACKED"/> -- the elements of the chain will be packed together. The
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
            public int matchConstraintDefaultWidth = ConstraintWidget.MATCH_CONSTRAINT_SPREAD;
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
            public int matchConstraintDefaultHeight = ConstraintWidget.MATCH_CONSTRAINT_SPREAD;
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
            /// Specify the percentage when using the match constraint percent mode. From 0 to 1.
            /// </summary>
            public float matchConstraintPercentWidth = 1;

            /// <summary>
            /// Specify the percentage when using the match constraint percent mode. From 0 to 1.
            /// </summary>
            public float matchConstraintPercentHeight = 1;
            public int mBarrierDirection = UNSET;
            public int mBarrierMargin = 0;
            public int mHelperType = UNSET;
            public int[] mReferenceIds;
            public string mReferenceIdString;
            public string constraintTag;
            public bool constrainedWidth = false;
            public bool constrainedHeight = false;
            // TODO public boolean mChainUseRtl = false;
            public bool mBarrierAllowsGoneWidgets = true;
            /// <summary>
            /// Specify how this view is taken in account during the parent's wrap computation
            /// 
            /// Can be either of:
            /// WRAP_BEHAVIOR_INCLUDED the widget is taken in account for the wrap (default)
            /// WRAP_BEHAVIOR_HORIZONTAL_ONLY the widget will be included in the wrap only horizontally
            /// WRAP_BEHAVIOR_VERTICAL_ONLY the widget will be included in the wrap only vertically
            /// WRAP_BEHAVIOR_SKIPPED the widget is not part of the wrap computation
            /// </summary>
            public int wrapBehaviorInParent = ConstraintWidget.WRAP_BEHAVIOR_INCLUDED;

            public virtual void copyFrom(Layout src)
            {
                mIsGuideline = src.mIsGuideline;
                mWidth = src.mWidth;
                mApply = src.mApply;
                mHeight = src.mHeight;
                guideBegin = src.guideBegin;
                guideEnd = src.guideEnd;
                guidePercent = src.guidePercent;
                leftToLeft = src.leftToLeft;
                leftToRight = src.leftToRight;
                rightToLeft = src.rightToLeft;
                rightToRight = src.rightToRight;
                topToTop = src.topToTop;
                topToBottom = src.topToBottom;
                bottomToTop = src.bottomToTop;
                bottomToBottom = src.bottomToBottom;
                baselineToBaseline = src.baselineToBaseline;
                baselineToTop = src.baselineToTop;
                baselineToBottom = src.baselineToBottom;
                startToEnd = src.startToEnd;
                startToStart = src.startToStart;
                endToStart = src.endToStart;
                endToEnd = src.endToEnd;
                horizontalBias = src.horizontalBias;
                verticalBias = src.verticalBias;
                dimensionRatio = src.dimensionRatio;
                circleConstraint = src.circleConstraint;
                circleRadius = src.circleRadius;
                circleAngle = src.circleAngle;
                editorAbsoluteX = src.editorAbsoluteX;
                editorAbsoluteY = src.editorAbsoluteY;
                orientation = src.orientation;
                leftMargin = src.leftMargin;
                rightMargin = src.rightMargin;
                topMargin = src.topMargin;
                bottomMargin = src.bottomMargin;
                endMargin = src.endMargin;
                startMargin = src.startMargin;
                baselineMargin = src.baselineMargin;
                goneLeftMargin = src.goneLeftMargin;
                goneTopMargin = src.goneTopMargin;
                goneRightMargin = src.goneRightMargin;
                goneBottomMargin = src.goneBottomMargin;
                goneEndMargin = src.goneEndMargin;
                goneStartMargin = src.goneStartMargin;
                goneBaselineMargin = src.goneBaselineMargin;
                verticalWeight = src.verticalWeight;
                horizontalWeight = src.horizontalWeight;
                horizontalChainStyle = src.horizontalChainStyle;
                verticalChainStyle = src.verticalChainStyle;
                matchConstraintDefaultWidth = src.matchConstraintDefaultWidth;
                matchConstraintDefaultHeight = src.matchConstraintDefaultHeight;
                matchConstraintMaxWidth = src.matchConstraintMaxWidth;
                matchConstraintMaxHeight = src.matchConstraintMaxHeight;
                matchConstraintMinWidth = src.matchConstraintMinWidth;
                matchConstraintMinHeight = src.matchConstraintMinHeight;
                matchConstraintPercentWidth = src.matchConstraintPercentWidth;
                matchConstraintPercentHeight = src.matchConstraintPercentHeight;
                mBarrierDirection = src.mBarrierDirection;
                mBarrierMargin = src.mBarrierMargin;
                mHelperType = src.mHelperType;
                constraintTag = src.constraintTag;

                if (src.mReferenceIds != null && string.ReferenceEquals(src.mReferenceIdString, null))
                {
                    //mReferenceIds = Arrays.copyOf(src.mReferenceIds, src.mReferenceIds.Length);
                    mReferenceIds = ArrayHelperClass.Copy<int>(src.mReferenceIds, src.mReferenceIds.Length);
                }
                else
                {
                    mReferenceIds = null;
                }
                mReferenceIdString = src.mReferenceIdString;
                constrainedWidth = src.constrainedWidth;
                constrainedHeight = src.constrainedHeight;
                // TODO mChainUseRtl = t.mChainUseRtl;
                mBarrierAllowsGoneWidgets = src.mBarrierAllowsGoneWidgets;
                wrapBehaviorInParent = src.wrapBehaviorInParent;
            }

            internal static Dictionary<int, int> mapToConstant = new Dictionary<int, int>();
            internal const int BASELINE_TO_BASELINE = 1;
            internal const int BOTTOM_MARGIN = 2;
            internal const int BOTTOM_TO_BOTTOM = 3;
            internal const int BOTTOM_TO_TOP = 4;
            internal const int DIMENSION_RATIO = 5;
            internal const int EDITOR_ABSOLUTE_X = 6;
            internal const int EDITOR_ABSOLUTE_Y = 7;
            internal const int END_MARGIN = 8;
            internal const int END_TO_END = 9;
            internal const int END_TO_START = 10;
            internal const int GONE_BOTTOM_MARGIN = 11;
            internal const int GONE_END_MARGIN = 12;
            internal const int GONE_LEFT_MARGIN = 13;
            internal const int GONE_RIGHT_MARGIN = 14;
            internal const int GONE_START_MARGIN = 15;
            internal const int GONE_TOP_MARGIN = 16;
            internal const int GUIDE_BEGIN = 17;
            internal const int GUIDE_END = 18;
            internal const int GUIDE_PERCENT = 19;
            internal const int HORIZONTAL_BIAS = 20;
            internal const int LAYOUT_HEIGHT = 21;
            internal const int LAYOUT_WIDTH = 22;
            internal const int LEFT_MARGIN = 23;
            internal const int LEFT_TO_LEFT = 24;
            internal const int LEFT_TO_RIGHT = 25;
            internal const int ORIENTATION = 26;
            internal const int RIGHT_MARGIN = 27;
            internal const int RIGHT_TO_LEFT = 28;
            internal const int RIGHT_TO_RIGHT = 29;
            internal const int START_MARGIN = 30;
            internal const int START_TO_END = 31;
            internal const int START_TO_START = 32;
            internal const int TOP_MARGIN = 33;
            internal const int TOP_TO_BOTTOM = 34;
            internal const int TOP_TO_TOP = 35;
            internal const int VERTICAL_BIAS = 36;
            internal const int HORIZONTAL_WEIGHT = 37;
            internal const int VERTICAL_WEIGHT = 38;
            internal const int HORIZONTAL_STYLE = 39;
            internal const int VERTICAL_STYLE = 40;
            internal const int LAYOUT_CONSTRAINT_WIDTH = 41;
            internal const int LAYOUT_CONSTRAINT_HEIGHT = 42;

            internal const int CIRCLE = 61;
            internal const int CIRCLE_RADIUS = 62;
            internal const int CIRCLE_ANGLE = 63;
            internal const int WIDTH_PERCENT = 69;
            internal const int HEIGHT_PERCENT = 70;
            internal const int CHAIN_USE_RTL = 71;
            internal const int BARRIER_DIRECTION = 72;
            internal const int BARRIER_MARGIN = 73;
            internal const int CONSTRAINT_REFERENCED_IDS = 74;
            internal const int BARRIER_ALLOWS_GONE_WIDGETS = 75;
            internal const int UNUSED = 76;

            /*public virtual void dump(MotionScene scene, StringBuilder stringBuilder)
            {
                Field[] fields = this.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                stringBuilder.Append("\n");
                for (int i = 0; i < fields.Length; i++)
                {
                    Field field = fields[i];
                    string name = field.Name;
                    if (Modifier.isStatic(field.Modifiers))
                    {
                        continue;
                    }
                    //                 if (!field.isAccessible()) {
                    //                    continue;
                    //                }       if (!field.isAccessible()) {
                    //                    continue;
                    //                }

                    try
                    {
                        object value = field.get(this);
                        Type type = field.Type;
                        if (type == Integer.TYPE)
                        {
                            int? iValue = (int?)value;
                            if (iValue.Value != UNSET)
                            {
                                string stringId = scene.lookUpConstraintName(iValue.Value);
                                stringBuilder.Append("    ");
                                stringBuilder.Append(name);
                                stringBuilder.Append(" = \"");
                                stringBuilder.Append((string.ReferenceEquals(stringId, null)) ? iValue : stringId);
                                stringBuilder.Append("\"\n");
                            }
                        }
                        else if (type == Float.TYPE)
                        {
                            float? fValue = (float?)value;
                            if (fValue != UNSET)
                            {
                                stringBuilder.Append("    ");
                                stringBuilder.Append(name);
                                stringBuilder.Append(" = \"");
                                stringBuilder.Append(fValue);
                                stringBuilder.Append("\"\n");

                            }
                        }

                    }
                    catch (IllegalAccessException e)
                    {
                        Console.WriteLine(e.ToString());
                        Console.Write(e.StackTrace);
                    }


                }
            }*/
            #region Add From LayoutPamas

            // Internal use only
            public bool horizontalDimensionFixed = true;
            public bool verticalDimensionFixed = true;

            public bool needsBaseline = false;
            //public bool isGuideline = false;//重复mIsGuideline
            public bool isHelper = false;
            public bool isInPlaceholder = false;
            public bool isVirtualGroup = false;

            public int resolvedLeftToLeft = UNSET;
            public int resolvedLeftToRight = UNSET;
            public int resolvedRightToLeft = UNSET;
            public int resolvedRightToRight = UNSET;
            //public int resolveGoneLeftMargin = GONE_UNSET;
            public int resolveGoneLeftMargin = int.MaxValue;
            //public int resolveGoneRightMargin = GONE_UNSET;
            public int resolveGoneRightMargin = int.MaxValue;
            public float resolvedHorizontalBias = 0.5f;

            public bool helped = false;

            internal bool widthSet = true; // need to be set to false when we reactivate this in 3.0
            internal bool heightSet = true; // need to be set to false when we reactivate this in 3.0

            /// <summary>
            /// Add From layoutParams.
            /// 验证?看起来时可以处理WrapContent
            /// </summary>
            public void validate()
            {
                mIsGuideline = false;
                horizontalDimensionFixed = true;
                verticalDimensionFixed = true;
                ///////////////////////////////////////////////////////////////////////////////////////////
                // Layout margins handling TODO: re-activate in 3.0
                ///////////////////////////////////////////////////////////////////////////////////////////

                /*if (dimensionRatio != null && !widthSet && !heightSet)
                {
                    mWidth = MATCH_CONSTRAINT;
                    mHeight = MATCH_CONSTRAINT;
                }*/

                ///////////////////////////////////////////////////////////////////////////////////////////

                if (mWidth == WRAP_CONTENT && constrainedWidth)
                {
                    horizontalDimensionFixed = false;
                    if (matchConstraintDefaultWidth == MATCH_CONSTRAINT_SPREAD)
                    {
                        matchConstraintDefaultWidth = MATCH_CONSTRAINT_WRAP;
                    }
                }
                if (mHeight == WRAP_CONTENT && constrainedHeight)
                {
                    verticalDimensionFixed = false;
                    if (matchConstraintDefaultHeight == MATCH_CONSTRAINT_SPREAD)
                    {
                        matchConstraintDefaultHeight = MATCH_CONSTRAINT_WRAP;
                    }
                }
                if (mWidth == MATCH_CONSTRAINT || mWidth == MATCH_PARENT)
                {
                    horizontalDimensionFixed = false;
                    // We have to reset LayoutParams width/height to WRAP_CONTENT here, as some widgets like TextView
                    // will use the layout params directly as a hint to know if they need to request a layout
                    // when their content change (e.g. during setTextView)
                    if (mWidth == MATCH_CONSTRAINT && matchConstraintDefaultWidth == MATCH_CONSTRAINT_WRAP)
                    {
                        mWidth = WRAP_CONTENT;
                        constrainedWidth = true;
                    }
                }
                if (mHeight == MATCH_CONSTRAINT || mHeight == MATCH_PARENT)
                {
                    verticalDimensionFixed = false;
                    // We have to reset LayoutParams width/height to WRAP_CONTENT here, as some widgets like TextView
                    // will use the layout params directly as a hint to know if they need to request a layout
                    // when their content change (e.g. during setTextView)
                    if (mHeight == MATCH_CONSTRAINT && matchConstraintDefaultHeight == MATCH_CONSTRAINT_WRAP)
                    {
                        mHeight = WRAP_CONTENT;
                        constrainedHeight = true;
                    }
                }

                if (guidePercent != UNSET || guideBegin != UNSET || guideEnd != UNSET)
                {
                    mIsGuideline = true;
                    horizontalDimensionFixed = true;
                    verticalDimensionFixed = true;

                    //这里的给Guideline指定方向的逻辑放到ApplyTo中去,避免传递Widget
                    /*if (!(widget is androidx.constraintlayout.core.widgets.Guideline))//必须是Guideline,新建的话需要更新一些逻辑,TODO:
                    {
                        //widget = new Guideline();
                    }
                    else
                    {
                        var guideline = (androidx.constraintlayout.core.widgets.Guideline)widget;
                        if(guideline != null && orientation != UNSET)//如果有新的orientation
                        {
                            guideline.Orientation = orientation;
                        }
                    }*/
                }
            }
#endregion
        }
    }
}

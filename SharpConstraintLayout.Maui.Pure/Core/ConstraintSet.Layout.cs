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
namespace SharpConstraintLayout.Maui.Pure.Core
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
            public const int UNSET = ConstraintSet.UNSET;
            public static readonly int UNSET_GONE_MARGIN = int.MinValue;
            public int guideBegin = UNSET;
            public int guideEnd = UNSET;
            public float guidePercent = UNSET;
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
            public int startToStart = UNSET;
            public int endToStart = UNSET;
            public int endToEnd = UNSET;
            public float horizontalBias = 0.5f;
            public float verticalBias = 0.5f;
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
            public int goneLeftMargin = UNSET_GONE_MARGIN;
            public int goneTopMargin = UNSET_GONE_MARGIN;
            public int goneRightMargin = UNSET_GONE_MARGIN;
            public int goneBottomMargin = UNSET_GONE_MARGIN;
            public int goneEndMargin = UNSET_GONE_MARGIN;
            public int goneStartMargin = UNSET_GONE_MARGIN;
            public int goneBaselineMargin = UNSET_GONE_MARGIN;
            public float verticalWeight = UNSET;
            public float horizontalWeight = UNSET;
            public int horizontalChainStyle = CHAIN_SPREAD;
            public int verticalChainStyle = CHAIN_SPREAD;
            /// <summary>
            /// <see cref="ConstraintLayout.LayoutParams.matchConstraintDefaultWidth"/>
            /// </summary>
            public int matchConstraintDefaultWidth = ConstraintWidget.MATCH_CONSTRAINT_SPREAD;
            /// <summary>
            /// <see cref="ConstraintLayout.LayoutParams.matchConstraintDefaultHeight"/>
            /// </summary>
            public int matchConstraintDefaultHeight = ConstraintWidget.MATCH_CONSTRAINT_SPREAD;
            /// <summary>
            /// <see cref="ConstraintLayout.LayoutParams.matchConstraintMaxWidth"/>
            /// </summary>
            public int matchConstraintMaxWidth = UNSET;
            /// <summary>
            /// <see cref="ConstraintLayout.LayoutParams.matchConstraintMaxHeight"/>
            /// </summary>
            public int matchConstraintMaxHeight = UNSET;
            /// <summary>
            /// <see cref="ConstraintLayout.LayoutParams.matchConstraintMinWidth/>
            /// </summary>
            public int matchConstraintMinWidth = UNSET;
            /// <summary>
            /// <see cref="ConstraintLayout.LayoutParams.matchConstraintMinHeight"/>
            /// </summary>
            public int matchConstraintMinHeight = UNSET;
            /// <summary>
            /// <see cref="ConstraintLayout.LayoutParams.matchConstraintPercentWidth"/>
            /// </summary>
            public float matchConstraintPercentWidth = 1;
            /// <summary>
            /// <see cref="ConstraintLayout.LayoutParams.matchConstraintPercentHeight"/>
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
            /// <see cref="ConstraintLayout.LayoutParams.wrapBehaviorInParent">
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

            internal static Dictionary<int,int> mapToConstant = new Dictionary<int, int>();
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


            /*internal virtual void fillFromAttributeList(Context context, AttributeSet attrs)
            {
                TypedArray a = context.obtainStyledAttributes(attrs, R.styleable.Layout);
                mApply = true;
                //JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final int N = a.getIndexCount();
                int N = a.IndexCount;
                for (int i = 0; i < N; i++)
                {
                    int attr = a.getIndex(i);

                    switch (mapToConstant.get(attr))
                    {
                        case LEFT_TO_LEFT:
                            leftToLeft = lookupID(a, attr, leftToLeft);
                            break;
                        case LEFT_TO_RIGHT:
                            leftToRight = lookupID(a, attr, leftToRight);
                            break;
                        case RIGHT_TO_LEFT:
                            rightToLeft = lookupID(a, attr, rightToLeft);
                            break;
                        case RIGHT_TO_RIGHT:
                            rightToRight = lookupID(a, attr, rightToRight);
                            break;
                        case TOP_TO_TOP:
                            topToTop = lookupID(a, attr, topToTop);
                            break;
                        case TOP_TO_BOTTOM:
                            topToBottom = lookupID(a, attr, topToBottom);
                            break;
                        case BOTTOM_TO_TOP:
                            bottomToTop = lookupID(a, attr, bottomToTop);
                            break;
                        case BOTTOM_TO_BOTTOM:
                            bottomToBottom = lookupID(a, attr, bottomToBottom);
                            break;
                        case BASELINE_TO_BASELINE:
                            baselineToBaseline = lookupID(a, attr, baselineToBaseline);
                            break;
                        case BASELINE_TO_TOP:
                            baselineToTop = lookupID(a, attr, baselineToTop);
                            break;
                        case BASELINE_TO_BOTTOM:
                            baselineToBottom = lookupID(a, attr, baselineToBottom);
                            break;
                        case EDITOR_ABSOLUTE_X:
                            editorAbsoluteX = a.getDimensionPixelOffset(attr, editorAbsoluteX);
                            break;
                        case EDITOR_ABSOLUTE_Y:
                            editorAbsoluteY = a.getDimensionPixelOffset(attr, editorAbsoluteY);
                            break;
                        case GUIDE_BEGIN:
                            guideBegin = a.getDimensionPixelOffset(attr, guideBegin);
                            break;
                        case GUIDE_END:
                            guideEnd = a.getDimensionPixelOffset(attr, guideEnd);
                            break;
                        case GUIDE_PERCENT:
                            guidePercent = a.getFloat(attr, guidePercent);
                            break;
                        case ORIENTATION:
                            orientation = a.getInt(attr, orientation);
                            break;
                        case START_TO_END:
                            startToEnd = lookupID(a, attr, startToEnd);
                            break;
                        case START_TO_START:
                            startToStart = lookupID(a, attr, startToStart);
                            break;
                        case END_TO_START:
                            endToStart = lookupID(a, attr, endToStart);
                            break;
                        case END_TO_END:
                            endToEnd = lookupID(a, attr, endToEnd);
                            break;
                        case CIRCLE:
                            circleConstraint = lookupID(a, attr, circleConstraint);
                            break;
                        case CIRCLE_RADIUS:
                            circleRadius = a.getDimensionPixelSize(attr, circleRadius);
                            break;
                        case CIRCLE_ANGLE:
                            circleAngle = a.getFloat(attr, circleAngle);
                            break;
                        case GONE_LEFT_MARGIN:
                            goneLeftMargin = a.getDimensionPixelSize(attr, goneLeftMargin);
                            break;
                        case GONE_TOP_MARGIN:
                            goneTopMargin = a.getDimensionPixelSize(attr, goneTopMargin);
                            break;
                        case GONE_RIGHT_MARGIN:
                            goneRightMargin = a.getDimensionPixelSize(attr, goneRightMargin);
                            break;
                        case GONE_BOTTOM_MARGIN:
                            goneBottomMargin = a.getDimensionPixelSize(attr, goneBottomMargin);
                            break;
                        case GONE_START_MARGIN:
                            goneStartMargin = a.getDimensionPixelSize(attr, goneStartMargin);
                            break;
                        case GONE_END_MARGIN:
                            goneEndMargin = a.getDimensionPixelSize(attr, goneEndMargin);
                            break;
                        case GONE_BASELINE_MARGIN:
                            goneBaselineMargin = a.getDimensionPixelSize(attr, goneBaselineMargin);
                            break;
                        case HORIZONTAL_BIAS:
                            horizontalBias = a.getFloat(attr, horizontalBias);
                            break;
                        case VERTICAL_BIAS:
                            verticalBias = a.getFloat(attr, verticalBias);
                            break;
                        case LEFT_MARGIN:
                            leftMargin = a.getDimensionPixelSize(attr, leftMargin);
                            break;
                        case RIGHT_MARGIN:
                            rightMargin = a.getDimensionPixelSize(attr, rightMargin);
                            break;
                        case START_MARGIN:
                            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1)
                            {
                                startMargin = a.getDimensionPixelSize(attr, startMargin);
                            }
                            break;
                        case END_MARGIN:
                            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1)
                            {
                                endMargin = a.getDimensionPixelSize(attr, endMargin);
                            }
                            break;
                        case TOP_MARGIN:
                            topMargin = a.getDimensionPixelSize(attr, topMargin);
                            break;
                        case BOTTOM_MARGIN:
                            bottomMargin = a.getDimensionPixelSize(attr, bottomMargin);
                            break;
                        case BASELINE_MARGIN:
                            baselineMargin = a.getDimensionPixelSize(attr, baselineMargin);
                            break;
                        case LAYOUT_WIDTH:
                            mWidth = a.getLayoutDimension(attr, mWidth);
                            break;
                        case LAYOUT_HEIGHT:
                            mHeight = a.getLayoutDimension(attr, mHeight);
                            break;
                        case LAYOUT_CONSTRAINT_WIDTH:
                            ConstraintSet.parseDimensionConstraints(this, a, attr, HORIZONTAL);
                            break;
                        case LAYOUT_CONSTRAINT_HEIGHT:
                            ConstraintSet.parseDimensionConstraints(this, a, attr, VERTICAL);
                            break;
                        case WIDTH_DEFAULT:
                            widthDefault = a.getInt(attr, widthDefault);
                            break;
                        case HEIGHT_DEFAULT:
                            heightDefault = a.getInt(attr, heightDefault);
                            break;
                        case VERTICAL_WEIGHT:
                            verticalWeight = a.getFloat(attr, verticalWeight);
                            break;
                        case HORIZONTAL_WEIGHT:
                            horizontalWeight = a.getFloat(attr, horizontalWeight);
                            break;
                        case VERTICAL_STYLE:
                            verticalChainStyle = a.getInt(attr, verticalChainStyle);
                            break;
                        case HORIZONTAL_STYLE:
                            horizontalChainStyle = a.getInt(attr, horizontalChainStyle);
                            break;
                        case DIMENSION_RATIO:
                            dimensionRatio = a.getString(attr);
                            break;
                        case HEIGHT_MAX:
                            heightMax = a.getDimensionPixelSize(attr, heightMax);
                            break;
                        case WIDTH_MAX:
                            widthMax = a.getDimensionPixelSize(attr, widthMax);
                            break;
                        case HEIGHT_MIN:
                            heightMin = a.getDimensionPixelSize(attr, heightMin);
                            break;
                        case WIDTH_MIN:
                            widthMin = a.getDimensionPixelSize(attr, widthMin);
                            break;
                        case WIDTH_PERCENT:
                            widthPercent = a.getFloat(attr, 1);
                            break;
                        case HEIGHT_PERCENT:
                            heightPercent = a.getFloat(attr, 1);
                            break;
                        case CONSTRAINED_WIDTH:
                            constrainedWidth = a.getBoolean(attr, constrainedWidth);
                            break;
                        case CONSTRAINED_HEIGHT:
                            constrainedHeight = a.getBoolean(attr, constrainedHeight);
                            break;
                        case CHAIN_USE_RTL:
                            Log.e(TAG, "CURRENTLY UNSUPPORTED"); // TODO add support or remove
                                                                 //  TODO add support or remove  c.mChainUseRtl = a.getBoolean(attr,c.mChainUseRtl);
                            break;
                        case BARRIER_DIRECTION:
                            mBarrierDirection = a.getInt(attr, mBarrierDirection);
                            break;
                        case LAYOUT_WRAP_BEHAVIOR:
                            mWrapBehavior = a.getInt(attr, mWrapBehavior);
                            break;
                        case BARRIER_MARGIN:
                            mBarrierMargin = a.getDimensionPixelSize(attr, mBarrierMargin);
                            break;
                        case CONSTRAINT_REFERENCED_IDS:
                            mReferenceIdString = a.getString(attr);
                            break;
                        case BARRIER_ALLOWS_GONE_WIDGETS:
                            mBarrierAllowsGoneWidgets = a.getBoolean(attr, mBarrierAllowsGoneWidgets);
                            break;
                        case CONSTRAINT_TAG:
                            mConstraintTag = a.getString(attr);
                            break;
                        case UNUSED:
                            Log.w(TAG, "unused attribute 0x" + attr.ToString("x") + "   " + mapToConstant.get(attr));
                            break;
                        default:
                            Log.w(TAG, "Unknown attribute 0x" + attr.ToString("x") + "   " + mapToConstant.get(attr));

                            break;
                    }
                }
                a.recycle();
            }
*/

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
            public bool isGuideline = false;
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
            public void validate()
            {
                mIsGuideline = false;
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

                if (mWidth == WRAP_CONTENT && constrainedWidth)
                {
                    horizontalDimensionFixed = false;
                    //if (matchConstraintDefaultWidth == MATCH_CONSTRAINT_SPREAD)
                    if (matchConstraintDefaultWidth == MATCH_CONSTRAINT_SPREAD)
                    {
                        //matchConstraintDefaultWidth = MATCH_CONSTRAINT_WRAP;
                        matchConstraintDefaultWidth = MATCH_CONSTRAINT_WRAP;
                    }
                }
                if (mHeight == WRAP_CONTENT && constrainedHeight)
                {
                    verticalDimensionFixed = false;
                    //if (matchConstraintDefaultHeight == MATCH_CONSTRAINT_SPREAD)
                    if (matchConstraintDefaultHeight == MATCH_CONSTRAINT_SPREAD)
                    {
                        //matchConstraintDefaultHeight = MATCH_CONSTRAINT_WRAP;
                        matchConstraintDefaultHeight = MATCH_CONSTRAINT_WRAP;
                    }
                }
                if (mWidth == MATCH_CONSTRAINT || mWidth == ViewGroup.LayoutParams.MATCH_PARENT)
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
                if (mHeight == MATCH_CONSTRAINT || mHeight == ViewGroup.LayoutParams.MATCH_PARENT)
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
                    isGuideline = true;
                    horizontalDimensionFixed = true;
                    verticalDimensionFixed = true;
                    if (!(widget is Guideline)) {
                        widget = new Guideline();
                    }
                    ((Guideline)widget).setOrientation(orientation);
                }
            }

            public bool helped = false;
            #endregion
        }
    }
}

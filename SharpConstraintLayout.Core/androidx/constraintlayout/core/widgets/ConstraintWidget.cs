using System;
using System.Collections.Generic;
using System.Text;

/*
 * Copyright (C) 2015 The Android Open Source Project
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
namespace androidx.constraintlayout.core.widgets
{
    using androidx.constraintlayout.core;
    using WidgetFrame = androidx.constraintlayout.core.state.WidgetFrame;
    using androidx.constraintlayout.core.widgets.analyzer;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.LinearSystem.DEBUG;
using static androidx.constraintlayout.core.LinearSystem;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.LinearSystem.FULL_DEBUG;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
using static androidx.constraintlayout.core.widgets.ConstraintWidget.DimensionBehaviour;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;

    /// <summary>
    /// Implements a constraint Widget model supporting constraints relations between other widgets.
    /// <para>
    /// The widget has various anchors (i.e. Left, Top, Right, Bottom, representing their respective
    /// sides, as well as Baseline, Center_X and Center_Y). Connecting anchors from one widget to another
    /// represents a constraint relation between the two anchors; the <seealso cref="LinearSystem"/> will then
    /// be able to use this model to try to minimize the distances between connected anchors.
    /// </para>
    /// <para>
    /// If opposite anchors are connected (e.g. Left and Right anchors), if they have the same strength,
    /// the widget will be equally pulled toward their respective target anchor positions; if the widget
    /// has a fixed size, this means that the widget will be centered between the two target anchors. If
    /// the widget's size is allowed to adjust, the size of the widget will change to be as large as
    /// necessary so that the widget's anchors and the target anchors' distances are zero.
    /// </para>
    /// Constraints are set by connecting a widget's anchor to another via the
    /// <seealso cref="#connect"/> function.
    /// </summary>
    public class ConstraintWidget
    {
        private bool InstanceFieldsInitialized = false;

        private void InitializeInstanceFields()
        {
            frame = new WidgetFrame(this);
            mLeft = new ConstraintAnchor(this, ConstraintAnchor.Type.LEFT);
            mTop = new ConstraintAnchor(this, ConstraintAnchor.Type.TOP);
            mRight = new ConstraintAnchor(this, ConstraintAnchor.Type.RIGHT);
            mBottom = new ConstraintAnchor(this, ConstraintAnchor.Type.BOTTOM);
            mBaseline = new ConstraintAnchor(this, ConstraintAnchor.Type.BASELINE);
            mCenterX = new ConstraintAnchor(this, ConstraintAnchor.Type.CENTER_X);
            mCenterY = new ConstraintAnchor(this, ConstraintAnchor.Type.CENTER_Y);
            mCenter = new ConstraintAnchor(this, ConstraintAnchor.Type.CENTER);
            mListAnchors = new ConstraintAnchor[] {mLeft, mRight, mTop, mBottom, mBaseline, mCenter};
        }

        private const bool AUTOTAG_CENTER = false;
        protected internal const int SOLVER = 1;
        protected internal const int DIRECT = 2;

        // apply an intrinsic size when wrap content for spread dimensions
        private const bool USE_WRAP_DIMENSION_FOR_SPREAD = false;

        ////////////////////////////////////////////////////////////////////////////////////////////////
        // Graph measurements
        ////////////////////////////////////////////////////////////////////////////////////////////////

        public bool measured = false;
        public WidgetRun[] run = new WidgetRun[2];
        public ChainRun horizontalChainRun;
        public ChainRun verticalChainRun;

        public HorizontalWidgetRun horizontalRun = null;
        public VerticalWidgetRun verticalRun = null;

        public bool[] isTerminalWidget = new bool[] {true, true};
        internal bool mResolvedHasRatio = false;
        private bool mMeasureRequested = true;
        private bool OPTIMIZE_WRAP = false;
        private bool OPTIMIZE_WRAP_ON_RESOLVED = true;

        private int mWidthOverride = -1;
        private int mHeightOverride = -1;

        public WidgetFrame frame;

        public string stringId;

        public virtual WidgetRun getRun(int orientation)
        {
            if (orientation == HORIZONTAL)
            {
                return horizontalRun;
            }
            else if (orientation == VERTICAL)
            {
                return verticalRun;
            }
            return null;
        }

        private bool resolvedHorizontal = false;
        private bool resolvedVertical = false;

        private bool horizontalSolvingPass = false;
        private bool verticalSolvingPass = false;

        public virtual void setFinalFrame(int left, int top, int right, int bottom, int baseline, int orientation)
        {
            setFrame(left, top, right, bottom);
            BaselineDistance = baseline;
            if (orientation == HORIZONTAL)
            {
                resolvedHorizontal = true;
                resolvedVertical = false;
            }
            else if (orientation == VERTICAL)
            {
                resolvedHorizontal = false;
                resolvedVertical = true;
            }
            else if (orientation == BOTH)
            {
                resolvedHorizontal = true;
                resolvedVertical = true;
            }
            else
            {
                resolvedHorizontal = false;
                resolvedVertical = false;
            }
        }

        public virtual int FinalLeft
        {
            set
            {
                mLeft.FinalValue = value;
                mX = value;
            }
        }

        public virtual int FinalTop
        {
            set
            {
                mTop.FinalValue = value;
                mY = value;
            }
        }

        public virtual void resetSolvingPassFlag()
        {
            horizontalSolvingPass = false;
            verticalSolvingPass = false;
        }

        public virtual bool HorizontalSolvingPassDone
        {
            get
            {
                return horizontalSolvingPass;
            }
        }

        public virtual bool VerticalSolvingPassDone
        {
            get
            {
                return verticalSolvingPass;
            }
        }

        public virtual void markHorizontalSolvingPassDone()
        {
            horizontalSolvingPass = true;
        }

        public virtual void markVerticalSolvingPassDone()
        {
            verticalSolvingPass = true;
        }

        public virtual void setFinalHorizontal(int x1, int x2)
        {
            if (resolvedHorizontal)
            {
                return;
            }
            mLeft.FinalValue = x1;
            mRight.FinalValue = x2;
            mX = x1;
            mWidth = x2 - x1;
            resolvedHorizontal = true;
            if (LinearSystem.FULL_DEBUG)
            {
                Console.WriteLine("*** SET FINAL HORIZONTAL FOR " + DebugName + " : " + x1 + " -> " + x2 + " (width: " + mWidth + ")");
            }
        }

        public virtual void setFinalVertical(int y1, int y2)
        {
            if (resolvedVertical)
            {
                return;
            }
            mTop.FinalValue = y1;
            mBottom.FinalValue = y2;
            mY = y1;
            mHeight = y2 - y1;
            if (hasBaseline_Renamed)
            {
                mBaseline.FinalValue = y1 + mBaselineDistance;
            }
            resolvedVertical = true;
            if (LinearSystem.FULL_DEBUG)
            {
                Console.WriteLine("*** SET FINAL VERTICAL FOR " + DebugName + " : " + y1 + " -> " + y2 + " (height: " + mHeight + ")");
            }
        }

        public virtual int FinalBaseline
        {
            set
            {
                if (!hasBaseline_Renamed)
                {
                    return;
                }
                int y1 = value - mBaselineDistance;
                int y2 = y1 + mHeight;
                mY = y1;
                mTop.FinalValue = y1;
                mBottom.FinalValue = y2;
                mBaseline.FinalValue = value;
                resolvedVertical = true;
            }
        }

        public virtual bool ResolvedHorizontally
        {
            get
            {
                return resolvedHorizontal || (mLeft.hasFinalValue() && mRight.hasFinalValue());
            }
        }

        public virtual bool ResolvedVertically
        {
            get
            {
                return resolvedVertical || (mTop.hasFinalValue() && mBottom.hasFinalValue());
            }
        }

        public virtual void resetFinalResolution()
        {
            resolvedHorizontal = false;
            resolvedVertical = false;
            horizontalSolvingPass = false;
            verticalSolvingPass = false;
            for (int i = 0, mAnchorsSize = mAnchors.Count; i < mAnchorsSize; i++)
            {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ConstraintAnchor anchor = mAnchors.get(i);
                ConstraintAnchor anchor = mAnchors[i];
                anchor.resetFinalResolution();
            }
        }

        public virtual void ensureMeasureRequested()
        {
            mMeasureRequested = true;
        }

        public virtual bool hasDependencies()
        {
            for (int i = 0, mAnchorsSize = mAnchors.Count; i < mAnchorsSize; i++)
            {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ConstraintAnchor anchor = mAnchors.get(i);
                ConstraintAnchor anchor = mAnchors[i];
                if (anchor.hasDependents())
                {
                    return true;
                }
            }
            return false;
        }

        public virtual bool hasDanglingDimension(int orientation)
        {
            if (orientation == HORIZONTAL)
            {
                int horizontalTargets = (mLeft.mTarget != null ? 1 : 0) + (mRight.mTarget != null ? 1 : 0);
                return horizontalTargets < 2;
            }
            else
            {
                int verticalTargets = (mTop.mTarget != null ? 1 : 0) + (mBottom.mTarget != null ? 1 : 0) + (mBaseline.mTarget != null ? 1 : 0);
                return verticalTargets < 2;
            }
        }

        public virtual bool hasResolvedTargets(int orientation, int size)
        {
            if (orientation == HORIZONTAL)
            {
                if (mLeft.mTarget != null && mLeft.mTarget.hasFinalValue() && mRight.mTarget != null && mRight.mTarget.hasFinalValue())
                {
                    return ((mRight.mTarget.FinalValue - mRight.Margin) - (mLeft.mTarget.FinalValue + mLeft.Margin)) >= size;
                }
            }
            else
            {
                if (mTop.mTarget != null && mTop.mTarget.hasFinalValue() && mBottom.mTarget != null && mBottom.mTarget.hasFinalValue())
                {
                    return ((mBottom.mTarget.FinalValue - mBottom.Margin) - (mTop.mTarget.FinalValue + mTop.Margin)) >= size;
                }
            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////

        public const int MATCH_CONSTRAINT_SPREAD = 0;
        public const int MATCH_CONSTRAINT_WRAP = 1;
        public const int MATCH_CONSTRAINT_PERCENT = 2;
        public const int MATCH_CONSTRAINT_RATIO = 3;
        public const int MATCH_CONSTRAINT_RATIO_RESOLVED = 4;

        public const int UNKNOWN = -1;
        public const int HORIZONTAL = 0;
        public const int VERTICAL = 1;
        public const int BOTH = 2;

        public const int VISIBLE = 0;
        public const int INVISIBLE = 4;
        public const int GONE = 8;

        // Values of the chain styles

        /// <summary>
        /// Spread like divide equally
        /// </summary>
        public const int CHAIN_SPREAD = 0;
        public const int CHAIN_SPREAD_INSIDE = 1;
        public const int CHAIN_PACKED = 2;

        // Values of the wrap behavior in parent
        public const int WRAP_BEHAVIOR_INCLUDED = 0; // default
        public const int WRAP_BEHAVIOR_HORIZONTAL_ONLY = 1;
        public const int WRAP_BEHAVIOR_VERTICAL_ONLY = 2;
        public const int WRAP_BEHAVIOR_SKIPPED = 3;

        // Support for direct resolution
        public int mHorizontalResolution = UNKNOWN;
        public int mVerticalResolution = UNKNOWN;

        private const int WRAP = -2;

        private int mWrapBehaviorInParent = WRAP_BEHAVIOR_INCLUDED;

        public int mMatchConstraintDefaultWidth = MATCH_CONSTRAINT_SPREAD;
        public int mMatchConstraintDefaultHeight = MATCH_CONSTRAINT_SPREAD;
        public int[] mResolvedMatchConstraintDefault = new int[2];

        public int mMatchConstraintMinWidth = 0;
        public int mMatchConstraintMaxWidth = 0;
        public float mMatchConstraintPercentWidth = 1;
        public int mMatchConstraintMinHeight = 0;
        public int mMatchConstraintMaxHeight = 0;
        public float mMatchConstraintPercentHeight = 1;
        public bool mIsWidthWrapContent;
        public bool mIsHeightWrapContent;

        internal int mResolvedDimensionRatioSide = UNKNOWN;
        internal float mResolvedDimensionRatio = 1.0f;

        private int[] mMaxDimension = new int[] {int.MaxValue, int.MaxValue};
        private float mCircleConstraintAngle = 0;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        private bool hasBaseline_Renamed = false;
        private bool inPlaceholder;

        private bool mInVirtualLayout = false;

        public virtual bool InVirtualLayout
        {
            get
            {
                return mInVirtualLayout;
            }
            set
            {
                mInVirtualLayout = value;
            }
        }


        public virtual int MaxHeight
        {
            get
            {
                return mMaxDimension[VERTICAL];
            }
            set
            {
                mMaxDimension[VERTICAL] = value;
            }
        }

        public virtual int MaxWidth
        {
            get
            {
                return mMaxDimension[HORIZONTAL];
            }
            set
            {
                mMaxDimension[HORIZONTAL] = value;
            }
        }



        public virtual bool SpreadWidth
        {
            get
            {
                return mMatchConstraintDefaultWidth == MATCH_CONSTRAINT_SPREAD && mDimensionRatio == 0 && mMatchConstraintMinWidth == 0 && mMatchConstraintMaxWidth == 0 && mListDimensionBehaviors[HORIZONTAL] == MATCH_CONSTRAINT;
            }
        }

        public virtual bool SpreadHeight
        {
            get
            {
                return mMatchConstraintDefaultHeight == MATCH_CONSTRAINT_SPREAD && mDimensionRatio == 0 && mMatchConstraintMinHeight == 0 && mMatchConstraintMaxHeight == 0 && mListDimensionBehaviors[VERTICAL] == MATCH_CONSTRAINT;
            }
        }

        public virtual bool HasBaseline
        {
            set
            {
                this.hasBaseline_Renamed = value;
            }
            get
            {
                return hasBaseline_Renamed;
            }
        }


        public virtual bool InPlaceholder
        {
            get
            {
                return inPlaceholder;
            }
            set
            {
                this.inPlaceholder = value;
            }
        }


        protected internal virtual void setInBarrier(int orientation, bool value)
        {
            mIsInBarrier[orientation] = value;
        }

        public virtual bool isInBarrier(int orientation)
        {
            return mIsInBarrier[orientation];
        }

        public virtual bool MeasureRequested
        {
            set
            {
                mMeasureRequested = value;
            }
            get
            {
                return mMeasureRequested && mVisibility != GONE;
            }
        }


        public virtual int WrapBehaviorInParent
        {
            set
            {
                if (value >= 0 && value <= WRAP_BEHAVIOR_SKIPPED)
                {
                    mWrapBehaviorInParent = value;
                }
            }
            get
            {
                return mWrapBehaviorInParent;
            }
        }


        /// <summary>
        /// Keep a cache of the last measure cache as we can bypass remeasures during the onMeasure...
        /// the View's measure cache will only be reset in onLayout, so too late for us.
        /// </summary>
        private int mLastHorizontalMeasureSpec = 0;
        private int mLastVerticalMeasureSpec = 0;
        public virtual int LastHorizontalMeasureSpec
        {
            get
            {
                return mLastHorizontalMeasureSpec;
            }
        }
        public virtual int LastVerticalMeasureSpec
        {
            get
            {
                return mLastVerticalMeasureSpec;
            }
        }
        public virtual void setLastMeasureSpec(int horizontal, int vertical)
        {
            mLastHorizontalMeasureSpec = horizontal;
            mLastVerticalMeasureSpec = vertical;
            MeasureRequested = false;
        }

        /// <summary>
        /// Define how the widget will resize
        /// </summary>
        public enum DimensionBehaviour
        {
            FIXED,
            WRAP_CONTENT,
            MATCH_CONSTRAINT,
            MATCH_PARENT
        }

        // The anchors available on the widget
        // note: all anchors should be added to the mAnchors array (see addAnchors())
        public ConstraintAnchor mLeft;
        public ConstraintAnchor mTop;
        public ConstraintAnchor mRight;
        public ConstraintAnchor mBottom;
        public ConstraintAnchor mBaseline;
        internal ConstraintAnchor mCenterX;
        internal ConstraintAnchor mCenterY;
        public ConstraintAnchor mCenter;

        public const int ANCHOR_LEFT = 0;
        public const int ANCHOR_RIGHT = 1;
        public const int ANCHOR_TOP = 2;
        public const int ANCHOR_BOTTOM = 3;
        public const int ANCHOR_BASELINE = 4;

        public ConstraintAnchor[] mListAnchors;
        protected internal List<ConstraintAnchor> mAnchors = new List<ConstraintAnchor>();

        private bool[] mIsInBarrier = new bool[2];

        // The horizontal and vertical behaviour for the widgets' dimensions
        internal const int DIMENSION_HORIZONTAL = 0;
        internal const int DIMENSION_VERTICAL = 1;
        public DimensionBehaviour[] mListDimensionBehaviors = new DimensionBehaviour[] {DimensionBehaviour.FIXED, DimensionBehaviour.FIXED};

        // Parent of this widget
        public ConstraintWidget mParent = null;

        // Dimensions of the widget
        internal int mWidth = 0;
        internal int mHeight = 0;
        public float mDimensionRatio = 0;
        protected internal int mDimensionRatioSide = UNKNOWN;

        // Origin of the widget
        protected internal int mX = 0;
        protected internal int mY = 0;
        internal int mRelX = 0;
        internal int mRelY = 0;

        // Root offset
        protected internal int mOffsetX = 0;
        protected internal int mOffsetY = 0;

        // Baseline distance relative to the top of the widget
        internal int mBaselineDistance = 0;

        // Minimum sizes for the widget
        protected internal int mMinWidth;
        protected internal int mMinHeight;

        // Percentages used for biasing one connection over another when dual connections
        // of the same strength exist
        public static float DEFAULT_BIAS = 0.5f;
        internal float mHorizontalBiasPercent = DEFAULT_BIAS;
        internal float mVerticalBiasPercent = DEFAULT_BIAS;

        // The companion widget (typically, the real widget we represent)
        private object mCompanionWidget;

        // This is used to possibly "skip" a position while inside a container. For example,
        // a container like Table can use this to implement empty cells
        // (the item positioned after the empty cell will have a skip value of 1)
        private int mContainerItemSkip = 0;

        // Contains the visibility status of the widget (VISIBLE, INVISIBLE, or GONE)
        private int mVisibility = VISIBLE;

        private string mDebugName = null;
        private string mType = null;

        internal int mDistToTop;
        internal int mDistToLeft;
        internal int mDistToRight;
        internal int mDistToBottom;
        internal bool mLeftHasCentered;
        internal bool mRightHasCentered;
        internal bool mTopHasCentered;
        internal bool mBottomHasCentered;
        internal bool mHorizontalWrapVisited;
        internal bool mVerticalWrapVisited;
        internal bool mGroupsToSolver = false;

        // Chain support
        internal int mHorizontalChainStyle = CHAIN_SPREAD;
        internal int mVerticalChainStyle = CHAIN_SPREAD;
        internal bool mHorizontalChainFixedPosition;
        internal bool mVerticalChainFixedPosition;

        public float[] mWeight = new float[] {UNKNOWN, UNKNOWN};

        protected internal ConstraintWidget[] mListNextMatchConstraintsWidget = new ConstraintWidget[] {null, null};
        protected internal ConstraintWidget[] mNextChainWidget = new ConstraintWidget[] {null, null};

        internal ConstraintWidget mHorizontalNextWidget = null;
        internal ConstraintWidget mVerticalNextWidget = null;

        // TODO: see if we can make this simpler
        public virtual void reset()
        {
            mLeft.reset();
            mTop.reset();
            mRight.reset();
            mBottom.reset();
            mBaseline.reset();
            mCenterX.reset();
            mCenterY.reset();
            mCenter.reset();
            mParent = null;
            mCircleConstraintAngle = 0;
            mWidth = 0;
            mHeight = 0;
            mDimensionRatio = 0;
            mDimensionRatioSide = UNKNOWN;
            mX = 0;
            mY = 0;
            mOffsetX = 0;
            mOffsetY = 0;
            mBaselineDistance = 0;
            mMinWidth = 0;
            mMinHeight = 0;
            mHorizontalBiasPercent = DEFAULT_BIAS;
            mVerticalBiasPercent = DEFAULT_BIAS;
            mListDimensionBehaviors[DIMENSION_HORIZONTAL] = DimensionBehaviour.FIXED;
            mListDimensionBehaviors[DIMENSION_VERTICAL] = DimensionBehaviour.FIXED;
            mCompanionWidget = null;
            mContainerItemSkip = 0;
            mVisibility = VISIBLE;
            mType = null;
            mHorizontalWrapVisited = false;
            mVerticalWrapVisited = false;
            mHorizontalChainStyle = CHAIN_SPREAD;
            mVerticalChainStyle = CHAIN_SPREAD;
            mHorizontalChainFixedPosition = false;
            mVerticalChainFixedPosition = false;
            mWeight[DIMENSION_HORIZONTAL] = UNKNOWN;
            mWeight[DIMENSION_VERTICAL] = UNKNOWN;
            mHorizontalResolution = UNKNOWN;
            mVerticalResolution = UNKNOWN;
            mMaxDimension[HORIZONTAL] = int.MaxValue;
            mMaxDimension[VERTICAL] = int.MaxValue;
            mMatchConstraintDefaultWidth = MATCH_CONSTRAINT_SPREAD;
            mMatchConstraintDefaultHeight = MATCH_CONSTRAINT_SPREAD;
            mMatchConstraintPercentWidth = 1;
            mMatchConstraintPercentHeight = 1;
            mMatchConstraintMaxWidth = int.MaxValue;
            mMatchConstraintMaxHeight = int.MaxValue;
            mMatchConstraintMinWidth = 0;
            mMatchConstraintMinHeight = 0;
            mResolvedHasRatio = false;
            mResolvedDimensionRatioSide = UNKNOWN;
            mResolvedDimensionRatio = 1f;
            mGroupsToSolver = false;
            isTerminalWidget[HORIZONTAL] = true;
            isTerminalWidget[VERTICAL] = true;
            mInVirtualLayout = false;
            mIsInBarrier[HORIZONTAL] = false;
            mIsInBarrier[VERTICAL] = false;
            mMeasureRequested = true;
            mResolvedMatchConstraintDefault[HORIZONTAL] = 0;
            mResolvedMatchConstraintDefault[VERTICAL] = 0;
            mWidthOverride = -1;
            mHeightOverride = -1;
        }

        ///////////////////////////////////SERIALIZE///////////////////////////////////////////////

        private void serializeAnchor(StringBuilder ret, string side, ConstraintAnchor a)
        {
            if (a.mTarget == null)
            {
                return;
            }
            ret.Append(side);
            ret.Append(" : [ '");
            ret.Append(a.mTarget);
            ret.Append("',");
            ret.Append(a.mMargin);
            ret.Append(",");
            ret.Append(a.mGoneMargin);
            ret.Append(",");
            ret.Append(" ] ,\n");
        }
        private void serializeCircle(StringBuilder ret, ConstraintAnchor a, float angle)
        {
            if (a.mTarget == null)
            {
                return;
            }

            ret.Append("circle : [ '");
            ret.Append(a.mTarget);
            ret.Append("',");
            ret.Append(a.mMargin);
            ret.Append(",");
            ret.Append(angle);
            ret.Append(",");
            ret.Append(" ] ,\n");
        }

        private void serializeAttribute(StringBuilder ret, string type, float value, float def)
        {
            if (value == def)
            {
                return;
            }
            ret.Append(type);
            ret.Append(" :   ");
            ret.Append(def);
            ret.Append(",\n");
        }

        private void serializeDimensionRatio(StringBuilder ret, string type, float value, int whichSide)
        {
            if (value == 0)
            {
                return;
            }
            ret.Append(type);
            ret.Append(" :  [");
            ret.Append(value);
            ret.Append(",");
            ret.Append(whichSide);
            ret.Append("");
            ret.Append("],\n");
        }

        private void serializeSize(StringBuilder ret, string type, int size, int min, int max, int @override, int matchConstraintMin, int matchConstraintDefault, float MatchConstraintPercent, float weight)
        {
            ret.Append(type);
            ret.Append(" :  {\n");
            serializeAttribute(ret,"size",size,int.MinValue);
            serializeAttribute(ret,"min",min,0);
            serializeAttribute(ret,"max",max, int.MaxValue);
            serializeAttribute(ret,"matchMin",matchConstraintMin, 0);
            serializeAttribute(ret,"matchDef",matchConstraintDefault, MATCH_CONSTRAINT_SPREAD);
            serializeAttribute(ret,"matchPercent",matchConstraintDefault, 1);
            ret.Append("},\n");
        }

        public virtual StringBuilder serialize(StringBuilder ret)
        {
            ret.Append("{\n");
            serializeAnchor(ret,"left", mLeft);
            serializeAnchor(ret,"top", mTop);
            serializeAnchor(ret,"right", mRight);
            serializeAnchor(ret,"bottom", mBottom);
            serializeAnchor(ret,"baseline", mBaseline);
            serializeAnchor(ret,"centerX", mCenterX);
            serializeAnchor(ret,"centerY", mCenterY);
            serializeCircle(ret, mCenter,mCircleConstraintAngle);

            serializeSize(ret,"width", mWidth, mMinWidth, mMaxDimension[HORIZONTAL], mWidthOverride, mMatchConstraintMinWidth, mMatchConstraintDefaultWidth, mMatchConstraintPercentWidth, mWeight[DIMENSION_HORIZONTAL]);

            serializeSize(ret,"height", mHeight, mMinHeight, mMaxDimension[VERTICAL], mHeightOverride, mMatchConstraintMinHeight, mMatchConstraintDefaultHeight, mMatchConstraintPercentHeight, mWeight[DIMENSION_VERTICAL]);

            serializeDimensionRatio(ret,"dimensionRatio",mDimensionRatio, mDimensionRatioSide);
            serializeAttribute(ret,"horizontalBias",mHorizontalBiasPercent,DEFAULT_BIAS);
            serializeAttribute(ret,"verticalBias",mVerticalBiasPercent,DEFAULT_BIAS);
            ret.Append("}\n");

            return ret;
        }
        ///////////////////////////////////END SERIALIZE///////////////////////////////////////////

        public int horizontalGroup = -1;
        public int verticalGroup = -1;

        public virtual bool oppositeDimensionDependsOn(int orientation)
        {
            int oppositeOrientation = (orientation == HORIZONTAL) ? VERTICAL : HORIZONTAL;
            DimensionBehaviour dimensionBehaviour = mListDimensionBehaviors[orientation];
            DimensionBehaviour oppositeDimensionBehaviour = mListDimensionBehaviors[oppositeOrientation];
            return dimensionBehaviour == MATCH_CONSTRAINT && oppositeDimensionBehaviour == MATCH_CONSTRAINT;
                    //&& mDimensionRatio != 0;
        }

        public virtual bool oppositeDimensionsTied()
        {
            return (mListDimensionBehaviors[HORIZONTAL] == MATCH_CONSTRAINT && mListDimensionBehaviors[VERTICAL] == MATCH_CONSTRAINT); // isInHorizontalChain() || isInVerticalChain() ||
        }

        public virtual bool hasDimensionOverride()
        {
            return mWidthOverride != -1 || mHeightOverride != -1;
        }

        /*-----------------------------------------------------------------------*/
        // Creation
        /*-----------------------------------------------------------------------*/

        /// <summary>
        /// Default constructor
        /// </summary>
        public ConstraintWidget()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
            addAnchors();
        }

        public ConstraintWidget(string debugName)
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
            addAnchors();
            DebugName = debugName;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="x">      x position </param>
        /// <param name="y">      y position </param>
        /// <param name="width">  width of the layout </param>
        /// <param name="height"> height of the layout </param>
        public ConstraintWidget(int x, int y, int width, int height)
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
            mX = x;
            mY = y;
            mWidth = width;
            mHeight = height;
            addAnchors();
        }

        public ConstraintWidget(string debugName, int x, int y, int width, int height) : this(x, y, width, height)
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
            DebugName = debugName;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="width">  width of the layout </param>
        /// <param name="height"> height of the layout </param>
        public ConstraintWidget(int width, int height) : this(0, 0, width, height)
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        public virtual void ensureWidgetRuns()
        {
            if (horizontalRun == null)
            {
                horizontalRun = new HorizontalWidgetRun(this);
            }
            if (verticalRun == null)
            {
                verticalRun = new VerticalWidgetRun(this);
            }
        }

        public ConstraintWidget(string debugName, int width, int height) : this(width, height)
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
            DebugName = debugName;
        }

        /// <summary>
        /// Reset the solver variables of the anchors
        /// </summary>
        public virtual void resetSolverVariables(Cache cache)
        {
            mLeft.resetSolverVariable(cache);
            mTop.resetSolverVariable(cache);
            mRight.resetSolverVariable(cache);
            mBottom.resetSolverVariable(cache);
            mBaseline.resetSolverVariable(cache);
            mCenter.resetSolverVariable(cache);
            mCenterX.resetSolverVariable(cache);
            mCenterY.resetSolverVariable(cache);
        }

        /// <summary>
        /// Add all the anchors to the mAnchors array
        /// </summary>
        private void addAnchors()
        {
            mAnchors.Add(mLeft);
            mAnchors.Add(mTop);
            mAnchors.Add(mRight);
            mAnchors.Add(mBottom);
            mAnchors.Add(mCenterX);
            mAnchors.Add(mCenterY);
            mAnchors.Add(mCenter);
            mAnchors.Add(mBaseline);
        }

        /// <summary>
        /// Returns true if the widget is the root widget
        /// </summary>
        /// <returns> true if root widget, false otherwise </returns>
        public virtual bool Root
        {
            get
            {
                return mParent == null;
            }
        }

        /// <summary>
        /// Returns the parent of this widget if there is one
        /// </summary>
        /// <returns> parent </returns>
        public virtual ConstraintWidget Parent
        {
            get
            {
                return mParent;
            }
            set
            {
                mParent = value;
            }
        }


        /// <summary>
        /// Keep track of wrap_content for width
        /// </summary>
        /// <param name="widthWrapContent"> </param>
        public virtual bool WidthWrapContent
        {
            set
            {
                this.mIsWidthWrapContent = value;
            }
            get
            {
                return mIsWidthWrapContent;
            }
        }


        /// <summary>
        /// Keep track of wrap_content for height
        /// </summary>
        /// <param name="heightWrapContent"> </param>
        public virtual bool HeightWrapContent
        {
            set
            {
                this.mIsHeightWrapContent = value;
            }
            get
            {
                return mIsHeightWrapContent;
            }
        }


        /// <summary>
        /// Set a circular constraint
        /// </summary>
        /// <param name="target"> the target widget we will use as the center of the circle </param>
        /// <param name="angle">  the angle (from 0 to 360) </param>
        /// <param name="radius"> the radius used </param>
        public virtual void connectCircularConstraint(ConstraintWidget target, float angle, int radius)
        {
            immediateConnect(ConstraintAnchor.Type.CENTER, target, ConstraintAnchor.Type.CENTER, radius, 0);
            mCircleConstraintAngle = angle;
        }

        /// <summary>
        /// Returns the type string if set
        /// </summary>
        /// <returns> type (null if not set) </returns>
        public virtual string Type
        {
            get
            {
                return mType;
            }
            set
            {
                mType = value;
            }
        }


        /// <summary>
        /// Set the visibility for this widget
        /// </summary>
        /// <param name="visibility"> either VISIBLE, INVISIBLE, or GONE </param>
        public virtual int Visibility
        {
            set
            {
                mVisibility = value;
            }
            get
            {
                return mVisibility;
            }
        }


        /// <summary>
        /// Returns the name of this widget (used for debug purposes)
        /// </summary>
        /// <returns> the debug name </returns>
        public virtual string DebugName
        {
            get
            {
                return mDebugName;
            }
            set
            {
                mDebugName = value;
            }
        }


        /// <summary>
        /// Utility debug function. Sets the names of the anchors in the solver given
        /// a widget's name. The given name is used as a prefix, resulting in anchors' names
        /// of the form:
        /// <p/>
        /// <ul>
        /// <li>{name}.left</li>
        /// <li>{name}.top</li>
        /// <li>{name}.right</li>
        /// <li>{name}.bottom</li>
        /// <li>{name}.baseline</li>
        /// </ul>
        /// </summary>
        /// <param name="system"> solver used </param>
        /// <param name="name">   name of the widget </param>
        public virtual void setDebugSolverName(LinearSystem system, string name)
        {
            mDebugName = name;
            SolverVariable left = system.createObjectVariable(mLeft);
            SolverVariable top = system.createObjectVariable(mTop);
            SolverVariable right = system.createObjectVariable(mRight);
            SolverVariable bottom = system.createObjectVariable(mBottom);
            left.Name = name + ".left";
            top.Name = name + ".top";
            right.Name = name + ".right";
            bottom.Name = name + ".bottom";
            SolverVariable baseline = system.createObjectVariable(mBaseline);
            baseline.Name = name + ".baseline";
        }

        /// <summary>
        /// Create all the system variables for this widget
        /// </summary>
        /// <param name="system">
        /// @suppress </param>
        public virtual void createObjectVariables(LinearSystem system)
        {
            SolverVariable left = system.createObjectVariable(mLeft);
            SolverVariable top = system.createObjectVariable(mTop);
            SolverVariable right = system.createObjectVariable(mRight);
            SolverVariable bottom = system.createObjectVariable(mBottom);
            if (mBaselineDistance > 0)
            {
                SolverVariable baseline = system.createObjectVariable(mBaseline);
            }
        }

        /// <summary>
        /// Returns a string representation of the ConstraintWidget
        /// </summary>
        /// <returns> string representation of the widget </returns>
        public override string ToString()
        {
            return (!string.ReferenceEquals(mType, null) ? "type: " + mType + " " : "") + (!string.ReferenceEquals(mDebugName, null) ? "id: " + mDebugName + " " : "") + "(" + mX + ", " + mY + ") - (" + mWidth + " x " + mHeight + ")";
        }

        /*-----------------------------------------------------------------------*/
        // Position
        /*-----------------------------------------------------------------------*/
        // The widget position is expressed in two ways:
        // - relative to its direct parent container (getX(), getY())
        // - relative to the root container (getDrawX(), getDrawY())
        // Additionally, getDrawX()/getDrawY() are used when animating the
        // widget position on screen
        /*-----------------------------------------------------------------------*/

        /// <summary>
        /// Return the x position of the widget, relative to its container
        /// </summary>
        /// <returns> x position </returns>
        public virtual int X
        {
            get
            {
                if (mParent != null && mParent is ConstraintWidgetContainer)
                {
                    return ((ConstraintWidgetContainer)mParent).mPaddingLeft + mX;
                }
                return mX;
            }
            set
            {
                mX = value;
            }
        }

        /// <summary>
        /// Return the y position of the widget, relative to its container
        /// </summary>
        /// <returns> y position </returns>
        public virtual int Y
        {
            get
            {
                if (mParent != null && mParent is ConstraintWidgetContainer)
                {
                    return ((ConstraintWidgetContainer)mParent).mPaddingTop + mY;
                }
                return mY;
            }
            set
            {
                mY = value;
            }
        }

        /// <summary>
        /// Return the width of the widget
        /// </summary>
        /// <returns> width width </returns>
        public virtual int Width
        {
            get
            {
                if (mVisibility == ConstraintWidget.GONE)
                {
                    return 0;
                }
                return mWidth;
            }
            set
            {
                mWidth = value;
                if (mWidth < mMinWidth)
                {
                    mWidth = mMinWidth;
                }
            }
        }

        public virtual int OptimizerWrapWidth
        {
            get
            {
                int w = mWidth;
                if (mListDimensionBehaviors[DIMENSION_HORIZONTAL] == MATCH_CONSTRAINT)
                {
                    if (mMatchConstraintDefaultWidth == MATCH_CONSTRAINT_WRAP)
                    {
                        w = Math.Max(mMatchConstraintMinWidth, w);
                    }
                    else if (mMatchConstraintMinWidth > 0)
                    {
                        w = mMatchConstraintMinWidth;
                        mWidth = w;
                    }
                    else
                    {
                        w = 0;
                    }
                    if (mMatchConstraintMaxWidth > 0 && mMatchConstraintMaxWidth < w)
                    {
                        w = mMatchConstraintMaxWidth;
                    }
                }
                return w;
            }
        }

        public virtual int OptimizerWrapHeight
        {
            get
            {
                int h = mHeight;
                if (mListDimensionBehaviors[DIMENSION_VERTICAL] == MATCH_CONSTRAINT)
                {
                    if (mMatchConstraintDefaultHeight == MATCH_CONSTRAINT_WRAP)
                    {
                        h = Math.Max(mMatchConstraintMinHeight, h);
                    }
                    else if (mMatchConstraintMinHeight > 0)
                    {
                        h = mMatchConstraintMinHeight;
                        mHeight = h;
                    }
                    else
                    {
                        h = 0;
                    }
                    if (mMatchConstraintMaxHeight > 0 && mMatchConstraintMaxHeight < h)
                    {
                        h = mMatchConstraintMaxHeight;
                    }
                }
                return h;
            }
        }

        /// <summary>
        /// Return the height of the widget
        /// </summary>
        /// <returns> height height </returns>
        public virtual int Height
        {
            get
            {
                if (mVisibility == ConstraintWidget.GONE)
                {
                    return 0;
                }
                return mHeight;
            }
            set
            {
                mHeight = value;
                if (mHeight < mMinHeight)
                {
                    mHeight = mMinHeight;
                }
            }
        }

        /// <summary>
        /// Get a dimension of the widget in a particular orientation.
        /// </summary>
        /// <param name="orientation"> </param>
        /// <returns> The dimension of the specified orientation. </returns>
        public virtual int getLength(int orientation)
        {
            if (orientation == HORIZONTAL)
            {
                return Width;
            }
            else if (orientation == VERTICAL)
            {
                return Height;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Return the x position of the widget, relative to the root
        /// (without animation)
        /// </summary>
        /// <returns> x position </returns>
        protected internal virtual int RootX
        {
            get
            {
                return mX + mOffsetX;
            }
        }

        /// <summary>
        /// Return the y position of the widget, relative to the root
        /// (without animation)
        /// 
        /// @return
        /// </summary>
        protected internal virtual int RootY
        {
            get
            {
                return mY + mOffsetY;
            }
        }

        /// <summary>
        /// Return the minimum width of the widget
        /// </summary>
        /// <returns> minimum width </returns>
        public virtual int MinWidth
        {
            get
            {
                return mMinWidth;
            }
            set
            {
                if (value < 0)
                {
                    mMinWidth = 0;
                }
                else
                {
                    mMinWidth = value;
                }
            }
        }

        /// <summary>
        /// Return the minimum height of the widget
        /// </summary>
        /// <returns> minimum height </returns>
        public virtual int MinHeight
        {
            get
            {
                return mMinHeight;
            }
            set
            {
                if (value < 0)
                {
                    mMinHeight = 0;
                }
                else
                {
                    mMinHeight = value;
                }
            }
        }

        /// <summary>
        /// Return the left position of the widget (similar to <seealso cref="#getX()"/>)
        /// </summary>
        /// <returns> left position of the widget </returns>
        public virtual int Left
        {
            get
            {
                return X;
            }
        }

        /// <summary>
        /// Return the top position of the widget (similar to <seealso cref="#getY()"/>)
        /// </summary>
        /// <returns> top position of the widget </returns>
        public virtual int Top
        {
            get
            {
                return Y;
            }
        }

        /// <summary>
        /// Return the right position of the widget
        /// </summary>
        /// <returns> right position of the widget </returns>
        public virtual int Right
        {
            get
            {
                return X + mWidth;
            }
        }

        /// <summary>
        /// Return the bottom position of the widget
        /// </summary>
        /// <returns> bottom position of the widget </returns>
        public virtual int Bottom
        {
            get
            {
                return Y + mHeight;
            }
        }

        /// <summary>
        /// Returns all the horizontal margin of the widget.
        /// </summary>
        public virtual int HorizontalMargin
        {
            get
            {
                int margin = 0;
                if (mLeft != null)
                {
                    margin += mLeft.mMargin;
                }
                if (mRight != null)
                {
                    margin += mRight.mMargin;
                }
                return margin;
            }
        }

        /// <summary>
        /// Returns all the vertical margin of the widget
        /// </summary>
        public virtual int VerticalMargin
        {
            get
            {
                int margin = 0;
                if (mLeft != null)
                {
                    margin += mTop.mMargin;
                }
                if (mRight != null)
                {
                    margin += mBottom.mMargin;
                }
                return margin;
            }
        }

        /// <summary>
        /// Return the horizontal percentage bias that is used when two opposite connections
        /// exist of the same strength.
        /// </summary>
        /// <returns> horizontal percentage bias </returns>
        public virtual float HorizontalBiasPercent
        {
            get
            {
                return mHorizontalBiasPercent;
            }
            set
            {
                mHorizontalBiasPercent = value;
            }
        }

        /// <summary>
        /// Return the vertical percentage bias that is used when two opposite connections
        /// exist of the same strength.
        /// </summary>
        /// <returns> vertical percentage bias </returns>
        public virtual float VerticalBiasPercent
        {
            get
            {
                return mVerticalBiasPercent;
            }
            set
            {
                mVerticalBiasPercent = value;
            }
        }

        /// <summary>
        /// Return the percentage bias that is used when two opposite connections exist of the same
        /// strength in a particular orientation.
        /// </summary>
        /// <param name="orientation"> Orientation <seealso cref="#HORIZONTAL"/>/<seealso cref="#VERTICAL"/>. </param>
        /// <returns> Respective percentage bias. </returns>
        public virtual float getBiasPercent(int orientation)
        {
            if (orientation == HORIZONTAL)
            {
                return mHorizontalBiasPercent;
            }
            else if (orientation == VERTICAL)
            {
                return mVerticalBiasPercent;
            }
            else
            {
                return UNKNOWN;
            }
        }

        /// <summary>
        /// Return true if this widget has a baseline
        /// </summary>
        /// <returns> true if the widget has a baseline, false otherwise </returns>
        public virtual bool hasBaseline()
        {
            return hasBaseline_Renamed;
        }

        /// <summary>
        /// Return the baseline distance relative to the top of the widget
        /// </summary>
        /// <returns> baseline </returns>
        public virtual int BaselineDistance
        {
            get
            {
                return mBaselineDistance;
            }
            set
            {
                mBaselineDistance = value;
                hasBaseline_Renamed = value > 0;
            }
        }

        /// <summary>
        /// Return the companion widget. Typically, this would be the real
        /// widget we represent with this instance of ConstraintWidget.
        /// </summary>
        /// <returns> the companion widget, if set. </returns>
        public virtual object CompanionWidget
        {
            get
            {
                return mCompanionWidget;
            }
            set
            {
                mCompanionWidget = value;
            }
        }

        /// <summary>
        /// Return the array of anchors of this widget
        /// </summary>
        /// <returns> array of anchors </returns>
        public virtual List<ConstraintAnchor> Anchors
        {
            get
            {
                return mAnchors;
            }
        }



        /// <summary>
        /// Set both the origin in (x, y) of the widget, relative to its container
        /// </summary>
        /// <param name="x"> x position </param>
        /// <param name="y"> y position </param>
        public virtual void setOrigin(int x, int y)
        {
            mX = x;
            mY = y;
        }

        /// <summary>
        /// Set the offset of this widget relative to the root widget
        /// </summary>
        /// <param name="x"> horizontal offset </param>
        /// <param name="y"> vertical offset </param>
        public virtual void setOffset(int x, int y)
        {
            mOffsetX = x;
            mOffsetY = y;
        }

        /// <summary>
        /// Set the margin to be used when connected to a widget with a visibility of GONE
        /// </summary>
        /// <param name="type">       the anchor to set the margin on </param>
        /// <param name="goneMargin"> the margin value to use </param>
        public virtual void setGoneMargin(ConstraintAnchor.Type type, int goneMargin)
        {
            switch (type)
            {
                case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.LEFT:
                {
                    mLeft.mGoneMargin = goneMargin;
                }
                break;
                case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.TOP:
                {
                    mTop.mGoneMargin = goneMargin;
                }
                break;
                case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.RIGHT:
                {
                    mRight.mGoneMargin = goneMargin;
                }
                break;
                case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.BOTTOM:
                {
                    mBottom.mGoneMargin = goneMargin;
                }
                break;
                case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.BASELINE:
                {
                    mBaseline.mGoneMargin = goneMargin;
                }
                break;
                case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.CENTER:
                case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.CENTER_X:
                case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.CENTER_Y:
                case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.NONE:
                    break;
            }
        }



        /// <summary>
        /// Set the dimension of a widget in a particular orientation.
        /// </summary>
        /// <param name="length">      Size of the dimension. </param>
        /// <param name="orientation"> HORIZONTAL or VERTICAL </param>
        public virtual void setLength(int length, int orientation)
        {
            if (orientation == HORIZONTAL)
            {
                Width = length;
            }
            else if (orientation == VERTICAL)
            {
                Height = length;
            }
        }

        /// <summary>
        /// Set the horizontal style when MATCH_CONSTRAINT is set
        /// </summary>
        /// <param name="horizontalMatchStyle"> MATCH_CONSTRAINT_SPREAD or MATCH_CONSTRAINT_WRAP </param>
        /// <param name="min">                  minimum value </param>
        /// <param name="max">                  maximum value </param>
        /// <param name="percent">              Percent width </param>
        public virtual void setHorizontalMatchStyle(int horizontalMatchStyle, int min, int max, float percent)
        {
            mMatchConstraintDefaultWidth = horizontalMatchStyle;
            mMatchConstraintMinWidth = min;
            mMatchConstraintMaxWidth = (max == int.MaxValue) ? 0 : max;
            mMatchConstraintPercentWidth = percent;
            if (percent > 0 && percent < 1 && mMatchConstraintDefaultWidth == MATCH_CONSTRAINT_SPREAD)
            {
                mMatchConstraintDefaultWidth = MATCH_CONSTRAINT_PERCENT;
            }
        }

        /// <summary>
        /// Set the vertical style when MATCH_CONSTRAINT is set
        /// </summary>
        /// <param name="verticalMatchStyle"> MATCH_CONSTRAINT_SPREAD or MATCH_CONSTRAINT_WRAP </param>
        /// <param name="min">                minimum value </param>
        /// <param name="max">                maximum value </param>
        /// <param name="percent">            Percent height </param>
        public virtual void setVerticalMatchStyle(int verticalMatchStyle, int min, int max, float percent)
        {
            mMatchConstraintDefaultHeight = verticalMatchStyle;
            mMatchConstraintMinHeight = min;
            mMatchConstraintMaxHeight = (max == int.MaxValue) ? 0 : max;
            mMatchConstraintPercentHeight = percent;
            if (percent > 0 && percent < 1 && mMatchConstraintDefaultHeight == MATCH_CONSTRAINT_SPREAD)
            {
                mMatchConstraintDefaultHeight = MATCH_CONSTRAINT_PERCENT;
            }
        }

        /// <summary>
        /// Set the ratio of the widget
        /// </summary>
        /// <param name="ratio"> given string of format [H|V],[float|x:y] or [float|x:y] </param>
        public virtual void setDimensionRatio(string ratio)
        {
            if (string.ReferenceEquals(ratio, null) || ratio.Length == 0)
            {
                mDimensionRatio = 0;
                return;
            }
            int dimensionRatioSide = UNKNOWN;
            float dimensionRatio = 0;
            int len = ratio.Length;
            int commaIndex = ratio.IndexOf(',');
            if (commaIndex > 0 && commaIndex < len - 1)
            {
                string dimension = ratio.Substring(0, commaIndex);
                if (dimension.Equals("W", StringComparison.CurrentCultureIgnoreCase))
                {
                    dimensionRatioSide = HORIZONTAL;
                }
                else if (dimension.Equals("H", StringComparison.CurrentCultureIgnoreCase))
                {
                    dimensionRatioSide = VERTICAL;
                }
                commaIndex++;
            }
            else
            {
                commaIndex = 0;
            }
            int colonIndex = ratio.IndexOf(':');

            if (colonIndex >= 0 && colonIndex < len - 1)
            {
                string nominator = ratio.Substring(commaIndex, colonIndex - commaIndex);
                string denominator = ratio.Substring(colonIndex + 1);
                if (nominator.Length > 0 && denominator.Length > 0)
                {
                    try
                    {
                        float nominatorValue = float.Parse(nominator);
                        float denominatorValue = float.Parse(denominator);
                        if (nominatorValue > 0 && denominatorValue > 0)
                        {
                            if (dimensionRatioSide == VERTICAL)
                            {
                                dimensionRatio = Math.Abs(denominatorValue / nominatorValue);
                            }
                            else
                            {
                                dimensionRatio = Math.Abs(nominatorValue / denominatorValue);
                            }
                        }
                    }
                    catch (System.FormatException)
                    {
                        // Ignore
                    }
                }
            }
            else
            {
                string r = ratio.Substring(commaIndex);
                if (r.Length > 0)
                {
                    try
                    {
                        dimensionRatio = float.Parse(r);
                    }
                    catch (System.FormatException)
                    {
                        // Ignore
                    }
                }
            }

            if (dimensionRatio > 0)
            {
                mDimensionRatio = dimensionRatio;
                mDimensionRatioSide = dimensionRatioSide;
            }
        }

        /// <summary>
        /// Set the ratio of the widget
        /// The ratio will be applied if at least one of the dimension (width or height) is set to a behaviour
        /// of DimensionBehaviour.MATCH_CONSTRAINT -- the dimension's value will be set to the other dimension * ratio.
        /// </summary>
        /// <param name="ratio"> A float value that describes W/H or H/W depending on the provided dimensionRatioSide </param>
        /// <param name="dimensionRatioSide"> The side the ratio should be calculated on, HORIZONTAL, VERTICAL, or UNKNOWN </param>
        public virtual void setDimensionRatio(float ratio, int dimensionRatioSide)
        {
            mDimensionRatio = ratio;
            mDimensionRatioSide = dimensionRatioSide;
        }

        /// <summary>
        /// Return the current ratio of this widget
        /// </summary>
        /// <returns> the dimension ratio (HORIZONTAL, VERTICAL, or UNKNOWN) </returns>
        public virtual float getDimensionRatio()
        {
            return mDimensionRatio;
        }

        /// <summary>
        /// Return the current side on which ratio will be applied
        /// </summary>
        /// <returns> HORIZONTAL, VERTICAL, or UNKNOWN </returns>
        public virtual int DimensionRatioSide
        {
            get
            {
                return mDimensionRatioSide;
            }
        }





        /// <summary>
        /// Set both width and height of the widget
        /// </summary>
        /// <param name="w"> width </param>
        /// <param name="h"> height </param>
        public virtual void setDimension(int w, int h)
        {
            mWidth = w;
            if (mWidth < mMinWidth)
            {
                mWidth = mMinWidth;
            }
            mHeight = h;
            if (mHeight < mMinHeight)
            {
                mHeight = mMinHeight;
            }
        }

        /// <summary>
        /// Set the position+dimension of the widget given left/top/right/bottom
        /// </summary>
        /// <param name="left">   left side position of the widget </param>
        /// <param name="top">    top side position of the widget </param>
        /// <param name="right">  right side position of the widget </param>
        /// <param name="bottom"> bottom side position of the widget </param>
        public virtual void setFrame(int left, int top, int right, int bottom)
        {
            int w = right - left;
            int h = bottom - top;

            mX = left;
            mY = top;

            if (mVisibility == ConstraintWidget.GONE)
            {
                mWidth = 0;
                mHeight = 0;
                return;
            }

            // correct dimensional instability caused by rounding errors
            if (mListDimensionBehaviors[DIMENSION_HORIZONTAL] == DimensionBehaviour.FIXED && w < mWidth)
            {
                w = mWidth;
            }
            if (mListDimensionBehaviors[DIMENSION_VERTICAL] == DimensionBehaviour.FIXED && h < mHeight)
            {
                h = mHeight;
            }

            mWidth = w;
            mHeight = h;

            if (mHeight < mMinHeight)
            {
                mHeight = mMinHeight;
            }
            if (mWidth < mMinWidth)
            {
                mWidth = mMinWidth;
            }
            if (mMatchConstraintMaxWidth > 0 && mListDimensionBehaviors[HORIZONTAL] == MATCH_CONSTRAINT)
            {
                mWidth = Math.Min(mWidth, mMatchConstraintMaxWidth);
            }
            if (mMatchConstraintMaxHeight > 0 && mListDimensionBehaviors[VERTICAL] == MATCH_CONSTRAINT)
            {
                mHeight = Math.Min(mHeight, mMatchConstraintMaxHeight);
            }
            if (w != mWidth)
            {
                mWidthOverride = mWidth;
            }
            if (h != mHeight)
            {
                mHeightOverride = mHeight;
            }

            if (LinearSystem.FULL_DEBUG)
            {
                Console.WriteLine("update from solver " + mDebugName + " " + mX + ":" + mY + " - " + mWidth + " x " + mHeight);
            }
        }

        /// <summary>
        /// Set the position+dimension of the widget based on starting/ending positions on one dimension.
        /// </summary>
        /// <param name="start">       Left/Top side position of the widget. </param>
        /// <param name="end">         Right/Bottom side position of the widget. </param>
        /// <param name="orientation"> Orientation being set (HORIZONTAL/VERTICAL). </param>
        public virtual void setFrame(int start, int end, int orientation)
        {
            if (orientation == HORIZONTAL)
            {
                setHorizontalDimension(start, end);
            }
            else if (orientation == VERTICAL)
            {
                setVerticalDimension(start, end);
            }
        }

        /// <summary>
        /// Set the positions for the horizontal dimension only
        /// </summary>
        /// <param name="left">   left side position of the widget </param>
        /// <param name="right">  right side position of the widget </param>
        public virtual void setHorizontalDimension(int left, int right)
        {
            mX = left;
            mWidth = right - left;
            if (mWidth < mMinWidth)
            {
                mWidth = mMinWidth;
            }
        }

        /// <summary>
        /// Set the positions for the vertical dimension only
        /// </summary>
        /// <param name="top">    top side position of the widget </param>
        /// <param name="bottom"> bottom side position of the widget </param>
        public virtual void setVerticalDimension(int top, int bottom)
        {
            mY = top;
            mHeight = bottom - top;
            if (mHeight < mMinHeight)
            {
                mHeight = mMinHeight;
            }
        }

        /// <summary>
        /// Get the left/top position of the widget relative to the outer side of the container (right/bottom).
        /// </summary>
        /// <param name="orientation"> Orientation by which to find the relative positioning of the widget. </param>
        /// <returns> The relative position of the widget. </returns>
        internal virtual int getRelativePositioning(int orientation)
        {
            if (orientation == HORIZONTAL)
            {
                return mRelX;
            }
            else if (orientation == VERTICAL)
            {
                return mRelY;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Set the left/top position of the widget relative to the outer side of the container (right/bottom).
        /// </summary>
        /// <param name="offset">      Offset of the relative position. </param>
        /// <param name="orientation"> Orientation of the offset being set. </param>
        internal virtual void setRelativePositioning(int offset, int orientation)
        {
            if (orientation == HORIZONTAL)
            {
                mRelX = offset;
            }
            else if (orientation == VERTICAL)
            {
                mRelY = offset;
            }
        }



        /// <summary>
        /// Set the skip value for this widget. This can be used when a widget is in a container,
        /// so that container can position the widget as if it was positioned further in the list
        /// of widgets. For example, with Table, this is used to skip empty cells
        /// (the widget after an empty cell will have a skip value of one)
        /// </summary>
        /// <param name="skip"> </param>
        public virtual int ContainerItemSkip
        {
            set
            {
                if (value >= 0)
                {
                    mContainerItemSkip = value;
                }
                else
                {
                    mContainerItemSkip = 0;
                }
            }
            get
            {
                return mContainerItemSkip;
            }
        }


        /// <summary>
        /// Set the horizontal weight (only used in chains)
        /// </summary>
        /// <param name="horizontalWeight"> Floating point value weight </param>
        public virtual float HorizontalWeight
        {
            set
            {
                mWeight[DIMENSION_HORIZONTAL] = value;
            }
        }

        /// <summary>
        /// Set the vertical weight (only used in chains)
        /// </summary>
        /// <param name="verticalWeight"> Floating point value weight </param>
        public virtual float VerticalWeight
        {
            set
            {
                mWeight[DIMENSION_VERTICAL] = value;
            }
        }

        /// <summary>
        /// Set the chain starting from this widget to be packed.
        /// The horizontal bias will control how elements of the chain are positioned.
        /// </summary>
        /// <param name="horizontalChainStyle"> (CHAIN_SPREAD, CHAIN_SPREAD_INSIDE, CHAIN_PACKED) </param>
        public virtual int HorizontalChainStyle
        {
            set
            {
                mHorizontalChainStyle = value;
            }
            get
            {
                return mHorizontalChainStyle;
            }
        }


        /// <summary>
        /// Set the chain starting from this widget to be packed.
        /// The vertical bias will control how elements of the chain are positioned.
        /// </summary>
        /// <param name="verticalChainStyle"> (CHAIN_SPREAD, CHAIN_SPREAD_INSIDE, CHAIN_PACKED) </param>
        public virtual int VerticalChainStyle
        {
            set
            {
                mVerticalChainStyle = value;
            }
            get
            {
                return mVerticalChainStyle;
            }
        }


        /// <summary>
        /// Returns true if this widget should be used in a barrier
        /// </summary>
        public virtual bool allowedInBarrier()
        {
            return mVisibility != GONE;
        }

        /*-----------------------------------------------------------------------*/
        // Connections
        /*-----------------------------------------------------------------------*/

        /// <summary>
        /// Immediate connection to an anchor without any checks.
        /// </summary>
        /// <param name="startType">  The type of anchor on this widget </param>
        /// <param name="target">     The target widget </param>
        /// <param name="endType">    The type of anchor on the target widget </param>
        /// <param name="margin">     How much margin we want to keep as a minimum distance between the two anchors </param>
        /// <param name="goneMargin"> How much margin we want to keep if the target is set to {@code View.GONE} </param>
        public virtual void immediateConnect(ConstraintAnchor.Type startType, ConstraintWidget target, ConstraintAnchor.Type endType, int margin, int goneMargin)
        {
            ConstraintAnchor startAnchor = getAnchor(startType);
            ConstraintAnchor endAnchor = target.getAnchor(endType);
            startAnchor.connect(endAnchor, margin, goneMargin, true);
        }

        /// <summary>
        /// Connect the given anchors together (the from anchor should be owned by this widget)
        /// </summary>
        /// <param name="from">    the anchor we are connecting from (of this widget) </param>
        /// <param name="to">      the anchor we are connecting to </param>
        /// <param name="margin">  how much margin we want to have </param>
        public virtual void connect(ConstraintAnchor from, ConstraintAnchor to, int margin)
        {
            if (from.Owner == this)
            {
                connect(from.getType(), to.Owner, to.getType(), margin);
            }
        }

        /// <summary>
        /// Connect a given anchor of this widget to another anchor of a target widget
        /// </summary>
        /// <param name="constraintFrom"> which anchor of this widget to connect from </param>
        /// <param name="target">         the target widget </param>
        /// <param name="constraintTo">   the target anchor on the target widget </param>
        public virtual void connect(ConstraintAnchor.Type constraintFrom, ConstraintWidget target, ConstraintAnchor.Type constraintTo)
        {
            if (DEBUG)
            {
                Console.WriteLine(this.DebugName + " connect " + constraintFrom + " to " + target + " " + constraintTo);
            }
            connect(constraintFrom, target, constraintTo, 0);
        }

        /// <summary>
        /// Connect a given anchor of this widget to another anchor of a target widget
        /// </summary>
        /// <param name="constraintFrom"> which anchor of this widget to connect from </param>
        /// <param name="target">         the target widget </param>
        /// <param name="constraintTo">   the target anchor on the target widget </param>
        /// <param name="margin">         how much margin we want to keep as a minimum distance between the two anchors </param>
        public virtual void connect(ConstraintAnchor.Type constraintFrom, ConstraintWidget target, ConstraintAnchor.Type constraintTo, int margin)
        {
            if (constraintFrom == ConstraintAnchor.Type.CENTER)
            {
                // If we have center, we connect instead to the corresponding
                // left/right or top/bottom pairs
                if (constraintTo == ConstraintAnchor.Type.CENTER)
                {
                    ConstraintAnchor left = getAnchor(ConstraintAnchor.Type.LEFT);
                    ConstraintAnchor right = getAnchor(ConstraintAnchor.Type.RIGHT);
                    ConstraintAnchor top = getAnchor(ConstraintAnchor.Type.TOP);
                    ConstraintAnchor bottom = getAnchor(ConstraintAnchor.Type.BOTTOM);
                    bool centerX = false;
                    bool centerY = false;
                    if ((left != null && left.Connected) || (right != null && right.Connected))
                    {
                        // don't apply center here
                    }
                    else
                    {
                        connect(ConstraintAnchor.Type.LEFT, target, ConstraintAnchor.Type.LEFT, 0);
                        connect(ConstraintAnchor.Type.RIGHT, target, ConstraintAnchor.Type.RIGHT, 0);
                        centerX = true;
                    }
                    if ((top != null && top.Connected) || (bottom != null && bottom.Connected))
                    {
                        // don't apply center here
                    }
                    else
                    {
                        connect(ConstraintAnchor.Type.TOP, target, ConstraintAnchor.Type.TOP, 0);
                        connect(ConstraintAnchor.Type.BOTTOM, target, ConstraintAnchor.Type.BOTTOM, 0);
                        centerY = true;
                    }
                    if (centerX && centerY)
                    {
                        ConstraintAnchor center = getAnchor(ConstraintAnchor.Type.CENTER);
                        center.connect(target.getAnchor(ConstraintAnchor.Type.CENTER), 0);
                    }
                    else if (centerX)
                    {
                        ConstraintAnchor center = getAnchor(ConstraintAnchor.Type.CENTER_X);
                        center.connect(target.getAnchor(ConstraintAnchor.Type.CENTER_X), 0);
                    }
                    else if (centerY)
                    {
                        ConstraintAnchor center = getAnchor(ConstraintAnchor.Type.CENTER_Y);
                        center.connect(target.getAnchor(ConstraintAnchor.Type.CENTER_Y), 0);
                    }
                }
                else if ((constraintTo == ConstraintAnchor.Type.LEFT) || (constraintTo == ConstraintAnchor.Type.RIGHT))
                {
                    connect(ConstraintAnchor.Type.LEFT, target, constraintTo, 0);
                    connect(ConstraintAnchor.Type.RIGHT, target, constraintTo, 0);
                    ConstraintAnchor center = getAnchor(ConstraintAnchor.Type.CENTER);
                    center.connect(target.getAnchor(constraintTo), 0);
                }
                else if ((constraintTo == ConstraintAnchor.Type.TOP) || (constraintTo == ConstraintAnchor.Type.BOTTOM))
                {
                    connect(ConstraintAnchor.Type.TOP, target, constraintTo, 0);
                    connect(ConstraintAnchor.Type.BOTTOM, target, constraintTo, 0);
                    ConstraintAnchor center = getAnchor(ConstraintAnchor.Type.CENTER);
                    center.connect(target.getAnchor(constraintTo), 0);
                }
            }
            else if (constraintFrom == ConstraintAnchor.Type.CENTER_X && (constraintTo == ConstraintAnchor.Type.LEFT || constraintTo == ConstraintAnchor.Type.RIGHT))
            {
                ConstraintAnchor left = getAnchor(ConstraintAnchor.Type.LEFT);
                ConstraintAnchor targetAnchor = target.getAnchor(constraintTo);
                ConstraintAnchor right = getAnchor(ConstraintAnchor.Type.RIGHT);
                left.connect(targetAnchor, 0);
                right.connect(targetAnchor, 0);
                ConstraintAnchor centerX = getAnchor(ConstraintAnchor.Type.CENTER_X);
                centerX.connect(targetAnchor, 0);
            }
            else if (constraintFrom == ConstraintAnchor.Type.CENTER_Y && (constraintTo == ConstraintAnchor.Type.TOP || constraintTo == ConstraintAnchor.Type.BOTTOM))
            {
                ConstraintAnchor targetAnchor = target.getAnchor(constraintTo);
                ConstraintAnchor top = getAnchor(ConstraintAnchor.Type.TOP);
                top.connect(targetAnchor, 0);
                ConstraintAnchor bottom = getAnchor(ConstraintAnchor.Type.BOTTOM);
                bottom.connect(targetAnchor, 0);
                ConstraintAnchor centerY = getAnchor(ConstraintAnchor.Type.CENTER_Y);
                centerY.connect(targetAnchor, 0);
            }
            else if (constraintFrom == ConstraintAnchor.Type.CENTER_X && constraintTo == ConstraintAnchor.Type.CENTER_X)
            {
                // Center X connection will connect left & right
                ConstraintAnchor left = getAnchor(ConstraintAnchor.Type.LEFT);
                ConstraintAnchor leftTarget = target.getAnchor(ConstraintAnchor.Type.LEFT);
                left.connect(leftTarget, 0);
                ConstraintAnchor right = getAnchor(ConstraintAnchor.Type.RIGHT);
                ConstraintAnchor rightTarget = target.getAnchor(ConstraintAnchor.Type.RIGHT);
                right.connect(rightTarget, 0);
                ConstraintAnchor centerX = getAnchor(ConstraintAnchor.Type.CENTER_X);
                centerX.connect(target.getAnchor(constraintTo), 0);
            }
            else if (constraintFrom == ConstraintAnchor.Type.CENTER_Y && constraintTo == ConstraintAnchor.Type.CENTER_Y)
            {
                // Center Y connection will connect top & bottom.
                ConstraintAnchor top = getAnchor(ConstraintAnchor.Type.TOP);
                ConstraintAnchor topTarget = target.getAnchor(ConstraintAnchor.Type.TOP);
                top.connect(topTarget, 0);
                ConstraintAnchor bottom = getAnchor(ConstraintAnchor.Type.BOTTOM);
                ConstraintAnchor bottomTarget = target.getAnchor(ConstraintAnchor.Type.BOTTOM);
                bottom.connect(bottomTarget, 0);
                ConstraintAnchor centerY = getAnchor(ConstraintAnchor.Type.CENTER_Y);
                centerY.connect(target.getAnchor(constraintTo), 0);
            }
            else
            {
                ConstraintAnchor fromAnchor = getAnchor(constraintFrom);
                ConstraintAnchor toAnchor = target.getAnchor(constraintTo);
                if (fromAnchor.isValidConnection(toAnchor))
                {
                    // make sure that the baseline takes precedence over top/bottom
                    // and reversely, reset the baseline if we are connecting top/bottom
                    if (constraintFrom == ConstraintAnchor.Type.BASELINE)
                    {
                        ConstraintAnchor top = getAnchor(ConstraintAnchor.Type.TOP);
                        ConstraintAnchor bottom = getAnchor(ConstraintAnchor.Type.BOTTOM);
                        if (top != null)
                        {
                            top.reset();
                        }
                        if (bottom != null)
                        {
                            bottom.reset();
                        }
                    }
                    else if ((constraintFrom == ConstraintAnchor.Type.TOP) || (constraintFrom == ConstraintAnchor.Type.BOTTOM))
                    {
                        ConstraintAnchor baseline = getAnchor(ConstraintAnchor.Type.BASELINE);
                        if (baseline != null)
                        {
                            baseline.reset();
                        }
                        ConstraintAnchor center = getAnchor(ConstraintAnchor.Type.CENTER);
                        if (center.Target != toAnchor)
                        {
                            center.reset();
                        }
                        ConstraintAnchor opposite = getAnchor(constraintFrom).Opposite;
                        ConstraintAnchor centerY = getAnchor(ConstraintAnchor.Type.CENTER_Y);
                        if (centerY.Connected)
                        {
                            opposite.reset();
                            centerY.reset();
                        }
                        else
                        {
                            if (AUTOTAG_CENTER)
                            {
                                // let's see if we need to mark center_y as connected
                                if (opposite.Connected && opposite.Target.Owner == toAnchor.Owner)
                                {
                                    ConstraintAnchor targetCenterY = toAnchor.Owner.getAnchor(ConstraintAnchor.Type.CENTER_Y);
                                    centerY.connect(targetCenterY, 0);
                                }
                            }
                        }
                    }
                    else if ((constraintFrom == ConstraintAnchor.Type.LEFT) || (constraintFrom == ConstraintAnchor.Type.RIGHT))
                    {
                        ConstraintAnchor center = getAnchor(ConstraintAnchor.Type.CENTER);
                        if (center.Target != toAnchor)
                        {
                            center.reset();
                        }
                        ConstraintAnchor opposite = getAnchor(constraintFrom).Opposite;
                        ConstraintAnchor centerX = getAnchor(ConstraintAnchor.Type.CENTER_X);
                        if (centerX.Connected)
                        {
                            opposite.reset();
                            centerX.reset();
                        }
                        else
                        {
                            if (AUTOTAG_CENTER)
                            {
                                // let's see if we need to mark center_x as connected
                                if (opposite.Connected && opposite.Target.Owner == toAnchor.Owner)
                                {
                                    ConstraintAnchor targetCenterX = toAnchor.Owner.getAnchor(ConstraintAnchor.Type.CENTER_X);
                                    centerX.connect(targetCenterX, 0);
                                }
                            }
                        }

                    }
                    fromAnchor.connect(toAnchor, margin);
                }
            }
        }

        /// <summary>
        /// Reset all the constraints set on this widget
        /// </summary>
        public virtual void resetAllConstraints()
        {
            resetAnchors();
            VerticalBiasPercent = DEFAULT_BIAS;
            HorizontalBiasPercent = DEFAULT_BIAS;
        }

        /// <summary>
        /// Reset the given anchor
        /// </summary>
        /// <param name="anchor"> the anchor we want to reset </param>
        public virtual void resetAnchor(ConstraintAnchor anchor)
        {
            if (Parent != null)
            {
                if (Parent is ConstraintWidgetContainer)
                {
                    ConstraintWidgetContainer parent = (ConstraintWidgetContainer) Parent;
                    if (parent.handlesInternalConstraints())
                    {
                        return;
                    }
                }
            }
            ConstraintAnchor left = getAnchor(ConstraintAnchor.Type.LEFT);
            ConstraintAnchor right = getAnchor(ConstraintAnchor.Type.RIGHT);
            ConstraintAnchor top = getAnchor(ConstraintAnchor.Type.TOP);
            ConstraintAnchor bottom = getAnchor(ConstraintAnchor.Type.BOTTOM);
            ConstraintAnchor center = getAnchor(ConstraintAnchor.Type.CENTER);
            ConstraintAnchor centerX = getAnchor(ConstraintAnchor.Type.CENTER_X);
            ConstraintAnchor centerY = getAnchor(ConstraintAnchor.Type.CENTER_Y);

            if (anchor == center)
            {
                if (left.Connected && right.Connected && left.Target == right.Target)
                {
                    left.reset();
                    right.reset();
                }
                if (top.Connected && bottom.Connected && top.Target == bottom.Target)
                {
                    top.reset();
                    bottom.reset();
                }
                mHorizontalBiasPercent = 0.5f;
                mVerticalBiasPercent = 0.5f;
            }
            else if (anchor == centerX)
            {
                if (left.Connected && right.Connected && left.Target.Owner == right.Target.Owner)
                {
                    left.reset();
                    right.reset();
                }
                mHorizontalBiasPercent = 0.5f;
            }
            else if (anchor == centerY)
            {
                if (top.Connected && bottom.Connected && top.Target.Owner == bottom.Target.Owner)
                {
                    top.reset();
                    bottom.reset();
                }
                mVerticalBiasPercent = 0.5f;
            }
            else if (anchor == left || anchor == right)
            {
                if (left.Connected && left.Target == right.Target)
                {
                    center.reset();
                }
            }
            else if (anchor == top || anchor == bottom)
            {
                if (top.Connected && top.Target == bottom.Target)
                {
                    center.reset();
                }
            }
            anchor.reset();
        }

        /// <summary>
        /// Reset all connections
        /// </summary>
        public virtual void resetAnchors()
        {
            ConstraintWidget parent = Parent;
            if (parent != null && parent is ConstraintWidgetContainer)
            {
                ConstraintWidgetContainer parentContainer = (ConstraintWidgetContainer) Parent;
                if (parentContainer.handlesInternalConstraints())
                {
                    return;
                }
            }
            for (int i = 0, mAnchorsSize = mAnchors.Count; i < mAnchorsSize; i++)
            {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ConstraintAnchor anchor = mAnchors.get(i);
                ConstraintAnchor anchor = mAnchors[i];
                anchor.reset();
            }
        }

        /// <summary>
        /// Given a type of anchor, returns the corresponding anchor.
        /// </summary>
        /// <param name="anchorType"> type of the anchor (LEFT, TOP, RIGHT, BOTTOM, BASELINE, CENTER_X, CENTER_Y) </param>
        /// <returns> the matching anchor </returns>
        public virtual ConstraintAnchor getAnchor(ConstraintAnchor.Type anchorType)
        {
            switch (anchorType)
            {
                case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.LEFT:
                {
                    return mLeft;
                }
                case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.TOP:
                {
                    return mTop;
                }
                case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.RIGHT:
                {
                    return mRight;
                }
                case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.BOTTOM:
                {
                    return mBottom;
                }
                case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.BASELINE:
                {
                    return mBaseline;
                }
                case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.CENTER_X:
                {
                    return mCenterX;
                }
                case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.CENTER_Y:
                {
                    return mCenterY;
                }
                case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.CENTER:
                {
                    return mCenter;
                }
                case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.NONE:
                    return null;
            }
            throw new AssertionError(anchorType.ToString());
        }

        /// <summary>
        /// Accessor for the horizontal dimension behaviour
        /// </summary>
        /// <returns> dimension behaviour </returns>
        public virtual DimensionBehaviour HorizontalDimensionBehaviour
        {
            get
            {
                return mListDimensionBehaviors[DIMENSION_HORIZONTAL];
            }
            set
            {
                mListDimensionBehaviors[DIMENSION_HORIZONTAL] = value;
            }
        }

        /// <summary>
        /// Accessor for the vertical dimension behaviour
        /// </summary>
        /// <returns> dimension behaviour </returns>
        public virtual DimensionBehaviour VerticalDimensionBehaviour
        {
            get
            {
                return mListDimensionBehaviors[DIMENSION_VERTICAL];
            }
            set
            {
                mListDimensionBehaviors[DIMENSION_VERTICAL] = value;
            }
        }

        /// <summary>
        /// Get the widget's <seealso cref="DimensionBehaviour"/> in an specific orientation.
        /// </summary>
        /// <param name="orientation"> </param>
        /// <returns> The <seealso cref="DimensionBehaviour"/> of the widget. </returns>
        public virtual DimensionBehaviour? getDimensionBehaviour(int orientation)
        {
            if (orientation == HORIZONTAL)
            {
                return HorizontalDimensionBehaviour;
            }
            else if (orientation == VERTICAL)
            {
                return VerticalDimensionBehaviour;
            }
            else
            {
                return null;
            }
        }



        /// <summary>
        /// Test if you are in a Horizontal chain
        /// </summary>
        /// <returns> true if in a horizontal chain </returns>
        public virtual bool InHorizontalChain
        {
            get
            {
                if ((mLeft.mTarget != null && mLeft.mTarget.mTarget == mLeft) || (mRight.mTarget != null && mRight.mTarget.mTarget == mRight))
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Return the previous chain member if one exists
        /// </summary>
        /// <param name="orientation"> HORIZONTAL or VERTICAL </param>
        /// <returns> the previous chain member or null if we are the first chain element </returns>
        public virtual ConstraintWidget getPreviousChainMember(int orientation)
        {
            if (orientation == HORIZONTAL)
            {
                if (mLeft.mTarget != null && mLeft.mTarget.mTarget == mLeft)
                {
                    return mLeft.mTarget.mOwner;
                }
            }
            else if (orientation == VERTICAL)
            {
                if (mTop.mTarget != null && mTop.mTarget.mTarget == mTop)
                {
                    return mTop.mTarget.mOwner;
                }
            }
            return null;
        }

        /// <summary>
        /// Return the next chain member if one exists
        /// </summary>
        /// <param name="orientation"> HORIZONTAL or VERTICAL </param>
        /// <returns> the next chain member or null if we are the last chain element </returns>
        public virtual ConstraintWidget getNextChainMember(int orientation)
        {
            if (orientation == HORIZONTAL)
            {
                if (mRight.mTarget != null && mRight.mTarget.mTarget == mRight)
                {
                    return mRight.mTarget.mOwner;
                }
            }
            else if (orientation == VERTICAL)
            {
                if (mBottom.mTarget != null && mBottom.mTarget.mTarget == mBottom)
                {
                    return mBottom.mTarget.mOwner;
                }
            }
            return null;
        }

        /// <summary>
        /// if in a horizontal chain return the left most widget in the chain.
        /// </summary>
        /// <returns> left most widget in chain or null </returns>
        public virtual ConstraintWidget HorizontalChainControlWidget
        {
            get
            {
                ConstraintWidget found = null;
                if (InHorizontalChain)
                {
                    ConstraintWidget tmp = this;
    
                    while (found == null && tmp != null)
                    {
                        ConstraintAnchor anchor = tmp.getAnchor(ConstraintAnchor.Type.LEFT);
                        ConstraintAnchor targetOwner = (anchor == null) ? null : anchor.Target;
                        ConstraintWidget target = (targetOwner == null) ? null : targetOwner.Owner;
                        if (target == Parent)
                        {
                            found = tmp;
                            break;
                        }
                        ConstraintAnchor targetAnchor = (target == null) ? null : target.getAnchor(ConstraintAnchor.Type.RIGHT).Target;
                        if (targetAnchor != null && targetAnchor.Owner != tmp)
                        {
                            found = tmp;
                        }
                        else
                        {
                            tmp = target;
                        }
                    }
                }
                return found;
            }
        }


        /// <summary>
        /// Test if you are in a vertical chain
        /// </summary>
        /// <returns> true if in a vertical chain </returns>
        public virtual bool InVerticalChain
        {
            get
            {
                if ((mTop.mTarget != null && mTop.mTarget.mTarget == mTop) || (mBottom.mTarget != null && mBottom.mTarget.mTarget == mBottom))
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// if in a vertical chain return the top most widget in the chain.
        /// </summary>
        /// <returns> top most widget in chain or null </returns>
        public virtual ConstraintWidget VerticalChainControlWidget
        {
            get
            {
                ConstraintWidget found = null;
                if (InVerticalChain)
                {
                    ConstraintWidget tmp = this;
                    while (found == null && tmp != null)
                    {
                        ConstraintAnchor anchor = tmp.getAnchor(ConstraintAnchor.Type.TOP);
                        ConstraintAnchor targetOwner = (anchor == null) ? null : anchor.Target;
                        ConstraintWidget target = (targetOwner == null) ? null : targetOwner.Owner;
                        if (target == Parent)
                        {
                            found = tmp;
                            break;
                        }
                        ConstraintAnchor targetAnchor = (target == null) ? null : target.getAnchor(ConstraintAnchor.Type.BOTTOM).Target;
                        if (targetAnchor != null && targetAnchor.Owner != tmp)
                        {
                            found = tmp;
                        }
                        else
                        {
                            tmp = target;
                        }
                    }
    
                }
                return found;
            }
        }

        /// <summary>
        /// Determine if the widget is the first element of a chain in a given orientation.
        /// </summary>
        /// <param name="orientation"> Either <seealso cref="#HORIZONTAL"/> or <seealso cref="#VERTICAL"/> </param>
        /// <returns> if the widget is the head of a chain </returns>
        private bool isChainHead(int orientation)
        {
            int offset = orientation * 2;
            return (mListAnchors[offset].mTarget != null && mListAnchors[offset].mTarget.mTarget != mListAnchors[offset]) && (mListAnchors[offset + 1].mTarget != null && mListAnchors[offset + 1].mTarget.mTarget == mListAnchors[offset + 1]);
        }


        /*-----------------------------------------------------------------------*/
        // Constraints
        /*-----------------------------------------------------------------------*/

        /// <summary>
        /// Add this widget to the solver
        /// </summary>
        /// <param name="system"> the solver we want to add the widget to </param>
        /// <param name="optimize"> true if <seealso cref="Optimizer#OPTIMIZATION_GRAPH"/> is on </param>
        public virtual void addToSolver(LinearSystem system, bool optimize)
        {
            if (LinearSystem.FULL_DEBUG)
            {
                Console.WriteLine("\n----------------------------------------------");
                Console.WriteLine("-- adding " + DebugName + " to the solver");
                if (InVirtualLayout)
                {
                    Console.WriteLine("-- note: is in virtual layout");
                }
                Console.WriteLine("----------------------------------------------\n");
            }

            SolverVariable left = system.createObjectVariable(mLeft);
            SolverVariable right = system.createObjectVariable(mRight);
            SolverVariable top = system.createObjectVariable(mTop);
            SolverVariable bottom = system.createObjectVariable(mBottom);
            SolverVariable baseline = system.createObjectVariable(mBaseline);

            bool horizontalParentWrapContent = false;
            bool verticalParentWrapContent = false;
            if (mParent != null)
            {
                horizontalParentWrapContent = mParent != null ? mParent.mListDimensionBehaviors[DIMENSION_HORIZONTAL] == WRAP_CONTENT : false;
                verticalParentWrapContent = mParent != null ? mParent.mListDimensionBehaviors[DIMENSION_VERTICAL] == WRAP_CONTENT : false;

                switch (mWrapBehaviorInParent)
                {
                    case WRAP_BEHAVIOR_SKIPPED:
                    {
                        horizontalParentWrapContent = false;
                        verticalParentWrapContent = false;
                    }
                    break;
                    case WRAP_BEHAVIOR_HORIZONTAL_ONLY:
                    {
                        verticalParentWrapContent = false;
                    }
                    break;
                    case WRAP_BEHAVIOR_VERTICAL_ONLY:
                    {
                        horizontalParentWrapContent = false;
                    }
                    break;
                }
            }

            if (mVisibility == GONE && !hasDependencies() && !mIsInBarrier[HORIZONTAL] && !mIsInBarrier[VERTICAL])
            {
                return;
            }

            if (resolvedHorizontal || resolvedVertical)
            {
                if (LinearSystem.FULL_DEBUG)
                {
                    Console.WriteLine("\n----------------------------------------------");
                    Console.WriteLine("-- setting " + DebugName + " to " + mX + ", " + mY + " " + mWidth + " x " + mHeight);
                    Console.WriteLine("----------------------------------------------\n");
                }
                // For now apply all, but that won't work for wrap/wrap layouts.
                if (resolvedHorizontal)
                {
                    system.addEquality(left, mX);
                    system.addEquality(right, mX + mWidth);
                    if (horizontalParentWrapContent && mParent != null)
                    {
                        if (OPTIMIZE_WRAP_ON_RESOLVED)
                        {
                            ConstraintWidgetContainer container = (ConstraintWidgetContainer) mParent;
                            container.addHorizontalWrapMinVariable(mLeft);
                            container.addHorizontalWrapMaxVariable(mRight);
                        }
                        else
                        {
                            int wrapStrength = SolverVariable.STRENGTH_EQUALITY;
                            system.addGreaterThan(system.createObjectVariable(mParent.mRight), right,0, wrapStrength);
                        }
                    }
                }
                if (resolvedVertical)
                {
                    system.addEquality(top, mY);
                    system.addEquality(bottom, mY + mHeight);
                    if (mBaseline.hasDependents())
                    {
                        system.addEquality(baseline, mY + mBaselineDistance);
                    }
                    if (verticalParentWrapContent && mParent != null)
                    {
                        if (OPTIMIZE_WRAP_ON_RESOLVED)
                        {
                            ConstraintWidgetContainer container = (ConstraintWidgetContainer) mParent;
                            container.addVerticalWrapMinVariable(mTop);
                            container.addVerticalWrapMaxVariable(mBottom);
                        }
                        else
                        {
                            int wrapStrength = SolverVariable.STRENGTH_EQUALITY;
                            system.addGreaterThan(system.createObjectVariable(mParent.mBottom), bottom,0, wrapStrength);
                        }
                    }
                }
                if (resolvedHorizontal && resolvedVertical)
                {
                    resolvedHorizontal = false;
                    resolvedVertical = false;
                    if (LinearSystem.FULL_DEBUG)
                    {
                        Console.WriteLine("\n----------------------------------------------");
                        Console.WriteLine("-- setting COMPLETED for " + DebugName);
                        Console.WriteLine("----------------------------------------------\n");
                    }
                    return;
                }
            }

            if (LinearSystem.sMetrics != null)
            {
                LinearSystem.sMetrics.widgets++;
            }
            if (FULL_DEBUG)
            {
                if (optimize && horizontalRun != null && verticalRun != null)
                {
                    Console.WriteLine("-- horizontal run : " + horizontalRun.start + " : " + horizontalRun.end);
                    Console.WriteLine("-- vertical run : " + verticalRun.start + " : " + verticalRun.end);
                }
            }
            if (optimize && horizontalRun != null && verticalRun != null && horizontalRun.start.resolved && horizontalRun.end.resolved && verticalRun.start.resolved && verticalRun.end.resolved)
            {

                if (LinearSystem.sMetrics != null)
                {
                    LinearSystem.sMetrics.graphSolved++;
                }
                system.addEquality(left, horizontalRun.start.value);
                system.addEquality(right, horizontalRun.end.value);
                system.addEquality(top, verticalRun.start.value);
                system.addEquality(bottom, verticalRun.end.value);
                system.addEquality(baseline, verticalRun.baseline.value);
                if (mParent != null)
                {
                    if (horizontalParentWrapContent && isTerminalWidget[HORIZONTAL] && !InHorizontalChain)
                    {
                        SolverVariable parentMax = system.createObjectVariable(mParent.mRight);
                        system.addGreaterThan(parentMax, right, 0, SolverVariable.STRENGTH_FIXED);
                    }
                    if (verticalParentWrapContent && isTerminalWidget[VERTICAL] && !InVerticalChain)
                    {
                        SolverVariable parentMax = system.createObjectVariable(mParent.mBottom);
                        system.addGreaterThan(parentMax, bottom, 0, SolverVariable.STRENGTH_FIXED);
                    }
                }
                resolvedHorizontal = false;
                resolvedVertical = false;
                return; // we are done here
            }
            if (LinearSystem.sMetrics != null)
            {
                LinearSystem.sMetrics.linearSolved++;
            }

            bool inHorizontalChain = false;
            bool inVerticalChain = false;

            if (mParent != null)
            {
                // Add this widget to a horizontal chain if it is the Head of it.
                if (isChainHead(HORIZONTAL))
                {
                    ((ConstraintWidgetContainer) mParent).addChain(this, HORIZONTAL);
                    inHorizontalChain = true;
                }
                else
                {
                    inHorizontalChain = InHorizontalChain;
                }

                // Add this widget to a vertical chain if it is the Head of it.
                if (isChainHead(VERTICAL))
                {
                    ((ConstraintWidgetContainer) mParent).addChain(this, VERTICAL);
                    inVerticalChain = true;
                }
                else
                {
                    inVerticalChain = InVerticalChain;
                }

                if (!inHorizontalChain && horizontalParentWrapContent && mVisibility != GONE && mLeft.mTarget == null && mRight.mTarget == null)
                {
                    if (FULL_DEBUG)
                    {
                        Console.WriteLine("<>1 ADDING H WRAP GREATER FOR " + DebugName);
                    }
                    SolverVariable parentRight = system.createObjectVariable(mParent.mRight);
                    system.addGreaterThan(parentRight, right, 0, SolverVariable.STRENGTH_LOW);
                }

                if (!inVerticalChain && verticalParentWrapContent && mVisibility != GONE && mTop.mTarget == null && mBottom.mTarget == null && mBaseline == null)
                {
                    if (FULL_DEBUG)
                    {
                        Console.WriteLine("<>1 ADDING V WRAP GREATER FOR " + DebugName);
                    }
                    SolverVariable parentBottom = system.createObjectVariable(mParent.mBottom);
                    system.addGreaterThan(parentBottom, bottom, 0, SolverVariable.STRENGTH_LOW);
                }
            }

            int width = mWidth;
            if (width < mMinWidth)
            {
                width = mMinWidth;
            }
            int height = mHeight;
            if (height < mMinHeight)
            {
                height = mMinHeight;
            }

            // Dimensions can be either fixed (a given value) or dependent on the solver if set to MATCH_CONSTRAINT
            bool horizontalDimensionFixed = mListDimensionBehaviors[DIMENSION_HORIZONTAL] != MATCH_CONSTRAINT;
            bool verticalDimensionFixed = mListDimensionBehaviors[DIMENSION_VERTICAL] != MATCH_CONSTRAINT;

            // We evaluate the dimension ratio here as the connections can change.
            // TODO: have a validation pass after connection instead
            bool useRatio = false;
            mResolvedDimensionRatioSide = mDimensionRatioSide;
            mResolvedDimensionRatio = mDimensionRatio;

            int matchConstraintDefaultWidth = mMatchConstraintDefaultWidth;
            int matchConstraintDefaultHeight = mMatchConstraintDefaultHeight;

            if (mDimensionRatio > 0 && mVisibility != GONE)
            {
                useRatio = true;
                if (mListDimensionBehaviors[DIMENSION_HORIZONTAL] == MATCH_CONSTRAINT && matchConstraintDefaultWidth == MATCH_CONSTRAINT_SPREAD)
                {
                    matchConstraintDefaultWidth = MATCH_CONSTRAINT_RATIO;
                }
                if (mListDimensionBehaviors[DIMENSION_VERTICAL] == MATCH_CONSTRAINT && matchConstraintDefaultHeight == MATCH_CONSTRAINT_SPREAD)
                {
                    matchConstraintDefaultHeight = MATCH_CONSTRAINT_RATIO;
                }

                if (mListDimensionBehaviors[DIMENSION_HORIZONTAL] == MATCH_CONSTRAINT && mListDimensionBehaviors[DIMENSION_VERTICAL] == MATCH_CONSTRAINT && matchConstraintDefaultWidth == MATCH_CONSTRAINT_RATIO && matchConstraintDefaultHeight == MATCH_CONSTRAINT_RATIO)
                {
                    setupDimensionRatio(horizontalParentWrapContent, verticalParentWrapContent, horizontalDimensionFixed, verticalDimensionFixed);
                }
                else if (mListDimensionBehaviors[DIMENSION_HORIZONTAL] == MATCH_CONSTRAINT && matchConstraintDefaultWidth == MATCH_CONSTRAINT_RATIO)
                {
                    mResolvedDimensionRatioSide = HORIZONTAL;
                    width = (int)(mResolvedDimensionRatio * mHeight);
                    if (mListDimensionBehaviors[DIMENSION_VERTICAL] != MATCH_CONSTRAINT)
                    {
                        matchConstraintDefaultWidth = MATCH_CONSTRAINT_RATIO_RESOLVED;
                        useRatio = false;
                    }
                }
                else if (mListDimensionBehaviors[DIMENSION_VERTICAL] == MATCH_CONSTRAINT && matchConstraintDefaultHeight == MATCH_CONSTRAINT_RATIO)
                {
                    mResolvedDimensionRatioSide = VERTICAL;
                    if (mDimensionRatioSide == UNKNOWN)
                    {
                        // need to reverse the ratio as the parsing is done in horizontal mode
                        mResolvedDimensionRatio = 1 / mResolvedDimensionRatio;
                    }
                    height = (int)(mResolvedDimensionRatio * mWidth);
                    if (mListDimensionBehaviors[DIMENSION_HORIZONTAL] != MATCH_CONSTRAINT)
                    {
                        matchConstraintDefaultHeight = MATCH_CONSTRAINT_RATIO_RESOLVED;
                        useRatio = false;
                    }
                }
            }

            mResolvedMatchConstraintDefault[HORIZONTAL] = matchConstraintDefaultWidth;
            mResolvedMatchConstraintDefault[VERTICAL] = matchConstraintDefaultHeight;
            mResolvedHasRatio = useRatio;

            bool useHorizontalRatio = useRatio && (mResolvedDimensionRatioSide == HORIZONTAL || mResolvedDimensionRatioSide == UNKNOWN);

            bool useVerticalRatio = useRatio && (mResolvedDimensionRatioSide == VERTICAL || mResolvedDimensionRatioSide == UNKNOWN);

            // Horizontal resolution
            bool wrapContent = (mListDimensionBehaviors[DIMENSION_HORIZONTAL] == WRAP_CONTENT) && (this is ConstraintWidgetContainer);
            if (wrapContent)
            {
                width = 0;
            }

            bool applyPosition = true;
            if (mCenter.Connected)
            {
                applyPosition = false;
            }

            bool isInHorizontalBarrier = mIsInBarrier[HORIZONTAL];
            bool isInVerticalBarrier = mIsInBarrier[VERTICAL];

            if (mHorizontalResolution != DIRECT && !resolvedHorizontal)
            {
                if (!optimize || !(horizontalRun != null && horizontalRun.start.resolved && horizontalRun.end.resolved))
                {
                    SolverVariable parentMax = mParent != null ? system.createObjectVariable(mParent.mRight) : null;
                    SolverVariable parentMin = mParent != null ? system.createObjectVariable(mParent.mLeft) : null;
                    applyConstraints(system, true, horizontalParentWrapContent, verticalParentWrapContent, isTerminalWidget[HORIZONTAL], parentMin, parentMax, mListDimensionBehaviors[DIMENSION_HORIZONTAL], wrapContent, mLeft, mRight, mX, width, mMinWidth, mMaxDimension[HORIZONTAL], mHorizontalBiasPercent, useHorizontalRatio, mListDimensionBehaviors[VERTICAL] == MATCH_CONSTRAINT, inHorizontalChain, inVerticalChain, isInHorizontalBarrier, matchConstraintDefaultWidth, matchConstraintDefaultHeight, mMatchConstraintMinWidth, mMatchConstraintMaxWidth, mMatchConstraintPercentWidth, applyPosition);
                }
                else if (optimize)
                {
                    system.addEquality(left, horizontalRun.start.value);
                    system.addEquality(right, horizontalRun.end.value);
                    if (mParent != null)
                    {
                        if (horizontalParentWrapContent && isTerminalWidget[HORIZONTAL] && !InHorizontalChain)
                        {
                            if (FULL_DEBUG)
                            {
                                Console.WriteLine("<>2 ADDING H WRAP GREATER FOR " + DebugName);
                            }
                            SolverVariable parentMax = system.createObjectVariable(mParent.mRight);
                            system.addGreaterThan(parentMax, right, 0, SolverVariable.STRENGTH_FIXED);
                        }
                    }
                }
            }

            bool applyVerticalConstraints = true;
            if (optimize && verticalRun != null && verticalRun.start.resolved && verticalRun.end.resolved)
            {
                system.addEquality(top, verticalRun.start.value);
                system.addEquality(bottom, verticalRun.end.value);
                system.addEquality(baseline, verticalRun.baseline.value);
                if (mParent != null)
                {
                    if (!inVerticalChain && verticalParentWrapContent && isTerminalWidget[VERTICAL])
                    {
                        if (FULL_DEBUG)
                        {
                            Console.WriteLine("<>2 ADDING V WRAP GREATER FOR " + DebugName);
                        }
                        SolverVariable parentMax = system.createObjectVariable(mParent.mBottom);
                        system.addGreaterThan(parentMax, bottom, 0, SolverVariable.STRENGTH_FIXED);
                    }
                }
                applyVerticalConstraints = false;
            }
            if (mVerticalResolution == DIRECT)
            {
                if (LinearSystem.FULL_DEBUG)
                {
                    Console.WriteLine("\n----------------------------------------------");
                    Console.WriteLine("-- DONE adding " + DebugName + " to the solver");
                    Console.WriteLine("-- SKIP VERTICAL RESOLUTION");
                    Console.WriteLine("----------------------------------------------\n");
                }
                applyVerticalConstraints = false;
            }
            if (applyVerticalConstraints && !resolvedVertical)
            {
                // Vertical Resolution
                wrapContent = (mListDimensionBehaviors[DIMENSION_VERTICAL] == WRAP_CONTENT) && (this is ConstraintWidgetContainer);
                if (wrapContent)
                {
                    height = 0;
                }

                SolverVariable parentMax = mParent != null ? system.createObjectVariable(mParent.mBottom) : null;
                SolverVariable parentMin = mParent != null ? system.createObjectVariable(mParent.mTop) : null;

                if (mBaselineDistance > 0 || mVisibility == GONE)
                {
                    // if we are GONE we might still have to deal with baseline, even if our baseline distance would be zero
                    if (mBaseline.mTarget != null)
                    {
                        system.addEquality(baseline, top, BaselineDistance, SolverVariable.STRENGTH_FIXED);
                        SolverVariable baselineTarget = system.createObjectVariable(mBaseline.mTarget);
                        int baselineMargin = mBaseline.Margin;
                        system.addEquality(baseline, baselineTarget, baselineMargin, SolverVariable.STRENGTH_FIXED);
                        applyPosition = false;
                        if (verticalParentWrapContent)
                        {
                            if (FULL_DEBUG)
                            {
                                Console.WriteLine("<>3 ADDING V WRAP GREATER FOR " + DebugName);
                            }
                            SolverVariable end = system.createObjectVariable(mBottom);
                            int wrapStrength = SolverVariable.STRENGTH_EQUALITY;
                            system.addGreaterThan(parentMax, end, 0, wrapStrength);
                        }
                    }
                    else if (mVisibility == GONE)
                    {
                        // TODO: use the constraints graph here to help
                        system.addEquality(baseline, top, mBaseline.Margin, SolverVariable.STRENGTH_FIXED);
                    }
                    else
                    {
                        system.addEquality(baseline, top, BaselineDistance, SolverVariable.STRENGTH_FIXED);
                    }
                }

                applyConstraints(system, false, verticalParentWrapContent, horizontalParentWrapContent, isTerminalWidget[VERTICAL], parentMin, parentMax, mListDimensionBehaviors[DIMENSION_VERTICAL], wrapContent, mTop, mBottom, mY, height, mMinHeight, mMaxDimension[VERTICAL], mVerticalBiasPercent, useVerticalRatio, mListDimensionBehaviors[HORIZONTAL] == MATCH_CONSTRAINT, inVerticalChain, inHorizontalChain, isInVerticalBarrier, matchConstraintDefaultHeight, matchConstraintDefaultWidth, mMatchConstraintMinHeight, mMatchConstraintMaxHeight, mMatchConstraintPercentHeight, applyPosition);
            }

            if (useRatio)
            {
                int strength = SolverVariable.STRENGTH_FIXED;
                if (mResolvedDimensionRatioSide == VERTICAL)
                {
                    system.addRatio(bottom, top, right, left, mResolvedDimensionRatio, strength);
                }
                else
                {
                    system.addRatio(right, left, bottom, top, mResolvedDimensionRatio, strength);
                }
            }

            if (mCenter.Connected)
            {
                system.addCenterPoint(this, mCenter.Target.Owner, (float)MathExtension.toRadians(mCircleConstraintAngle + 90), mCenter.Margin);
            }

            if (LinearSystem.FULL_DEBUG)
            {
                Console.WriteLine("\n----------------------------------------------");
                Console.WriteLine("-- DONE adding " + DebugName + " to the solver");
                Console.WriteLine("----------------------------------------------\n");
            }
            resolvedHorizontal = false;
            resolvedVertical = false;
        }

        /// <summary>
        /// Used to select which widgets should be added to the solver first
        /// @return
        /// </summary>
        internal virtual bool addFirst()
        {
            return this is VirtualLayout || this is Guideline;
        }

        /// <summary>
        /// Resolves the dimension ratio parameters
        /// (mResolvedDimensionRatioSide & mDimensionRatio)
        /// </summary>
        /// <param name="hParentWrapContent">       true if parent is in wrap content horizontally </param>
        /// <param name="vParentWrapContent">       true if parent is in wrap content vertically </param>
        /// <param name="horizontalDimensionFixed"> true if this widget horizontal dimension is fixed </param>
        /// <param name="verticalDimensionFixed">   true if this widget vertical dimension is fixed </param>
        public virtual void setupDimensionRatio(bool hParentWrapContent, bool vParentWrapContent, bool horizontalDimensionFixed, bool verticalDimensionFixed)
        {
            if (mResolvedDimensionRatioSide == UNKNOWN)
            {
                if (horizontalDimensionFixed && !verticalDimensionFixed)
                {
                    mResolvedDimensionRatioSide = HORIZONTAL;
                }
                else if (!horizontalDimensionFixed && verticalDimensionFixed)
                {
                    mResolvedDimensionRatioSide = VERTICAL;
                    if (mDimensionRatioSide == UNKNOWN)
                    {
                        // need to reverse the ratio as the parsing is done in horizontal mode
                        mResolvedDimensionRatio = 1 / mResolvedDimensionRatio;
                    }
                }
            }

            if (mResolvedDimensionRatioSide == HORIZONTAL && !(mTop.Connected && mBottom.Connected))
            {
                mResolvedDimensionRatioSide = VERTICAL;
            }
            else if (mResolvedDimensionRatioSide == VERTICAL && !(mLeft.Connected && mRight.Connected))
            {
                mResolvedDimensionRatioSide = HORIZONTAL;
            }

            // if dimension is still unknown... check parentWrap
            if (mResolvedDimensionRatioSide == UNKNOWN)
            {
                if (!(mTop.Connected && mBottom.Connected && mLeft.Connected && mRight.Connected))
                {
                    // only do that if not all connections are set
                    if (mTop.Connected && mBottom.Connected)
                    {
                        mResolvedDimensionRatioSide = HORIZONTAL;
                    }
                    else if (mLeft.Connected && mRight.Connected)
                    {
                        mResolvedDimensionRatio = 1 / mResolvedDimensionRatio;
                        mResolvedDimensionRatioSide = VERTICAL;
                    }
                }
            }

            if (false && mResolvedDimensionRatioSide == UNKNOWN)
            {
                if (hParentWrapContent && !vParentWrapContent)
                {
                    mResolvedDimensionRatioSide = HORIZONTAL;
                }
                else if (!hParentWrapContent && vParentWrapContent)
                {
                    mResolvedDimensionRatio = 1 / mResolvedDimensionRatio;
                    mResolvedDimensionRatioSide = VERTICAL;
                }
            }

            if (mResolvedDimensionRatioSide == UNKNOWN)
            {
                if (mMatchConstraintMinWidth > 0 && mMatchConstraintMinHeight == 0)
                {
                    mResolvedDimensionRatioSide = HORIZONTAL;
                }
                else if (mMatchConstraintMinWidth == 0 && mMatchConstraintMinHeight > 0)
                {
                    mResolvedDimensionRatio = 1 / mResolvedDimensionRatio;
                    mResolvedDimensionRatioSide = VERTICAL;
                }
            }

            if (false && mResolvedDimensionRatioSide == UNKNOWN && hParentWrapContent && vParentWrapContent)
            {
                mResolvedDimensionRatio = 1 / mResolvedDimensionRatio;
                mResolvedDimensionRatioSide = VERTICAL;
            }
        }

        /// <summary>
        /// Apply the constraints in the system depending on the existing anchors, in one dimension </summary>
        ///  <param name="system">                the linear system we are adding constraints to </param>
        /// <param name="parentWrapContent"> </param>
        /// <param name="isTerminal"> </param>
        /// <param name="parentMax"> </param>
        /// <param name="dimensionBehaviour"> </param>
        /// <param name="wrapContent">           is the widget trying to wrap its content (i.e. its size will depends on its content) </param>
        /// <param name="beginAnchor">           the first anchor </param>
        /// <param name="endAnchor">             the second anchor </param>
        /// <param name="beginPosition">         the original position of the anchor </param>
        /// <param name="dimension">             the dimension </param>
        /// <param name="maxDimension"> </param>
        /// <param name="oppositeVariable"> </param>
        /// <param name="matchPercentDimension"> the percentage relative to the parent, applied if in match constraint and percent mode </param>
        /// <param name="applyPosition"> </param>
        private void applyConstraints(LinearSystem system, bool isHorizontal, bool parentWrapContent, bool oppositeParentWrapContent, bool isTerminal, SolverVariable parentMin, SolverVariable parentMax, DimensionBehaviour dimensionBehaviour, bool wrapContent, ConstraintAnchor beginAnchor, ConstraintAnchor endAnchor, int beginPosition, int dimension, int minDimension, int maxDimension, float bias, bool useRatio, bool oppositeVariable, bool inChain, bool oppositeInChain, bool inBarrier, int matchConstraintDefault, int oppositeMatchConstraintDefault, int matchMinDimension, int matchMaxDimension, float matchPercentDimension, bool applyPosition)
        {
            SolverVariable begin = system.createObjectVariable(beginAnchor);
            SolverVariable end = system.createObjectVariable(endAnchor);
            SolverVariable beginTarget = system.createObjectVariable(beginAnchor.Target);
            SolverVariable endTarget = system.createObjectVariable(endAnchor.Target);

            if (androidx.constraintlayout.core.LinearSystem.Metrics != null)
            {
                androidx.constraintlayout.core.LinearSystem.Metrics.nonresolvedWidgets++;
            }

            bool isBeginConnected = beginAnchor.Connected;
            bool isEndConnected = endAnchor.Connected;
            bool isCenterConnected = mCenter.Connected;

            bool variableSize = false;

            int numConnections = 0;
            if (isBeginConnected)
            {
                numConnections++;
            }
            if (isEndConnected)
            {
                numConnections++;
            }
            if (isCenterConnected)
            {
                numConnections++;
            }

            if (useRatio)
            {
                matchConstraintDefault = MATCH_CONSTRAINT_RATIO;
            }
            switch (dimensionBehaviour)
            {
                case androidx.constraintlayout.core.widgets.ConstraintWidget.DimensionBehaviour.FIXED:
                {
                    variableSize = false;
                }
                break;
                case androidx.constraintlayout.core.widgets.ConstraintWidget.DimensionBehaviour.WRAP_CONTENT:
                {
                    variableSize = false;
                }
                break;
                case androidx.constraintlayout.core.widgets.ConstraintWidget.DimensionBehaviour.MATCH_PARENT:
                {
                    variableSize = false;
                }
                break;
                case androidx.constraintlayout.core.widgets.ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT:
                {
                    variableSize = matchConstraintDefault != MATCH_CONSTRAINT_RATIO_RESOLVED;
                }
                break;
            }


            if (mWidthOverride != -1 && isHorizontal)
            {
                if (FULL_DEBUG)
                {
                    Console.WriteLine("OVERRIDE WIDTH to " + mWidthOverride);
                }
                variableSize = false;
                dimension = mWidthOverride;
                mWidthOverride = -1;
            }
            if (mHeightOverride != -1 && !isHorizontal)
            {
                if (FULL_DEBUG)
                {
                    Console.WriteLine("OVERRIDE HEIGHT to " + mHeightOverride);
                }
                variableSize = false;
                dimension = mHeightOverride;
                mHeightOverride = -1;
            }

            if (mVisibility == ConstraintWidget.GONE)
            {
                dimension = 0;
                variableSize = false;
            }

            // First apply starting direct connections (more solver-friendly)
            if (applyPosition)
            {
                if (!isBeginConnected && !isEndConnected && !isCenterConnected)
                {
                    system.addEquality(begin, beginPosition);
                }
                else if (isBeginConnected && !isEndConnected)
                {
                    system.addEquality(begin, beginTarget, beginAnchor.Margin, SolverVariable.STRENGTH_FIXED);
                }
            }

            // Then apply the dimension
            if (!variableSize)
            {
                if (wrapContent)
                {
                    system.addEquality(end, begin, 0, SolverVariable.STRENGTH_HIGH);
                    if (minDimension > 0)
                    {
                        system.addGreaterThan(end, begin, minDimension, SolverVariable.STRENGTH_FIXED);
                    }
                    if (maxDimension < int.MaxValue)
                    {
                        system.addLowerThan(end, begin, maxDimension, SolverVariable.STRENGTH_FIXED);
                    }
                }
                else
                {
                    system.addEquality(end, begin, dimension, SolverVariable.STRENGTH_FIXED);
                }
            }
            else
            {
                if (numConnections != 2 && !useRatio && ((matchConstraintDefault == MATCH_CONSTRAINT_WRAP) || (matchConstraintDefault == MATCH_CONSTRAINT_SPREAD)))
                {
                    variableSize = false;
                    int d = Math.Max(matchMinDimension, dimension);
                    if (matchMaxDimension > 0)
                    {
                        d = Math.Min(matchMaxDimension, d);
                    }
                    system.addEquality(end, begin, d, SolverVariable.STRENGTH_FIXED);
                }
                else
                {
                    if (matchMinDimension == WRAP)
                    {
                        matchMinDimension = dimension;
                    }
                    if (matchMaxDimension == WRAP)
                    {
                        matchMaxDimension = dimension;
                    }
                    if (dimension > 0 && matchConstraintDefault != MATCH_CONSTRAINT_WRAP)
                    {
                        if (USE_WRAP_DIMENSION_FOR_SPREAD && (matchConstraintDefault == MATCH_CONSTRAINT_SPREAD))
                        {
                            system.addGreaterThan(end, begin, dimension, SolverVariable.STRENGTH_HIGHEST);
                        }
                        dimension = 0;
                    }

                    if (matchMinDimension > 0)
                    {
                        system.addGreaterThan(end, begin, matchMinDimension, SolverVariable.STRENGTH_FIXED);
                        dimension = Math.Max(dimension, matchMinDimension);
                    }
                    if (matchMaxDimension > 0)
                    {
                        bool applyLimit = true;
                        if (parentWrapContent && matchConstraintDefault == MATCH_CONSTRAINT_WRAP)
                        {
                            applyLimit = false;
                        }
                        if (applyLimit)
                        {
                            system.addLowerThan(end, begin, matchMaxDimension, SolverVariable.STRENGTH_FIXED);
                        }
                        dimension = Math.Min(dimension, matchMaxDimension);
                    }
                    if (matchConstraintDefault == MATCH_CONSTRAINT_WRAP)
                    {
                        if (parentWrapContent)
                        {
                            system.addEquality(end, begin, dimension, SolverVariable.STRENGTH_FIXED);
                        }
                        else if (inChain)
                        {
                            system.addEquality(end, begin, dimension, SolverVariable.STRENGTH_EQUALITY);
                            system.addLowerThan(end, begin, dimension, SolverVariable.STRENGTH_FIXED);
                        }
                        else
                        {
                            system.addEquality(end, begin, dimension, SolverVariable.STRENGTH_EQUALITY);
                            system.addLowerThan(end, begin, dimension, SolverVariable.STRENGTH_FIXED);
                        }
                    }
                    else if (matchConstraintDefault == MATCH_CONSTRAINT_PERCENT)
                    {
                        SolverVariable percentBegin = null;
                        SolverVariable percentEnd = null;
                        if (beginAnchor.getType() == ConstraintAnchor.Type.TOP || beginAnchor.getType() == ConstraintAnchor.Type.BOTTOM)
                        {
                            // vertical
                            percentBegin = system.createObjectVariable(mParent.getAnchor(ConstraintAnchor.Type.TOP));
                            percentEnd = system.createObjectVariable(mParent.getAnchor(ConstraintAnchor.Type.BOTTOM));
                        }
                        else
                        {
                            percentBegin = system.createObjectVariable(mParent.getAnchor(ConstraintAnchor.Type.LEFT));
                            percentEnd = system.createObjectVariable(mParent.getAnchor(ConstraintAnchor.Type.RIGHT));
                        }
                        system.addConstraint(system.createRow().createRowDimensionRatio(end, begin, percentEnd, percentBegin, matchPercentDimension));
                        if (parentWrapContent)
                        {
                            variableSize = false;
                        }
                    }
                    else
                    {
                        isTerminal = true;
                    }
                }
            }

            if (!applyPosition || inChain)
            {
                // If we don't need to apply the position, let's finish now.
                if (LinearSystem.FULL_DEBUG)
                {
                    Console.WriteLine("only deal with dimension for " + mDebugName + ", not positioning (applyPosition: " + applyPosition + " inChain: " + inChain + ")");
                }
                if (numConnections < 2 && parentWrapContent && isTerminal)
                {
                    system.addGreaterThan(begin, parentMin, 0, SolverVariable.STRENGTH_FIXED);
                    bool applyEnd = isHorizontal || (mBaseline.mTarget == null);
                    if (!isHorizontal && mBaseline.mTarget != null)
                    {
                        // generally we wouldn't take the current widget in the wrap content, but if the connected element is a ratio widget,
                        // then we can contribute (as the ratio widget may not be enough by itself) to it.
                        ConstraintWidget target = mBaseline.mTarget.mOwner;
                        if (target.mDimensionRatio != 0 && target.mListDimensionBehaviors[0] == MATCH_CONSTRAINT && target.mListDimensionBehaviors[1] == MATCH_CONSTRAINT)
                        {
                            applyEnd = true;
                        }
                        else
                        {
                            applyEnd = false;
                        }
                    }
                    if (applyEnd)
                    {
                        if (FULL_DEBUG)
                        {
                            Console.WriteLine("<>4 ADDING WRAP GREATER FOR " + DebugName);
                        }
                        system.addGreaterThan(parentMax, end, 0, SolverVariable.STRENGTH_FIXED);
                    }
                }
                return;
            }

            // Ok, we are dealing with single or centered constraints, let's apply them

            int wrapStrength = SolverVariable.STRENGTH_EQUALITY;

            if (!isBeginConnected && !isEndConnected && !isCenterConnected)
            {
                // note we already applied the start position before, no need to redo it...
            }
            else if (isBeginConnected && !isEndConnected)
            {
                // note we already applied the start position before, no need to redo it...

                // If we are constrained to a barrier, make sure that we are not bypassed in the wrap
                ConstraintWidget beginWidget = beginAnchor.mTarget.mOwner;
                if (parentWrapContent && beginWidget is Barrier)
                {
                    wrapStrength = SolverVariable.STRENGTH_FIXED;
                }
            }
            else if (!isBeginConnected && isEndConnected)
            {
                system.addEquality(end, endTarget, -endAnchor.Margin, SolverVariable.STRENGTH_FIXED);
                if (parentWrapContent)
                {
                    if (OPTIMIZE_WRAP && begin.isFinalValue && mParent != null)
                    {
                        ConstraintWidgetContainer container = (ConstraintWidgetContainer) mParent;
                        if (isHorizontal)
                        {
                            container.addHorizontalWrapMinVariable(beginAnchor);
                        }
                        else
                        {
                            container.addVerticalWrapMinVariable(beginAnchor);
                        }
                    }
                    else
                    {
                        if (FULL_DEBUG)
                        {
                            Console.WriteLine("<>5 ADDING WRAP GREATER FOR " + DebugName);
                        }
                        system.addGreaterThan(begin, parentMin, 0, SolverVariable.STRENGTH_EQUALITY);
                    }
                }
            }
            else if (isBeginConnected && isEndConnected)
            {
                bool applyBoundsCheck = true;
                bool applyCentering = false;
                bool applyStrongChecks = false;
                bool applyRangeCheck = false;
                int rangeCheckStrength = SolverVariable.STRENGTH_EQUALITY;
                int boundsCheckStrength = SolverVariable.STRENGTH_HIGHEST; // TODO: might not need it here (it's overridden)
                int centeringStrength = SolverVariable.STRENGTH_BARRIER;

                if (parentWrapContent)
                {
                    rangeCheckStrength = SolverVariable.STRENGTH_EQUALITY;
                }
                ConstraintWidget beginWidget = beginAnchor.mTarget.mOwner;
                ConstraintWidget endWidget = endAnchor.mTarget.mOwner;
                ConstraintWidget parent = Parent;

                if (variableSize)
                {
                    if (matchConstraintDefault == MATCH_CONSTRAINT_SPREAD)
                    {
                        if (matchMaxDimension == 0 && matchMinDimension == 0)
                        {
                            applyStrongChecks = true;
                            rangeCheckStrength = SolverVariable.STRENGTH_FIXED;
                            boundsCheckStrength = SolverVariable.STRENGTH_FIXED;
                            // Optimization in case of centering in parent
                            if (beginTarget.isFinalValue && endTarget.isFinalValue)
                            {
                                system.addEquality(begin, beginTarget, beginAnchor.Margin, SolverVariable.STRENGTH_FIXED);
                                system.addEquality(end, endTarget, -endAnchor.Margin, SolverVariable.STRENGTH_FIXED);
                                return;
                            }
                        }
                        else
                        {
                            applyCentering = true;
                            rangeCheckStrength = SolverVariable.STRENGTH_EQUALITY;
                            boundsCheckStrength = SolverVariable.STRENGTH_EQUALITY;
                            applyBoundsCheck = true;
                            applyRangeCheck = true;
                        }
                        if (beginWidget is Barrier || endWidget is Barrier)
                        {
                            boundsCheckStrength = SolverVariable.STRENGTH_HIGHEST;
                        }
                    }
                    else if (matchConstraintDefault == MATCH_CONSTRAINT_PERCENT)
                    {
                        applyCentering = true;
                        rangeCheckStrength = SolverVariable.STRENGTH_EQUALITY;
                        boundsCheckStrength = SolverVariable.STRENGTH_EQUALITY;
                        applyBoundsCheck = true;
                        applyRangeCheck = true;
                        if (beginWidget is Barrier || endWidget is Barrier)
                        {
                            boundsCheckStrength = SolverVariable.STRENGTH_HIGHEST;
                        }
                    }
                    else if (matchConstraintDefault == MATCH_CONSTRAINT_WRAP)
                    {
                        applyCentering = true;
                        applyRangeCheck = true;
                        rangeCheckStrength = SolverVariable.STRENGTH_FIXED;
                    }
                    else if (matchConstraintDefault == MATCH_CONSTRAINT_RATIO)
                    {
                        if (mResolvedDimensionRatioSide == UNKNOWN)
                        {
                            applyCentering = true;
                            applyRangeCheck = true;
                            applyStrongChecks = true;
                            rangeCheckStrength = SolverVariable.STRENGTH_FIXED;
                            boundsCheckStrength = SolverVariable.STRENGTH_EQUALITY;
                            if (oppositeInChain)
                            {
                                boundsCheckStrength = SolverVariable.STRENGTH_EQUALITY;
                                centeringStrength = SolverVariable.STRENGTH_HIGHEST;
                                if (parentWrapContent)
                                {
                                    centeringStrength = SolverVariable.STRENGTH_EQUALITY;
                                }
                            }
                            else
                            {
                                centeringStrength = SolverVariable.STRENGTH_FIXED;
                            }
                        }
                        else
                        {
                            applyCentering = true;
                            applyRangeCheck = true;
                            applyStrongChecks = true;
                            if (useRatio)
                            {
                                // useRatio is true if the side we base ourselves on for the ratio is this one
                                // if that's not the case, we need to have a stronger constraint.
                                bool otherSideInvariable = oppositeMatchConstraintDefault == MATCH_CONSTRAINT_PERCENT || oppositeMatchConstraintDefault == MATCH_CONSTRAINT_WRAP;
                                if (!otherSideInvariable)
                                {
                                    rangeCheckStrength = SolverVariable.STRENGTH_FIXED;
                                    boundsCheckStrength = SolverVariable.STRENGTH_EQUALITY;
                                }
                            }
                            else
                            {
                                rangeCheckStrength = SolverVariable.STRENGTH_EQUALITY;
                                if (matchMaxDimension > 0)
                                {
                                    boundsCheckStrength = SolverVariable.STRENGTH_EQUALITY;
                                }
                                else if (matchMaxDimension == 0 && matchMinDimension == 0)
                                {
                                    if (!oppositeInChain)
                                    {
                                        boundsCheckStrength = SolverVariable.STRENGTH_FIXED;
                                    }
                                    else
                                    {
                                        if (beginWidget != parent && endWidget != parent)
                                        {
                                            rangeCheckStrength = SolverVariable.STRENGTH_HIGHEST;
                                        }
                                        else
                                        {
                                            rangeCheckStrength = SolverVariable.STRENGTH_EQUALITY;
                                        }
                                        boundsCheckStrength = SolverVariable.STRENGTH_HIGHEST;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    applyCentering = true;
                    applyRangeCheck = true;

                    // Let's optimize away if we can...
                    if (beginTarget.isFinalValue && endTarget.isFinalValue)
                    {
                        system.addCentering(begin, beginTarget, beginAnchor.Margin, bias, endTarget, end, endAnchor.Margin, SolverVariable.STRENGTH_FIXED);
                        if (parentWrapContent && isTerminal)
                        {
                            int margin = 0;
                            if (endAnchor.mTarget != null)
                            {
                                margin = endAnchor.Margin;
                            }
                            if (endTarget != parentMax)
                            { // if not already applied
                                if (FULL_DEBUG)
                                {
                                    Console.WriteLine("<>6 ADDING WRAP GREATER FOR " + DebugName);
                                }
                                system.addGreaterThan(parentMax, end, margin, wrapStrength);
                            }
                        }
                        return;
                    }
                }

                if (applyRangeCheck && beginTarget == endTarget && beginWidget != parent)
                {
                    // no need to apply range / bounds check if we are centered on the same anchor
                    applyRangeCheck = false;
                    applyBoundsCheck = false;
                }

                if (applyCentering)
                {
                    if (!variableSize && !oppositeVariable && !oppositeInChain && beginTarget == parentMin && endTarget == parentMax)
                    {
                        // for fixed size widgets, we can simplify the constraints
                        centeringStrength = SolverVariable.STRENGTH_FIXED;
                        rangeCheckStrength = SolverVariable.STRENGTH_FIXED;
                        applyBoundsCheck = false;
                        parentWrapContent = false;
                    }

                    system.addCentering(begin, beginTarget, beginAnchor.Margin, bias, endTarget, end, endAnchor.Margin, centeringStrength);
                }

                if (mVisibility == GONE && !endAnchor.hasDependents())
                {
                    return;
                }

                if (applyRangeCheck)
                {
                    if (parentWrapContent && beginTarget != endTarget && !variableSize)
                    {
                        if (beginWidget is Barrier || endWidget is Barrier)
                        {
                            rangeCheckStrength = SolverVariable.STRENGTH_BARRIER;
                        }
                    }
                    system.addGreaterThan(begin, beginTarget, beginAnchor.Margin, rangeCheckStrength);
                    system.addLowerThan(end, endTarget, -endAnchor.Margin, rangeCheckStrength);
                }

                if (parentWrapContent && inBarrier && !(beginWidget is Barrier || endWidget is Barrier) && !(endWidget == parent))
                { // if we are referenced by a barrier
                    // ... but not directly constrained by it
                    // ... then make sure we can hold our own
                    boundsCheckStrength = SolverVariable.STRENGTH_BARRIER;
                    rangeCheckStrength = SolverVariable.STRENGTH_BARRIER;
                    applyBoundsCheck = true;
                }

                if (applyBoundsCheck)
                {
                    if (applyStrongChecks && (!oppositeInChain || oppositeParentWrapContent))
                    {
                        int strength = boundsCheckStrength;
                        if (beginWidget == parent || endWidget == parent)
                        {
                            strength = SolverVariable.STRENGTH_BARRIER;
                        }
                        if (beginWidget is Guideline || endWidget is Guideline)
                        {
                            strength = SolverVariable.STRENGTH_EQUALITY;
                        }
                        if (beginWidget is Barrier || endWidget is Barrier)
                        {
                            strength = SolverVariable.STRENGTH_EQUALITY;
                        }
                        if (oppositeInChain)
                        {
                            strength = SolverVariable.STRENGTH_EQUALITY;
                        }
                        boundsCheckStrength = Math.Max(strength, boundsCheckStrength);
                    }

                    if (parentWrapContent)
                    {
                        boundsCheckStrength = Math.Min(rangeCheckStrength, boundsCheckStrength);
                        if (useRatio && !oppositeInChain && (beginWidget == parent || endWidget == parent))
                        {
                            // When using ratio, relax some strength to allow other parts of the system
                            // to take precedence rather than driving it
                            boundsCheckStrength = SolverVariable.STRENGTH_HIGHEST;
                        }
                    }
                    system.addEquality(begin, beginTarget, beginAnchor.Margin, boundsCheckStrength);
                    system.addEquality(end, endTarget, -endAnchor.Margin, boundsCheckStrength);
                }

                if (parentWrapContent)
                {
                    int margin = 0;
                    if (parentMin == beginTarget)
                    {
                        margin = beginAnchor.Margin;
                    }
                    if (beginTarget != parentMin)
                    { // already done otherwise
                        if (FULL_DEBUG)
                        {
                            Console.WriteLine("<>7 ADDING WRAP GREATER FOR " + DebugName);
                        }
                        system.addGreaterThan(begin, parentMin, margin, wrapStrength);
                    }
                }

                if (parentWrapContent && variableSize && minDimension == 0 && matchMinDimension == 0)
                {
                    if (FULL_DEBUG)
                    {
                        Console.WriteLine("<>8 ADDING WRAP GREATER FOR " + DebugName);
                    }
                    if (variableSize && matchConstraintDefault == MATCH_CONSTRAINT_RATIO)
                    {
                        system.addGreaterThan(end, begin, 0, SolverVariable.STRENGTH_FIXED);
                    }
                    else
                    {
                        system.addGreaterThan(end, begin, 0, wrapStrength);
                    }
                }
            }

            if (parentWrapContent && isTerminal)
            {
                int margin = 0;
                if (endAnchor.mTarget != null)
                {
                    margin = endAnchor.Margin;
                }
                if (endTarget != parentMax)
                { // if not already applied
                    if (OPTIMIZE_WRAP && end.isFinalValue && mParent != null)
                    {
                        ConstraintWidgetContainer container = (ConstraintWidgetContainer) mParent;
                        if (isHorizontal)
                        {
                            container.addHorizontalWrapMaxVariable(endAnchor);
                        }
                        else
                        {
                            container.addVerticalWrapMaxVariable(endAnchor);
                        }
                        return;
                    }
                    if (FULL_DEBUG)
                    {
                        Console.WriteLine("<>9 ADDING WRAP GREATER FOR " + DebugName);
                    }
                    system.addGreaterThan(parentMax, end, margin, wrapStrength);
                }
            }
        }

        /// <summary>
        /// Update the widget from the values generated by the solver
        /// </summary>
        /// <param name="system"> the solver we get the values from. </param>
        /// <param name="optimize"> true if <seealso cref="Optimizer#OPTIMIZATION_GRAPH"/> is on </param>
        public virtual void updateFromSolver(LinearSystem system, bool optimize)
        {
            int left = system.getObjectVariableValue(mLeft);
            int top = system.getObjectVariableValue(mTop);
            int right = system.getObjectVariableValue(mRight);
            int bottom = system.getObjectVariableValue(mBottom);

            if (optimize && horizontalRun != null && horizontalRun.start.resolved && horizontalRun.end.resolved)
            {
                left = horizontalRun.start.value;
                right = horizontalRun.end.value;
            }
            if (optimize && verticalRun != null && verticalRun.start.resolved && verticalRun.end.resolved)
            {
                top = verticalRun.start.value;
                bottom = verticalRun.end.value;
            }

            int w = right - left;
            int h = bottom - top;
            if (w < 0 || h < 0 || left == int.MinValue || left == int.MaxValue || top == int.MinValue || top == int.MaxValue || right == int.MinValue || right == int.MaxValue || bottom == int.MinValue || bottom == int.MaxValue)
            {
                left = 0;
                top = 0;
                right = 0;
                bottom = 0;
            }
            setFrame(left, top, right, bottom);
            if (DEBUG)
            {
                Console.WriteLine(" *** UPDATE FROM SOLVER " + this);
            }
        }

        public virtual void copy(ConstraintWidget src, Dictionary<ConstraintWidget, ConstraintWidget> map)
        {
            // Support for direct resolution
            mHorizontalResolution = src.mHorizontalResolution;
            mVerticalResolution = src.mVerticalResolution;

            mMatchConstraintDefaultWidth = src.mMatchConstraintDefaultWidth;
            mMatchConstraintDefaultHeight = src.mMatchConstraintDefaultHeight;

            mResolvedMatchConstraintDefault[0] = src.mResolvedMatchConstraintDefault[0];
            mResolvedMatchConstraintDefault[1] = src.mResolvedMatchConstraintDefault[1];

            mMatchConstraintMinWidth = src.mMatchConstraintMinWidth;
            mMatchConstraintMaxWidth = src.mMatchConstraintMaxWidth;
            mMatchConstraintMinHeight = src.mMatchConstraintMinHeight;
            mMatchConstraintMaxHeight = src.mMatchConstraintMaxHeight;
            mMatchConstraintPercentHeight = src.mMatchConstraintPercentHeight;
            mIsWidthWrapContent = src.mIsWidthWrapContent;
            mIsHeightWrapContent = src.mIsHeightWrapContent;

            mResolvedDimensionRatioSide = src.mResolvedDimensionRatioSide;
            mResolvedDimensionRatio = src.mResolvedDimensionRatio;

            mMaxDimension = src.mMaxDimension.Copy<int>(src.mMaxDimension.Length);
            mCircleConstraintAngle = src.mCircleConstraintAngle;
            hasBaseline_Renamed = src.hasBaseline_Renamed;
            inPlaceholder = src.inPlaceholder;

            // The anchors available on the widget
            // note: all anchors should be added to the mAnchors array (see addAnchors())

            mLeft.reset();
            mTop.reset();
            mRight.reset();
            mBottom.reset();
            mBaseline.reset();
            mCenterX.reset();
            mCenterY.reset();
            mCenter.reset();
            //mListDimensionBehaviors = Arrays.copyOf(mListDimensionBehaviors, 2);
            mListDimensionBehaviors = mListDimensionBehaviors.Copy<DimensionBehaviour>(2);//TODO:验证这个数组复制正确性
            mParent = (mParent == null) ? null : map[src.mParent];

            mWidth = src.mWidth;
            mHeight = src.mHeight;
            mDimensionRatio = src.mDimensionRatio;
            mDimensionRatioSide = src.mDimensionRatioSide;

            mX = src.mX;
            mY = src.mY;
            mRelX = src.mRelX;
            mRelY = src.mRelY;

            mOffsetX = src.mOffsetX;
            mOffsetY = src.mOffsetY;

            mBaselineDistance = src.mBaselineDistance;
            mMinWidth = src.mMinWidth;
            mMinHeight = src.mMinHeight;

            mHorizontalBiasPercent = src.mHorizontalBiasPercent;
            mVerticalBiasPercent = src.mVerticalBiasPercent;

            mCompanionWidget = src.mCompanionWidget;
            mContainerItemSkip = src.mContainerItemSkip;
            mVisibility = src.mVisibility;
            mDebugName = src.mDebugName;
            mType = src.mType;

            mDistToTop = src.mDistToTop;
            mDistToLeft = src.mDistToLeft;
            mDistToRight = src.mDistToRight;
            mDistToBottom = src.mDistToBottom;
            mLeftHasCentered = src.mLeftHasCentered;
            mRightHasCentered = src.mRightHasCentered;

            mTopHasCentered = src.mTopHasCentered;
            mBottomHasCentered = src.mBottomHasCentered;

            mHorizontalWrapVisited = src.mHorizontalWrapVisited;
            mVerticalWrapVisited = src.mVerticalWrapVisited;

            mHorizontalChainStyle = src.mHorizontalChainStyle;
            mVerticalChainStyle = src.mVerticalChainStyle;
            mHorizontalChainFixedPosition = src.mHorizontalChainFixedPosition;
            mVerticalChainFixedPosition = src.mVerticalChainFixedPosition;
            mWeight[0] = src.mWeight[0];
            mWeight[1] = src.mWeight[1];

            mListNextMatchConstraintsWidget[0] = src.mListNextMatchConstraintsWidget[0];
            mListNextMatchConstraintsWidget[1] = src.mListNextMatchConstraintsWidget[1];

            mNextChainWidget[0] = src.mNextChainWidget[0];
            mNextChainWidget[1] = src.mNextChainWidget[1];

            mHorizontalNextWidget = (src.mHorizontalNextWidget == null) ? null : map[src.mHorizontalNextWidget];
            mVerticalNextWidget = (src.mVerticalNextWidget == null) ? null : map[src.mVerticalNextWidget];
        }

        public virtual void updateFromRuns(bool updateHorizontal, bool updateVertical)
        {
            updateHorizontal &= horizontalRun.Resolved;
            updateVertical &= verticalRun.Resolved;
            int left = horizontalRun.start.value;
            int top = verticalRun.start.value;
            int right = horizontalRun.end.value;
            int bottom = verticalRun.end.value;
            int w = right - left;
            int h = bottom - top;
            if (w < 0 || h < 0 || left == int.MinValue || left == int.MaxValue || top == int.MinValue || top == int.MaxValue || right == int.MinValue || right == int.MaxValue || bottom == int.MinValue || bottom == int.MaxValue)
            {
                left = 0;
                top = 0;
                right = 0;
                bottom = 0;
            }

            w = right - left;
            h = bottom - top;

            if (updateHorizontal)
            {
                mX = left;
            }
            if (updateVertical)
            {
                mY = top;
            }

            if (mVisibility == ConstraintWidget.GONE)
            {
                mWidth = 0;
                mHeight = 0;
                return;
            }

            // correct dimensional instability caused by rounding errors
            if (updateHorizontal)
            {
                if (mListDimensionBehaviors[DIMENSION_HORIZONTAL] == DimensionBehaviour.FIXED && w < mWidth)
                {
                    w = mWidth;
                }
                mWidth = w;
                if (mWidth < mMinWidth)
                {
                    mWidth = mMinWidth;
                }
            }

            if (updateVertical)
            {
                if (mListDimensionBehaviors[DIMENSION_VERTICAL] == DimensionBehaviour.FIXED && h < mHeight)
                {
                    h = mHeight;
                }
                mHeight = h;
                if (mHeight < mMinHeight)
                {
                    mHeight = mMinHeight;
                }
            }

        }

        public virtual void addChildrenToSolverByDependency(ConstraintWidgetContainer container, LinearSystem system, HashSet<ConstraintWidget> widgets, int orientation, bool addSelf)
        {
            if (addSelf)
            {
                if (!widgets.Contains(this))
                {
                    return;
                }
                Optimizer.checkMatchParent(container, system, this);
                widgets.Remove(this);
                addToSolver(system, container.optimizeFor(Optimizer.OPTIMIZATION_GRAPH));
            }
            if (orientation == HORIZONTAL)
            {
                HashSet<ConstraintAnchor> dependents = mLeft.Dependents;
                if (dependents != null)
                {
                    foreach (ConstraintAnchor anchor in dependents)
                    {
                        anchor.mOwner.addChildrenToSolverByDependency(container, system, widgets, orientation, true);
                    }
                }
                dependents = mRight.Dependents;
                if (dependents != null)
                {
                    foreach (ConstraintAnchor anchor in dependents)
                    {
                        anchor.mOwner.addChildrenToSolverByDependency(container, system, widgets, orientation, true);
                    }
                }
            }
            else
            {
                HashSet<ConstraintAnchor> dependents = mTop.Dependents;
                if (dependents != null)
                {
                    foreach (ConstraintAnchor anchor in dependents)
                    {
                        anchor.mOwner.addChildrenToSolverByDependency(container, system, widgets, orientation, true);
                    }
                }
                dependents = mBottom.Dependents;
                if (dependents != null)
                {
                    foreach (ConstraintAnchor anchor in dependents)
                    {
                        anchor.mOwner.addChildrenToSolverByDependency(container, system, widgets, orientation, true);
                    }
                }
                dependents = mBaseline.Dependents;
                if (dependents != null)
                {
                    foreach (ConstraintAnchor anchor in dependents)
                    {
                        anchor.mOwner.addChildrenToSolverByDependency(container, system, widgets, orientation, true);
                    }
                }
            }
            // horizontal
        }

    }

}
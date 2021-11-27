using System;
using System.Collections.Generic;

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
    using BasicMeasure = androidx.constraintlayout.core.widgets.analyzer.BasicMeasure;
    using DependencyGraph = androidx.constraintlayout.core.widgets.analyzer.DependencyGraph;
    using Direct = androidx.constraintlayout.core.widgets.analyzer.Direct;
    using Grouping = androidx.constraintlayout.core.widgets.analyzer.Grouping;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.LinearSystem.FULL_DEBUG;
using static androidx.constraintlayout.core.LinearSystem;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.DimensionBehaviour.FIXED;
using static androidx.constraintlayout.core.widgets.ConstraintWidget.DimensionBehaviour;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;

    /// <summary>
    /// A container of ConstraintWidget that can layout its children
    /// </summary>
    public class ConstraintWidgetContainer : WidgetContainer
    {
        private bool InstanceFieldsInitialized = false;

        private void InitializeInstanceFields()
        {
            mBasicMeasureSolver = new BasicMeasure(this);
            mDependencyGraph = new DependencyGraph(this);
        }


        private const int MAX_ITERATIONS = 8;

        private static readonly bool DEBUG = FULL_DEBUG;
        private const bool DEBUG_LAYOUT = false;
        internal const bool DEBUG_GRAPH = false;

        internal BasicMeasure mBasicMeasureSolver;

        ////////////////////////////////////////////////////////////////////////////////////////////////
        // Graph measures
        ////////////////////////////////////////////////////////////////////////////////////////////////

        public DependencyGraph mDependencyGraph;
        private int pass; // number of layout passes

        /// <summary>
        /// Invalidate the graph of constraints
        /// </summary>
        public virtual void invalidateGraph()
        {
            mDependencyGraph.invalidateGraph();
        }

        /// <summary>
        /// Invalidate the widgets measures
        /// </summary>
        public virtual void invalidateMeasures()
        {
            mDependencyGraph.invalidateMeasures();
        }


        public virtual bool directMeasure(bool optimizeWrap)
        {
            return mDependencyGraph.directMeasure(optimizeWrap);
    //        int paddingLeft = getX();
    //        int paddingTop = getY();
    //        if (mDependencyGraph.directMeasureSetup(optimizeWrap)) {
    //            mDependencyGraph.measureWidgets();
    //            boolean allResolved = mDependencyGraph.directMeasureWithOrientation(optimizeWrap, HORIZONTAL);
    //            allResolved &= mDependencyGraph.directMeasureWithOrientation(optimizeWrap, VERTICAL);
    //            for (ConstraintWidget child : mChildren) {
    //                child.setDrawX(child.getDrawX() + paddingLeft);
    //                child.setDrawY(child.getDrawY() + paddingTop);
    //            }
    //            setX(paddingLeft);
    //            setY(paddingTop);
    //            return allResolved;
    //        }
    //        return false;
        }

        public virtual bool directMeasureSetup(bool optimizeWrap)
        {
            return mDependencyGraph.directMeasureSetup(optimizeWrap);
        }

        public virtual bool directMeasureWithOrientation(bool optimizeWrap, int orientation)
        {
            return mDependencyGraph.directMeasureWithOrientation(optimizeWrap, orientation);
        }

        public virtual void defineTerminalWidgets()
        {
            mDependencyGraph.defineTerminalWidgets(HorizontalDimensionBehaviour, VerticalDimensionBehaviour);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Measure the layout
        /// </summary>
        /// <param name="optimizationLevel"> </param>
        /// <param name="widthMode"> </param>
        /// <param name="widthSize"> </param>
        /// <param name="heightMode"> </param>
        /// <param name="heightSize"> </param>
        /// <param name="paddingX"> </param>
        /// <param name="paddingY"> </param>
        public virtual long measure(int optimizationLevel, int widthMode, int widthSize, int heightMode, int heightSize, int lastMeasureWidth, int lastMeasureHeight, int paddingX, int paddingY)
        {
            mPaddingLeft = paddingX;
            mPaddingTop = paddingY;
            return mBasicMeasureSolver.solverMeasure(this, optimizationLevel, paddingX, paddingY, widthMode, widthSize, heightMode, heightSize, lastMeasureWidth, lastMeasureHeight);
        }

        public virtual void updateHierarchy()
        {
            mBasicMeasureSolver.updateHierarchy(this);
        }

        protected internal BasicMeasure.Measurer mMeasurer = null;

        public virtual BasicMeasure.Measurer Measurer
        {
            set
            {
                mMeasurer = value;
                mDependencyGraph.Measurer = value;
            }
            get
            {
                return mMeasurer;
            }
        }


        private bool mIsRtl = false;
        public Metrics mMetrics;

        public virtual void fillMetrics(Metrics metrics)
        {
            mMetrics = metrics;
            mSystem.fillMetrics(metrics);
        }

        protected internal LinearSystem mSystem = new LinearSystem();

        internal int mPaddingLeft;
        internal int mPaddingTop;
        internal int mPaddingRight;
        internal int mPaddingBottom;

        public int mHorizontalChainsSize = 0;
        public int mVerticalChainsSize = 0;

        internal ChainHead[] mVerticalChainsArray = new ChainHead[4];
        internal ChainHead[] mHorizontalChainsArray = new ChainHead[4];

        public bool mGroupsWrapOptimized = false;
        public bool mHorizontalWrapOptimized = false;
        public bool mVerticalWrapOptimized = false;
        public int mWrapFixedWidth = 0;
        public int mWrapFixedHeight = 0;

        private int mOptimizationLevel = Optimizer.OPTIMIZATION_STANDARD;
        public bool mSkipSolver = false;

        private bool mWidthMeasuredTooSmall = false;
        private bool mHeightMeasuredTooSmall = false;

        /*-----------------------------------------------------------------------*/
        // Construction
        /*-----------------------------------------------------------------------*/

        /// <summary>
        /// Default constructor
        /// </summary>
        public ConstraintWidgetContainer()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="x">      x position </param>
        /// <param name="y">      y position </param>
        /// <param name="width">  width of the layout </param>
        /// <param name="height"> height of the layout </param>
        public ConstraintWidgetContainer(int x, int y, int width, int height) : base(x, y, width, height)
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="width">  width of the layout </param>
        /// <param name="height"> height of the layout </param>
        public ConstraintWidgetContainer(int width, int height) : base(width, height)
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        public ConstraintWidgetContainer(string debugName, int width, int height) : base(width, height)
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
            DebugName = debugName;
        }

        /// <summary>
        /// Resolves the system directly when possible
        /// </summary>
        /// <param name="value"> optimization level </param>
        public virtual int OptimizationLevel
        {
            set
            {
                mOptimizationLevel = value;
                LinearSystem.USE_DEPENDENCY_ORDERING = optimizeFor(Optimizer.OPTIMIZATION_DEPENDENCY_ORDERING);
            }
            get
            {
                return mOptimizationLevel;
            }
        }


        /// <summary>
        /// Returns true if the given feature should be optimized
        /// </summary>
        /// <param name="feature">
        /// @return </param>
        public virtual bool optimizeFor(int feature)
        {
            return (mOptimizationLevel & feature) == feature;
        }

        /// <summary>
        /// Specify the xml type for the container
        /// 
        /// @return
        /// </summary>
        public override string Type
        {
            get
            {
                return "ConstraintLayout";
            }
        }

        public override void reset()
        {
            mSystem.reset();
            mPaddingLeft = 0;
            mPaddingRight = 0;
            mPaddingTop = 0;
            mPaddingBottom = 0;
            mSkipSolver = false;
            base.reset();
        }

        /// <summary>
        /// Return true if the width given is too small for the content laid out
        /// </summary>
        public virtual bool WidthMeasuredTooSmall
        {
            get
            {
                return mWidthMeasuredTooSmall;
            }
        }

        /// <summary>
        /// Return true if the height given is too small for the content laid out
        /// </summary>
        public virtual bool HeightMeasuredTooSmall
        {
            get
            {
                return mHeightMeasuredTooSmall;
            }
        }

        internal int mDebugSolverPassCount = 0;

        private WeakReference<ConstraintAnchor> verticalWrapMin = null;
        private WeakReference<ConstraintAnchor> horizontalWrapMin = null;
        private WeakReference<ConstraintAnchor> verticalWrapMax = null;
        private WeakReference<ConstraintAnchor> horizontalWrapMax = null;

        internal virtual void addVerticalWrapMinVariable(ConstraintAnchor top)
        {
            if(verticalWrapMin != null)
            {
                ConstraintAnchor verticalWrap;
                verticalWrapMin.TryGetTarget(out verticalWrap);
                if (verticalWrap == null || top.FinalValue > verticalWrap.FinalValue)
                {
                    verticalWrapMin = new WeakReference<ConstraintAnchor>(top);
                }
            }
            else
            {
                verticalWrapMin = new WeakReference<ConstraintAnchor>(top);
            }
        }

        public virtual void addHorizontalWrapMinVariable(ConstraintAnchor left)
        {
            if(horizontalWrapMin != null)
            {
                ConstraintAnchor horizontalWrap;
                horizontalWrapMin.TryGetTarget(out horizontalWrap);
                if (horizontalWrap == null || left.FinalValue > horizontalWrap.FinalValue)
                {
                    horizontalWrapMin = new WeakReference<ConstraintAnchor>(left);
                }
            }
            else
            {
                horizontalWrapMin = new WeakReference<ConstraintAnchor>(left);
            }
            
        }

        internal virtual void addVerticalWrapMaxVariable(ConstraintAnchor bottom)
        {
            if(verticalWrapMax != null)
            {
                ConstraintAnchor verticalWrap;
                verticalWrapMax.TryGetTarget(out verticalWrap);
                if (verticalWrap == null || bottom.FinalValue > verticalWrap.FinalValue)
                {
                    verticalWrapMax = new WeakReference<ConstraintAnchor>(bottom);
                }
            }
            else
            {
                verticalWrapMax = new WeakReference<ConstraintAnchor>(bottom);
            }
            
        }

        public virtual void addHorizontalWrapMaxVariable(ConstraintAnchor right)
        {
            if(horizontalWrapMax != null)
            {
                ConstraintAnchor horizontalWrap;
                horizontalWrapMax.TryGetTarget(out horizontalWrap);
                if (horizontalWrap == null || right.FinalValue > horizontalWrap.FinalValue)
                {
                    horizontalWrapMax = new WeakReference<ConstraintAnchor>(right);
                }
            }
            else
            {
                horizontalWrapMax = new WeakReference<ConstraintAnchor>(right);
            }
        }

        private void addMinWrap(ConstraintAnchor constraintAnchor, SolverVariable parentMin)
        {
            SolverVariable variable = mSystem.createObjectVariable(constraintAnchor);
            int wrapStrength = SolverVariable.STRENGTH_EQUALITY;
            mSystem.addGreaterThan(variable, parentMin, 0, wrapStrength);
        }

        private void addMaxWrap(ConstraintAnchor constraintAnchor, SolverVariable parentMax)
        {
            SolverVariable variable = mSystem.createObjectVariable(constraintAnchor);
            int wrapStrength = SolverVariable.STRENGTH_EQUALITY;
            mSystem.addGreaterThan(parentMax, variable, 0, wrapStrength);
        }

        internal HashSet<ConstraintWidget> widgetsToAdd = new HashSet<ConstraintWidget>();

        /// <summary>
        /// Add this widget to the solver
        /// </summary>
        /// <param name="system"> the solver we want to add the widget to </param>
        public virtual bool addChildrenToSolver(LinearSystem system)
        {
            if (DEBUG)
            {
                Console.WriteLine("\n#######################################");
                Console.WriteLine("##    ADD CHILDREN TO SOLVER  (" + mDebugSolverPassCount + ") ##");
                Console.WriteLine("#######################################\n");
                mDebugSolverPassCount++;
            }

            bool optimize = optimizeFor(Optimizer.OPTIMIZATION_GRAPH);
            addToSolver(system, optimize);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildren.size();
            int count = mChildren.Count;

            bool hasBarriers = false;
            for (int i = 0; i < count; i++)
            {
                ConstraintWidget widget = mChildren[i];
                widget.setInBarrier(HORIZONTAL, false);
                widget.setInBarrier(VERTICAL, false);
                if (widget is Barrier)
                {
                    hasBarriers = true;
                }
            }

            if (hasBarriers)
            {
                for (int i = 0; i < count; i++)
                {
                    ConstraintWidget widget = mChildren[i];
                    if (widget is Barrier)
                    {
                        ((Barrier) widget).markWidgets();
                    }
                }
            }

            widgetsToAdd.Clear();
            for (int i = 0; i < count; i++)
            {
                ConstraintWidget widget = mChildren[i];
                if (widget.addFirst())
                {
                    if (widget is VirtualLayout)
                    {
                        widgetsToAdd.Add(widget);
                    }
                    else
                    {
                        widget.addToSolver(system, optimize);
                    }
                }
            }

            // If we have virtual layouts, we need to add them to the solver in the correct
            // order (in case they reference one another)
            while (widgetsToAdd.Count > 0)
            {
                int numLayouts = widgetsToAdd.Count;
                VirtualLayout layout = null;
                foreach (ConstraintWidget widget in widgetsToAdd)
                {
                    layout = (VirtualLayout) widget;

                    // we'll go through the virtual layouts that references others first, to give
                    // them a shot at setting their constraints.
                    if (layout.contains(widgetsToAdd))
                    {
                        layout.addToSolver(system, optimize);
                        widgetsToAdd.Remove(layout);
                        break;
                    }
                }
                if (numLayouts == widgetsToAdd.Count)
                {
                    // looks we didn't find anymore dependency, let's add everything.
                    foreach (ConstraintWidget widget in widgetsToAdd)
                    {
                        widget.addToSolver(system, optimize);
                    }
                    widgetsToAdd.Clear();
                }
            }

            if (LinearSystem.USE_DEPENDENCY_ORDERING)
            {
                HashSet<ConstraintWidget> widgetsToAdd = new HashSet<ConstraintWidget>();
                for (int i = 0; i < count; i++)
                {
                    ConstraintWidget widget = mChildren[i];
                    if (!widget.addFirst())
                    {
                        widgetsToAdd.Add(widget);
                    }
                }
                int orientation = VERTICAL;
                if (HorizontalDimensionBehaviour == WRAP_CONTENT)
                {
                    orientation = HORIZONTAL;
                }
                addChildrenToSolverByDependency(this, system, widgetsToAdd, orientation, false);
                foreach (ConstraintWidget widget in widgetsToAdd)
                {
                    Optimizer.checkMatchParent(this, system, widget);
                    widget.addToSolver(system, optimize);
                }
            }
            else
            {

                for (int i = 0; i < count; i++)
                {
                    ConstraintWidget widget = mChildren[i];
                    if (widget is ConstraintWidgetContainer)
                    {
                        DimensionBehaviour horizontalBehaviour = widget.mListDimensionBehaviors[DIMENSION_HORIZONTAL];
                        DimensionBehaviour verticalBehaviour = widget.mListDimensionBehaviors[DIMENSION_VERTICAL];
                        if (horizontalBehaviour == WRAP_CONTENT)
                        {
                            widget.HorizontalDimensionBehaviour = FIXED;
                        }
                        if (verticalBehaviour == WRAP_CONTENT)
                        {
                            widget.VerticalDimensionBehaviour = FIXED;
                        }
                        widget.addToSolver(system, optimize);
                        if (horizontalBehaviour == WRAP_CONTENT)
                        {
                            widget.HorizontalDimensionBehaviour = horizontalBehaviour;
                        }
                        if (verticalBehaviour == WRAP_CONTENT)
                        {
                            widget.VerticalDimensionBehaviour = verticalBehaviour;
                        }
                    }
                    else
                    {
                        Optimizer.checkMatchParent(this, system, widget);
                        if (!(widget.addFirst()))
                        {
                            widget.addToSolver(system, optimize);
                        }
                    }
                }
            }

            if (mHorizontalChainsSize > 0)
            {
                Chain.applyChainConstraints(this, system, null, HORIZONTAL);
            }
            if (mVerticalChainsSize > 0)
            {
                Chain.applyChainConstraints(this, system, null, VERTICAL);
            }
            return true;
        }

        /// <summary>
        /// Update the frame of the layout and its children from the solver
        /// </summary>
        /// <param name="system"> the solver we get the values from. </param>
        public virtual bool updateChildrenFromSolver(LinearSystem system, bool[] flags)
        {
            flags[Optimizer.FLAG_RECOMPUTE_BOUNDS] = false;
            bool optimize = optimizeFor(Optimizer.OPTIMIZATION_GRAPH);
            updateFromSolver(system, optimize);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildren.size();
            int count = mChildren.Count;
            bool hasOverride = false;
            for (int i = 0; i < count; i++)
            {
                ConstraintWidget widget = mChildren[i];
                widget.updateFromSolver(system, optimize);
                if (widget.hasDimensionOverride())
                {
                    hasOverride = true;
                }
            }
            return hasOverride;
        }

        public override void updateFromRuns(bool updateHorizontal, bool updateVertical)
        {
            base.updateFromRuns(updateHorizontal, updateVertical);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildren.size();
            int count = mChildren.Count;
            for (int i = 0; i < count; i++)
            {
                ConstraintWidget widget = mChildren[i];
                widget.updateFromRuns(updateHorizontal, updateVertical);
            }
        }

        /// <summary>
        /// Set the padding on this container. It will apply to the position of the children.
        /// </summary>
        /// <param name="left">   left padding </param>
        /// <param name="top">    top padding </param>
        /// <param name="right">  right padding </param>
        /// <param name="bottom"> bottom padding </param>
        public virtual void setPadding(int left, int top, int right, int bottom)
        {
            mPaddingLeft = left;
            mPaddingTop = top;
            mPaddingRight = right;
            mPaddingBottom = bottom;
        }

        /// <summary>
        /// Set the rtl status. This has implications for Chains.
        /// </summary>
        /// <param name="isRtl"> true if we are in RTL. </param>
        public virtual bool Rtl
        {
            set
            {
                mIsRtl = value;
            }
            get
            {
                return mIsRtl;
            }
        }


        /*-----------------------------------------------------------------------*/
        // Overloaded methods from ConstraintWidget
        /*-----------------------------------------------------------------------*/

        public BasicMeasure.Measure mMeasure = new BasicMeasure.Measure();

        public static bool measure(int level, ConstraintWidget widget, BasicMeasure.Measurer measurer, BasicMeasure.Measure measure, int measureStrategy)
        {
            if (DEBUG)
            {
                Console.WriteLine(Direct.ls(level) + "(M) call to measure " + widget.DebugName);
            }
            if (measurer == null)
            {
                return false;
            }
            if (widget.Visibility == GONE || widget is Guideline || widget is Barrier)
            {
                if (DEBUG)
                {
                    Console.WriteLine(Direct.ls(level) + "(M) no measure needed for " + widget.DebugName);
                }
                measure.measuredWidth = 0;
                measure.measuredHeight = 0;
                return false;
            }

            measure.horizontalBehavior = widget.HorizontalDimensionBehaviour;
            measure.verticalBehavior = widget.VerticalDimensionBehaviour;
            measure.horizontalDimension = widget.Width;
            measure.verticalDimension = widget.Height;
            measure.measuredNeedsSolverPass = false;
            measure.measureStrategy = measureStrategy;

            bool horizontalMatchConstraints = (measure.horizontalBehavior == DimensionBehaviour.MATCH_CONSTRAINT);
            bool verticalMatchConstraints = (measure.verticalBehavior == DimensionBehaviour.MATCH_CONSTRAINT);

            bool horizontalUseRatio = horizontalMatchConstraints && widget.mDimensionRatio > 0;
            bool verticalUseRatio = verticalMatchConstraints && widget.mDimensionRatio > 0;

            if (horizontalMatchConstraints && widget.hasDanglingDimension(HORIZONTAL) && widget.mMatchConstraintDefaultWidth == MATCH_CONSTRAINT_SPREAD && !horizontalUseRatio)
            {
                horizontalMatchConstraints = false;
                measure.horizontalBehavior = WRAP_CONTENT;
                if (verticalMatchConstraints && widget.mMatchConstraintDefaultHeight == MATCH_CONSTRAINT_SPREAD)
                {
                    // if match x match, size would be zero.
                    measure.horizontalBehavior = FIXED;
                }
            }

            if (verticalMatchConstraints && widget.hasDanglingDimension(VERTICAL) && widget.mMatchConstraintDefaultHeight == MATCH_CONSTRAINT_SPREAD && !verticalUseRatio)
            {
                verticalMatchConstraints = false;
                measure.verticalBehavior = WRAP_CONTENT;
                if (horizontalMatchConstraints && widget.mMatchConstraintDefaultWidth == MATCH_CONSTRAINT_SPREAD)
                {
                    // if match x match, size would be zero.
                    measure.verticalBehavior = FIXED;
                }
            }

            if (widget.ResolvedHorizontally)
            {
                horizontalMatchConstraints = false;
                measure.horizontalBehavior = FIXED;
            }
            if (widget.ResolvedVertically)
            {
                verticalMatchConstraints = false;
                measure.verticalBehavior = FIXED;
            }

            if (horizontalUseRatio)
            {
                if (widget.mResolvedMatchConstraintDefault[HORIZONTAL] == ConstraintWidget.MATCH_CONSTRAINT_RATIO_RESOLVED)
                {
                    measure.horizontalBehavior = FIXED;
                }
                else if (!verticalMatchConstraints)
                {
                    // let's measure here
                    int measuredHeight;
                    if (measure.verticalBehavior == FIXED)
                    {
                        measuredHeight = measure.verticalDimension;
                    }
                    else
                    {
                        measure.horizontalBehavior = WRAP_CONTENT;
                        measurer.measure(widget, measure);
                        measuredHeight = measure.measuredHeight;
                    }
                    measure.horizontalBehavior = FIXED;
                    // regardless of which side we are using for the ratio, getDimensionRatio() already
                    // made sure that it's expressed in WxH format, so we can simply go and multiply
                    measure.horizontalDimension = (int)(widget.getDimensionRatio() * measuredHeight);
                    if (DEBUG)
                    {
                        Console.WriteLine("(M) Measured once for ratio on horizontal side...");
                    }
                }
            }
            if (verticalUseRatio)
            {
                if (widget.mResolvedMatchConstraintDefault[VERTICAL] == ConstraintWidget.MATCH_CONSTRAINT_RATIO_RESOLVED)
                {
                    measure.verticalBehavior = FIXED;
                }
                else if (!horizontalMatchConstraints)
                {
                    // let's measure here
                    int measuredWidth;
                    if (measure.horizontalBehavior == FIXED)
                    {
                        measuredWidth = measure.horizontalDimension;
                    }
                    else
                    {
                        measure.verticalBehavior = WRAP_CONTENT;
                        measurer.measure(widget, measure);
                        measuredWidth = measure.measuredWidth;
                    }
                    measure.verticalBehavior = FIXED;
                    if (widget.DimensionRatioSide == -1)
                    {
                        // regardless of which side we are using for the ratio, getDimensionRatio() already
                        // made sure that it's expressed in WxH format, so we can simply go and divide
                        measure.verticalDimension = (int)(measuredWidth / widget.getDimensionRatio());
                    }
                    else
                    {
                        // getDimensionRatio() already got reverted, so we can simply multiply
                        measure.verticalDimension = (int)(widget.getDimensionRatio() * measuredWidth);
                    }
                    if (DEBUG)
                    {
                        Console.WriteLine("(M) Measured once for ratio on vertical side...");
                    }
                }
            }

            measurer.measure(widget, measure);
            widget.Width = measure.measuredWidth;
            widget.Height = measure.measuredHeight;
            widget.HasBaseline = measure.measuredHasBaseline;
            widget.BaselineDistance = measure.measuredBaseline;
            measure.measureStrategy = BasicMeasure.Measure.SELF_DIMENSIONS;
            if (DEBUG)
            {
                Console.WriteLine("(M) Measured " + widget.DebugName + " with : " + widget.HorizontalDimensionBehaviour + " x " + widget.VerticalDimensionBehaviour + " => " + widget.Width + " x " + widget.Height);
            }
            return measure.measuredNeedsSolverPass;
        }

        internal static int myCounter = 0;

        /// <summary>
        /// Layout the tree of widgets
        /// </summary>
        public override void layout()
        {
            if (DEBUG)
            {
                Console.WriteLine("\n#####################################");
                Console.WriteLine("##          CL LAYOUT PASS           ##");
                Console.WriteLine("#####################################\n");
                mDebugSolverPassCount = 0;
            }

            mX = 0;
            mY = 0;

            mWidthMeasuredTooSmall = false;
            mHeightMeasuredTooSmall = false;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildren.size();
            int count = mChildren.Count;

            int preW = Math.Max(0, Width);
            int preH = Math.Max(0, Height);
            DimensionBehaviour originalVerticalDimensionBehaviour = mListDimensionBehaviors[DIMENSION_VERTICAL];
            DimensionBehaviour originalHorizontalDimensionBehaviour = mListDimensionBehaviors[DIMENSION_HORIZONTAL];

            if (DEBUG_LAYOUT)
            {
                Console.WriteLine("layout with preW: " + preW + " (" + mListDimensionBehaviors[DIMENSION_HORIZONTAL] + ") preH: " + preH + " (" + mListDimensionBehaviors[DIMENSION_VERTICAL] + ")");
            }

            if (mMetrics != null)
            {
                mMetrics.layouts++;
            }


            bool wrap_override = false;

            if (FULL_DEBUG)
            {
                Console.WriteLine("OPTIMIZATION LEVEL " + mOptimizationLevel);
            }

            // Only try the direct optimization in the first layout pass
            if (pass == 0 && Optimizer.enabled(mOptimizationLevel, Optimizer.OPTIMIZATION_DIRECT))
            {
                if (FULL_DEBUG)
                {
                    Console.WriteLine("Direct pass " + myCounter++);
                }
                Direct.solvingPass(this, Measurer);
                if (FULL_DEBUG)
                {
                    Console.WriteLine("Direct pass done.");
                }
                for (int i = 0; i < count; i++)
                {
                    ConstraintWidget child = mChildren[i];
                    if (FULL_DEBUG)
                    {
                        if (child.InHorizontalChain)
                        {
                            Console.Write("H");
                        }
                        else
                        {
                            Console.Write(" ");
                        }
                        if (child.InVerticalChain)
                        {
                            Console.Write("V");
                        }
                        else
                        {
                            Console.Write(" ");
                        }
                        if (child.ResolvedHorizontally && child.ResolvedVertically)
                        {
                            Console.Write("*");
                        }
                        else
                        {
                            Console.Write(" ");
                        }
                        Console.WriteLine("[" + i + "] child " + child.DebugName + " H: " + child.ResolvedHorizontally + " V: " + child.ResolvedVertically);
                    }
                    if (child.MeasureRequested && !(child is Guideline) && !(child is Barrier) && !(child is VirtualLayout) && !(child.InVirtualLayout))
                    {
                        DimensionBehaviour? widthBehavior = child.getDimensionBehaviour(HORIZONTAL);
                        DimensionBehaviour? heightBehavior = child.getDimensionBehaviour(VERTICAL);

                        bool skip = widthBehavior == DimensionBehaviour.MATCH_CONSTRAINT && child.mMatchConstraintDefaultWidth != MATCH_CONSTRAINT_WRAP && heightBehavior == DimensionBehaviour.MATCH_CONSTRAINT && child.mMatchConstraintDefaultHeight != MATCH_CONSTRAINT_WRAP;
                        if (!skip)
                        {
                            BasicMeasure.Measure measure = new BasicMeasure.Measure();
                            ConstraintWidgetContainer.measure(0, child, mMeasurer, measure, BasicMeasure.Measure.SELF_DIMENSIONS);
                        }
                    }
                }
                // let's measure children
                if (FULL_DEBUG)
                {
                    Console.WriteLine("Direct pass all done.");
                }
            }
            else
            {
                if (FULL_DEBUG)
                {
                    Console.WriteLine("No DIRECT PASS");
                }
            }

            if (count > 2 && (originalHorizontalDimensionBehaviour == WRAP_CONTENT || originalVerticalDimensionBehaviour == WRAP_CONTENT) && (Optimizer.enabled(mOptimizationLevel, Optimizer.OPTIMIZATION_GROUPING)))
            {
                if (Grouping.simpleSolvingPass(this, Measurer))
                {
                    if (originalHorizontalDimensionBehaviour == WRAP_CONTENT)
                    {
                        if (preW < Width && preW > 0)
                        {
                            if (DEBUG_LAYOUT)
                            {
                                Console.WriteLine("Override width " + Width + " to " + preH);
                            }
                            Width = preW;
                            mWidthMeasuredTooSmall = true;
                        }
                        else
                        {
                            preW = Width;
                        }
                    }
                    if (originalVerticalDimensionBehaviour == WRAP_CONTENT)
                    {
                        if (preH < Height && preH > 0)
                        {
                            if (DEBUG_LAYOUT)
                            {
                                Console.WriteLine("Override height " + Height + " to " + preH);
                            }
                            Height = preH;
                            mHeightMeasuredTooSmall = true;
                        }
                        else
                        {
                            preH = Height;
                        }
                    }
                    wrap_override = true;
                    if (DEBUG_LAYOUT)
                    {
                        Console.WriteLine("layout post opt, preW: " + preW + " (" + mListDimensionBehaviors[DIMENSION_HORIZONTAL] + ") preH: " + preH + " (" + mListDimensionBehaviors[DIMENSION_VERTICAL] + "), new size " + Width + " x " + Height);
                    }
                }
            }
            bool useGraphOptimizer = optimizeFor(Optimizer.OPTIMIZATION_GRAPH) || optimizeFor(Optimizer.OPTIMIZATION_GRAPH_WRAP);

            mSystem.graphOptimizer = false;
            mSystem.newgraphOptimizer = false;

            if (mOptimizationLevel != Optimizer.OPTIMIZATION_NONE && useGraphOptimizer)
            {
                mSystem.newgraphOptimizer = true;
            }

            int countSolve = 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<ConstraintWidget> allChildren = mChildren;
            IList<ConstraintWidget> allChildren = mChildren;
            bool hasWrapContent = HorizontalDimensionBehaviour == WRAP_CONTENT || VerticalDimensionBehaviour == WRAP_CONTENT;

            // Reset the chains before iterating on our children
            resetChains();
            countSolve = 0;

            // Before we solve our system, we should call layout() on any
            // of our children that is a container.
            for (int i = 0; i < count; i++)
            {
                ConstraintWidget widget = mChildren[i];
                if (widget is WidgetContainer)
                {
                    ((WidgetContainer) widget).layout();
                }
            }
            bool optimize = optimizeFor(Optimizer.OPTIMIZATION_GRAPH);

            // Now let's solve our system as usual
            bool needsSolving = true;
            while (needsSolving)
            {
                countSolve++;
                try
                {
                    mSystem.reset();
                    resetChains();
                    if (DEBUG)
                    {
                        string debugName = DebugName;
                        if (string.ReferenceEquals(debugName, null))
                        {
                            debugName = "root";
                        }
                        setDebugSolverName(mSystem, debugName);
                        for (int i = 0; i < count; i++)
                        {
                            ConstraintWidget widget = mChildren[i];
                            if (!string.ReferenceEquals(widget.DebugName, null))
                            {
                                widget.setDebugSolverName(mSystem, widget.DebugName);
                            }
                        }
                    }
                    else
                    {
                        createObjectVariables(mSystem);
                        for (int i = 0; i < count; i++)
                        {
                            ConstraintWidget widget = mChildren[i];
                            widget.createObjectVariables(mSystem);
                        }
                    }
                    needsSolving = addChildrenToSolver(mSystem);
                    
                    if(verticalWrapMin != null)
                    {
                        ConstraintAnchor verticalwrapmin;
                        verticalWrapMin.TryGetTarget(out verticalwrapmin);
                        if (verticalwrapmin != null)
                        {
                            addMinWrap(verticalwrapmin, mSystem.createObjectVariable(mTop));
                            verticalWrapMin = null;
                        }
                    }
                    if (verticalWrapMax != null)
                    {
                        ConstraintAnchor verticalwrapmax;
                        verticalWrapMax.TryGetTarget(out verticalwrapmax);
                        if (verticalwrapmax != null)
                        {
                            addMaxWrap(verticalwrapmax, mSystem.createObjectVariable(mBottom));
                            verticalWrapMax = null;
                        }
                    }
                    if(horizontalWrapMin != null)
                    {
                        ConstraintAnchor horizontalwrapmin;
                        horizontalWrapMin.TryGetTarget(out horizontalwrapmin);
                        if (horizontalwrapmin != null)
                        {
                            addMinWrap(horizontalwrapmin, mSystem.createObjectVariable(mLeft));
                            horizontalWrapMin = null;
                        }
                    }
                    
                    if(horizontalWrapMax != null)
                    {
                        ConstraintAnchor horizontalwrapmax;
                        horizontalWrapMax.TryGetTarget(out horizontalwrapmax);
                        if (horizontalwrapmax != null)
                        {
                            addMaxWrap(horizontalwrapmax, mSystem.createObjectVariable(mRight));
                            horizontalWrapMax = null;
                        }
                    }
                    
                    if (needsSolving)
                    {
                        mSystem.minimize();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.Write(e.StackTrace);
                    Console.WriteLine("EXCEPTION : " + e);
                }
                if (needsSolving)
                {
                    needsSolving = updateChildrenFromSolver(mSystem, Optimizer.flags);
                }
                else
                {
                    updateFromSolver(mSystem, optimize);
                    for (int i = 0; i < count; i++)
                    {
                        ConstraintWidget widget = mChildren[i];
                        widget.updateFromSolver(mSystem, optimize);
                    }
                    needsSolving = false;
                }

                if (hasWrapContent && countSolve < MAX_ITERATIONS && Optimizer.flags[Optimizer.FLAG_RECOMPUTE_BOUNDS])
                {
                    // let's get the new bounds
                    int maxX = 0;
                    int maxY = 0;
                    for (int i = 0; i < count; i++)
                    {
                        ConstraintWidget widget = mChildren[i];
                        maxX = Math.Max(maxX, widget.mX + widget.Width);
                        maxY = Math.Max(maxY, widget.mY + widget.Height);
                    }
                    maxX = Math.Max(mMinWidth, maxX);
                    maxY = Math.Max(mMinHeight, maxY);
                    if (originalHorizontalDimensionBehaviour == WRAP_CONTENT)
                    {
                        if (Width < maxX)
                        {
                            if (DEBUG_LAYOUT)
                            {
                                Console.WriteLine("layout override width from " + Width + " vs " + maxX);
                            }
                            Width = maxX;
                            mListDimensionBehaviors[DIMENSION_HORIZONTAL] = WRAP_CONTENT; // force using the solver
                            wrap_override = true;
                            needsSolving = true;
                        }
                    }
                    if (originalVerticalDimensionBehaviour == WRAP_CONTENT)
                    {
                        if (Height < maxY)
                        {
                            if (DEBUG_LAYOUT)
                            {
                                Console.WriteLine("layout override height from " + Height + " vs " + maxY);
                            }
                            Height = maxY;
                            mListDimensionBehaviors[DIMENSION_VERTICAL] = WRAP_CONTENT; // force using the solver
                            wrap_override = true;
                            needsSolving = true;
                        }
                    }
                }
                if (true)
                {
                    int width = Math.Max(mMinWidth, Width);
                    if (width > Width)
                    {
                        if (DEBUG_LAYOUT)
                        {
                            Console.WriteLine("layout override 2, width from " + Width + " vs " + width);
                        }
                        Width = width;
                        mListDimensionBehaviors[DIMENSION_HORIZONTAL] = FIXED;
                        wrap_override = true;
                        needsSolving = true;
                    }
                    int height = Math.Max(mMinHeight, Height);
                    if (height > Height)
                    {
                        if (DEBUG_LAYOUT)
                        {
                            Console.WriteLine("layout override 2, height from " + Height + " vs " + height);
                        }
                        Height = height;
                        mListDimensionBehaviors[DIMENSION_VERTICAL] = FIXED;
                        wrap_override = true;
                        needsSolving = true;
                    }

                    if (!wrap_override)
                    {
                        if (mListDimensionBehaviors[DIMENSION_HORIZONTAL] == WRAP_CONTENT && preW > 0)
                        {
                            if (Width > preW)
                            {
                                if (DEBUG_LAYOUT)
                                {
                                    Console.WriteLine("layout override 3, width from " + Width + " vs " + preW);
                                }
                                mWidthMeasuredTooSmall = true;
                                wrap_override = true;
                                mListDimensionBehaviors[DIMENSION_HORIZONTAL] = FIXED;
                                Width = preW;
                                needsSolving = true;
                            }
                        }
                        if (mListDimensionBehaviors[DIMENSION_VERTICAL] == WRAP_CONTENT && preH > 0)
                        {
                            if (Height > preH)
                            {
                                if (DEBUG_LAYOUT)
                                {
                                    Console.WriteLine("layout override 3, height from " + Height + " vs " + preH);
                                }
                                mHeightMeasuredTooSmall = true;
                                wrap_override = true;
                                mListDimensionBehaviors[DIMENSION_VERTICAL] = FIXED;
                                Height = preH;
                                needsSolving = true;
                            }
                        }
                    }

                    if (countSolve > MAX_ITERATIONS)
                    {
                        needsSolving = false;
                    }
                }
            }
            if (DEBUG_LAYOUT)
            {
                Console.WriteLine("Solved system in " + countSolve + " iterations (" + Width + " x " + Height + ")");
            }

            mChildren = (List<ConstraintWidget>) allChildren;

            if (wrap_override)
            {
                mListDimensionBehaviors[DIMENSION_HORIZONTAL] = originalHorizontalDimensionBehaviour;
                mListDimensionBehaviors[DIMENSION_VERTICAL] = originalVerticalDimensionBehaviour;
            }

            resetSolverVariables(mSystem.Cache);
        }

        /// <summary>
        /// Indicates if the container knows how to layout its content on its own
        /// </summary>
        /// <returns> true if the container does the layout, false otherwise </returns>
        public virtual bool handlesInternalConstraints()
        {
            return false;
        }

        /*-----------------------------------------------------------------------*/
        // Guidelines
        /*-----------------------------------------------------------------------*/

        /// <summary>
        /// Accessor to the vertical guidelines contained in the table.
        /// </summary>
        /// <returns> array of guidelines </returns>
        public virtual List<Guideline> VerticalGuidelines
        {
            get
            {
                List<Guideline> guidelines = new List<Guideline>();
                for (int i = 0, mChildrenSize = mChildren.Count; i < mChildrenSize; i++)
                {
    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
    //ORIGINAL LINE: final ConstraintWidget widget = mChildren.get(i);
                    ConstraintWidget widget = mChildren[i];
                    if (widget is Guideline)
                    {
                        Guideline guideline = (Guideline) widget;
                        if (guideline.Orientation == Guideline.VERTICAL)
                        {
                            guidelines.Add(guideline);
                        }
                    }
                }
                return guidelines;
            }
        }

        /// <summary>
        /// Accessor to the horizontal guidelines contained in the table.
        /// </summary>
        /// <returns> array of guidelines </returns>
        public virtual List<Guideline> HorizontalGuidelines
        {
            get
            {
                List<Guideline> guidelines = new List<Guideline>();
                for (int i = 0, mChildrenSize = mChildren.Count; i < mChildrenSize; i++)
                {
    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
    //ORIGINAL LINE: final ConstraintWidget widget = mChildren.get(i);
                    ConstraintWidget widget = mChildren[i];
                    if (widget is Guideline)
                    {
                        Guideline guideline = (Guideline) widget;
                        if (guideline.Orientation == Guideline.HORIZONTAL)
                        {
                            guidelines.Add(guideline);
                        }
                    }
                }
                return guidelines;
            }
        }

        public virtual LinearSystem System
        {
            get
            {
                return mSystem;
            }
        }

        /*-----------------------------------------------------------------------*/
        // Chains
        /*-----------------------------------------------------------------------*/

        /// <summary>
        /// Reset the chains array. Need to be called before layout.
        /// </summary>
        private void resetChains()
        {
            mHorizontalChainsSize = 0;
            mVerticalChainsSize = 0;
        }

        /// <summary>
        /// Add the chain which constraintWidget is part of. Called by ConstraintWidget::addToSolver()
        /// </summary>
        /// <param name="constraintWidget"> </param>
        /// <param name="type">             HORIZONTAL or VERTICAL chain </param>
        internal virtual void addChain(ConstraintWidget constraintWidget, int type)
        {
            ConstraintWidget widget = constraintWidget;
            if (type == HORIZONTAL)
            {
                addHorizontalChain(widget);
            }
            else if (type == VERTICAL)
            {
                addVerticalChain(widget);
            }
        }

        /// <summary>
        /// Add a widget to the list of horizontal chains. The widget is the left-most widget
        /// of the chain which doesn't have a left dual connection.
        /// </summary>
        /// <param name="widget"> widget starting the chain </param>
        private void addHorizontalChain(ConstraintWidget widget)
        {
            if (mHorizontalChainsSize + 1 >= mHorizontalChainsArray.Length)
            {
                mHorizontalChainsArray = mHorizontalChainsArray.Copy<ChainHead>(mHorizontalChainsArray.Length * 2);
            }
            mHorizontalChainsArray[mHorizontalChainsSize] = new ChainHead(widget, HORIZONTAL, Rtl);
            mHorizontalChainsSize++;
        }

        /// <summary>
        /// Add a widget to the list of vertical chains. The widget is the top-most widget
        /// of the chain which doesn't have a top dual connection.
        /// </summary>
        /// <param name="widget"> widget starting the chain </param>
        private void addVerticalChain(ConstraintWidget widget)
        {
            if (mVerticalChainsSize + 1 >= mVerticalChainsArray.Length)
            {
                mVerticalChainsArray = mVerticalChainsArray.Copy<ChainHead>(mVerticalChainsArray.Length * 2);
            }
            mVerticalChainsArray[mVerticalChainsSize] = new ChainHead(widget, VERTICAL, Rtl);
            mVerticalChainsSize++;
        }

        /// <summary>
        /// Keep track of the # of passes </summary>
        /// <param name="pass"> </param>
        public virtual int Pass
        {
            set
            {
                this.pass = value;
            }
        }
    }

}
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using Windows.Foundation;

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
using View = Microsoft.UI.Xaml.FrameworkElement;
using GuidelineView = SharpConstraintLayout.Maui.Core.Guideline;
using System.Diagnostics;

namespace SharpConstraintLayout.Maui.Core
{
    using LinearSystem = androidx.constraintlayout.core.LinearSystem;
    using Metrics = androidx.constraintlayout.core.Metrics;
    using ConstraintAnchor = androidx.constraintlayout.core.widgets.ConstraintAnchor;
    using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;
    using ConstraintWidgetContainer = androidx.constraintlayout.core.widgets.ConstraintWidgetContainer;
    using GuidelineWidget = androidx.constraintlayout.core.widgets.Guideline;
    using Optimizer = androidx.constraintlayout.core.widgets.Optimizer;
    using BasicMeasure = androidx.constraintlayout.core.widgets.analyzer.BasicMeasure;
    using static SharpConstraintLayout.Maui.Core.ConstraintSet;

    public class ConstraintLayout : Panel
    {
        private bool InstanceFieldsInitialized = false;

        private void InitializeInstanceFields()
        {
            mMeasurer = new Measurer(this, this);
        }

        /// <summary>
        /// @suppress
        /// </summary>
        public const string VERSION = "ConstraintLayout-2.1.1";
        private const string TAG = "ConstraintLayout";

        private const bool USE_CONSTRAINTS_HELPER = true;
        private const bool DEBUG = LinearSystem.FULL_DEBUG;
        private const bool DEBUG_DRAW_CONSTRAINTS = false;
        private const bool MEASURE = false;
        private const bool OPTIMIZE_HEIGHT_CHANGE = false;

        internal Dictionary<int, View> mChildrenByIds = new Dictionary<int, View>();
        private Dictionary<int, ConstraintWidget> mapToWidget = new Dictionary<int, ConstraintWidget>();
        // This array keep a list of helper objects if they are present
        private List<ConstraintHelper> mConstraintHelpers = new List<ConstraintHelper>(4);

        protected internal ConstraintWidgetContainer mLayoutWidget = new ConstraintWidgetContainer();

        private int mMinWidth = 0;
        private int mMinHeight = 0;
        private int mMaxWidth = int.MaxValue;
        private int mMaxHeight = int.MaxValue;

        protected internal bool mDirtyHierarchy = true;
        private int mOptimizationLevel = Optimizer.OPTIMIZATION_STANDARD;
        private ConstraintSet mConstraintSet = null;
        protected internal ConstraintLayoutStates mConstraintLayoutSpec = null;

        private int mConstraintSetId = -1;

        private Dictionary<string, int?> mDesignIds = new Dictionary<string, int?>();

        // Cache last measure
        private int mLastMeasureWidth = -1;
        private int mLastMeasureHeight = -1;
        internal int mLastMeasureWidthSize = -1;
        internal int mLastMeasureHeightSize = -1;
        internal int mLastMeasureWidthMode = MeasureSpec.UNSPECIFIED;
        internal int mLastMeasureHeightMode = MeasureSpec.UNSPECIFIED;
        private Dictionary<int, ConstraintWidget> mTempMapIdToWidget = new Dictionary<int, ConstraintWidget>();

        /// <summary>
        /// @suppress
        /// </summary>
        public const int DESIGN_INFO_ID = 0;
        private ConstraintsChangedListener mConstraintsChangedListener;
        private Metrics mMetrics;

        /* private static SharedValues sSharedValues = null;

         /// <summary>
         /// Returns the SharedValues instance, creating it if it doesn't exist.
         /// </summary>
         /// <returns> the SharedValues instance </returns>
         public static SharedValues SharedValues
         {
             get
             {
                 if (sSharedValues == null)
                 {
                     sSharedValues = new SharedValues();
                 }
                 return sSharedValues;
             }
         }

         /// <summary>
         /// @suppress
         /// </summary>
         public virtual void setDesignInformation(int type, object value1, object value2)
         {
             if (type == DESIGN_INFO_ID && value1 is string && value2 is int?)
             {
                 if (mDesignIds == null)
                 {
                     mDesignIds = new Dictionary<>();
                 }
                 string name = (string)value1;
                 int index = name.IndexOf("/", StringComparison.Ordinal);
                 if (index != -1)
                 {
                     name = name.Substring(index + 1);
                 }
                 int id = (int?)value2;
                 mDesignIds[name] = id;
             }
         }

         /// <summary>
         /// @suppress
         /// </summary>
         public virtual object getDesignInformation(int type, object value)
         {
             if (type == DESIGN_INFO_ID && value is string)
             {
                 string name = (string)value;
                 if (mDesignIds != null && mDesignIds.ContainsKey(name))
                 {
                     return mDesignIds[name];
                 }
             }
             return null;
         }*/

        //ORIGINAL LINE: public ConstraintLayout(@NonNull android.content.Context context)
        public ConstraintLayout() : base()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
            init();
        }

        /*//ORIGINAL LINE: public ConstraintLayout(@NonNull android.content.Context context, @Nullable android.util.AttributeSet attrs)
        public ConstraintLayout(Context context, AttributeSet attrs) : base(context, attrs)
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
            init(attrs, 0, 0);
        }

        //ORIGINAL LINE: public ConstraintLayout(@NonNull android.content.Context context, @Nullable android.util.AttributeSet attrs, int defStyleAttr)
        public ConstraintLayout(Context context, AttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
            init(attrs, defStyleAttr, 0);
        }

        //ORIGINAL LINE: @TargetApi(android.os.Build.VERSION_CODES.LOLLIPOP) public ConstraintLayout(@NonNull android.content.Context context, @Nullable android.util.AttributeSet attrs, int defStyleAttr, int defStyleRes)
        public ConstraintLayout(Context context, AttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
            init(attrs, defStyleAttr, defStyleRes);
        }*/

        int id = -1;

        /// <summary>
        /// @suppress
        /// </summary>
        public int Id
        {
            set
            {
                mChildrenByIds.Remove(id);
                id = value;
                mChildrenByIds.Add(id, this);
            }
            get
            {
                if (id == -1)
                    id = GetHashCode();
                return id;
            }
        }

        // -------------------------------------------------------------------------------------------
        // Measure widgets callbacks
        // -------------------------------------------------------------------------------------------


        // -------------------------------------------------------------------------------------------

        internal class Measurer : BasicMeasure.Measurer
        {
            private readonly ConstraintLayout outerInstance;

            internal ConstraintLayout layout;
            internal int paddingTop;
            internal int paddingBottom;
            internal int paddingWidth;
            internal int paddingHeight;
            internal int layoutWidthSpec;
            internal int layoutHeightSpec;

            public virtual void captureLayoutInfo(int widthSpec, int heightSpec, int top, int bottom, int width, int height)
            {
                paddingTop = top;
                paddingBottom = bottom;
                paddingWidth = width;
                paddingHeight = height;
                layoutWidthSpec = widthSpec;
                layoutHeightSpec = heightSpec;
            }

            public Measurer(ConstraintLayout outerInstance, ConstraintLayout l)
            {
                this.outerInstance = outerInstance;
                layout = l;
            }

            //ORIGINAL LINE: @SuppressLint("WrongCall") @Override public final void measure(androidx.constraintlayout.core.widgets.ConstraintWidget widget, androidx.constraintlayout.core.widgets.analyzer.BasicMeasure.Measure measure)
            public void measure(ConstraintWidget widget, BasicMeasure.Measure measure)
            {
                if (widget == null)
                {
                    return;
                }
                if (widget.Visibility == GONE && !widget.InPlaceholder)
                {
                    measure.measuredWidth = 0;
                    measure.measuredHeight = 0;
                    measure.measuredBaseline = 0;
                    return;
                }
                if (widget.Parent == null)
                {
                    return;
                }

                long startMeasure;
                long endMeasure;

                if (MEASURE)
                {
                    //startMeasure = System.nanoTime();
                    startMeasure = DateTimeHelperClass.nanoTime();
                }

                ConstraintWidget.DimensionBehaviour horizontalBehavior = measure.horizontalBehavior;
                ConstraintWidget.DimensionBehaviour verticalBehavior = measure.verticalBehavior;

                int horizontalDimension = measure.horizontalDimension;
                int verticalDimension = measure.verticalDimension;

                int horizontalSpec = 0;
                int verticalSpec = 0;

                int heightPadding = paddingTop + paddingBottom;
                int widthPadding = paddingWidth;

                View child = (View)widget.CompanionWidget;

                switch (horizontalBehavior)
                {
                    case ConstraintWidget.DimensionBehaviour.FIXED:
                        {
                            horizontalSpec = MeasureSpec.makeMeasureSpec(horizontalDimension, MeasureSpec.EXACTLY);
                        }
                        break;
                    case ConstraintWidget.DimensionBehaviour.WRAP_CONTENT:
                        {
                            horizontalSpec = getChildMeasureSpec(layoutWidthSpec, widthPadding, WRAP_CONTENT);
                        }
                        break;
                    case ConstraintWidget.DimensionBehaviour.MATCH_PARENT:
                        {
                            // Horizontal spec must account for margin as well as padding here.
                            horizontalSpec = getChildMeasureSpec(layoutWidthSpec, widthPadding + widget.HorizontalMargin, LayoutParams.MATCH_PARENT);
                        }
                        break;
                    case ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT:
                        {
                            horizontalSpec = getChildMeasureSpec(layoutWidthSpec, widthPadding, WRAP_CONTENT);
                            bool shouldDoWrap = widget.mMatchConstraintDefaultWidth == MATCH_CONSTRAINT_WRAP;
                            if (measure.measureStrategy == BasicMeasure.Measure.TRY_GIVEN_DIMENSIONS || measure.measureStrategy == BasicMeasure.Measure.USE_GIVEN_DIMENSIONS)
                            {
                                // the solver gives us our new dimension, but if we previously had it measured with
                                // a wrap, it can be incorrect if the other side was also variable.
                                // So in that case, we have to double-check the other side is stable (else we can't
                                // just assume the wrap value will be correct).
                                bool otherDimensionStable = child.MeasuredHeight == widget.Height;
                                bool useCurrent = measure.measureStrategy == BasicMeasure.Measure.USE_GIVEN_DIMENSIONS || !shouldDoWrap || (shouldDoWrap && otherDimensionStable) || (child is Placeholder) || (widget.ResolvedHorizontally);
                                if (useCurrent)
                                {
                                    horizontalSpec = MeasureSpec.makeMeasureSpec(widget.Width, MeasureSpec.EXACTLY);
                                }
                            }
                        }
                        break;
                }

                switch (verticalBehavior)
                {
                    case ConstraintWidget.DimensionBehaviour.FIXED:
                        {
                            verticalSpec = MeasureSpec.makeMeasureSpec(verticalDimension, MeasureSpec.EXACTLY);
                        }
                        break;
                    case ConstraintWidget.DimensionBehaviour.WRAP_CONTENT:
                        {
                            verticalSpec = getChildMeasureSpec(layoutHeightSpec, heightPadding, WRAP_CONTENT);
                        }
                        break;
                    case ConstraintWidget.DimensionBehaviour.MATCH_PARENT:
                        {
                            // Vertical spec must account for margin as well as padding here.
                            verticalSpec = getChildMeasureSpec(layoutHeightSpec, heightPadding + widget.VerticalMargin, LayoutParams.MATCH_PARENT);
                        }
                        break;
                    case ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT:
                        {
                            verticalSpec = getChildMeasureSpec(layoutHeightSpec, heightPadding, WRAP_CONTENT);
                            bool shouldDoWrap = widget.mMatchConstraintDefaultHeight == MATCH_CONSTRAINT_WRAP;
                            if (measure.measureStrategy == BasicMeasure.Measure.TRY_GIVEN_DIMENSIONS || measure.measureStrategy == BasicMeasure.Measure.USE_GIVEN_DIMENSIONS)
                            {
                                // the solver gives us our new dimension, but if we previously had it measured with
                                // a wrap, it can be incorrect if the other side was also variable.
                                // So in that case, we have to double-check the other side is stable (else we can't
                                // just assume the wrap value will be correct).
                                bool otherDimensionStable = child.MeasuredWidth == widget.Width;
                                bool useCurrent = measure.measureStrategy == BasicMeasure.Measure.USE_GIVEN_DIMENSIONS || !shouldDoWrap || (shouldDoWrap && otherDimensionStable) || (child is Placeholder) || (widget.ResolvedVertically);
                                if (useCurrent)
                                {
                                    verticalSpec = MeasureSpec.makeMeasureSpec(widget.Height, MeasureSpec.EXACTLY);
                                }
                            }
                        }
                        break;
                }

                ConstraintWidgetContainer container = (ConstraintWidgetContainer)widget.Parent;
                if (container != null && Optimizer.enabled(outerInstance.mOptimizationLevel, Optimizer.OPTIMIZATION_CACHE_MEASURES))
                {
                    if (child.MeasuredWidth == widget.Width && child.MeasuredWidth < container.Width && child.MeasuredHeight == widget.Height && child.MeasuredHeight < container.Height && child.Baseline == widget.BaselineDistance && !widget.MeasureRequested)
                    // note: the container check replicates legacy behavior, but we might want
                    // to not enforce that in 3.0
                    {
                        bool similar = isSimilarSpec(widget.LastHorizontalMeasureSpec, horizontalSpec, widget.Width) && isSimilarSpec(widget.LastVerticalMeasureSpec, verticalSpec, widget.Height);
                        if (similar)
                        {
                            measure.measuredWidth = widget.Width;
                            measure.measuredHeight = widget.Height;
                            measure.measuredBaseline = widget.BaselineDistance;
                            // if the dimensions of the solver widget are already the same as the real view, no need to remeasure.
                            if (DEBUG)
                            {
                                Console.WriteLine("SKIPPED " + widget);
                            }
                            return;
                        }
                    }
                }

                bool horizontalMatchConstraints = (horizontalBehavior == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT);
                bool verticalMatchConstraints = (verticalBehavior == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT);

                bool verticalDimensionKnown = (verticalBehavior == ConstraintWidget.DimensionBehaviour.MATCH_PARENT || verticalBehavior == ConstraintWidget.DimensionBehaviour.FIXED);
                bool horizontalDimensionKnown = (horizontalBehavior == ConstraintWidget.DimensionBehaviour.MATCH_PARENT || horizontalBehavior == ConstraintWidget.DimensionBehaviour.FIXED);
                bool horizontalUseRatio = horizontalMatchConstraints && widget.mDimensionRatio > 0;
                bool verticalUseRatio = verticalMatchConstraints && widget.mDimensionRatio > 0;

                if (child == null)
                {
                    return;
                }
                LayoutParams @params = (LayoutParams)child.LayoutParams;

                int width = 0;
                int height = 0;
                int baseline = 0;

                if ((measure.measureStrategy == BasicMeasure.Measure.TRY_GIVEN_DIMENSIONS || measure.measureStrategy == BasicMeasure.Measure.USE_GIVEN_DIMENSIONS) || !(horizontalMatchConstraints && widget.mMatchConstraintDefaultWidth == MATCH_CONSTRAINT_SPREAD && verticalMatchConstraints && widget.mMatchConstraintDefaultHeight == MATCH_CONSTRAINT_SPREAD))
                {

                    if (child is VirtualLayout && widget is androidx.constraintlayout.core.widgets.VirtualLayout)
                    {
                        androidx.constraintlayout.core.widgets.VirtualLayout layout = (androidx.constraintlayout.core.widgets.VirtualLayout)widget;
                        ((VirtualLayout)child).onMeasure(layout, horizontalSpec, verticalSpec);
                    }
                    else
                    {
                        child.measure(horizontalSpec, verticalSpec);
                    }
                    widget.setLastMeasureSpec(horizontalSpec, verticalSpec);

                    int w = child.MeasuredWidth;
                    int h = child.MeasuredHeight;
                    baseline = child.Baseline;

                    width = w;
                    height = h;

                    if (DEBUG)
                    {
                        string measurement = MeasureSpec.ToString(horizontalSpec) + " x " + MeasureSpec.ToString(verticalSpec) + " => " + width + " x " + height;
                        Console.WriteLine("    (M) measure " + " (" + widget.DebugName + ") : " + measurement);
                    }

                    if (widget.mMatchConstraintMinWidth > 0)
                    {
                        width = Math.Max(widget.mMatchConstraintMinWidth, width);
                    }
                    if (widget.mMatchConstraintMaxWidth > 0)
                    {
                        width = Math.Min(widget.mMatchConstraintMaxWidth, width);
                    }
                    if (widget.mMatchConstraintMinHeight > 0)
                    {
                        height = Math.Max(widget.mMatchConstraintMinHeight, height);
                    }
                    if (widget.mMatchConstraintMaxHeight > 0)
                    {
                        height = Math.Min(widget.mMatchConstraintMaxHeight, height);
                    }

                    bool optimizeDirect = Optimizer.enabled(outerInstance.mOptimizationLevel, Optimizer.OPTIMIZATION_DIRECT);
                    if (!optimizeDirect)
                    {
                        if (horizontalUseRatio && verticalDimensionKnown)
                        {
                            float ratio = widget.mDimensionRatio;
                            width = (int)(0.5f + height * ratio);
                        }
                        else if (verticalUseRatio && horizontalDimensionKnown)
                        {
                            float ratio = widget.mDimensionRatio;
                            height = (int)(0.5f + width / ratio);
                        }
                    }

                    if (w != width || h != height)
                    {
                        if (w != width)
                        {
                            horizontalSpec = MeasureSpec.makeMeasureSpec(width, MeasureSpec.EXACTLY);
                        }
                        if (h != height)
                        {
                            verticalSpec = MeasureSpec.makeMeasureSpec(height, MeasureSpec.EXACTLY);
                        }
                        child.measure(horizontalSpec, verticalSpec);

                        widget.setLastMeasureSpec(horizontalSpec, verticalSpec);
                        width = child.MeasuredWidth;
                        height = child.MeasuredHeight;
                        baseline = child.Baseline;
                        if (DEBUG)
                        {
                            string measurement2 = MeasureSpec.ToString(horizontalSpec) + " x " + MeasureSpec.ToString(verticalSpec) + " => " + width + " x " + height;
                            Console.WriteLine("measure (b) " + widget.DebugName + " : " + measurement2);
                        }
                    }

                }

                bool hasBaseline = baseline != -1;

                measure.measuredNeedsSolverPass = (width != measure.horizontalDimension) || (height != measure.verticalDimension);
                if (@params.needsBaseline)
                {
                    hasBaseline = true;
                }
                if (hasBaseline && baseline != -1 && widget.BaselineDistance != baseline)
                {
                    measure.measuredNeedsSolverPass = true;
                }
                measure.measuredWidth = width;
                measure.measuredHeight = height;
                measure.measuredHasBaseline = hasBaseline;
                measure.measuredBaseline = baseline;
                if (MEASURE)
                {
                    endMeasure = System.nanoTime();
                    if (outerInstance.mMetrics != null)
                    {
                        outerInstance.mMetrics.measuresWidgetsDuration += (endMeasure - startMeasure);
                    }
                }
            }

            /// <summary>
            /// Returns true if the previous measure spec is equivalent to the new one.
            /// - if it's the same...
            /// - if it's not, but the previous was AT_MOST or UNSPECIFIED and the new one
            ///   is EXACTLY with the same size.
            /// </summary>
            /// <param name="lastMeasureSpec"> </param>
            /// <param name="spec"> </param>
            /// <param name="widgetSize">
            /// @return </param>
            internal virtual bool isSimilarSpec(int lastMeasureSpec, int spec, int widgetSize)
            {
                if (lastMeasureSpec == spec)
                {
                    return true;
                }
                int lastMode = MeasureSpec.getMode(lastMeasureSpec);
                int lastSize = MeasureSpec.getSize(lastMeasureSpec);
                int mode = MeasureSpec.getMode(spec);
                int size = MeasureSpec.getSize(spec);
                if (mode == MeasureSpec.EXACTLY && (lastMode == MeasureSpec.AT_MOST || lastMode == MeasureSpec.UNSPECIFIED) && widgetSize == size)
                {
                    return true;
                }
                return false;
            }

            public void didMeasures()
            {
                //ORIGINAL LINE: final int widgetsCount = layout.getChildCount();
                int widgetsCount = layout.ChildCount;
                for (int i = 0; i < widgetsCount; i++)
                {
                    //ORIGINAL LINE: final android.view.View child = layout.getChildAt(i);
                    View child = (View)layout.Children[i];
                    if (child is Placeholder)
                    {
                        ((Placeholder)child).updatePostMeasure(layout);
                    }
                }
                // TODO refactor into an updatePostMeasure interface
                //ORIGINAL LINE: final int helperCount = layout.mConstraintHelpers.size();
                int helperCount = layout.mConstraintHelpers.Count;
                if (helperCount > 0)
                {
                    for (int i = 0; i < helperCount; i++)
                    {
                        ConstraintHelper helper = layout.mConstraintHelpers[i];
                        helper.updatePostMeasure(layout);
                    }
                }
            }
        }

        internal Measurer mMeasurer;

        private void init()
        {
            mLayoutWidget.CompanionWidget = this;
            mLayoutWidget.Measurer = mMeasurer;
            mChildrenByIds.Add(Id, this);
            mConstraintSet = null;
            /*if (attrs != null)
            {
                TypedArray a = Context.obtainStyledAttributes(attrs, R.styleable.ConstraintLayout_Layout, defStyleAttr, defStyleRes);
                //JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final int N = a.getIndexCount();
                int N = a.IndexCount;
                for (int i = 0; i < N; i++)
                {
                    int attr = a.getIndex(i);
                    if (attr == R.styleable.ConstraintLayout_Layout_android_minWidth)
                    {
                        mMinWidth = a.getDimensionPixelOffset(attr, mMinWidth);
                    }
                    else if (attr == R.styleable.ConstraintLayout_Layout_android_minHeight)
                    {
                        mMinHeight = a.getDimensionPixelOffset(attr, mMinHeight);
                    }
                    else if (attr == R.styleable.ConstraintLayout_Layout_android_maxWidth)
                    {
                        mMaxWidth = a.getDimensionPixelOffset(attr, mMaxWidth);
                    }
                    else if (attr == R.styleable.ConstraintLayout_Layout_android_maxHeight)
                    {
                        mMaxHeight = a.getDimensionPixelOffset(attr, mMaxHeight);
                    }
                    else if (attr == R.styleable.ConstraintLayout_Layout_layout_optimizationLevel)
                    {
                        mOptimizationLevel = a.getInt(attr, mOptimizationLevel);
                    }
                    else if (attr == R.styleable.ConstraintLayout_Layout_layoutDescription)
                    {
                        int id = a.getResourceId(attr, 0);
                        if (id != 0)
                        {
                            try
                            {
                                parseLayoutDescription(id);
                            }
                            catch (Resources.NotFoundException)
                            {
                                mConstraintLayoutSpec = null;
                            }
                        }
                    }
                    else if (attr == R.styleable.ConstraintLayout_Layout_constraintSet)
                    {
                        int id = a.getResourceId(attr, 0);
                        try
                        {
                            mConstraintSet = new ConstraintSet();
                            mConstraintSet.load(Context, id);
                        }
                        catch (Resources.NotFoundException)
                        {
                            mConstraintSet = null;
                        }
                        mConstraintSetId = id;
                    }
                }
                a.recycle();
            }*/
            mLayoutWidget.OptimizationLevel = mOptimizationLevel;
        }

        /*/// <summary>
        /// Subclasses can override the handling of layoutDescription
        /// </summary>
        /// <param name="id"> </param>
        protected internal virtual void parseLayoutDescription(int id)
        {
            mConstraintLayoutSpec = new ConstraintLayoutStates(Context, this, id);
        }*/


#if WINDOWS
        private new UIElementCollection Children { get; }

        public void AddView(View element)
        {
            Children?.Add(element);
            onViewAdded(element);
        }

        public void RemoveView(View element)
        {
            Children?.Remove(element);
            onViewRemoved(element);
        }

        public void RemoveAll()
        {
            Children?.Clear();
        }

        public int ChildCount { get { return Children.Count; } }

        public View findViewById(int id)
        {
            return mChildrenByIds[id];
        }
#endif
        /// <summary>
        /// @suppress
        /// </summary>
        public void onViewAdded(View view)
        {
            int id = view.GetHashCode();
            //base.onViewAdded(view);
            ConstraintWidget widget = getViewWidget(view);
            if (view is GuidelineView)
            {
                if (!(widget is GuidelineWidget))
                {
                    /*LayoutParams layoutParams = (LayoutParams)view.LayoutParams;
                    layoutParams.widget = new GuidelineWidget();
                    layoutParams.isGuideline = true;
                    ((GuidelineWidget)layoutParams.widget).Orientation = layoutParams.orientation;
                    */
                    mapToWidget.Add(id, new GuidelineWidget()
                    {
                        //TODO:Orientaion
                    });
                }
            }
            if (view is ConstraintHelper)
            {
                ConstraintHelper helper = (ConstraintHelper)view;
                helper.validateParams();
                //LayoutParams layoutParams = (LayoutParams)view.LayoutParams;
                //layoutParams.isHelper = true;
                if (!mConstraintHelpers.Contains(helper))
                {
                    mConstraintHelpers.Add(helper);
                }
            }
            mChildrenByIds.Add(view.GetHashCode(), view);
            mDirtyHierarchy = true;
        }

        /// <summary>
        /// @suppress
        /// </summary>
        public void onViewRemoved(View view)
        {
            //base.onViewRemoved(view);
            mChildrenByIds.Remove(view.GetHashCode());
            ConstraintWidget widget = getViewWidget(view);
            mLayoutWidget.remove(widget);
            if (view is ConstraintHelper)
                mConstraintHelpers.Remove(view as ConstraintHelper);
            mDirtyHierarchy = true;
        }

        /// <summary>
        /// Set the min width for this view
        /// </summary>
        /// <param name="value"> </param>
        public virtual int MinWidth
        {
            set
            {
                if (value == mMinWidth)
                {
                    return;
                }
                mMinWidth = value;
                //requestLayout();
                UpdateLayout();
            }
            get
            {
                return mMinWidth;
            }
        }

        /// <summary>
        /// Set the min height for this view
        /// </summary>
        /// <param name="value"> </param>
        public virtual int MinHeight
        {
            set
            {
                if (value == mMinHeight)
                {
                    return;
                }
                mMinHeight = value;
                //requestLayout();
                UpdateLayout();
            }
            get
            {
                return mMinHeight;
            }
        }



        /// <summary>
        /// Set the max width for this view
        /// </summary>
        /// <param name="value"> </param>
        public virtual int MaxWidth
        {
            set
            {
                if (value == mMaxWidth)
                {
                    return;
                }
                mMaxWidth = value;
                //requestLayout();
                UpdateLayout();
            }
            get
            {
                return mMaxWidth;
            }
        }

        /// <summary>
        /// Set the max height for this view
        /// </summary>
        /// <param name="value"> </param>
        public virtual int MaxHeight
        {
            set
            {
                if (value == mMaxHeight)
                {
                    return;
                }
                mMaxHeight = value;
                //requestLayout();
                UpdateLayout();
            }
            get
            {
                return mMaxHeight;
            }
        }

        private bool updateHierarchy()
        {
            //ORIGINAL LINE: final int count = getChildCount();
            int count = ChildCount;

            bool recompute = false;
            for (int i = 0; i < count; i++)
            {
                //ORIGINAL LINE: final android.view.View child = getChildAt(i);
                View child = (View)Children[i];
                if (child.LayoutRequested)
                {
                    recompute = true;
                    break;
                }
            }
            if (recompute)
            {
                setChildrenConstraints();
            }
            return recompute;
        }

        private void setChildrenConstraints()
        {
            //ORIGINAL LINE: final boolean isInEditMode = DEBUG || isInEditMode();
            bool isInEditMode = DEBUG || InEditMode;

            //ORIGINAL LINE: final int count = getChildCount();
            int count = ChildCount;

            // Make sure everything is fully reset before anything else
            for (int i = 0; i < count; i++)
            {
                View child = (View)Children[i];
                ConstraintWidget widget = getViewWidget(child);
                if (widget == null)
                {
                    continue;
                }
                widget.reset();
            }

            /*if (isInEditMode)
            {
                // In design mode, let's make sure we keep track of the ids; in Studio, a build step
                // might not have been done yet, so asking the system for ids can break. So to be safe,
                // we save the current ids, which helpers can ask for.
                for (int i = 0; i < count; i++)
                {
                    //ORIGINAL LINE: final android.view.View view = getChildAt(i);
                    View child = (View)Children[i];
                    try
                    {
                        string IdAsString = Resources.getResourceName(view.Id);
                        setDesignInformation(DESIGN_INFO_ID, IdAsString, view.Id);
                        int slashIndex = IdAsString.IndexOf('/');
                        if (slashIndex != -1)
                        {
                            IdAsString = IdAsString.Substring(slashIndex + 1);
                        }
                        getTargetWidget(view.Id).DebugName = IdAsString;
                    }
                    catch (Resources.NotFoundException)
                    {
                        // nothing
                    }
                }
            }
            else if (DEBUG)
            {
                mLayoutWidget.DebugName = "root";
                for (int i = 0; i < count; i++)
                {
                    //JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
                    //ORIGINAL LINE: final android.view.View view = getChildAt(i);
                    View view = getChildAt(i);
                    try
                    {
                        string IdAsString = Resources.getResourceName(view.Id);
                        setDesignInformation(DESIGN_INFO_ID, IdAsString, view.Id);
                        int slashIndex = IdAsString.IndexOf('/');
                        if (slashIndex != -1)
                        {
                            IdAsString = IdAsString.Substring(slashIndex + 1);
                        }
                        getTargetWidget(view.Id).DebugName = IdAsString;
                    }
                    catch (Resources.NotFoundException)
                    {
                        // nothing
                    }
                }
            }*/

            /*if (USE_CONSTRAINTS_HELPER && mConstraintSetId != -1)
            {
                for (int i = 0; i < count; i++)
                {
                    //ORIGINAL LINE: final android.view.View child = getChildAt(i);
                    View child = (View)Children[i];
                    if (child.GetHashCode() == mConstraintSetId && child is Constraints)
                    {
                        mConstraintSet = ((Constraints)child).ConstraintSet;
                    }
                }
            }*/

            if (mConstraintSet != null)
            {
                mConstraintSet.applyToInternal(this, true);
            }

            mLayoutWidget.removeAllChildren();

            //ORIGINAL LINE: final int helperCount = mConstraintHelpers.size();
            int helperCount = mConstraintHelpers.Count;
            if (helperCount > 0)
            {
                for (int i = 0; i < helperCount; i++)
                {
                    ConstraintHelper helper = mConstraintHelpers[i];
                    helper.updatePreLayout(this);
                }
            }

            // TODO refactor into an updatePreLayout interface
            for (int i = 0; i < count; i++)
            {
                View child = (View)Children[i];
                if (child is Placeholder)
                {
                    ((Placeholder)child).updatePreLayout(this);
                }
            }

            mTempMapIdToWidget.Clear();
            mTempMapIdToWidget.Add(LayoutParams.PARENT_ID, mLayoutWidget);
            mTempMapIdToWidget.Add(Id, mLayoutWidget);
            for (int i = 0; i < count; i++)
            {
                //ORIGINAL LINE: final android.view.View child = getChildAt(i);
                View child = (View)Children[i];
                ConstraintWidget widget = getViewWidget(child);
                mTempMapIdToWidget.Add(child.GetHashCode(), widget);
            }

            for (int i = 0; i < count; i++)
            {
                //ORIGINAL LINE: final android.view.View child = getChildAt(i);
                View child = (View)Children[i];
                ConstraintWidget widget = getViewWidget(child);
                if (widget == null)
                {
                    continue;
                }
                //ORIGINAL LINE: final LayoutParams layoutParams = (LayoutParams) child.getLayoutParams();
                //LayoutParams layoutParams = (LayoutParams)child.LayoutParams;
                Constraint layoutConstraint = ConstraintSet.Constraints[child.GetHashCode()];
                mLayoutWidget.add(widget);
                //applyConstraintsFromLayoutParams(isInEditMode, child, widget, layoutParams, mTempMapIdToWidget);
                applyConstraintsFromLayoutParams(isInEditMode, child, widget, layoutConstraint, mTempMapIdToWidget);
            }
        }


        //protected internal virtual void applyConstraintsFromLayoutParams(bool isInEditMode, View child, ConstraintWidget widget, LayoutParams layoutParams, Dictionary<int,ConstraintWidget> idToWidget)
        protected internal virtual void applyConstraintsFromLayoutParams(bool isInEditMode, View child, ConstraintWidget widget, Constraint layoutParams, Dictionary<int, ConstraintWidget> idToWidget)
        {

            layoutParams.layout.validate();
            layoutParams.layout.helped = false;

            widget.Visibility = (int)child.Visibility;
            if (layoutParams.layout.isInPlaceholder)
            {
                widget.InPlaceholder = true;
                widget.Visibility = (int)Microsoft.UI.Xaml.Visibility.Collapsed;
            }
            widget.CompanionWidget = child;

            if (child is ConstraintHelper)
            {
                ConstraintHelper helper = (ConstraintHelper)child;
                helper.resolveRtl(widget, mLayoutWidget.Rtl);
            }
            //if (layoutParams.isGuideline)
            if (layoutParams.layout.mIsGuideline)
            {
                GuidelineWidget guideline = (GuidelineWidget)widget;
                /*int resolvedGuideBegin = layoutParams.resolvedGuideBegin;
                int resolvedGuideEnd = layoutParams.resolvedGuideEnd;
                float resolvedGuidePercent = layoutParams.resolvedGuidePercent;
                if (Build.VERSION.SDK_INT < Build.VERSION_CODES.JELLY_BEAN_MR1)
                {*/
                var resolvedGuideBegin = layoutParams.layout.guideBegin;
                var resolvedGuideEnd = layoutParams.layout.guideEnd;
                var resolvedGuidePercent = layoutParams.layout.guidePercent;
                /*}*/
                if (resolvedGuidePercent != UNSET)
                {
                    //guideline.GuidePercent = resolvedGuidePercent;
                    guideline.setGuidePercent(resolvedGuidePercent);
                }
                else if (resolvedGuideBegin != UNSET)
                {
                    guideline.GuideBegin = resolvedGuideBegin;
                }
                else if (resolvedGuideEnd != UNSET)
                {
                    guideline.GuideEnd = resolvedGuideEnd;
                }
            }
            else
            {
                // Get the left/right constraints resolved for RTL
                /*int resolvedLeftToLeft = layoutParams.resolvedLeftToLeft;
                int resolvedLeftToRight = layoutParams.resolvedLeftToRight;
                int resolvedRightToLeft = layoutParams.resolvedRightToLeft;
                int resolvedRightToRight = layoutParams.resolvedRightToRight;
                int resolveGoneLeftMargin = layoutParams.resolveGoneLeftMargin;
                int resolveGoneRightMargin = layoutParams.resolveGoneRightMargin;
                float resolvedHorizontalBias = layoutParams.resolvedHorizontalBias;
*/
                /*if (Build.VERSION.SDK_INT < Build.VERSION_CODES.JELLY_BEAN_MR1)
                {*/
                // Pre JB MR1, left/right should take precedence, unless they are
                // not defined and somehow a corresponding start/end constraint exists
                var resolvedLeftToLeft = layoutParams.layout.leftToLeft;
                var resolvedLeftToRight = layoutParams.layout.leftToRight;
                var resolvedRightToLeft = layoutParams.layout.rightToLeft;
                var resolvedRightToRight = layoutParams.layout.rightToRight;
                var resolveGoneLeftMargin = layoutParams.layout.goneLeftMargin;
                var resolveGoneRightMargin = layoutParams.layout.goneRightMargin;
                var resolvedHorizontalBias = layoutParams.layout.horizontalBias;

                if (resolvedLeftToLeft == UNSET && resolvedLeftToRight == UNSET)
                {
                    if (layoutParams.layout.startToStart != UNSET)
                    {
                        resolvedLeftToLeft = layoutParams.layout.startToStart;
                    }
                    else if (layoutParams.layout.startToEnd != UNSET)
                    {
                        resolvedLeftToRight = layoutParams.layout.startToEnd;
                    }
                }
                if (resolvedRightToLeft == UNSET && resolvedRightToRight == UNSET)
                {
                    if (layoutParams.layout.endToStart != UNSET)
                    {
                        resolvedRightToLeft = layoutParams.layout.endToStart;
                    }
                    else if (layoutParams.layout.endToEnd != UNSET)
                    {
                        resolvedRightToRight = layoutParams.layout.endToEnd;
                    }
                }
                /*}*/

                // Circular constraint
                if (layoutParams.layout.circleConstraint != UNSET)
                {
                    ConstraintWidget target = idToWidget[layoutParams.layout.circleConstraint];
                    if (target != null)
                    {
                        widget.connectCircularConstraint(target, layoutParams.layout.circleAngle, layoutParams.layout.circleRadius);
                    }
                }
                else
                {
                    // Left constraint
                    if (resolvedLeftToLeft != UNSET)
                    {
                        ConstraintWidget target = idToWidget[resolvedLeftToLeft];
                        if (target != null)
                        {
                            widget.immediateConnect(ConstraintAnchor.Type.LEFT, target, ConstraintAnchor.Type.LEFT, layoutParams.layout.leftMargin, resolveGoneLeftMargin);
                        }
                    }
                    else if (resolvedLeftToRight != UNSET)
                    {
                        ConstraintWidget target = idToWidget[resolvedLeftToRight];
                        if (target != null)
                        {
                            widget.immediateConnect(ConstraintAnchor.Type.LEFT, target, ConstraintAnchor.Type.RIGHT, layoutParams.layout.leftMargin, resolveGoneLeftMargin);
                        }
                    }

                    // Right constraint
                    if (resolvedRightToLeft != UNSET)
                    {
                        ConstraintWidget target = idToWidget[resolvedRightToLeft];
                        if (target != null)
                        {
                            widget.immediateConnect(ConstraintAnchor.Type.RIGHT, target, ConstraintAnchor.Type.LEFT, layoutParams.layout.rightMargin, resolveGoneRightMargin);
                        }
                    }
                    else if (resolvedRightToRight != UNSET)
                    {
                        ConstraintWidget target = idToWidget[resolvedRightToRight];
                        if (target != null)
                        {
                            widget.immediateConnect(ConstraintAnchor.Type.RIGHT, target, ConstraintAnchor.Type.RIGHT, layoutParams.layout.rightMargin, resolveGoneRightMargin);
                        }
                    }

                    // Top constraint
                    if (layoutParams.layout.topToTop != UNSET)
                    {
                        ConstraintWidget target = idToWidget[layoutParams.layout.topToTop];
                        if (target != null)
                        {
                            widget.immediateConnect(ConstraintAnchor.Type.TOP, target, ConstraintAnchor.Type.TOP, layoutParams.layout.topMargin, layoutParams.layout.goneTopMargin);
                        }
                    }
                    else if (layoutParams.layout.topToBottom != UNSET)
                    {
                        ConstraintWidget target = idToWidget[layoutParams.layout.topToBottom];
                        if (target != null)
                        {
                            widget.immediateConnect(ConstraintAnchor.Type.TOP, target, ConstraintAnchor.Type.BOTTOM, layoutParams.layout.topMargin, layoutParams.layout.goneTopMargin);
                        }
                    }

                    // Bottom constraint
                    if (layoutParams.layout.bottomToTop != UNSET)
                    {
                        ConstraintWidget target = idToWidget[layoutParams.layout.bottomToTop];
                        if (target != null)
                        {
                            widget.immediateConnect(ConstraintAnchor.Type.BOTTOM, target, ConstraintAnchor.Type.TOP, layoutParams.layout.bottomMargin, layoutParams.layout.goneBottomMargin);
                        }
                    }
                    else if (layoutParams.layout.bottomToBottom != UNSET)
                    {
                        ConstraintWidget target = idToWidget[layoutParams.layout.bottomToBottom];
                        if (target != null)
                        {
                            widget.immediateConnect(ConstraintAnchor.Type.BOTTOM, target, ConstraintAnchor.Type.BOTTOM, layoutParams.layout.bottomMargin, layoutParams.layout.goneBottomMargin);
                        }
                    }

                    // Baseline constraint
                    if (layoutParams.layout.baselineToBaseline != UNSET)
                    {
                        setWidgetBaseline(widget, layoutParams, idToWidget, layoutParams.layout.baselineToBaseline, ConstraintAnchor.Type.BASELINE);
                    }
                    else if (layoutParams.layout.baselineToTop != UNSET)
                    {
                        setWidgetBaseline(widget, layoutParams, idToWidget, layoutParams.layout.baselineToTop, ConstraintAnchor.Type.TOP);
                    }
                    else if (layoutParams.layout.baselineToBottom != UNSET)
                    {
                        setWidgetBaseline(widget, layoutParams, idToWidget, layoutParams.layout.baselineToBottom, ConstraintAnchor.Type.BOTTOM);
                    }

                    if (resolvedHorizontalBias >= 0)
                    {
                        widget.HorizontalBiasPercent = resolvedHorizontalBias;
                    }
                    if (layoutParams.layout.verticalBias >= 0)
                    {
                        widget.VerticalBiasPercent = layoutParams.layout.verticalBias;
                    }
                }

                if (isInEditMode && ((layoutParams.layout.editorAbsoluteX != UNSET) || (layoutParams.layout.editorAbsoluteY != UNSET)))
                {
                    widget.setOrigin(layoutParams.layout.editorAbsoluteX, layoutParams.layout.editorAbsoluteY);
                }

                // FIXME: need to agree on the correct magic value for this rather than simply using zero.
                if (!layoutParams.layout.horizontalDimensionFixed)
                {
                    if (layoutParams.layout.mWidth == ViewGroup.LayoutParams.MATCH_PARENT)
                    {
                        if (layoutParams.layout.constrainedWidth)
                        {
                            widget.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
                        }
                        else
                        {
                            widget.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_PARENT;
                        }
                        widget.getAnchor(ConstraintAnchor.Type.LEFT).mMargin = layoutParams.layout.leftMargin;
                        widget.getAnchor(ConstraintAnchor.Type.RIGHT).mMargin = layoutParams.layout.rightMargin;
                    }
                    else
                    {
                        widget.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
                        widget.Width = 0;
                    }
                }
                else
                {
                    widget.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
                    widget.Width = layoutParams.layout.mWidth;
                    if (layoutParams.layout.mWidth == WRAP_CONTENT)
                    {
                        widget.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
                    }
                }
                if (!layoutParams.layout.verticalDimensionFixed)
                {
                    if (layoutParams.layout.mHeight ==LayoutParams.MATCH_PARENT)
                    {
                        if (layoutParams.layout.constrainedHeight)
                        {
                            widget.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
                        }
                        else
                        {
                            widget.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_PARENT;
                        }
                        widget.getAnchor(ConstraintAnchor.Type.TOP).mMargin = layoutParams.layout.topMargin;
                        widget.getAnchor(ConstraintAnchor.Type.BOTTOM).mMargin = layoutParams.layout.bottomMargin;
                    }
                    else
                    {
                        widget.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
                        widget.Height = 0;
                    }
                }
                else
                {
                    widget.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
                    widget.Height = layoutParams.layout.mHeight;
                    if (layoutParams.layout.mHeight == WRAP_CONTENT)
                    {
                        widget.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
                    }
                }

                widget.setDimensionRatio(layoutParams.layout.dimensionRatio);
                widget.HorizontalWeight = layoutParams.layout.horizontalWeight;
                widget.VerticalWeight = layoutParams.layout.verticalWeight;
                widget.HorizontalChainStyle = layoutParams.layout.horizontalChainStyle;
                widget.VerticalChainStyle = layoutParams.layout.verticalChainStyle;
                widget.WrapBehaviorInParent = layoutParams.layout.mWrapBehavior;
                widget.setHorizontalMatchStyle(layoutParams.layout.widthDefault, layoutParams.layout.widthMin, layoutParams.layout.widthMax, layoutParams.layout.widthPercent);
                widget.setVerticalMatchStyle(layoutParams.layout.heightDefault, layoutParams.layout.heightMin, layoutParams.layout.heightMax, layoutParams.layout.heightPercent);
            }
        }

        //private void setWidgetBaseline(ConstraintWidget widget, LayoutParams layoutParams, SparseArray<ConstraintWidget> idToWidget, int baselineTarget, ConstraintAnchor.Type type)
        private void setWidgetBaseline(ConstraintWidget widget, ConstraintSet.Constraint layoutParams, Dictionary<int,ConstraintWidget> idToWidget, int baselineTarget, ConstraintAnchor.Type type)
        {
            View view = mChildrenByIds[baselineTarget];
            ConstraintWidget target = idToWidget[baselineTarget];
            //if (target != null && view != null && view.LayoutParams is LayoutParams)
            if (target != null && view != null)
            {
                layoutParams.layout.needsBaseline = true;
                if (type == ConstraintAnchor.Type.BASELINE)
                {
                    // baseline to baseline
                    /*LayoutParams targetParams = (LayoutParams)view.LayoutParams;
                    targetParams.needsBaseline = true;
                    targetParams.widget.HasBaseline = true;*/
                    var targetParams = ConstraintSet.Constraints[baselineTarget];
                    targetParams.layout.needsBaseline = true;
                    target.HasBaseline = true;
                }
                ConstraintAnchor baseline = widget.getAnchor(ConstraintAnchor.Type.BASELINE);
                ConstraintAnchor targetAnchor = target.getAnchor(type);
                baseline.connect(targetAnchor, layoutParams.layout.baselineMargin, layoutParams.layout.goneBaselineMargin, true);
                widget.HasBaseline = true;
                widget.getAnchor(ConstraintAnchor.Type.TOP).reset();
                widget.getAnchor(ConstraintAnchor.Type.BOTTOM).reset();
            }
        }

        private ConstraintWidget getTargetWidget(int id)
        {
            //if (id == LayoutParams.PARENT_ID)
            if (id == LayoutParams.PARENT_ID || id == this.GetHashCode())
            {
                return mLayoutWidget;
            }
            else
            {
                /*View view = mChildrenByIds[id];
                if (view == null)
                {
                    view = findViewById(id);
                    if (view != null && view != this && view.Parent == this)
                    {
                        onViewAdded(view);
                    }
                }
                if (view == this)
                {
                    return mLayoutWidget;
                }
                return view == null ? null : ((LayoutParams)view.LayoutParams).widget;*/
                return mapToWidget[id];
            }
        }

        /// <param name="view">
        /// @return
        /// @suppress </param>
        public ConstraintWidget getViewWidget(View view)
        {
            if (view == this)
            {
                return mLayoutWidget;
            }
            if (view != null)
            {
                /*if (view.LayoutParams is LayoutParams)
                {
                    return ((LayoutParams)view.LayoutParams).widget;
                }
                view.LayoutParams = generateLayoutParams(view.LayoutParams);
                if (view.LayoutParams is LayoutParams)
                {
                    return ((LayoutParams)view.LayoutParams).widget;
                }*/
                return mapToWidget[view.GetHashCode()];
            }
            return null;
        }

        /// <param name="metrics">
        /// @suppress Fills metrics object </param>
        public virtual void fillMetrics(Metrics metrics)
        {
            mMetrics = metrics;
            mLayoutWidget.fillMetrics(metrics);
        }

        private int mOnMeasureWidthMeasureSpec = 0;
        private int mOnMeasureHeightMeasureSpec = 0;

        /// <summary>
        /// Handles measuring a layout
        /// </summary>
        /// <param name="layout"> </param>
        /// <param name="optimizationLevel"> </param>
        /// <param name="widthMeasureSpec"> </param>
        /// <param name="heightMeasureSpec"> </param>
        protected internal virtual void resolveSystem(ConstraintWidgetContainer layout, int optimizationLevel, int widthMeasureSpec, int heightMeasureSpec)
        {
            int widthMode = MeasureSpec.getMode(widthMeasureSpec);
            int widthSize = MeasureSpec.getSize(widthMeasureSpec);
            int heightMode = MeasureSpec.getMode(heightMeasureSpec);
            int heightSize = MeasureSpec.getSize(heightMeasureSpec);
#if __ANDROID__
            int paddingY = Math.Max(0, PaddingTop);
            int paddingBottom = Math.Max(0, PaddingBottom);
            int paddingHeight = paddingY + paddingBottom;
            int paddingWidth = PaddingWidth;
#elif WINDOWS
            double paddingY = Math.Max(0, Margin.Top);
            double paddingBottom = Math.Max(0,Margin.Bottom);
            double paddingHeight = paddingY + paddingBottom;
            double paddingWidth = PaddingWidth;
#endif
            int paddingX;
            mMeasurer.captureLayoutInfo(widthMeasureSpec, heightMeasureSpec, paddingY, paddingBottom, paddingWidth, paddingHeight);

            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1)
            {
                int paddingStart = Math.Max(0, PaddingStart);
                int paddingEnd = Math.Max(0, PaddingEnd);
                if (paddingStart > 0 || paddingEnd > 0)
                {
                    if (Rtl)
                    {
                        paddingX = paddingEnd;
                    }
                    else
                    {
                        paddingX = paddingStart;
                    }
                }
                else
                {
                    paddingX = Math.Max(0, PaddingLeft);
                }
            }
            else
            {
                paddingX = Math.Max(0, PaddingLeft);
            }

            widthSize -= paddingWidth;
            heightSize -= paddingHeight;

            setSelfDimensionBehaviour(layout, widthMode, widthSize, heightMode, heightSize);
            layout.measure(optimizationLevel, widthMode, widthSize, heightMode, heightSize, mLastMeasureWidth, mLastMeasureHeight, paddingX, paddingY);
        }

        /// <summary>
        /// Handles calling setMeasuredDimension()
        /// </summary>
        /// <param name="widthMeasureSpec"> </param>
        /// <param name="heightMeasureSpec"> </param>
        /// <param name="measuredWidth"> </param>
        /// <param name="measuredHeight"> </param>
        /// <param name="isWidthMeasuredTooSmall"> </param>
        /// <param name="isHeightMeasuredTooSmall"> </param>
        protected internal virtual void resolveMeasuredDimension(int widthMeasureSpec, int heightMeasureSpec, int measuredWidth, int measuredHeight, bool isWidthMeasuredTooSmall, bool isHeightMeasuredTooSmall)
        {
            int childState = 0;
            int heightPadding = mMeasurer.paddingHeight;
            int widthPadding = mMeasurer.paddingWidth;

            int androidLayoutWidth = measuredWidth + widthPadding;
            int androidLayoutHeight = measuredHeight + heightPadding;

            int resolvedWidthSize = resolveSizeAndState(androidLayoutWidth, widthMeasureSpec, childState);
            int resolvedHeightSize = resolveSizeAndState(androidLayoutHeight, heightMeasureSpec, childState << MEASURED_HEIGHT_STATE_SHIFT);
            resolvedWidthSize &= MEASURED_SIZE_MASK;
            resolvedHeightSize &= MEASURED_SIZE_MASK;
            resolvedWidthSize = Math.Min(mMaxWidth, resolvedWidthSize);
            resolvedHeightSize = Math.Min(mMaxHeight, resolvedHeightSize);
            if (isWidthMeasuredTooSmall)
            {
                resolvedWidthSize |= MEASURED_STATE_TOO_SMALL;
            }
            if (isHeightMeasuredTooSmall)
            {
                resolvedHeightSize |= MEASURED_STATE_TOO_SMALL;
            }
            setMeasuredDimension(resolvedWidthSize, resolvedHeightSize);
            mLastMeasureWidth = resolvedWidthSize;
            mLastMeasureHeight = resolvedHeightSize;
        }
#if WINDOWS
        protected override Size MeasureOverride(Size availableSize)
        {
            onMeasure(availableSize.Width, availableSize.Height);
            return base.MeasureOverride(availableSize);
        }
#endif
        /// <summary>
        /// {@inheritDoc}
        /// </summary>
        protected internal void onMeasure(double widthMeasureSpec, double heightMeasureSpec)
        {
            long time = 0;
            if (DEBUG)
            {
                time = DateTimeHelperClass.CurrentUnixTimeMillis();
            }

            bool sameSpecsAsPreviousMeasure = (mOnMeasureWidthMeasureSpec == widthMeasureSpec && mOnMeasureHeightMeasureSpec == heightMeasureSpec);
            sameSpecsAsPreviousMeasure = false; //TODO re-enable
            if (!mDirtyHierarchy && !sameSpecsAsPreviousMeasure)
            {
                // it's possible that, if we are already marked for a relayout, a view would not call to request a layout;
                // in that case we'd miss updating the hierarchy correctly (window insets change may do that -- we receive
                // a second onMeasure before onLayout).
                // We have to iterate on our children to verify that none set a request layout flag...
                //ORIGINAL LINE: final int count = getChildCount();
                int count = ChildCount;
                for (int i = 0; i < count; i++)
                {
                    //ORIGINAL LINE: final android.view.View child = getChildAt(i);
                    View child = (View)Children[i];
                    if (child.LayoutRequested)
                    {
                        if (DEBUG)
                        {
                            Debug.WriteLine("### CHILD " + child + " REQUESTED LAYOUT, FORCE DIRTY HIERARCHY");
                        }
                        mDirtyHierarchy = true;
                        break;
                    }
                }
            }

            if (!mDirtyHierarchy)
            {
                if (sameSpecsAsPreviousMeasure)
                {
                    resolveMeasuredDimension(widthMeasureSpec, heightMeasureSpec, mLayoutWidget.Width, mLayoutWidget.Height, mLayoutWidget.WidthMeasuredTooSmall, mLayoutWidget.HeightMeasuredTooSmall);
                    return;
                }
                if (OPTIMIZE_HEIGHT_CHANGE && mOnMeasureWidthMeasureSpec == widthMeasureSpec && MeasureSpec.getMode(widthMeasureSpec) == MeasureSpec.EXACTLY && MeasureSpec.getMode(heightMeasureSpec) == MeasureSpec.AT_MOST && MeasureSpec.getMode(mOnMeasureHeightMeasureSpec) == MeasureSpec.AT_MOST)
                {
                    int newSize = MeasureSpec.getSize(heightMeasureSpec);
                    if (DEBUG)
                    {
                        Console.WriteLine("### COMPATIBLE REQ " + newSize + " >= ? " + mLayoutWidget.Height);
                    }
                    if (newSize >= mLayoutWidget.Height && !mLayoutWidget.HeightMeasuredTooSmall)
                    {
                        mOnMeasureWidthMeasureSpec = widthMeasureSpec;
                        mOnMeasureHeightMeasureSpec = heightMeasureSpec;
                        resolveMeasuredDimension(widthMeasureSpec, heightMeasureSpec, mLayoutWidget.Width, mLayoutWidget.Height, mLayoutWidget.WidthMeasuredTooSmall, mLayoutWidget.HeightMeasuredTooSmall);
                        return;
                    }
                }
            }
            mOnMeasureWidthMeasureSpec = widthMeasureSpec;
            mOnMeasureHeightMeasureSpec = heightMeasureSpec;

            if (DEBUG)
            {
                Console.WriteLine("### ON MEASURE " + mDirtyHierarchy + " of " + mLayoutWidget.DebugName + " onMeasure width: " + MeasureSpec.ToString(widthMeasureSpec) + " height: " + MeasureSpec.ToString(heightMeasureSpec) + this);
            }

            mLayoutWidget.Rtl = Rtl;

            if (mDirtyHierarchy)
            {
                mDirtyHierarchy = false;
                if (updateHierarchy())
                {
                    mLayoutWidget.updateHierarchy();
                }
            }

            resolveSystem(mLayoutWidget, mOptimizationLevel, widthMeasureSpec, heightMeasureSpec);
            resolveMeasuredDimension(widthMeasureSpec, heightMeasureSpec, mLayoutWidget.Width, mLayoutWidget.Height, mLayoutWidget.WidthMeasuredTooSmall, mLayoutWidget.HeightMeasuredTooSmall);

            if (DEBUG)
            {
                time = DateTimeHelperClass.CurrentUnixTimeMillis() - time;
                Console.WriteLine(mLayoutWidget.DebugName + " (" + ChildCount + ") DONE onMeasure width: " + MeasureSpec.ToString(widthMeasureSpec) + " height: " + MeasureSpec.ToString(heightMeasureSpec) + " => " + mLastMeasureWidth + " x " + mLastMeasureHeight + " lasted " + time);
            }
        }

        protected internal virtual bool Rtl
        {
            get
            {
                if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1)
                {
                    bool isRtlSupported = (Context.ApplicationInfo.flags & ApplicationInfo.FLAG_SUPPORTS_RTL) != 0;
                    return isRtlSupported && (View.LAYOUT_DIRECTION_RTL == LayoutDirection);
                }
                return false;
            }
        }

        /// <summary>
        /// Compute the padding width, taking in account RTL start/end padding if available and present. </summary>
        /// <returns> padding width </returns>
        private int PaddingWidth
        {
            get
            {
#if __ANDROID__
                int widthPadding = Math.Max(0, PaddingLeft) + Math.Max(0, PaddingRight);
                int rtlPadding = 0;

                if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1)
                {
                    rtlPadding = Math.Max(0, PaddingStart) + Math.Max(0, PaddingEnd);
                }
                if (rtlPadding > 0)
                {
                    widthPadding = rtlPadding;
                }
#elif WINDOWS //Windows中,RTL时坐标系会水平翻转,但用户定义布局还是和之前一样弄,没有Start和End之分
                int widthPadding = (int)(Math.Max(0, Margin.Left) + Math.Max(0,Margin.Right));
                int rtlPadding = 0;
                rtlPadding = (int)(Math.Max(0, Margin.Left) + Math.Max(0, Margin.Right));
                if (rtlPadding > 0)
                {
                    widthPadding = rtlPadding;
                }
#endif
                return widthPadding;
            }
        }

        protected internal virtual void setSelfDimensionBehaviour(ConstraintWidgetContainer layout, int widthMode, int widthSize, int heightMode, int heightSize)
        {

            int heightPadding = mMeasurer.paddingHeight;
            int widthPadding = mMeasurer.paddingWidth;

            ConstraintWidget.DimensionBehaviour widthBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
            ConstraintWidget.DimensionBehaviour heightBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;

            int desiredWidth = 0;
            int desiredHeight = 0;
            //JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int childCount = getChildCount();
            int childCount = ChildCount;

            switch (widthMode)
            {
                case MeasureSpec.AT_MOST:
                    {
                        widthBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
                        desiredWidth = widthSize;
                        if (childCount == 0)
                        {
                            desiredWidth = Math.Max(0, mMinWidth);
                        }
                    }
                    break;
                case MeasureSpec.UNSPECIFIED:
                    {
                        widthBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
                        if (childCount == 0)
                        {
                            desiredWidth = Math.Max(0, mMinWidth);
                        }
                    }
                    break;
                case MeasureSpec.EXACTLY:
                    {
                        desiredWidth = Math.Min(mMaxWidth - widthPadding, widthSize);
                    }
                    break;
            }
            switch (heightMode)
            {
                case MeasureSpec.AT_MOST:
                    {
                        heightBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
                        desiredHeight = heightSize;
                        if (childCount == 0)
                        {
                            desiredHeight = Math.Max(0, mMinHeight);
                        }
                    }
                    break;
                case MeasureSpec.UNSPECIFIED:
                    {
                        heightBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
                        if (childCount == 0)
                        {
                            desiredHeight = Math.Max(0, mMinHeight);
                        }
                    }
                    break;
                case MeasureSpec.EXACTLY:
                    {
                        desiredHeight = Math.Min(mMaxHeight - heightPadding, heightSize);
                    }
                    break;
            }

            if (desiredWidth != layout.Width || desiredHeight != layout.Height)
            {
                layout.invalidateMeasures();
            }
            layout.X = 0;
            layout.Y = 0;
            layout.MaxWidth = mMaxWidth - widthPadding;
            layout.MaxHeight = mMaxHeight - heightPadding;
            layout.MinWidth = 0;
            layout.MinHeight = 0;
            layout.HorizontalDimensionBehaviour = widthBehaviour;
            layout.Width = desiredWidth;
            layout.VerticalDimensionBehaviour = heightBehaviour;
            layout.Height = desiredHeight;
            layout.MinWidth = mMinWidth - widthPadding;
            layout.MinHeight = mMinHeight - heightPadding;
        }

        /// <summary>
        /// Set the State of the ConstraintLayout, causing it to load a particular ConstraintSet.
        /// For states with variants the variant with matching width and height constraintSet will be chosen
        /// </summary>
        /// <param name="id">           the constraint set state </param>
        /// <param name="screenWidth">  the width of the screen in pixels </param>
        /// <param name="screenHeight"> the height of the screen in pixels </param>
        public virtual void setState(int id, int screenWidth, int screenHeight)
        {
            if (mConstraintLayoutSpec != null)
            {
                mConstraintLayoutSpec.updateConstraints(id, screenWidth, screenHeight);
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }

        /// <summary>
        /// {@inheritDoc}
        /// </summary>
        protected internal override void onLayout(bool changed, int left, int top, int right, int bottom)
        {
            if (DEBUG)
            {
                Console.WriteLine(mLayoutWidget.DebugName + " onLayout changed: " + changed + " left: " + left + " top: " + top + " right: " + right + " bottom: " + bottom + " (" + (right - left) + " x " + (bottom - top) + ")");
            }
            //JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int widgetsCount = getChildCount();
            int widgetsCount = ChildCount;
            //JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final boolean isInEditMode = isInEditMode();
            bool isInEditMode = InEditMode;
            for (int i = 0; i < widgetsCount; i++)
            {
                //JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final android.view.View child = getChildAt(i);
                View child = getChildAt(i);
                LayoutParams @params = (LayoutParams)child.LayoutParams;
                ConstraintWidget widget = @params.widget;

                if (child.Visibility == GONE && !@params.isGuideline && !@params.isHelper && !@params.isVirtualGroup && !isInEditMode)
                {
                    // If we are in edit mode, let's layout the widget so that they are at "the right place"
                    // visually in the editor (as we get our positions from layoutlib)
                    continue;
                }
                if (@params.isInPlaceholder)
                {
                    continue;
                }
                int l = widget.X;
                int t = widget.Y;
                int r = l + widget.Width;
                int b = t + widget.Height;

                /*if (DEBUG)
                {
                    if (child.Visibility != View.GONE && (child.MeasuredWidth != widget.Width || child.MeasuredHeight != widget.Height))
                    {
                        int deltaX = Math.Abs(child.MeasuredWidth - widget.Width);
                        int deltaY = Math.Abs(child.MeasuredHeight - widget.Height);
                        if (deltaX > 1 || deltaY > 1)
                        {
                            Console.WriteLine("child " + child + " measuredWidth " + child.MeasuredWidth + " vs " + widget.Width + " x measureHeight " + child.MeasuredHeight + " vs " + widget.Height);
                        }
                    }
                }*/

                child.Arrange(new Rect(l, t, r - l, b - t));
                if (child is Placeholder)
                {
                    Placeholder holder = (Placeholder)child;
                    View content = holder.Content;
                    if (content != null)
                    {
                        content.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
                        content.Arrange(new Rect(l, t, r - l, b - t));
                    }
                }
            }
            //ORIGINAL LINE: final int helperCount = mConstraintHelpers.size();
            int helperCount = mConstraintHelpers.Count;
            if (helperCount > 0)
            {
                for (int i = 0; i < helperCount; i++)
                {
                    ConstraintHelper helper = mConstraintHelpers[i];
                    helper.updatePostLayout(this);
                }
            }
        }

        /// <summary>
        /// Set the optimization for the layout resolution.
        /// <para>
        /// The optimization can be any of the following:
        /// <ul>
        /// <li>Optimizer.OPTIMIZATION_NONE</li>
        /// <li>Optimizer.OPTIMIZATION_STANDARD</li>
        /// <li>a mask composed of specific optimizations</li>
        /// </ul>
        /// The mask can be composed of any combination of the following:
        /// <ul>
        /// <li>Optimizer.OPTIMIZATION_DIRECT  </li>
        /// <li>Optimizer.OPTIMIZATION_BARRIER  </li>
        /// <li>Optimizer.OPTIMIZATION_CHAIN  (experimental) </li>
        /// <li>Optimizer.OPTIMIZATION_DIMENSIONS  (experimental) </li>
        /// </ul>
        /// Note that the current implementation of Optimizer.OPTIMIZATION_STANDARD is as a mask of DIRECT and BARRIER.
        /// </para>
        /// </summary>
        /// <param name="level"> optimization level
        /// @since 1.1 </param>
        public virtual int OptimizationLevel
        {
            set
            {
                mOptimizationLevel = value;
                mLayoutWidget.OptimizationLevel = value;
            }
            get
            {
                return mLayoutWidget.OptimizationLevel;
            }
        }


        /*/// <summary>
        /// @suppress
        /// </summary>
        public override LayoutParams generateLayoutParams(AttributeSet attrs)
        {
            return new LayoutParams(Context, attrs);
        }

        /// <summary>
        /// {@inheritDoc}
        /// </summary>
        protected internal override LayoutParams generateDefaultLayoutParams()
        {
            return new LayoutParams(LayoutParams.WRAP_CONTENT, LayoutParams.WRAP_CONTENT);
        }

        /// <summary>
        /// {@inheritDoc}
        /// </summary>
        protected internal override ViewGroup.LayoutParams generateLayoutParams(ViewGroup.LayoutParams p)
        {
            return new LayoutParams(p);
        }

        /// <summary>
        /// {@inheritDoc}
        /// </summary>
        protected internal override bool checkLayoutParams(ViewGroup.LayoutParams p)
        {
            return p is LayoutParams;
        }*/

        /// <summary>
        /// Sets a ConstraintSet object to manage constraints. The ConstraintSet overrides LayoutParams of child views.
        /// </summary>
        /// <param name="set"> Layout children using ConstraintSet </param>
        public virtual ConstraintSet ConstraintSet
        {
            set
            {
                mConstraintSet = value;
            }
            get { return mConstraintSet; }
        }

        /// <param name="id"> the view id </param>
        /// <returns> the child view, can return null
        /// @suppress Return a direct child view by its id if it exists </returns>
        public virtual View getViewById(int id)
        {
            return mChildrenByIds[id];
        }

        /*/// <summary>
        /// @suppress
        /// </summary>
        protected internal override void dispatchDraw(Canvas canvas)
        {
            if (mConstraintHelpers != null)
            {
                //JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final int helperCount = mConstraintHelpers.size();
                int helperCount = mConstraintHelpers.Count;
                if (helperCount > 0)
                {
                    for (int i = 0; i < helperCount; i++)
                    {
                        ConstraintHelper helper = mConstraintHelpers[i];
                        helper.updatePreDraw(this);
                    }
                }
            }

            base.dispatchDraw(canvas);

            if (DEBUG || InEditMode)
            {
                float cw = Width;
                float ch = Height;
                float ow = 1080;
                float oh = 1920;
                //JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final int count = getChildCount();
                int count = ChildCount;
                for (int i = 0; i < count; i++)
                {
                    View child = getChildAt(i);
                    if (child.Visibility == GONE)
                    {
                        continue;
                    }
                    object tag = child.Tag;
                    if (tag != null && tag is string)
                    {
                        string coordinates = (string)tag;
                        string[] split = coordinates.Split(",", true);
                        if (split.Length == 4)
                        {
                            int x = int.Parse(split[0]);
                            int y = int.Parse(split[1]);
                            int w = int.Parse(split[2]);
                            int h = int.Parse(split[3]);
                            x = (int)((x / ow) * cw);
                            y = (int)((y / oh) * ch);
                            w = (int)((w / ow) * cw);
                            h = (int)((h / oh) * ch);
                            Paint paint = new Paint();
                            paint.Color = Color.RED;
                            canvas.drawLine(x, y, x + w, y, paint);
                            canvas.drawLine(x + w, y, x + w, y + h, paint);
                            canvas.drawLine(x + w, y + h, x, y + h, paint);
                            canvas.drawLine(x, y + h, x, y, paint);
                            paint.Color = Color.GREEN;
                            canvas.drawLine(x, y, x + w, y + h, paint);
                            canvas.drawLine(x, y + h, x + w, y, paint);
                        }
                    }
                }
            }
            if (DEBUG_DRAW_CONSTRAINTS)
            {
                //JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final int count = getChildCount();
                int count = ChildCount;
                for (int i = 0; i < count; i++)
                {
                    View child = getChildAt(i);
                    if (child.Visibility == GONE)
                    {
                        continue;
                    }
                    ConstraintWidget widget = getViewWidget(child);
                    if (widget.mTop.Connected)
                    {
                        ConstraintWidget target = widget.mTop.mTarget.mOwner;
                        int x1 = widget.X + widget.Width / 2;
                        int y1 = widget.Y;
                        int x2 = target.X + target.Width / 2;
                        int y2 = 0;
                        if (widget.mTop.mTarget.mType == ConstraintAnchor.Type.TOP)
                        {
                            y2 = target.Y;
                        }
                        else
                        {
                            y2 = target.Y + target.Height;
                        }
                        Paint paint = new Paint();
                        paint.Color = Color.RED;
                        paint.StrokeWidth = 4;
                        canvas.drawLine(x1, y1, x2, y2, paint);
                    }
                    if (widget.mBottom.Connected)
                    {
                        ConstraintWidget target = widget.mBottom.mTarget.mOwner;
                        int x1 = widget.X + widget.Width / 2;
                        int y1 = widget.Y + widget.Height;
                        int x2 = target.X + target.Width / 2;
                        int y2 = 0;
                        if (widget.mBottom.mTarget.mType == ConstraintAnchor.Type.TOP)
                        {
                            y2 = target.Y;
                        }
                        else
                        {
                            y2 = target.Y + target.Height;
                        }
                        Paint paint = new Paint();
                        paint.StrokeWidth = 4;
                        paint.Color = Color.RED;
                        canvas.drawLine(x1, y1, x2, y2, paint);
                    }
                }
            }
        }*/

        public virtual ConstraintsChangedListener OnConstraintsChanged
        {
            set
            {
                this.mConstraintsChangedListener = value;
                if (mConstraintLayoutSpec != null)
                {
                    mConstraintLayoutSpec.OnConstraintsChanged = value;
                }
            }
        }

        /// <summary>
        /// Load a layout description file from the resources.
        /// </summary>
        /// <param name="layoutDescription"> The resource id, or 0 to reset the layout description. </param>
        /*public virtual void loadLayoutDescription(int layoutDescription)
        {
            if (layoutDescription != 0)
            {
                try
                {
                    mConstraintLayoutSpec = new ConstraintLayoutStates(Context, this, layoutDescription);
                }
                catch (Resources.NotFoundException)
                {
                    mConstraintLayoutSpec = null;
                }
            }
            else
            {
                mConstraintLayoutSpec = null;
            }
        }*/

        /// <summary>
        /// {@inheritDoc}
        /// </summary>
        public void UpdateLayout()
        {
            markHierarchyDirty();
            base.UpdateLayout();
        }


        private void markHierarchyDirty()
        {
            mDirtyHierarchy = true;
            // reset measured cache
            mLastMeasureWidth = -1;
            mLastMeasureHeight = -1;
            mLastMeasureWidthSize = -1;
            mLastMeasureHeightSize = -1;
            mLastMeasureWidthMode = MeasureSpec.UNSPECIFIED;
            mLastMeasureHeightMode = MeasureSpec.UNSPECIFIED;
        }
    }

}
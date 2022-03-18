using System;
using System.Collections.Generic;
using System.Diagnostics;



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
using static SharpConstraintLayout.Maui.Widget.ConstraintSet;
namespace SharpConstraintLayout.Maui.Widget
{

    using androidx.constraintlayout.core.widgets;
    using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;
    using ConstraintWidgetContainer = androidx.constraintlayout.core.widgets.ConstraintWidgetContainer;
    using GuidelineWidget = androidx.constraintlayout.core.widgets.Guideline;
    using Optimizer = androidx.constraintlayout.core.widgets.Optimizer;
    using BasicMeasure = androidx.constraintlayout.core.widgets.analyzer.BasicMeasure;
    using VirtualLayoutWidget = androidx.constraintlayout.core.widgets.VirtualLayout;
#if WINDOWS
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Windows.Foundation;
#elif __IOS__
    using Panel = UIKit.UIView;
    using UIElement = UIKit.UIView;
    using CoreGraphics;
    using Size = CoreGraphics.CGSize;
#elif __ANDROID__
#endif

    /// <summary>
    /// ConstraintLayout is a AndroidX layout form google. Now you can use it at iOS and WinUI, that means you can reuse some layout code.<br/>
    /// 
    /// Notice the size of ConstraintLayout need be set by its parent. Such as add a ConstraintLayout to UIView and constrainted size is <see cref="ConstraintSet.MatchParent"/>, if you not set <code>AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight</code>
    /// ,when you rotate screen, maybe constraintlayout's width and height is not correct.<br/>
    /// Another, not set <see cref="ConstraintSet.MatchParent"/> when Parent can have infinity size, such as StackPanel can have infinity height at WinUI, if you set ConstraintLayout is <see cref="ConstraintSet.MatchParent"/>, it will throw exception or get false size.
    /// </summary>
    public class ConstraintLayout : Panel
    {
        public const string VERSION = "ConstraintLayout-2.1.1";
        public const bool ISSUPPORTRTLPLATFORM = false;//现在完全不支持,需要对照原来的代码看删除了那些关于Rtl的

        public static bool DEBUG = true;//if is true,will print some layout info.
        public static bool MEASURE = false;//if is true,will print time of measure+layout spend.
        private readonly ConstraintWidgetContainer mLayout = new ConstraintWidgetContainer();//default widget
        private readonly Dictionary<int, UIElement> idToViews = new Dictionary<int, UIElement>();
        public readonly Dictionary<int, ConstraintWidget> idsToConstraintWidgets = new Dictionary<int, ConstraintWidget>();
        private Dictionary<int, ConstraintWidget> mTempMapIdToWidget = new Dictionary<int, ConstraintWidget>();
        // This array keep a list of helper objects if they are present
        private List<ConstraintHelper> mConstraintHelpers = new List<ConstraintHelper>(4);
        /// <summary>
        /// 使用其中的Constraints保存布局信息,其用来替代View的LayoutParams,所以每个View对应一个LayoutParams
        /// </summary>
        public ConstraintSet mConstraintSet { get; set; }
        private int mOptimizationLevel = Optimizer.OPTIMIZATION_STANDARD;
        public ConstraintLayout()
        {
            mLayout.CompanionWidget = this;
            mConstraintSet = new ConstraintSet();//默认创建一个保存最开始的Constraints,之后新建的clone其中的信息
            //constraintLayout have default Widget,need add it to dictionary,we can use it like other child later.
            //ConstraintSet.Constraints.Add(ConstraintSet.PARENT_ID, new ConstraintSet.Constraint());//对于Layout,都用ParentID代替GetHashCode,这是因为Layout可以在ApplyTo时替换
            //这里换种思路,不管是ParentId还是HashCode对应的应该都是同一个约束,修改也是同一个
            var rootConstraint = new ConstraintSet.Constraint();
            mConstraintSet.Constraints.Add(ConstraintSet.ParentId, rootConstraint);
            mConstraintSet.Constraints.Add(this.GetHashCode(), rootConstraint);

            mLayout.Measurer = new MeasurerAnonymousInnerClass(this);

            //ClipToBounds = true;//view in ConstraintLayout always easy out of bounds
        }

#if WINDOWS
        public void AddView(UIElement element)
        {
            Children?.Add(element);
            OnAddedView(element);
        }

        public void RemoveView(UIElement element)
        {
            Children?.Remove(element);
            OnRemovedView(element);
        }

        public void RemoveAll()
        {
            Children?.Clear();
        }

        public int ChildCount { get { return Children != null ? Children.Count : 0; } }
#elif __IOS__
        private UIElement[] Children { get { return Subviews; } }
        public override void AddSubview(UIElement view)
        {
            base.AddSubview(view);
            OnAddedView(view);
        }
        public void AddView(UIElement element)
        {
            base.AddSubview(element);
            OnAddedView(element);
        }
        public void RemoveView(UIElement element)
        {
            element.RemoveFromSuperview();
            OnRemovedView(element);
        }

        public void RemoveAll()
        {
            foreach(var element in this.Subviews)
            {
                element.RemoveFromSuperview();
                OnRemovedView(element);
            }
        }

        public int ChildCount { get { return Subviews.Length; } }
#endif
        public UIElement findViewById(int id)
        {
            return idToViews[id];
        }

        public UIElement getViewById(int id)
        {
            return idToViews[id];
        }

        protected void OnAddedView(UIElement element)
        {
            if (element == null)
            {
                return;
            }

            var id = element.GetHashCode();

            //Add to three dictionary(Wighet,View,Constraints)
            ConstraintWidget widget = CreateOrGetWidgetAndAddToLayout(id);
            mConstraintSet.Constraints.Add(id, new ConstraintSet.Constraint());
            idToViews.Add(id, element as UIElement);
            // Do stuff with the added object
            if (element is Guideline)//Guideline have default widget
            {
                Guideline guideLine = (Guideline)element;
                idsToConstraintWidgets[id] = guideLine.mGuideline;//替换widget类型
                mConstraintSet.Constraints[id].layout.mIsGuideline = true;
            }

            if (element is ConstraintHelper)//ConstraintHelper also have default widget
            {
                ConstraintHelper helper = (ConstraintHelper)element;
                helper.ValidateParams(idsToConstraintWidgets);//其中会替换widget类型
                if (!mConstraintHelpers.Contains(helper))
                {
                    mConstraintHelpers.Add(helper);
                }
                mConstraintSet.Constraints[id].layout.isHelper = true;
            }
        }

        protected void OnRemovedView(UIElement element)
        {
            if (element == null)
            {
                return;
            }

            var id = element.GetHashCode();
            idToViews.Remove(id);
            ConstraintWidget widget = CreateOrGetWidgetAndAddToLayout(id);
            idsToConstraintWidgets.Remove(id);
            if (element is ConstraintHelper)
                mConstraintHelpers.Remove(element as ConstraintHelper);
        }

        private ConstraintWidget CreateOrGetWidgetAndAddToLayout(int id)
        {
            if (id == this.GetHashCode() || id == ConstraintSet.ParentId)
            {
                return RootWidget;
            }
            else
            {
                if (idsToConstraintWidgets.ContainsKey(id))
                    return idsToConstraintWidgets[id];
                else
                {
                    ConstraintWidget widget = new ConstraintWidget();
                    idsToConstraintWidgets.Add(id, widget);
                    return widget;
                }
            }
        }

        bool isInfinityAvailabelSize = false;
#if WINDOWS
        protected override Size MeasureOverride(Size availableSize)
#elif __IOS__
        protected Size MeasureOverride(Size availableSize)
#endif
        {
            if (DEBUG) Debug.WriteLine($"{nameof(MeasureOverride)} {this} {availableSize}");

            if (mConstraintSet.IsChanged)
            {
                //约束到child的widget
                if (updateHierarchy())
                {
                    RootWidget.updateHierarchy();
                }

                //更新约束到Container的Widget
                var constraint = mConstraintSet.Constraints[this.GetHashCode()];
                applyConstraintsFromLayoutParams(false, this, RootWidget, constraint, idsToConstraintWidgets);

                mConstraintSet.IsChanged = false;
            }
#if WINDOWS
            int availableWidth = 0;
            int availableHeight = 0;
            //sometimes no fixsize
            if (double.IsPositiveInfinity(availableSize.Width))
            { availableWidth = int.MaxValue; isInfinityAvailabelSize = true; }
            else
                availableWidth = (int)availableSize.Width;

            if (double.IsPositiveInfinity(availableSize.Height))
            { availableHeight = int.MaxValue; isInfinityAvailabelSize = true; }
            else
                availableHeight = (int)availableSize.Height;
#elif __IOS__
            //iOS中,如果指定了Frame,那么能获得自身的值,如果没有,那么可以取Superview的Frame,因为布局child必须要layout的大小去参照
            int availableWidth = (int)availableSize.Width;
            int availableHeight = (int)availableSize.Height;  
#endif
            //Container的大小需要根据程序指定
            switch (RootWidget.HorizontalDimensionBehaviour)
            {
                case ConstraintWidget.DimensionBehaviour.FIXED:
                    {
                        //已知是Fixed时说明默认没有赋值或者赋值了,赋值了的话原始的约束绝对有值,那么Widget.Width不用变,没有赋值的直接按available大小处理,因为
                        //父布局会指定值,而我们不指定则代表遵从父布局的指定
                        if (mConstraintSet.Constraints[this.GetHashCode()].layout.mWidth <= 0)//用原始的约束来判断是否为固定值
                            RootWidget.Width = availableWidth;
                    }
                    break;

                case ConstraintWidget.DimensionBehaviour.WRAP_CONTENT:
                case ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT:
                case ConstraintWidget.DimensionBehaviour.MATCH_PARENT:
                    //需要足够的空间,这个空间由Layout的父布局决定
                    RootWidget.Width = availableWidth;
                    break;
            }

            switch (RootWidget.VerticalDimensionBehaviour)
            {
                case ConstraintWidget.DimensionBehaviour.FIXED:
                    {
                        if (mConstraintSet.Constraints[this.GetHashCode()].layout.mHeight <= 0)//用原始的约束来判断是否为固定值
                        {
                            RootWidget.Height = availableHeight;
                        }
                    }
                    break;

                case ConstraintWidget.DimensionBehaviour.WRAP_CONTENT:
                case ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT:
                case ConstraintWidget.DimensionBehaviour.MATCH_PARENT:
                    //需要足够的空间,这个空间由Layout的父布局决定
                    RootWidget.Height = availableHeight;
                    break;
            }

            //分析Windows测量
            //对Layout的约束中,
            //Fixed直接将值给Widget值,无需使用available;
            //MatchParent需要available,直接赋值,如果存在无限大情况,也直接赋值,在Arrange时再次取finalSize赋值测量,当然,注意WrapLayout这类可以容纳无限大的布局可能出现问题.
            //MatchContraint和WrapContent是大小不明的,需要依赖child的测量,而child需要根据父布局的可容纳大小来测量,因此也将available值传入

            //分析iOS测量
            //iOS的Layout没有Measure方法,其IntrinsicContentSize需要知道测量方式才能计算出来,这里不适用
            //Fixed和赋值Frame一样,
            //如果需要MatchParent

            //传入布局的测量数据,用于测量child时
            (RootWidget.Measurer as MeasurerAnonymousInnerClass).captureLayoutInfo(RootWidget.Width, RootWidget.Height, 0, 0, 0, 0);

            //交给Container去测量
            RootWidget.OptimizationLevel = mOptimizationLevel;
            RootWidget.layout();
            RootWidget.measure(mOptimizationLevel, BasicMeasure.EXACTLY, RootWidget.Width, BasicMeasure.EXACTLY, RootWidget.Height, 0, 0, 0, 0);
#if WINDOWS
            //如果存在一个方向是无限值,且Layout是MatchParent,则会出错
            if (isInfinityAvailabelSize && (RootWidget.HorizontalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_PARENT || RootWidget.VerticalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_PARENT))
            {
                Debug.Fail($"ConstraintLayout's Parent {this.Parent} has infinity size, please not let ConstraintLayout is MatchParent.");
                throw new InvalidOperationException($"ConstraintLayout's Parent {this.Parent} has infinity size, please not let ConstraintLayout is MatchParent.");
            }
#endif
            return new Size(RootWidget.Width, RootWidget.Height);
        }


#if WINDOWS
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (DEBUG) Debug.WriteLine($"{nameof(ArrangeOverride)} {this} {finalSize}");

            //何时需要重新测量?需要Parent大小但MeasureOverride中拿不到时,那么就只有MeasureOverride中拿到的值是无限值且layout需要MatchParent时.
            /*if (isInfinityAvailabelSize && (RootWidget.HorizontalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_PARENT || RootWidget.VerticalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_PARENT))
            {
                var parentSize = (Parent as UIElement).DesiredSize;
                RootWidget.Width = (int)parentSize.Width;
                RootWidget.Height = (int)parentSize.Height;
                RootWidget.layout();
                RootWidget.measure(mOptimizationLevel, BasicMeasure.EXACTLY, RootWidget.Width, BasicMeasure.EXACTLY, RootWidget.Height, 0, 0, 0, 0);
            }*/

            //layout child
            foreach (ConstraintWidget child in mLayout.Children)
            {
                UIElement component = (UIElement)child.CompanionWidget;

                if (child.Visibility == Gone && !(component is Guideline) && !(component is ConstraintHelper) && !(component is VirtualLayout))
                {
                    // If we are in edit mode, let's layout the widget so that they are at "the right place"
                    // visually in the editor (as we get our positions from layoutlib)
                    continue;
                }

                if (child.InPlaceholder)
                {
                    continue;
                }

                if (component != null)
                {
                    if (DEBUG) Debug.WriteLine($"{nameof(ArrangeOverride)} {component} {new Rect(child.X, child.Y, child.Width, child.Height)}");
                    component.Arrange(new Rect(child.X, child.Y, child.Width, child.Height));
                }

                if (component is Placeholder)
                {
                    Placeholder holder = (Placeholder)component;
                    UIElement content = holder.Content;
                    if (content != null)
                    {
                        content.Visibility = Visibility.Visible;
                        content.Arrange(new Rect(child.X, child.Y, child.Width, child.Height));
                    }
                }
            }

            return new Size(RootWidget.Width, RootWidget.Height);//这里必须返回Widget的大小,因为返回值决定了layout的绘制范围?
        }

#elif __IOS__
        public override Size IntrinsicContentSize => this.Frame.Size;

        /// <summary>
        /// 貌似在开始会被调用两次,应该在第二次时是能获得superview和child默认大小的
        /// </summary>
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            if (DEBUG) Debug.WriteLine($"{nameof(LayoutSubviews)} {this} {this.Frame}");
            if (DEBUG) Debug.WriteLine($"{nameof(LayoutSubviews)} {Superview} {this.Superview?.Frame}");

            //得到自身或Superview的大小作为availableSize
            if (this.Frame.Size.Width > 0)
            { 
                    MeasureOverride(this.Frame.Size);
            }
            else
            {
                if(Superview!=null)
                    MeasureOverride(this.Superview.Frame.Size);
            }
            
            if (DEBUG) Debug.WriteLine($"{nameof(LayoutSubviews)} RootWidget {this.RootWidget.ToString()}");

            //更新layout的大小
            Frame = new CGRect(Frame.X,Frame.Y,RootWidget.Width, RootWidget.Height);
            //layout child
            foreach (ConstraintWidget child in mLayout.Children)
            {
                UIElement component = (UIElement)child.CompanionWidget;

                if (child.Visibility == Gone && !(component is Guideline) && !(component is ConstraintHelper) && !(component is VirtualLayout)) {
                    // If we are in edit mode, let's layout the widget so that they are at "the right place"
                    // visually in the editor (as we get our positions from layoutlib)
                    continue;
                }

                if (child.InPlaceholder) {
                    continue;
                }

                if (component != null) 
                {
                    if (DEBUG) Debug.WriteLine($"{nameof(LayoutSubviews)} {component} {new CGRect(child.X, child.Y, child.Width, child.Height)}");
                    component.Frame = (new CoreGraphics.CGRect(child.X, child.Y, child.Width, child.Height));
                }

                if (component is Placeholder) {
                    Placeholder holder = (Placeholder)component;
                    UIElement content = holder.Content;
                    if (content != null)
                    {
                        content.Hidden = false; 
                        content.Frame = (new CoreGraphics.CGRect(child.X, child.Y, child.Width, child.Height));
                    }
                }
            }
        }
#endif

        /// <summary>
        /// default widget of ConstraintLayout.
        /// you can constrol more action by it.
        /// </summary>
        public virtual ConstraintWidgetContainer RootWidget
        {
            get
            {
                return mLayout;
            }
        }

        /// <summary>
        /// get widget of this view.
        /// </summary>
        /// <param name="view"></param>
        /// <returns>widget is a virtual control for caculate constraint</returns>
        public ConstraintWidget GetWidget(UIElement view)
        {
            if (view == this)
                return RootWidget;
            return this.idsToConstraintWidgets[view.GetHashCode()];
        }

        //copy from swing
        private class MeasurerAnonymousInnerClass : BasicMeasure.Measurer
        {
            private readonly ConstraintLayout outerInstance;

            internal int paddingTop = 0;
            internal int paddingBottom = 0;
            internal int paddingWidth = 0;
            internal int paddingHeight = 0;
            internal int layoutWidthSpec = 0;
            internal int layoutHeightSpec = 0;

            public virtual void captureLayoutInfo(int widthSpec, int heightSpec, int top, int bottom, int width, int height)
            {
                paddingTop = top;
                paddingBottom = bottom;
                paddingWidth = width;
                paddingHeight = height;
                layoutWidthSpec = widthSpec;
                layoutHeightSpec = heightSpec;
            }

            public MeasurerAnonymousInnerClass(ConstraintLayout outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public virtual void measure(ConstraintWidget widget, BasicMeasure.Measure measure)
            {
#if WPF
                if (widget is VirtualLayout)
                    outerInstance.measureFlow(widget, measure);
                else
                    outerInstance.innerMeasure(widget, measure);
#elif WINDOWS || __IOS__
                AndroidMeasure(widget, measure);
#endif
            }

            public virtual void didMeasures()
            {
                //ORIGINAL LINE: final int widgetsCount = layout.getChildCount();
                int widgetsCount = outerInstance.ChildCount;
                for (int i = 0; i < widgetsCount; i++)
                {
                    //ORIGINAL LINE: final android.view.View child = layout.getChildAt(i);
                    UIElement child = (UIElement)outerInstance.Children[i];
                    if (child is Placeholder)
                    {
                        ((Placeholder)child).UpdatePostMeasure(outerInstance);
                    }
                }
                // TODO refactor into an updatePostMeasure interface
                //ORIGINAL LINE: final int helperCount = layout.mConstraintHelpers.size();
                int helperCount = outerInstance.mConstraintHelpers.Count;
                if (helperCount > 0)
                {
                    for (int i = 0; i < helperCount; i++)
                    {
                        ConstraintHelper helper = outerInstance.mConstraintHelpers[i];
                        helper.UpdatePostMeasure(outerInstance);
                    }
                }
            }

            /// <summary>
            /// copy from android,计算child的大小,这里的原则是,wrapcontent的使用自身大小,可能需要测量,matchparent的拿parent大小,matchcontraint的拿0
            /// </summary>
            /// <param name="widget">child 的 widget</param>
            /// <param name="measure"></param>
            private void AndroidMeasure(ConstraintWidget widget, BasicMeasure.Measure measure)
            {
                if (widget == null)
                {
                    return;
                }
                if (widget.Visibility == Gone && !widget.InPlaceholder)
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
                    startMeasure = DateTimeHelperClass.nanoTime();
                }

                /**
                 * 分析怎么测量Child
                 */
                ConstraintWidget.DimensionBehaviour horizontalBehavior = measure.horizontalBehavior;
                ConstraintWidget.DimensionBehaviour verticalBehavior = measure.verticalBehavior;

                int horizontalDimension = measure.horizontalDimension;
                int verticalDimension = measure.verticalDimension;

                int horizontalSpec = 0;
                int verticalSpec = 0;

                int heightPadding = paddingTop + paddingBottom;
                int widthPadding = paddingWidth;
                UIElement child = (UIElement)widget.CompanionWidget;
                switch (horizontalBehavior)
                {
                    case ConstraintWidget.DimensionBehaviour.FIXED:
                        {
                            horizontalSpec = horizontalDimension;
                        }
                        break;
                    case ConstraintWidget.DimensionBehaviour.WRAP_CONTENT:
                        {
                            horizontalSpec = layoutWidthSpec + paddingWidth;
                        }
                        break;
                    case ConstraintWidget.DimensionBehaviour.MATCH_PARENT:
                        {
                            // Horizontal spec must account for margin as well as padding here.
                            //horizontalSpec = getChildMeasureSpec(layoutWidthSpec, widthPadding + widget.HorizontalMargin, LayoutParams.MATCH_PARENT);
                            horizontalSpec = layoutWidthSpec - (widthPadding + widget.HorizontalMargin);
                        }
                        break;
                    case ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT:
                        {
                            horizontalSpec = layoutWidthSpec;
                            bool shouldDoWrap = widget.mMatchConstraintDefaultWidth == MatchConstraintWrap;
                            if (measure.measureStrategy == BasicMeasure.Measure.TRY_GIVEN_DIMENSIONS || measure.measureStrategy == BasicMeasure.Measure.USE_GIVEN_DIMENSIONS)
                            {
                                // the solver gives us our new dimension, but if we previously had it measured with
                                // a wrap, it can be incorrect if the other side was also variable.
                                // So in that case, we have to double-check the other side is stable (else we can't
                                // just assume the wrap value will be correct).
#if WINDOWS
                                bool otherDimensionStable = child.DesiredSize.Height == widget.Height;
#elif __IOS__
                                bool otherDimensionStable = child.Bounds.Size.Height == widget.Height;
#endif
                                bool useCurrent = measure.measureStrategy == BasicMeasure.Measure.USE_GIVEN_DIMENSIONS || !shouldDoWrap || (shouldDoWrap && otherDimensionStable) || (child is Placeholder) || (widget.ResolvedHorizontally);
                                if (useCurrent)
                                {
                                    horizontalSpec = widget.Width;
                                }
                            }
                        }
                        break;
                }

                switch (verticalBehavior)
                {
                    case ConstraintWidget.DimensionBehaviour.FIXED:
                        {
                            verticalSpec = verticalDimension;
                        }
                        break;
                    case ConstraintWidget.DimensionBehaviour.WRAP_CONTENT:
                        {
                            verticalSpec = layoutHeightSpec + heightPadding;
                        }
                        break;
                    case ConstraintWidget.DimensionBehaviour.MATCH_PARENT:
                        {
                            // Vertical spec must account for margin as well as padding here.
                            verticalSpec = layoutHeightSpec - (heightPadding + widget.VerticalMargin);
                        }
                        break;
                    case ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT:
                        {
                            verticalSpec = layoutHeightSpec - heightPadding;
                            bool shouldDoWrap = widget.mMatchConstraintDefaultHeight == MatchConstraintWrap;
                            if (measure.measureStrategy == BasicMeasure.Measure.TRY_GIVEN_DIMENSIONS || measure.measureStrategy == BasicMeasure.Measure.USE_GIVEN_DIMENSIONS)
                            {
                                // the solver gives us our new dimension, but if we previously had it measured with
                                // a wrap, it can be incorrect if the other side was also variable.
                                // So in that case, we have to double-check the other side is stable (else we can't
                                // just assume the wrap value will be correct).
#if WINDOWS
                                bool otherDimensionStable = child.DesiredSize.Width == widget.Width;
#elif __IOS__
                                bool otherDimensionStable = child.Bounds.Size.Width == widget.Width;
#endif
                                bool useCurrent = measure.measureStrategy == BasicMeasure.Measure.USE_GIVEN_DIMENSIONS || !shouldDoWrap || (shouldDoWrap && otherDimensionStable) || (child is Placeholder) || (widget.ResolvedVertically);
                                if (useCurrent)
                                {
                                    verticalSpec = widget.Height;
                                }
                            }
                        }
                        break;
                }

                ConstraintWidgetContainer container = (ConstraintWidgetContainer)widget.Parent;
                /*if (container != null && Optimizer.enabled(outerInstance.mOptimizationLevel, Optimizer.OPTIMIZATION_CACHE_MEASURES))
                {
                    if (child.Frame.Size.Width == widget.Width && child.Frame.Size.Width < container.Width && child.Frame.Size.Height == widget.Height && child.Frame.Size.Height < container.Height && child.GetBaseline() == widget.BaselineDistance && !widget.MeasureRequested)
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
                }*/

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
                Constraint @params = outerInstance.mConstraintSet.Constraints[child.GetHashCode()];
                int width = 0;
                int height = 0;
                int baseline = 0;

                if ((measure.measureStrategy == BasicMeasure.Measure.TRY_GIVEN_DIMENSIONS || measure.measureStrategy == BasicMeasure.Measure.USE_GIVEN_DIMENSIONS) || !(horizontalMatchConstraints && widget.mMatchConstraintDefaultWidth == MatchConstraintSpread && verticalMatchConstraints && widget.mMatchConstraintDefaultHeight == MatchConstraintSpread))
                {

                    if (child is VirtualLayout && widget is androidx.constraintlayout.core.widgets.VirtualLayout)
                    {
                        androidx.constraintlayout.core.widgets.VirtualLayout layout = (androidx.constraintlayout.core.widgets.VirtualLayout)widget;
                        ((VirtualLayout)child).OnMeasure(layout, horizontalSpec, verticalSpec);
                    }
                    else
                    {
#if WINDOWS
                        child.Measure(new Size(horizontalSpec, verticalSpec));
#elif __IOS__
                        //iOS没有Measure
#endif
                    }
                    widget.setLastMeasureSpec(horizontalSpec, verticalSpec);
#if WINDOWS
                    int w = (int)child.DesiredSize.Width;
                    int h = (int)child.DesiredSize.Height;
#elif __IOS__
                    int w = (int)child.IntrinsicContentSize.Width;
                    int h = (int)child.IntrinsicContentSize.Height;
#endif
                    baseline = (int)child.GetBaseline();

                    width = w;
                    height = h;

                    if (DEBUG)
                    {
                        //string measurement = MeasureSpec.ToString(horizontalSpec) + " x " + MeasureSpec.ToString(verticalSpec) + " => " + width + " x " + height;
                        //Console.WriteLine("    (M) measure " + " (" + widget.DebugName + ") : " + measurement);
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
                            horizontalSpec = width;
                        }
                        if (h != height)
                        {
                            verticalSpec = height;
                        }
#if WINDOWS
                        child.Measure(new Size(horizontalSpec, verticalSpec));
#elif __IOS__
                        //iOS没有测量Child的方法
#endif
                        widget.setLastMeasureSpec(horizontalSpec, verticalSpec);
#if WINDOWS
                        width = (int)child.DesiredSize.Width;
                        height = (int)child.DesiredSize.Height;
                        baseline = (int)child.GetBaseline();
#elif __IOS__
                        width = (int)child.IntrinsicContentSize.Width;
                        height = (int)child.IntrinsicContentSize.Height;
                        baseline = (int)child.GetBaseline();
#endif
                        /*if (DEBUG)
                        {
                            string measurement2 = MeasureSpec.ToString(horizontalSpec) + " x " + MeasureSpec.ToString(verticalSpec) + " => " + width + " x " + height;
                            Console.WriteLine("measure (b) " + widget.DebugName + " : " + measurement2);
                        }*/
                    }
                }

                bool hasBaseline = baseline != -1;

                measure.measuredNeedsSolverPass = (width != measure.horizontalDimension) || (height != measure.verticalDimension);
                if (@params.layout.needsBaseline)
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
                /*if (MEASURE)
                {
                    endMeasure = DateTimeHelperClass.nanoTime();
                    if (outerInstance.mMetrics != null)
                    {
                        outerInstance.mMetrics.measuresWidgetsDuration += (endMeasure - startMeasure);
                    }
                }*/

                //原来的逻辑没有正确处理:没有根据WRAP_CONTENT和MATCH_PARENT,MATCH_CONTRAINT去处理宽高, 
                //这里我添加判断
                switch (horizontalBehavior)
                {
                    case ConstraintWidget.DimensionBehaviour.FIXED:
                        width = horizontalDimension;
                        break;
                    case ConstraintWidget.DimensionBehaviour.WRAP_CONTENT:
                        break;
                    case ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT:
                        width = widget.Width;
                        break;
                    case ConstraintWidget.DimensionBehaviour.MATCH_PARENT:
                        width = layoutWidthSpec;
                        break;
                }
                switch (verticalBehavior)
                {
                    case ConstraintWidget.DimensionBehaviour.FIXED:
                        height = verticalDimension;
                        break;
                    case ConstraintWidget.DimensionBehaviour.WRAP_CONTENT:
                        break;
                    case ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT:
                        height = widget.Height;
                        break;
                    case ConstraintWidget.DimensionBehaviour.MATCH_PARENT:
                        height = layoutHeightSpec;
                        break;
                }
#if WINDOWS
                //child.Measure(new Size(width, height));

#endif
                measure.measuredWidth = width;
                measure.measuredHeight = height;
            }
        }

        /// <summary>
        /// copy from swing
        /// </summary>
        /// <param name="constraintWidget"></param>
        /// <param name="measure"></param>
        private void innerMeasure(ConstraintWidget constraintWidget, BasicMeasure.Measure measure)
        {
            UIElement component = (UIElement)constraintWidget.CompanionWidget;
            int measuredWidth = constraintWidget.Width;
            int measuredHeight = constraintWidget.Height;
            //if (DEBUG)
            //Debug.WriteLine($"{(component as FrameworkElement)?.Tag as string} measureWidget " + measuredWidth + " " + measuredHeight);
            if (measure.horizontalBehavior == ConstraintWidget.DimensionBehaviour.WRAP_CONTENT)
            {
#if WINDOWS
                //measuredWidth = component.MinimumSize.width;
                measuredWidth = (int)(component.DesiredSize.Width + 0.5);
#elif __IOS__
                measuredWidth = (int)(component.Frame.Width + 0.5);
#endif
            }
            else if (measure.horizontalBehavior == ConstraintWidget.DimensionBehaviour.MATCH_PARENT)
            {
                measuredWidth = mLayout.Width;
            }
            if (measure.verticalBehavior == ConstraintWidget.DimensionBehaviour.WRAP_CONTENT)
            {
#if WINDOWS
                //measuredHeight = component.MinimumSize.height;
                measuredHeight = (int)(component.DesiredSize.Height + 0.5);
#elif __IOS__
                measuredHeight = (int)(component.Frame.Height + 0.5);
#endif
            }
            else if (measure.verticalBehavior == ConstraintWidget.DimensionBehaviour.MATCH_PARENT)
            {
                measuredHeight = mLayout.Height;
            }
            measure.measuredWidth = measuredWidth;
            measure.measuredHeight = measuredHeight;
            //baseline
            measure.measuredBaseline = (int)component.GetBaseline();
        }

        /// <summary>
        /// copy from FlowTest
        /// </summary>
        /// <param name="widget"></param>
        /// <param name="measure"></param>
        private void measureFlow(ConstraintWidget widget, BasicMeasure.Measure measure)
        {
            ConstraintWidget.DimensionBehaviour horizontalBehavior = measure.horizontalBehavior;
            ConstraintWidget.DimensionBehaviour verticalBehavior = measure.verticalBehavior;
            int horizontalDimension = measure.horizontalDimension;
            int verticalDimension = measure.verticalDimension;

            if (widget is VirtualLayoutWidget)
            {
                VirtualLayoutWidget layout = (VirtualLayoutWidget)widget;
                int widthMode = BasicMeasure.UNSPECIFIED;
                int heightMode = BasicMeasure.UNSPECIFIED;
                int widthSize = 0;
                int heightSize = 0;
                if (layout.HorizontalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_PARENT)
                {
                    widthSize = layout.Parent != null ? layout.Parent.Width : 0;
                    widthMode = BasicMeasure.EXACTLY;
                }
                else if (horizontalBehavior == ConstraintWidget.DimensionBehaviour.FIXED)
                {
                    widthSize = horizontalDimension;
                    widthMode = BasicMeasure.EXACTLY;
                }
                if (layout.VerticalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_PARENT)
                {
                    heightSize = layout.Parent != null ? layout.Parent.Height : 0;
                    heightMode = BasicMeasure.EXACTLY;
                }
                else if (verticalBehavior == ConstraintWidget.DimensionBehaviour.FIXED)
                {
                    heightSize = verticalDimension;
                    heightMode = BasicMeasure.EXACTLY;
                }
                layout.measure(widthMode, widthSize, heightMode, heightSize);
                measure.measuredWidth = layout.MeasuredWidth;
                measure.measuredHeight = layout.MeasuredHeight;
            }
            else
            {
                if (horizontalBehavior == ConstraintWidget.DimensionBehaviour.FIXED)
                {
                    measure.measuredWidth = horizontalDimension;
                }
                if (verticalBehavior == ConstraintWidget.DimensionBehaviour.FIXED)
                {
                    measure.measuredHeight = verticalDimension;
                }
            }
        }


        private bool updateHierarchy()
        {
            //int count = ChildCount;

            //Android中的逻辑是只要有一个布局变了,就需要调用setChildrenConstraints,其也是更新全部的内容,所以这里我直接标记为重新计算
            //bool recompute = false;
            bool recompute = true;
            /*for (int i = 0; i < count; i++)
            {
                //ORIGINAL LINE: final android.view.View child = getChildAt(i);
                UIElement child = Children[i];
                if (child.LayoutRequested)
                {
                    recompute = true;
                    break;
                }
            }*/
            if (recompute)
            {
                setChildrenConstraints();
            }
            return recompute;
        }

        bool InEditMode = false;
        private void setChildrenConstraints()
        {
            //ORIGINAL LINE: final boolean isInEditMode = DEBUG || isInEditMode();
            bool isInEditMode = DEBUG || InEditMode;

            //ORIGINAL LINE: final int count = getChildCount();
            int count = ChildCount;

            // Make sure everything is fully reset before anything else
            for (int i = 0; i < count; i++)
            {
                UIElement child = Children[i];
                ConstraintWidget widget = GetWidget(child);
                if (widget == null)
                {
                    continue;
                }
                widget.reset();//全部widget的数据重置
            }

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

            /*if (ConstraintSet != null)
            {
                //这一步我不执行,因为这一步会清除ConstraintSet,我需要保留这个信息去替换LayoutParams的功能
                //ConstraintSet.applyToInternal(this, true);
            }*/

            RootWidget.removeAllChildren();

            int helperCount = mConstraintHelpers.Count;
            if (helperCount > 0)
            {
                for (int i = 0; i < helperCount; i++)
                {
                    ConstraintHelper helper = mConstraintHelpers[i];
                    helper.UpdatePreLayout(this);//可以整理一下数据
                }
            }

            // TODO refactor into an updatePreLayout interface
            for (int i = 0; i < count; i++)
            {
                UIElement child = Children[i];
                if (child is Placeholder)
                {
                    ((Placeholder)child).UpdatePreLayout(this);
                }
            }

            mTempMapIdToWidget.Clear();
            mTempMapIdToWidget.Add(ConstraintSet.ParentId, RootWidget);
            mTempMapIdToWidget.Add(this.GetHashCode(), RootWidget);
            for (int i = 0; i < count; i++)//添加widgets到临时字典
            {
                UIElement child = Children[i];
                ConstraintWidget widget = GetWidget(child);
                mTempMapIdToWidget.Add(child.GetHashCode(), widget);
            }

            for (int i = 0; i < count; i++)//apply constraint到每个widget
            {
                UIElement child = Children[i];
                ConstraintWidget widget = GetWidget(child);
                if (widget == null)
                {
                    continue;
                }
                Constraint layoutConstraint = mConstraintSet.Constraints[child.GetHashCode()];
                RootWidget.add(widget);
                applyConstraintsFromLayoutParams(isInEditMode, child, widget, layoutConstraint, mTempMapIdToWidget);
            }
        }


        /// <summary>
        /// Apply constraint to widget
        /// </summary>
        /// <param name="isInEditMode"></param>
        /// <param name="child">view</param>
        /// <param name="widget">view's widget</param>
        /// <param name="layoutParams">constraint</param>
        /// <param name="idToWidget">a widget dict for find widget</param>
        protected internal virtual void applyConstraintsFromLayoutParams(bool isInEditMode, UIElement child, ConstraintWidget widget, Constraint layoutParams, Dictionary<int, ConstraintWidget> idToWidget)
        {
            //layoutParams.layout.validate();//很奇怪,实在不理解为什么要重置layoutparams再从里面取参数
            layoutParams.layout.helped = false;
            
            widget.Visibility = layoutParams.propertySet.visibility;//这里我设置为从constraints中取,涉及布局的都交给constraints
            
            //这里我添加对View的可见性设置,因为不知道为什么widget在Windows上设置不了Invisible
            ConstraintHelper.SetPlatformVisibility(child, widget.Visibility);

            if (layoutParams.layout.isInPlaceholder)
            {
                widget.InPlaceholder = true;
                widget.Visibility = ConstraintSet.Gone;
            }
            widget.CompanionWidget = child;

            if (child is ConstraintHelper)
            {
                ConstraintHelper helper = (ConstraintHelper)child;
                helper.ResolveRtl(widget, RootWidget.Rtl);
            }

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
                if (resolvedGuidePercent != Unset)
                {
                    //guideline.GuidePercent = resolvedGuidePercent;
                    guideline.setGuidePercent(resolvedGuidePercent);
                }
                else if (resolvedGuideBegin != Unset)
                {
                    guideline.GuideBegin = resolvedGuideBegin;
                }
                else if (resolvedGuideEnd != Unset)
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

                if (resolvedLeftToLeft == Unset && resolvedLeftToRight == Unset)
                {
                    if (layoutParams.layout.startToStart != Unset)
                    {
                        resolvedLeftToLeft = layoutParams.layout.startToStart;
                    }
                    else if (layoutParams.layout.startToEnd != Unset)
                    {
                        resolvedLeftToRight = layoutParams.layout.startToEnd;
                    }
                }
                if (resolvedRightToLeft == Unset && resolvedRightToRight == Unset)
                {
                    if (layoutParams.layout.endToStart != Unset)
                    {
                        resolvedRightToLeft = layoutParams.layout.endToStart;
                    }
                    else if (layoutParams.layout.endToEnd != Unset)
                    {
                        resolvedRightToRight = layoutParams.layout.endToEnd;
                    }
                }
                /*}*/

                // Circular constraint
                if (layoutParams.layout.circleConstraint != Unset)
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
                    if (resolvedLeftToLeft != Unset)
                    {
                        ConstraintWidget target = idToWidget[resolvedLeftToLeft];
                        if (target != null)
                        {
                            widget.immediateConnect(ConstraintAnchor.Type.LEFT, target, ConstraintAnchor.Type.LEFT, layoutParams.layout.leftMargin, resolveGoneLeftMargin);
                        }
                    }
                    else if (resolvedLeftToRight != Unset)
                    {
                        ConstraintWidget target = idToWidget[resolvedLeftToRight];
                        if (target != null)
                        {
                            widget.immediateConnect(ConstraintAnchor.Type.LEFT, target, ConstraintAnchor.Type.RIGHT, layoutParams.layout.leftMargin, resolveGoneLeftMargin);
                        }
                    }

                    // Right constraint
                    if (resolvedRightToLeft != Unset)
                    {
                        ConstraintWidget target = idToWidget[resolvedRightToLeft];
                        if (target != null)
                        {
                            widget.immediateConnect(ConstraintAnchor.Type.RIGHT, target, ConstraintAnchor.Type.LEFT, layoutParams.layout.rightMargin, resolveGoneRightMargin);
                        }
                    }
                    else if (resolvedRightToRight != Unset)
                    {
                        ConstraintWidget target = idToWidget[resolvedRightToRight];
                        if (target != null)
                        {
                            widget.immediateConnect(ConstraintAnchor.Type.RIGHT, target, ConstraintAnchor.Type.RIGHT, layoutParams.layout.rightMargin, resolveGoneRightMargin);
                        }
                    }

                    // Top constraint
                    if (layoutParams.layout.topToTop != Unset)
                    {
                        ConstraintWidget target = idToWidget[layoutParams.layout.topToTop];
                        if (target != null)
                        {
                            widget.immediateConnect(ConstraintAnchor.Type.TOP, target, ConstraintAnchor.Type.TOP, layoutParams.layout.topMargin, layoutParams.layout.goneTopMargin);
                        }
                    }
                    else if (layoutParams.layout.topToBottom != Unset)
                    {
                        ConstraintWidget target = idToWidget[layoutParams.layout.topToBottom];
                        if (target != null)
                        {
                            widget.immediateConnect(ConstraintAnchor.Type.TOP, target, ConstraintAnchor.Type.BOTTOM, layoutParams.layout.topMargin, layoutParams.layout.goneTopMargin);
                        }
                    }

                    // Bottom constraint
                    if (layoutParams.layout.bottomToTop != Unset)
                    {
                        ConstraintWidget target = idToWidget[layoutParams.layout.bottomToTop];
                        if (target != null)
                        {
                            widget.immediateConnect(ConstraintAnchor.Type.BOTTOM, target, ConstraintAnchor.Type.TOP, layoutParams.layout.bottomMargin, layoutParams.layout.goneBottomMargin);
                        }
                    }
                    else if (layoutParams.layout.bottomToBottom != Unset)
                    {
                        ConstraintWidget target = idToWidget[layoutParams.layout.bottomToBottom];
                        if (target != null)
                        {
                            widget.immediateConnect(ConstraintAnchor.Type.BOTTOM, target, ConstraintAnchor.Type.BOTTOM, layoutParams.layout.bottomMargin, layoutParams.layout.goneBottomMargin);
                        }
                    }

                    // Baseline constraint
                    if (layoutParams.layout.baselineToBaseline != Unset)
                    {
                        setWidgetBaseline(widget, layoutParams, idToWidget, layoutParams.layout.baselineToBaseline, ConstraintAnchor.Type.BASELINE);
                    }
                    else if (layoutParams.layout.baselineToTop != Unset)
                    {
                        setWidgetBaseline(widget, layoutParams, idToWidget, layoutParams.layout.baselineToTop, ConstraintAnchor.Type.TOP);
                    }
                    else if (layoutParams.layout.baselineToBottom != Unset)
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

                if (isInEditMode && ((layoutParams.layout.editorAbsoluteX != Unset) || (layoutParams.layout.editorAbsoluteY != Unset)))
                {
                    widget.setOrigin(layoutParams.layout.editorAbsoluteX, layoutParams.layout.editorAbsoluteY);
                }

                // FIXME: need to agree on the correct magic value for this rather than simply using zero.
                if (!layoutParams.layout.horizontalDimensionFixed)//Match_Parent和Match_Constraint是不固定
                {
                    if (layoutParams.layout.mWidth == ConstraintSet.MatchParent)
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
                else//具体数值或Warp_Content是固定
                {
                    widget.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
                    widget.Width = layoutParams.layout.mWidth;
                    if (layoutParams.layout.mWidth == WrapContent)
                    {
                        widget.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
                    }
                }
                if (!layoutParams.layout.verticalDimensionFixed)
                {
                    if (layoutParams.layout.mHeight == ConstraintSet.MatchParent)
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
                    if (layoutParams.layout.mHeight == WrapContent)
                    {
                        widget.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
                    }
                }

                widget.setDimensionRatio(layoutParams.layout.dimensionRatio);
                widget.HorizontalWeight = layoutParams.layout.horizontalWeight;
                widget.VerticalWeight = layoutParams.layout.verticalWeight;
                widget.HorizontalChainStyle = layoutParams.layout.horizontalChainStyle;
                widget.VerticalChainStyle = layoutParams.layout.verticalChainStyle;
                widget.WrapBehaviorInParent = layoutParams.layout.wrapBehaviorInParent;
                widget.setHorizontalMatchStyle(layoutParams.layout.matchConstraintDefaultWidth, layoutParams.layout.matchConstraintMinWidth, layoutParams.layout.matchConstraintMaxWidth, layoutParams.layout.matchConstraintPercentWidth);
                widget.setVerticalMatchStyle(layoutParams.layout.matchConstraintDefaultHeight, layoutParams.layout.matchConstraintMinHeight, layoutParams.layout.matchConstraintMaxHeight, layoutParams.layout.matchConstraintPercentHeight);
            }
        }

        private void setWidgetBaseline(ConstraintWidget widget, ConstraintSet.Constraint layoutParams, Dictionary<int, ConstraintWidget> idToWidget, int baselineTarget, ConstraintAnchor.Type type)
        {
            UIElement view = idToViews[baselineTarget];
            ConstraintWidget target = idToWidget[baselineTarget];
            if (target != null && view != null)
            {
                layoutParams.layout.needsBaseline = true;
                if (type == ConstraintAnchor.Type.BASELINE)
                {
                    // baseline to baseline
                    var targetParams = mConstraintSet.Constraints[baselineTarget];
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
    }
}
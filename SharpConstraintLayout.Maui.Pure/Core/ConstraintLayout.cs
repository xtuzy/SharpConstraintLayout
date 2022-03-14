using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;



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
using static SharpConstraintLayout.Maui.Pure.Core.ConstraintSet;
namespace SharpConstraintLayout.Maui.Pure.Core
{
    using androidx.constraintlayout.core.widgets;
    using androidx.constraintlayout.core.widgets.analyzer;
    using Utils = androidx.constraintlayout.core.motion.utils.Utils;
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
#elif __ANDROID__
#endif

    /// <summary>
    /// WPF implementation of ConstraintLayout copy from Swing's ConstraintLayout
    /// </summary>
    public class ConstraintLayout : Panel
    {
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
        public ConstraintSet ConstraintSet { get; set; }
        private int mOptimizationLevel = Optimizer.OPTIMIZATION_STANDARD;
        public ConstraintLayout()
        {
            mLayout.CompanionWidget = this;
            ConstraintSet = new ConstraintSet();//默认创建一个保存最开始的Constraints,之后新建的clone其中的信息
            //constraintLayout have default Widget,need add it to dictionary,we can use it like other child later.
            //ConstraintSet.Constraints.Add(ConstraintSet.PARENT_ID, new ConstraintSet.Constraint());//对于Layout,都用ParentID代替GetHashCode,这是因为Layout可以在ApplyTo时替换
            //这里换种思路,不管是ParentId还是HashCode对应的应该都是同一个约束,修改也是同一个
            var rootConstraint = new ConstraintSet.Constraint();
            ConstraintSet.Constraints.Add(ConstraintSet.PARENT_ID, rootConstraint);
            ConstraintSet.Constraints.Add(this.GetHashCode(), rootConstraint);

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
            ConstraintSet.Constraints.Add(id, new ConstraintSet.Constraint());
            idToViews.Add(id, element as UIElement);
            // Do stuff with the added object
            if (element is Core.Guideline)//Guideline have default widget
            {
                idsToConstraintWidgets[id] = new GuidelineWidget();//替换widget类型
                ConstraintSet.Constraints[id].layout.isGuideline = true;
                (idsToConstraintWidgets[id] as GuidelineWidget).Orientation = ConstraintSet.Constraints[id].layout.orientation;
            }

            if (element is ConstraintHelper)//ConstraintHelper also have default widget
            {
                ConstraintHelper helper = (ConstraintHelper)element;
                helper.validateParams();//其中会替换widget类型
                if (!mConstraintHelpers.Contains(helper))
                {
                    mConstraintHelpers.Add(helper);
                }
                ConstraintSet.Constraints[id].layout.isHelper = true;
            }
            //TODO:?标记需要刷新
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
            if (id == this.GetHashCode() || id == ConstraintSet.PARENT_ID)
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
#if WPF
        bool isInfinity = false;
        protected override Size MeasureOverride(Size availableSize)
        {
            //更新约束到widget
            if (updateHierarchy())
            {
                RootWidget.updateHierarchy();
            }
            //first measure all child size,we need know some default size.
            foreach (UIElement child in Children)
            {
                var widget = GetWidget(child);
                //匹配约束的先不测量,因为没有固定的值
                if (widget.HorizontalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT
                    && widget.VerticalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT)
                    continue;
                //if(!IsMeasureValid)
                child.Measure(availableSize);
            }

            //we have know some view default size, so we can calculate other view size that they should be.
            if (double.IsPositiveInfinity(availableSize.Width))
            {
                mLayout.Width = int.MaxValue;
                isInfinity = true;
            }
            else
            {
                mLayout.Width = (int)availableSize.Width;
            }

            if (double.IsPositiveInfinity(availableSize.Height))
            {
                mLayout.Height = int.MaxValue;
                isInfinity = true;
            }
            else
            {
                mLayout.Height = (int)availableSize.Height;
            }
            mLayout.OptimizationLevel = mOptimizationLevel;
            mLayout.layout();
            mLayout.measure(mOptimizationLevel, BasicMeasure.EXACTLY, mLayout.Width, BasicMeasure.EXACTLY, mLayout.Height, 0, 0, 0, 0);
            double w;
            double h; 
            //now we know all view corrected size in constaint, so give it to child,let them to caculate their child.
            foreach (UIElement child in Children)
            {

                var widget = GetWidget(child);

                if (widget.HorizontalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT
                    || widget.VerticalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT)
                {
                    if (widget.HorizontalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.WRAP_CONTENT)
                        //newMeasureSize.Width = (int)child.DesiredSize.Width;
                        w = availableSize.Width;
                    else
                        w = widget.Width;
                    if (widget.VerticalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.WRAP_CONTENT)
                        //newMeasureSize.Height = (int)child.DesiredSize.Height;
                        h = availableSize.Height;
                    else
                        h = widget.Height;
                    child.Measure(new Size(w, h));
                }
                if (DEBUG)
                    Debug.WriteLine($"{(child as FrameworkElement).Tag as string},Size {widget.Width},{widget.Height} ,Baseline {widget.BaselineDistance} ");
            }

            //return availableSize;
            //自身的位置绘制由父控件决定,所以这里要传的正确.
            //如果自身是要Match_Parent,而父控件传进来MaxValue,那么应该传回MaxValue?
            //如果自身是Match_Parent,而父控件能传来特定值,那么直接传回特定值
            //如果自身是Warp_Content和Match_Constraint,那么应该能从子控件约束中计算出特定值传回
            /*if(mLayout.HorizontalDimensionBehaviour==ConstraintWidget.DimensionBehaviour.MATCH_PARENT&&double.IsInfinity(availableSize.Width))
                w= 0;
            else*/
            w = mLayout.Width;
            /*if(mLayout.VerticalDimensionBehaviour==ConstraintWidget.DimensionBehaviour.MATCH_PARENT&&double.IsInfinity(availableSize.Height))
                h= 0;
            else*/
            h = mLayout.Height;
            return new Size(w, h);
        }
#elif WINDOWS
       bool isInfinityMeasure = false;
        protected override Size MeasureOverride(Size availableSize)
        {
            if (DEBUG) Debug.WriteLine($"{nameof(MeasureOverride)} {this} {availableSize}");

            //更新约束到widget
            if (updateHierarchy())
            {
                RootWidget.updateHierarchy();
            }

            //sometimes no fixsize
            if (double.IsPositiveInfinity(availableSize.Width))
            { RootWidget.Width = int.MaxValue; isInfinityMeasure = true; }
            else
                RootWidget.Width = (int)availableSize.Width;

            if (double.IsPositiveInfinity(availableSize.Height))
            { RootWidget.Height = int.MaxValue; isInfinityMeasure = true; }
            else
                RootWidget.Height = (int)availableSize.Height;

            //计算Layout的大小
            switch (RootWidget.HorizontalDimensionBehaviour)
            {
                case ConstraintWidget.DimensionBehaviour.FIXED:
                    //Constraints中的已经赋值过了
                    break;
                case ConstraintWidget.DimensionBehaviour.WRAP_CONTENT:
                    RootWidget.Width = RootWidget.Width;//TODO:TEST
                    break;
                case ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT:
                    RootWidget.Width = RootWidget.Width;//TODO:TEST
                    break;
                case ConstraintWidget.DimensionBehaviour.MATCH_PARENT:
                    RootWidget.Width = RootWidget.Width;//TODO:TEST
                    break;
            }

            switch (RootWidget.VerticalDimensionBehaviour)
            {
                case ConstraintWidget.DimensionBehaviour.FIXED:
                    //Constraints中的已经赋值过了
                    break;
                case ConstraintWidget.DimensionBehaviour.WRAP_CONTENT:
                    RootWidget.Height = RootWidget.Height;//TODO:TEST
                    break;
                case ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT:
                    RootWidget.Height = RootWidget.Height;//TODO:TEST
                    break;
                case ConstraintWidget.DimensionBehaviour.MATCH_PARENT:
                    RootWidget.Height = RootWidget.Height;//TODO:TEST
                    break;
            }
            //传入布局的测量数据,用于测量child时
            (mLayout.Measurer as MeasurerAnonymousInnerClass).captureLayoutInfo(mLayout.Width, mLayout.Height, 0, 0, 0, 0);
            //交给Container去测量
            mLayout.OptimizationLevel = mOptimizationLevel;
            mLayout.layout();
            mLayout.measure(mOptimizationLevel, BasicMeasure.EXACTLY, mLayout.Width, BasicMeasure.EXACTLY, mLayout.Height, 0, 0, 0, 0);
            return new Size(RootWidget.Width, RootWidget.Height);
        }
#elif __IOS__
        /// <summary>
        /// iOS没有Measure,Layout的Measure我们直接看ConstraintWidget.DimensionBehaviour
        /// </summary>
        protected void MeasureOverride()
        {
            //更新约束到widget
            if (updateHierarchy())
            {
                RootWidget.updateHierarchy();
            }
            //计算Layout的大小
            switch (RootWidget.HorizontalDimensionBehaviour)
            {
                case ConstraintWidget.DimensionBehaviour.FIXED:
                    //Constraints中的已经赋值过了
                    break;
                case ConstraintWidget.DimensionBehaviour.WRAP_CONTENT:
                    RootWidget.Width = 0;//TODO:TEST
                    break;
                case ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT:
                    RootWidget.Width = 0;//TODO:TEST
                    break;
                case ConstraintWidget.DimensionBehaviour.MATCH_PARENT:
                    RootWidget.Width = (int)Superview.Frame.Width;//TODO:TEST
                    break;
            }

            switch (RootWidget.VerticalDimensionBehaviour)
            {
                case ConstraintWidget.DimensionBehaviour.FIXED:
                    //Constraints中的已经赋值过了
                    break;
                case ConstraintWidget.DimensionBehaviour.WRAP_CONTENT:
                    RootWidget.Height = 0;//TODO:TEST
                    break;
                case ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT:
                    RootWidget.Height = 0;//TODO:TEST
                    break;
                case ConstraintWidget.DimensionBehaviour.MATCH_PARENT:
                    RootWidget.Height = (int)Superview.Frame.Height;//TODO:TEST
                    break;
            }
            //传入布局的测量数据,用于测量child时
            (mLayout.Measurer as MeasurerAnonymousInnerClass).captureLayoutInfo(mLayout.Width, mLayout.Height, 0, 0, 0, 0);
            //交给Container去测量
            mLayout.OptimizationLevel = mOptimizationLevel;
            mLayout.layout();
            mLayout.measure(mOptimizationLevel, BasicMeasure.EXACTLY, mLayout.Width, BasicMeasure.EXACTLY, mLayout.Height, 0, 0, 0, 0);
        }
#endif

#if WPF
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (DEBUG)
                Debug.WriteLine($"{this.Tag as string},finalSize {finalSize}, DesiredSize {this.DesiredSize}");

            //recalculate?,because when constraintlayout size be define by parent,it size need parent to arrange
            //such as it as child of listview,listview will send double.infinity to measure,if you set listview's content to strenth,
            //you need get that size at Arrage. 
            if (isInfinity && finalSize.Width != 0 && finalSize.Height != 0)//only when parent give me measure size is size isInfinity and finalSize can use(not 0),we recalculate layout.
            {
                if (DEBUG)
                    Debug.WriteLine($"{this.Tag as string} Re layout");
                mLayout.Width = (int)finalSize.Width;
                mLayout.Height = (int)finalSize.Height;
                mLayout.layout();
                mLayout.OptimizationLevel = Optimizer.OPTIMIZATION_STANDARD;
                mLayout.measure(Optimizer.OPTIMIZATION_STANDARD, BasicMeasure.EXACTLY, (int)finalSize.Width, BasicMeasure.EXACTLY, (int)finalSize.Height, 0, 0, 0, 0);
                //isInfinity = false;//Wpf's ArrangeOverride can be load mutiple times, not by measure.
            }

            //layout child
            foreach (ConstraintWidget child in mLayout.Children)
            {
                UIElement component = (UIElement)child.CompanionWidget;
                if (component != null)
                {
                    component.Arrange(new Rect(child.X, child.Y, child.Width, child.Height));
                }
            }

            return finalSize;
        }
#elif WINDOWS
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (DEBUG) Debug.WriteLine($"{nameof(ArrangeOverride)} {this} {finalSize}");
            if (isInfinityMeasure)
            {
                mLayout.layout();
                RootWidget.measure(mOptimizationLevel, BasicMeasure.EXACTLY, (int)finalSize.Width, BasicMeasure.EXACTLY, (int)finalSize.Height, 0, 0, 0, 0);
            }
            //layout child
            foreach (ConstraintWidget child in mLayout.Children)
            {
                UIElement component = (UIElement)child.CompanionWidget;
                if (component != null)
                {
                    if (DEBUG) Debug.WriteLine($"{nameof(ArrangeOverride)} {component} {new Rect(child.X, child.Y, child.Width, child.Height)}");
                    component.Arrange(new Rect(child.X, child.Y, child.Width, child.Height));
                }
            }
            return finalSize;
        }
#elif __IOS__ 
        /// <summary>
        /// 貌似在开始会被调用两次,应该在第二次时是能获得superview和child默认大小的
        /// </summary>
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            if (DEBUG) Debug.WriteLine($"{nameof(LayoutSubviews)} {this} {this.Frame}");

            //得到默认大小后测量
            MeasureOverride();

            //layout child
            foreach (ConstraintWidget child in mLayout.Children)
            {
                UIElement component = (UIElement)child.CompanionWidget;
                if (component != null)
                {
                    if (DEBUG) Debug.WriteLine($"{nameof(LayoutSubviews)} {component} {new CGRect(child.X, child.Y, child.Width, child.Height)}");
                    component.Frame = (new CoreGraphics.CGRect(child.X, child.Y, child.Width, child.Height));
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
                AndroidMeasure(widget,measure);
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
                        ((Placeholder)child).updatePostMeasure(outerInstance);
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
                        helper.updatePostMeasure(outerInstance);
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
                            bool shouldDoWrap = widget.mMatchConstraintDefaultWidth == MATCH_CONSTRAINT_WRAP;
                            if (measure.measureStrategy == BasicMeasure.Measure.TRY_GIVEN_DIMENSIONS || measure.measureStrategy == BasicMeasure.Measure.USE_GIVEN_DIMENSIONS)
                            {
                                // the solver gives us our new dimension, but if we previously had it measured with
                                // a wrap, it can be incorrect if the other side was also variable.
                                // So in that case, we have to double-check the other side is stable (else we can't
                                // just assume the wrap value will be correct).
#if WINDOWS
                                bool otherDimensionStable = child.DesiredSize.Height == widget.Height;
#elif __IOS__
                                bool otherDimensionStable = child.Frame.Size.Height == widget.Height;
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
                            bool shouldDoWrap = widget.mMatchConstraintDefaultHeight == MATCH_CONSTRAINT_WRAP;
                            if (measure.measureStrategy == BasicMeasure.Measure.TRY_GIVEN_DIMENSIONS || measure.measureStrategy == BasicMeasure.Measure.USE_GIVEN_DIMENSIONS)
                            {
                                // the solver gives us our new dimension, but if we previously had it measured with
                                // a wrap, it can be incorrect if the other side was also variable.
                                // So in that case, we have to double-check the other side is stable (else we can't
                                // just assume the wrap value will be correct).
#if WINDOWS
                                bool otherDimensionStable = child.DesiredSize.Width == widget.Width;
#elif __IOS__
                                bool otherDimensionStable = child.Frame.Size.Width == widget.Width;
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
                Constraint @params = outerInstance.ConstraintSet.Constraints[child.GetHashCode()];
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
                    int w = (int)child.Frame.Width;
                    int h = (int)child.Frame.Height;
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
                        width = (int)child.Frame.Width;
                        height = (int)child.Frame.Height;
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
                widget.reset();//全部重置?
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

            if (ConstraintSet != null)
            {
                //这一步我不执行,因为这一步会清除ConstraintSet,我需要保留这个信息去替换LayoutParams的功能
                //ConstraintSet.applyToInternal(this, true);
            }

            RootWidget.removeAllChildren();

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
                UIElement child = Children[i];
                if (child is Placeholder)
                {
                    ((Placeholder)child).updatePreLayout(this);
                }
            }

            mTempMapIdToWidget.Clear();
            mTempMapIdToWidget.Add(ConstraintSet.PARENT_ID, RootWidget);
            mTempMapIdToWidget.Add(this.GetHashCode(), RootWidget);
            for (int i = 0; i < count; i++)//添加widgets到临时字典
            {
                UIElement child = Children[i];
                ConstraintWidget widget = GetWidget(child);
                mTempMapIdToWidget.Add(child.GetHashCode(), widget);
            }

            for (int i = 0; i < count; i++)//apply每个constraint到widget
            {
                UIElement child = Children[i];
                ConstraintWidget widget = GetWidget(child);
                if (widget == null)
                {
                    continue;
                }
                //LayoutParams layoutParams = (LayoutParams)child.LayoutParams;
                Constraint layoutConstraint = ConstraintSet.Constraints[child.GetHashCode()];
                RootWidget.add(widget);
                //applyConstraintsFromLayoutParams(isInEditMode, child, widget, layoutParams, mTempMapIdToWidget);
                applyConstraintsFromLayoutParams(isInEditMode, child, widget, layoutConstraint, mTempMapIdToWidget);
            }
        }


        //protected internal virtual void applyConstraintsFromLayoutParams(bool isInEditMode, View child, ConstraintWidget widget, LayoutParams layoutParams, Dictionary<int,ConstraintWidget> idToWidget)
        protected internal virtual void applyConstraintsFromLayoutParams(bool isInEditMode, UIElement child, ConstraintWidget widget, Constraint layoutParams, Dictionary<int, ConstraintWidget> idToWidget)
        {

            //layoutParams.layout.validate();//很奇怪,实在不理解为什么要重置layoutparams再从里面取参数
            layoutParams.layout.helped = false;
            /*#if WINDOWS
                        widget.Visibility = (child.Visibility == Visibility.Visible?ConstraintSet.VISIBLE:(child.Opacity==0? ConstraintSet.INVISIBLE:ConstraintSet.GONE));
#elif __IOS__
                widget.Visibility = (child.Hidden == true ? ConstraintSet.INVISIBLE: child.Opacity == 0?ConstraintSet.GONE: ConstraintSet.VISIBLE);
#endif*/
                widget.Visibility = layoutParams.propertySet.visibility;//这里我设置为从constraints中取,涉及布局的都交给constraints
            if (layoutParams.layout.isInPlaceholder)
            {
                widget.InPlaceholder = true;
                widget.Visibility = ConstraintSet.GONE;
            }
            widget.CompanionWidget = child;

            if (child is ConstraintHelper)
            {
                ConstraintHelper helper = (ConstraintHelper)child;
                helper.resolveRtl(widget, RootWidget.Rtl);
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
                if (!layoutParams.layout.horizontalDimensionFixed)//Match_Parent和Match_Constraint是不固定
                {
                    if (layoutParams.layout.mWidth == ConstraintSet.MATCH_PARENT)
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
                    if (layoutParams.layout.mWidth == WRAP_CONTENT)
                    {
                        widget.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
                    }
                }
                if (!layoutParams.layout.verticalDimensionFixed)
                {
                    if (layoutParams.layout.mHeight == ConstraintSet.MATCH_PARENT)
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
                widget.WrapBehaviorInParent = layoutParams.layout.wrapBehaviorInParent;
                widget.setHorizontalMatchStyle(layoutParams.layout.matchConstraintDefaultWidth, layoutParams.layout.matchConstraintMinWidth, layoutParams.layout.matchConstraintMaxWidth, layoutParams.layout.matchConstraintPercentWidth);
                widget.setVerticalMatchStyle(layoutParams.layout.matchConstraintDefaultHeight, layoutParams.layout.matchConstraintMinHeight, layoutParams.layout.matchConstraintMaxHeight, layoutParams.layout.matchConstraintPercentHeight);
            }
        }

        private void setWidgetBaseline(ConstraintWidget widget, ConstraintSet.Constraint layoutParams, Dictionary<int, ConstraintWidget> idToWidget, int baselineTarget, ConstraintAnchor.Type type)
        {
            UIElement view = idToViews[baselineTarget];
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
    }
}
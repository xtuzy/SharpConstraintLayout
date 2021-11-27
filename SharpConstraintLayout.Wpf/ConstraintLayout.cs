using androidx.constraintlayout.core.widgets;
using androidx.constraintlayout.core.widgets.analyzer;
using SharpConstraintLayout.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

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

namespace SharpConstraintLayout.Wpf
{
    using Utils = androidx.constraintlayout.core.motion.utils.Utils;
    using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;
    using ConstraintWidgetContainer = androidx.constraintlayout.core.widgets.ConstraintWidgetContainer;
    using Guideline = androidx.constraintlayout.core.widgets.Guideline;
    using Optimizer = androidx.constraintlayout.core.widgets.Optimizer;
    using BasicMeasure = androidx.constraintlayout.core.widgets.analyzer.BasicMeasure;

    /// <summary>
    /// WPF implementation of ConstraintLayout copy from Swing's ConstraintLayout
    /// </summary>
    public class ConstraintLayout : Panel
    {
        public bool DEBUG = false;//if is true,will print some layout info.
        public bool TEST = false;//if is true,will print time of measure+layout spend.
        private readonly ConstraintWidgetContainer mLayout = new ConstraintWidgetContainer();//default widget
        private readonly Dictionary<UIElement, string> viewsToIds = new Dictionary<UIElement, string>();
        private readonly Dictionary<string, ConstraintWidget> idsToConstraintWidgets = new Dictionary<string, ConstraintWidget>();


        public ConstraintLayout()
        {
            //constraintLayout have default Widget,need add it to dictionary,we can use it like other child later.
            var comp = this as UIElement;
            string id = comp.GetHashCode().ToString();
            ConstraintWidget constraintWidget = mLayout;
            constraintWidget.stringId = id;
            constraintWidget.CompanionWidget = comp;
            idsToConstraintWidgets[id] = constraintWidget;
            viewsToIds[comp] = id;

            mLayout.Measurer = new MeasurerAnonymousInnerClass(this);

            ClipToBounds = true;//view in ConstraintLayout always easy out of bounds
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            // Track when objects are added and removed
            if (visualAdded != null)
            {
                // Do stuff with the added object
                if (visualAdded is GuideLine)//GuideLine have default widget
                {
                    var comp = visualAdded as GuideLine;
                    string id = comp.GetHashCode().ToString();
                    ConstraintWidget constraintWidget = comp.Guideline;
                    mLayout.add(constraintWidget);
                    constraintWidget.stringId = id;
                    constraintWidget.CompanionWidget = comp;
                    idsToConstraintWidgets[id] = constraintWidget;
                    viewsToIds[comp] = id;
                }

                else
                {
                    var comp = visualAdded as UIElement;
                    string id = comp.GetHashCode().ToString();
                    ConstraintWidget constraintWidget = new ConstraintWidget();
                    //set default width/height is WRAP_CONTENT
                    constraintWidget.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
                    constraintWidget.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
                    mLayout.add(constraintWidget);
                    constraintWidget.stringId = id;
                    constraintWidget.CompanionWidget = comp;
                    idsToConstraintWidgets[id] = constraintWidget;
                    viewsToIds[comp] = id;
                }
            }

            if (visualRemoved != null)
            {
                // Do stuff with the removed object
                var comp = visualRemoved as UIElement;
                string id = viewsToIds[comp];
                ConstraintWidget widget = idsToConstraintWidgets[id];
                mLayout.remove(widget);
                idsToConstraintWidgets.Remove(id);
                viewsToIds.Remove(comp);
            }

            // Call base function
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }

        Stopwatch swTest = null;
        /// <summary>
        /// current times, sum of spend time,limited times
        /// </summary>
        (int, List<long>, int) countTestData = (0, new List<long>(), 10);
        bool isInfinity = false;
        protected override Size MeasureOverride(Size availableSize)
        {
            if (TEST)
            {
                if (swTest != null && countTestData.Item1 <= countTestData.Item3)
                {
                    countTestData.Item1++;
                    countTestData.Item2.Add(swTest.ElapsedTicks);
                    //swTest.Restart();
                }

                //if (swTest == null)
                {
                    swTest = Stopwatch.StartNew();
                }
            }

            //first measure all child size,we need know some default size.
            foreach (UIElement child in InternalChildren)
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

            mLayout.layout();
            //mLayout.OptimizationLevel = Optimizer.OPTIMIZATION_STANDARD;
            //mLayout.measure(Optimizer.OPTIMIZATION_STANDARD, BasicMeasure.EXACTLY, mLayout.Width, BasicMeasure.EXACTLY, mLayout.Height, 0, 0, 0, 0);

            double w;
            double h;
            //now we know all view corrected size in constaint, so give it to child,let them to caculate their child.
            foreach (UIElement child in InternalChildren)
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
            }

            if (DEBUG)
            {
                Debug.WriteLine($"{(this.Parent as FrameworkElement).Tag as string},Size {(this.Parent as FrameworkElement).Width},{(this.Parent as FrameworkElement).Height},DesiredSize {(this.Parent as FrameworkElement).DesiredSize}");
                Debug.WriteLine($"{this.Tag as string},availableSize {availableSize},DesiredSize {this.DesiredSize},ContainerWidgetSize {mLayout.Width},{mLayout.Height}");
                foreach (UIElement child in InternalChildren)
                {
                    var view = child as FrameworkElement;
                    Debug.WriteLine($"{view?.Tag as string},Size {view?.Width},{view?.Height}, DesiredSize {view?.DesiredSize}");
                }
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

        protected override Size ArrangeOverride(Size finalSize)
        {

            if (DEBUG)
                Debug.WriteLine($"{this.Tag as string},finalSize {finalSize}, DesiredSize {this.DesiredSize}");
            
            //recalculate?,because when constraintlayout size be define by parent,it size need parent to arrange
            //such as it as child of listview,listview will send double.infinity to measure,if you set listview's content to strenth,
            //you need get that size at Arrage. 
            if (isInfinity)//only when parent give me measure size is size isInfinity,we recalculate layout.
            {
                mLayout.Width = (int)finalSize.Width;
                mLayout.Height = (int)finalSize.Height;
                mLayout.layout();
                mLayout.OptimizationLevel = Optimizer.OPTIMIZATION_STANDARD;
                mLayout.measure(Optimizer.OPTIMIZATION_STANDARD, BasicMeasure.EXACTLY, (int)finalSize.Width, BasicMeasure.EXACTLY, (int)finalSize.Height, 0, 0, 0, 0);
                isInfinity = false;
            }

            //layout child
            foreach (ConstraintWidget child in mLayout.Children)
            {
                UIElement component = (UIElement)child.CompanionWidget;
                if (component != null)
                {
                    if (DEBUG)
                        Debug.WriteLine($"{(component as FrameworkElement)?.Tag as string} arrange " + child.X + " " + child.Y + " " + child.Width + " " + child.Height);
                    component.Arrange(new Rect(child.X, child.Y, child.Width, child.Height));
                }
            }

            if (TEST)
            {
                swTest.Stop();
                if (countTestData.Item1 == countTestData.Item3)
                {
                    long nanosecPerTick = (1000L * 1000L * 1000L) / Stopwatch.Frequency;
                    long count = 0;
                    foreach (var item in countTestData.Item2)
                    {
                        count += item;

                    }
                    Debug.WriteLine($"{this.Tag as string}, Count {countTestData.Item1}Times, Single Measure+Layout Average Spend Time: {(count * nanosecPerTick * 1.0) / 1000000 / countTestData.Item3}ms");
                    countTestData.Item1 = 0;
                    countTestData.Item2.Clear();
                }
            }
            return finalSize;
        }


        /// <summary>
        /// default widget of ConstraintLayout
        /// </summary>
        public virtual ConstraintWidgetContainer Root
        {
            get
            {
                return mLayout;
            }
        }

        /// <summary>
        /// get widget of this view
        /// </summary>
        /// <param name="view"></param>
        /// <returns>widget is a virtual control for caculate constraint</returns>
        internal ConstraintWidget GetWidget(UIElement view)
        {
            return this.idsToConstraintWidgets[this.viewsToIds[view]];
        }

        //convert from java
        private class MeasurerAnonymousInnerClass : BasicMeasure.Measurer
        {
            private readonly ConstraintLayout outerInstance;

            public MeasurerAnonymousInnerClass(ConstraintLayout outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public virtual void measure(ConstraintWidget widget, BasicMeasure.Measure measure)
            {
                outerInstance.innerMeasure(widget, measure);
            }

            public virtual void didMeasures()
            {

            }
        }

        private void innerMeasure(ConstraintWidget constraintWidget, BasicMeasure.Measure measure)
        {
            UIElement component = (UIElement)constraintWidget.CompanionWidget;
            int measuredWidth = constraintWidget.Width;
            int measuredHeight = constraintWidget.Height;
            //if (DEBUG)
            //Debug.WriteLine($"{(component as FrameworkElement)?.Tag as string} measureWidget " + measuredWidth + " " + measuredHeight);
            if (measure.horizontalBehavior == ConstraintWidget.DimensionBehaviour.WRAP_CONTENT)
            {
                //measuredWidth = component.MinimumSize.width;
                measuredWidth = (int)component.DesiredSize.Width;
            }
            else if (measure.horizontalBehavior == ConstraintWidget.DimensionBehaviour.MATCH_PARENT)
            {
                measuredWidth = mLayout.Width;
            }
            if (measure.verticalBehavior == ConstraintWidget.DimensionBehaviour.WRAP_CONTENT)
            {
                //measuredHeight = component.MinimumSize.height;
                measuredHeight = (int)component.DesiredSize.Height;
            }
            else if (measure.verticalBehavior == ConstraintWidget.DimensionBehaviour.MATCH_PARENT)
            {
                measuredHeight = mLayout.Height;
            }
            measure.measuredWidth = measuredWidth;
            measure.measuredHeight = measuredHeight;
        }
    }
}
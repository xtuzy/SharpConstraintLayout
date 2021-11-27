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

namespace SharpConstraintLayout.Example.Reload
{
    using Utils = androidx.constraintlayout.core.motion.utils.Utils;
    using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;
    using ConstraintWidgetContainer = androidx.constraintlayout.core.widgets.ConstraintWidgetContainer;
    using Guideline = androidx.constraintlayout.core.widgets.Guideline;
    using Optimizer = androidx.constraintlayout.core.widgets.Optimizer;
    using BasicMeasure = androidx.constraintlayout.core.widgets.analyzer.BasicMeasure;
    using Component = System.Windows.FrameworkElement;
    using Container = DependencyObject;

    public delegate void CustomLayoutDelegate(ZLayout container);
    /// <summary>
    /// Basic implementation of ConstraintLayout as a Swing LayoutManager
    /// </summary>
    public class ZLayout : Panel
    {
        private const bool DEBUG = false;
        private readonly ConstraintWidgetContainer mLayout = new ConstraintWidgetContainer();
        private readonly Dictionary<Component, string> viewsToIds = new Dictionary<Component, string>();
        private readonly Dictionary<string, ConstraintWidget> idsToConstraintWidgets = new Dictionary<string, ConstraintWidget>();

        public ZLayout()
        {
            //ConstraintLayout具有自己的Widget,需要将自己也加入
            var comp = this as FrameworkElement;
            string id = comp.GetHashCode().ToString();
            ConstraintWidget constraintWidget = mLayout;
            constraintWidget.stringId = id;
            constraintWidget.CompanionWidget = comp;
            idsToConstraintWidgets[id] = constraintWidget;
            viewsToIds[comp] = id;

            mLayout.Measurer = new MeasurerAnonymousInnerClass(this);
        }

        

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            // Track when objects are added and removed
            if (visualAdded != null)
            {
                // Do stuff with the added object
                if(visualAdded is GuideLine)//GuideLine具有自己的控件
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
                    var comp = visualAdded as FrameworkElement;
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

                var comp = visualRemoved as FrameworkElement;
                string id = viewsToIds[comp];
                ConstraintWidget widget = idsToConstraintWidgets[id];
                mLayout.remove(widget);
                idsToConstraintWidgets.Remove(id);
                viewsToIds.Remove(comp);
            }

            // Call base function
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child in InternalChildren)
            {
                if (!child.IsMeasureValid)
                    child.Measure(availableSize);
            }
            return availableSize;
        }

        public CustomLayoutDelegate CustomLayout;
        public bool isHadDefine = false;
        protected override Size ArrangeOverride(Size finalSize)
        {
            //return base.ArrangeOverride(finalSize);
            int width = (int)finalSize.Width;
            int height = (int)finalSize.Height;
            
            mLayout.Width = width;
            mLayout.Height = height;
            if (CustomLayout != null && isHadDefine == false)//自定义布局,在中间可以利用约束布局计算位置再调整
            {
                CustomLayout.Invoke(this);
                //isHadDefine = true;
            }
            mLayout.layout();
            mLayout.OptimizationLevel = Optimizer.OPTIMIZATION_STANDARD;
            mLayout.measure(Optimizer.OPTIMIZATION_STANDARD, BasicMeasure.EXACTLY, width, BasicMeasure.EXACTLY, height, 0, 0, 0, 0);

            foreach (ConstraintWidget child in mLayout.Children)
            {
                Component component = (Component)child.CompanionWidget;
                if (component != null)
                {
                    if (DEBUG)
                    {
                        Debug.WriteLine("applying "+child.X + " " + child.Y + " " + child.Width + " " + child.Height);
                    }
                    component.Arrange(new Rect( child.X, child.Y, child.Width, child.Height));
                }
            }

            return finalSize;
        }

        public virtual ConstraintWidgetContainer Root
        {
            get
            {
                return mLayout;
            }
        }

        public ConstraintWidget GetWidget(Component view)
        {
            return this.idsToConstraintWidgets[this.viewsToIds[view]];
        }

        public Rect GetCustomLayoutInfo(Component view)
        {
            var child = GetWidget(view);
            return new Rect(child.X, child.Y, child.Width, child.Height);
        }

        public void UpdateCustomLayoutInfo()
        {
            mLayout.layout();
        }

        private class MeasurerAnonymousInnerClass : BasicMeasure.Measurer
        {
            private readonly ZLayout outerInstance;

            public MeasurerAnonymousInnerClass(ZLayout outerInstance)
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
            Component component = (Component)constraintWidget.CompanionWidget;
            int measuredWidth = constraintWidget.Width;
            int measuredHeight = constraintWidget.Height;
            if (DEBUG)
            {
                Debug.WriteLine(" measure " + measuredWidth + " " + measuredHeight);
            }
            if (measure.horizontalBehavior == ConstraintWidget.DimensionBehaviour.WRAP_CONTENT)
            {
                //measuredWidth = component.MinimumSize.width;
                measuredWidth = (int)component.DesiredSize.Width;//DesiredSize应该是测量的默认大小
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
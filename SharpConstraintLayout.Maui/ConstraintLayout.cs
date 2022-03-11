using androidx.constraintlayout.core.widgets;
using FontBaseline.Maui.Skia;
using Microsoft.Maui.Layouts;
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

namespace SharpConstraintLayout.Maui
{
    using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;
    using ConstraintWidgetContainer = androidx.constraintlayout.core.widgets.ConstraintWidgetContainer;
    using Optimizer = androidx.constraintlayout.core.widgets.Optimizer;
    using BasicMeasure = androidx.constraintlayout.core.widgets.analyzer.BasicMeasure;
    using Size = Microsoft.Maui.Graphics.Size;

    /// <summary>
    /// WPF implementation of ConstraintLayout copy from Swing's ConstraintLayout
    /// </summary>
    public class ConstraintLayout : AbsoluteLayout,IConstraintLayout
    {
        bool debug=true;
        public bool DEBUG { set => debug = value; get => debug; }//if is true,will print some layout info.
        bool test = false;
        public bool TEST { set => test = value; get => test; }//if is true,will print time of measure+layout spend.

        private readonly ConstraintWidgetContainer mLayout = new ConstraintWidgetContainer();//default widget
        private readonly Dictionary<UIElement, string> viewsToIds = new Dictionary<UIElement, string>();
        private readonly Dictionary<string, ConstraintWidget> idsToConstraintWidgets = new Dictionary<string, ConstraintWidget>();

        /// <summary>
        /// default widget of ConstraintLayout.
        /// you can constrol more action by it.
        /// </summary>
        public virtual ConstraintWidgetContainer Root
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
            return this.idsToConstraintWidgets[this.viewsToIds[view]];
        }

        /// <summary>
        /// Set width of ConstraintLayout self.<br/>
        /// Default is <see cref="ConstraintSet.SizeType.WrapContent"/>.<br/>
        /// Notice:It is different <see cref="ConstraintSetExtensions.SetWidth(FrameworkElement, ConstraintSet.SizeType, float)(FrameworkElement, ConstraintSet.SizeType, float)"/>.
        /// </summary>
        /// <param name="fromView"></param>
        /// <param name="constraint"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public ConstraintSet.SizeType WidthType
        {
            set
            {
                var fromWidget = this.Root;
                fromWidget.HorizontalDimensionBehaviour = ConstraintSet.ConstraintSizeTypeDic[(int)value];
            }
        }

        /// <summary>
        /// Set height of ConstraintLayout self.<br/>
        /// Default is <see cref="ConstraintSet.SizeType.WrapContent"/>.<br/>
        /// Notice:It is different <see cref="ConstraintSetExtensions.SetHeight(FrameworkElement, ConstraintSet.SizeType, float)"/>.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public ConstraintSet.SizeType HeightType
        {
            set
            {
                var fromWidget = this.Root;
                fromWidget.VerticalDimensionBehaviour = ConstraintSet.ConstraintSizeTypeDic[(int)value];
            }
        }

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

            //ClipToBounds = true;//view in ConstraintLayout always easy out of bounds
        }

        protected override void OnChildAdded(Element child)
        {
            OnVisualChildrenChanged(child, null);
            base.OnChildAdded(child);
        }

        protected override void OnChildRemoved(Element child, int oldLogicalIndex)
        {
            OnVisualChildrenChanged(null, child);
            base.OnChildRemoved(child, oldLogicalIndex);
        }

        protected void OnVisualChildrenChanged(Element visualAdded, Element visualRemoved)
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
                else if (visualAdded is BarrierLine)
                {
                    var comp = visualAdded as BarrierLine;
                    string id = comp.GetHashCode().ToString();
                    ConstraintWidget constraintWidget = comp.Barrier;
                    mLayout.add(constraintWidget);
                    constraintWidget.stringId = id;
                    constraintWidget.CompanionWidget = comp;
                    idsToConstraintWidgets[id] = constraintWidget;
                    viewsToIds[comp] = id;
                }
                else if (visualAdded is FlowBox)
                {
                    var comp = visualAdded as FlowBox;
                    string id = comp.GetHashCode().ToString();
                    ConstraintWidget constraintWidget = comp.Flow;
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
            //base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }

        protected override ILayoutManager CreateLayoutManager()
        {
            return new ConstraintLayoutManager(this);
        }

        //copy from swing
        private class MeasurerAnonymousInnerClass : BasicMeasure.Measurer
        {
            private readonly ConstraintLayout outerInstance;

            public MeasurerAnonymousInnerClass(ConstraintLayout outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public virtual void measure(ConstraintWidget widget, BasicMeasure.Measure measure)
            {
                if (widget is VirtualLayout)
                    outerInstance.measureFlow(widget, measure);
                else
                    outerInstance.innerMeasure(widget, measure);
            }

            public virtual void didMeasures()
            {

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
                //measuredWidth = component.MinimumSize.width;
                measuredWidth = (int)(component.DesiredSize.Width + 0.5);
            }
            else if (measure.horizontalBehavior == ConstraintWidget.DimensionBehaviour.MATCH_PARENT)
            {
                measuredWidth = mLayout.Width;
            }
            if (measure.verticalBehavior == ConstraintWidget.DimensionBehaviour.WRAP_CONTENT)
            {
                //measuredHeight = component.MinimumSize.height;
                measuredHeight = (int)(component.DesiredSize.Height + 0.5);
            }
            else if (measure.verticalBehavior == ConstraintWidget.DimensionBehaviour.MATCH_PARENT)
            {
                measuredHeight = mLayout.Height;
            }
            measure.measuredWidth = measuredWidth;
            measure.measuredHeight = measuredHeight;

            //baseline
            double baselineToTopHeight = 0;
            if (component is Button)
            {
                /**
                 * Button text can set VerticalContentAlignment center bottom top 
                 */
                var button = (Button)component;
                /* if (!(button.Content is string))
                     return;
                 */

                (double fontSize, float baselineToTextCenterHeight) = new FontBaselineHelper().GetBaseline(button);

                baselineToTopHeight = button.DesiredSize.Height / 2 + baselineToTextCenterHeight;
                measure.measuredBaseline = (int)baselineToTopHeight;
            }
            else if (component is ITextAlignment)
            {
                var textView = (Microsoft.Maui.Controls.Internals.IFontElement)component;
                if (textView == null) return;
                (double fontSize, float baselineToTextCenterHeight) = new FontBaselineHelper().GetBaseline(textView);
                var textAlignment = component as ITextAlignment;
                if (textAlignment.VerticalTextAlignment == TextAlignment.Start)
                {
                    baselineToTopHeight = textView.FontSize / 2 + baselineToTextCenterHeight;
                }
                else if (textAlignment.VerticalTextAlignment == TextAlignment.Center)
                {
                    baselineToTopHeight = ((VisualElement)component).DesiredSize.Height / 2 + baselineToTextCenterHeight;
                }
                else if (textAlignment.VerticalTextAlignment == TextAlignment.End)
                {
                    baselineToTopHeight = ((VisualElement)component).DesiredSize.Height - (textView.FontSize / 2 - baselineToTextCenterHeight);

                }
                measure.measuredBaseline = (int)baselineToTopHeight;
            }
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

            if (widget is VirtualLayout)
            {
                VirtualLayout layout = (VirtualLayout)widget;
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

    }
}
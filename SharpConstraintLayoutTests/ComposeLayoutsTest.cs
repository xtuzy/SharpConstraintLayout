using NUnit.Framework;
using System;

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

namespace androidx.constraintlayout.core
{
    using ConstraintReference = androidx.constraintlayout.core.state.ConstraintReference;
    using Dimension = androidx.constraintlayout.core.state.Dimension;
    using State = androidx.constraintlayout.core.state.State;
    using GuidelineReference = androidx.constraintlayout.core.state.helpers.GuidelineReference;
    using ConstraintAnchor = androidx.constraintlayout.core.widgets.ConstraintAnchor;
    using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;
    using ConstraintWidgetContainer = androidx.constraintlayout.core.widgets.ConstraintWidgetContainer;
    using Guideline = androidx.constraintlayout.core.widgets.Guideline;
    using Optimizer = androidx.constraintlayout.core.widgets.Optimizer;
    using BasicMeasure = androidx.constraintlayout.core.widgets.analyzer.BasicMeasure;

    //using Test = org.junit.Test;

    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.junit.Assert.Assert.AreEqual;
    [TestFixture]
    public class ComposeLayoutsTest
    {

        internal static BasicMeasure.Measurer sMeasurer = new MeasurerAnonymousInnerClass();

        private class MeasurerAnonymousInnerClass : BasicMeasure.Measurer
        {
            public MeasurerAnonymousInnerClass()
            {
            }


            public virtual void measure(ConstraintWidget widget, BasicMeasure.Measure measure)
            {
                ConstraintWidget.DimensionBehaviour horizontalBehavior = measure.horizontalBehavior;
                ConstraintWidget.DimensionBehaviour verticalBehavior = measure.verticalBehavior;
                int horizontalDimension = measure.horizontalDimension;
                int verticalDimension = measure.verticalDimension;

                Console.WriteLine("Measure (strategy : " + measure.measureStrategy + ") : " + widget.CompanionWidget + " " + horizontalBehavior + " (" + horizontalDimension + ") x " + verticalBehavior + " (" + verticalDimension + ")");

                if (horizontalBehavior == ConstraintWidget.DimensionBehaviour.FIXED)
                {
                    measure.measuredWidth = horizontalDimension;
                }
                else if (horizontalBehavior == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT)
                {
                    measure.measuredWidth = horizontalDimension;
                    if (widget.CompanionWidget.Equals("box") && measure.measureStrategy == BasicMeasure.Measure.SELF_DIMENSIONS)
                    {
                        // && measure.measuredWidth == 0
                        measure.measuredWidth = 1080;
                    }
                }
                else if (horizontalBehavior == ConstraintWidget.DimensionBehaviour.WRAP_CONTENT)
                {
                    if (widget.CompanionWidget.Equals("box"))
                    {
                        measure.measuredWidth = 1080;
                    }
                }
                if (verticalBehavior == ConstraintWidget.DimensionBehaviour.FIXED)
                {
                    measure.measuredHeight = verticalDimension;
                }
                else if (verticalBehavior == ConstraintWidget.DimensionBehaviour.WRAP_CONTENT)
                {
                    if (widget.CompanionWidget.Equals("box"))
                    {
                        measure.measuredHeight = measure.measuredWidth / 2;
                    }
                }
                Console.WriteLine("Measure widget " + widget.CompanionWidget + " => " + measure.measuredWidth + " x " + measure.measuredHeight);
            }

            public virtual void didMeasures()
            {

            }
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void dividerMatchTextHeight_inWrapConstraintLayout_longText()
        [Test]
        public virtual void dividerMatchTextHeight_inWrapConstraintLayout_longText()
        {
            State state = new State();
            ConstraintReference parent = state.constraints(State.PARENT);
            state.verticalGuideline("guideline").percent(0.5f);

            state.constraints("box").centerHorizontally(parent).centerVertically(parent).startToEnd("guideline").width(Dimension.Suggested(Dimension.WRAP_DIMENSION)).height(Dimension.Wrap()).View = "box";
            state.constraints("divider").centerHorizontally(parent).centerVertically(parent).width(Dimension.Fixed(1)).height(Dimension.Percent(0, 0.8f).suggested(0)).View = "divider";
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1080, 1977);
            state.apply(root);
            root.Width = 1080;
            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
            ConstraintWidget box = state.constraints("box").ConstraintWidget;
            ConstraintWidget guideline = state.guideline("guideline", ConstraintWidget.VERTICAL).ConstraintWidget;
            ConstraintWidget divider = state.constraints("divider").ConstraintWidget;
            root.DebugName = "root";
            box.DebugName = "box";
            guideline.DebugName = "guideline";
            divider.DebugName = "divider";

            root.Measurer = sMeasurer;
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.OptimizationLevel = Optimizer.OPTIMIZATION_STANDARD;
            //root.setOptimizationLevel(Optimizer.OPTIMIZATION_NONE);
            root.measure(root.OptimizationLevel, 0, 0, 0, 0, 0, 0, 0, 0);

            Console.WriteLine("root: " + root);
            Console.WriteLine("box: " + box);
            Console.WriteLine("guideline: " + guideline);
            Console.WriteLine("divider: " + divider);

            Assert.AreEqual(root.Width / 2, box.Width);
            Assert.AreEqual(root.Width / 2 / 2, box.Height);
            Assert.AreEqual(1, divider.Width);
            Assert.AreEqual((int)(box.Height * 0.8), divider.Height);
        }
    }

}
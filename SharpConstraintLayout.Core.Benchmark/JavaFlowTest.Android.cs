
using System;
using System.Collections.Generic;

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

namespace SharpConstraintLayout.Core.Benchmark
{
    using ConstraintAnchor = AndroidX.ConstraintLayout.Core.Widgets.ConstraintAnchor;
    using ConstraintWidget = AndroidX.ConstraintLayout.Core.Widgets.ConstraintWidget;
    using ConstraintWidgetContainer = AndroidX.ConstraintLayout.Core.Widgets.ConstraintWidgetContainer;
    using Flow = AndroidX.ConstraintLayout.Core.Widgets.Flow;
    using Optimizer = AndroidX.ConstraintLayout.Core.Widgets.Optimizer;
    using VirtualLayout = AndroidX.ConstraintLayout.Core.Widgets.VirtualLayout;
    using BasicMeasure = AndroidX.ConstraintLayout.Core.Widgets.Analyzer.BasicMeasure;

    //using Test = org.junit.Test;

    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static androidx.constraintlayout.core.widgets.analyzer.BasicMeasure.EXACTLY;
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static androidx.constraintlayout.core.widgets.analyzer.BasicMeasure.UNSPECIFIED;
    using static androidx.constraintlayout.core.widgets.analyzer.BasicMeasure;
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.junit.Assert.Assert.AreEqual;
    public class JavaFlowTest
    {
        internal static BasicMeasure.IMeasurer sMeasurer = new MeasurerAnonymousInnerClass();

        private class MeasurerAnonymousInnerClass :Java.Lang.Object, BasicMeasure.IMeasurer
        {
            public MeasurerAnonymousInnerClass()
            {
            }

            public virtual void Measure(ConstraintWidget widget, BasicMeasure.Measure measure)
            {
                ConstraintWidget.DimensionBehaviour horizontalBehavior = measure.HorizontalBehavior;
                ConstraintWidget.DimensionBehaviour verticalBehavior = measure.VerticalBehavior;
                int horizontalDimension = measure.HorizontalDimension;
                int verticalDimension = measure.VerticalDimension;

                if (widget is VirtualLayout)
                {
                    VirtualLayout layout = (VirtualLayout)widget;
                    int widthMode = UNSPECIFIED;
                    int heightMode = UNSPECIFIED;
                    int widthSize = 0;
                    int heightSize = 0;
                    if (layout.HorizontalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MatchParent)
                    {
                        widthSize = layout.Parent != null ? layout.Parent.Width : 0;
                        widthMode = EXACTLY;
                    }
                    else if (horizontalBehavior == ConstraintWidget.DimensionBehaviour.Fixed)
                    {
                        widthSize = horizontalDimension;
                        widthMode = EXACTLY;
                    }
                    if (layout.VerticalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MatchParent)
                    {
                        heightSize = layout.Parent != null ? layout.Parent.Height : 0;
                        heightMode = EXACTLY;
                    }
                    else if (verticalBehavior == ConstraintWidget.DimensionBehaviour.Fixed)
                    {
                        heightSize = verticalDimension;
                        heightMode = EXACTLY;
                    }
                    layout.Measure(widthMode, widthSize, heightMode, heightSize);
                    measure.MeasuredWidth = layout.MeasuredWidth;
                    measure.MeasuredHeight = layout.MeasuredHeight;
                }
                else
                {
                    if (horizontalBehavior == ConstraintWidget.DimensionBehaviour.Fixed)
                    {
                        measure.MeasuredWidth = horizontalDimension;
                    }
                    if (verticalBehavior == ConstraintWidget.DimensionBehaviour.Fixed)
                    {
                        measure.MeasuredHeight = verticalDimension;
                    }
                }
            }

            public virtual void DidMeasures()
            {

            }
        }

        public virtual void TestFlowWrapNone(int childCount = 100)
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1080, 1500)
            {
                DebugName = "root",
            };
            root.Measurer = sMeasurer;

            Flow flow = new Flow();
            flow.DebugName = "Flow";

            root.Add(flow);

            var buttonCount = childCount;
            var buttonList = new List<ConstraintWidget>();
            for (int i = 0; i < buttonCount; i++)
            {
                var button = new ConstraintWidget() { Width = 20, Height = 20 };
                button.DebugName = "Button" + i;
                buttonList.Add(button);
                root.Add(button);
                flow.Add(button);
            }

            flow.SetOrientation(Flow.Horizontal);
            flow.SetWrapMode(Flow.WrapNone);
            flow.SetHorizontalStyle(Flow.ChainPacked);

            flow.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WrapContent;
            flow.Connect(ConstraintAnchor.Type.Top, root, ConstraintAnchor.Type.Top);
            flow.Connect(ConstraintAnchor.Type.Bottom, root, ConstraintAnchor.Type.Bottom);
            flow.Connect(ConstraintAnchor.Type.Left, root, ConstraintAnchor.Type.Left);

            //Console.WriteLine("Flow WRAP_CONTENT 水平居中");
            flow.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WrapContent;
            flow.Connect(ConstraintAnchor.Type.Right, root, ConstraintAnchor.Type.Right);

            var measureTime = DateTimeHelperClass.CurrentUnixTimeMillis();
            root.Measure(Optimizer.OptimizationNone, 0, 0, 0, 0, 0, 0, 0, 0);
            root.Layout();
        }

    }

}
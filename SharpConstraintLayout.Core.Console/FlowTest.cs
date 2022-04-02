
using BenchmarkDotNet.Attributes;
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
    using ConstraintAnchor = androidx.constraintlayout.core.widgets.ConstraintAnchor;
    using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;
    using ConstraintWidgetContainer = androidx.constraintlayout.core.widgets.ConstraintWidgetContainer;
    using Flow = androidx.constraintlayout.core.widgets.Flow;
    using Optimizer = androidx.constraintlayout.core.widgets.Optimizer;
    using VirtualLayout = androidx.constraintlayout.core.widgets.VirtualLayout;
    using BasicMeasure = androidx.constraintlayout.core.widgets.analyzer.BasicMeasure;

    //using Test = org.junit.Test;

    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static androidx.constraintlayout.core.widgets.analyzer.BasicMeasure.EXACTLY;
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static androidx.constraintlayout.core.widgets.analyzer.BasicMeasure.UNSPECIFIED;
    using static androidx.constraintlayout.core.widgets.analyzer.BasicMeasure;
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.junit.Assert.Assert.AreEqual;
    public class FlowTest
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

                if (widget is VirtualLayout)
                {
                    VirtualLayout layout = (VirtualLayout)widget;
                    int widthMode = UNSPECIFIED;
                    int heightMode = UNSPECIFIED;
                    int widthSize = 0;
                    int heightSize = 0;
                    if (layout.HorizontalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_PARENT)
                    {
                        widthSize = layout.Parent != null ? layout.Parent.Width : 0;
                        widthMode = EXACTLY;
                    }
                    else if (horizontalBehavior == ConstraintWidget.DimensionBehaviour.FIXED)
                    {
                        widthSize = horizontalDimension;
                        widthMode = EXACTLY;
                    }
                    if (layout.VerticalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_PARENT)
                    {
                        heightSize = layout.Parent != null ? layout.Parent.Height : 0;
                        heightMode = EXACTLY;
                    }
                    else if (verticalBehavior == ConstraintWidget.DimensionBehaviour.FIXED)
                    {
                        heightSize = verticalDimension;
                        heightMode = EXACTLY;
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

            public virtual void didMeasures()
            {

            }
        }

        [Benchmark]
        public void BenchmarkFlowWrapNone()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1080, 1500)
            {
                //DebugName = "root",
            };
            root.Measurer = sMeasurer;

            Flow flow = new Flow();
            //flow.DebugName = "Flow";

            root.add(flow);

            var buttonCount = 1000;
            var buttonList = new List<ConstraintWidget>();
            for (int i = 0; i < buttonCount; i++)
            {
                var button = new ConstraintWidget() { Width = 20, Height = 20 };
                //button.DebugName = "Button" + i;
                buttonList.Add(button);
                root.add(button);
                flow.add(button);
            }

            flow.Orientation = Flow.HORIZONTAL;
            flow.WrapMode = Flow.WRAP_NONE;
            flow.HorizontalStyle = Flow.CHAIN_PACKED;
            flow.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            flow.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            flow.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            flow.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            flow.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            flow.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            root.measure(Optimizer.OPTIMIZATION_NONE, 0, 0, 0, 0, 0, 0, 0, 0);
            root.layout();
        }

        ConstraintWidgetContainer root;
        List<ConstraintWidget> buttonList;
        /// <summary>
        /// 用来对比Java的测试时间,Java单元测试不知道是不是在Release模式下,Measure很快(第一次87,改变Anchor后47),比C#的单元测试输出(130,99)的快.
        /// 所以我用Console.WriteLine来输出测试时间,看看是不是在Release模式下,能快点.
        /// Debug(120,87)
        /// Release(97,37)是最好的
        /// </summary>
        public virtual void measureFlowWrapNoneMemoryAnalysis1000Widget(int w = 0, int h = 0)
        {
            if (root == null)
            {
                var measureTime = DateTimeHelperClass.CurrentUnixTimeMillis();
                root = new ConstraintWidgetContainer(0, 0, 1080, 1500)
                {
                    DebugName = "root",
                    Measurer = sMeasurer
                };

                Flow flow = new Flow();
                flow.DebugName = "Flow";

                root.add(flow);

                var buttonCount = 1000;
                buttonList = new List<ConstraintWidget>();
                for (int i = 0; i < buttonCount; i++)
                {
                    var button = new ConstraintWidget() { Width = 20, Height = 20 };
                    button.DebugName = "Button" + i;
                    buttonList.Add(button);
                    root.add(button);
                    flow.add(button);
                }

                flow.Orientation = Flow.HORIZONTAL;
                flow.WrapMode = Flow.WRAP_NONE;
                flow.HorizontalStyle = Flow.CHAIN_PACKED;

                flow.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
                flow.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
                flow.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
                flow.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);

                Console.WriteLine("Flow WRAP_CONTENT 水平居中");
                flow.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
                flow.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);

                Console.WriteLine("Add 1000 Widget Time: " + (DateTimeHelperClass.CurrentUnixTimeMillis() - measureTime));
                measureTime = DateTimeHelperClass.CurrentUnixTimeMillis();
                root.measure(Optimizer.OPTIMIZATION_NONE, 0, 0, 0, 0, 0, 0, 0, 0);
                root.layout();
                Console.WriteLine("Flow Measure Time: " + (DateTimeHelperClass.CurrentUnixTimeMillis() - measureTime));

                Console.WriteLine("a) root: " + root);
                Console.WriteLine("flow: " + flow);
                Console.WriteLine("buttonList[0]: " + buttonList[0]);
                Console.WriteLine("buttonList[lasted]: " + buttonList[buttonCount - 1]);
                SimpleTest.AreEqual(buttonList[buttonCount - 1].Left - 1080, -buttonList[0].Right, 2, nameof(BenchmarkFlowWrapNone));//水平居中

                Console.WriteLine("Flow WRAP_CONTENT 靠Parent左");

                measureTime = DateTimeHelperClass.CurrentUnixTimeMillis();
                flow.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
                flow.resetAnchor(flow.getAnchor(ConstraintAnchor.Type.RIGHT));
                Console.WriteLine("Flow Change Anchor Time: " + (DateTimeHelperClass.CurrentUnixTimeMillis() - measureTime));

                measureTime = DateTimeHelperClass.CurrentUnixTimeMillis();
                root.measure(Optimizer.OPTIMIZATION_NONE, 0, 0, 0, 0, 0, 0, 0, 0);
                root.layout();
                Console.WriteLine("Flow Measure Time: " + (DateTimeHelperClass.CurrentUnixTimeMillis() - measureTime));

                Console.WriteLine("a) root: " + root);
                Console.WriteLine("flow: " + flow);
                Console.WriteLine("buttonList[0]: " + buttonList[0]);
                Console.WriteLine("buttonList[lasted]: " + buttonList[buttonCount - 1]);
                SimpleTest.AreEqual(buttonList[0].Left, 0, nameof(BenchmarkFlowWrapNone));
            }
            else
            {
                //随机改变几个Control的大小
                var button1 = buttonList[new Random(h).Next(0, 1000 - 1)];
                button1.Width = w;
                button1.Height = h;
                var button2 = buttonList[new Random(w).Next(0, 1000 - 1)];
                button2.Width = w;
                button2.Height = h;
                var measureTime = DateTimeHelperClass.CurrentUnixTimeMillis();
                root.measure(Optimizer.OPTIMIZATION_NONE, 0, 0, 0, 0, 0, 0, 0, 0);
                root.layout();
                Console.WriteLine("Flow Measure Time: " + (DateTimeHelperClass.CurrentUnixTimeMillis() - measureTime));
            }
        }

        public virtual void measureFlowWrapChainMemoryAnalysis100x10Widget()
        {
            int w = 0;
            int h = 0;
            var measureTime = DateTimeHelperClass.CurrentUnixTimeMillis();
            ConstraintWidgetContainer Container = new ConstraintWidgetContainer(0, 0, 1080, 1500)
            {
                DebugName = "continer",
                Measurer = sMeasurer
            };
            Flow containerFlow = new Flow()
            {
                DebugName = "containerFlow",
                Orientation = Flow.HORIZONTAL,
                WrapMode = Flow.WRAP_CHAIN,
                HorizontalStyle = Flow.CHAIN_SPREAD,
                VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT,
            };
            containerFlow.connect(ConstraintAnchor.Type.TOP, Container, ConstraintAnchor.Type.TOP);
            containerFlow.connect(ConstraintAnchor.Type.BOTTOM, Container, ConstraintAnchor.Type.BOTTOM);
            containerFlow.connect(ConstraintAnchor.Type.LEFT, Container, ConstraintAnchor.Type.LEFT);
            containerFlow.connect(ConstraintAnchor.Type.RIGHT, Container, ConstraintAnchor.Type.RIGHT);
            Container.add(containerFlow);
            
            List<ConstraintWidgetContainer> constraintWidgetContainerList = new List<ConstraintWidgetContainer>();
            for (var index = 0; index<10;index++)
            {
                ConstraintWidgetContainer root;
                List<ConstraintWidget> buttonList;
                
                root = new ConstraintWidgetContainer(0, 0, 100, 150)
                {
                    DebugName = "root"+index,
                    Measurer = sMeasurer
                };
                constraintWidgetContainerList.Add(root);
                
                Flow flow = new Flow()
                {
                    DebugName = "Flow",
                    Orientation = Flow.HORIZONTAL,
                    WrapMode = Flow.WRAP_CHAIN,
                    HorizontalStyle = Flow.CHAIN_SPREAD,
                    VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT
                };
                flow.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
                flow.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
                flow.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
                root.add(flow);

                var buttonCount = 100;
                buttonList = new List<ConstraintWidget>();
                for (int i = 0; i < buttonCount; i++)
                {
                    var button = new ConstraintWidget() { Width = 20, Height = 20 };
                    //button.DebugName = "Button" + i;
                    buttonList.Add(button);
                    root.add(button);
                    flow.add(button);
                }
                
                root.measure(Optimizer.OPTIMIZATION_NONE, 0, 0, 0, 0, 0, 0, 0, 0);
                root.layout();
            }
            foreach (var item in constraintWidgetContainerList)
            {
                Container.add(item);
                containerFlow.add(item);
            }
            Container.measure(Optimizer.OPTIMIZATION_NONE, 0, 0, 0, 0, 0, 0, 0, 0);
            Container.layout();
            Console.WriteLine("Flow Add And Measure Time: " + (DateTimeHelperClass.CurrentUnixTimeMillis() - measureTime));
            Console.WriteLine($"{Container.DebugName}: {Container}");
            Console.WriteLine($"{containerFlow.DebugName}: {containerFlow}");
            foreach (var item in constraintWidgetContainerList)
            {
                Console.WriteLine($"{item.DebugName}: {item}");
            }
            measureTime = DateTimeHelperClass.CurrentUnixTimeMillis();
            Container.Width = 2000;
            Container.Height = 2000;
            Container.measure(Optimizer.OPTIMIZATION_NONE, 0, 0, 0, 0, 0, 0, 0, 0);
            Container.layout();
            Console.WriteLine("Flow Re Measure Time: " + (DateTimeHelperClass.CurrentUnixTimeMillis() - measureTime));
        }

        public void Measure_Views_Benchmark()
        {
            var measureTime = DateTimeHelperClass.CurrentUnixTimeMillis();
            ConstraintWidgetContainer Container = new ConstraintWidgetContainer(0, 0, 1000, 1000)
            {
                DebugName = "root",
                Measurer = sMeasurer
            };
            var guideline = new androidx.constraintlayout.core.widgets.Guideline()
            {
            };
            guideline.setGuidePercent(0.5f);
            guideline.Orientation = androidx.constraintlayout.core.widgets.Guideline.VERTICAL;
            Container.add(guideline);
            int rowCount = 50;
            int columCount = 10;
            List<ConstraintWidget> widgets = new List<ConstraintWidget>();
            for (var row = 0; row < rowCount; row++)
            {
                for (var colum = 0; colum < columCount; colum++)
                {
                    var child = new ConstraintWidget()
                    {
                    };
                    widgets.Add(child);
                    Container.add(child);
                    //size
                    child.Width = 20;
                    child.Height = 20;
                    child.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
                    child.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
                    //position
                    child.connect(ConstraintAnchor.Type.LEFT, guideline, ConstraintAnchor.Type.LEFT, row * 20);
                    child.connect(ConstraintAnchor.Type.TOP, Container, ConstraintAnchor.Type.TOP, colum * 20);                    
                }
            }
            Console.WriteLine($"ConstraintLayout Add {rowCount * columCount} View Spend Time:" + (DateTimeHelperClass.CurrentUnixTimeMillis() - measureTime) + "ms");
            measureTime = DateTimeHelperClass.CurrentUnixTimeMillis();
            Container.measure(Optimizer.OPTIMIZATION_NONE, 0, 0, 0, 0, 0, 0, 0, 0);
            Container.layout();
            Console.WriteLine($"ConstraintLayout First Measure {rowCount * columCount} View Spend Time:" + (DateTimeHelperClass.CurrentUnixTimeMillis() - measureTime) + "ms");
            measureTime = DateTimeHelperClass.CurrentUnixTimeMillis();
            Container.Height = 2000;
            Container.Width = 2000;
            Container.measure(Optimizer.OPTIMIZATION_NONE, 0, 0, 0, 0, 0, 0, 0, 0);
            Container.layout();
            Console.WriteLine($"ConstraintLayout Re Measure {rowCount * columCount} View Spend Time:" + (DateTimeHelperClass.CurrentUnixTimeMillis() - measureTime) + "ms");
            foreach (var widget in widgets)
            {
                Console.WriteLine($"{widget}");
            }
        }
    }
}
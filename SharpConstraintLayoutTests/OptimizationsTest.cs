using System;

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
namespace androidx.constraintlayout.core
{
    using androidx.constraintlayout.core.widgets;
    using Type = androidx.constraintlayout.core.widgets.ConstraintAnchor.Type;
    using DimensionBehaviour = androidx.constraintlayout.core.widgets.ConstraintWidget.DimensionBehaviour;
    using BasicMeasure = androidx.constraintlayout.core.widgets.analyzer.BasicMeasure;

    //using Test = org.junit.Test;
    using NUnit.Framework;

    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.junit.Assert.Assert.AreEqual;
    [TestFixture]
    public class OptimizationsTest
    {
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testGoneMatchConstraint()
        [Test]
        public virtual void testGoneMatchConstraint()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 800);
            ConstraintWidget A = new ConstraintWidget("A", 0, 10);
            ConstraintWidget B = new ConstraintWidget("B", 10, 10);
            root.DebugName = "root";

            root.add(A);
            root.add(B);

            A.connect(Type.TOP, root, Type.TOP, 8);
            A.connect(Type.LEFT, root, Type.LEFT, 8);
            A.connect(Type.RIGHT, root, Type.RIGHT, 8);
            A.connect(Type.BOTTOM, root, Type.BOTTOM, 8);
            A.VerticalBiasPercent = 0.2f;
            A.HorizontalBiasPercent = 0.2f;
            A.HorizontalDimensionBehaviour = DimensionBehaviour.MATCH_CONSTRAINT;
            B.connect(Type.TOP, A, Type.BOTTOM);

            Metrics metrics = new Metrics();
            root.fillMetrics(metrics);
            root.OptimizationLevel = Optimizer.OPTIMIZATION_STANDARD;
            root.layout();

            Console.WriteLine("1) A: " + A);
            Assert.AreEqual(A.Left, 8);
            Assert.AreEqual(A.Top, 163);
            Assert.AreEqual(A.Right, 592);
            Assert.AreEqual(A.Bottom, 173);

            A.Visibility = ConstraintWidget.GONE;
            root.layout();

            Console.WriteLine("2) A: " + A);
            Assert.AreEqual(A.Left, 120);
            Assert.AreEqual(A.Top, 160);
            Assert.AreEqual(A.Right, 120);
            Assert.AreEqual(A.Bottom, 160);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void test3EltsChain()
        [Test]
        public virtual void test3EltsChain()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            C.DebugName = "C";
            root.add(A);
            root.add(B);
            root.add(C);

            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            C.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 40);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 30);

            Metrics metrics = new Metrics();
            root.fillMetrics(metrics);
            root.OptimizationLevel = Optimizer.OPTIMIZATION_STANDARD;
            //        root.setOptimizationLevel(Optimizer.OPTIMIZATION_NONE);
            A.HorizontalChainStyle = ConstraintWidget.CHAIN_SPREAD_INSIDE;
            root.layout();
            Console.WriteLine("1) root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Console.WriteLine(metrics);
            Assert.AreEqual(A.Left, 40);
            Assert.AreEqual(B.Left, 255);
            Assert.AreEqual(C.Left, 470);

            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            C.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            root.layout();
            Console.WriteLine("2) root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Console.WriteLine(metrics);
            Assert.AreEqual(A.Left, 40);
            Assert.AreEqual(B.Left, 217, 1);
            Assert.AreEqual(C.Left, 393);
            Assert.AreEqual(A.Width, 177, 1);
            Assert.AreEqual(B.Width, 176, 1);
            Assert.AreEqual(C.Width, 177, 1);

            A.HorizontalChainStyle = ConstraintWidget.CHAIN_SPREAD;
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT, 7);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT, 3);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT, 7);
            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT, 3);

            root.layout();
            Console.WriteLine("3) root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Console.WriteLine(metrics);

            Assert.AreEqual(A.Left, 40);
            Assert.AreEqual(B.Left, 220);
            Assert.AreEqual(C.Left, 400, 1);
            Assert.AreEqual(A.Width, 170, 1);
            Assert.AreEqual(B.Width, 170, 1);
            Assert.AreEqual(C.Width, 170, 1);

            A.HorizontalChainStyle = ConstraintWidget.CHAIN_SPREAD_INSIDE;

            A.Visibility = ConstraintWidget.GONE;
            root.layout();
            Console.WriteLine("4) root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Console.WriteLine(metrics);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(B.Left, 3);
            Assert.AreEqual(C.Left, 292, 1);
            Assert.AreEqual(A.Width, 0);
            Assert.AreEqual(B.Width, 279, 1);
            Assert.AreEqual(C.Width, 278, 1);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testBasicChain()
        [Test]
        public virtual void testBasicChain()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            root.add(A);
            root.add(B);

            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);

            Metrics metrics = new Metrics();
            root.fillMetrics(metrics);
            root.OptimizationLevel = Optimizer.OPTIMIZATION_STANDARD;
            root.layout();
            Console.WriteLine("1) root: " + root + " A: " + A + " B: " + B);
            Console.WriteLine(metrics);
            Assert.AreEqual(A.Left, 133);
            Assert.AreEqual(B.Left, 367, 1);

            ConstraintWidget C = new ConstraintWidget(100, 20);
            C.DebugName = "C";
            root.add(C);
            C.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT);
            root.layout();
            Console.WriteLine("2) root: " + root + " A: " + A + " B: " + B);
            Console.WriteLine(metrics);
            Assert.AreEqual(A.Left, 133);
            Assert.AreEqual(B.Left, 367, 1);
            Assert.AreEqual(C.Left, B.Right);

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 40);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT, 100);
            A.HorizontalChainStyle = ConstraintWidget.CHAIN_PACKED;

            root.layout();
            Console.WriteLine("3) root: " + root + " A: " + A + " B: " + B);
            Console.WriteLine(metrics);
            Assert.AreEqual(A.Left, 170);
            Assert.AreEqual(B.Left, 370);

            A.HorizontalBiasPercent = 0;
            root.layout();
            Console.WriteLine("4) root: " + root + " A: " + A + " B: " + B);
            Console.WriteLine(metrics);
            Assert.AreEqual(A.Left, 40);
            Assert.AreEqual(B.Left, 240);

            A.HorizontalBiasPercent = 0.5f;
            A.Visibility = ConstraintWidget.GONE;
            //        root.setOptimizationLevel(Optimizer.OPTIMIZATION_NONE);
            root.layout();
            Console.WriteLine("5) root: " + root + " A: " + A + " B: " + B);
            Console.WriteLine(metrics);
            Assert.AreEqual(A.Left, 250);
            Assert.AreEqual(B.Left, 250);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testBasicChain2()
        [Test]
        public virtual void testBasicChain2()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            root.add(A);
            root.add(B);

            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);

            ConstraintWidget C = new ConstraintWidget(100, 20);
            C.DebugName = "C";
            root.add(C);
            C.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT);

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 40);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT, 100);
            A.HorizontalChainStyle = ConstraintWidget.CHAIN_PACKED;

            A.HorizontalBiasPercent = 0.5f;
            A.Visibility = ConstraintWidget.GONE;
            //        root.setOptimizationLevel(Optimizer.OPTIMIZATION_NONE);
            root.layout();
            Console.WriteLine("5) root: " + root + " A: " + A + " B: " + B);
            Assert.AreEqual(A.Left, 250);
            Assert.AreEqual(B.Left, 250);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testBasicRatio()
        [Test]
        public virtual void testBasicRatio()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            root.add(A);
            root.add(B);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.BOTTOM, A, ConstraintAnchor.Type.BOTTOM);
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.setDimensionRatio("1:1");
            Metrics metrics = new Metrics();
            root.fillMetrics(metrics);
            root.OptimizationLevel = Optimizer.OPTIMIZATION_STANDARD;
            root.layout();
            Console.WriteLine("1) root: " + root + " A: " + A + " B: " + B);
            Console.WriteLine(metrics);
            Assert.AreEqual(A.Height, A.Width);
            Assert.AreEqual(B.Top, (A.Height - B.Height) / 2);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testBasicBaseline()
        [Test]
        public virtual void testBasicBaseline()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            //A.BaselineDistance = 8;
            //B.BaselineDistance = 8;
           
            root.add(A);
            root.add(B);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            A.BaselineDistance = 1;
            B.BaselineDistance = 1;
            B.connect(ConstraintAnchor.Type.BASELINE, A, ConstraintAnchor.Type.BASELINE);
            A.BaselineDistance = 8;
            B.BaselineDistance = 7;
            Metrics metrics = new Metrics();
            root.fillMetrics(metrics);
            root.OptimizationLevel = Optimizer.OPTIMIZATION_STANDARD;
            root.layout();
            Console.WriteLine("1) root: " + root + " A: " + A + " B: " + B);
            Console.WriteLine(metrics);
            Assert.AreEqual(A.Top, 290);
            Assert.AreEqual(B.Top, A.Top+1);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testBasicMatchConstraints()
        [Test]
        public virtual void testBasicMatchConstraints()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            root.add(A);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            Metrics metrics = new Metrics();
            root.fillMetrics(metrics);
            root.OptimizationLevel = Optimizer.OPTIMIZATION_STANDARD;
            root.layout();
            Console.WriteLine("1) root: " + root + " A: " + A);
            Console.WriteLine(metrics);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(A.Top, 0);
            Assert.AreEqual(A.Right, root.Width);
            Assert.AreEqual(A.Bottom, root.Height);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 10);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, 20);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 30);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 40);
            root.layout();
            Console.WriteLine("2) root: " + root + " A: " + A);
            Console.WriteLine(metrics);
            Assert.AreEqual(A.Left, 30);
            Assert.AreEqual(A.Top, 10);
            Assert.AreEqual(A.Right, root.Width - 40);
            Assert.AreEqual(A.Bottom, root.Height - 20);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testBasicCenteringPositioning()
        [Test]
        public virtual void testBasicCenteringPositioning()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            root.add(A);
            long time = DateTimeHelperClass.nanoTime();
            Metrics metrics = new Metrics();
            root.fillMetrics(metrics);
            root.OptimizationLevel = Optimizer.OPTIMIZATION_STANDARD;
            root.layout();
            time = DateTimeHelperClass.nanoTime() - time;
            Console.WriteLine("A) execution time: " + time);
            Console.WriteLine("1) root: " + root + " A: " + A);
            Console.WriteLine(metrics);
            Assert.AreEqual(A.Left, (root.Width - A.Width) / 2);
            Assert.AreEqual(A.Top, (root.Height - A.Height) / 2);
            A.HorizontalBiasPercent = 0.3f;
            A.VerticalBiasPercent = 0.3f;
            root.layout();
            Console.WriteLine("2) root: " + root + " A: " + A);
            Console.WriteLine(metrics);
            Assert.AreEqual(A.Left, (int)((root.Width - A.Width) * 0.3f));
            Assert.AreEqual(A.Top, (int)((root.Height - A.Height) * 0.3f));
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 10);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 30);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 50);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, 20);
            root.layout();
            Console.WriteLine("3) root: " + root + " A: " + A);
            Console.WriteLine(metrics);
            Assert.AreEqual(A.Left, (int)((root.Width - A.Width - 40) * 0.3f) + 10);
            Assert.AreEqual(A.Top, (int)((root.Height - A.Height - 70) * 0.3f) + 50);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testBasicVerticalPositioning()
        [Test]
        public virtual void testBasicVerticalPositioning()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            int margin = 13;
            int marginR = 27;

            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 31);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 27);
            B.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 27);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM, 104);
            root.add(A);
            root.add(B);
            root.OptimizationLevel = Optimizer.OPTIMIZATION_STANDARD;
            long time = DateTimeHelperClass.nanoTime();
            //        root.layout();
            //        time = System.nanoTime() - time;
            //        System.out.println("A) execution time: " + time);
            //        System.out.println("a - root: " + root + " A: " + A + " B: " + B);
            //
            //        Assert.AreEqual(A.getLeft(), 27);
            //        Assert.AreEqual(A.getTop(), 31);
            //        Assert.AreEqual(B.getLeft(), 27);
            //        Assert.AreEqual(B.getTop(), 155);

            A.Visibility = ConstraintWidget.GONE;
            Metrics metrics = new Metrics();
            root.fillMetrics(metrics);
            root.layout();
            Console.WriteLine("b - root: " + root + " A: " + A + " B: " + B);
            Console.WriteLine(metrics);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(A.Top, 0);
            Assert.AreEqual(B.Left, 27);
            Assert.AreEqual(B.Top, 104);
            // root: id: root (0, 0) - (600 x 600) wrap: (0 x 0) A: id: A (27, 31) - (100 x 20) wrap: (0 x 0) B: id: B (27, 155) - (100 x 20) wrap: (0 x 0)

        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testBasicVerticalGuidelinePositioning()
        [Test]
        public virtual void testBasicVerticalGuidelinePositioning()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            Guideline guidelineA = new Guideline();
            guidelineA.Orientation = Guideline.HORIZONTAL;
            guidelineA.GuideEnd = 67;
            root.DebugName = "root";
            A.DebugName = "A";
            guidelineA.DebugName = "guideline";
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 31);
            A.connect(ConstraintAnchor.Type.BOTTOM, guidelineA, ConstraintAnchor.Type.TOP, 12);
            root.add(A);
            root.add(guidelineA);
            root.OptimizationLevel = Optimizer.OPTIMIZATION_STANDARD;
            long time = DateTimeHelperClass.nanoTime();
            root.layout();
            time = DateTimeHelperClass.nanoTime() - time;
            Console.WriteLine("A) execution time: " + time);
            Console.WriteLine("root: " + root + " A: " + A + " guide: " + guidelineA);
            Assert.AreEqual(A.Top, 266);
            Assert.AreEqual(guidelineA.Top, 533);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testSimpleCenterPositioning()
        [Test]
        public virtual void testSimpleCenterPositioning()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            int margin = 13;
            int marginR = 27;
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, margin);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, -margin);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, margin);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, -marginR);
            root.add(A);
            root.OptimizationLevel = Optimizer.OPTIMIZATION_STANDARD;
            long time = DateTimeHelperClass.nanoTime();
            root.layout();
            time = DateTimeHelperClass.nanoTime() - time;
            Console.WriteLine("A) execution time: " + time);
            Console.WriteLine("root: " + root + " A: " + A);
            Assert.AreEqual(A.Left, 270, 1);
            Assert.AreEqual(A.Top, 303, 1);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testSimpleGuideline()
        [Test]
        public virtual void testSimpleGuideline()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            Guideline guidelineA = new Guideline();
            ConstraintWidget A = new ConstraintWidget(100, 20);
            guidelineA.Orientation = Guideline.VERTICAL;
            guidelineA.GuideBegin = 100;
            root.DebugName = "root";
            A.DebugName = "A";
            guidelineA.DebugName = "guidelineA";
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 32);
            A.connect(ConstraintAnchor.Type.LEFT, guidelineA, ConstraintAnchor.Type.LEFT, 2);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 7);
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            root.add(guidelineA);
            root.add(A);
            root.OptimizationLevel = Optimizer.OPTIMIZATION_STANDARD;
            Metrics metrics = new Metrics();
            root.fillMetrics(metrics);
            long time = DateTimeHelperClass.nanoTime();
            root.layout();
            Assert.AreEqual(A.Left, 102);
            Assert.AreEqual(A.Top, 32);
            Assert.AreEqual(A.Width, 491);
            Assert.AreEqual(A.Height, 20);
            Assert.AreEqual(guidelineA.Left, 100);
            time = DateTimeHelperClass.nanoTime() - time;
            Console.WriteLine("A) execution time: " + time);
            Console.WriteLine("root: " + root + " A: " + A + " guideline: " + guidelineA);
            Console.WriteLine(metrics);
            root.Width = 700;
            time = DateTimeHelperClass.nanoTime();
            root.layout();
            time = DateTimeHelperClass.nanoTime() - time;
            Console.WriteLine("B) execution time: " + time);
            Console.WriteLine("root: " + root + " A: " + A + " guideline: " + guidelineA);
            Console.WriteLine(metrics);
            Assert.AreEqual(A.Left, 102);
            Assert.AreEqual(A.Top, 32);
            Assert.AreEqual(A.Width, 591);
            Assert.AreEqual(A.Height, 20);
            Assert.AreEqual(guidelineA.Left, 100);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testSimple()
        [Test]
        public virtual void testSimple()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            C.DebugName = "C";

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 10);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 20);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT, 10);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM, 20);
            C.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT, 30);
            C.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.BOTTOM, 20);
            root.add(A);
            root.add(B);
            root.add(C);
            root.OptimizationLevel = Optimizer.OPTIMIZATION_STANDARD;

            long time = DateTimeHelperClass.nanoTime();
            root.layout();
            time = DateTimeHelperClass.nanoTime() - time;
            Console.WriteLine("execution time: " + time);
            Console.WriteLine("root: " + root + " A: " + A + " B: " + B + " C: " + C);

            Assert.AreEqual(A.Left, 10);
            Assert.AreEqual(A.Top, 20);
            Assert.AreEqual(B.Left, 120);
            Assert.AreEqual(B.Top, 60);
            Assert.AreEqual(C.Left, 140);
            Assert.AreEqual(C.Top, 100);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testGuideline()
        [Test]
        public virtual void testGuideline()
        {
            testVerticalGuideline(Optimizer.OPTIMIZATION_NONE);
            testVerticalGuideline(Optimizer.OPTIMIZATION_STANDARD);
            testHorizontalGuideline(Optimizer.OPTIMIZATION_NONE);
            testHorizontalGuideline(Optimizer.OPTIMIZATION_STANDARD);
        }

        public virtual void testVerticalGuideline(int directResolution)
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            root.OptimizationLevel = directResolution;
            ConstraintWidget A = new ConstraintWidget(100, 20);
            Guideline guideline = new Guideline();
            guideline.Orientation = Guideline.VERTICAL;
            root.DebugName = "root";
            A.DebugName = "A";
            guideline.DebugName = "guideline";
            root.add(A);
            root.add(guideline);
            A.connect(ConstraintAnchor.Type.LEFT, guideline, ConstraintAnchor.Type.LEFT, 16);
            guideline.GuideBegin = 100;
            root.layout();
            Console.WriteLine("res: " + directResolution + " root: " + root + " A: " + A + " guideline: " + guideline);
            Assert.AreEqual(guideline.Left, 100);
            Assert.AreEqual(A.Left, 116);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(A.Height, 20);
            Assert.AreEqual(A.Top, 0);
            guideline.setGuidePercent(0.5f);
            root.layout();
            Console.WriteLine("res: " + directResolution + " root: " + root + " A: " + A + " guideline: " + guideline);
            Assert.AreEqual(guideline.Left, root.Width / 2);
            Assert.AreEqual(A.Left, 316);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(A.Height, 20);
            Assert.AreEqual(A.Top, 0);
            guideline.GuideEnd = 100;
            root.layout();
            Console.WriteLine("res: " + directResolution + " root: " + root + " A: " + A + " guideline: " + guideline);
            Assert.AreEqual(guideline.Left, 500);
            Assert.AreEqual(A.Left, 516);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(A.Height, 20);
            Assert.AreEqual(A.Top, 0);
        }

        public virtual void testHorizontalGuideline(int directResolution)
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            root.OptimizationLevel = directResolution;
            ConstraintWidget A = new ConstraintWidget(100, 20);
            Guideline guideline = new Guideline();
            guideline.Orientation = Guideline.HORIZONTAL;
            root.DebugName = "root";
            A.DebugName = "A";
            guideline.DebugName = "guideline";
            root.add(A);
            root.add(guideline);
            A.connect(ConstraintAnchor.Type.TOP, guideline, ConstraintAnchor.Type.TOP, 16);
            guideline.GuideBegin = 100;
            root.layout();
            Console.WriteLine("res: " + directResolution + " root: " + root + " A: " + A + " guideline: " + guideline);
            Assert.AreEqual(guideline.Top, 100);
            Assert.AreEqual(A.Top, 116);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(A.Height, 20);
            Assert.AreEqual(A.Left, 0);
            guideline.setGuidePercent(0.5f);
            root.layout();
            Console.WriteLine("res: " + directResolution + " root: " + root + " A: " + A + " guideline: " + guideline);
            Assert.AreEqual(guideline.Top, root.Height / 2);
            Assert.AreEqual(A.Top, 316);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(A.Height, 20);
            Assert.AreEqual(A.Left, 0);
            guideline.GuideEnd = 100;
            root.layout();
            Console.WriteLine("res: " + directResolution + " root: " + root + " A: " + A + " guideline: " + guideline);
            Assert.AreEqual(guideline.Top, 500);
            Assert.AreEqual(A.Top, 516);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(A.Height, 20);
            Assert.AreEqual(A.Left, 0);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testBasicCentering()
        [Test]
        public virtual void testBasicCentering()
        {
            testBasicCentering(Optimizer.OPTIMIZATION_NONE);
            testBasicCentering(Optimizer.OPTIMIZATION_STANDARD);
        }

        public virtual void testBasicCentering(int directResolution)
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            root.OptimizationLevel = directResolution;
            ConstraintWidget A = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            root.add(A);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 10);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 10);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 10);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, 10);
            root.layout();
            Console.WriteLine("res: " + directResolution + " root: " + root + " A: " + A);
            Assert.AreEqual(A.Left, 250);
            Assert.AreEqual(A.Top, 290);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testPercent()
        [Test]
        public virtual void testPercent()
        {
            testPercent(Optimizer.OPTIMIZATION_NONE);
            testPercent(Optimizer.OPTIMIZATION_STANDARD);
        }

        public virtual void testPercent(int directResolution)
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            root.OptimizationLevel = directResolution;
            ConstraintWidget A = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            root.add(A);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 10);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 10);
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_PERCENT, 0, 0, 0.5f);
            A.setVerticalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_PERCENT, 0, 0, 0.5f);
            root.layout();
            Console.WriteLine("res: " + directResolution + " root: " + root + " A: " + A);
            Assert.AreEqual(A.Left, 10);
            Assert.AreEqual(A.Top, 10);
            Assert.AreEqual(A.Width, 300);
            Assert.AreEqual(A.Height, 300);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testDependency()
        [Test]
        public virtual void testDependency()
        {
            testDependency(Optimizer.OPTIMIZATION_NONE);
            testDependency(Optimizer.OPTIMIZATION_STANDARD);
        }

        public virtual void testDependency(int directResolution)
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            root.OptimizationLevel = directResolution;
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            C.DebugName = "C";
            root.add(A);
            root.add(B);
            root.add(C);
            A.BaselineDistance = 8;
            B.BaselineDistance = 8;
            C.BaselineDistance = 8;
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 10);
            A.connect(ConstraintAnchor.Type.BASELINE, B, ConstraintAnchor.Type.BASELINE);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT, 16);
            B.connect(ConstraintAnchor.Type.BASELINE, C, ConstraintAnchor.Type.BASELINE);
            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT, 48);
            C.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 32);
            root.layout();
            Console.WriteLine("res: " + directResolution + " root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Left, 10);
            Assert.AreEqual(A.Top, 32);
            Assert.AreEqual(B.Left, 126);
            Assert.AreEqual(B.Top, 32);
            Assert.AreEqual(C.Left, 274);
            Assert.AreEqual(C.Top, 32);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testDependency2()
        [Test]
        public virtual void testDependency2()
        {
            testDependency2(Optimizer.OPTIMIZATION_NONE);
            testDependency2(Optimizer.OPTIMIZATION_STANDARD);
        }

        public virtual void testDependency2(int directResolution)
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            root.OptimizationLevel = directResolution;
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            A.DebugName = "A";
            B.DebugName = "B";
            C.DebugName = "C";
            root.add(A);
            root.add(B);
            root.add(C);
            A.BaselineDistance = 8;
            B.BaselineDistance = 8;
            C.BaselineDistance = 8;
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            A.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.BOTTOM, A, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.LEFT, C, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP);
            C.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 12);
            root.layout();
            Console.WriteLine("res: " + directResolution + " root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Left, 12);
            Assert.AreEqual(A.Top, 580);
            Assert.AreEqual(B.Left, 12);
            Assert.AreEqual(B.Top, 560);
            Assert.AreEqual(C.Left, 12);
            Assert.AreEqual(C.Top, 540);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testDependency3()
        [Test]
        public virtual void testDependency3()
        {
            testDependency3(Optimizer.OPTIMIZATION_NONE);
            testDependency3(Optimizer.OPTIMIZATION_STANDARD);
        }

        public virtual void testDependency3(int directResolution)
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            root.OptimizationLevel = directResolution;
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            C.DebugName = "C";
            root.add(A);
            root.add(B);
            root.add(C);
            A.BaselineDistance = 8;
            B.BaselineDistance = 8;
            C.BaselineDistance = 8;
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 10);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 20);
            B.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 30);
            B.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, 60);
            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 10);
            C.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP);
            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT, 20);
            root.layout();
            Console.WriteLine("res: " + directResolution + " root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Left, 10);
            Assert.AreEqual(A.Top, 20);
            Assert.AreEqual(B.Left, 260);
            Assert.AreEqual(B.Top, 520);
            Assert.AreEqual(C.Left, 380);
            Assert.AreEqual(C.Top, 500);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testDependency4()
        [Test]
        public virtual void testDependency4()
        {
            testDependency4(Optimizer.OPTIMIZATION_NONE);
            testDependency4(Optimizer.OPTIMIZATION_STANDARD);
        }

        public virtual void testDependency4(int directResolution)
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            root.OptimizationLevel = directResolution;
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            root.add(A);
            root.add(B);
            A.BaselineDistance = 8;
            B.BaselineDistance = 8;
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 10);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 20);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 10);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, 20);
            B.connect(ConstraintAnchor.Type.RIGHT, A, ConstraintAnchor.Type.RIGHT, 30);
            B.connect(ConstraintAnchor.Type.BOTTOM, A, ConstraintAnchor.Type.BOTTOM, 60);
            root.layout();
            Console.WriteLine("res: " + directResolution + " root: " + root + " A: " + A + " B: " + B);
            Assert.AreEqual(A.Left, 250);
            Assert.AreEqual(A.Top, 290);
            Assert.AreEqual(B.Left, 220);
            Assert.AreEqual(B.Top, 230);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testDependency5()
        [Test]
        public virtual void testDependency5()
        {
            testDependency5(Optimizer.OPTIMIZATION_NONE);
            testDependency5(Optimizer.OPTIMIZATION_STANDARD);
        }

        public virtual void testDependency5(int directResolution)
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            root.OptimizationLevel = directResolution;
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            ConstraintWidget D = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            C.DebugName = "C";
            D.DebugName = "D";
            root.add(A);
            root.add(B);
            root.add(C);
            root.add(D);
            A.BaselineDistance = 8;
            B.BaselineDistance = 8;
            C.BaselineDistance = 8;
            D.BaselineDistance = 8;
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 10);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 10);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 20);
            A.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, 10);
            C.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.BOTTOM);
            C.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.RIGHT, 20);
            D.connect(ConstraintAnchor.Type.TOP, C, ConstraintAnchor.Type.BOTTOM);
            D.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.RIGHT, 20);
            root.layout();
            Console.WriteLine("res: " + directResolution + " root: " + root + " A: " + A + " B: " + B + " C: " + C + " D: " + D);
            Assert.AreEqual(A.Left, 250);
            Assert.AreEqual(A.Top, 197);
            Assert.AreEqual(B.Left, 250);
            Assert.AreEqual(B.Top, 393);
            Assert.AreEqual(C.Left, 230);
            Assert.AreEqual(C.Top, 413);
            Assert.AreEqual(D.Left, 210);
            Assert.AreEqual(D.Top, 433);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testUnconstrainedDependency()
        [Test]
        public virtual void testUnconstrainedDependency()
        {
            testUnconstrainedDependency(Optimizer.OPTIMIZATION_NONE);
            testUnconstrainedDependency(Optimizer.OPTIMIZATION_STANDARD);
        }

        public virtual void testUnconstrainedDependency(int directResolution)
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            root.OptimizationLevel = directResolution;
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            C.DebugName = "C";
            root.add(A);
            root.add(B);
            root.add(C);
            A.BaselineDistance = 8;
            B.BaselineDistance = 8;
            C.BaselineDistance = 8;
            A.setFrame(142, 96, 242, 130);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT, 10);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.TOP, 100);
            C.connect(ConstraintAnchor.Type.RIGHT, A, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.BASELINE, A, ConstraintAnchor.Type.BASELINE);
            root.layout();
            Console.WriteLine("res: " + directResolution + " root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Left, 142);
            Assert.AreEqual(A.Top, 96);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(A.Height, 34);
            Assert.AreEqual(B.Left, 252);
            Assert.AreEqual(B.Top, 196);
            Assert.AreEqual(C.Left, 42);
            Assert.AreEqual(C.Top, 96);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testFullLayout()
        [Test]
        public virtual void testFullLayout()
        {
            testFullLayout(Optimizer.OPTIMIZATION_NONE);
            testFullLayout(Optimizer.OPTIMIZATION_STANDARD);
        }

        public virtual void testFullLayout(int directResolution)
        {
            // Horizontal :
            // r <- A
            // r <- B <- C <- D
            //      B <- E
            // r <- F
            // r <- G
            // Vertical:
            // r <- A <- B <- C <- D <- E
            // r <- F <- G
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            root.OptimizationLevel = directResolution;
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            ConstraintWidget D = new ConstraintWidget(100, 20);
            ConstraintWidget E = new ConstraintWidget(100, 20);
            ConstraintWidget F = new ConstraintWidget(100, 20);
            ConstraintWidget G = new ConstraintWidget(100, 20);
            A.DebugName = "A";
            B.DebugName = "B";
            C.DebugName = "C";
            D.DebugName = "D";
            E.DebugName = "E";
            F.DebugName = "F";
            G.DebugName = "G";
            root.add(G);
            root.add(A);
            root.add(B);
            root.add(E);
            root.add(C);
            root.add(D);
            root.add(F);
            A.BaselineDistance = 8;
            B.BaselineDistance = 8;
            C.BaselineDistance = 8;
            D.BaselineDistance = 8;
            E.BaselineDistance = 8;
            F.BaselineDistance = 8;
            G.BaselineDistance = 8;
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 20);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM, 40);
            B.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 16);
            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT, 16);
            C.connect(ConstraintAnchor.Type.BASELINE, B, ConstraintAnchor.Type.BASELINE);
            D.connect(ConstraintAnchor.Type.TOP, C, ConstraintAnchor.Type.BOTTOM);
            D.connect(ConstraintAnchor.Type.LEFT, C, ConstraintAnchor.Type.LEFT);
            E.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.RIGHT);
            E.connect(ConstraintAnchor.Type.BASELINE, D, ConstraintAnchor.Type.BASELINE);
            F.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            F.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            G.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 16);
            G.connect(ConstraintAnchor.Type.BASELINE, F, ConstraintAnchor.Type.BASELINE);
            root.layout();

            Console.WriteLine(" direct: " + directResolution + " -> A: " + A + " B: " + B + " C: " + C + " D: " + D + " E: " + E + " F: " + F + " G: " + G);
            Assert.AreEqual(A.Left, 250);
            Assert.AreEqual(A.Top, 20);
            Assert.AreEqual(B.Left, 16);
            Assert.AreEqual(B.Top, 80);
            Assert.AreEqual(C.Left, 132);
            Assert.AreEqual(C.Top, 80);
            Assert.AreEqual(D.Left, 132);
            Assert.AreEqual(D.Top, 100);
            Assert.AreEqual(E.Left, 16);
            Assert.AreEqual(E.Top, 100);
            Assert.AreEqual(F.Left, 500);
            Assert.AreEqual(F.Top, 580);
            Assert.AreEqual(G.Left, 16);
            Assert.AreEqual(G.Top, 580);
        }

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
                Console.WriteLine("*** MEASURE " + widget + " ***");

                if (horizontalBehavior == ConstraintWidget.DimensionBehaviour.FIXED)
                {
                    measure.measuredWidth = horizontalDimension;
                }
                else if (horizontalBehavior == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT)
                {
                    measure.measuredWidth = horizontalDimension;
                }
                if (verticalBehavior == ConstraintWidget.DimensionBehaviour.FIXED)
                {
                    measure.measuredHeight = verticalDimension;
                    measure.measuredBaseline = 8;
                }
                else
                {
                    measure.measuredHeight = verticalDimension;
                    measure.measuredBaseline = 8;
                }
            }

            public virtual void didMeasures()
            {

            }
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testComplexLayout()
        [Test]
        public virtual void testComplexLayout()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            root.OptimizationLevel = Optimizer.OPTIMIZATION_GROUPING;
            ConstraintWidget A = new ConstraintWidget(100, 100);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            ConstraintWidget D = new ConstraintWidget(30, 30);
            ConstraintWidget E = new ConstraintWidget(30, 30);
            ConstraintWidget F = new ConstraintWidget(30, 30);
            ConstraintWidget G = new ConstraintWidget(100, 20);
            ConstraintWidget H = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            C.DebugName = "C";
            D.DebugName = "D";
            E.DebugName = "E";
            F.DebugName = "F";
            G.DebugName = "G";
            H.DebugName = "H";
            root.add(G);
            root.add(A);
            root.add(B);
            root.add(E);
            root.add(C);
            root.add(D);
            root.add(F);
            root.add(H);
            B.BaselineDistance = 8;
            C.BaselineDistance = 8;
            D.BaselineDistance = 8;
            E.BaselineDistance = 8;
            F.BaselineDistance = 8;
            G.BaselineDistance = 8;
            H.BaselineDistance = 8;

            A.connect(Type.TOP, root, Type.TOP, 16);
            A.connect(Type.LEFT, root, Type.LEFT, 16);
            A.connect(Type.BOTTOM, root, Type.BOTTOM, 16);

            B.connect(Type.TOP, A, Type.TOP);
            B.connect(Type.LEFT, A, Type.RIGHT, 16);

            C.connect(Type.TOP, root, Type.TOP);
            C.connect(Type.LEFT, A, Type.RIGHT, 16);
            C.connect(Type.BOTTOM, root, Type.BOTTOM);

            D.connect(Type.BOTTOM, A, Type.BOTTOM);
            D.connect(Type.LEFT, A, Type.RIGHT, 16);

            E.connect(Type.BOTTOM, D, Type.BOTTOM);
            E.connect(Type.LEFT, D, Type.RIGHT, 16);

            F.connect(Type.BOTTOM, E, Type.BOTTOM);
            F.connect(Type.LEFT, E, Type.RIGHT, 16);

            G.connect(Type.TOP, root, Type.TOP);
            G.connect(Type.RIGHT, root, Type.RIGHT, 16);
            G.connect(Type.BOTTOM, root, Type.BOTTOM);

            H.connect(Type.BOTTOM, root, Type.BOTTOM, 16);
            H.connect(Type.RIGHT, root, Type.RIGHT, 16);

            root.Measurer = sMeasurer;
            root.layout();
            Console.WriteLine(" direct: -> A: " + A + " B: " + B + " C: " + C + " D: " + D + " E: " + E + " F: " + F + " G: " + G + " H: " + H);

            Assert.AreEqual(A.Left, 16);
            Assert.AreEqual(A.Top, 250);

            Assert.AreEqual(B.Left, 132);
            Assert.AreEqual(B.Top, 250);

            Assert.AreEqual(C.Left, 132);
            Assert.AreEqual(C.Top, 290);

            Assert.AreEqual(D.Left, 132);
            Assert.AreEqual(D.Top, 320);

            Assert.AreEqual(E.Left, 178);
            Assert.AreEqual(E.Top, 320);

            Assert.AreEqual(F.Left, 224);
            Assert.AreEqual(F.Top, 320);

            Assert.AreEqual(G.Left, 484);
            Assert.AreEqual(G.Top, 290);

            Assert.AreEqual(H.Left, 484);
            Assert.AreEqual(H.Top, 564);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testComplexLayoutWrap()
        [Test]
        public virtual void testComplexLayoutWrap()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            root.OptimizationLevel = Optimizer.OPTIMIZATION_DIRECT;
            ConstraintWidget A = new ConstraintWidget(100, 100);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            ConstraintWidget D = new ConstraintWidget(30, 30);
            ConstraintWidget E = new ConstraintWidget(30, 30);
            ConstraintWidget F = new ConstraintWidget(30, 30);
            ConstraintWidget G = new ConstraintWidget(100, 20);
            ConstraintWidget H = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            C.DebugName = "C";
            D.DebugName = "D";
            E.DebugName = "E";
            F.DebugName = "F";
            G.DebugName = "G";
            H.DebugName = "H";
            root.add(G);
            root.add(A);
            root.add(B);
            root.add(E);
            root.add(C);
            root.add(D);
            root.add(F);
            root.add(H);
            B.BaselineDistance = 8;
            C.BaselineDistance = 8;
            D.BaselineDistance = 8;
            E.BaselineDistance = 8;
            F.BaselineDistance = 8;
            G.BaselineDistance = 8;
            H.BaselineDistance = 8;

            A.connect(Type.TOP, root, Type.TOP, 16);
            A.connect(Type.LEFT, root, Type.LEFT, 16);
            A.connect(Type.BOTTOM, root, Type.BOTTOM, 16);

            B.connect(Type.TOP, A, Type.TOP);
            B.connect(Type.LEFT, A, Type.RIGHT, 16);

            C.connect(Type.TOP, root, Type.TOP);
            C.connect(Type.LEFT, A, Type.RIGHT, 16);
            C.connect(Type.BOTTOM, root, Type.BOTTOM);

            D.connect(Type.BOTTOM, A, Type.BOTTOM);
            D.connect(Type.LEFT, A, Type.RIGHT, 16);

            E.connect(Type.BOTTOM, D, Type.BOTTOM);
            E.connect(Type.LEFT, D, Type.RIGHT, 16);

            F.connect(Type.BOTTOM, E, Type.BOTTOM);
            F.connect(Type.LEFT, E, Type.RIGHT, 16);

            G.connect(Type.TOP, root, Type.TOP);
            G.connect(Type.RIGHT, root, Type.RIGHT, 16);
            G.connect(Type.BOTTOM, root, Type.BOTTOM);

            H.connect(Type.BOTTOM, root, Type.BOTTOM, 16);
            H.connect(Type.RIGHT, root, Type.RIGHT, 16);

            root.Measurer = sMeasurer;
            root.VerticalDimensionBehaviour = DimensionBehaviour.WRAP_CONTENT;
            root.layout();

            Console.WriteLine(" direct: -> A: " + A + " B: " + B + " C: " + C + " D: " + D + " E: " + E + " F: " + F + " G: " + G + " H: " + H);

            Assert.AreEqual(A.Left, 16);
            Assert.AreEqual(A.Top, 16);

            Assert.AreEqual(B.Left, 132);
            Assert.AreEqual(B.Top, 16);

            Assert.AreEqual(C.Left, 132);
            Assert.AreEqual(C.Top, 56);

            Assert.AreEqual(D.Left, 132);
            Assert.AreEqual(D.Top, 86);

            Assert.AreEqual(E.Left, 178);
            Assert.AreEqual(E.Top, 86);

            Assert.AreEqual(F.Left, 224);
            Assert.AreEqual(F.Top, 86);

            Assert.AreEqual(G.Left, 484);
            Assert.AreEqual(G.Top, 56);

            Assert.AreEqual(H.Left, 484);
            Assert.AreEqual(H.Top, 96);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testChainLayoutWrap()
        [Test]
        public virtual void testChainLayoutWrap()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            root.OptimizationLevel = Optimizer.OPTIMIZATION_GROUPING;
            ConstraintWidget A = new ConstraintWidget(100, 100);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            C.DebugName = "C";
            root.add(A);
            root.add(B);
            root.add(C);
            A.BaselineDistance = 28;
            B.BaselineDistance = 8;
            C.BaselineDistance = 8;

            A.connect(Type.TOP, root, Type.TOP, 16);
            A.connect(Type.LEFT, root, Type.LEFT, 16);
            A.connect(Type.RIGHT, B, Type.LEFT);
            A.connect(Type.BOTTOM, root, Type.BOTTOM, 16);

            B.connect(Type.BASELINE, A, Type.BASELINE);
            B.connect(Type.LEFT, A, Type.RIGHT);
            B.connect(Type.RIGHT, C, Type.LEFT);

            C.connect(Type.BASELINE, B, Type.BASELINE);
            C.connect(Type.LEFT, B, Type.RIGHT);
            C.connect(Type.RIGHT, root, Type.RIGHT, 16);

            root.Measurer = sMeasurer;
            //root.setWidth(332);
            root.HorizontalDimensionBehaviour = DimensionBehaviour.WRAP_CONTENT;
            //root.setVerticalDimensionBehaviour(DimensionBehaviour.WRAP_CONTENT);
            root.layout();

            Console.WriteLine(" direct: -> A: " + A + " B: " + B + " C: " + C);

            Assert.AreEqual(A.Left, 16);
            Assert.AreEqual(A.Top, 250);

            Assert.AreEqual(B.Left, 116);
            Assert.AreEqual(B.Top, 270);

            Assert.AreEqual(C.Left, 216);
            Assert.AreEqual(C.Top, 270);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testChainLayoutWrap2()
        [Test]
        public virtual void testChainLayoutWrap2()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            root.OptimizationLevel = Optimizer.OPTIMIZATION_GROUPING;
            ConstraintWidget A = new ConstraintWidget(100, 100);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            ConstraintWidget D = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            C.DebugName = "C";
            D.DebugName = "D";
            root.add(A);
            root.add(B);
            root.add(C);
            root.add(D);
            A.BaselineDistance = 28;
            B.BaselineDistance = 8;
            C.BaselineDistance = 8;
            D.BaselineDistance = 8;

            A.connect(Type.TOP, root, Type.TOP, 16);
            A.connect(Type.LEFT, root, Type.LEFT, 16);
            A.connect(Type.RIGHT, B, Type.LEFT);
            A.connect(Type.BOTTOM, root, Type.BOTTOM, 16);

            B.connect(Type.BASELINE, A, Type.BASELINE);
            B.connect(Type.LEFT, A, Type.RIGHT);
            B.connect(Type.RIGHT, C, Type.LEFT);

            C.connect(Type.BASELINE, B, Type.BASELINE);
            C.connect(Type.LEFT, B, Type.RIGHT);
            C.connect(Type.RIGHT, D, Type.LEFT, 16);

            D.connect(Type.RIGHT, root, Type.RIGHT);
            D.connect(Type.BOTTOM, root, Type.BOTTOM);

            root.Measurer = sMeasurer;
            //root.setWidth(332);
            root.HorizontalDimensionBehaviour = DimensionBehaviour.WRAP_CONTENT;
            //root.setVerticalDimensionBehaviour(DimensionBehaviour.WRAP_CONTENT);
            root.layout();

            Console.WriteLine(" direct: -> A: " + A + " B: " + B + " C: " + C + " D: " + D);

            Assert.AreEqual(A.Left, 16);
            Assert.AreEqual(A.Top, 250);

            Assert.AreEqual(B.Left, 116);
            Assert.AreEqual(B.Top, 270);

            Assert.AreEqual(C.Left, 216);
            Assert.AreEqual(C.Top, 270);

            Assert.AreEqual(D.Left, 332);
            Assert.AreEqual(D.Top, 580);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testChainLayoutWrapGuideline()
        [Test]
        public virtual void testChainLayoutWrapGuideline()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            root.OptimizationLevel = Optimizer.OPTIMIZATION_GROUPING;
            ConstraintWidget A = new ConstraintWidget(100, 20);
            Guideline guideline = new Guideline();
            guideline.Orientation = Guideline.VERTICAL;
            guideline.GuideEnd = 100;
            root.DebugName = "root";
            A.DebugName = "A";
            guideline.DebugName = "guideline";
            root.add(A);
            root.add(guideline);
            A.BaselineDistance = 28;

            A.connect(Type.LEFT, guideline, Type.LEFT, 16);
            A.connect(Type.BOTTOM, root, Type.BOTTOM, 16);


            root.Measurer = sMeasurer;
            //root.setHorizontalDimensionBehaviour(DimensionBehaviour.WRAP_CONTENT);
            root.VerticalDimensionBehaviour = DimensionBehaviour.WRAP_CONTENT;
            root.layout();

            Console.WriteLine(" direct: -> A: " + A + " guideline: " + guideline);

            Assert.AreEqual(A.Left, 516);
            Assert.AreEqual(A.Top, 0);

            Assert.AreEqual(guideline.Left, 500);
        }


        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testChainLayoutWrapGuidelineChain()
        [Test]
        public virtual void testChainLayoutWrapGuidelineChain()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            root.OptimizationLevel = Optimizer.OPTIMIZATION_GROUPING;
            ConstraintWidget A = new ConstraintWidget(20, 20);
            ConstraintWidget B = new ConstraintWidget(20, 20);
            ConstraintWidget C = new ConstraintWidget(20, 20);
            ConstraintWidget D = new ConstraintWidget(20, 20);
            ConstraintWidget A2 = new ConstraintWidget(20, 20);
            ConstraintWidget B2 = new ConstraintWidget(20, 20);
            ConstraintWidget C2 = new ConstraintWidget(20, 20);
            ConstraintWidget D2 = new ConstraintWidget(20, 20);
            Guideline guidelineStart = new Guideline();
            Guideline guidelineEnd = new Guideline();
            guidelineStart.Orientation = Guideline.VERTICAL;
            guidelineEnd.Orientation = Guideline.VERTICAL;
            guidelineStart.GuideBegin = 30;
            guidelineEnd.GuideEnd = 30;
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            C.DebugName = "C";
            D.DebugName = "D";
            A2.DebugName = "A2";
            B2.DebugName = "B2";
            C2.DebugName = "C2";
            D2.DebugName = "D2";
            guidelineStart.DebugName = "guidelineStart";
            guidelineEnd.DebugName = "guidelineEnd";
            root.add(A);
            root.add(B);
            root.add(C);
            root.add(D);
            root.add(A2);
            root.add(B2);
            root.add(C2);
            root.add(D2);
            root.add(guidelineStart);
            root.add(guidelineEnd);

            C.Visibility = ConstraintWidget.GONE;
            ChainConnect(Type.LEFT, guidelineStart, Type.RIGHT, guidelineEnd, A, B, C, D);
            ChainConnect(Type.LEFT, root, Type.RIGHT, root, A2, B2, C2, D2);


            root.Measurer = sMeasurer;
            root.HorizontalDimensionBehaviour = DimensionBehaviour.WRAP_CONTENT;
            //root.setVerticalDimensionBehaviour(DimensionBehaviour.WRAP_CONTENT);
            root.layout();

            Console.WriteLine(" direct: -> A: " + A + " guideline: " + guidelineStart + " ebnd " + guidelineEnd + " B: " + B + " C: " + C + " D: " + D);
            Console.WriteLine(" direct: -> A2: " + A2 + " B2: " + B2 + " C2: " + C2 + " D2: " + D2);

            Assert.AreEqual(A.Left, 30);
            Assert.AreEqual(B.Left, 50);
            Assert.AreEqual(C.Left, 70);
            Assert.AreEqual(D.Left, 70);
            Assert.AreEqual(guidelineStart.Left, 30);
            Assert.AreEqual(guidelineEnd.Left, 90);
            Assert.AreEqual(A2.Left, 8);
            Assert.AreEqual(B2.Left, 36);
            Assert.AreEqual(C2.Left, 64);
            Assert.AreEqual(D2.Left, 92);
        }

        private void ChainConnect(Type start, ConstraintWidget startTarget, Type end, ConstraintWidget endTarget, params ConstraintWidget[] widgets)
        {
            widgets[0].connect(start, startTarget, start);
            ConstraintWidget previousWidget = null;
            for (int i = 0; i < widgets.Length; i++)
            {
                if (previousWidget != null)
                {
                    widgets[i].connect(start, previousWidget, end);
                }
                if (i < widgets.Length - 1)
                {
                    widgets[i].connect(end, widgets[i + 1], start);
                }
                previousWidget = widgets[i];
            }
            if (previousWidget != null)
            {
                previousWidget.connect(end, endTarget, end);
            }
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testChainLayoutWrapGuidelineChainVertical()
        [Test]
        public virtual void testChainLayoutWrapGuidelineChainVertical()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            root.OptimizationLevel = Optimizer.OPTIMIZATION_GROUPING;
            ConstraintWidget A = new ConstraintWidget(20, 20);
            ConstraintWidget B = new ConstraintWidget(20, 20);
            ConstraintWidget C = new ConstraintWidget(20, 20);
            ConstraintWidget D = new ConstraintWidget(20, 20);
            ConstraintWidget A2 = new ConstraintWidget(20, 20);
            ConstraintWidget B2 = new ConstraintWidget(20, 20);
            ConstraintWidget C2 = new ConstraintWidget(20, 20);
            ConstraintWidget D2 = new ConstraintWidget(20, 20);
            Guideline guidelineStart = new Guideline();
            Guideline guidelineEnd = new Guideline();
            guidelineStart.Orientation = Guideline.HORIZONTAL;
            guidelineEnd.Orientation = Guideline.HORIZONTAL;
            guidelineStart.GuideBegin = 30;
            guidelineEnd.GuideEnd = 30;
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            C.DebugName = "C";
            D.DebugName = "D";
            A2.DebugName = "A2";
            B2.DebugName = "B2";
            C2.DebugName = "C2";
            D2.DebugName = "D2";
            guidelineStart.DebugName = "guidelineStart";
            guidelineEnd.DebugName = "guidelineEnd";
            root.add(A);
            root.add(B);
            root.add(C);
            root.add(D);
            root.add(A2);
            root.add(B2);
            root.add(C2);
            root.add(D2);
            root.add(guidelineStart);
            root.add(guidelineEnd);

            C.Visibility = ConstraintWidget.GONE;
            ChainConnect(Type.TOP, guidelineStart, Type.BOTTOM, guidelineEnd, A, B, C, D);
            ChainConnect(Type.TOP, root, Type.BOTTOM, root, A2, B2, C2, D2);


            root.Measurer = sMeasurer;
            //root.setHorizontalDimensionBehaviour(DimensionBehaviour.WRAP_CONTENT);
            root.VerticalDimensionBehaviour = DimensionBehaviour.WRAP_CONTENT;
            root.layout();

            Console.WriteLine(" direct: -> A: " + A + " guideline: " + guidelineStart + " ebnd " + guidelineEnd + " B: " + B + " C: " + C + " D: " + D);
            Console.WriteLine(" direct: -> A2: " + A2 + " B2: " + B2 + " C2: " + C2 + " D2: " + D2);

            Assert.AreEqual(A.Top, 30);
            Assert.AreEqual(B.Top, 50);
            Assert.AreEqual(C.Top, 70);
            Assert.AreEqual(D.Top, 70);
            Assert.AreEqual(guidelineStart.Top, 30);
            Assert.AreEqual(guidelineEnd.Top, 90);
            Assert.AreEqual(A2.Top, 8);
            Assert.AreEqual(B2.Top, 36);
            Assert.AreEqual(C2.Top, 64);
            Assert.AreEqual(D2.Top, 92);

            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(B.Left, 0);
            Assert.AreEqual(C.Left, 0);
            Assert.AreEqual(D.Left, 0);
            Assert.AreEqual(A2.Left, 0);
            Assert.AreEqual(B2.Left, 0);
            Assert.AreEqual(C2.Left, 0);
            Assert.AreEqual(D2.Left, 0);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testChainLayoutWrapRatioChain()
        [Test]
        public virtual void testChainLayoutWrapRatioChain()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            root.OptimizationLevel = Optimizer.OPTIMIZATION_GROUPING;
            ConstraintWidget A = new ConstraintWidget(20, 20);
            ConstraintWidget B = new ConstraintWidget(20, 20);
            ConstraintWidget C = new ConstraintWidget(20, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            C.DebugName = "C";
            root.add(A);
            root.add(B);
            root.add(C);

            ChainConnect(Type.TOP, root, Type.BOTTOM, root, A, B, C);
            A.connect(Type.LEFT, root, Type.LEFT);
            B.connect(Type.LEFT, root, Type.LEFT);
            C.connect(Type.LEFT, root, Type.LEFT);
            A.connect(Type.RIGHT, root, Type.RIGHT);
            B.connect(Type.RIGHT, root, Type.RIGHT);
            C.connect(Type.RIGHT, root, Type.RIGHT);
            A.VerticalChainStyle = ConstraintWidget.CHAIN_SPREAD_INSIDE;
            B.HorizontalDimensionBehaviour = DimensionBehaviour.MATCH_CONSTRAINT;
            B.VerticalDimensionBehaviour = DimensionBehaviour.MATCH_CONSTRAINT;
            B.setDimensionRatio("1:1");

            root.Measurer = sMeasurer;
            //root.setHorizontalDimensionBehaviour(DimensionBehaviour.WRAP_CONTENT);
            //        root.layout();
            //
            //        System.out.println(" direct: -> A: " + A + " B: " + B +  " C: "  + C);
            //
            //        Assert.AreEqual(A.getTop(), 0);
            //        Assert.AreEqual(B.getTop(), 20);
            //        Assert.AreEqual(C.getTop(), 580);
            //        Assert.AreEqual(A.getLeft(), 290);
            //        Assert.AreEqual(B.getLeft(), 20);
            //        Assert.AreEqual(C.getLeft(), 290);
            //        Assert.AreEqual(B.getWidth(), 560);
            //        Assert.AreEqual(B.getHeight(), B.getWidth());
            //
            //        //root.setHorizontalDimensionBehaviour(DimensionBehaviour.WRAP_CONTENT);
            //        root.setVerticalDimensionBehaviour(DimensionBehaviour.WRAP_CONTENT);
            //        root.layout();

            root.HorizontalDimensionBehaviour = DimensionBehaviour.WRAP_CONTENT;
            root.VerticalDimensionBehaviour = DimensionBehaviour.FIXED;
            root.Height = 600;
            root.layout();

            Console.WriteLine(" direct: -> A: " + A + " B: " + B + " C: " + C);

            Assert.AreEqual(A.Top, 0);
            Assert.AreEqual(B.Top, 290);
            Assert.AreEqual(C.Top, 580);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(B.Left, 0);
            Assert.AreEqual(C.Left, 0);
            Assert.AreEqual(B.Width, 20);
            Assert.AreEqual(B.Height, B.Width);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testLayoutWrapBarrier()
        [Test]
        public virtual void testLayoutWrapBarrier()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer("root", 600, 600);
            root.OptimizationLevel = Optimizer.OPTIMIZATION_GROUPING;
            ConstraintWidget A = new ConstraintWidget("A", 20, 20);
            ConstraintWidget B = new ConstraintWidget("B", 20, 20);
            ConstraintWidget C = new ConstraintWidget("C", 20, 20);
            Barrier barrier = new Barrier("Barrier");
            barrier.BarrierType = Barrier.BOTTOM;
            root.add(A);
            root.add(B);
            root.add(C);
            root.add(barrier);

            A.connect(Type.TOP, root, Type.TOP);
            B.connect(Type.TOP, A, Type.BOTTOM);
            B.Visibility = ConstraintWidget.GONE;
            C.connect(Type.TOP, barrier, Type.TOP);
            barrier.add(A);
            barrier.add(B);

            root.Measurer = sMeasurer;
            root.VerticalDimensionBehaviour = DimensionBehaviour.WRAP_CONTENT;
            root.layout();

            Console.WriteLine(" direct: -> root: " + root + " A: " + A + " B: " + B + " C: " + C + " Barrier: " + barrier.Top);

            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(A.Top, 0);
            Assert.AreEqual(B.Left, 0);
            Assert.AreEqual(B.Top, 20);
            Assert.AreEqual(C.Left, 0);
            Assert.AreEqual(C.Top, 20);
            Assert.AreEqual(barrier.Top, 20);
            Assert.AreEqual(root.Height, 40);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testLayoutWrapGuidelinesMatch()
        [Test]
        public virtual void testLayoutWrapGuidelinesMatch()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer("root", 600, 600);
            root.OptimizationLevel = Optimizer.OPTIMIZATION_GROUPING;
            //root.setOptimizationLevel(Optimizer.OPTIMIZATION_NONE);
            ConstraintWidget A = new ConstraintWidget("A", 20, 20);
            Guideline left = new Guideline();
            left.Orientation = Guideline.VERTICAL;
            left.GuideBegin = 30;
            left.DebugName = "L";
            Guideline right = new Guideline();
            right.Orientation = Guideline.VERTICAL;
            right.GuideEnd = 30;
            right.DebugName = "R";
            Guideline top = new Guideline();
            top.Orientation = Guideline.HORIZONTAL;
            top.GuideBegin = 30;
            top.DebugName = "T";
            Guideline bottom = new Guideline();
            bottom.Orientation = Guideline.HORIZONTAL;
            bottom.GuideEnd = 30;
            bottom.DebugName = "B";

            root.add(A);
            root.add(left);
            root.add(right);
            root.add(top);
            root.add(bottom);

            A.HorizontalDimensionBehaviour = DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = DimensionBehaviour.MATCH_CONSTRAINT;
            A.connect(Type.LEFT, left, Type.LEFT);
            A.connect(Type.RIGHT, right, Type.RIGHT);
            A.connect(Type.TOP, top, Type.TOP);
            A.connect(Type.BOTTOM, bottom, Type.BOTTOM);

            root.Measurer = sMeasurer;
            root.VerticalDimensionBehaviour = DimensionBehaviour.WRAP_CONTENT;
            root.layout();

            Console.WriteLine(" direct: -> root: " + root + " A: " + A + " L: " + left + " R: " + right + " T: " + top + " B: " + bottom);

            Assert.AreEqual(root.Height, 60);
            Assert.AreEqual(A.Left, 30);
            Assert.AreEqual(A.Top, 30);
            Assert.AreEqual(A.Width, 540);
            Assert.AreEqual(A.Height, 0);

        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testLayoutWrapMatch()
        [Test]
        public virtual void testLayoutWrapMatch()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer("root", 600, 600);
            root.OptimizationLevel = Optimizer.OPTIMIZATION_GROUPING;
            //        root.setOptimizationLevel(Optimizer.OPTIMIZATION_NONE);
            ConstraintWidget A = new ConstraintWidget("A", 50, 20);
            ConstraintWidget B = new ConstraintWidget("B", 50, 30);
            ConstraintWidget C = new ConstraintWidget("C", 50, 20);

            root.add(A);
            root.add(B);
            root.add(C);

            A.connect(Type.LEFT, root, Type.LEFT);
            A.connect(Type.TOP, root, Type.TOP);
            B.connect(Type.LEFT, A, Type.RIGHT);
            B.connect(Type.RIGHT, C, Type.LEFT);
            B.connect(Type.TOP, A, Type.BOTTOM);
            B.connect(Type.BOTTOM, C, Type.TOP);
            C.connect(Type.RIGHT, root, Type.RIGHT);
            C.connect(Type.BOTTOM, root, Type.BOTTOM);

            B.HorizontalDimensionBehaviour = DimensionBehaviour.MATCH_CONSTRAINT;
            B.VerticalDimensionBehaviour = DimensionBehaviour.WRAP_CONTENT;

            root.Measurer = sMeasurer;
            root.VerticalDimensionBehaviour = DimensionBehaviour.WRAP_CONTENT;
            root.layout();

            Console.WriteLine(" direct: -> root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(B.Top, 20);
            Assert.AreEqual(B.Bottom, 50);
            Assert.AreEqual(B.Left, 50);
            Assert.AreEqual(B.Right, 550);
            Assert.AreEqual(root.Height, 70);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testLayoutWrapBarrier2()
        [Test]
        public virtual void testLayoutWrapBarrier2()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer("root", 600, 600);
            root.OptimizationLevel = Optimizer.OPTIMIZATION_GROUPING;
            //root.setOptimizationLevel(Optimizer.OPTIMIZATION_NONE);
            ConstraintWidget A = new ConstraintWidget("A", 50, 20);
            ConstraintWidget B = new ConstraintWidget("B", 50, 30);
            ConstraintWidget C = new ConstraintWidget("C", 50, 20);
            Guideline guideline = new Guideline();
            guideline.DebugName = "end";
            guideline.GuideEnd = 40;
            guideline.Orientation = ConstraintWidget.VERTICAL;
            Barrier barrier = new Barrier();
            barrier.BarrierType = Barrier.LEFT;
            barrier.DebugName = "barrier";
            barrier.add(B);
            barrier.add(C);

            root.add(A);
            root.add(B);
            root.add(C);
            root.add(barrier);
            root.add(guideline);

            A.connect(Type.LEFT, root, Type.LEFT);
            A.connect(Type.RIGHT, barrier, Type.LEFT);
            B.connect(Type.RIGHT, guideline, Type.RIGHT);
            C.connect(Type.RIGHT, root, Type.RIGHT);

            root.Measurer = sMeasurer;
            root.HorizontalDimensionBehaviour = DimensionBehaviour.WRAP_CONTENT;
            root.layout();

            Console.WriteLine(" direct: -> root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(root.Width, 140);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testLayoutWrapBarrier3()
        [Test]
        public virtual void testLayoutWrapBarrier3()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer("root", 600, 600);
            root.OptimizationLevel = Optimizer.OPTIMIZATION_GROUPING;
            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            ConstraintWidget A = new ConstraintWidget("A", 50, 20);
            ConstraintWidget B = new ConstraintWidget("B", 50, 30);
            ConstraintWidget C = new ConstraintWidget("C", 50, 20);
            Guideline guideline = new Guideline();
            guideline.DebugName = "end";
            guideline.GuideEnd = 40;
            guideline.Orientation = ConstraintWidget.VERTICAL;
            Barrier barrier = new Barrier();
            barrier.BarrierType = Barrier.LEFT;
            barrier.DebugName = "barrier";
            barrier.add(B);
            barrier.add(C);

            root.add(A);
            root.add(B);
            root.add(C);
            root.add(barrier);
            root.add(guideline);

            A.connect(Type.LEFT, root, Type.LEFT);
            A.connect(Type.RIGHT, barrier, Type.LEFT);
            B.connect(Type.RIGHT, guideline, Type.RIGHT);
            C.connect(Type.RIGHT, root, Type.RIGHT);

            root.Measurer = sMeasurer;
            root.HorizontalDimensionBehaviour = DimensionBehaviour.WRAP_CONTENT;
            root.layout();

            Console.WriteLine(" direct: -> root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(root.Width, 140);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testSimpleGuideline2()
        [Test]
        public virtual void testSimpleGuideline2()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer("root", 600, 600);
            Guideline guidelineStart = new Guideline();
            guidelineStart.DebugName = "start";
            guidelineStart.setGuidePercent(0.1f);
            guidelineStart.Orientation = ConstraintWidget.VERTICAL;
            Guideline guidelineEnd = new Guideline();
            guidelineEnd.DebugName = "end";
            guidelineEnd.GuideEnd = 40;
            guidelineEnd.Orientation = ConstraintWidget.VERTICAL;
            ConstraintWidget A = new ConstraintWidget("A", 50, 20);
            root.add(A);
            root.add(guidelineStart);
            root.add(guidelineEnd);

            A.HorizontalDimensionBehaviour = DimensionBehaviour.MATCH_CONSTRAINT;
            A.connect(Type.LEFT, guidelineStart, Type.LEFT);
            A.connect(Type.RIGHT, guidelineEnd, Type.RIGHT);

            root.Measurer = sMeasurer;
            //root.setHorizontalDimensionBehaviour(DimensionBehaviour.WRAP_CONTENT);
            root.layout();
            Console.WriteLine(" root: " + root);
            Console.WriteLine("guideline start: " + guidelineStart);
            Console.WriteLine("guideline end: " + guidelineEnd);
        }
    }

}
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
    using Barrier = androidx.constraintlayout.core.widgets.Barrier;
    using ConstraintAnchor = androidx.constraintlayout.core.widgets.ConstraintAnchor;
    using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;
    using ConstraintWidgetContainer = androidx.constraintlayout.core.widgets.ConstraintWidgetContainer;
    using Guideline = androidx.constraintlayout.core.widgets.Guideline;
    using Optimizer = androidx.constraintlayout.core.widgets.Optimizer;
    //using Test = org.junit.Test;

    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.junit.Assert.Assert.AreEqual;

    /// <summary>
    /// Basic wrap test
    /// </summary>
    [TestFixture]
    public class WrapTest
    {

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testBasic()
        [Test]
        public virtual void testBasic()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            root.add(A);

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            //A大小由约束控制,也就是需要父控件大小
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            //父布局由子控件大小决定,与上面的冲突
            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;

            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A);

            A.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_SPREAD, 0, 100, 0);
            A.setVerticalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_SPREAD, 0, 60, 0);
            root.layout();
            Console.WriteLine("b) root: " + root + " A: " + A);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testBasic2()
        [Test]
        public virtual void testBasic2()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 600);
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
            //|A| | |
            //| |B| |
            //| | |C|
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.BOTTOM, C, ConstraintAnchor.Type.TOP);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_SPREAD, 0, 100, 1);
            B.setVerticalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_SPREAD, 0, 60, 1);

            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;

            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            root.layout();
            Console.WriteLine("root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(root.Width, 200);
            Assert.AreEqual(root.Height, 40);

            B.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_SPREAD, 20, 100, 1);
            B.setVerticalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_SPREAD, 30, 60, 1);
            root.Width = 0;
            root.Height = 0;
            root.layout();
            Console.WriteLine("root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(root.Width, 220);
            Assert.AreEqual(root.Height, 70);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testRatioWrap()
        [Test]
        public virtual void testRatioWrap()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 100, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            root.add(A);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);

            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.setDimensionRatio("1:1");

            root.Height = 0;
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;

            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            root.layout();
            Console.WriteLine("root: " + root + " A: " + A);
            Assert.AreEqual(root.Width, 100);
            Assert.AreEqual(root.Height, 100);

            root.Height = 600;
            root.Width = 0;
            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
            root.layout();
            Console.WriteLine("root: " + root + " A: " + A);
            Assert.AreEqual(root.Width, 600);
            Assert.AreEqual(root.Height, 600);

            root.Width = 100;
            root.Height = 600;
            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;

            root.layout();
            Console.WriteLine("root: " + root + " A: " + A);
            Assert.AreEqual(root.Width, 0);
            Assert.AreEqual(root.Height, 0);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testRatioWrap2()
        [Test]
        public virtual void testRatioWrap2()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            root.add(A);
            root.add(B);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);

            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.setDimensionRatio("1:1");

            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;

            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            root.layout();
            Console.WriteLine("root: " + root + " A: " + A + " B: " + B);
            Assert.AreEqual(root.Width, 100);
            Assert.AreEqual(root.Height, 120);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testRatioWrap3()
        [Test]
        public virtual void testRatioWrap3()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 500, 600);
            ConstraintWidget A = new ConstraintWidget(100, 60);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            C.DebugName = "C";
            root.add(A);
            root.add(B);
            root.add(C);

            A.BaselineDistance = 100;
            B.BaselineDistance = 10;
            C.BaselineDistance = 10;

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            A.VerticalBiasPercent = 0;

            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.BASELINE, A, ConstraintAnchor.Type.BASELINE);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT);

            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.BASELINE, B, ConstraintAnchor.Type.BASELINE);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);

            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.setDimensionRatio("1:1");

            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            root.layout();
            Console.WriteLine("root: " + root + " A: " + A + " B: " + B + " C: " + C);

            Assert.AreEqual(A.Width, 300);
            Assert.AreEqual(A.Height, 300);
            Assert.AreEqual(B.Left, 300);
            Assert.AreEqual(B.Top, 90);
            Assert.AreEqual(B.Width, 100);
            Assert.AreEqual(B.Height, 20);
            Assert.AreEqual(C.Left, 400);
            Assert.AreEqual(C.Top, 90);
            Assert.AreEqual(C.Width, 100);
            Assert.AreEqual(C.Height, 20);

            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            A.BaselineDistance = 10;

            root.layout();
            Console.WriteLine("root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(root.Width, 220);
            Assert.AreEqual(root.Height, 20);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testGoneChainWrap()
        [Test]
        public virtual void testGoneChainWrap()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 500, 600);
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

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            D.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);

            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.BOTTOM, C, ConstraintAnchor.Type.TOP);
            C.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.BOTTOM);
            C.connect(ConstraintAnchor.Type.BOTTOM, D, ConstraintAnchor.Type.TOP);
            D.connect(ConstraintAnchor.Type.TOP, C, ConstraintAnchor.Type.BOTTOM);
            D.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);

            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            D.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;

            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            root.layout();
            Console.WriteLine("root: " + root + " A: " + A + " B: " + B + " C: " + C + " D: " + D);
            Assert.AreEqual(root.Height, 40);

            A.VerticalChainStyle = ConstraintWidget.CHAIN_PACKED;
            root.layout();
            Console.WriteLine("root: " + root + " A: " + A + " B: " + B + " C: " + C + " D: " + D);
            Assert.AreEqual(root.Height, 40);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testWrap()
        [Test]
        public virtual void testWrap()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 500, 600);
            ConstraintWidget A = new ConstraintWidget(100, 0);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            ConstraintWidget D = new ConstraintWidget(100, 40);
            ConstraintWidget E = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            C.DebugName = "C";
            D.DebugName = "D";
            E.DebugName = "E";
            root.add(A);
            root.add(B);
            root.add(C);
            root.add(D);
            root.add(E);
            //|A |B|
            //|  |C|
            //|D
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            D.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, C, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            C.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.BOTTOM);
            D.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);

            E.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            E.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            E.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);

            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            root.layout();
            Console.WriteLine("root: " + root + " A: " + A + " B: " + B + " C: " + C + " D: " + D + " E: " + E);
            Assert.AreEqual(root.Height, 80);
            Assert.AreEqual(E.Top, 30);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testWrap2()
        [Test]
        public virtual void testWrap2()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 500, 600);
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
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            D.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            D.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            A.connect(ConstraintAnchor.Type.TOP, C, ConstraintAnchor.Type.BOTTOM, 30);
            A.connect(ConstraintAnchor.Type.BOTTOM, D, ConstraintAnchor.Type.TOP, 40);
            B.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();
            Console.WriteLine("root: " + root + " A: " + A + " B: " + B + " C: " + C + " D: " + D);
            Assert.AreEqual(C.Top, 0);
            Assert.AreEqual(A.Top, C.Bottom + 30);
            Assert.AreEqual(D.Top, A.Bottom + 40);
            Assert.AreEqual(root.Height, 20 + 30 + 20 + 40 + 20);

        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testWrap3()
        [Test]
        public virtual void testWrap3()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 500, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            root.add(A);
            root.add(B);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 200);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.LEFT, 250);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();
            Console.WriteLine("root: " + root + " A: " + A + " B: " + B);
            Assert.AreEqual(root.Width, A.Width + 200);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(B.Left, 250);
            Assert.AreEqual(B.Width, 100);
            Assert.AreEqual(B.Right > root.Width, true);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testWrap4()
        [Test]
        public virtual void testWrap4()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 500, 600);
            ConstraintWidget A = new ConstraintWidget(80, 80);
            ConstraintWidget B = new ConstraintWidget(60, 60);
            ConstraintWidget C = new ConstraintWidget(50, 100);
            Barrier barrier1 = new Barrier();
            barrier1.BarrierType = Barrier.BOTTOM;
            Barrier barrier2 = new Barrier();
            barrier2.BarrierType = Barrier.BOTTOM;

            barrier1.add(A);
            barrier1.add(B);

            barrier2.add(C);

            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            C.DebugName = "C";
            barrier1.DebugName = "B1";
            barrier2.DebugName = "B2";

            root.add(A);
            root.add(B);
            root.add(C);
            root.add(barrier1);
            root.add(barrier2);

            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, barrier1, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.BOTTOM, barrier1, ConstraintAnchor.Type.BOTTOM);

            C.connect(ConstraintAnchor.Type.TOP, barrier1, ConstraintAnchor.Type.BOTTOM);
            C.connect(ConstraintAnchor.Type.BOTTOM, barrier2, ConstraintAnchor.Type.TOP);

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);

            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();
            Console.WriteLine("root: " + root);
            Console.WriteLine("A: " + A);
            Console.WriteLine("B: " + B);
            Console.WriteLine("C: " + C);
            Console.WriteLine("B1: " + barrier1);
            Console.WriteLine("B2: " + barrier2);
            Assert.AreEqual(A.Top >= 0, true);
            Assert.AreEqual(B.Top >= 0, true);
            Assert.AreEqual(C.Top >= 0, true);
            Assert.AreEqual(root.Height, Math.Max(A.Height, B.Height) + C.Height);

        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testWrap5()
        [Test]
        public virtual void testWrap5()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 500, 600);
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

            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 8);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 8);

            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 8);
            B.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, 8);

            C.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
            C.connect(ConstraintAnchor.Type.BOTTOM, C, ConstraintAnchor.Type.TOP);
            D.HorizontalBiasPercent = 0.557f;
            D.VerticalBiasPercent = 0.8f;

            D.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            D.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            D.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
            D.connect(ConstraintAnchor.Type.BOTTOM, C, ConstraintAnchor.Type.TOP);
            D.HorizontalBiasPercent = 0.557f;
            D.VerticalBiasPercent = 0.28f;

            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();
            Console.WriteLine("root: " + root);
            Console.WriteLine("A: " + A);
            Console.WriteLine("B: " + B);
            Console.WriteLine("C: " + C);
            Console.WriteLine("D: " + D);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testWrap6()
        [Test]
        public virtual void testWrap6()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 500, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            Guideline guideline = new Guideline();
            guideline.Orientation = ConstraintWidget.VERTICAL;
            guideline.setGuidePercent( 0.5f);
            root.DebugName = "root";
            A.DebugName = "A";
            guideline.DebugName = "guideline";

            root.add(A);
            root.add(guideline);

            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 8);
            A.connect(ConstraintAnchor.Type.LEFT, guideline, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_WRAP, 0, 0, 0);

            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();
            Console.WriteLine("root: " + root);
            Console.WriteLine("A: " + A);
            Console.WriteLine("guideline: " + guideline);

            Assert.AreEqual(root.Width, A.Width * 2);
            Assert.AreEqual(root.Height, A.Height + 8);
            Assert.AreEqual((float)guideline.Left, root.Width / 2f, 0f);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(A.Height, 20);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testWrap7()
        [Test]
        public virtual void testWrap7()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 500, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget divider = new ConstraintWidget(1, 20);
            Guideline guideline = new Guideline();
            guideline.Orientation = ConstraintWidget.VERTICAL;
            guideline.setGuidePercent(0.5f);
            root.DebugName = "root";
            A.DebugName = "A";
            divider.DebugName = "divider";
            guideline.DebugName = "guideline";

            root.add(A);
            root.add(divider);
            root.add(guideline);

            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);

            A.connect(ConstraintAnchor.Type.LEFT, guideline, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);

            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_WRAP, 0, 0, 0);

            divider.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            divider.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            divider.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            divider.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            divider.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);

            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();
            Console.WriteLine("root: " + root);
            Console.WriteLine("A: " + A);
            Console.WriteLine("divider: " + divider);
            Console.WriteLine("guideline: " + guideline);

            Assert.AreEqual(root.Width, A.Width * 2);
            Assert.AreEqual(root.Height, A.Height);
            Assert.AreEqual((float)guideline.Left, root.Width / 2f, 0f);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(A.Height, 20);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testWrap8()
        [Test]
        public virtual void testWrap8()
        {
            // check_048
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1080, 1080);
            ConstraintWidget button56 = new ConstraintWidget(231, 126);
            ConstraintWidget button60 = new ConstraintWidget(231, 126);
            ConstraintWidget button63 = new ConstraintWidget(368, 368);
            ConstraintWidget button65 = new ConstraintWidget(231, 126);

            button56.DebugName = "button56";
            button60.DebugName = "button60";
            button63.DebugName = "button63";
            button65.DebugName = "button65";

            root.add(button56);
            root.add(button60);
            root.add(button63);
            root.add(button65);

            button56.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 42);
            button56.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 42);
            //button56.setBaselineDistance(77);

            button60.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 42);
            button60.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, 79);
            //button60.setBaselineDistance(77);

            button63.connect(ConstraintAnchor.Type.LEFT, button56, ConstraintAnchor.Type.RIGHT, 21);
            button63.connect(ConstraintAnchor.Type.RIGHT, button60, ConstraintAnchor.Type.LEFT, 21);
            button63.connect(ConstraintAnchor.Type.TOP, button56, ConstraintAnchor.Type.BOTTOM, 21);
            button63.connect(ConstraintAnchor.Type.BOTTOM, button60, ConstraintAnchor.Type.TOP, 21);
            //button63.setBaselineDistance(155);
            button63.VerticalBiasPercent = 0.8f;

            button65.connect(ConstraintAnchor.Type.LEFT, button56, ConstraintAnchor.Type.RIGHT, 21);
            button65.connect(ConstraintAnchor.Type.RIGHT, button60, ConstraintAnchor.Type.LEFT, 21);
            button65.connect(ConstraintAnchor.Type.TOP, button56, ConstraintAnchor.Type.BOTTOM, 21);
            button65.connect(ConstraintAnchor.Type.BOTTOM, button60, ConstraintAnchor.Type.TOP, 21);
            //button65.setBaselineDistance(77);
            button65.VerticalBiasPercent = 0.28f;

            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();
            Console.WriteLine("root: " + root);
            Console.WriteLine("button56: " + button56);
            Console.WriteLine("button60: " + button60);
            Console.WriteLine("button63: " + button63);
            Console.WriteLine("button65: " + button65);

            Assert.AreEqual(root.Width, 1080);
            Assert.AreEqual(root.Height, 783);
            Assert.AreEqual(button56.Left, 42);
            Assert.AreEqual(button56.Top, 42);
            Assert.AreEqual(button60.Left, 807);
            Assert.AreEqual(button60.Top, 578);
            Assert.AreEqual(button63.Left, 356);
            Assert.AreEqual(button63.Top, 189);
            Assert.AreEqual(button65.Left, 425);
            Assert.AreEqual(button65.Top, 257);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testWrap9()
        [Test]
        public virtual void testWrap9()
        {
            // b/161826272
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1080, 1080);
            ConstraintWidget text = new ConstraintWidget(270, 30);
            ConstraintWidget view = new ConstraintWidget(10, 10);

            root.DebugName = "root";
            text.DebugName = "text";
            view.DebugName = "view";

            root.add(text);
            root.add(view);

            text.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            text.connect(ConstraintAnchor.Type.TOP, view, ConstraintAnchor.Type.TOP);

            view.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            view.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            view.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            view.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);

            view.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            view.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            view.setVerticalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_PERCENT, 0, 0, 0.2f);
            view.setDimensionRatio("1:1");

            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;

            root.layout();
            Console.WriteLine("root: " + root);
            Console.WriteLine("text: " + text);
            Console.WriteLine("view: " + view);

            Assert.AreEqual(view.Width, view.Height);
            Assert.AreEqual(view.Height, (int)(0.2 * root.Height));
            Assert.AreEqual(root.Width, Math.Max(text.Width, view.Width));
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testBarrierWrap()
        [Test]
        public virtual void testBarrierWrap()
        {
            // b/165028374

            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1080, 1080);
            ConstraintWidget view = new ConstraintWidget(200, 200);
            ConstraintWidget space = new ConstraintWidget(50, 50);
            ConstraintWidget button = new ConstraintWidget(100, 80);
            ConstraintWidget text = new ConstraintWidget(90, 30);

            Barrier barrier = new Barrier();
            barrier.BarrierType = Barrier.BOTTOM;
            barrier.add(button);
            barrier.add(space);

            root.DebugName = "root";
            view.DebugName = "view";
            space.DebugName = "space";
            button.DebugName = "button";
            text.DebugName = "text";
            barrier.DebugName = "barrier";

            root.add(view);
            root.add(space);
            root.add(button);
            root.add(text);
            root.add(barrier);

            view.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            space.connect(ConstraintAnchor.Type.TOP, view, ConstraintAnchor.Type.BOTTOM);
            button.connect(ConstraintAnchor.Type.TOP, view, ConstraintAnchor.Type.BOTTOM);
            button.connect(ConstraintAnchor.Type.BOTTOM, text, ConstraintAnchor.Type.TOP);
            text.connect(ConstraintAnchor.Type.TOP, barrier, ConstraintAnchor.Type.BOTTOM);
            text.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);

            button.VerticalBiasPercent = 1f;
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;

            root.layout();
            Console.WriteLine("root: " + root);
            Console.WriteLine("view: " + view);
            Console.WriteLine("space: " + space);
            Console.WriteLine("button: " + button);
            Console.WriteLine("barrier: " + barrier);
            Console.WriteLine("text: " + text);

            Assert.AreEqual(view.Top, 0);
            Assert.AreEqual(view.Bottom, 200);
            Assert.AreEqual(space.Top, 200);
            Assert.AreEqual(space.Bottom, 250);
            Assert.AreEqual(button.Top, 200);
            Assert.AreEqual(button.Bottom, 280);
            Assert.AreEqual(barrier.Top, 280);
            Assert.AreEqual(text.Top, barrier.Top);
        }

    }

}
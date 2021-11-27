using NUnit.Framework;
using System;
using System.Collections.Generic;

/*
 * Copyright (C) 2016 The Android Open Source Project
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
    using ConstraintAnchor = androidx.constraintlayout.core.widgets.ConstraintAnchor;
    using Type = androidx.constraintlayout.core.widgets.ConstraintAnchor.Type;
    using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;
    using DimensionBehaviour = androidx.constraintlayout.core.widgets.ConstraintWidget.DimensionBehaviour;
    using ConstraintWidgetContainer = androidx.constraintlayout.core.widgets.ConstraintWidgetContainer;
    using Optimizer = androidx.constraintlayout.core.widgets.Optimizer;
    //using Test = org.junit.Test;

    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.junit.Assert.Assert.AreEqual;
    [TestFixture]
    public class AdvancedChainTesto
    {

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testComplexChainWeights()
        [Test]
        public virtual void testComplexChainWeights()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 800);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);

            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);

            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 0);
            A.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP, 0);

            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM, 0);
            B.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, 0);

            root.add(A);
            root.add(B);

            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;

            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            root.layout();

            Console.WriteLine("root: " + root);
            Console.WriteLine("A: " + A);
            Console.WriteLine("B: " + B);

            Assert.AreEqual(A.Width, 800);
            Assert.AreEqual(B.Width, 800);
            Assert.AreEqual(A.Height, 400);
            Assert.AreEqual(B.Height, 400);
            Assert.AreEqual(A.Top, 0);
            Assert.AreEqual(B.Top, 400);

            A.setDimensionRatio("16:3");

            root.layout();

            Console.WriteLine("root: " + root);
            Console.WriteLine("A: " + A);
            Console.WriteLine("B: " + B);

            Assert.AreEqual(A.Width, 800);
            Assert.AreEqual(B.Width, 800);
            Assert.AreEqual(A.Height, 150);
            Assert.AreEqual(B.Height, 150);
            Assert.AreEqual(A.Top, 167);
            Assert.AreEqual(B.Top, 483);

            B.VerticalWeight = 1;

            root.layout();

            Console.WriteLine("root: " + root);
            Console.WriteLine("A: " + A);
            Console.WriteLine("B: " + B);

            Assert.AreEqual(A.Width, 800);
            Assert.AreEqual(B.Width, 800);
            Assert.AreEqual(A.Height, 150);
            Assert.AreEqual(B.Height, 650);
            Assert.AreEqual(A.Top, 0);
            Assert.AreEqual(B.Top, 150);

            A.VerticalWeight = 1;

            root.layout();

            Console.WriteLine("root: " + root);
            Console.WriteLine("A: " + A);
            Console.WriteLine("B: " + B);

            Assert.AreEqual(A.Width, 800);
            Assert.AreEqual(B.Width, 800);
            Assert.AreEqual(A.Height, 150);
            Assert.AreEqual(B.Height, 150);
            Assert.AreEqual(A.Top, 167);
            Assert.AreEqual(B.Top, 483);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testTooSmall()
        [Test]
        public virtual void testTooSmall()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 800);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);

            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            C.setDebugSolverName(root.System, "C");

            root.add(A);
            root.add(B);
            root.add(C);

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);

            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT, 100);
            C.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT, 100);

            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.BOTTOM, C, ConstraintAnchor.Type.TOP);
            C.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.BOTTOM);
            C.connect(ConstraintAnchor.Type.BOTTOM, A, ConstraintAnchor.Type.BOTTOM);

            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            root.layout();

            Console.WriteLine("A: " + A);
            Console.WriteLine("B: " + B);
            Console.WriteLine("C: " + C);
            Assert.AreEqual(A.Top, 390);
            Assert.AreEqual(B.Top, 380);
            Assert.AreEqual(C.Top, 400);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testChainWeights()
        [Test]
        public virtual void testChainWeights()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 800);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);

            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 0);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT, 0);

            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT, 0);
            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 0);

            root.add(A);
            root.add(B);

            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.HorizontalWeight = 1;
            B.HorizontalWeight = 0;

            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            root.layout();

            Console.WriteLine("A: " + A);
            Console.WriteLine("B: " + B);
            Assert.AreEqual(A.Width, 800, 1);
            Assert.AreEqual(B.Width, 0, 1);
            Assert.AreEqual(A.Left, 0, 1);
            Assert.AreEqual(B.Left, 800, 1);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testChain3Weights()
        [Test]
        public virtual void testChain3Weights()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 800);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);

            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            C.setDebugSolverName(root.System, "C");

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 0);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT, 0);

            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT, 0);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT, 0);

            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT, 0);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 0);

            root.add(A);
            root.add(B);
            root.add(C);

            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            C.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;

            A.HorizontalWeight = 1;
            B.HorizontalWeight = 0;
            C.HorizontalWeight = 1;

            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            root.layout();

            Console.WriteLine("A: " + A);
            Console.WriteLine("B: " + B);
            Console.WriteLine("C: " + C);

            Assert.AreEqual(A.Width, 400);
            Assert.AreEqual(B.Width, 0);
            Assert.AreEqual(C.Width, 400);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(B.Left, 400);
            Assert.AreEqual(C.Left, 400);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testChainLastGone()
        [Test]
        public virtual void testChainLastGone()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 800);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            ConstraintWidget D = new ConstraintWidget(100, 20);
            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            C.setDebugSolverName(root.System, "C");
            D.setDebugSolverName(root.System, "D");
            root.add(A);
            root.add(B);
            root.add(C);
            root.add(D);

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 0);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 0);

            B.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 0);
            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 0);

            C.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 0);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 0);

            D.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 0);
            D.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 0);

            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 0);
            A.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP, 0);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM, 0);
            B.connect(ConstraintAnchor.Type.BOTTOM, C, ConstraintAnchor.Type.TOP, 0);
            C.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.BOTTOM, 0);
            C.connect(ConstraintAnchor.Type.BOTTOM, D, ConstraintAnchor.Type.TOP, 0);
            D.connect(ConstraintAnchor.Type.TOP, C, ConstraintAnchor.Type.BOTTOM, 0);
            D.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, 0);

            B.Visibility = ConstraintWidget.GONE;
            D.Visibility = ConstraintWidget.GONE;

            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            root.layout();

            Console.WriteLine("A: " + A);
            Console.WriteLine("B: " + B);
            Console.WriteLine("C: " + C);
            Console.WriteLine("D: " + D);

            Assert.AreEqual(A.Top, 253);
            Assert.AreEqual(C.Top, 527);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testRatioChainGone()
        [Test]
        public virtual void testRatioChainGone()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 800);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            ConstraintWidget ratio = new ConstraintWidget(100, 20);

            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            C.setDebugSolverName(root.System, "C");
            ratio.setDebugSolverName(root.System, "ratio");

            root.add(A);
            root.add(B);
            root.add(C);
            root.add(ratio);

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 0);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 0);

            B.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 0);
            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 0);

            C.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 0);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 0);

            ratio.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 0);
            ratio.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 0);
            ratio.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 0);

            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 0);
            A.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP, 0);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM, 0);
            B.connect(ConstraintAnchor.Type.BOTTOM, ratio, ConstraintAnchor.Type.BOTTOM, 0);
            C.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.TOP, 0);
            C.connect(ConstraintAnchor.Type.BOTTOM, ratio, ConstraintAnchor.Type.BOTTOM, 0);

            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            C.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            ratio.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;

            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            C.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            ratio.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            ratio.setDimensionRatio("4:3");

            B.Visibility = ConstraintWidget.GONE;
            C.Visibility = ConstraintWidget.GONE;

            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            root.layout();

            Console.WriteLine("A: " + A);
            Console.WriteLine("B: " + B);
            Console.WriteLine("C: " + C);
            Console.WriteLine("ratio: " + ratio);

            Assert.AreEqual(A.Height, 600);

            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;

            root.layout();

            Console.WriteLine("A: " + A);
            Console.WriteLine("B: " + B);
            Console.WriteLine("C: " + C);
            Console.WriteLine("ratio: " + ratio);
            Console.WriteLine("root: " + root);

            Assert.AreEqual(A.Height, 600);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testSimpleHorizontalChainPacked()
        [Test]
        public virtual void testSimpleHorizontalChainPacked()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);

            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            List<ConstraintWidget> widgets = new List<ConstraintWidget>();
            widgets.Add(A);
            widgets.Add(B);
            widgets.Add(root);
            root.add(A);
            root.add(B);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 20);
            B.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 20);

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 0);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT, 0);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT, 0);
            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 0);
            A.HorizontalChainStyle = ConstraintWidget.CHAIN_PACKED;
            root.layout();
            Console.WriteLine("a) A: " + A + " B: " + B);
            Assert.AreEqual(A.Left - root.Left, root.Right - B.Right, 1);
            Assert.AreEqual(B.Left - A.Right, 0, 1);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testSimpleVerticalTChainPacked()
        [Test]
        public virtual void testSimpleVerticalTChainPacked()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);

            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            List<ConstraintWidget> widgets = new List<ConstraintWidget>();
            widgets.Add(A);
            widgets.Add(B);
            widgets.Add(root);
            root.add(A);
            root.add(B);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 20);
            B.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 20);

            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 0);
            A.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP, 0);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM, 0);
            B.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, 0);
            A.VerticalChainStyle = ConstraintWidget.CHAIN_PACKED;
            root.layout();
            Console.WriteLine("a) A: " + A + " B: " + B);
            Assert.AreEqual(A.Top - root.Top, root.Bottom - B.Bottom, 1);
            Assert.AreEqual(B.Top - A.Bottom, 0, 1);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testHorizontalChainStyles()
        [Test]
        public virtual void testHorizontalChainStyles()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            root.add(A);
            root.add(B);
            root.add(C);
            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            C.setDebugSolverName(root.System, "C");
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 0);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT, 0);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT, 0);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT, 0);
            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT, 0);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 0);
            root.layout();
            Console.WriteLine("       spread) root: " + root + " A: " + A + " B: " + B + " C: " + C);
            int gap = (root.Width - A.Width - B.Width - C.Width) / 4;
            int size = 100;
            Assert.AreEqual(A.Width, size);
            Assert.AreEqual(B.Width, size);
            Assert.AreEqual(C.Width, size);
            Assert.AreEqual(gap, A.Left);
            Assert.AreEqual(A.Right + gap, B.Left);
            Assert.AreEqual(root.Width - gap - C.Width, C.Left);
            A.HorizontalChainStyle = ConstraintWidget.CHAIN_SPREAD_INSIDE;
            root.layout();
            Console.WriteLine("spread inside) root: " + root + " A: " + A + " B: " + B + " C: " + C);
            gap = (root.Width - A.Width - B.Width - C.Width) / 2;
            Assert.AreEqual(A.Width, size);
            Assert.AreEqual(B.Width, size);
            Assert.AreEqual(C.Width, size);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(A.Right + gap, B.Left);
            Assert.AreEqual(root.Width, C.Right);
            A.HorizontalChainStyle = ConstraintWidget.CHAIN_PACKED;
            root.layout();
            Console.WriteLine("       packed) root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Width, size);
            Assert.AreEqual(B.Width, size);
            Assert.AreEqual(C.Width, size);
            Assert.AreEqual(A.Left, gap);
            Assert.AreEqual(root.Width - gap, C.Right);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testVerticalChainStyles()
        [Test]
        public virtual void testVerticalChainStyles()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            root.add(A);
            root.add(B);
            root.add(C);
            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            C.setDebugSolverName(root.System, "C");
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 0);
            A.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP, 0);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM, 0);
            B.connect(ConstraintAnchor.Type.BOTTOM, C, ConstraintAnchor.Type.TOP, 0);
            C.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.BOTTOM, 0);
            C.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, 0);
            root.layout();
            Console.WriteLine("       spread) root: " + root + " A: " + A + " B: " + B + " C: " + C);
            int gap = (root.Height - A.Height - B.Height - C.Height) / 4;
            int size = 20;
            Assert.AreEqual(A.Height, size);
            Assert.AreEqual(B.Height, size);
            Assert.AreEqual(C.Height, size);
            Assert.AreEqual(gap, A.Top);
            Assert.AreEqual(A.Bottom + gap, B.Top);
            Assert.AreEqual(root.Height - gap - C.Height, C.Top);
            A.VerticalChainStyle = ConstraintWidget.CHAIN_SPREAD_INSIDE;
            root.layout();
            Console.WriteLine("spread inside) root: " + root + " A: " + A + " B: " + B + " C: " + C);
            gap = (root.Height - A.Height - B.Height - C.Height) / 2;
            Assert.AreEqual(A.Height, size);
            Assert.AreEqual(B.Height, size);
            Assert.AreEqual(C.Height, size);
            Assert.AreEqual(A.Top, 0);
            Assert.AreEqual(A.Bottom + gap, B.Top);
            Assert.AreEqual(root.Height, C.Bottom);
            A.VerticalChainStyle = ConstraintWidget.CHAIN_PACKED;
            root.layout();
            Console.WriteLine("       packed) root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Height, size);
            Assert.AreEqual(B.Height, size);
            Assert.AreEqual(C.Height, size);
            Assert.AreEqual(A.Top, gap);
            Assert.AreEqual(root.Height - gap, C.Bottom);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testPacked()
        [Test]
        public virtual void testPacked()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            root.add(A);
            root.add(B);
            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 0);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT, 0);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT, 0);
            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 0);
            int gap = (root.Width - A.Width - B.Width) / 2;
            int size = 100;
            A.HorizontalChainStyle = ConstraintWidget.CHAIN_PACKED;
            root.layout();
            root.OptimizationLevel = 0;
            Console.WriteLine("       packed) root: " + root + " A: " + A + " B: " + B);
            Assert.AreEqual(A.Width, size);
            Assert.AreEqual(B.Width, size);
            Assert.AreEqual(A.Left, gap);
        }
    }

}
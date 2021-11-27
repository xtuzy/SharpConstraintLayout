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
    using androidx.constraintlayout.core.widgets;
    using NUnit.Framework;

    //using Test = org.junit.Test;

    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.junit.Assert.Assert.AreEqual;
    [TestFixture]
    public class ChainTest
    {

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCenteringElementsWithSpreadChain()
[Test]
        public virtual void testCenteringElementsWithSpreadChain()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            ConstraintWidget D = new ConstraintWidget(100, 20);
            ConstraintWidget E = new ConstraintWidget(600, 20);

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

            A.connect(ConstraintAnchor.Type.LEFT, E, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, E, ConstraintAnchor.Type.RIGHT);

            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;

            C.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.RIGHT, A, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);

            D.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.LEFT);
            D.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.RIGHT);
            D.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.BOTTOM);

            root.layout();
            Console.WriteLine("A: " + A + " B: " + B + " C: " + C + " D: " + D + " E: " + E);
            Assert.AreEqual(A.Width, 300);
            Assert.AreEqual(B.Width, A.Width);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testBasicChainMatch()
        [Test]
        public virtual void testBasicChainMatch()
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

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            C.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);

            A.HorizontalChainStyle = ConstraintWidget.CHAIN_SPREAD;
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            C.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.BaselineDistance = 8;
            B.BaselineDistance = 8;
            C.BaselineDistance = 8;

            root.OptimizationLevel = Optimizer.OPTIMIZATION_STANDARD | Optimizer.OPTIMIZATION_CHAIN;
            root.layout();
            Console.WriteLine("A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(A.Right, 200);
            Assert.AreEqual(B.Left, 200);
            Assert.AreEqual(B.Right, 400);
            Assert.AreEqual(C.Left, 400);
            Assert.AreEqual(C.Right, 600);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testSpreadChainGone()
        [Test]
        public virtual void testSpreadChainGone()
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

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);

            A.HorizontalChainStyle = ConstraintWidget.CHAIN_SPREAD;
            A.Visibility = ConstraintWidget.GONE;

            root.layout();
            Console.WriteLine("A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(A.Right, 0);
            Assert.AreEqual(B.Left, 133);
            Assert.AreEqual(B.Right, 233);
            Assert.AreEqual(C.Left, 367);
            Assert.AreEqual(C.Right, 467);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testPackChainGone()
        [Test]
        public virtual void testPackChainGone()
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

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 100);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 20);

            A.HorizontalChainStyle = ConstraintWidget.CHAIN_PACKED;
            B.setGoneMargin(ConstraintAnchor.Type.RIGHT, 100);
            C.Visibility = ConstraintWidget.GONE;

            root.layout();
            Console.WriteLine("A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Left, 200);
            Assert.AreEqual(B.Left, 300);
            Assert.AreEqual(C.Left, 500);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(B.Width, 100);
            Assert.AreEqual(C.Width, 0);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testSpreadInsideChain2()
        [Test]
        public virtual void testSpreadInsideChain2()
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

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT, 25);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);

            A.HorizontalChainStyle = ConstraintWidget.CHAIN_SPREAD_INSIDE;
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;

            root.layout();
            Console.WriteLine("A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(A.Right, 100);
            Assert.AreEqual(B.Left, 100);
            Assert.AreEqual(B.Right, 475);
            Assert.AreEqual(C.Left, 500);
            Assert.AreEqual(C.Right, 600);
        }


        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testPackChain2()
        [Test]
        public virtual void testPackChain2()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            root.add(A);
            root.add(B);
            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.HorizontalChainStyle = ConstraintWidget.CHAIN_PACKED;
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_WRAP, 0, 0, 1);
            root.layout();
            Console.WriteLine("e) A: " + A + " B: " + B);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(B.Width, 100);
            Assert.AreEqual(A.Left, root.Width - B.Right);
            Assert.AreEqual(B.Left, A.Left + A.Width);
            // e) A: id: A (200, 0) - (100 x 20) B: id: B (300, 0) - (100 x 20) - pass
            // e) A: id: A (0, 0) - (100 x 20) B: id: B (100, 0) - (100 x 20)
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testPackChain()
        [Test]
        public virtual void testPackChain()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            root.add(A);
            root.add(B);
            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.HorizontalChainStyle = ConstraintWidget.CHAIN_PACKED;
            root.layout();
            Console.WriteLine("a) A: " + A + " B: " + B);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(B.Width, 100);
            Assert.AreEqual(A.Left, root.Width - B.Right);
            Assert.AreEqual(B.Left, A.Left + A.Width);
            A.Visibility = ConstraintWidget.GONE;
            root.layout();
            Console.WriteLine("b) A: " + A + " B: " + B);
            Assert.AreEqual(A.Width, 0);
            Assert.AreEqual(B.Width, 100);
            Assert.AreEqual(A.Left, root.Width - B.Right);
            Assert.AreEqual(B.Left, A.Left + A.Width);
            B.Visibility = ConstraintWidget.GONE;
            root.layout();
            Console.WriteLine("c) A: " + A + " B: " + B);
            Assert.AreEqual(A.Width, 0);
            Assert.AreEqual(B.Width, 0);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(B.Left, A.Left + A.Width);
            A.Visibility = ConstraintWidget.VISIBLE;
            A.Width = 100;
            root.layout();
            Console.WriteLine("d) A: " + A + " B: " + B);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(B.Width, 0);
            Assert.AreEqual(A.Left, root.Width - B.Right);
            Assert.AreEqual(B.Left, A.Left + A.Width);
            A.Visibility = ConstraintWidget.VISIBLE;
            A.Width = 100;
            A.Height = 20;
            B.Visibility = ConstraintWidget.VISIBLE;
            B.Width = 100;
            B.Height = 20;
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_WRAP, 0, 0, 1);
            root.layout();
            Console.WriteLine("e) A: " + A + " B: " + B);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(B.Width, 100);
            Assert.AreEqual(A.Left, root.Width - B.Right);
            Assert.AreEqual(B.Left, A.Left + A.Width);
            B.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_SPREAD, 0, 0, 1);
            root.layout();
            Console.WriteLine("f) A: " + A + " B: " + B);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(B.Width, 500);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(B.Left, 100);
            B.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_SPREAD, 0, 50, 1);
            root.layout();
            Console.WriteLine("g) A: " + A + " B: " + B);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(B.Width, 50);
            Assert.AreEqual(A.Left, root.Width - B.Right);
            Assert.AreEqual(B.Left, A.Left + A.Width);
            B.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_PERCENT, 0, 0, 0.3f);
            root.layout();
            Console.WriteLine("h) A: " + A + " B: " + B);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(B.Width, (int)(0.3f * 600));
            Assert.AreEqual(A.Left, root.Width - B.Right);
            Assert.AreEqual(B.Left, A.Left + A.Width);
            B.setDimensionRatio("16:9");
            B.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_RATIO, 0, 0, 1);
            root.layout();
            Console.WriteLine("i) A: " + A + " B: " + B);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(B.Width, (int)(16f / 9f * 20), 1);
            Assert.AreEqual(A.Left, root.Width - B.Right, 1);
            Assert.AreEqual(B.Left, A.Left + A.Width);
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_SPREAD, 0, 0, 1);
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_SPREAD, 0, 0, 1);
            B.setDimensionRatio(0, 0);
            A.Visibility = ConstraintWidget.VISIBLE;
            A.Width = 100;
            A.Height = 20;
            B.Visibility = ConstraintWidget.VISIBLE;
            B.Width = 100;
            B.Height = 20;
            root.layout();
            Console.WriteLine("j) A: " + A + " B: " + B);
            Assert.AreEqual(A.Width, B.Width);
            Assert.AreEqual(A.Width + B.Width, root.Width);
            A.HorizontalWeight = 1;
            B.HorizontalWeight = 3;
            root.layout();
            Console.WriteLine("k) A: " + A + " B: " + B);
            Assert.AreEqual(A.Width * 3, B.Width);
            Assert.AreEqual(A.Width + B.Width, root.Width);
        }

        /// <summary>
        /// testPackChain with current Chain Optimizations.
        /// </summary>
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testPackChainOpt()
        [Test]
        public virtual void testPackChainOpt()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            root.add(A);
            root.add(B);
            root.OptimizationLevel = Optimizer.OPTIMIZATION_DIRECT | Optimizer.OPTIMIZATION_BARRIER | Optimizer.OPTIMIZATION_CHAIN;
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.HorizontalChainStyle = ConstraintWidget.CHAIN_PACKED;
            root.layout();
            Console.WriteLine("a) A: " + A + " B: " + B);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(B.Width, 100);
            Assert.AreEqual(A.Left, root.Width - B.Right);
            Assert.AreEqual(B.Left, A.Left + A.Width);
            A.Visibility = ConstraintWidget.GONE;
            root.layout();
            Console.WriteLine("b) A: " + A + " B: " + B);
            Assert.AreEqual(A.Width, 0);
            Assert.AreEqual(B.Width, 100);
            Assert.AreEqual(A.Left, root.Width - B.Right);
            Assert.AreEqual(B.Left, A.Left + A.Width);
            B.Visibility = ConstraintWidget.GONE;
            root.layout();
            Console.WriteLine("c) A: " + A + " B: " + B);
            Assert.AreEqual(A.Width, 0);
            Assert.AreEqual(B.Width, 0);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(B.Left, A.Left + A.Width);
            A.Visibility = ConstraintWidget.VISIBLE;
            A.Width = 100;
            root.layout();
            Console.WriteLine("d) A: " + A + " B: " + B);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(B.Width, 0);
            Assert.AreEqual(A.Left, root.Width - B.Right);
            Assert.AreEqual(B.Left, A.Left + A.Width);
            A.Visibility = ConstraintWidget.VISIBLE;
            A.Width = 100;
            A.Height = 20;
            B.Visibility = ConstraintWidget.VISIBLE;
            B.Width = 100;
            B.Height = 20;
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_WRAP, 0, 0, 1);
            root.layout();
            Console.WriteLine("e) A: " + A + " B: " + B);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(B.Width, 100);
            Assert.AreEqual(A.Left, root.Width - B.Right);
            Assert.AreEqual(B.Left, A.Left + A.Width);
            B.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_SPREAD, 0, 0, 1);
            root.layout();
            Console.WriteLine("f) A: " + A + " B: " + B);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(B.Width, 500);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(B.Left, 100);
            B.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_SPREAD, 0, 50, 1);
            root.layout();
            Console.WriteLine("g) A: " + A + " B: " + B);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(B.Width, 50);
            Assert.AreEqual(A.Left, root.Width - B.Right);
            Assert.AreEqual(B.Left, A.Left + A.Width);
            B.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_PERCENT, 0, 0, 0.3f);
            root.layout();
            Console.WriteLine("h) A: " + A + " B: " + B);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(B.Width, (int)(0.3f * 600));
            Assert.AreEqual(A.Left, root.Width - B.Right);
            Assert.AreEqual(B.Left, A.Left + A.Width);
            B.setDimensionRatio("16:9");
            B.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_RATIO, 0, 0, 1);
            root.layout();
            Console.WriteLine("i) A: " + A + " B: " + B);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(B.Width, (int)(16f / 9f * 20), 1);
            Assert.AreEqual(A.Left, root.Width - B.Right, 1);
            Assert.AreEqual(B.Left, A.Left + A.Width);
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_SPREAD, 0, 0, 1);
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_SPREAD, 0, 0, 1);
            B.setDimensionRatio(0, 0);
            A.Visibility = ConstraintWidget.VISIBLE;
            A.Width = 100;
            A.Height = 20;
            B.Visibility = ConstraintWidget.VISIBLE;
            B.Width = 100;
            B.Height = 20;
            root.layout();
            Console.WriteLine("j) A: " + A + " B: " + B);
            Assert.AreEqual(A.Width, B.Width);
            Assert.AreEqual(A.Width + B.Width, root.Width);
            A.HorizontalWeight = 1;
            B.HorizontalWeight = 3;
            root.layout();
            Console.WriteLine("k) A: " + A + " B: " + B);
            Assert.AreEqual(A.Width * 3, B.Width);
            Assert.AreEqual(A.Width + B.Width, root.Width);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testSpreadChain()
        [Test]
        public virtual void testSpreadChain()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            root.add(A);
            root.add(B);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.HorizontalChainStyle = ConstraintWidget.CHAIN_SPREAD;
            root.layout();
            Console.WriteLine("a) A: " + A + " B: " + B);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(B.Width, 100);
            Assert.AreEqual(A.Left, B.Left - A.Right, 1);
            Assert.AreEqual(B.Left - A.Right, root.Width - B.Right, 1);
            B.Visibility = ConstraintWidget.GONE;
            root.layout();
            Console.WriteLine("b) A: " + A + " B: " + B);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testSpreadInsideChain()
        [Test]
        public virtual void testSpreadInsideChain()
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
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.HorizontalChainStyle = ConstraintWidget.CHAIN_SPREAD_INSIDE;
            root.layout();
            Console.WriteLine("a) A: " + A + " B: " + B);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(B.Width, 100);
            Assert.AreEqual(B.Right, root.Width);

            B.reset();
            root.add(B);
            B.DebugName = "B";
            B.Width = 100;
            B.Height = 20;
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            root.layout();
            Console.WriteLine("b) A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(B.Width, 100);
            Assert.AreEqual(C.Width, 100);
            Assert.AreEqual(B.Left - A.Right, C.Left - B.Right);
            int gap = (root.Width - A.Width - B.Width - C.Width) / 2;
            Assert.AreEqual(B.Left, A.Right + gap);
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
            List<ConstraintWidget> widgets = new List<ConstraintWidget>();
            widgets.Add(A);
            widgets.Add(B);
            widgets.Add(root);
            root.add(A);
            root.add(B);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            root.layout();
            Console.WriteLine("a) A: " + A + " B: " + B);
            Assert.AreEqual(A.Width, B.Width, 1);
            Assert.AreEqual(A.Left - root.Left, root.Right - B.Right, 1);
            Assert.AreEqual(A.Left - root.Left, B.Left - A.Right, 1);
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            root.layout();
            Console.WriteLine("b) A: " + A + " B: " + B);
            Assert.AreEqual(A.Width, root.Width - B.Width);
            Assert.AreEqual(B.Width, 100);
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
            A.Width = 100;
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            root.layout();
            Console.WriteLine("a) A: " + A + " B: " + B);
            Assert.AreEqual(B.Width, root.Width - A.Width);
            Assert.AreEqual(A.Width, 100);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testBasicVerticalChain()
        [Test]
        public virtual void testBasicVerticalChain()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            A.DebugName = "A";
            B.DebugName = "B";
            List<ConstraintWidget> widgets = new List<ConstraintWidget>();
            widgets.Add(A);
            widgets.Add(B);
            widgets.Add(root);
            root.add(A);
            root.add(B);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            root.layout();
            Console.WriteLine("a) A: " + A + " B: " + B);
            Assert.AreEqual(A.Height, B.Height, 1);
            Assert.AreEqual(A.Top - root.Top, root.Bottom - B.Bottom, 1);
            Assert.AreEqual(A.Top - root.Top, B.Top - A.Bottom, 1);
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            root.layout();
            Console.WriteLine("b) A: " + A + " B: " + B);
            Assert.AreEqual(A.Height, root.Height - B.Height);
            Assert.AreEqual(B.Height, 20);
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
            A.Height = 20;
            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            root.layout();
            Console.WriteLine("c) A: " + A + " B: " + B);
            Assert.AreEqual(B.Height, root.Height - A.Height);
            Assert.AreEqual(A.Height, 20);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testBasicChainThreeElements1()
        [Test]
        public virtual void testBasicChainThreeElements1()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            int marginL = 7;
            int marginR = 27;
            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            C.setDebugSolverName(root.System, "C");
            List<ConstraintWidget> widgets = new List<ConstraintWidget>();
            widgets.Add(A);
            widgets.Add(B);
            widgets.Add(C);
            widgets.Add(root);
            root.add(A);
            root.add(B);
            root.add(C);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 0);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT, 0);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT, 0);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT, 0);
            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT, 0);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 0);
            root.layout();
            Console.WriteLine("a) A: " + A + " B: " + B + " C: " + C);
            // all elements spread equally
            Assert.AreEqual(A.Width, B.Width, 1);
            Assert.AreEqual(B.Width, C.Width, 1);
            Assert.AreEqual(A.Left - root.Left, root.Right - C.Right, 1);
            Assert.AreEqual(A.Left - root.Left, B.Left - A.Right, 1);
            Assert.AreEqual(B.Left - A.Right, C.Left - B.Right, 1);
            // a) A: id: A (125, 0) - (100 x 20) B: id: B (350, 0) - (100 x 20) C: id: C (575, 0) - (100 x 20)
            // a) A: id: A (0, 0) - (100 x 20) B: id: B (100, 0) - (100 x 20) C: id: C (450, 0) - (100 x 20)
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testBasicChainThreeElements()
        [Test]
        public virtual void testBasicChainThreeElements()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            int marginL = 7;
            int marginR = 27;
            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            C.setDebugSolverName(root.System, "C");
            List<ConstraintWidget> widgets = new List<ConstraintWidget>();
            widgets.Add(A);
            widgets.Add(B);
            widgets.Add(C);
            widgets.Add(root);
            root.add(A);
            root.add(B);
            root.add(C);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 0);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT, 0);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT, 0);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT, 0);
            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT, 0);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 0);
            root.layout();
            Console.WriteLine("a) A: " + A + " B: " + B + " C: " + C);
            // all elements spread equally
            Assert.AreEqual(A.Width, B.Width, 1);
            Assert.AreEqual(B.Width, C.Width, 1);
            Assert.AreEqual(A.Left - root.Left, root.Right - C.Right, 1);
            Assert.AreEqual(A.Left - root.Left, B.Left - A.Right, 1);
            Assert.AreEqual(B.Left - A.Right, C.Left - B.Right, 1);
            // A marked as 0dp, B == C, A takes the rest
            A.getAnchor(ConstraintAnchor.Type.LEFT).Margin = marginL;
            A.getAnchor(ConstraintAnchor.Type.RIGHT).Margin = marginR;
            B.getAnchor(ConstraintAnchor.Type.LEFT).Margin = marginL;
            B.getAnchor(ConstraintAnchor.Type.RIGHT).Margin = marginR;
            C.getAnchor(ConstraintAnchor.Type.LEFT).Margin = marginL;
            C.getAnchor(ConstraintAnchor.Type.RIGHT).Margin = marginR;
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            root.layout();
            Console.WriteLine("b) A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Left - root.Left - marginL, root.Right - C.Right - marginR);
            Assert.AreEqual(C.Left - B.Right, B.Left - A.Right);
            int matchWidth = root.Width - B.Width - C.Width - marginL - marginR - 4 * (B.Left - A.Right);
            Assert.AreEqual(A.Width, 498);
            Assert.AreEqual(B.Width, C.Width);
            Assert.AreEqual(B.Width, 100);
            checkPositions(A, B, C);
            // B marked as 0dp, A == C, B takes the rest
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
            A.Width = 100;
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            root.layout();
            Console.WriteLine("c) A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(B.Width, 498);
            Assert.AreEqual(A.Width, C.Width);
            Assert.AreEqual(A.Width, 100);
            checkPositions(A, B, C);
            // C marked as 0dp, A == B, C takes the rest
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
            B.Width = 100;
            C.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            root.layout();
            Console.WriteLine("d) A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(C.Width, 498);
            Assert.AreEqual(A.Width, B.Width);
            Assert.AreEqual(A.Width, 100);
            checkPositions(A, B, C);
            // A & B marked as 0dp, C == 100
            C.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
            C.Width = 100;
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            root.layout();
            Console.WriteLine("e) A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(C.Width, 100);
            Assert.AreEqual(A.Width, B.Width); // L
            Assert.AreEqual(A.Width, 299);
            checkPositions(A, B, C);
            // A & C marked as 0dp, B == 100
            C.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
            B.Width = 100;
            root.layout();
            Console.WriteLine("f) A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(B.Width, 100);
            Assert.AreEqual(A.Width, C.Width);
            Assert.AreEqual(A.Width, 299);
            checkPositions(A, B, C);
            // B & C marked as 0dp, A == 100
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
            A.Width = 100;
            root.layout();
            Console.WriteLine("g) A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(B.Width, C.Width);
            Assert.AreEqual(B.Width, 299);
            checkPositions(A, B, C);
            // A == 0dp, B & C == 100, C is gone
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.Width = 100;
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
            B.Width = 100;
            C.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
            C.Width = 100;
            C.Visibility = ConstraintWidget.GONE;
            root.layout();
            Console.WriteLine("h) A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Width, 632);
            Assert.AreEqual(B.Width, 100);
            Assert.AreEqual(C.Width, 0);
            checkPositions(A, B, C);
        }

        private void checkPositions(ConstraintWidget A, ConstraintWidget B, ConstraintWidget C)
        {
            Assert.AreEqual(A.Left <= A.Right, true);
            Assert.AreEqual(A.Right <= B.Left, true);
            Assert.AreEqual(B.Left <= B.Right, true);
            Assert.AreEqual(B.Right <= C.Left, true);
            Assert.AreEqual(C.Left <= C.Right, true);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testBasicVerticalChainThreeElements()
        [Test]
        public virtual void testBasicVerticalChainThreeElements()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            int marginT = 7;
            int marginB = 27;
            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            C.setDebugSolverName(root.System, "C");
            List<ConstraintWidget> widgets = new List<ConstraintWidget>();
            widgets.Add(A);
            widgets.Add(B);
            widgets.Add(C);
            widgets.Add(root);
            root.add(A);
            root.add(B);
            root.add(C);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 0);
            A.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP, 0);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM, 0);
            B.connect(ConstraintAnchor.Type.BOTTOM, C, ConstraintAnchor.Type.TOP, 0);
            C.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.BOTTOM, 0);
            C.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, 0);
            root.layout();
            Console.WriteLine("a) A: " + A + " B: " + B + " C: " + C);
            // all elements spread equally
            Assert.AreEqual(A.Height, B.Height, 1);
            Assert.AreEqual(B.Height, C.Height, 1);
            Assert.AreEqual(A.Top - root.Top, root.Bottom - C.Bottom, 1);
            Assert.AreEqual(A.Top - root.Top, B.Top - A.Bottom, 1);
            Assert.AreEqual(B.Top - A.Bottom, C.Top - B.Bottom, 1);
            // A marked as 0dp, B == C, A takes the rest
            A.getAnchor(ConstraintAnchor.Type.TOP).Margin = marginT;
            A.getAnchor(ConstraintAnchor.Type.BOTTOM).Margin = marginB;
            B.getAnchor(ConstraintAnchor.Type.TOP).Margin = marginT;
            B.getAnchor(ConstraintAnchor.Type.BOTTOM).Margin = marginB;
            C.getAnchor(ConstraintAnchor.Type.TOP).Margin = marginT;
            C.getAnchor(ConstraintAnchor.Type.BOTTOM).Margin = marginB;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            root.layout();
            Console.WriteLine("b) A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Top, 7);
            Assert.AreEqual(C.Bottom, 573);
            Assert.AreEqual(B.Bottom, 519);
            Assert.AreEqual(A.Height, 458);
            Assert.AreEqual(B.Height, C.Height);
            Assert.AreEqual(B.Height, 20);
            checkVerticalPositions(A, B, C);
            // B marked as 0dp, A == C, B takes the rest
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
            A.Height = 20;
            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            root.layout();
            Console.WriteLine("c) A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(B.Height, 458);
            Assert.AreEqual(A.Height, C.Height);
            Assert.AreEqual(A.Height, 20);
            checkVerticalPositions(A, B, C);
            // C marked as 0dp, A == B, C takes the rest
            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
            B.Height = 20;
            C.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            root.layout();
            Console.WriteLine("d) A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(C.Height, 458);
            Assert.AreEqual(A.Height, B.Height);
            Assert.AreEqual(A.Height, 20);
            checkVerticalPositions(A, B, C);
            // A & B marked as 0dp, C == 20
            C.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
            C.Height = 20;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            root.layout();
            Console.WriteLine("e) A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(C.Height, 20);
            Assert.AreEqual(A.Height, B.Height); // L
            Assert.AreEqual(A.Height, 239);
            checkVerticalPositions(A, B, C);
            // A & C marked as 0dp, B == 20
            C.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
            B.Height = 20;
            root.layout();
            Console.WriteLine("f) A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(B.Height, 20);
            Assert.AreEqual(A.Height, C.Height);
            Assert.AreEqual(A.Height, 239);
            checkVerticalPositions(A, B, C);
            // B & C marked as 0dp, A == 20
            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
            A.Height = 20;
            root.layout();
            Console.WriteLine("g) A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Height, 20);
            Assert.AreEqual(B.Height, C.Height);
            Assert.AreEqual(B.Height, 239);
            checkVerticalPositions(A, B, C);
            // A == 0dp, B & C == 20, C is gone
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.Height = 20;
            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
            B.Height = 20;
            C.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
            C.Height = 20;
            C.Visibility = ConstraintWidget.GONE;
            root.layout();
            Console.WriteLine("h) A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Height, 512);
            Assert.AreEqual(B.Height, 20);
            Assert.AreEqual(C.Height, 0);
            checkVerticalPositions(A, B, C);
        }

        private void checkVerticalPositions(ConstraintWidget A, ConstraintWidget B, ConstraintWidget C)
        {
            Assert.AreEqual(A.Top <= A.Bottom, true);
            Assert.AreEqual(A.Bottom <= B.Top, true);
            Assert.AreEqual(B.Top <= B.Bottom, true);
            Assert.AreEqual(B.Bottom <= C.Top, true);
            Assert.AreEqual(C.Top <= C.Bottom, true);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testHorizontalChainWeights()
        [Test]
        public virtual void testHorizontalChainWeights()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            int marginL = 7;
            int marginR = 27;
            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            C.setDebugSolverName(root.System, "C");
            List<ConstraintWidget> widgets = new List<ConstraintWidget>();
            widgets.Add(A);
            widgets.Add(B);
            widgets.Add(C);
            widgets.Add(root);
            root.add(A);
            root.add(B);
            root.add(C);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, marginL);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT, marginR);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT, marginL);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT, marginR);
            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT, marginL);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, marginR);
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            C.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.HorizontalWeight = 1;
            B.HorizontalWeight = 1;
            C.HorizontalWeight = 1;
            root.layout();
            Console.WriteLine("a) A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Width, B.Width, 1);
            Assert.AreEqual(B.Width, C.Width, 1);
            A.HorizontalWeight = 1;
            B.HorizontalWeight = 2;
            C.HorizontalWeight = 1;
            root.layout();
            Console.WriteLine("b) A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(2 * A.Width, B.Width, 1);
            Assert.AreEqual(A.Width, C.Width, 1);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testVerticalChainWeights()
        [Test]
        public virtual void testVerticalChainWeights()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            int marginT = 7;
            int marginB = 27;
            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            C.setDebugSolverName(root.System, "C");
            List<ConstraintWidget> widgets = new List<ConstraintWidget>();
            widgets.Add(A);
            widgets.Add(B);
            widgets.Add(C);
            widgets.Add(root);
            root.add(A);
            root.add(B);
            root.add(C);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, marginT);
            A.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP, marginB);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM, marginT);
            B.connect(ConstraintAnchor.Type.BOTTOM, C, ConstraintAnchor.Type.TOP, marginB);
            C.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.BOTTOM, marginT);
            C.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, marginB);
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            C.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalWeight = 1;
            B.VerticalWeight = 1;
            C.VerticalWeight = 1;
            root.layout();
            Console.WriteLine("a) A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Height, B.Height, 1);
            Assert.AreEqual(B.Height, C.Height, 1);
            A.VerticalWeight = 1;
            B.VerticalWeight = 2;
            C.VerticalWeight = 1;
            root.layout();
            Console.WriteLine("b) A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(2 * A.Height, B.Height, 1);
            Assert.AreEqual(A.Height, C.Height, 1);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testHorizontalChainPacked()
        [Test]
        public virtual void testHorizontalChainPacked()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            int marginL = 7;
            int marginR = 27;
            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            C.setDebugSolverName(root.System, "C");
            List<ConstraintWidget> widgets = new List<ConstraintWidget>();
            widgets.Add(A);
            widgets.Add(B);
            widgets.Add(C);
            widgets.Add(root);
            root.add(A);
            root.add(B);
            root.add(C);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, marginL);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT, marginR);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT, marginL);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT, marginR);
            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT, marginL);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, marginR);
            A.HorizontalChainStyle = ConstraintWidget.CHAIN_PACKED;
            root.layout();
            Console.WriteLine("a) A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Left - root.Left - marginL, root.Right - marginR - C.Right, 1);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testVerticalChainPacked()
        [Test]
        public virtual void testVerticalChainPacked()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            int marginT = 7;
            int marginB = 27;
            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            C.setDebugSolverName(root.System, "C");
            List<ConstraintWidget> widgets = new List<ConstraintWidget>();
            widgets.Add(A);
            widgets.Add(B);
            widgets.Add(C);
            widgets.Add(root);
            root.add(A);
            root.add(B);
            root.add(C);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, marginT);
            A.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP, marginB);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM, marginT);
            B.connect(ConstraintAnchor.Type.BOTTOM, C, ConstraintAnchor.Type.TOP, marginB);
            C.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.BOTTOM, marginT);
            C.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, marginB);
            A.VerticalChainStyle = ConstraintWidget.CHAIN_PACKED;
            root.layout();
            Console.WriteLine("a) A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Top - root.Top - marginT, root.Bottom - marginB - C.Bottom, 1);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testHorizontalChainComplex()
        [Test]
        public virtual void testHorizontalChainComplex()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            ConstraintWidget D = new ConstraintWidget(50, 20);
            ConstraintWidget E = new ConstraintWidget(50, 20);
            ConstraintWidget F = new ConstraintWidget(50, 20);
            int marginL = 7;
            int marginR = 19;
            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            C.setDebugSolverName(root.System, "C");
            D.setDebugSolverName(root.System, "D");
            E.setDebugSolverName(root.System, "E");
            F.setDebugSolverName(root.System, "F");
            root.add(A);
            root.add(B);
            root.add(C);
            root.add(D);
            root.add(E);
            root.add(F);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, marginL);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT, marginR);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT, marginL);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT, marginR);
            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT, marginL);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, marginR);
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            C.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            D.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.LEFT, 0);
            D.connect(ConstraintAnchor.Type.RIGHT, A, ConstraintAnchor.Type.RIGHT, 0);
            E.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.LEFT, 0);
            E.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.RIGHT, 0);
            F.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.LEFT, 0);
            F.connect(ConstraintAnchor.Type.RIGHT, A, ConstraintAnchor.Type.RIGHT, 0);
            root.layout();
            Console.WriteLine("a) A: " + A + " B: " + B + " C: " + C);
            Console.WriteLine("a) D: " + D + " E: " + E + " F: " + F);
            Assert.AreEqual(A.Width, B.Width, 1);
            Assert.AreEqual(B.Width, C.Width, 1);
            Assert.AreEqual(A.Width, 307, 1);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testVerticalChainComplex()
        [Test]
        public virtual void testVerticalChainComplex()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            ConstraintWidget D = new ConstraintWidget(50, 20);
            ConstraintWidget E = new ConstraintWidget(50, 20);
            ConstraintWidget F = new ConstraintWidget(50, 20);
            int marginT = 7;
            int marginB = 19;
            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            C.setDebugSolverName(root.System, "C");
            D.setDebugSolverName(root.System, "D");
            E.setDebugSolverName(root.System, "E");
            F.setDebugSolverName(root.System, "F");
            root.add(A);
            root.add(B);
            root.add(C);
            root.add(D);
            root.add(E);
            root.add(F);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, marginT);
            A.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP, marginB);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM, marginT);
            B.connect(ConstraintAnchor.Type.BOTTOM, C, ConstraintAnchor.Type.TOP, marginB);
            C.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.BOTTOM, marginT);
            C.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, marginB);
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            C.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            D.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.TOP, 0);
            D.connect(ConstraintAnchor.Type.BOTTOM, A, ConstraintAnchor.Type.BOTTOM, 0);
            E.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.TOP, 0);
            E.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.BOTTOM, 0);
            F.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.TOP, 0);
            F.connect(ConstraintAnchor.Type.BOTTOM, A, ConstraintAnchor.Type.BOTTOM, 0);
            root.layout();
            Console.WriteLine("a) A: " + A + " B: " + B + " C: " + C);
            Console.WriteLine("a) D: " + D + " E: " + E + " F: " + F);
            Assert.AreEqual(A.Height, B.Height, 1);
            Assert.AreEqual(B.Height, C.Height, 1);
            Assert.AreEqual(A.Height, 174, 1);
        }


        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testHorizontalChainComplex2()
        [Test]
        public virtual void testHorizontalChainComplex2()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 379, 591);
            ConstraintWidget A = new ConstraintWidget(100, 185);
            ConstraintWidget B = new ConstraintWidget(100, 185);
            ConstraintWidget C = new ConstraintWidget(100, 185);
            ConstraintWidget D = new ConstraintWidget(53, 17);
            ConstraintWidget E = new ConstraintWidget(42, 17);
            ConstraintWidget F = new ConstraintWidget(47, 17);
            int marginL = 0;
            int marginR = 0;
            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            C.setDebugSolverName(root.System, "C");
            D.setDebugSolverName(root.System, "D");
            E.setDebugSolverName(root.System, "E");
            F.setDebugSolverName(root.System, "F");
            root.add(A);
            root.add(B);
            root.add(C);
            root.add(D);
            root.add(E);
            root.add(F);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 16);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, 16);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, marginL);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT, marginR);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT, marginL);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT, marginR);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.TOP, 0);
            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT, marginL);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, marginR);
            C.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.TOP, 0);
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            C.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            D.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.LEFT, 0);
            D.connect(ConstraintAnchor.Type.RIGHT, A, ConstraintAnchor.Type.RIGHT, 0);
            D.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM, 0);
            E.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.LEFT, 0);
            E.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.RIGHT, 0);
            E.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM, 0);
            F.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.LEFT, 0);
            F.connect(ConstraintAnchor.Type.RIGHT, A, ConstraintAnchor.Type.RIGHT, 0);
            F.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM, 0);
            root.layout();
            Console.WriteLine("a) A: " + A + " B: " + B + " C: " + C);
            Console.WriteLine("a) D: " + D + " E: " + E + " F: " + F);
            Assert.AreEqual(A.Width, B.Width, 1);
            Assert.AreEqual(B.Width, C.Width, 1);
            Assert.AreEqual(A.Width, 126);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testVerticalChainBaseline()
        [Test]
        public virtual void testVerticalChainBaseline()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            root.add(A);
            root.add(B);
            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            C.setDebugSolverName(root.System, "C");
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 0);
            A.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP, 0);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM, 0);
            B.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, 0);
            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A + " B: " + B);
            int Ay = A.Top;
            int By = B.Top;
            Assert.AreEqual(A.Top - root.Top, root.Bottom - B.Bottom, 1);
            Assert.AreEqual(B.Top - A.Bottom, A.Top - root.Top, 1);
            root.add(C);
            A.BaselineDistance = 7;
            C.BaselineDistance = 7;
            C.connect(ConstraintAnchor.Type.BASELINE, A, ConstraintAnchor.Type.BASELINE, 0);
            root.layout();
            Console.WriteLine("b) root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(Ay, C.Top, 1);
            A.VerticalChainStyle = ConstraintWidget.CHAIN_PACKED;
            root.layout();
            Console.WriteLine("c) root: " + root + " A: " + A + " B: " + B + " C: " + C);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testWrapHorizontalChain()
        [Test]
        public virtual void testWrapHorizontalChain()
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
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, 0);
            B.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 0);
            B.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, 0);
            C.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 0);
            C.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, 0);

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 0);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT, 0);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT, 0);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT, 0);
            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT, 0);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 0);
            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A + " B: " + B + " C: " + C);
            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();
            Console.WriteLine("b) root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(root.Height, A.Height);
            Assert.AreEqual(root.Height, B.Height);
            Assert.AreEqual(root.Height, C.Height);
            Assert.AreEqual(root.Width, A.Width + B.Width + C.Width);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testWrapVerticalChain()
        [Test]
        public virtual void testWrapVerticalChain()
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
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 0);
            B.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 0);
            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 0);
            C.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 0);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 0);

            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 0);
            A.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP, 0);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM, 0);
            B.connect(ConstraintAnchor.Type.BOTTOM, C, ConstraintAnchor.Type.TOP, 0);
            C.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.BOTTOM, 0);
            C.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, 0);
            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A + " B: " + B);
            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();
            Console.WriteLine("b) root: " + root + " A: " + A + " B: " + B);
            Assert.AreEqual(root.Width, A.Width);
            Assert.AreEqual(root.Width, B.Width);
            Assert.AreEqual(root.Width, C.Width);
            Assert.AreEqual(root.Height, A.Height + B.Height + C.Height);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testPackWithBaseline()
        [Test]
        public virtual void testPackWithBaseline()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 411, 603);
            ConstraintWidget A = new ConstraintWidget(118, 93, 88, 48);
            ConstraintWidget B = new ConstraintWidget(206, 93, 88, 48);
            ConstraintWidget C = new ConstraintWidget(69, 314, 88, 48);
            ConstraintWidget D = new ConstraintWidget(83, 458, 88, 48);
            root.add(A);
            root.add(B);
            root.add(C);
            root.add(D);
            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            C.setDebugSolverName(root.System, "C");
            D.setDebugSolverName(root.System, "D");
            A.BaselineDistance = 29;
            B.BaselineDistance = 29;
            C.BaselineDistance = 29;
            D.BaselineDistance = 29;
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 100);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.BASELINE, A, ConstraintAnchor.Type.BASELINE);
            C.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            D.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            D.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
            C.connect(ConstraintAnchor.Type.BOTTOM, D, ConstraintAnchor.Type.TOP);
            D.connect(ConstraintAnchor.Type.TOP, C, ConstraintAnchor.Type.BOTTOM);
            D.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            A.HorizontalChainStyle = ConstraintWidget.CHAIN_PACKED;
            C.VerticalChainStyle = ConstraintWidget.CHAIN_PACKED;
            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A + " B: " + B);
            Console.WriteLine("a) root: " + root + " C: " + C + " D: " + D);
            C.getAnchor(ConstraintAnchor.Type.TOP).reset();
            root.layout();
            C.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A + " B: " + B);
            Console.WriteLine("a) root: " + root + " C: " + C + " D: " + D);
            Assert.AreEqual(C.Bottom, D.Top);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testBasicGoneChain()
        [Test]
        public virtual void testBasicGoneChain()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
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
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.RIGHT, D, ConstraintAnchor.Type.LEFT);
            D.connect(ConstraintAnchor.Type.LEFT, C, ConstraintAnchor.Type.RIGHT);
            D.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.HorizontalChainStyle = ConstraintWidget.CHAIN_SPREAD_INSIDE;
            B.Visibility = ConstraintWidget.GONE;
            root.layout();
            Console.WriteLine("a) A: " + A + " B: " + B + " C: " + C + " D: " + D);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(C.Left, 250);
            Assert.AreEqual(D.Left, 500);
            B.Visibility = ConstraintWidget.VISIBLE;
            D.Visibility = ConstraintWidget.GONE;
            root.layout();
            Console.WriteLine("b) A: " + A + " B: " + B + " C: " + C + " D: " + D);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testGonePackChain()
        [Test]
        public virtual void testGonePackChain()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            Guideline guideline = new Guideline();
            ConstraintWidget D = new ConstraintWidget(100, 20);
            guideline.Orientation = Guideline.VERTICAL;
            guideline.GuideBegin = 200;
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            guideline.DebugName = "guideline";
            D.DebugName = "D";
            root.add(A);
            root.add(B);
            root.add(guideline);
            root.add(D);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, guideline, ConstraintAnchor.Type.LEFT);
            D.connect(ConstraintAnchor.Type.LEFT, guideline, ConstraintAnchor.Type.RIGHT);
            D.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.HorizontalChainStyle = ConstraintWidget.CHAIN_PACKED;
            A.Visibility = ConstraintWidget.GONE;
            B.Visibility = ConstraintWidget.GONE;
            root.layout();
            Console.WriteLine("a) A: " + A + " B: " + B + " guideline: " + guideline + " D: " + D);
            Assert.AreEqual(A.Width, 0);
            Assert.AreEqual(B.Width, 0);
            Assert.AreEqual(guideline.Left, 200);
            Assert.AreEqual(D.Left, 350);
            A.HorizontalChainStyle = ConstraintWidget.CHAIN_SPREAD;
            root.layout();
            Console.WriteLine("b) A: " + A + " B: " + B + " guideline: " + guideline + " D: " + D);
            Assert.AreEqual(A.Width, 0);
            Assert.AreEqual(B.Width, 0);
            Assert.AreEqual(guideline.Left, 200);
            Assert.AreEqual(D.Left, 350);
            A.HorizontalChainStyle = ConstraintWidget.CHAIN_SPREAD_INSIDE;
            root.layout();
            Console.WriteLine("c) A: " + A + " B: " + B + " guideline: " + guideline + " D: " + D);
            Assert.AreEqual(A.Width, 0);
            Assert.AreEqual(B.Width, 0);
            Assert.AreEqual(guideline.Left, 200);
            Assert.AreEqual(D.Left, 350);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testVerticalGonePackChain()
        [Test]
        public virtual void testVerticalGonePackChain()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            Guideline guideline = new Guideline();
            ConstraintWidget D = new ConstraintWidget(100, 20);
            guideline.Orientation = Guideline.HORIZONTAL;
            guideline.GuideBegin = 200;
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            guideline.DebugName = "guideline";
            D.DebugName = "D";
            root.add(A);
            root.add(B);
            root.add(guideline);
            root.add(D);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.BOTTOM, guideline, ConstraintAnchor.Type.TOP);
            D.connect(ConstraintAnchor.Type.TOP, guideline, ConstraintAnchor.Type.BOTTOM);
            D.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            A.VerticalChainStyle = ConstraintWidget.CHAIN_PACKED;
            A.Visibility = ConstraintWidget.GONE;
            B.Visibility = ConstraintWidget.GONE;
            root.layout();
            Console.WriteLine("a) A: " + A + " B: " + B + " guideline: " + guideline + " D: " + D);
            Assert.AreEqual(A.Height, 0);
            Assert.AreEqual(B.Height, 0);
            Assert.AreEqual(guideline.Top, 200);
            Assert.AreEqual(D.Top, 390);
            A.VerticalChainStyle = ConstraintWidget.CHAIN_SPREAD;
            root.layout();
            Console.WriteLine("b) A: " + A + " B: " + B + " guideline: " + guideline + " D: " + D);
            Assert.AreEqual(A.Height, 0);
            Assert.AreEqual(B.Height, 0);
            Assert.AreEqual(guideline.Top, 200);
            Assert.AreEqual(D.Top, 390);
            A.VerticalChainStyle = ConstraintWidget.CHAIN_SPREAD_INSIDE;
            root.layout();
            Console.WriteLine("c) A: " + A + " B: " + B + " guideline: " + guideline + " D: " + D);
            Assert.AreEqual(A.Height, 0);
            Assert.AreEqual(B.Height, 0);
            Assert.AreEqual(guideline.Top, 200);
            Assert.AreEqual(D.Top, 390);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testVerticalDanglingChain()
        [Test]
        public virtual void testVerticalDanglingChain()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 1000);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            root.add(A);
            root.add(B);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP, 7);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM, 9);
            root.layout();
            Console.WriteLine("a) A: " + A + " B: " + B);
            Assert.AreEqual(A.Top, 0);
            Assert.AreEqual(B.Top, A.Height + Math.Max(7, 9));
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testHorizontalWeightChain()
        [Test]
        public virtual void testHorizontalWeightChain()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 1000);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            Guideline guidelineLeft = new Guideline();
            Guideline guidelineRight = new Guideline();

            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            C.DebugName = "C";
            guidelineLeft.DebugName = "guidelineLeft";
            guidelineRight.DebugName = "guidelineRight";
            root.add(A);
            root.add(B);
            root.add(C);
            root.add(guidelineLeft);
            root.add(guidelineRight);

            guidelineLeft.Orientation = Guideline.VERTICAL;
            guidelineRight.Orientation = Guideline.VERTICAL;
            guidelineLeft.GuideBegin = 20;
            guidelineRight.GuideEnd = 20;

            A.connect(ConstraintAnchor.Type.LEFT, guidelineLeft, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.RIGHT, guidelineRight, ConstraintAnchor.Type.RIGHT);
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            C.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.HorizontalWeight = 1;
            B.HorizontalWeight = 1;
            C.HorizontalWeight = 1;
            root.layout();
            Console.WriteLine("a) A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Left, 20);
            Assert.AreEqual(B.Left, 207);
            Assert.AreEqual(C.Left, 393);
            Assert.AreEqual(A.Width, 187);
            Assert.AreEqual(B.Width, 186);
            Assert.AreEqual(C.Width, 187);
            C.Visibility = ConstraintWidget.GONE;
            root.layout();
            Console.WriteLine("b) A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Left, 20);
            Assert.AreEqual(B.Left, 300);
            Assert.AreEqual(C.Left, 580);
            Assert.AreEqual(A.Width, 280);
            Assert.AreEqual(B.Width, 280);
            Assert.AreEqual(C.Width, 0);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testVerticalGoneChain()
        [Test]
        public virtual void testVerticalGoneChain()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            List<ConstraintWidget> widgets = new List<ConstraintWidget>();
            widgets.Add(A);
            widgets.Add(B);
            widgets.Add(root);
            root.add(A);
            root.add(B);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 16);
            A.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP);
            A.getAnchor(ConstraintAnchor.Type.BOTTOM).GoneMargin = 16;
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, 16);
            A.VerticalChainStyle = ConstraintWidget.CHAIN_PACKED;
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A + " B: " + B);
            Assert.AreEqual(A.Height, B.Height, 1);
            Assert.AreEqual(A.Top - root.Top, root.Bottom - B.Bottom, 1);
            Assert.AreEqual(A.Bottom, B.Top);

            B.Visibility = ConstraintWidget.GONE;
            root.layout();
            Console.WriteLine("b) root: " + root + " A: " + A + " B: " + B);
            Assert.AreEqual(A.Top - root.Top, root.Bottom - A.Bottom);
            Assert.AreEqual(root.Height, 52);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testVerticalGoneChain2()
        [Test]
        public virtual void testVerticalGoneChain2()
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
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 16);
            A.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.BOTTOM, C, ConstraintAnchor.Type.TOP);
            B.getAnchor(ConstraintAnchor.Type.TOP).GoneMargin = 16;
            B.getAnchor(ConstraintAnchor.Type.BOTTOM).GoneMargin = 16;
            C.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.BOTTOM);
            C.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, 16);
            A.VerticalChainStyle = ConstraintWidget.CHAIN_PACKED;
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Top - root.Top, root.Bottom - C.Bottom, 1);
            Assert.AreEqual(A.Bottom, B.Top);

            A.Visibility = ConstraintWidget.GONE;
            C.Visibility = ConstraintWidget.GONE;
            root.layout();
            Console.WriteLine("b) root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(B.Top - root.Top, root.Bottom - B.Bottom);
            Assert.AreEqual(root.Height, 52);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testVerticalSpreadInsideChain()
        [Test]
        public virtual void testVerticalSpreadInsideChain()
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
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 16);
            A.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.BOTTOM, C, ConstraintAnchor.Type.TOP);
            C.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.BOTTOM);
            C.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, 16);

            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            C.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;

            A.VerticalChainStyle = ConstraintWidget.CHAIN_SPREAD_INSIDE;
            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A + " B: " + B + " C: " + C);

            Assert.AreEqual(A.Height, B.Height, 1);
            Assert.AreEqual(B.Height, C.Height, 1);
            Assert.AreEqual(A.Height, (root.Height - 32) / 3, 1);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testHorizontalSpreadMaxChain()
        [Test]
        public virtual void testHorizontalSpreadMaxChain()
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
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);

            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            C.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;

            A.VerticalChainStyle = ConstraintWidget.CHAIN_SPREAD_INSIDE;
            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Width, B.Width, 1);
            Assert.AreEqual(B.Width, C.Width, 1);
            Assert.AreEqual(A.Width, 200, 1);

            A.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_SPREAD, 0, 50, 1);
            B.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_SPREAD, 0, 50, 1);
            C.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_SPREAD, 0, 50, 1);
            root.layout();
            Console.WriteLine("b) root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Width, B.Width, 1);
            Assert.AreEqual(B.Width, C.Width, 1);
            Assert.AreEqual(A.Width, 50, 1);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testPackCenterChain()
        [Test]
        public virtual void testPackCenterChain()
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

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 16);
            B.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 16);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 16);

            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            C.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            C.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);

            A.VerticalChainStyle = ConstraintWidget.CHAIN_PACKED;
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.MinHeight = 300;
            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(root.Height, 300);
            Assert.AreEqual(C.Top, (root.Height - C.Height) / 2);
            Assert.AreEqual(A.Top, (root.Height - A.Height - B.Height) / 2);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testPackCenterChainGone()
        [Test]
        public virtual void testPackCenterChainGone()
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

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 16);
            B.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 16);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 16);

            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.BOTTOM, C, ConstraintAnchor.Type.TOP);
            C.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.BOTTOM);
            C.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);

            A.VerticalChainStyle = ConstraintWidget.CHAIN_PACKED;
            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(600, root.Height);
            Assert.AreEqual(20, A.Height);
            Assert.AreEqual(20, B.Height);
            Assert.AreEqual(20, C.Height);
            Assert.AreEqual(270, A.Top);
            Assert.AreEqual(290, B.Top);
            Assert.AreEqual(310, C.Top);

            A.Visibility = ConstraintWidget.GONE;
            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(600, root.Height);
            Assert.AreEqual(0, A.Height);
            Assert.AreEqual(20, B.Height);
            Assert.AreEqual(20, C.Height); // todo not done
            Assert.AreEqual(A.Top, B.Top);
            Assert.AreEqual((600 - 40) / 2, B.Top);
            Assert.AreEqual(B.Top + B.Height, C.Top);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testSpreadInsideChainWithMargins()
        [Test]
        public virtual void testSpreadInsideChainWithMargins()
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

            int marginOut = 0;

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, marginOut);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, marginOut);
            A.HorizontalChainStyle = ConstraintWidget.CHAIN_SPREAD_INSIDE;
            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Left, marginOut);
            Assert.AreEqual(C.Right, root.Width - marginOut);
            Assert.AreEqual(B.Left, A.Right + (C.Left - A.Right - B.Width) / 2);

            marginOut = 20;
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, marginOut);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, marginOut);
            root.layout();
            Console.WriteLine("b) root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Left, marginOut);
            Assert.AreEqual(C.Right, root.Width - marginOut);
            Assert.AreEqual(B.Left, A.Right + (C.Left - A.Right - B.Width) / 2);
        }
    }
}
using NUnit.Framework;
using System;

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
    using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;
    using ConstraintWidgetContainer = androidx.constraintlayout.core.widgets.ConstraintWidgetContainer;
    using Optimizer = androidx.constraintlayout.core.widgets.Optimizer;
    //using Test = org.junit.Test;

    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.junit.Assert.Assert.AreEqual;
    [TestFixture]
    public class ChainWrapContentTest
    {

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testVerticalWrapContentChain()
        [Test]
        public virtual void testVerticalWrapContentChain()
        {
            testVerticalWrapContentChain(Optimizer.OPTIMIZATION_NONE);
            testVerticalWrapContentChain(Optimizer.OPTIMIZATION_STANDARD);
        }

        public virtual void testVerticalWrapContentChain(int directResolution)
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
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 10);
            A.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.BOTTOM, C, ConstraintAnchor.Type.TOP);
            C.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.BOTTOM);
            C.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, 32);
            root.layout();
            Console.WriteLine("res: " + directResolution + " root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Top, 10);
            Assert.AreEqual(B.Top, 30);
            Assert.AreEqual(C.Top, 30);
            Assert.AreEqual(root.Height, 82);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testHorizontalWrapContentChain()
        [Test]
        public virtual void testHorizontalWrapContentChain()
        {
            testHorizontalWrapContentChain(Optimizer.OPTIMIZATION_NONE);
            testHorizontalWrapContentChain(Optimizer.OPTIMIZATION_STANDARD);
        }

        public virtual void testHorizontalWrapContentChain(int directResolution)
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
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 10);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 32);
            root.layout();
            Console.WriteLine("1/ res: " + directResolution + " root: " + root + " A: " + A + " B: " + B + " C: " + C);
            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();
            Console.WriteLine("2/ res: " + directResolution + " root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Left, 10);
            Assert.AreEqual(B.Left, 110);
            Assert.AreEqual(C.Left, 110);
            Assert.AreEqual(root.Width, 242);
            root.MinWidth = 400;
            root.layout();
            Console.WriteLine("3/ res: " + directResolution + " root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Left, 10);
            Assert.AreEqual(B.Left, 110);
            Assert.AreEqual(C.Left, 268);
            Assert.AreEqual(root.Width, 400);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testVerticalWrapContentChain3Elts()
        [Test]
        public virtual void testVerticalWrapContentChain3Elts()
        {
            testVerticalWrapContentChain3Elts(Optimizer.OPTIMIZATION_NONE);
            testVerticalWrapContentChain3Elts(Optimizer.OPTIMIZATION_STANDARD);
        }

        public virtual void testVerticalWrapContentChain3Elts(int directResolution)
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
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            C.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 10);
            A.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.BOTTOM, C, ConstraintAnchor.Type.TOP);
            C.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.BOTTOM);
            C.connect(ConstraintAnchor.Type.BOTTOM, D, ConstraintAnchor.Type.TOP);
            D.connect(ConstraintAnchor.Type.TOP, C, ConstraintAnchor.Type.BOTTOM);
            D.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, 32);
            root.layout();
            Console.WriteLine("res: " + directResolution + " root: " + root + " A: " + A + " B: " + B + " C: " + C + " D: " + D);
            Assert.AreEqual(A.Top, 10);
            Assert.AreEqual(B.Top, 30);
            Assert.AreEqual(C.Top, 30);
            Assert.AreEqual(D.Top, 30);
            Assert.AreEqual(root.Height, 82);
            root.MinHeight = 300;
            root.layout();
            Console.WriteLine("res: " + directResolution + " root: " + root + " A: " + A + " B: " + B + " C: " + C + " D: " + D);
            Assert.AreEqual(A.Top, 10);
            Assert.AreEqual(B.Top, 30);
            Assert.AreEqual(C.Top, 139);
            Assert.AreEqual(D.Top, 248);
            Assert.AreEqual(root.Height, 300);
            root.Height = 600;
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
            root.layout();
            Console.WriteLine("res: " + directResolution + " root: " + root + " A: " + A + " B: " + B + " C: " + C + " D: " + D);
            Assert.AreEqual(A.Top, 10);
            Assert.AreEqual(B.Top, 30);
            Assert.AreEqual(C.Top, 289);
            Assert.AreEqual(D.Top, 548);
            Assert.AreEqual(root.Height, 600);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testHorizontalWrapContentChain3Elts()
        [Test]
        public virtual void testHorizontalWrapContentChain3Elts()
        {
            testHorizontalWrapContentChain3Elts(Optimizer.OPTIMIZATION_NONE);
            testHorizontalWrapContentChain3Elts(Optimizer.OPTIMIZATION_STANDARD);
        }

        public virtual void testHorizontalWrapContentChain3Elts(int directResolution)
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
            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            C.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 10);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.RIGHT, D, ConstraintAnchor.Type.LEFT);
            D.connect(ConstraintAnchor.Type.LEFT, C, ConstraintAnchor.Type.RIGHT);
            D.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 32);
            root.layout();
            Console.WriteLine("res: " + directResolution + " root: " + root + " A: " + A + " B: " + B + " C: " + C + " D: " + D);
            Assert.AreEqual(A.Left, 10);
            Assert.AreEqual(B.Left, 110);
            Assert.AreEqual(C.Left, 110);
            Assert.AreEqual(D.Left, 110);
            Assert.AreEqual(root.Width, 242);
            root.MinWidth = 300;
            root.layout();
            Console.WriteLine("res: " + directResolution + " root: " + root + " A: " + A + " B: " + B + " C: " + C + " D: " + D);
            Assert.AreEqual(A.Left, 10);
            Assert.AreEqual(B.Left, 110);
            Assert.AreEqual(C.Left, 139);
            Assert.AreEqual(D.Left, 168);
            Assert.AreEqual(root.Width, 300);
            root.Width = 600;
            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
            root.layout();
            Console.WriteLine("res: " + directResolution + " root: " + root + " A: " + A + " B: " + B + " C: " + C + " D: " + D);
            Assert.AreEqual(A.Left, 10);
            Assert.AreEqual(B.Left, 110);
            Assert.AreEqual(C.Left, 289);
            Assert.AreEqual(D.Left, 468);
            Assert.AreEqual(root.Width, 600);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testHorizontalWrapChain()
        [Test]
        public virtual void testHorizontalWrapChain()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 1000);
            ConstraintWidget A = new ConstraintWidget(20, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(20, 20);
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
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_WRAP, 0, 0, 0);
            B.Width = 600;
            root.layout();
            Console.WriteLine("a) A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(B.Left, 20);
            Assert.AreEqual(C.Left, 580);
            A.HorizontalChainStyle = ConstraintWidget.CHAIN_PACKED;
            B.Width = 600;
            root.layout();
            Console.WriteLine("b) A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(B.Left, 20);
            Assert.AreEqual(C.Left, 580); // doesn't expand beyond
            B.Width = 100;
            root.layout();
            Console.WriteLine("c) A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Left, 230);
            Assert.AreEqual(B.Left, 250);
            Assert.AreEqual(C.Left, 350);
            B.Width = 600;
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            C.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            root.layout();
            Console.WriteLine("d) root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(root.Height, 20);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(B.Left, 20);
            Assert.AreEqual(C.Left, 580);
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
            B.Width = 600;
            root.Width = 0;
            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();
            Console.WriteLine("e) root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(root.Height, 20);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(B.Left, 20);
            Assert.AreEqual(C.Left, 620);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testWrapChain()
        [Test]
        public virtual void testWrapChain()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1440, 1944);
            ConstraintWidget A = new ConstraintWidget(308, 168);
            ConstraintWidget B = new ConstraintWidget(308, 168);
            ConstraintWidget C = new ConstraintWidget(308, 168);
            ConstraintWidget D = new ConstraintWidget(308, 168);
            ConstraintWidget E = new ConstraintWidget(308, 168);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            C.DebugName = "C";
            D.DebugName = "D";
            E.DebugName = "E";
            root.add(E);
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
            E.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            E.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            E.connect(ConstraintAnchor.Type.TOP, C, ConstraintAnchor.Type.BOTTOM);
            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A + " B: " + B + " C: " + C + " D: " + D + " E: " + E);
            Assert.AreEqual(root.Width, 1440);
            Assert.AreEqual(root.Height, 336);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testWrapDanglingChain()
        [Test]
        public virtual void testWrapDanglingChain()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1440, 1944);
            ConstraintWidget A = new ConstraintWidget(308, 168);
            ConstraintWidget B = new ConstraintWidget(308, 168);
            ConstraintWidget C = new ConstraintWidget(308, 168);
            ConstraintWidget D = new ConstraintWidget(308, 168);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            root.add(A);
            root.add(B);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A + " B: " + B);
            Assert.AreEqual(root.Width, 616);
            Assert.AreEqual(root.Height, 168);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(B.Left, 308);
            Assert.AreEqual(A.Width, 308);
            Assert.AreEqual(B.Width, 308);
        }
    }

}
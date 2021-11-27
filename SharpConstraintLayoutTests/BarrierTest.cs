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
    using androidx.constraintlayout.core.widgets;
    using NUnit.Framework;

    //using Test = org.junit.Test;

    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.junit.Assert.Assert.AreEqual;

    /// <summary>
    /// Tests for Barriers
    /// </summary>
	[TestFixture]
    public class BarrierTest
    {

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void barrierConstrainedWidth()
        [Test]
        public virtual void barrierConstrainedWidth()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(200, 20);
            Barrier barrier = new Barrier();
            Guideline guidelineStart = new Guideline();
            Guideline guidelineEnd = new Guideline();
            guidelineStart.Orientation = ConstraintWidget.VERTICAL;
            guidelineEnd.Orientation = ConstraintWidget.VERTICAL;
            guidelineStart.GuideBegin = 30;
            guidelineEnd.GuideEnd = 20;

            root.setDebugSolverName(root.System, "root");
            guidelineStart.setDebugSolverName(root.System, "guidelineStart");
            guidelineEnd.setDebugSolverName(root.System, "guidelineEnd");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            barrier.setDebugSolverName(root.System, "Barrier");
            barrier.BarrierType = Barrier.LEFT;

            barrier.add(A);
            barrier.add(B);

            root.add(A);
            root.add(B);
            root.add(guidelineStart);
            root.add(guidelineEnd);
            root.add(barrier);

            A.connect(ConstraintAnchor.Type.LEFT, guidelineStart, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, guidelineEnd, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.LEFT, guidelineStart, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.RIGHT, guidelineEnd, ConstraintAnchor.Type.RIGHT);
            A.HorizontalBiasPercent = 1;
            B.HorizontalBiasPercent = 1;

            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();
            Console.WriteLine("root: " + root);
            Console.WriteLine("guidelineStart: " + guidelineStart);
            Console.WriteLine("guidelineEnd: " + guidelineEnd);
            Console.WriteLine("A: " + A);
            Console.WriteLine("B: " + B);
            Console.WriteLine("barrier: " + barrier);
            Assert.AreEqual(root.Width, 250);
            Assert.AreEqual(guidelineStart.Left, 30);
            Assert.AreEqual(guidelineEnd.Left, 230);
            Assert.AreEqual(A.Left, 130);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(B.Left, 30);
            Assert.AreEqual(B.Width, 200);
            Assert.AreEqual(barrier.Left, 30);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void barrierImage()
        [Test]
        public virtual void barrierImage()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(200, 20);
            ConstraintWidget C = new ConstraintWidget(60, 60);
            Barrier barrier = new Barrier();

            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            C.setDebugSolverName(root.System, "C");
            barrier.setDebugSolverName(root.System, "Barrier");
            barrier.BarrierType = Barrier.RIGHT;

            barrier.add(A);
            barrier.add(B);

            root.add(A);
            root.add(B);
            root.add(C);
            root.add(barrier);

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP);

            B.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);

            A.VerticalChainStyle = ConstraintWidget.CHAIN_SPREAD_INSIDE;

            C.HorizontalBiasPercent = 1;
            C.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            C.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            C.connect(ConstraintAnchor.Type.LEFT, barrier, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);

            root.OptimizationLevel = Optimizer.OPTIMIZATION_STANDARD;
            root.layout();
            Console.WriteLine("A: " + A + " B: " + B + " C: " + C + " barrier: " + barrier);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(A.Top, 0);
            Assert.AreEqual(B.Left, 0);
            Assert.AreEqual(B.Top, 580);
            Assert.AreEqual(C.Left, 740);
            Assert.AreEqual(C.Top, 270);
            Assert.AreEqual(barrier.Left, 200);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void barrierTooStrong()
        [Test]
        public virtual void barrierTooStrong()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(60, 60);
            ConstraintWidget B = new ConstraintWidget(100, 200);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            Barrier barrier = new Barrier();

            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            C.setDebugSolverName(root.System, "C");
            barrier.setDebugSolverName(root.System, "Barrier");
            barrier.BarrierType = Barrier.BOTTOM;

            barrier.add(B);

            root.add(A);
            root.add(B);
            root.add(C);
            root.add(barrier);

            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);

            B.connect(ConstraintAnchor.Type.TOP, C, ConstraintAnchor.Type.BOTTOM);
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_PARENT;

            C.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_PARENT;
            C.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            C.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            C.connect(ConstraintAnchor.Type.BOTTOM, A, ConstraintAnchor.Type.BOTTOM);

            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            root.layout();
            Console.WriteLine("A: " + A + " B: " + B + " C: " + C + " barrier: " + barrier);
            Assert.AreEqual(A.Left, 740);
            Assert.AreEqual(A.Top, 0);
            Assert.AreEqual(B.Left, 0);
            Assert.AreEqual(B.Top, 60);
            Assert.AreEqual(B.Width, 800);
            Assert.AreEqual(B.Height, 200);
            Assert.AreEqual(C.Left, 0);
            Assert.AreEqual(C.Top, 0);
            Assert.AreEqual(C.Width, 800);
            Assert.AreEqual(C.Height, 60);
            Assert.AreEqual(barrier.Bottom, 260);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void barrierMax()
        [Test]
        public virtual void barrierMax()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(150, 20);
            Barrier barrier = new Barrier();

            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            barrier.setDebugSolverName(root.System, "Barrier");

            barrier.add(A);

            root.add(A);
            root.add(barrier);
            root.add(B);

            barrier.BarrierType = Barrier.RIGHT;

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.LEFT, barrier, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            B.HorizontalBiasPercent = 0;
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_SPREAD, 0, 150, 1);
            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            root.layout();

            Console.WriteLine("A: " + A + " B: " + B + " barrier: " + barrier);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(barrier.Left, 100);
            Assert.AreEqual(B.Left, 100);
            Assert.AreEqual(B.Width, 150);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void barrierCenter()
        [Test]
        public virtual void barrierCenter()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            Barrier barrier = new Barrier();

            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            barrier.setDebugSolverName(root.System, "Barrier");

            barrier.add(A);

            root.add(A);
            root.add(barrier);

            barrier.BarrierType = Barrier.RIGHT;

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 10);
            A.connect(ConstraintAnchor.Type.RIGHT, barrier, ConstraintAnchor.Type.RIGHT, 30);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            root.layout();

            Console.WriteLine("A: " + A + " barrier: " + barrier);
            Assert.AreEqual(A.Left, 10);
            Assert.AreEqual(barrier.Left, 140);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void barrierCenter2()
        [Test]
        public virtual void barrierCenter2()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            Barrier barrier = new Barrier();

            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            barrier.setDebugSolverName(root.System, "Barrier");

            barrier.add(A);

            root.add(A);
            root.add(barrier);

            barrier.BarrierType = Barrier.LEFT;

            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 10);
            A.connect(ConstraintAnchor.Type.LEFT, barrier, ConstraintAnchor.Type.LEFT, 30);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            root.layout();

            Console.WriteLine("A: " + A + " barrier: " + barrier);
            Assert.AreEqual(A.Right, root.Width - 10);
            Assert.AreEqual(barrier.Left, A.Left - 30);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void barrierCenter3()
        [Test]
        public virtual void barrierCenter3()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            Barrier barrier = new Barrier();

            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            barrier.setDebugSolverName(root.System, "Barrier");

            barrier.add(A);
            barrier.add(B);

            root.add(A);
            root.add(B);
            root.add(barrier);

            barrier.BarrierType = Barrier.LEFT;

            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);

            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
            A.Width = 100;
            B.Width = 200;
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_WRAP, 0, 0, 0);
            A.HorizontalBiasPercent = 1;
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_WRAP, 0, 0, 0);
            B.HorizontalBiasPercent = 1;

            root.layout();

            Console.WriteLine("A: " + A + " B: " + B + " barrier: " + barrier);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(B.Width, 200);
            Assert.AreEqual(barrier.Left, B.Left);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void barrierCenter4()
        [Test]
        public virtual void barrierCenter4()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(150, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            Barrier barrier = new Barrier();

            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            barrier.setDebugSolverName(root.System, "Barrier");

            barrier.add(A);
            barrier.add(B);

            root.add(A);
            root.add(B);
            root.add(barrier);

            barrier.BarrierType = Barrier.LEFT;

            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.connect(ConstraintAnchor.Type.LEFT, barrier, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);

            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.LEFT, barrier, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
            A.HorizontalBiasPercent = 0;
            B.HorizontalBiasPercent = 0;

            root.layout();

            Console.WriteLine("A: " + A + " B: " + B + " barrier: " + barrier);
            Assert.AreEqual(A.Right, root.Width);
            Assert.AreEqual(barrier.Left, Math.Min(A.Left, B.Left));
            Assert.AreEqual(A.Left, barrier.Left);
            Assert.AreEqual(B.Left, barrier.Left);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void barrierCenter5()
        [Test]
        public virtual void barrierCenter5()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(150, 20);
            ConstraintWidget C = new ConstraintWidget(200, 20);
            Barrier barrier = new Barrier();

            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            C.setDebugSolverName(root.System, "C");
            barrier.setDebugSolverName(root.System, "Barrier");

            barrier.add(A);
            barrier.add(B);
            barrier.add(C);

            root.add(A);
            root.add(B);
            root.add(C);
            root.add(barrier);

            barrier.BarrierType = Barrier.RIGHT;

            A.connect(ConstraintAnchor.Type.RIGHT, barrier, ConstraintAnchor.Type.RIGHT);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);

            B.connect(ConstraintAnchor.Type.RIGHT, barrier, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);

            C.connect(ConstraintAnchor.Type.RIGHT, barrier, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.BOTTOM);

            A.HorizontalBiasPercent = 0;
            B.HorizontalBiasPercent = 0;
            C.HorizontalBiasPercent = 0;

            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            C.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_WRAP, 0, 0, 0);
            B.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_WRAP, 0, 0, 0);
            C.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_WRAP, 0, 0, 0);

            root.layout();

            Console.WriteLine("A: " + A + " B: " + B + " C: " + C + " barrier: " + barrier);
            Assert.AreEqual(barrier.Right, Math.Max(Math.Max(A.Right, B.Right), C.Right));
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(B.Width, 150);
            Assert.AreEqual(C.Width, 200);
        }


        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void basic()
        [Test]
        public virtual void basic()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(150, 20);
            Barrier barrier = new Barrier();

            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            barrier.setDebugSolverName(root.System, "Barrier");

            root.add(A);
            root.add(B);
            root.add(barrier);

            barrier.BarrierType = Barrier.LEFT;

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 50);

            B.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM, 20);

            barrier.add(A);
            barrier.add(B);

            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            root.layout();

            Console.WriteLine("A: " + A + " B: " + B + " barrier: " + barrier);
            Assert.AreEqual(barrier.Left, B.Left);

            barrier.BarrierType = Barrier.RIGHT;
            root.layout();
            Console.WriteLine("A: " + A + " B: " + B + " barrier: " + barrier);
            Assert.AreEqual(barrier.Right, B.Right);

            barrier.BarrierType = Barrier.LEFT;
            B.Width = 10;
            root.layout();
            Console.WriteLine("A: " + A + " B: " + B + " barrier: " + barrier);
            Assert.AreEqual(barrier.Left, A.Left);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void basic2()
        [Test]
        public virtual void basic2()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(150, 20);
            Barrier barrier = new Barrier();

            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            barrier.setDebugSolverName(root.System, "Barrier");

            root.add(A);
            root.add(B);
            root.add(barrier);

            barrier.BarrierType = Barrier.BOTTOM;

            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.setVerticalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_WRAP, 0, 0, 0);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);

            B.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.TOP, barrier, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);

            barrier.add(A);

            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            root.layout();

            Console.WriteLine("A: " + A + " B: " + B + " barrier: " + barrier);
            Assert.AreEqual(barrier.Top, A.Bottom);
            float actual = barrier.Bottom + (root.Bottom - barrier.Bottom - B.Height) / 2f;
            Assert.AreEqual((float)B.Top, actual, 1f);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void basic3()
        [Test]
        public virtual void basic3()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(150, 20);
            Barrier barrier = new Barrier();

            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            barrier.setDebugSolverName(root.System, "Barrier");

            root.add(A);
            root.add(B);
            root.add(barrier);

            barrier.BarrierType = Barrier.RIGHT;

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);

            B.connect(ConstraintAnchor.Type.LEFT, barrier, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);

            barrier.add(A);

            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();

            Console.WriteLine("root: " + root + " A: " + A + " B: " + B + " barrier: " + barrier);
            Assert.AreEqual(barrier.Right, A.Right);
            Assert.AreEqual(root.Width, A.Width + B.Width);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void basic4()
        [Test]
        public virtual void basic4()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            Barrier barrier = new Barrier();

            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            C.setDebugSolverName(root.System, "C");
            barrier.setDebugSolverName(root.System, "Barrier");

            root.add(A);
            root.add(B);
            root.add(C);
            root.add(barrier);

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);

            B.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
            B.Visibility = ConstraintWidget.GONE;

            C.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.TOP, barrier, ConstraintAnchor.Type.TOP);

            barrier.add(A);
            barrier.add(B);

            barrier.BarrierType = Barrier.BOTTOM;

            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            root.layout();

            Console.WriteLine("root: " + root + " A: " + A + " B: " + B + " C: " + C + " barrier: " + barrier);
            Assert.AreEqual(B.Top, A.Bottom);
            Assert.AreEqual(barrier.Top, B.Bottom);
            Assert.AreEqual(C.Top, barrier.Top);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void growArray()
        [Test]
        public virtual void growArray()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(150, 20);
            ConstraintWidget C = new ConstraintWidget(175, 20);
            ConstraintWidget D = new ConstraintWidget(200, 20);
            ConstraintWidget E = new ConstraintWidget(125, 20);
            Barrier barrier = new Barrier();

            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            C.setDebugSolverName(root.System, "C");
            D.setDebugSolverName(root.System, "D");
            E.setDebugSolverName(root.System, "E");
            barrier.setDebugSolverName(root.System, "Barrier");

            root.add(A);
            root.add(B);
            root.add(C);
            root.add(D);
            root.add(E);
            root.add(barrier);

            barrier.BarrierType = Barrier.LEFT;

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 50);

            B.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM, 20);

            C.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.BOTTOM, 20);

            D.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            D.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            D.connect(ConstraintAnchor.Type.TOP, C, ConstraintAnchor.Type.BOTTOM, 20);


            E.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            E.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            E.connect(ConstraintAnchor.Type.TOP, D, ConstraintAnchor.Type.BOTTOM, 20);

            barrier.add(A);
            barrier.add(B);
            barrier.add(C);
            barrier.add(D);
            barrier.add(E);

            root.layout();

            Console.WriteLine("A: " + A + " B: " + B + " C: " + C + " D: " + D + " E: " + E + " barrier: " + barrier);
            Assert.AreEqual(A.Left, (root.Width - A.Width) / 2, 1);
            Assert.AreEqual(B.Left, (root.Width - B.Width) / 2, 1);
            Assert.AreEqual(C.Left, (root.Width - C.Width) / 2, 1);
            Assert.AreEqual(D.Left, (root.Width - D.Width) / 2, 1);
            Assert.AreEqual(E.Left, (root.Width - E.Width) / 2, 1);
            Assert.AreEqual(barrier.Left, D.Left);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void connection()
        [Test]
        public virtual void connection()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(150, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            Barrier barrier = new Barrier();

            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            C.setDebugSolverName(root.System, "C");
            barrier.setDebugSolverName(root.System, "Barrier");

            root.add(A);
            root.add(B);
            root.add(C);
            root.add(barrier);

            barrier.BarrierType = Barrier.LEFT;

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 50);

            B.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM, 20);

            C.connect(ConstraintAnchor.Type.LEFT, barrier, ConstraintAnchor.Type.LEFT, 0);
            barrier.add(A);
            barrier.add(B);

            root.layout();

            Console.WriteLine("A: " + A + " B: " + B + " C: " + C + " barrier: " + barrier);
            Assert.AreEqual(barrier.Left, B.Left);
            Assert.AreEqual(C.Left, barrier.Left);

        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void withGuideline()
        [Test]
        public virtual void withGuideline()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            Barrier barrier = new Barrier();
            Guideline guideline = new Guideline();

            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            barrier.setDebugSolverName(root.System, "Barrier");
            guideline.setDebugSolverName(root.System, "Guideline");

            guideline.Orientation = ConstraintWidget.VERTICAL;
            guideline.GuideBegin = 200;
            barrier.BarrierType = Barrier.RIGHT;

            root.add(A);
            root.add(barrier);
            root.add(guideline);

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 10);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 50);

            barrier.add(A);
            barrier.add(guideline);

            root.layout();

            Console.WriteLine("A: " + A + " guideline: " + guideline + " barrier: " + barrier);
            Assert.AreEqual(barrier.Left, guideline.Left);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void wrapIssue()
        [Test]
        public virtual void wrapIssue()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            Barrier barrier = new Barrier();
            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");
            barrier.setDebugSolverName(root.System, "Barrier");
            barrier.BarrierType = Barrier.BOTTOM;

            root.add(A);
            root.add(B);
            root.add(barrier);

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 0);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 0);

            barrier.add(A);

            B.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 0);
            B.connect(ConstraintAnchor.Type.TOP, barrier, ConstraintAnchor.Type.BOTTOM, 0);
            B.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, 0);

            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();
            Console.WriteLine("1/ root: " + root + " A: " + A + " B: " + B + " barrier: " + barrier);

            Assert.AreEqual(barrier.Top, A.Bottom);
            Assert.AreEqual(B.Top, barrier.Bottom);
            Assert.AreEqual(root.Height, A.Height + B.Height);

            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.setVerticalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_WRAP, 0, 0, 0);

            root.layout();
            Console.WriteLine("2/ root: " + root + " A: " + A + " B: " + B + " barrier: " + barrier);

            Assert.AreEqual(barrier.Top, A.Bottom);
            Assert.AreEqual(B.Top, barrier.Bottom);
            Assert.AreEqual(root.Height, A.Height + B.Height);
        }
    }

}
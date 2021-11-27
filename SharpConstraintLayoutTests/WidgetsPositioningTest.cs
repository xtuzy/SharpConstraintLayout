using System;
using System.Collections.Generic;
using System.Threading;

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
    using NUnit.Framework;

    //using Before = org.junit.Before;
    //using Test = org.junit.Test;

    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.junit.Assert.Assert.AreEqual;
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.junit.Assert.assertTrue;
    [TestFixture]
    public class WidgetsPositioningTest
    {

        internal LinearSystem s = new LinearSystem();
        internal bool optimize = false;

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Before public void setUp()
        [SetUp]
        public virtual void setUp()
        {
            s = new LinearSystem();
            LinearEquation.resetNaming();
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testCentering()
        [Test]
        public virtual void testCentering()
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidget B = new ConstraintWidget(20, 100);
            ConstraintWidget B = new ConstraintWidget(20, 100);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidget C = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 200);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.TOP, 0);
            B.connect(ConstraintAnchor.Type.BOTTOM, A, ConstraintAnchor.Type.BOTTOM, 0);
            C.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.TOP, 0);
            C.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.BOTTOM, 0);
            root.add(A);
            root.add(B);
            root.add(C);
            root.layout();
            Console.WriteLine("A: " + A + " B: " + B + " C: " + C);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testDimensionRatio()
        [Test]
        public virtual void testDimensionRatio()
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidget A = new ConstraintWidget(0, 0, 600, 600);
            ConstraintWidget A = new ConstraintWidget(0, 0, 600, 600);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidget B = new ConstraintWidget(100, 100);
            ConstraintWidget B = new ConstraintWidget(100, 100);
            List<ConstraintWidget> widgets = new List<ConstraintWidget>();
            widgets.Add(A);
            widgets.Add(B);
            const int margin = 10;
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.LEFT, margin);
            B.connect(ConstraintAnchor.Type.RIGHT, A, ConstraintAnchor.Type.RIGHT, margin);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.TOP, margin);
            B.connect(ConstraintAnchor.Type.BOTTOM, A, ConstraintAnchor.Type.BOTTOM, margin);
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.DebugName = "A";
            B.DebugName = "B";
            const float ratio = 0.3f;
            // First, let's check vertical ratio
            B.setDimensionRatio(ratio, ConstraintWidget.VERTICAL);
            runTestOnWidgets(widgets, () =>
        {
            Console.WriteLine("a) A: " + A + " B: " + B);
            Assert.AreEqual(B.Width, A.Width - 2 * margin);
            Assert.AreEqual(B.Height, (int)(ratio * B.Width));
            Assert.AreEqual(B.Top - A.Top, (int)((A.Height - B.Height) / 2));
            Assert.AreEqual(A.Bottom - B.Bottom, (int)((A.Height - B.Height) / 2));
            Assert.AreEqual(B.Top - A.Top, A.Bottom - B.Bottom);
        });
            B.VerticalBiasPercent = 1;
            runTestOnWidgets(widgets, () =>
        {
            Console.WriteLine("b) A: " + A + " B: " + B);
            Assert.AreEqual(B.Width, A.Width - 2 * margin);
            Assert.AreEqual(B.Height, (int)(ratio * B.Width));
            Assert.AreEqual(B.Top, A.Height - B.Height - margin);
            Assert.AreEqual(A.Bottom, B.Bottom + margin);
        });
            B.VerticalBiasPercent = 0;
            runTestOnWidgets(widgets, () =>
        {
            Console.WriteLine("c) A: " + A + " B: " + B);
            Assert.AreEqual(B.Width, A.Width - 2 * margin);
            Assert.AreEqual(B.Height, (int)(ratio * B.Width));
            Assert.AreEqual(B.Top, A.Top + margin);
            Assert.AreEqual(B.Bottom, A.Top + B.Height + margin);
        });
            // Then, let's check horizontal ratio
            B.setDimensionRatio(ratio, ConstraintWidget.HORIZONTAL);
            runTestOnWidgets(widgets, () =>
        {
            Console.WriteLine("d) A: " + A + " B: " + B);
            Assert.AreEqual(B.Height, A.Height - 2 * margin);
            Assert.AreEqual(B.Width, (int)(ratio * B.Height));
            Assert.AreEqual(B.Left - A.Left, (int)((A.Width - B.Width) / 2));
            Assert.AreEqual(A.Right - B.Right, (int)((A.Width - B.Width) / 2));
        });
            B.HorizontalBiasPercent = 1;
            runTestOnWidgets(widgets, () =>
        {
            Console.WriteLine("e) A: " + A + " B: " + B);
            Assert.AreEqual(B.Height, A.Height - 2 * margin);
            Assert.AreEqual(B.Width, (int)(ratio * B.Height));
            Assert.AreEqual(B.Right, A.Right - margin);
            Assert.AreEqual(B.Left, A.Right - B.Width - margin);
        });
            B.HorizontalBiasPercent = 0;
            runTestOnWidgets(widgets, () =>
        {
            Console.WriteLine("f) A: " + A + " B: " + B);
            Assert.AreEqual(B.Height, A.Height - 2 * margin);
            Assert.AreEqual(B.Width, (int)(ratio * B.Height));
            Assert.AreEqual(B.Right, A.Left + margin + B.Width);
            Assert.AreEqual(B.Left, A.Left + margin);
        });
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testCreateManyVariables()
        [Test]
        public virtual void testCreateManyVariables()
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidgetContainer rootWidget = new ConstraintWidgetContainer(0, 0, 600, 400);
            ConstraintWidgetContainer rootWidget = new ConstraintWidgetContainer(0, 0, 600, 400);
            ConstraintWidget previous = new ConstraintWidget(0, 0, 100, 20);
            rootWidget.add(previous);
            for (int i = 0; i < 100; i++)
            {
                ConstraintWidget w = new ConstraintWidget(0, 0, 100, 20);
                w.connect(ConstraintAnchor.Type.LEFT, previous, ConstraintAnchor.Type.RIGHT, 20);
                w.connect(ConstraintAnchor.Type.RIGHT, rootWidget, ConstraintAnchor.Type.RIGHT, 20);
                rootWidget.add(w);
            }
            rootWidget.layout();
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testWidgetCenterPositioning()
        [Test]
        public virtual void testWidgetCenterPositioning()
        {
            const int x = 20;
            const int y = 30;
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidget rootWidget = new ConstraintWidget(x, y, 600, 400);
            ConstraintWidget rootWidget = new ConstraintWidget(x, y, 600, 400);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidget centeredWidget = new ConstraintWidget(100, 20);
            ConstraintWidget centeredWidget = new ConstraintWidget(100, 20);
            List<ConstraintWidget> widgets = new List<ConstraintWidget>();
            centeredWidget.resetSolverVariables(s.Cache);
            rootWidget.resetSolverVariables(s.Cache);
            widgets.Add(centeredWidget);
            widgets.Add(rootWidget);

            centeredWidget.DebugName = "A";
            rootWidget.DebugName = "Root";
            centeredWidget.connect(ConstraintAnchor.Type.CENTER_X, rootWidget, ConstraintAnchor.Type.CENTER_X);
            centeredWidget.connect(ConstraintAnchor.Type.CENTER_Y, rootWidget, ConstraintAnchor.Type.CENTER_Y);

            runTestOnWidgets(widgets, () =>
        {
            Console.WriteLine("\n*** rootWidget: " + rootWidget + " centeredWidget: " + centeredWidget);
            int left = centeredWidget.Left;
            int top = centeredWidget.Top;
            int right = centeredWidget.Right;
            int bottom = centeredWidget.Bottom;
            Assert.AreEqual(left, x + 250);
            Assert.AreEqual(right, x + 350);
            Assert.AreEqual(top, y + 190);
            Assert.AreEqual(bottom, y + 210);
        });
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testBaselinePositioning()
        [Test]
        public virtual void testBaselinePositioning()
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidget A = new ConstraintWidget(20, 230, 200, 70);
            ConstraintWidget A = new ConstraintWidget(20, 230, 200, 70);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidget B = new ConstraintWidget(200, 60, 200, 100);
            ConstraintWidget B = new ConstraintWidget(200, 60, 200, 100);
            List<ConstraintWidget> widgets = new List<ConstraintWidget>();
            widgets.Add(A);
            widgets.Add(B);
            A.DebugName = "A";
            B.DebugName = "B";
            A.BaselineDistance = 40;
            B.BaselineDistance = 60;
            B.connect(ConstraintAnchor.Type.BASELINE, A, ConstraintAnchor.Type.BASELINE);
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 1000);
            root.DebugName = "root";
            root.add(A);
            root.add(B);
            root.layout();
            Assert.AreEqual(B.Top + B.BaselineDistance, A.Top + A.BaselineDistance);
            runTestOnWidgets(widgets, () =>
        {
            Assert.AreEqual(B.Top + B.BaselineDistance, A.Top + A.BaselineDistance);
        });
        }

        //@Test
        public virtual void testAddingWidgets()
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 1000);
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 1000);
            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            //List<ConstraintWidget> widgetsA = new ArrayList();
            List<ConstraintWidget> widgetsA = new List<ConstraintWidget>();
            //List<ConstraintWidget> widgetsB = new ArrayList();
            List<ConstraintWidget> widgetsB = new List<ConstraintWidget>();
            for (int i = 0; i < 1000; i++)
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final ConstraintWidget A = new ConstraintWidget(0, 0, 200, 20);
                ConstraintWidget A = new ConstraintWidget(0, 0, 200, 20);
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final ConstraintWidget B = new ConstraintWidget(0, 0, 200, 20);
                ConstraintWidget B = new ConstraintWidget(0, 0, 200, 20);
                A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
                A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
                A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
                B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
                B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
                B.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
                widgetsA.Add(A);
                widgetsB.Add(B);
                root.add(A);
                root.add(B);
            }
            root.layout();
            foreach (ConstraintWidget widget in widgetsA)
            {
                Assert.AreEqual(widget.Left, 200);
                Assert.AreEqual(widget.Top, 0);
            }
            foreach (ConstraintWidget widget in widgetsB)
            {
                Assert.AreEqual(widget.Left, 600);
                Assert.AreEqual(widget.Top, 980);
            }
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testWidgetTopRightPositioning()
        [Test]
        public virtual void testWidgetTopRightPositioning()
        {
            // Easy to tweak numbers to test larger systems
            int numLoops = 10;
            int numWidgets = 100;

            for (int j = 0; j < numLoops; j++)
            {
                s.reset();
                //List<ConstraintWidget> widgets = new ArrayList();
                List<ConstraintWidget> widgets = new List<ConstraintWidget>();
                int w = 100 + j;
                int h = 20 + j;
                ConstraintWidget first = new ConstraintWidget(w, h);
                widgets.Add(first);
                ConstraintWidget previous = first;
                int margin = 20;
                for (int i = 0; i < numWidgets; i++)
                {
                    ConstraintWidget widget = new ConstraintWidget(w, h);
                    widget.connect(ConstraintAnchor.Type.LEFT, previous, ConstraintAnchor.Type.RIGHT, margin);
                    widget.connect(ConstraintAnchor.Type.TOP, previous, ConstraintAnchor.Type.BOTTOM, margin);
                    widgets.Add(widget);
                    previous = widget;
                }
                foreach (ConstraintWidget widget in widgets)
                {
                    widget.addToSolver(s, optimize);
                }
                try
                {
                    s.minimize();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.Write(e.StackTrace);
                }
                for (int i = 0; i < widgets.Count; i++)
                {
                    ConstraintWidget widget = widgets[i];
                    widget.updateFromSolver(s, optimize);
                    int left = widget.Left;
                    int top = widget.Top;
                    int right = widget.Right;
                    int bottom = widget.Bottom;
                    Assert.AreEqual(left, i * (w + margin));
                    Assert.AreEqual(right, i * (w + margin) + w);
                    Assert.AreEqual(top, i * (h + margin));
                    Assert.AreEqual(bottom, i * (h + margin) + h);
                }
            }
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testWrapSimpleWrapContent()
        [Test]
        public virtual void testWrapSimpleWrapContent()
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 1000);
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 1000);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidget A = new ConstraintWidget(0, 0, 200, 20);
            ConstraintWidget A = new ConstraintWidget(0, 0, 200, 20);
            List<ConstraintWidget> widgets = new List<ConstraintWidget>();
            widgets.Add(root);
            widgets.Add(A);

            root.setDebugSolverName(s, "root");
            A.setDebugSolverName(s, "A");

            root.add(A);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;

            runTestOnWidgets(widgets, () =>
        {
            Console.WriteLine("Simple Wrap: " + root + ", " + A);
            Assert.AreEqual(root.Width, A.Width);
            Assert.AreEqual(root.Height, A.Height);
            Assert.AreEqual(A.Width, 200);
            Assert.AreEqual(A.Height, 20);
        });
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testMatchConstraint()
        [Test]
        public virtual void testMatchConstraint()
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidgetContainer root = new ConstraintWidgetContainer(50, 50, 500, 500);
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(50, 50, 500, 500);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidget A = new ConstraintWidget(10, 20, 100, 30);
            ConstraintWidget A = new ConstraintWidget(10, 20, 100, 30);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidget B = new ConstraintWidget(150, 200, 100, 30);
            ConstraintWidget B = new ConstraintWidget(150, 200, 100, 30);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidget C = new ConstraintWidget(50, 50);
            ConstraintWidget C = new ConstraintWidget(50, 50);
            List<ConstraintWidget> widgets = new List<ConstraintWidget>();
            A.DebugName = "A";
            B.DebugName = "B";
            C.DebugName = "C";
            root.DebugName = "root";
            root.add(A);
            root.add(B);
            root.add(C);
            widgets.Add(root);
            widgets.Add(A);
            widgets.Add(B);
            widgets.Add(C);

            C.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            C.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            C.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
            C.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP);
            runTestOnWidgets(widgets, () =>
        {
            Assert.AreEqual(C.X, A.Right);
            Assert.AreEqual(C.Right, B.X);
            Assert.AreEqual(C.Y, A.Bottom);
            Assert.AreEqual(C.Bottom, B.Y);
        });
        }

        // Obsolete @Test
        public virtual void testWidgetStrengthPositioning()
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidget root = new ConstraintWidget(400, 400);
            ConstraintWidget root = new ConstraintWidget(400, 400);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidget A = new ConstraintWidget(20, 20);
            ConstraintWidget A = new ConstraintWidget(20, 20);
            List<ConstraintWidget> widgets = new List<ConstraintWidget>();
            widgets.Add(root);
            widgets.Add(A);

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            Console.WriteLine("Widget A centered inside Root");
            runTestOnWidgets(widgets, () =>
        {
            Assert.AreEqual(A.Left, 190);
            Assert.AreEqual(A.Right, 210);
            Assert.AreEqual(A.Top, 190);
            Assert.AreEqual(A.Bottom, 210);
        });
            Console.WriteLine("Widget A weak left, should move to the right");
            A.getAnchor(ConstraintAnchor.Type.LEFT); //.setStrength(ConstraintAnchor.Strength.WEAK);
            runTestOnWidgets(widgets, () =>
        {
            Assert.AreEqual(A.Left, 380);
            Assert.AreEqual(A.Right, 400);
        });
            Console.WriteLine("Widget A weak right, should go back to center");
            A.getAnchor(ConstraintAnchor.Type.RIGHT); //.setStrength(ConstraintAnchor.Strength.WEAK);
            runTestOnWidgets(widgets, () =>
        {
            Assert.AreEqual(A.Left, 190);
            Assert.AreEqual(A.Right, 210);
        });
            Console.WriteLine("Widget A strong left, should move to the left");
            A.getAnchor(ConstraintAnchor.Type.LEFT); //.setStrength(ConstraintAnchor.Strength.STRONG);
            runTestOnWidgets(widgets, () =>
        {
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(A.Right, 20);
            Assert.AreEqual(root.Width, 400);
        });
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testWidgetPositionMove()
        [Test]
        public virtual void testWidgetPositionMove()
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidget A = new ConstraintWidget(0, 0, 100, 20);
            ConstraintWidget A = new ConstraintWidget(0, 0, 100, 20);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidget B = new ConstraintWidget(0, 30, 200, 20);
            ConstraintWidget B = new ConstraintWidget(0, 30, 200, 20);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidget C = new ConstraintWidget(0, 60, 100, 20);
            ConstraintWidget C = new ConstraintWidget(0, 60, 100, 20);
            List<ConstraintWidget> widgets = new List<ConstraintWidget>();
            widgets.Add(A);
            widgets.Add(B);
            widgets.Add(C);
            A.setDebugSolverName(s, "A");
            B.setDebugSolverName(s, "B");
            C.setDebugSolverName(s, "C");

            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            C.setOrigin(200, 0);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.RIGHT);

            //ThreadStart check = () =>
            Action check = () =>
            {
                Assert.AreEqual(A.Width, 100);
                Assert.AreEqual(B.Width, 200);
                Assert.AreEqual(C.Width, 100);
            };
            runTestOnWidgets(widgets, check);
            Console.WriteLine("A: " + A + " B: " + B + " C: " + C);
            C.setOrigin(100, 0);
            //        runTestOnUIWidgets(widgets);
            runTestOnWidgets(widgets, check);
            Console.WriteLine("A: " + A + " B: " + B + " C: " + C);
            C.setOrigin(50, 0);
            runTestOnWidgets(widgets, check);
            Console.WriteLine("A: " + A + " B: " + B + " C: " + C);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testWrapProblem()
        [Test]
        public virtual void testWrapProblem()
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidgetContainer root = new ConstraintWidgetContainer(400, 400);
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(400, 400);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidget A = new ConstraintWidget(80, 300);
            ConstraintWidget A = new ConstraintWidget(80, 300);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidget B = new ConstraintWidget(250, 80);
            ConstraintWidget B = new ConstraintWidget(250, 80);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final java.util.ArrayList<ConstraintWidget> widgets = new java.util.ArrayList<ConstraintWidget>();
            List<ConstraintWidget> widgets = new List<ConstraintWidget>();
            widgets.Add(root);
            widgets.Add(B);
            widgets.Add(A);
            A.Parent = root;
            B.Parent = root;
            root.setDebugSolverName(s, "root");
            A.setDebugSolverName(s, "A");
            B.setDebugSolverName(s, "B");

            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            //        B.getAnchor(ConstraintAnchor.Type.TOP).setStrength(ConstraintAnchor.Strength.WEAK);

            runTestOnWidgets(widgets, () =>
        {
            Assert.AreEqual(A.Width, 80);
            Assert.AreEqual(A.Height, 300);
            Assert.AreEqual(B.Width, 250);
            Assert.AreEqual(B.Height, 80);
            Assert.AreEqual(A.Y, 0);
            Assert.AreEqual(B.Y, 110);
        });
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testGuideline()
        [Test]
        public virtual void testGuideline()
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidgetContainer root = new ConstraintWidgetContainer(400, 400);
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(400, 400);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final Guideline guideline = new Guideline();
            Guideline guideline = new Guideline();
            root.add(guideline);
            root.add(A);
            guideline.setGuidePercent(0.50f);
            guideline.Orientation = Guideline.VERTICAL;
            root.DebugName = "root";
            A.DebugName = "A";
            guideline.DebugName = "guideline";

            List<ConstraintWidget> widgets = new List<ConstraintWidget>();
            widgets.Add(root);
            widgets.Add(A);
            widgets.Add(guideline);

            A.connect(ConstraintAnchor.Type.LEFT, guideline, ConstraintAnchor.Type.LEFT);
            ThreadStart check = () =>
        {
            Console.WriteLine("" + root + " " + A + " " + guideline);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(A.Height, 20);
            Assert.AreEqual(A.X, 200);
        };
            runTest(root, check);
            guideline.setGuidePercent(0);
            runTest(root, () =>
        {
            Console.WriteLine("" + root + " " + A + " " + guideline);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(A.Height, 20);
            Assert.AreEqual(A.X, 0);
        });
            guideline.GuideBegin = 150;
            runTest(root, () =>
        {
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(A.Height, 20);
            Assert.AreEqual(A.X, 150);
        });
            Console.WriteLine("" + root + " " + A + " " + guideline);
            guideline.GuideEnd = 150;
            runTest(root, () =>
        {
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(A.Height, 20);
            Assert.AreEqual(A.X, 250);
        });
            Console.WriteLine("" + root + " " + A + " " + guideline);
            guideline.Orientation = Guideline.HORIZONTAL;
            A.resetAnchors();
            A.connect(ConstraintAnchor.Type.TOP, guideline, ConstraintAnchor.Type.TOP);
            guideline.GuideBegin = 150;
            runTest(root, () =>
        {
            Console.WriteLine("" + root + " " + A + " " + guideline);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(A.Height, 20);
            Assert.AreEqual(A.Y, 150);
        });
            Console.WriteLine("" + root + " " + A + " " + guideline);
            A.resetAnchors();
            A.connect(ConstraintAnchor.Type.TOP, guideline, ConstraintAnchor.Type.BOTTOM);
            runTest(root, () =>
        {
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(A.Height, 20);
            Assert.AreEqual(A.Y, 150);
        });
            Console.WriteLine("" + root + " " + A + " " + guideline);
        }

        private void runTest(ConstraintWidgetContainer root, ThreadStart check)
        {
            root.layout();
            //check.run();
            check.Invoke();
        }


        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testGuidelinePosition()
        [Test]
        public virtual void testGuidelinePosition()
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidgetContainer root = new ConstraintWidgetContainer(800, 400);
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(800, 400);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final Guideline guideline = new Guideline();
            Guideline guideline = new Guideline();
            root.add(guideline);
            root.add(A);
            root.add(B);
            guideline.setGuidePercent(0.651f);
            guideline.Orientation = Guideline.VERTICAL;
            root.setDebugSolverName(s, "root");
            A.setDebugSolverName(s, "A");
            B.setDebugSolverName(s, "B");
            guideline.setDebugSolverName(s, "guideline");

            List<ConstraintWidget> widgets = new List<ConstraintWidget>();
            widgets.Add(root);
            widgets.Add(A);
            widgets.Add(B);
            widgets.Add(guideline);

            A.connect(ConstraintAnchor.Type.LEFT, guideline, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, guideline, ConstraintAnchor.Type.RIGHT);
            //ThreadStart check = () =>
            Action check = () =>
        {
            Console.WriteLine("" + root + " A: " + A + " " + " B: " + B + " guideline: " + guideline);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(A.Height, 20);
            Assert.AreEqual(A.X, 521);
            Assert.AreEqual(B.Right, 521);
        };
            runTestOnWidgets(widgets, check);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testWidgetInfeasiblePosition()
        [Test]
        public virtual void testWidgetInfeasiblePosition()
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            List<ConstraintWidget> widgets = new List<ConstraintWidget>();
            widgets.Add(B);
            widgets.Add(A);
            A.resetSolverVariables(s.Cache);
            B.resetSolverVariables(s.Cache);

            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.RIGHT, A, ConstraintAnchor.Type.LEFT);
            runTestOnWidgets(widgets, () =>
        {
            // TODO: this fail -- need to figure the best way to fix this.
            //                Assert.AreEqual(A.getWidth(), 100);
            //                Assert.AreEqual(B.getWidth(), 100);
        });
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testWidgetMultipleDependentPositioning()
        [Test]
        public virtual void testWidgetMultipleDependentPositioning()
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidget root = new ConstraintWidget(400, 400);
            ConstraintWidget root = new ConstraintWidget(400, 400);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            root.setDebugSolverName(s, "root");
            A.setDebugSolverName(s, "A");
            B.setDebugSolverName(s, "B");
            List<ConstraintWidget> widgets = new List<ConstraintWidget>();
            widgets.Add(root);
            widgets.Add(B);
            widgets.Add(A);

            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 10);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, 10);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);

            root.resetSolverVariables(s.Cache);
            A.resetSolverVariables(s.Cache);
            B.resetSolverVariables(s.Cache);

            runTestOnWidgets(widgets, () =>
        {
            Console.WriteLine("root: " + root + " A: " + A + " B: " + B);
            Assert.AreEqual(root.Height, 400);
            Assert.AreEqual(root.Height, 400);
            Assert.AreEqual(A.Height, 20);
            Assert.AreEqual(B.Height, 20);
            Assert.AreEqual(A.Top - root.Top, root.Bottom - A.Bottom);
            Assert.AreEqual(B.Top - A.Bottom, root.Bottom - B.Bottom);
        });
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testMinSize()
        [Test]
        public virtual void testMinSize()
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidgetContainer root = new ConstraintWidgetContainer(600, 400);
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(600, 400);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            root.DebugName = "root";
            A.DebugName = "A";
            root.add(A);
            root.OptimizationLevel = 0;
            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A);
            Assert.AreEqual(root.Width, 600);
            Assert.AreEqual(root.Height, 400);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(A.Height, 20);
            Assert.AreEqual(A.Left - root.Left, root.Right - A.Right);
            Assert.AreEqual(A.Top - root.Top, root.Bottom - A.Bottom);
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();
            Console.WriteLine("b) root: " + root + " A: " + A);
            Assert.AreEqual(root.Height, A.Height);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(A.Height, 20);
            Assert.AreEqual(A.Left - root.Left, root.Right - A.Right);
            Assert.AreEqual(A.Top - root.Top, root.Bottom - A.Bottom);
            root.MinHeight = 200;
            root.layout();
            Console.WriteLine("c) root: " + root + " A: " + A);
            Assert.AreEqual(root.Height, 200);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(A.Height, 20);
            Assert.AreEqual(A.Left - root.Left, root.Right - A.Right);
            Assert.AreEqual(A.Top - root.Top, root.Bottom - A.Bottom);
            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();
            Console.WriteLine("d) root: " + root + " A: " + A);
            Assert.AreEqual(root.Width, A.Width);
            Assert.AreEqual(root.Height, 200);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(A.Height, 20);
            Assert.AreEqual(A.Left - root.Left, root.Right - A.Right);
            Assert.AreEqual(A.Top - root.Top, root.Bottom - A.Bottom);
            root.MinWidth = 300;
            root.layout();
            Console.WriteLine("e) root: " + root + " A: " + A);
            Assert.AreEqual(root.Width, 300);
            Assert.AreEqual(root.Height, 200);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(A.Height, 20);
            Assert.AreEqual(A.Left - root.Left, root.Right - A.Right);
            Assert.AreEqual(A.Top - root.Top, root.Bottom - A.Bottom);
        }
        /*
        * Insert the widgets in all permutations
        * (to test that the insert order
        * doesn't impact the resolution)
        */

        //private void runTestOnWidgets(List<ConstraintWidget> widgets, ThreadStart check)
        private void runTestOnWidgets(List<ConstraintWidget> widgets, Action check)
        {
            List<int?> tail = new List<int?>();
            for (int i = 0; i < widgets.Count; i++)
            {
                tail.Add(i);
            }
            addToSolverWithPermutation(widgets, new List<int?>(), tail, check);
        }

        private void runTestOnUIWidgets(List<ConstraintWidget> widgets)
        {
            for (int i = 0; i < widgets.Count; i++)
            {
                ConstraintWidget widget = widgets[i];
                if (!string.ReferenceEquals(widget.DebugName, null))
                {
                    widget.setDebugSolverName(s, widget.DebugName);
                }
                widget.resetSolverVariables(s.Cache);
                widget.addToSolver(s, optimize);
            }
            try
            {
                s.minimize();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
            }
            for (int j = 0; j < widgets.Count; j++)
            {
                ConstraintWidget w = widgets[j];
                w.updateFromSolver(s, optimize);
                Console.WriteLine(" " + w);
            }
        }

        //private void addToSolverWithPermutation(List<ConstraintWidget> widgets, List<int?> list, List<int?> tail, ThreadStart check)
        private void addToSolverWithPermutation(List<ConstraintWidget> widgets, List<int?> list, List<int?> tail, Action check)
        {
            if (tail.Count > 0)
            {
                int n = tail.Count;
                for (int i = 0; i < n; i++)
                {
                    list.Add(tail[i]);
                    List<int?> permuted = new List<int?>(tail);
                    permuted.RemoveAt(i);
                    addToSolverWithPermutation(widgets, list, permuted, check);
                    //JAVA TO C# CONVERTER TODO TASK: The overload of the 'remove' method cannot be determined:
                    list.RemoveAt(list.Count - 1);
                }
            }
            else
            {
                //            System.out.print("Adding widgets in order: ");
                s.reset();
                for (int i = 0; i < list.Count; i++)
                {
                    int index = list[i].Value;
                    //                System.out.print(" " + index);
                    ConstraintWidget widget = widgets[index];
                    widget.resetSolverVariables(s.Cache);
                }
                for (int i = 0; i < list.Count; i++)
                {
                    int index = list[i].Value;
                    //                System.out.print(" " + index);
                    ConstraintWidget widget = widgets[index];
                    if (!string.ReferenceEquals(widget.DebugName, null))
                    {
                        widget.setDebugSolverName(s, widget.DebugName);
                    }
                    widget.addToSolver(s, optimize);
                }
                //            System.out.println("");
                //            s.displayReadableRows();
                try
                {
                    s.minimize();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.Write(e.StackTrace);
                }
                for (int j = 0; j < widgets.Count; j++)
                {
                    ConstraintWidget w = widgets[j];
                    w.updateFromSolver(s, optimize);
                }
                //            try {
                //check.run();
                check.Invoke();
                //            } catch (AssertionError e) {
                //                System.out.println("Assertion error: " + e);
                //                runTestOnUIWidgets(widgets);
                //            }
            }
        }

    }

}
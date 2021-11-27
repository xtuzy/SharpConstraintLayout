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
    using Guideline = androidx.constraintlayout.core.widgets.Guideline;
    //using Test = org.junit.Test;

    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.junit.Assert.Assert.AreEqual;

    /// <summary>
    /// Basic visibility behavior test in the solver
    /// </summary>
    /// 
    [TestFixture]
    public class VisibilityTest
    {

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testGoneSingleConnection()
        [Test]
        public virtual void testGoneSingleConnection()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");

            int margin = 175;
            int goneMargin = 42;
            root.add(A);
            root.add(B);

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, margin);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, margin);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT, margin);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM, margin);

            root.layout();
            Console.WriteLine("a) A: " + A + " B: " + B);
            Assert.AreEqual(root.Width, 800);
            Assert.AreEqual(root.Height, 600);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(A.Height, 20);
            Assert.AreEqual(B.Width, 100);
            Assert.AreEqual(B.Height, 20);
            Assert.AreEqual(A.Left, root.Left + margin);
            Assert.AreEqual(A.Top, root.Top + margin);
            Assert.AreEqual(B.Left, A.Right + margin);
            Assert.AreEqual(B.Top, A.Bottom + margin);

            A.Visibility = ConstraintWidget.GONE;
            root.layout();
            Console.WriteLine("b) A: " + A + " B: " + B);
            Assert.AreEqual(root.Width, 800);
            Assert.AreEqual(root.Height, 600);
            Assert.AreEqual(A.Width, 0);
            Assert.AreEqual(A.Height, 0);
            Assert.AreEqual(B.Width, 100);
            Assert.AreEqual(B.Height, 20);
            Assert.AreEqual(A.Left, root.Left);
            Assert.AreEqual(A.Top, root.Top);
            Assert.AreEqual(B.Left, A.Right + margin);
            Assert.AreEqual(B.Top, A.Bottom + margin);

            B.setGoneMargin(ConstraintAnchor.Type.LEFT, goneMargin);
            B.setGoneMargin(ConstraintAnchor.Type.TOP, goneMargin);

            root.layout();
            Console.WriteLine("c) A: " + A + " B: " + B);
            Assert.AreEqual(root.Width, 800);
            Assert.AreEqual(root.Height, 600);
            Assert.AreEqual(A.Width, 0);
            Assert.AreEqual(A.Height, 0);
            Assert.AreEqual(B.Width, 100);
            Assert.AreEqual(B.Height, 20);
            Assert.AreEqual(A.Left, root.Left);
            Assert.AreEqual(A.Top, root.Top);
            Assert.AreEqual(B.Left, A.Right + goneMargin);
            Assert.AreEqual(B.Top, A.Bottom + goneMargin);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testGoneDualConnection()
        [Test]
        public virtual void testGoneDualConnection()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            Guideline guideline = new Guideline();
            guideline.setGuidePercent(0.5f);
            guideline.Orientation = ConstraintWidget.HORIZONTAL;
            root.setDebugSolverName(root.System, "root");
            A.setDebugSolverName(root.System, "A");
            B.setDebugSolverName(root.System, "B");

            root.add(A);
            root.add(B);
            root.add(guideline);

            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, guideline, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);

            root.layout();
            Console.WriteLine("a) A: " + A + " B: " + B + " guideline " + guideline);
            Assert.AreEqual(root.Width, 800);
            Assert.AreEqual(root.Height, 600);
            Assert.AreEqual(A.Left, root.Left);
            Assert.AreEqual(A.Right, root.Right);
            Assert.AreEqual(B.Left, root.Left);
            Assert.AreEqual(B.Right, root.Right);
            Assert.AreEqual(guideline.Top, root.Height / 2);
            Assert.AreEqual(A.Top, root.Top);
            Assert.AreEqual(A.Bottom, guideline.Top);
            Assert.AreEqual(B.Top, A.Bottom);
            Assert.AreEqual(B.Bottom, root.Bottom);

            A.Visibility = ConstraintWidget.GONE;
            root.layout();
            Console.WriteLine("b) A: " + A + " B: " + B + " guideline " + guideline);
            Assert.AreEqual(root.Width, 800);
            Assert.AreEqual(root.Height, 600);
            Assert.AreEqual(A.Width, 0);
            Assert.AreEqual(A.Height, 0);
            Assert.AreEqual(A.Left, 400);
            Assert.AreEqual(A.Right, 400);
            Assert.AreEqual(B.Left, root.Left);
            Assert.AreEqual(B.Right, root.Right);
            Assert.AreEqual(guideline.Top, root.Height / 2);
            Assert.AreEqual(A.Top, 150);
            Assert.AreEqual(A.Bottom, 150);
            Assert.AreEqual(B.Top, 150);
            Assert.AreEqual(B.Bottom, root.Bottom);
        }
    }

}
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
    using ConstraintAnchor = androidx.constraintlayout.core.widgets.ConstraintAnchor;
    using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;
    using ConstraintWidgetContainer = androidx.constraintlayout.core.widgets.ConstraintWidgetContainer;

    //using Test = org.junit.Test;

    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.junit.Assert.Assert.AreEqual;
    [TestFixture]
    public class PriorityTest
    {

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testPriorityChainHorizontal()
        [Test]
        public virtual void testPriorityChainHorizontal()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 600);
            ConstraintWidget A = new ConstraintWidget(400, 20);
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
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);

            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT);

            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.RIGHT, A, ConstraintAnchor.Type.RIGHT);

            B.HorizontalChainStyle = ConstraintWidget.CHAIN_PACKED;
            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Width, 400);
            Assert.AreEqual(B.Width, 100);
            Assert.AreEqual(C.Width, 100);
            Assert.AreEqual(A.Left, 300);
            Assert.AreEqual(B.Left, 400);
            Assert.AreEqual(C.Left, 500);

            B.HorizontalChainStyle = ConstraintWidget.CHAIN_SPREAD;
            root.layout();
            Console.WriteLine("b) root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Width, 400);
            Assert.AreEqual(B.Width, 100);
            Assert.AreEqual(C.Width, 100);
            Assert.AreEqual(A.Left, 300);
            Assert.AreEqual(B.Left, 367);
            Assert.AreEqual(C.Left, 533);

            B.HorizontalChainStyle = ConstraintWidget.CHAIN_SPREAD_INSIDE;
            root.layout();
            Console.WriteLine("c) root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Width, 400);
            Assert.AreEqual(B.Width, 100);
            Assert.AreEqual(C.Width, 100);
            Assert.AreEqual(A.Left, 300);
            Assert.AreEqual(B.Left, 300);
            Assert.AreEqual(C.Left, 600);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testPriorityChainVertical()
        [Test]
        public virtual void testPriorityChainVertical()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 1000);
            ConstraintWidget A = new ConstraintWidget(400, 400);
            ConstraintWidget B = new ConstraintWidget(100, 100);
            ConstraintWidget C = new ConstraintWidget(100, 100);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            C.DebugName = "C";
            root.add(A);
            root.add(B);
            root.add(C);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);

            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.BOTTOM, C, ConstraintAnchor.Type.TOP);

            C.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.BOTTOM);
            C.connect(ConstraintAnchor.Type.BOTTOM, A, ConstraintAnchor.Type.BOTTOM);

            B.VerticalChainStyle = ConstraintWidget.CHAIN_PACKED;
            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Height, 400);
            Assert.AreEqual(B.Height, 100);
            Assert.AreEqual(C.Height, 100);
            Assert.AreEqual(A.Top, 300);
            Assert.AreEqual(B.Top, 400);
            Assert.AreEqual(C.Top, 500);

            B.VerticalChainStyle = ConstraintWidget.CHAIN_SPREAD;
            root.layout();
            Console.WriteLine("b) root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Height, 400);
            Assert.AreEqual(B.Height, 100);
            Assert.AreEqual(C.Height, 100);
            Assert.AreEqual(A.Top, 300);
            Assert.AreEqual(B.Top, 367);
            Assert.AreEqual(C.Top, 533);

            B.VerticalChainStyle = ConstraintWidget.CHAIN_SPREAD_INSIDE;
            root.layout();
            Console.WriteLine("c) root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Height, 400);
            Assert.AreEqual(B.Height, 100);
            Assert.AreEqual(C.Height, 100);
            Assert.AreEqual(A.Top, 300);
            Assert.AreEqual(B.Top, 300);
            Assert.AreEqual(C.Top, 600);
        }
    }


}
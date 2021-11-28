using NUnit.Framework;
using System;

/*
 * Copyright (C) 2017 The Android Open Source Project
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
    public class CenterWrapTest
    {

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testRatioCenter()
        [Test]
        public virtual void testRatioCenter()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            root.DebugName = "Root";
            A.DebugName = "A";
            B.DebugName = "B";
            root.add(A);
            root.add(B);

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.setDimensionRatio(0.3f, ConstraintWidget.VERTICAL);

            B.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.setDimensionRatio(1f, ConstraintWidget.VERTICAL);
            //        root.setVerticalDimensionBehaviour(ConstraintWidget.DimensionBehaviour.WRAP_CONTENT);
            root.OptimizationLevel = 0;
            root.layout();
            Console.WriteLine("root: " + root + " A: " + A +" B:"+B);//we can see conflict
            Assert.AreEqual(800 * 0.3, A.Height);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testSimpleWrap()
        [Test]
        public virtual void testSimpleWrap()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            root.DebugName = "Root";
            A.DebugName = "A";
            root.add(A);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.OptimizationLevel = 0;
            root.layout();
            Console.WriteLine("root: " + root + " A: " + A);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(A.Height, 20);
            Assert.AreEqual(root.Width, 100);
            Assert.AreEqual(root.Height, 20);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testSimpleWrap2()
        [Test]
        public virtual void testSimpleWrap2()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            root.DebugName = "Root";
            A.DebugName = "A";
            root.add(A);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.OptimizationLevel = 0;
            root.layout();
            Console.WriteLine("root: " + root + " A: " + A);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(A.Height, 20);
            Assert.AreEqual(root.Width, 100);
            Assert.AreEqual(root.Height, 20);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testWrap()
        [Test]
        public virtual void testWrap()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            root.DebugName = "Root";
            A.DebugName = "A";
            B.DebugName = "B";
            C.DebugName = "C";
            root.add(A);
            root.add(B);
            root.add(C);

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.TOP);
            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.BOTTOM);
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.OptimizationLevel = 0;
            root.layout();
            Console.WriteLine("root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(B.Width, 100);
            Assert.AreEqual(C.Width, 100);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(A.Height, 20);
            Assert.AreEqual(B.Height, 20);
            Assert.AreEqual(C.Height, 20);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testWrapHeight()
        [Test]
        public virtual void testWrapHeight()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget TL = new ConstraintWidget(100, 20);
            ConstraintWidget TRL = new ConstraintWidget(100, 20);
            ConstraintWidget TBL = new ConstraintWidget(100, 20);
            ConstraintWidget IMG = new ConstraintWidget(100, 100);

            root.DebugName = "root";
            TL.DebugName = "TL";
            TRL.DebugName = "TRL";
            TBL.DebugName = "TBL";
            IMG.DebugName = "IMG";

            // vertical

            TL.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            TL.connect(ConstraintAnchor.Type.BOTTOM, TBL, ConstraintAnchor.Type.BOTTOM);
            TRL.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            //TRL.connect(ConstraintAnchor.Type.BOTTOM, TBL, ConstraintAnchor.Type.TOP);
            TBL.connect(ConstraintAnchor.Type.TOP, TRL, ConstraintAnchor.Type.BOTTOM);

            IMG.connect(ConstraintAnchor.Type.TOP, TBL, ConstraintAnchor.Type.BOTTOM);
            IMG.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);

            root.add(TL);
            root.add(TRL);
            root.add(TBL);
            root.add(IMG);

            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();
            Console.WriteLine("a) root: " + root + " TL: " + TL + " TRL: " + TRL + " TBL: " + TBL + " IMG: " + IMG);
            Assert.AreEqual(root.Height, 140);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testComplexLayout()
        [Test]
        public virtual void testComplexLayout()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget IMG = new ConstraintWidget(100, 100);

            int margin = 16;

            ConstraintWidget BUTTON = new ConstraintWidget(50, 50);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);

            IMG.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            IMG.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            IMG.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            IMG.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;

            BUTTON.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, margin);
            BUTTON.connect(ConstraintAnchor.Type.TOP, IMG, ConstraintAnchor.Type.BOTTOM);
            BUTTON.connect(ConstraintAnchor.Type.BOTTOM, IMG, ConstraintAnchor.Type.BOTTOM);

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, margin);
            A.connect(ConstraintAnchor.Type.TOP, BUTTON, ConstraintAnchor.Type.BOTTOM, margin);

            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, margin);
            B.connect(ConstraintAnchor.Type.TOP, BUTTON, ConstraintAnchor.Type.BOTTOM, margin);

            C.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, margin);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, margin);
            C.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM, margin);
            C.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);

            root.add(IMG);
            root.add(BUTTON);
            root.add(A);
            root.add(B);
            root.add(C);

            root.DebugName = "root";
            IMG.DebugName = "IMG";
            BUTTON.DebugName = "BUTTON";
            A.DebugName = "A";
            B.DebugName = "B";
            C.DebugName = "C";

            root.layout();
            Console.WriteLine("a) root: " + root + " IMG: " + IMG + " BUTTON: " + BUTTON + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(root.Width, 800);
            Assert.AreEqual(root.Height, 600);
            Assert.AreEqual(IMG.Width, root.Width);
            Assert.AreEqual(BUTTON.Width, 50);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(B.Width, 100);
            Assert.AreEqual(C.Width, 100);
            Assert.AreEqual(IMG.Height, 100);
            Assert.AreEqual(BUTTON.Height, 50);
            Assert.AreEqual(A.Height, 20);
            Assert.AreEqual(B.Height, 20);
            Assert.AreEqual(C.Height, 20);
            Assert.AreEqual(IMG.Left, 0);
            Assert.AreEqual(IMG.Right, root.Right);
            Assert.AreEqual(BUTTON.Left, 734);
            Assert.AreEqual(BUTTON.Top, IMG.Bottom - BUTTON.Height / 2);
            Assert.AreEqual(A.Left, margin);
            Assert.AreEqual(A.Top, BUTTON.Bottom + margin);
            Assert.AreEqual(B.Right, root.Right - margin);
            Assert.AreEqual(B.Top, A.Top);
            Assert.AreEqual(C.Left, 350);
            Assert.AreEqual(C.Right, 450);
            Assert.AreEqual(C.Top, 379, 1);

            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();
            root.OptimizationLevel = 0;
            Console.WriteLine("b) root: " + root + " IMG: " + IMG + " BUTTON: " + BUTTON + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(root.Width, 800);
            Assert.AreEqual(root.Height, 197);
            Assert.AreEqual(IMG.Width, root.Width);
            Assert.AreEqual(BUTTON.Width, 50);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(B.Width, 100);
            Assert.AreEqual(C.Width, 100);
            Assert.AreEqual(IMG.Height, 100);
            Assert.AreEqual(BUTTON.Height, 50);
            Assert.AreEqual(A.Height, 20);
            Assert.AreEqual(B.Height, 20);
            Assert.AreEqual(C.Height, 20);
            Assert.AreEqual(IMG.Left, 0);
            Assert.AreEqual(IMG.Right, root.Right);
            Assert.AreEqual(BUTTON.Left, 734);
            Assert.AreEqual(BUTTON.Top, IMG.Bottom - BUTTON.Height / 2);
            Assert.AreEqual(A.Left, margin);
            Assert.AreEqual(A.Top, BUTTON.Bottom + margin);
            Assert.AreEqual(B.Right, root.Right - margin);
            Assert.AreEqual(B.Top, A.Top);
            Assert.AreEqual(C.Left, 350);
            Assert.AreEqual(C.Right, 450);
            Assert.AreEqual(C.Top, A.Bottom + margin);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testWrapCenter()
        [Test]
        public virtual void testWrapCenter()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
            ConstraintWidget TextBox = new ConstraintWidget(100, 50);
            ConstraintWidget TextBoxGone = new ConstraintWidget(100, 50);
            ConstraintWidget ValueBox = new ConstraintWidget(20, 20);

            root.DebugName = "root";
            TextBox.DebugName = "TextBox";
            TextBoxGone.DebugName = "TextBoxGone";
            ValueBox.DebugName = "ValueBox";

            // vertical

            TextBox.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            TextBox.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 10);
            TextBox.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 10);
            TextBox.connect(ConstraintAnchor.Type.RIGHT, ValueBox, ConstraintAnchor.Type.LEFT, 10);

            ValueBox.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 10);
            ValueBox.connect(ConstraintAnchor.Type.TOP, TextBox, ConstraintAnchor.Type.TOP);
            ValueBox.connect(ConstraintAnchor.Type.BOTTOM, TextBox, ConstraintAnchor.Type.BOTTOM);

            TextBoxGone.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            TextBoxGone.connect(ConstraintAnchor.Type.TOP, TextBox, ConstraintAnchor.Type.BOTTOM, 10);
            TextBoxGone.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 10);
            TextBoxGone.connect(ConstraintAnchor.Type.RIGHT, TextBox, ConstraintAnchor.Type.RIGHT);
            TextBoxGone.Visibility = ConstraintWidget.GONE;

            root.add(TextBox);
            root.add(ValueBox);
            root.add(TextBoxGone);

            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();
            Console.WriteLine("a) root: " + root + " TextBox: " + TextBox + " ValueBox: " + ValueBox + " TextBoxGone: " + TextBoxGone);
            Assert.AreEqual(ValueBox.Top, TextBox.Top + ((TextBox.Height - ValueBox.Height) / 2));
            Assert.AreEqual(root.Height, 60);
        }
    }

}
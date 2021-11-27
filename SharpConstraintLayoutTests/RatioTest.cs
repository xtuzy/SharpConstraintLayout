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
    using Optimizer = androidx.constraintlayout.core.widgets.Optimizer;
    //using Test = org.junit.Test;

    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.junit.Assert.Assert.AreEqual;
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.junit.Assert.assertTrue;
    [TestFixture]
    public class RatioTest
    {

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testWrapRatio()
        [Test]
        public virtual void testWrapRatio()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 700, 1920);
            ConstraintWidget A = new ConstraintWidget(231, 126);
            ConstraintWidget B = new ConstraintWidget(231, 126);
            ConstraintWidget C = new ConstraintWidget(231, 126);

            root.DebugName = "root";
            root.add(A);
            root.add(B);
            root.add(C);

            A.DebugName = "A";
            B.DebugName = "B";
            C.DebugName = "C";

            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.setDimensionRatio("1:1");
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP);
            A.HorizontalChainStyle = ConstraintWidget.CHAIN_PACKED;
            A.HorizontalBiasPercent = 0.3f;
            A.VerticalChainStyle = ConstraintWidget.CHAIN_SPREAD_INSIDE;

            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.LEFT, 171);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.BOTTOM, C, ConstraintAnchor.Type.TOP);


            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.BOTTOM);
            C.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);

            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();
            Console.WriteLine("root: " + root);
            Console.WriteLine("A: " + A);
            Console.WriteLine("B: " + B);
            Console.WriteLine("C: " + C);

            Assert.AreEqual(A.Left >= 0, true);
            Assert.AreEqual(A.Width, A.Height);
            Assert.AreEqual(A.Width, 402);
            Assert.AreEqual(root.Width, 402);
            Assert.AreEqual(root.Height, 654);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(B.Top, 402);
            Assert.AreEqual(B.Left, 171);
            Assert.AreEqual(C.Top, 528);
            Assert.AreEqual(C.Left, 171);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testGuidelineRatioChainWrap()
        [Test]
        public virtual void testGuidelineRatioChainWrap()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 700, 1920);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            Guideline guideline = new Guideline();
            guideline.Orientation = Guideline.HORIZONTAL;
            guideline.GuideBegin = 100;

            root.DebugName = "root";
            root.add(A);
            root.add(B);
            root.add(C);
            root.add(guideline);

            A.DebugName = "A";
            B.DebugName = "B";
            C.DebugName = "C";

            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.setDimensionRatio("1:1");
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, guideline, ConstraintAnchor.Type.TOP);

            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.setDimensionRatio("1:1");
            B.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.BOTTOM, C, ConstraintAnchor.Type.TOP);


            C.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            C.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            C.setDimensionRatio("1:1");
            C.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.BOTTOM);
            C.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);

            root.Height = 0;
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();
            Console.WriteLine("root: " + root);
            Console.WriteLine("A: " + A);
            Console.WriteLine("B: " + B);
            Console.WriteLine("C: " + C);

            Assert.AreEqual(root.Height, 1500);

            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(A.Height, 100);

            Assert.AreEqual(B.Width, 700);
            Assert.AreEqual(B.Height, 700);

            Assert.AreEqual(C.Width, 700);
            Assert.AreEqual(C.Height, 700);

            Assert.AreEqual(A.Top, 0);
            Assert.AreEqual(B.Top, A.Bottom);
            Assert.AreEqual(C.Top, B.Bottom);

            Assert.AreEqual(A.Left, 300);
            Assert.AreEqual(B.Left, 0);
            Assert.AreEqual(C.Left, 0);

            root.Width = 0;
            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();
            Console.WriteLine("root: " + root);
            Console.WriteLine("A: " + A);
            Console.WriteLine("B: " + B);
            Console.WriteLine("C: " + C);

            Assert.AreEqual(root.Width, 100);
            Assert.AreEqual(root.Height, 300);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(A.Height, 100);
            Assert.AreEqual(B.Width, 100);
            Assert.AreEqual(B.Height, 100);
            Assert.AreEqual(C.Width, 100);
            Assert.AreEqual(C.Height, 100);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testComplexRatioChainWrap()
        [Test]
        public virtual void testComplexRatioChainWrap()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 700, 1920);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            ConstraintWidget D = new ConstraintWidget(100, 40);
            ConstraintWidget X = new ConstraintWidget(100, 20);
            ConstraintWidget Y = new ConstraintWidget(100, 20);
            ConstraintWidget Z = new ConstraintWidget(100, 40);

            root.DebugName = "root";
            root.add(A);
            root.add(B);
            root.add(C);
            root.add(D);
            root.add(X);
            root.add(Y);
            root.add(Z);

            A.DebugName = "A";
            B.DebugName = "B";
            C.DebugName = "C";
            D.DebugName = "D";
            X.DebugName = "X";
            Y.DebugName = "Y";
            Z.DebugName = "Z";

            X.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            X.connect(ConstraintAnchor.Type.BOTTOM, Y, ConstraintAnchor.Type.TOP);
            X.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            X.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            X.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            X.Height = 40;

            Y.connect(ConstraintAnchor.Type.TOP, X, ConstraintAnchor.Type.BOTTOM);
            Y.connect(ConstraintAnchor.Type.BOTTOM, Z, ConstraintAnchor.Type.TOP);
            Y.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            Y.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            Y.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            Y.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            Y.setDimensionRatio("1:1");

            Z.connect(ConstraintAnchor.Type.TOP, Y, ConstraintAnchor.Type.BOTTOM);
            Z.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            Z.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            Z.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            Z.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            Z.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            Z.setDimensionRatio("1:1");

            root.Width = 700;
            root.Height = 0;
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();
            Console.WriteLine("root: " + root);
            Console.WriteLine("X: " + X);
            Console.WriteLine("Y: " + Y);
            Console.WriteLine("Z: " + Z);

            Assert.AreEqual(root.Width, 700);
            Assert.AreEqual(root.Height, 1440);

            Assert.AreEqual(X.Left, 0);
            Assert.AreEqual(X.Top, 0);
            Assert.AreEqual(X.Width, 700);
            Assert.AreEqual(X.Height, 40);

            Assert.AreEqual(Y.Left, 0);
            Assert.AreEqual(Y.Top, 40);
            Assert.AreEqual(Y.Width, 700);
            Assert.AreEqual(Y.Height, 700);

            Assert.AreEqual(Z.Left, 0);
            Assert.AreEqual(Z.Top, 740);
            Assert.AreEqual(Z.Width, 700);
            Assert.AreEqual(Z.Height, 700);

            A.connect(ConstraintAnchor.Type.TOP, X, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.LEFT, X, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.setDimensionRatio("1:1");

            B.connect(ConstraintAnchor.Type.TOP, X, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT);
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.setDimensionRatio("1:1");

            C.connect(ConstraintAnchor.Type.TOP, X, ConstraintAnchor.Type.TOP);
            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.RIGHT, D, ConstraintAnchor.Type.LEFT);
            C.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            C.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            C.setDimensionRatio("1:1");

            D.connect(ConstraintAnchor.Type.TOP, X, ConstraintAnchor.Type.TOP);
            D.connect(ConstraintAnchor.Type.LEFT, C, ConstraintAnchor.Type.RIGHT);
            D.connect(ConstraintAnchor.Type.RIGHT, X, ConstraintAnchor.Type.RIGHT);
            D.connect(ConstraintAnchor.Type.BOTTOM, X, ConstraintAnchor.Type.BOTTOM);
            D.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            D.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            D.setDimensionRatio("1:1");

            root.Height = 0;
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();
            Console.WriteLine("root: " + root);
            Console.WriteLine("X: " + X);
            Console.WriteLine("Y: " + Y);
            Console.WriteLine("Z: " + Z);

            Assert.AreEqual(root.Width, 700);
            Assert.AreEqual(root.Height, 1440);

            Assert.AreEqual(X.Left, 0);
            Assert.AreEqual(X.Top, 0);
            Assert.AreEqual(X.Width, 700);
            Assert.AreEqual(X.Height, 40);

            Assert.AreEqual(Y.Left, 0);
            Assert.AreEqual(Y.Top, 40);
            Assert.AreEqual(Y.Width, 700);
            Assert.AreEqual(Y.Height, 700);

            Assert.AreEqual(Z.Left, 0);
            Assert.AreEqual(Z.Top, 740);
            Assert.AreEqual(Z.Width, 700);
            Assert.AreEqual(Z.Height, 700);

        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testRatioChainWrap()
        [Test]
        public virtual void testRatioChainWrap()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 1000);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            ConstraintWidget D = new ConstraintWidget(100, 40);
            root.DebugName = "root";
            root.add(A);
            root.add(B);
            root.add(C);
            root.add(D);
            A.DebugName = "A";
            B.DebugName = "B";
            C.DebugName = "C";
            D.DebugName = "D";

            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;

            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;

            C.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            C.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;

            D.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            D.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;

            D.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            D.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            D.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);

            A.connect(ConstraintAnchor.Type.LEFT, D, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.TOP, D, ConstraintAnchor.Type.TOP);
            A.setDimensionRatio("1:1");

            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.TOP, D, ConstraintAnchor.Type.TOP);
            B.setDimensionRatio("1:1");

            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.RIGHT, D, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.TOP, D, ConstraintAnchor.Type.TOP);
            C.connect(ConstraintAnchor.Type.BOTTOM, D, ConstraintAnchor.Type.BOTTOM);
            C.setDimensionRatio("1:1");

            //        root.layout();
            //        System.out.println("a) root: " + root + " D: " + D + " A: " + A + " B: " + B + " C: " + C);
            //
            //        root.setWidth(0);
            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();
            Console.WriteLine("b) root: " + root + " D: " + D + " A: " + A + " B: " + B + " C: " + C);

            Assert.AreEqual(root.Width, 120);
            Assert.AreEqual(D.Width, 120);
            Assert.AreEqual(A.Width, 40);
            Assert.AreEqual(A.Height, 40);
            Assert.AreEqual(B.Width, 40);
            Assert.AreEqual(B.Height, 40);
            Assert.AreEqual(C.Width, 40);
            Assert.AreEqual(C.Height, 40);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testRatioChainWrap2()
        [Test]
        public virtual void testRatioChainWrap2()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 1536);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            ConstraintWidget D = new ConstraintWidget(100, 40);
            ConstraintWidget E = new ConstraintWidget(100, 40);
            ConstraintWidget F = new ConstraintWidget(100, 40);
            root.DebugName = "root";
            root.add(A);
            root.add(B);
            root.add(C);
            root.add(D);
            root.add(E);
            root.add(F);
            A.DebugName = "A";
            B.DebugName = "B";
            C.DebugName = "C";
            D.DebugName = "D";
            E.DebugName = "E";
            F.DebugName = "F";

            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;

            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;

            C.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            C.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;

            D.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            D.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;

            E.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            E.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;

            F.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            F.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;

            D.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            D.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            D.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            D.connect(ConstraintAnchor.Type.BOTTOM, E, ConstraintAnchor.Type.TOP);

            A.connect(ConstraintAnchor.Type.LEFT, D, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.TOP, D, ConstraintAnchor.Type.TOP);
            A.setDimensionRatio("1:1");

            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.TOP, D, ConstraintAnchor.Type.TOP);
            B.setDimensionRatio("1:1");

            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.RIGHT, D, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.TOP, D, ConstraintAnchor.Type.TOP);
            C.connect(ConstraintAnchor.Type.BOTTOM, D, ConstraintAnchor.Type.BOTTOM);
            C.setDimensionRatio("1:1");

            E.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            E.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            E.connect(ConstraintAnchor.Type.TOP, D, ConstraintAnchor.Type.BOTTOM);
            E.connect(ConstraintAnchor.Type.BOTTOM, F, ConstraintAnchor.Type.TOP);
            E.setDimensionRatio("1:1");

            F.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            F.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            F.connect(ConstraintAnchor.Type.TOP, E, ConstraintAnchor.Type.BOTTOM);
            F.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            F.setDimensionRatio("1:1");

            root.layout();
            Console.WriteLine("a) root: " + root + " D: " + D + " A: " + A + " B: " + B + " C: " + C + " D: " + D + " E: " + E + " F: " + F);

            root.Width = 0;
            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();
            Console.WriteLine("b) root: " + root + " D: " + D + " A: " + A + " B: " + B + " C: " + C + " D: " + D + " E: " + E + " F: " + F);

            //Assert.AreEqual(root.getWidth(), 748);
            Assert.AreEqual(D.Width, root.Width);
            Assert.AreEqual(A.Width, D.Height);
            Assert.AreEqual(A.Height, D.Height);
            Assert.AreEqual(B.Width, D.Height);
            Assert.AreEqual(B.Height, D.Height);
            Assert.AreEqual(C.Width, D.Height);
            Assert.AreEqual(C.Height, D.Height);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testRatioMax()
        [Test]
        public virtual void testRatioMax()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 1000);
            ConstraintWidget A = new ConstraintWidget(100, 100);
            root.DebugName = "root";
            root.add(A);
            A.DebugName = "A";

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);

            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);

            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.setVerticalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_RATIO, 0, 150, 0);
            A.setDimensionRatio("W,16:9");

            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            root.layout();

            Console.WriteLine("a) root: " + root + " A: " + A);
            Assert.AreEqual(A.Width, 267);
            Assert.AreEqual(A.Height, 150);
            Assert.AreEqual(A.Top, 425);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testRatioMax2()
        [Test]
        public virtual void testRatioMax2()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 1000);
            ConstraintWidget A = new ConstraintWidget(100, 100);
            root.DebugName = "root";
            root.add(A);
            A.DebugName = "A";

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);

            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.setVerticalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_RATIO, 0, 150, 0);
            A.setDimensionRatio("16:9");

            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            root.layout();

            Console.WriteLine("a) root: " + root + " A: " + A);
            Assert.AreEqual(A.Width, 267, 1);
            Assert.AreEqual(A.Height, 150);
            Assert.AreEqual(A.Top, 425);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testRatioSingleTarget()
        [Test]
        public virtual void testRatioSingleTarget()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 1000);
            ConstraintWidget A = new ConstraintWidget(100, 100);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            root.add(A);
            root.add(B);
            A.DebugName = "A";
            B.DebugName = "B";

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;

            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.BOTTOM, A, ConstraintAnchor.Type.BOTTOM);
            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.setDimensionRatio("2:3");
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.LEFT, 50);

            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            root.layout();

            Console.WriteLine("a) root: " + root + " A: " + A + " B: " + B);
            Assert.AreEqual(B.Height, 150);
            Assert.AreEqual(B.Top, A.Bottom - B.Height / 2);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testSimpleWrapRatio()
        [Test]
        public virtual void testSimpleWrapRatio()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 1000);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            root.add(A);
            A.DebugName = "A";

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);


            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;

            A.setDimensionRatio("1:1");
            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();

            Console.WriteLine("a) root: " + root + " A: " + A);
            Assert.AreEqual(root.Width, 1000);
            Assert.AreEqual(root.Height, 1000);
            Assert.AreEqual(A.Width, 1000);
            Assert.AreEqual(A.Height, 1000);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testSimpleWrapRatio2()
        [Test]
        public virtual void testSimpleWrapRatio2()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 1000);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            root.add(A);
            A.DebugName = "A";

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);


            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;

            A.setDimensionRatio("1:1");
            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();

            Console.WriteLine("a) root: " + root + " A: " + A);
            Assert.AreEqual(root.Width, 1000);
            Assert.AreEqual(root.Height, 1000);
            Assert.AreEqual(A.Width, 1000);
            Assert.AreEqual(A.Height, 1000);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testNestedRatio()
        [Test]
        public virtual void testNestedRatio()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 1000);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            root.add(A);
            root.add(B);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP);

            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.RIGHT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);

            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;

            A.setDimensionRatio("1:1");
            B.setDimensionRatio("1:1");

            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();

            Console.WriteLine("a) root: " + root + " A: " + A + " B: " + B);
            Assert.AreEqual(root.Width, 500);
            Assert.AreEqual(A.Width, 500);
            Assert.AreEqual(B.Width, 500);
            Assert.AreEqual(root.Height, 1000);
            Assert.AreEqual(A.Height, 500);
            Assert.AreEqual(B.Height, 500);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testNestedRatio2()
        [Test]
        public virtual void testNestedRatio2()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 700, 1200);
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
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);

            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.RIGHT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.BOTTOM, A, ConstraintAnchor.Type.BOTTOM);

            C.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.RIGHT, A, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.TOP);
            C.connect(ConstraintAnchor.Type.BOTTOM, A, ConstraintAnchor.Type.BOTTOM);

            D.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.LEFT);
            D.connect(ConstraintAnchor.Type.RIGHT, A, ConstraintAnchor.Type.RIGHT);
            D.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.TOP);
            D.connect(ConstraintAnchor.Type.BOTTOM, A, ConstraintAnchor.Type.BOTTOM);

            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;

            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.VerticalBiasPercent = 0;

            C.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            C.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            C.VerticalBiasPercent = 0.5f;

            D.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            D.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            D.VerticalBiasPercent = 1;

            A.setDimensionRatio("1:1");
            B.setDimensionRatio("4:1");
            C.setDimensionRatio("4:1");
            D.setDimensionRatio("4:1");

            root.layout();

            Console.WriteLine("a) root: " + root + " A: " + A + " B: " + B + " C: " + C + " D: " + D);
            Assert.AreEqual(A.Width, 700);
            Assert.AreEqual(A.Height, 700);
            Assert.AreEqual(B.Width, A.Width);
            Assert.AreEqual(B.Height, B.Width / 4);
            Assert.AreEqual(B.Top, A.Top);
            Assert.AreEqual(C.Width, A.Width);
            Assert.AreEqual(C.Height, C.Width / 4);
            Assert.AreEqual(C.Top, (root.Height - C.Height) / 2, 1);
            Assert.AreEqual(D.Width, A.Width);
            Assert.AreEqual(D.Height, D.Width / 4);
            Assert.AreEqual(D.Top, A.Bottom - D.Height);

            root.Width = 300;
            root.layout();

            Console.WriteLine("b) root: " + root + " A: " + A + " B: " + B + " C: " + C + " D: " + D);
            Assert.AreEqual(A.Width, root.Width);
            Assert.AreEqual(A.Height, root.Width);
            Assert.AreEqual(B.Width, A.Width);
            Assert.AreEqual(B.Height, B.Width / 4);
            Assert.AreEqual(B.Top, A.Top);
            Assert.AreEqual(C.Width, A.Width);
            Assert.AreEqual(C.Height, C.Width / 4);
            Assert.AreEqual(C.Top, (root.Height - C.Height) / 2, 1);
            Assert.AreEqual(D.Width, A.Width);
            Assert.AreEqual(D.Height, D.Width / 4);
            Assert.AreEqual(D.Top, A.Bottom - D.Height);

            root.Width = 0;
            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();

            Console.WriteLine("c) root: " + root + " A: " + A + " B: " + B + " C: " + C + " D: " + D);
            Assert.True(root.Width > 0, "root width should be bigger than zero");
            Assert.AreEqual(A.Width, root.Width);
            Assert.AreEqual(A.Height, root.Width);
            Assert.AreEqual(B.Width, A.Width);
            Assert.AreEqual(B.Height, B.Width / 4);
            Assert.AreEqual(B.Top, A.Top);
            Assert.AreEqual(C.Width, A.Width);
            Assert.AreEqual(C.Height, C.Width / 4);
            Assert.AreEqual(C.Top, (root.Height - C.Height) / 2, 1);
            Assert.AreEqual(D.Width, A.Width);
            Assert.AreEqual(D.Height, D.Width / 4);
            Assert.AreEqual(D.Top, A.Bottom - D.Height);

            root.Width = 700;
            root.Height = 0;
            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();

            Console.WriteLine("d) root: " + root + " A: " + A + " B: " + B + " C: " + C + " D: " + D);
            Assert.True( root.Height > 0, "root width should be bigger than zero");
            Assert.AreEqual(A.Width, root.Width);
            Assert.AreEqual(A.Height, root.Width);
            Assert.AreEqual(B.Width, A.Width);
            Assert.AreEqual(B.Height, B.Width / 4);
            Assert.AreEqual(B.Top, A.Top);
            Assert.AreEqual(C.Width, A.Width);
            Assert.AreEqual(C.Height, C.Width / 4, 1);
            Assert.AreEqual(C.Top, (root.Height - C.Height) / 2, 1);
            Assert.AreEqual(D.Width, A.Width);
            Assert.AreEqual(D.Height, D.Width / 4);
            Assert.AreEqual(D.Top, A.Bottom - D.Height);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testNestedRatio3()
        [Test]
        public virtual void testNestedRatio3()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1080, 1536);
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

            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.setDimensionRatio("1:1");

            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.setDimensionRatio("3.5:1");

            C.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            C.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            C.setDimensionRatio("5:2");

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);

            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.RIGHT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.BOTTOM, A, ConstraintAnchor.Type.BOTTOM);
            B.VerticalBiasPercent = 0.9f;

            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.TOP);
            C.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.BOTTOM);
            C.VerticalBiasPercent = 0.9f;

            //        root.layout();
            //        System.out.println("A: " + A);
            //        System.out.println("B: " + B);
            //        System.out.println("C: " + C);
            //
            //        Assert.AreEqual((float)A.getWidth() / A.getHeight(), 1f, .1f);
            //        Assert.AreEqual((float)B.getWidth() / B.getHeight(), 3.5f, .1f);
            //        Assert.AreEqual((float)C.getWidth() / C.getHeight(), 2.5f, .1f);
            //        Assert.AreEqual(B.getTop() >= A.getTop(), true);
            //        Assert.AreEqual(B.getTop() <= A.getBottom(), true);
            //        Assert.AreEqual(C.getTop() >= B.getTop(), true);
            //        Assert.AreEqual(C.getBottom() <= B.getBottom(), true);

            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();
            Console.WriteLine("\nA: " + A);
            Console.WriteLine("B: " + B);
            Console.WriteLine("C: " + C);
            Assert.AreEqual((float)A.Width / A.Height, 1f, .1f);
            Assert.AreEqual((float)B.Width / B.Height, 3.5f, .1f);
            Assert.AreEqual((float)C.Width / C.Height, 2.5f, .1f);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testNestedRatio4()
        [Test]
        public virtual void testNestedRatio4()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 600);
            ConstraintWidget A = new ConstraintWidget(264, 144);
            ConstraintWidget B = new ConstraintWidget(264, 144);

            Guideline verticalGuideline = new Guideline();
            verticalGuideline.setGuidePercent (0.34f);
            verticalGuideline.Orientation = Guideline.VERTICAL;

            Guideline horizontalGuideline = new Guideline();
            horizontalGuideline.setGuidePercent (0.66f);
            horizontalGuideline.Orientation = Guideline.HORIZONTAL;

            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            horizontalGuideline.DebugName = "hGuideline";
            verticalGuideline.DebugName = "vGuideline";

            root.add(A);
            root.add(B);
            root.add(verticalGuideline);
            root.add(horizontalGuideline);

            A.Width = 200;
            A.Height = 200;
            A.connect(ConstraintAnchor.Type.BOTTOM, horizontalGuideline, ConstraintAnchor.Type.BOTTOM);
            A.connect(ConstraintAnchor.Type.LEFT, verticalGuideline, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, verticalGuideline, ConstraintAnchor.Type.RIGHT);
            A.connect(ConstraintAnchor.Type.TOP, horizontalGuideline, ConstraintAnchor.Type.TOP);

            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;

            B.setDimensionRatio("H,1:1");
            B.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_PERCENT, 0, 0, 0.3f);
            B.connect(ConstraintAnchor.Type.BOTTOM, horizontalGuideline, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.LEFT, verticalGuideline, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.RIGHT, verticalGuideline, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.TOP, horizontalGuideline, ConstraintAnchor.Type.TOP);

            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();
            Console.WriteLine("\nroot: " + root);
            Console.WriteLine("A: " + A);
            Console.WriteLine("B: " + B);
            Console.WriteLine("hG: " + horizontalGuideline);
            Console.WriteLine("vG: " + verticalGuideline);

            Assert.AreEqual(verticalGuideline.Left, 0.34f * root.Width, 1);
            Assert.AreEqual(horizontalGuideline.Top, 0.66f * root.Height, 1);
            Assert.True(A.Left >= 0);
            Assert.True(B.Left >= 0);
            Assert.AreEqual(A.Left, verticalGuideline.Left - A.Width / 2);
            Assert.AreEqual(A.Top, horizontalGuideline.Top - A.Height / 2);

            Assert.AreEqual(B.Left, verticalGuideline.Left - B.Width / 2);
            Assert.AreEqual(B.Top, horizontalGuideline.Top - B.Height / 2);

        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testBasicCenter()
        [Test]
        public virtual void testBasicCenter()
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
            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A);
            Assert.AreEqual(A.Left, 450);
            Assert.AreEqual(A.Top, 290);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(A.Height, 20);
            root.OptimizationLevel = Optimizer.OPTIMIZATION_STANDARD;
            root.layout();
            Console.WriteLine("b) root: " + root + " A: " + A);
            Assert.AreEqual(A.Left, 450);
            Assert.AreEqual(A.Top, 290);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(A.Height, 20);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testBasicCenter2()
        [Test]
        public virtual void testBasicCenter2()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            root.add(A);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.setVerticalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_RATIO, 0, 150, 0);
            A.setDimensionRatio("W,16:9");
            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual((float)A.Width / A.Height, 16f / 9f, .1f);
            Assert.AreEqual(A.Height, 150);
            Assert.AreEqual((float)A.Top, (root.Height - A.Height) / 2f, 0f);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testBasicRatio()
        [Test]
        public virtual void testBasicRatio()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 1000);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            root.add(A);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            A.VerticalBiasPercent = 0;
            A.HorizontalBiasPercent = 0;
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.setDimensionRatio("1:1");
            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(A.Top, 0);
            Assert.AreEqual(A.Width, 600);
            Assert.AreEqual(A.Height, 600);
            A.VerticalBiasPercent = 1;
            root.layout();
            Console.WriteLine("b) root: " + root + " A: " + A);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(A.Top, 400);
            Assert.AreEqual(A.Width, 600);
            Assert.AreEqual(A.Height, 600);

            A.VerticalBiasPercent = 0;
            root.OptimizationLevel = Optimizer.OPTIMIZATION_STANDARD;
            root.layout();
            Console.WriteLine("c) root: " + root + " A: " + A);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(A.Top, 0);
            Assert.AreEqual(A.Width, 600);
            Assert.AreEqual(A.Height, 600);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testBasicRatio2()
        [Test]
        public virtual void testBasicRatio2()
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
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.setDimensionRatio("1:1");
            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A);
            Assert.AreEqual(A.Left, 450);
            Assert.AreEqual(A.Top, 250);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(A.Height, 100);
            root.OptimizationLevel = Optimizer.OPTIMIZATION_STANDARD;
            root.layout();
            Console.WriteLine("b) root: " + root + " A: " + A);
            Assert.AreEqual(A.Left, 450);
            Assert.AreEqual(A.Top, 250);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(A.Height, 100);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testSimpleRatio()
        [Test]
        public virtual void testSimpleRatio()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 200, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            root.add(A);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.setDimensionRatio("3:2");
            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A);
            Assert.AreEqual((float)A.Width / A.Height, 3.0f / 2.0f, .1f);
            Assert.True( A.Top >= 0, "A.top > 0");
            Assert.True( A.Left >= 0, "A.left > 0");
            Assert.AreEqual( A.Top, root.Height - A.Bottom, "A vertically centered");
            Assert.AreEqual( A.Left, root.Right - A.Right, "A horizontally centered");
            A.setDimensionRatio("1:2");
            root.layout();
            Console.WriteLine("b) root: " + root + " A: " + A);
            Assert.AreEqual((float)A.Width / A.Height, 1.0f / 2.0f, .1f);
            Assert.True( A.Top >= 0, "A.top > 0");
            Assert.True( A.Left >= 0, "A.left > 0");
            Assert.AreEqual( A.Top, root.Height - A.Bottom, "A vertically centered");
            Assert.AreEqual( A.Left, root.Right - A.Right, "A horizontally centered");
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testRatioGuideline()
        [Test]
        public virtual void testRatioGuideline()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 400, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            Guideline guideline = new Guideline();
            guideline.Orientation = ConstraintWidget.VERTICAL;
            guideline.GuideBegin = 200;
            root.DebugName = "root";
            A.DebugName = "A";
            root.add(A);
            root.add(guideline);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, guideline, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.setDimensionRatio("3:2");
            root.layout();
            Console.WriteLine("a) root: " + root + " guideline: " + guideline + " A: " + A);
            Assert.AreEqual(A.Width / A.Height, 3 / 2);
            Assert.True( A.Top >= 0, "A.top > 0");
            Assert.True( A.Left >= 0, "A.left > 0");
            Assert.AreEqual( A.Top, root.Height - A.Bottom, "A vertically centered");
            Assert.AreEqual( A.Left, guideline.Left - A.Right, "A horizontally centered");
            A.setDimensionRatio("1:2");
            root.layout();
            Console.WriteLine("b) root: " + root + " guideline: " + guideline + " A: " + A);
            Assert.AreEqual(A.Width / A.Height, 1 / 2);
            Assert.True(A.Top >= 0, "A.top > 0");
            Assert.True( A.Left >= 0, "A.left > 0");
            Assert.AreEqual( A.Top, root.Height - A.Bottom, "A vertically centered");
            Assert.AreEqual(A.Left, guideline.Left - A.Right, "A horizontally centered");
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testRatioWithMinimum()
        [Test]
        public virtual void testRatioWithMinimum()
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
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.setDimensionRatio("16:9");
            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            root.Width = 0;
            root.Height = 0;
            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A);
            Assert.AreEqual(root.Width, 0);
            Assert.AreEqual(root.Height, 0);
            A.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_SPREAD, 100, 0, 0);
            root.Width = 0;
            root.Height = 0;
            root.layout();
            Console.WriteLine("b) root: " + root + " A: " + A);
            Assert.AreEqual(root.Width, 100);
            Assert.AreEqual(root.Height, 56);
            A.setVerticalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_SPREAD, 100, 0, 0);
            root.Width = 0;
            root.Height = 0;
            root.layout();
            Console.WriteLine("c) root: " + root + " A: " + A);
            Assert.AreEqual(root.Width, 178);
            Assert.AreEqual(root.Height, 100);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testRatioWithPercent()
        [Test]
        public virtual void testRatioWithPercent()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 1000);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            root.add(A);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.setDimensionRatio("1:1");
            A.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_PERCENT, 0, 0, 0.7f);
            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A);
            int w = (int)(0.7 * root.Width);
            Assert.AreEqual(A.Width, w);
            Assert.AreEqual(A.Height, w);
            Assert.AreEqual(A.Left, (root.Width - w) / 2);
            Assert.AreEqual(A.Top, (root.Height - w) / 2);

            root.OptimizationLevel = Optimizer.OPTIMIZATION_STANDARD;
            root.layout();
            Console.WriteLine("b) root: " + root + " A: " + A);
            Assert.AreEqual(A.Width, w);
            Assert.AreEqual(A.Height, w);
            Assert.AreEqual(A.Left, (root.Width - w) / 2);
            Assert.AreEqual(A.Top, (root.Height - w) / 2);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testRatio()
        [Test]
        public virtual void testRatio()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            root.add(A);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.setDimensionRatio("16:9");
            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A);
            Assert.AreEqual(A.Width, 1067);
            Assert.AreEqual(A.Height, 600);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testRatio2()
        [Test]
        public virtual void testRatio2()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1080, 1920);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            root.add(A);
            root.add(B);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalBiasPercent = 0.9f;
            A.setDimensionRatio("3.5:1");

            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.RIGHT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.BOTTOM, A, ConstraintAnchor.Type.BOTTOM);
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.HorizontalBiasPercent = 0.5f;
            B.VerticalBiasPercent = 0.9f;
            B.setDimensionRatio("4:2");

            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A + " B: " + B);
            // A: id: A (0, 414) - (600 x 172) B: (129, 414) - (342 x 172)
            Assert.AreEqual(A.Width / (float)A.Height, 3.5f, 0.1);
            Assert.AreEqual(B.Width / (float)B.Height, 2f, 0.1);
            Assert.AreEqual(A.Width, 1080, 1);
            Assert.AreEqual(A.Height, 309, 1);
            Assert.AreEqual(B.Width, 618, 1);
            Assert.AreEqual(B.Height, 309, 1);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(A.Top, 1450);
            Assert.AreEqual(B.Left, 231);
            Assert.AreEqual(B.Top, A.Top);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testRatio3()
        [Test]
        public virtual void testRatio3()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1080, 1920);
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
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalBiasPercent = 0.5f;
            A.setDimensionRatio("1:1");

            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.RIGHT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.BOTTOM, A, ConstraintAnchor.Type.BOTTOM);
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.HorizontalBiasPercent = 0.5f;
            B.VerticalBiasPercent = 0.9f;
            B.setDimensionRatio("3.5:1");

            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.TOP);
            C.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.BOTTOM);
            C.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            C.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            C.HorizontalBiasPercent = 0.5f;
            C.VerticalBiasPercent = 0.9f;
            C.setDimensionRatio("5:2");

            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A + " B: " + B + " C: " + C);
            // A: id: A (0, 414) - (600 x 172) B: (129, 414) - (342 x 172)
            Assert.AreEqual(A.Width / (float)A.Height, 1.0f, 0.1);
            Assert.AreEqual(B.Width / (float)B.Height, 3.5f, 0.1);
            Assert.AreEqual(C.Width / (float)C.Height, 2.5f, 0.1);
            Assert.AreEqual(A.Width, 1080, 1);
            Assert.AreEqual(A.Height, 1080, 1);
            Assert.AreEqual(B.Width, 1080, 1);
            Assert.AreEqual(B.Height, 309, 1);
            Assert.AreEqual(C.Width, 772, 1);
            Assert.AreEqual(C.Height, 309, 1);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(A.Top, 420);
            Assert.AreEqual(B.Top, 1114);
            Assert.AreEqual(C.Left, 154);
            Assert.AreEqual(C.Top, B.Top);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testDanglingRatio()
        [Test]
        public virtual void testDanglingRatio()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            root.add(A);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.setDimensionRatio("1:1");
            //        root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A);
            //        Assert.AreEqual(A.getWidth(), 1000);
            //        Assert.AreEqual(A.getHeight(), 1000);
            A.Width = 100;
            A.Height = 20;
            A.setDimensionRatio("W,1:1");
            root.layout();
            Console.WriteLine("b) root: " + root + " A: " + A);
            Assert.AreEqual(A.Width, 1000);
            Assert.AreEqual(A.Height, 1000);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testDanglingRatio2()
        [Test]
        public virtual void testDanglingRatio2()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 600);
            ConstraintWidget A = new ConstraintWidget(300, 200);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            root.add(A);
            B.DebugName = "B";
            root.add(B);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 20);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 100);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.BOTTOM, A, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT, 15);
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.setDimensionRatio("1:1");
            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A + " B: " + B);
            Assert.AreEqual(B.Left, 335);
            Assert.AreEqual(B.Top, 100);
            Assert.AreEqual(B.Width, 200);
            Assert.AreEqual(B.Height, 200);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testDanglingRatio3()
        [Test]
        public virtual void testDanglingRatio3()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 600);
            ConstraintWidget A = new ConstraintWidget(300, 200);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            root.add(A);
            B.DebugName = "B";
            root.add(B);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 20);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 100);
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.setDimensionRatio("h,1:1");
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.BOTTOM, A, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT, 15);
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.setDimensionRatio("w,1:1");
            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A + " B: " + B);
            Assert.AreEqual(A.Left, 20);
            Assert.AreEqual(A.Top, 100);
            Assert.AreEqual(A.Width, 300);
            Assert.AreEqual(A.Height, 300);
            Assert.AreEqual(B.Left, 335);
            Assert.AreEqual(B.Top, 100);
            Assert.AreEqual(B.Width, 300);
            Assert.AreEqual(B.Height, 300);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testChainRatio()
        [Test]
        public virtual void testChainRatio()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(300, 20);
            ConstraintWidget C = new ConstraintWidget(300, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            C.DebugName = "C";
            root.add(A);
            root.add(B);
            root.add(C);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.setDimensionRatio("1:1");
            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(A.Top, 100);
            Assert.AreEqual(A.Width, 400);
            Assert.AreEqual(A.Height, 400);

            Assert.AreEqual(B.Left, 400);
            Assert.AreEqual(B.Top, 0);
            Assert.AreEqual(B.Width, 300);
            Assert.AreEqual(B.Height, 20);

            Assert.AreEqual(C.Left, 700);
            Assert.AreEqual(C.Top, 0);
            Assert.AreEqual(C.Width, 300);
            Assert.AreEqual(C.Height, 20);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testChainRatio2()
        [Test]
        public virtual void testChainRatio2()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 1000);
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
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.setDimensionRatio("1:1");
            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(A.Top, 300);
            Assert.AreEqual(A.Width, 400);
            Assert.AreEqual(A.Height, 400);

            Assert.AreEqual(B.Left, 400);
            Assert.AreEqual(B.Top, 0);
            Assert.AreEqual(B.Width, 100);
            Assert.AreEqual(B.Height, 20);

            Assert.AreEqual(C.Left, 500);
            Assert.AreEqual(C.Top, 0);
            Assert.AreEqual(C.Width, 100);
            Assert.AreEqual(C.Height, 20);
        }


        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testChainRatio3()
        [Test]
        public virtual void testChainRatio3()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 1000);
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
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.BOTTOM, C, ConstraintAnchor.Type.TOP);
            C.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.BOTTOM);
            C.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.setDimensionRatio("1:1");
            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(A.Top, 90);
            Assert.AreEqual(A.Width, 600);
            Assert.AreEqual(A.Height, 600);

            Assert.AreEqual(B.Left, 0);
            Assert.AreEqual(B.Top, 780);
            Assert.AreEqual(B.Width, 100);
            Assert.AreEqual(B.Height, 20);

            Assert.AreEqual(C.Left, 0);
            Assert.AreEqual(C.Top, 890);
            Assert.AreEqual(C.Width, 100);
            Assert.AreEqual(C.Height, 20);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testChainRatio4()
        [Test]
        public virtual void testChainRatio4()
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
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.setDimensionRatio("4:3");
            root.layout();
            Console.WriteLine("a) root: " + root + " A: " + A + " B: " + B);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(A.Top, 113, 1);
            Assert.AreEqual(A.Width, 500);
            Assert.AreEqual(A.Height, 375);

            Assert.AreEqual(B.Left, 500);
            Assert.AreEqual(B.Top, 113, 1);
            Assert.AreEqual(B.Width, 500);
            Assert.AreEqual(B.Height, 375);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testChainRatio5()
        [Test]
        public virtual void testChainRatio5()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 700, 1200);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            root.add(B);
            root.add(A);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);

            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.setDimensionRatio("1:1");
            A.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_RATIO, 60, 0, 0);

            root.layout();

            Console.WriteLine("a) root: " + root + " A: " + A + " B: " + B);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(A.Top, 300);
            Assert.AreEqual(A.Width, 600);
            Assert.AreEqual(A.Height, 600);

            Assert.AreEqual(B.Left, 600);
            Assert.AreEqual(B.Top, 590);
            Assert.AreEqual(B.Width, 100);
            Assert.AreEqual(B.Height, 20);

            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_WRAP, 0, 0, 0);

            root.layout();

            Console.WriteLine("b) root: " + root + " A: " + A + " B: " + B);
            Assert.AreEqual(A.Left, 0);
            Assert.AreEqual(A.Top, 300);
            Assert.AreEqual(A.Width, 600);
            Assert.AreEqual(A.Height, 600);

            Assert.AreEqual(B.Left, 600);
            Assert.AreEqual(B.Top, 590);
            Assert.AreEqual(B.Width, 100);
            Assert.AreEqual(B.Height, 20);

            root.Width = 1080;
            root.Height = 1536;
            A.Width = 180;
            A.Height = 180;
            B.Width = 900;
            B.Height = 106;
            A.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_RATIO, 180, 0, 0);
            root.layout();
            Console.WriteLine("c) root: " + root + " A: " + A + " B: " + B);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testChainRatio6()
        [Test]
        public virtual void testChainRatio6()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 600);
            ConstraintWidget A = new ConstraintWidget(264, 144);
            ConstraintWidget B = new ConstraintWidget(264, 144);
            ConstraintWidget C = new ConstraintWidget(264, 144);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            C.DebugName = "C";
            root.add(A);
            root.add(B);
            root.add(C);
            A.VerticalChainStyle = ConstraintWidget.CHAIN_SPREAD_INSIDE;
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);

            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.BOTTOM, C, ConstraintAnchor.Type.TOP);
            C.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.BOTTOM);
            C.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            B.HorizontalBiasPercent = 0.501f;
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.setDimensionRatio("1:1");
            A.BaselineDistance = 88;
            C.BaselineDistance = 88;
            root.Width = 1080;
            root.Height = 2220;
            //        root.setHorizontalDimensionBehaviour(ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT);
            //        root.setVerticalDimensionBehaviour(ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT);
            //        root.layout();
            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.layout();
            Console.WriteLine("a) root: " + root);
            Console.WriteLine(" A: " + A);
            Console.WriteLine(" B: " + B);
            Console.WriteLine(" C: " + C);
            Assert.AreEqual(A.Width, B.Width);
            Assert.AreEqual(B.Width, B.Height);
            Assert.AreEqual(root.Width, C.Width);
            Assert.AreEqual(root.Height, A.Height + B.Height + C.Height);
        }

    }

}
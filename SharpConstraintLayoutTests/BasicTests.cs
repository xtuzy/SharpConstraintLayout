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

using androidx.constraintlayout.core.widgets;
using androidx.constraintlayout.core.widgets.analyzer;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace androidx.constraintlayout.core
{
    [NUnit.Framework.TestFixture]
    public class BasicTests
    {
        [Test()]
        public void WrapPercentTest()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 800);
            ConstraintWidget A = new ConstraintWidget(100, 30);
            root.DebugName = "root";
            A.DebugName = "A";

            A.HorizontalDimensionBehaviour = (ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT);
            A.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_PERCENT, BasicMeasure.WRAP_CONTENT, 0, 0.5f);

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);

            root.add(A);

            root.HorizontalDimensionBehaviour = (ConstraintWidget.DimensionBehaviour.WRAP_CONTENT);
            root.layout();

            Debug.WriteLine("root: " + root);
			Debug.WriteLine("A: " + A);
            Assert.AreEqual(A.Width, 100);
            Assert.AreEqual(root.Width, A.Width * 2);
        }

        [Test]
        public void MiddleSplitTest()
        {
			ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 800);
			root.Measurer = sMeasurer;
			ConstraintWidget A = new ConstraintWidget(400, 30);
			ConstraintWidget B = new ConstraintWidget(400, 60);
			Guideline guideline = new Guideline();
			ConstraintWidget divider = new ConstraintWidget(100, 30);

			root.DebugName = "root";
			A.DebugName = "A";
			B.DebugName = "B";
			guideline.DebugName = "guideline";
			divider.DebugName = "divider";

			root.add(A);
			root.add(B);
			root.add(guideline);
			root.add(divider);

			guideline.Orientation = Guideline.VERTICAL;
			guideline.setGuidePercent(0.5f);

			A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
			B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
			A.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_WRAP, 0, 0, 0);
			B.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_WRAP, 0, 0, 0);
			A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
			A.connect(ConstraintAnchor.Type.RIGHT, guideline, ConstraintAnchor.Type.LEFT);
			B.connect(ConstraintAnchor.Type.LEFT, guideline, ConstraintAnchor.Type.RIGHT);
			B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
			divider.Width = 1;
			divider.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
			divider.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
			divider.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);

			root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
			//        root.layout();
			root.updateHierarchy();
			root.measure(Optimizer.OPTIMIZATION_NONE, BasicMeasure.EXACTLY, 600, BasicMeasure.EXACTLY, 800, 0, 0, 0, 0);
			Console.WriteLine("root: " + root);
			Console.WriteLine("A: " + A);
			Console.WriteLine("B: " + B);
			Console.WriteLine("guideline: " + guideline);
			Console.WriteLine("divider: " + divider);

			Assert.AreEqual(A.Width, 300);
			Assert.AreEqual(B.Width, 300);
			Assert.AreEqual(A.Left, 0);
			Assert.AreEqual(B.Left, 300);
			Assert.AreEqual(divider.Height, 60);
			Assert.AreEqual(root.Width, 600);
			Assert.AreEqual(root.Height, 60); 
        }

		internal static BasicMeasure.Measurer sMeasurer = new MeasurerAnonymousInnerClass();

		private class MeasurerAnonymousInnerClass : BasicMeasure.Measurer
		{
			public MeasurerAnonymousInnerClass()
			{
			}


			public virtual void measure(ConstraintWidget widget, BasicMeasure.Measure measure)
			{
				ConstraintWidget.DimensionBehaviour horizontalBehavior = measure.horizontalBehavior;
				ConstraintWidget.DimensionBehaviour verticalBehavior = measure.verticalBehavior;
				int horizontalDimension = measure.horizontalDimension;
				int verticalDimension = measure.verticalDimension;

				if (horizontalBehavior == ConstraintWidget.DimensionBehaviour.FIXED)
				{
					measure.measuredWidth = horizontalDimension;
				}
				else if (horizontalBehavior == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT)
				{
					measure.measuredWidth = horizontalDimension;
				}
				if (verticalBehavior == ConstraintWidget.DimensionBehaviour.FIXED)
				{
					measure.measuredHeight = verticalDimension;
				}
				widget.MeasureRequested = false;
			}

			public virtual void didMeasures()
			{

			}
		}

        [Test]
        public void SimpleConstraintTest()
        {
			ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 800);
			root.Measurer = sMeasurer;
			ConstraintWidget A = new ConstraintWidget(100, 30);

			root.DebugName = "root";
			A.DebugName = "A";

			root.add(A);

			A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;

			A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 8);
			A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 8);

			root.OptimizationLevel = Optimizer.OPTIMIZATION_GRAPH;
			root.measure(Optimizer.OPTIMIZATION_GRAPH, 0, 0, 0, 0, 0, 0, 0, 0);
			//        root.layout();
			Debug.WriteLine("1) A: " + A);
		}

        [Test]
        public void SimpleWrapConstraint9Test()
        {
			ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 800);
			root.Measurer=(sMeasurer);
			ConstraintWidget A = new ConstraintWidget(100, 30);

			root.DebugName=("root");
			A.DebugName=("A");

			root.add(A);

			A.HorizontalDimensionBehaviour = (ConstraintWidget.DimensionBehaviour.FIXED);
			A.VerticalDimensionBehaviour=(ConstraintWidget.DimensionBehaviour.FIXED);

			int margin = 8;
			A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, margin);
			A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, margin);
			A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, margin);
			A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, margin);


			root.OptimizationLevel = (Optimizer.OPTIMIZATION_GRAPH_WRAP);
			root.measure(Optimizer.OPTIMIZATION_GRAPH_WRAP, 0, 0, 0, 0, 0, 0, 0, 0);
			//        root.layout();

			Debug.WriteLine("root: " + root);
			Debug.WriteLine("1) A: " + A);

			root.HorizontalDimensionBehaviour=(ConstraintWidget.DimensionBehaviour.WRAP_CONTENT);
			root.VerticalDimensionBehaviour=(ConstraintWidget.DimensionBehaviour.WRAP_CONTENT);
			root.measure(Optimizer.OPTIMIZATION_GRAPH_WRAP, 0, 0, 0, 0, 0, 0, 0, 0);

			Debug.WriteLine("root: " + root);
			Debug.WriteLine("1) A: " + A);
			Assert.AreEqual(root.Width, 116);
			Assert.AreEqual(root.Height, 46);
		}

        [Test]
        public void SimpleWrapConstraint10Test()
        {
			ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 800);
			root.Measurer=(sMeasurer);
			ConstraintWidget A = new ConstraintWidget(100, 30);

			root.DebugName=("root");
			A.DebugName=("A");

			root.add(A);

			A.HorizontalDimensionBehaviour=(ConstraintWidget.DimensionBehaviour.FIXED);
			A.VerticalDimensionBehaviour=(ConstraintWidget.DimensionBehaviour.FIXED);

			int margin = 8;
			A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, margin);
			A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, margin);
			A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, margin);
			A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, margin);


			//root.setVerticalDimensionBehaviour(ConstraintWidget.DimensionBehaviour.WRAP_CONTENT);

			root.measure(Optimizer.OPTIMIZATION_NONE, 0, 0, 0, 0, 0, 0, 0, 0);
			root.layout();

			Debug.WriteLine("root: " + root);
			Debug.WriteLine("1) A: " + A);

			root.HorizontalDimensionBehaviour=(ConstraintWidget.DimensionBehaviour.WRAP_CONTENT);
			root.measure(Optimizer.OPTIMIZATION_GRAPH, BasicMeasure.WRAP_CONTENT, 0, BasicMeasure.EXACTLY, 800, 0, 0, 0, 0);

			Debug.WriteLine("root: " + root);
			Debug.WriteLine("1) A: " + A);
			Assert.AreEqual(root.Width, 116);
			Assert.AreEqual(root.Height, 800);
			Assert.AreEqual(A.Left, 8);
			Assert.AreEqual(A.Top, 385);
			Assert.AreEqual(A.Width, 100);
			Assert.AreEqual(A.Height, 30);
		}

        [Test]
        public void SimpleWrapConstraint11Test()
        {
			ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 800);
			root.Measurer=(sMeasurer);
			ConstraintWidget A = new ConstraintWidget(10, 30);
			ConstraintWidget B = new ConstraintWidget(800, 30);
			ConstraintWidget C = new ConstraintWidget(10, 30);
			ConstraintWidget D = new ConstraintWidget(800, 30);

			root.DebugName=("root");
			A.DebugName=("A");
			B.DebugName=("B");
			C.DebugName=("C");
			D.DebugName=("D");

			root.add(A);
			root.add(B);
			root.add(C);
			root.add(D);

			B.HorizontalDimensionBehaviour=(ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT);
			B.VerticalDimensionBehaviour=(ConstraintWidget.DimensionBehaviour.FIXED);
			B.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_WRAP, 0, 0, 0);


			A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
			B.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
			C.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
			D.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.BOTTOM);

			A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
			A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
			B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
			B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT);
			C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT);
			C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
			D.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
			D.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT);

			root.layout();

			Debug.WriteLine("root: " + root);
			Debug.WriteLine("1) A: " + A);
			Debug.WriteLine("1) B: " + B);
			Debug.WriteLine("1) C: " + C);
			Debug.WriteLine("1) D: " + D);

			Assert.AreEqual(A.Left, 0);
			Assert.AreEqual(A.Width, 10);
			Assert.AreEqual(C.Width, 10);
			Assert.AreEqual(B.Left, A.Right);
			Assert.AreEqual(B.Width, root.Width - A.Width - C.Width);
			Assert.AreEqual(C.Left, root.Width - C.Width);
			Assert.AreEqual(D.Width, 800);
			Assert.AreEqual(D.Left, -99);
		}

        [Test]
        public void SimpleWrapConstraintTest()
        {
			ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 800);
			root.Measurer = sMeasurer;
			ConstraintWidget A = new ConstraintWidget(100, 30);
			ConstraintWidget B = new ConstraintWidget(100, 60);

			root.DebugName = "root";
			A.DebugName = "A";
			B.DebugName = "B";

			root.add(A);
			root.add(B);

			A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;

			A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 8);
			B.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, 8);
			A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 8);
			B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT, 8);

			root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
			root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
			root.measure(Optimizer.OPTIMIZATION_STANDARD, BasicMeasure.WRAP_CONTENT, 0, BasicMeasure.WRAP_CONTENT, 0, 0, 0, 0, 0);

			Console.WriteLine("0) root: " + root);
			Console.WriteLine("1) A: " + A);
			Console.WriteLine("2) B: " + B);
			Assert.AreEqual(root.Width, 216);
			Assert.AreEqual(root.Height, 68);
			Assert.AreEqual(A.Left, 8);
			Assert.AreEqual(A.Top, 8);
			Assert.AreEqual(A.Width, 100);
			Assert.AreEqual(A.Height, 30);
			Assert.AreEqual(B.Left, 116);
			Assert.AreEqual(B.Top, 0);
			Assert.AreEqual(B.Width, 100);
			Assert.AreEqual(B.Height, 60);

			root.measure(Optimizer.OPTIMIZATION_GRAPH, BasicMeasure.WRAP_CONTENT, 0, BasicMeasure.WRAP_CONTENT, 0, 0, 0, 0, 0);
			Console.WriteLine("0) root: " + root);
			Console.WriteLine("1) A: " + A);
			Console.WriteLine("2) B: " + B);
			Assert.AreEqual(root.Width, 216);
			Assert.AreEqual(root.Height, 68);
			Assert.AreEqual(A.Left, 8);
			Assert.AreEqual(A.Top, 8);
			Assert.AreEqual(A.Width, 100);
			Assert.AreEqual(A.Height, 30);
			Assert.AreEqual(B.Left, 116);
			Assert.AreEqual(B.Top, 0);
			Assert.AreEqual(B.Width, 100);
			Assert.AreEqual(B.Height, 60);
		}

        [Test]
        public void SimpleWrapConstraint2Test()
        {
			ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 800);
			root.Measurer = sMeasurer;
			ConstraintWidget A = new ConstraintWidget(100, 30);
			ConstraintWidget B = new ConstraintWidget(120, 60);

			root.DebugName = "root";
			A.DebugName = "A";
			B.DebugName = "B";

			root.add(A);
			root.add(B);

			A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;

			A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 8);
			B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM, 8);
			B.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, 8);
			A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 8);
			B.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 8);

			root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
			root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
			root.measure(Optimizer.OPTIMIZATION_STANDARD, BasicMeasure.WRAP_CONTENT, 0, BasicMeasure.WRAP_CONTENT, 0, 0, 0, 0, 0);
			//        root.layout();

			Console.WriteLine("0) root: " + root);
			Console.WriteLine("1) A: " + A);
			Console.WriteLine("2) B: " + B);
			Assert.AreEqual(root.Width, 128);
			Assert.AreEqual(root.Height, 114);
			Assert.AreEqual(A.Left, 8);
			Assert.AreEqual(A.Top, 8);
			Assert.AreEqual(A.Width, 100);
			Assert.AreEqual(A.Height, 30);
			Assert.AreEqual(B.Left, 8);
			Assert.AreEqual(B.Top, 46);
			Assert.AreEqual(B.Width, 120);
			Assert.AreEqual(B.Height, 60);

			root.measure(Optimizer.OPTIMIZATION_GRAPH, BasicMeasure.WRAP_CONTENT, 0, BasicMeasure.WRAP_CONTENT, 0, 0, 0, 0, 0);
			Console.WriteLine("0) root: " + root);
			Console.WriteLine("1) A: " + A);
			Console.WriteLine("2) B: " + B);
			Assert.AreEqual(root.Width, 128);
			Assert.AreEqual(root.Height, 114);
			Assert.AreEqual(A.Left, 8);
			Assert.AreEqual(A.Top, 8);
			Assert.AreEqual(A.Width, 100);
			Assert.AreEqual(A.Height, 30);
			Assert.AreEqual(B.Left, 8);
			Assert.AreEqual(B.Top, 46);
			Assert.AreEqual(B.Width, 120);
			Assert.AreEqual(B.Height, 60);
		}

        [Test]
        public void SimpleWrapConstraint3Test()
        {
			ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 800);
			root.Measurer = sMeasurer;
			ConstraintWidget A = new ConstraintWidget(100, 30);

			root.DebugName = "root";
			A.DebugName = "A";

			root.add(A);

			A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;

			A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 8);
			A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM, 8);
			A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 8);
			A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 8);

			root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
			root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;

			root.measure(Optimizer.OPTIMIZATION_STANDARD, BasicMeasure.WRAP_CONTENT, 0, BasicMeasure.WRAP_CONTENT, 0, 0, 0, 0, 0);

			Console.WriteLine("0) root: " + root);
			Console.WriteLine("1) A: " + A);
			Assert.AreEqual(root.Width, 116);
			Assert.AreEqual(root.Height, 46);
			Assert.AreEqual(A.Left, 8);
			Assert.AreEqual(A.Top, 8);
			Assert.AreEqual(A.Width, 100);
			Assert.AreEqual(A.Height, 30);

			root.measure(Optimizer.OPTIMIZATION_GRAPH, BasicMeasure.WRAP_CONTENT, 0, BasicMeasure.WRAP_CONTENT, 0, 0, 0, 0, 0);
			Console.WriteLine("0) root: " + root);
			Console.WriteLine("1) A: " + A);
			Assert.AreEqual(root.Width, 116);
			Assert.AreEqual(root.Height, 46);
			Assert.AreEqual(A.Left, 8);
			Assert.AreEqual(A.Top, 8);
			Assert.AreEqual(A.Width, 100);
			Assert.AreEqual(A.Height, 30);
		}

        [Test]
        public void SimpleWrapConstraint4Test()
        {
			ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 800);
			root.Measurer = sMeasurer;
			ConstraintWidget A = new ConstraintWidget(100, 30);
			ConstraintWidget B = new ConstraintWidget(100, 30);
			ConstraintWidget C = new ConstraintWidget(100, 30);
			ConstraintWidget D = new ConstraintWidget(100, 30);

			root.DebugName = "root";
			A.DebugName = "A";
			B.DebugName = "B";
			C.DebugName = "C";
			D.DebugName = "D";

			root.add(A);
			root.add(B);
			root.add(C);
			root.add(D);

			A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			C.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			C.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			D.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			D.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;

			A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 8);
			A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 8);

			B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM, 8);
			B.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 8);
			B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 8);

			C.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP, 8);
			C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT, 8);

			D.connect(ConstraintAnchor.Type.BOTTOM, C, ConstraintAnchor.Type.TOP, 8);
			D.connect(ConstraintAnchor.Type.LEFT, C, ConstraintAnchor.Type.RIGHT, 8);

			root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
			root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;


			root.measure(Optimizer.OPTIMIZATION_STANDARD, BasicMeasure.WRAP_CONTENT, 0, BasicMeasure.WRAP_CONTENT, 0, 0, 0, 0, 0);

			Console.WriteLine("0) root: " + root);
			Console.WriteLine("1) A: " + A);
			Console.WriteLine("2) B: " + B);
			Console.WriteLine("3) C: " + C);
			Console.WriteLine("4) D: " + D);
			Assert.AreEqual(root.Width, 532);
			Assert.AreEqual(root.Height, 76);
			Assert.AreEqual(A.Left, 8);
			Assert.AreEqual(A.Top, 8);
			Assert.AreEqual(A.Width, 100);
			Assert.AreEqual(A.Height, 30);
			Assert.AreEqual(B.Left, 216);
			Assert.AreEqual(B.Top, 46);
			Assert.AreEqual(B.Width, 100);
			Assert.AreEqual(B.Height, 30);
			Assert.AreEqual(C.Left, 324);
			Assert.AreEqual(C.Top, 8);
			Assert.AreEqual(C.Width, 100);
			Assert.AreEqual(C.Height, 30);
			Assert.AreEqual(D.Left, 432);
			Assert.AreEqual(D.Top, -28, 2);
			Assert.AreEqual(D.Width, 100);
			Assert.AreEqual(D.Height, 30);

			root.measure(Optimizer.OPTIMIZATION_GRAPH, BasicMeasure.WRAP_CONTENT, 0, BasicMeasure.WRAP_CONTENT, 0, 0, 0, 0, 0);
			Console.WriteLine("0) root: " + root);
			Console.WriteLine("1) A: " + A);
			Console.WriteLine("2) B: " + B);
			Console.WriteLine("3) C: " + C);
			Console.WriteLine("4) D: " + D);
			Assert.AreEqual(root.Width, 532);
			Assert.AreEqual(root.Height, 76);
			Assert.AreEqual(A.Left, 8);
			Assert.AreEqual(A.Top, 8);
			Assert.AreEqual(A.Width, 100);
			Assert.AreEqual(A.Height, 30);
			Assert.AreEqual(B.Left, 216);
			Assert.AreEqual(B.Top, 46);
			Assert.AreEqual(B.Width, 100);
			Assert.AreEqual(B.Height, 30);
			Assert.AreEqual(C.Left, 324);
			Assert.AreEqual(C.Top, 8);
			Assert.AreEqual(C.Width, 100);
			Assert.AreEqual(C.Height, 30);
			Assert.AreEqual(D.Left, 432);
			Assert.AreEqual(D.Top, -28, 2);
			Assert.AreEqual(D.Width, 100);
			Assert.AreEqual(D.Height, 30);
		}

        [Test]
        public void SimpleWrapConstraint5Test()
        {
			ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 800);
			root.Measurer = sMeasurer;
			ConstraintWidget A = new ConstraintWidget(100, 30);
			ConstraintWidget B = new ConstraintWidget(100, 30);
			ConstraintWidget C = new ConstraintWidget(100, 30);
			ConstraintWidget D = new ConstraintWidget(100, 30);

			root.DebugName = "root";
			A.DebugName = "A";
			B.DebugName = "B";
			C.DebugName = "C";
			D.DebugName = "D";

			root.add(A);
			root.add(B);
			root.add(C);
			root.add(D);

			A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			C.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			C.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			D.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			D.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;

			A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 8);
			A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 8);

			B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM, 8);
			B.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 8);
			B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 8);
			B.HorizontalBiasPercent = 0.2f;

			C.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP, 8);
			C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT, 8);

			D.connect(ConstraintAnchor.Type.BOTTOM, C, ConstraintAnchor.Type.TOP, 8);
			D.connect(ConstraintAnchor.Type.LEFT, C, ConstraintAnchor.Type.RIGHT, 8);

			root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
			root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;


			root.measure(Optimizer.OPTIMIZATION_STANDARD, BasicMeasure.WRAP_CONTENT, 0, BasicMeasure.WRAP_CONTENT, 0, 0, 0, 0, 0);

			Console.WriteLine("0) root: " + root);
			Console.WriteLine("1) A: " + A);
			Console.WriteLine("2) B: " + B);
			Console.WriteLine("3) C: " + C);
			Console.WriteLine("4) D: " + D);
			Assert.AreEqual(root.Width, 376);
			Assert.AreEqual(root.Height, 76);
			Assert.AreEqual(A.Left, 8);
			Assert.AreEqual(A.Top, 8);
			Assert.AreEqual(A.Width, 100);
			Assert.AreEqual(A.Height, 30);
			Assert.AreEqual(B.Left, 60);
			Assert.AreEqual(B.Top, 46);
			Assert.AreEqual(B.Width, 100);
			Assert.AreEqual(B.Height, 30);
			Assert.AreEqual(C.Left, 168);
			Assert.AreEqual(C.Top, 8);
			Assert.AreEqual(C.Width, 100);
			Assert.AreEqual(C.Height, 30);
			Assert.AreEqual(D.Left, 276);
			Assert.AreEqual(D.Top, -28, 2);
			Assert.AreEqual(D.Width, 100);
			Assert.AreEqual(D.Height, 30);

			root.measure(Optimizer.OPTIMIZATION_GRAPH, BasicMeasure.WRAP_CONTENT, 0, BasicMeasure.WRAP_CONTENT, 0, 0, 0, 0, 0);
			Console.WriteLine("0) root: " + root);
			Console.WriteLine("1) A: " + A);
			Console.WriteLine("2) B: " + B);
			Console.WriteLine("3) C: " + C);
			Console.WriteLine("4) D: " + D);
			Assert.AreEqual(root.Width, 376);
			Assert.AreEqual(root.Height, 76);
			Assert.AreEqual(A.Left, 8);
			Assert.AreEqual(A.Top, 8);
			Assert.AreEqual(A.Width, 100);
			Assert.AreEqual(A.Height, 30);
			Assert.AreEqual(B.Left, 60);
			Assert.AreEqual(B.Top, 46);
			Assert.AreEqual(B.Width, 100);
			Assert.AreEqual(B.Height, 30);
			Assert.AreEqual(C.Left, 168);
			Assert.AreEqual(C.Top, 8);
			Assert.AreEqual(C.Width, 100);
			Assert.AreEqual(C.Height, 30);
			Assert.AreEqual(D.Left, 276);
			Assert.AreEqual(D.Top, -28, 2);
			Assert.AreEqual(D.Width, 100);
			Assert.AreEqual(D.Height, 30);
		}

        [Test]
        public void SimpleWrapConstraint6Test()
        {
			ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 800);
			root.Measurer = sMeasurer;
			ConstraintWidget A = new ConstraintWidget(100, 30);
			ConstraintWidget B = new ConstraintWidget(100, 30);
			ConstraintWidget C = new ConstraintWidget(100, 30);
			ConstraintWidget D = new ConstraintWidget(100, 30);

			root.DebugName = "root";
			A.DebugName = "A";
			B.DebugName = "B";
			C.DebugName = "C";
			D.DebugName = "D";

			root.add(A);
			root.add(B);
			root.add(C);
			root.add(D);

			A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			C.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			C.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			D.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			D.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;

			A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 8);
			A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 8);

			B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM, 8);
			B.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 33);
			B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 16);
			B.HorizontalBiasPercent = 0.15f;

			C.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP, 8);
			C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT, 12);

			D.connect(ConstraintAnchor.Type.BOTTOM, C, ConstraintAnchor.Type.TOP, 8);
			D.connect(ConstraintAnchor.Type.LEFT, C, ConstraintAnchor.Type.RIGHT, 8);

			root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
			root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;


			root.measure(Optimizer.OPTIMIZATION_STANDARD, BasicMeasure.WRAP_CONTENT, 0, BasicMeasure.WRAP_CONTENT, 0, 0, 0, 0, 0);

			Console.WriteLine("0) root: " + root);
			Console.WriteLine("1) A: " + A);
			Console.WriteLine("2) B: " + B);
			Console.WriteLine("3) C: " + C);
			Console.WriteLine("4) D: " + D);
			Assert.AreEqual(root.Width, 389);
			Assert.AreEqual(root.Height, 76);
			Assert.AreEqual(A.Left, 8);
			Assert.AreEqual(A.Top, 8);
			Assert.AreEqual(A.Width, 100);
			Assert.AreEqual(A.Height, 30);
			Assert.AreEqual(B.Left, 69);
			Assert.AreEqual(B.Top, 46);
			Assert.AreEqual(B.Width, 100);
			Assert.AreEqual(B.Height, 30);
			Assert.AreEqual(C.Left, 181);
			Assert.AreEqual(C.Top, 8);
			Assert.AreEqual(C.Width, 100);
			Assert.AreEqual(C.Height, 30);
			Assert.AreEqual(D.Left, 289);
			Assert.AreEqual(D.Top, -28, 2);
			Assert.AreEqual(D.Width, 100);
			Assert.AreEqual(D.Height, 30);

			root.measure(Optimizer.OPTIMIZATION_GRAPH, BasicMeasure.WRAP_CONTENT, 0, BasicMeasure.WRAP_CONTENT, 0, 0, 0, 0, 0);
			Console.WriteLine("0) root: " + root);
			Console.WriteLine("1) A: " + A);
			Console.WriteLine("2) B: " + B);
			Console.WriteLine("3) C: " + C);
			Console.WriteLine("4) D: " + D);
			Assert.AreEqual(root.Width, 389);
			Assert.AreEqual(root.Height, 76);
			Assert.AreEqual(A.Left, 8);
			Assert.AreEqual(A.Top, 8);
			Assert.AreEqual(A.Width, 100);
			Assert.AreEqual(A.Height, 30);
			Assert.AreEqual(B.Left, 69);
			Assert.AreEqual(B.Top, 46);
			Assert.AreEqual(B.Width, 100);
			Assert.AreEqual(B.Height, 30);
			Assert.AreEqual(C.Left, 181);
			Assert.AreEqual(C.Top, 8);
			Assert.AreEqual(C.Width, 100);
			Assert.AreEqual(C.Height, 30);
			Assert.AreEqual(D.Left, 289);
			Assert.AreEqual(D.Top, -28, 2);
			Assert.AreEqual(D.Width, 100);
			Assert.AreEqual(D.Height, 30);
		}

        [Test]
        public void SimpleWrapConstraint7Test()
        {

			ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 800);
			root.Measurer = sMeasurer;
			ConstraintWidget A = new ConstraintWidget(100, 30);

			root.DebugName = "root";
			A.DebugName = "A";

			root.add(A);

			A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
			A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;

			A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 8);
			A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 8);
			A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 8);

			root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
			root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;

			root.measure(Optimizer.OPTIMIZATION_STANDARD, BasicMeasure.WRAP_CONTENT, 0, BasicMeasure.WRAP_CONTENT, 0, 0, 0, 0, 0);

			Console.WriteLine("0) root: " + root);
			Console.WriteLine("1) A: " + A);
			Assert.AreEqual(root.Width, 16);
			Assert.AreEqual(root.Height, 38);
			Assert.AreEqual(A.Left, 8);
			Assert.AreEqual(A.Top, 8);
			Assert.AreEqual(A.Width, 0);
			Assert.AreEqual(A.Height, 30);

			root.measure(Optimizer.OPTIMIZATION_GRAPH, BasicMeasure.WRAP_CONTENT, 0, BasicMeasure.WRAP_CONTENT, 0, 0, 0, 0, 0);
			Console.WriteLine("0) root: " + root);
			Console.WriteLine("1) A: " + A);
			Assert.AreEqual(root.Width, 16);
			Assert.AreEqual(root.Height, 38);
			Assert.AreEqual(A.Left, 8);
			Assert.AreEqual(A.Top, 8);
			Assert.AreEqual(A.Width, 0);
			Assert.AreEqual(A.Height, 30);
		}

        [Test]
        public void SimpleWrapConstraint8Test()
        {
			ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 800);
			root.Measurer = sMeasurer;
			ConstraintWidget A = new ConstraintWidget(100, 30);
			ConstraintWidget B = new ConstraintWidget(10, 30);
			ConstraintWidget C = new ConstraintWidget(10, 30);
			ConstraintWidget D = new ConstraintWidget(100, 30);

			root.DebugName = "root";
			A.DebugName = "A";
			B.DebugName = "B";
			C.DebugName = "C";
			D.DebugName = "D";

			root.add(A);
			root.add(B);
			root.add(C);
			root.add(D);

			A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
			C.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			D.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;

			A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			C.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			D.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;

			applyChain(ConstraintWidget.HORIZONTAL, A, B, C, D);
			A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
			D.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);

			root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
			root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;

			root.measure(Optimizer.OPTIMIZATION_STANDARD, BasicMeasure.WRAP_CONTENT, 0, BasicMeasure.WRAP_CONTENT, 0, 0, 0, 0, 0);

			Console.WriteLine("0) root: " + root);
			Console.WriteLine("1) A: " + A);
			Console.WriteLine("2) B: " + B);
			Console.WriteLine("3) C: " + C);
			Console.WriteLine("4) D: " + D);
			Assert.AreEqual(root.Width, 110);
			Assert.AreEqual(root.Height, 30);

			root.measure(Optimizer.OPTIMIZATION_GRAPH, BasicMeasure.WRAP_CONTENT, 0, BasicMeasure.WRAP_CONTENT, 0, 0, 0, 0, 0);
			Console.WriteLine("0) root: " + root);
			Console.WriteLine("1) A: " + A);
			Console.WriteLine("2) B: " + B);
			Console.WriteLine("3) C: " + C);
			Console.WriteLine("4) D: " + D);
			Assert.AreEqual(root.Width, 110);
			Assert.AreEqual(root.Height, 30);

		}

        [Test]
        public void SimpleCircleConstraintTest()
        {
			ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 800);
			root.Measurer = sMeasurer;
			ConstraintWidget A = new ConstraintWidget(100, 30);
			ConstraintWidget B = new ConstraintWidget(100, 30);

			root.DebugName = "root";
			A.DebugName = "A";
			B.DebugName = "B";

			root.add(A);
			root.add(B);

			A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;

			A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP, 8);
			A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 8);
			B.connectCircularConstraint(A, 30, 50);

			root.OptimizationLevel = Optimizer.OPTIMIZATION_GRAPH;
			root.measure(Optimizer.OPTIMIZATION_GRAPH, BasicMeasure.EXACTLY, 600, BasicMeasure.EXACTLY, 800, 0, 0, 0, 0);
			//        root.layout();

			Console.WriteLine("1) A: " + A);
			Console.WriteLine("2) B: " + B);
		}

		public virtual void applyChain(List<ConstraintWidget> widgets, int direction)
		{
			ConstraintWidget previous = widgets[0];
			for (int i = 1; i < widgets.Count; i++)
			{
				ConstraintWidget widget = widgets[i];
				if (direction == 0)
				{ // horizontal
					widget.connect(ConstraintAnchor.Type.LEFT, previous, ConstraintAnchor.Type.RIGHT);
					previous.connect(ConstraintAnchor.Type.RIGHT, widget, ConstraintAnchor.Type.LEFT);
				}
				else
				{
					widget.connect(ConstraintAnchor.Type.TOP, previous, ConstraintAnchor.Type.BOTTOM);
					previous.connect(ConstraintAnchor.Type.BOTTOM, widget, ConstraintAnchor.Type.TOP);
				}
				previous = widget;
			}
		}

		public virtual void applyChain(int direction, params ConstraintWidget[] widgets)
		{
			ConstraintWidget previous = widgets[0];
			for (int i = 1; i < widgets.Length; i++)
			{
				ConstraintWidget widget = widgets[i];
				if (direction == ConstraintWidget.HORIZONTAL)
				{
					widget.connect(ConstraintAnchor.Type.LEFT, previous, ConstraintAnchor.Type.RIGHT);
					previous.connect(ConstraintAnchor.Type.RIGHT, widget, ConstraintAnchor.Type.LEFT);
				}
				else
				{
					widget.connect(ConstraintAnchor.Type.TOP, previous, ConstraintAnchor.Type.BOTTOM);
					previous.connect(ConstraintAnchor.Type.BOTTOM, widget, ConstraintAnchor.Type.TOP);
				}
				previous = widget;
			}
		}

        [Test]
        public void RatioChainConstraintTest()
        {
			ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 800);
			root.Measurer = sMeasurer;
			ConstraintWidget A = new ConstraintWidget(100, 30);
			ConstraintWidget B = new ConstraintWidget(0, 30);
			ConstraintWidget C = new ConstraintWidget(0, 30);
			ConstraintWidget D = new ConstraintWidget(100, 30);

			root.DebugName = "root";
			A.DebugName = "A";
			B.DebugName = "B";

			root.add(A);
			root.add(B);
			root.add(C);
			root.add(D);

			A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
			C.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
			D.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;

			B.setDimensionRatio("w,1:1");

			A.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			C.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			D.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;

			applyChain(ConstraintWidget.HORIZONTAL, A, B, C, D);
			A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
			D.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);

			root.OptimizationLevel = Optimizer.OPTIMIZATION_GRAPH;
			root.measure(Optimizer.OPTIMIZATION_GRAPH, BasicMeasure.EXACTLY, 600, BasicMeasure.EXACTLY, 800, 0, 0, 0, 0);
			//        root.layout();

			Console.WriteLine("1) A: " + A);
			Console.WriteLine("2) B: " + B);
			Console.WriteLine("3) C: " + C);
			Console.WriteLine("4) D: " + D);
		}

        [Test]
        public void CycleConstraintsTest()
        {
			ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 800);
			root.Measurer = sMeasurer;
			ConstraintWidget A = new ConstraintWidget(100, 30);
			ConstraintWidget B = new ConstraintWidget(40, 20);
			ConstraintWidget C = new ConstraintWidget(40, 20);
			ConstraintWidget D = new ConstraintWidget(30, 30);

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

			B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
			B.connect(ConstraintAnchor.Type.LEFT, C, ConstraintAnchor.Type.LEFT);

			C.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.BOTTOM);
			C.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
			C.connect(ConstraintAnchor.Type.LEFT, D, ConstraintAnchor.Type.RIGHT);

			D.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.TOP);
			D.connect(ConstraintAnchor.Type.BOTTOM, C, ConstraintAnchor.Type.BOTTOM);
			D.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);

			root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
			root.measure(Optimizer.OPTIMIZATION_NONE, BasicMeasure.EXACTLY, 600, BasicMeasure.EXACTLY, 800, 0, 0, 0, 0);

			Console.WriteLine("1) A: " + A);
			Console.WriteLine("2) B: " + B);
			Console.WriteLine("3) C: " + C);
			Console.WriteLine("4) D: " + D);

			Assert.AreEqual(A.Top, 0);
			Assert.AreEqual(B.Top, 30);
			Assert.AreEqual(C.Top, 50);
			Assert.AreEqual(D.Top, 35);
		}

        [Test]
        public void GoneChainTest()
        {
			ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 800);
			root.Measurer = sMeasurer;
			ConstraintWidget A = new ConstraintWidget(100, 30);
			ConstraintWidget B = new ConstraintWidget(100, 30);
			ConstraintWidget C = new ConstraintWidget(100, 30);
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
			A.Visibility = ConstraintWidget.GONE;
			C.Visibility = ConstraintWidget.GONE;

			root.measure(Optimizer.OPTIMIZATION_NONE, BasicMeasure.EXACTLY, 600, BasicMeasure.EXACTLY, 800, 0, 0, 0, 0);

			Console.WriteLine("0) root: " + root);
			Console.WriteLine("1) A: " + A);
			Console.WriteLine("2) B: " + B);
			Console.WriteLine("3) C: " + C);

			Assert.AreEqual(B.Width, root.Width);
		}

        [Test]
        public void GoneChainWithCenterWidgetTest()
        {
			ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 800);
			root.Measurer = sMeasurer;
			ConstraintWidget A = new ConstraintWidget(100, 30);
			ConstraintWidget B = new ConstraintWidget(100, 30);
			ConstraintWidget C = new ConstraintWidget(100, 30);
			ConstraintWidget D = new ConstraintWidget(100, 30);
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
			C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
			B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
			A.Visibility = ConstraintWidget.GONE;
			C.Visibility = ConstraintWidget.GONE;
			D.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.LEFT);
			D.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.RIGHT);
			D.Visibility = ConstraintWidget.GONE;

			root.measure(Optimizer.OPTIMIZATION_NONE, BasicMeasure.EXACTLY, 600, BasicMeasure.EXACTLY, 800, 0, 0, 0, 0);

			Console.WriteLine("0) root: " + root);
			Console.WriteLine("1) A: " + A);
			Console.WriteLine("2) B: " + B);
			Console.WriteLine("3) C: " + C);
			Console.WriteLine("4) D: " + D);

			Assert.AreEqual(B.Width, root.Width);
		}

        [Test]
        public void BarrierTest()
        {
			ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 800);
			root.Measurer = sMeasurer;
			ConstraintWidget A = new ConstraintWidget(100, 30);
			ConstraintWidget B = new ConstraintWidget(100, 30);
			ConstraintWidget C = new ConstraintWidget(100, 30);
			ConstraintWidget D = new ConstraintWidget(100, 30);
			Barrier barrier1 = new Barrier();
			//Barrier barrier2 = new Barrier();
			root.DebugName = "root";
			A.DebugName = "A";
			B.DebugName = "B";
			C.DebugName = "C";
			D.DebugName = "D";
			barrier1.DebugName = "barrier1";
			//barrier2.setDebugName("barrier2");
			root.add(A);
			root.add(B);
			root.add(C);
			root.add(D);
			root.add(barrier1);
			//root.add(barrier2);

			A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
			barrier1.add(A);
			barrier1.BarrierType = Barrier.BOTTOM;

			B.connect(ConstraintAnchor.Type.TOP, barrier1, ConstraintAnchor.Type.BOTTOM);
			//barrier2.add(B);
			//barrier2.setBarrierType(Barrier.TOP);

			C.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.BOTTOM);
			D.connect(ConstraintAnchor.Type.TOP, C, ConstraintAnchor.Type.BOTTOM);
			D.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);

			root.measure(Optimizer.OPTIMIZATION_NONE, BasicMeasure.EXACTLY, 600, BasicMeasure.EXACTLY, 800, 0, 0, 0, 0);

			Console.WriteLine("0) root: " + root);
			Console.WriteLine("1) A: " + A);
			Console.WriteLine("2) barrier1: " + barrier1);
			Console.WriteLine("3) B: " + B);
			//System.out.println("4) barrier2: " + barrier2);
			Console.WriteLine("5) C: " + C);
			Console.WriteLine("6) D: " + D);

			Assert.AreEqual(A.Top, 0);
			Assert.AreEqual(B.Top, A.Bottom);
			Assert.AreEqual(barrier1.Top, A.Bottom);
			Assert.AreEqual(C.Top, B.Bottom);
			Assert.AreEqual(D.Top, 430);
			//        Assert.AreEqual(barrier2.getTop(), B.getTop());
		}

        [Test]
        public void DirectCenteringTest()
        {
			ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 192, 168);
			root.Measurer = sMeasurer;
			ConstraintWidget A = new ConstraintWidget(43, 43);
			ConstraintWidget B = new ConstraintWidget(59, 59);
			root.DebugName = "root";
			A.DebugName = "A";
			B.DebugName = "B";
			root.add(B);
			root.add(A);

			B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.TOP);
			B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.LEFT);
			B.connect(ConstraintAnchor.Type.RIGHT, A, ConstraintAnchor.Type.RIGHT);
			B.connect(ConstraintAnchor.Type.BOTTOM, A, ConstraintAnchor.Type.BOTTOM);

			A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
			A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
			A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
			A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);

			root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
			root.measure(Optimizer.OPTIMIZATION_NONE, BasicMeasure.EXACTLY, 100, BasicMeasure.EXACTLY, 100, 0, 0, 0, 0);

			Console.WriteLine("0) root: " + root);
			Console.WriteLine("1) A: " + A);
			Console.WriteLine("2) B: " + B);
			Assert.AreEqual(A.Top, 63);
			Assert.AreEqual(A.Left, 75);
			Assert.AreEqual(B.Top, 55);
			Assert.AreEqual(B.Left, 67);

			root.OptimizationLevel = Optimizer.OPTIMIZATION_STANDARD;
			root.measure(Optimizer.OPTIMIZATION_STANDARD, BasicMeasure.EXACTLY, 100, BasicMeasure.EXACTLY, 100, 0, 0, 0, 0);

			Console.WriteLine("0) root: " + root);
			Console.WriteLine("1) A: " + A);
			Console.WriteLine("2) B: " + B);
			Assert.AreEqual(63, A.Top);
			Assert.AreEqual(75, A.Left);
			Assert.AreEqual(55, B.Top);
			Assert.AreEqual(67, B.Left);
		}
	}
}

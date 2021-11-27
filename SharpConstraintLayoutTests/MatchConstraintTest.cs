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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
[TestFixture]
	public class MatchConstraintTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSimpleMinMatch()
[Test]
		public virtual void testSimpleMinMatch()
		{
			ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
			ConstraintWidget A = new ConstraintWidget(100, 20);
			ConstraintWidget B = new ConstraintWidget(100, 20);
			A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
			A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
			B.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
			B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
			A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
			A.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_SPREAD, 150, 200, 1);
			root.add(A);
			root.add(B);
			root.DebugName = "root";
			A.DebugName = "A";
			B.DebugName = "B";
			root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
			root.layout();
			Console.WriteLine("a) root: " + root + " A: " + A + " B: " + B);
			Assert.AreEqual(A.Width, 150);
			Assert.AreEqual(B.Width, 100);
			Assert.AreEqual(root.Width, 150);
			B.Width = 200;
			root.Width = 0;
			root.layout();
			Console.WriteLine("b) root: " + root + " A: " + A + " B: " + B);
			Assert.AreEqual(A.Width, 200);
			Assert.AreEqual(B.Width, 200);
			Assert.AreEqual(root.Width, 200);
			B.Width = 300;
			root.Width = 0;
			root.layout();
			Console.WriteLine("c) root: " + root + " A: " + A + " B: " + B);
			Assert.AreEqual(A.Width, 200);
			Assert.AreEqual(B.Width, 300);
			Assert.AreEqual(root.Width, 300);
		}

		//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		//ORIGINAL LINE: @Test public void testMinMaxMatch()
		[Test]
		public virtual void testMinMaxMatch()
		{
			ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
			Guideline guidelineA = new Guideline();
			guidelineA.Orientation = Guideline.VERTICAL;
			guidelineA.GuideBegin = 100;
			Guideline guidelineB = new Guideline();
			guidelineB.Orientation = Guideline.VERTICAL;
			guidelineB.GuideEnd = 100;
			root.add(guidelineA);
			root.add(guidelineB);
			ConstraintWidget A = new ConstraintWidget(100, 20);
			A.connect(ConstraintAnchor.Type.LEFT, guidelineA, ConstraintAnchor.Type.LEFT);
			A.connect(ConstraintAnchor.Type.RIGHT, guidelineB, ConstraintAnchor.Type.RIGHT);
			A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
			A.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_SPREAD, 150, 200, 1);
			root.add(A);
			root.DebugName = "root";
			guidelineA.DebugName = "guideline A";
			guidelineB.DebugName = "guideline B";
			A.DebugName = "A";
			root.layout();
			Console.WriteLine("a) root: " + root + " guideA: " + guidelineA + " A: " + A + " guideB: " + guidelineB);
			Assert.AreEqual(root.Width, 800);
			Assert.AreEqual(A.Width, 200);
			root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
			A.Width = 100;
			root.layout();
			Console.WriteLine("b) root: " + root + " guideA: " + guidelineA + " A: " + A + " guideB: " + guidelineB);
			Assert.AreEqual(root.Width, 350);
			Assert.AreEqual(A.Width, 150);

			A.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_WRAP, 150, 200, 1);
			root.layout();
			Console.WriteLine("c) root: " + root + " guideA: " + guidelineA + " A: " + A + " guideB: " + guidelineB);
			Assert.AreEqual(root.Width, 350);
			Assert.AreEqual(A.Width, 150);
			root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			root.Width = 800;
			root.layout();
			Console.WriteLine("d) root: " + root + " guideA: " + guidelineA + " A: " + A + " guideB: " + guidelineB);
			Assert.AreEqual(root.Width, 800);
			Assert.AreEqual(A.Width, 150); // because it's wrap
			A.Width = 250;
			root.layout();
			Console.WriteLine("e) root: " + root + " guideA: " + guidelineA + " A: " + A + " guideB: " + guidelineB);
			Assert.AreEqual(root.Width, 800);
			Assert.AreEqual(A.Width, 200);

			A.Width = 700;
			A.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_SPREAD, 150, 0, 1);
			root.layout();
			Console.WriteLine("f) root: " + root + " guideA: " + guidelineA + " A: " + A + " guideB: " + guidelineB);
			Assert.AreEqual(root.Width, 800);
			Assert.AreEqual(A.Width, 600);
			A.Width = 700;
			A.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_WRAP, 150, 0, 1);
			root.layout();
			Console.WriteLine("g) root: " + root + " guideA: " + guidelineA + " A: " + A + " guideB: " + guidelineB);
			Assert.AreEqual(root.Width, 800);
			Assert.AreEqual(A.Width, 600);

			A.Width = 700;
			root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
			root.Width = 0;
			root.layout();
			Console.WriteLine("h) root: " + root + " guideA: " + guidelineA + " A: " + A + " guideB: " + guidelineB);
			Assert.AreEqual(root.Width, 900);
			Assert.AreEqual(A.Width, 700);
			A.Width = 700;
			A.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_SPREAD, 150, 0, 1);
			root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
			root.layout();
			Assert.AreEqual(root.Width, 350);
			Assert.AreEqual(A.Width, 150);
			Console.WriteLine("i) root: " + root + " guideA: " + guidelineA + " A: " + A + " guideB: " + guidelineB);
		}

		//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		//ORIGINAL LINE: @Test public void testSimpleHorizontalMatch()
		[Test]
		public virtual void testSimpleHorizontalMatch()
		{
			ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
			ConstraintWidget A = new ConstraintWidget(100, 20);
			ConstraintWidget B = new ConstraintWidget(100, 20);
			ConstraintWidget C = new ConstraintWidget(100, 20);

			root.setDebugSolverName(root.System, "root");
			A.setDebugSolverName(root.System, "A");
			B.setDebugSolverName(root.System, "B");
			C.setDebugSolverName(root.System, "C");

			A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 0);
			B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 0);
			C.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT, 0);
			C.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT, 0);

			root.add(A);
			root.add(B);
			root.add(C);

			root.layout();
			Console.WriteLine("a) A: " + A + " B: " + B + " C: " + C);
			Assert.AreEqual(A.Width, 100);
			Assert.AreEqual(B.Width, 100);
			Assert.AreEqual(C.Width, 100);
			Assert.True(C.Left >= A.Right);
			Assert.True(C.Right <= B.Left);
			Assert.AreEqual(C.Left - A.Right, B.Left - C.Right);

			C.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
			root.layout();
			Console.WriteLine("b) A: " + A + " B: " + B + " C: " + C);

			Assert.AreEqual(A.Width, 100);
			Assert.AreEqual(B.Width, 100);
			Assert.AreEqual(C.Width, 600);
			Assert.True(C.Left >= A.Right);
			Assert.True(C.Right <= B.Left);
			Assert.AreEqual(C.Left - A.Right, B.Left - C.Right);

			C.Width = 144;
			C.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_WRAP, 0, 0, 0);
			root.layout();
			Console.WriteLine("c) A: " + A + " B: " + B + " C: " + C);
			Assert.AreEqual(A.Width, 100);
			Assert.AreEqual(B.Width, 100);
			Assert.AreEqual(C.Width, 144);
			Assert.True(C.Left >= A.Right);
			Assert.True(C.Right <= B.Left);
			Assert.AreEqual(C.Left - A.Right, B.Left - C.Right);

			C.Width = 1000;
			C.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_WRAP, 0, 0, 0);
			root.layout();
			Console.WriteLine("d) A: " + A + " B: " + B + " C: " + C);
			Assert.AreEqual(A.Width, 100);
			Assert.AreEqual(B.Width, 100);
			Assert.AreEqual(C.Width, 600);
			Assert.True(C.Left >= A.Right);
			Assert.True(C.Right <= B.Left);
			Assert.AreEqual(C.Left - A.Right, B.Left - C.Right);
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
			A.setHorizontalMatchStyle(ConstraintWidget.MATCH_CONSTRAINT_WRAP, 0, 0, 0);
			root.layout();
			Console.WriteLine("a) root: " + root + " A: " + A);
		}
	}

}
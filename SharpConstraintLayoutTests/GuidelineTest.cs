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
	using Guideline = androidx.constraintlayout.core.widgets.Guideline;
	//using Test = org.junit.Test;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.Assert.AreEqual;
[TestFixture]
	public class GuidelineTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWrapGuideline()
[Test]
		public virtual void testWrapGuideline()
		{
			ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
			ConstraintWidget A = new ConstraintWidget(100, 20);
			Guideline guidelineRight = new Guideline();
			guidelineRight.Orientation = Guideline.VERTICAL;
			Guideline guidelineBottom = new Guideline();
			guidelineBottom.Orientation = Guideline.HORIZONTAL;
			guidelineRight.setGuidePercent(0.64f);
			guidelineBottom.GuideEnd = 60;
			root.DebugName = "Root";
			A.DebugName = "A";
			guidelineRight.DebugName = "GuidelineRight";
			guidelineBottom.DebugName = "GuidelineBottom";
			A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
			A.connect(ConstraintAnchor.Type.RIGHT, guidelineRight, ConstraintAnchor.Type.RIGHT);
			A.connect(ConstraintAnchor.Type.BOTTOM, guidelineBottom, ConstraintAnchor.Type.TOP);
			root.add(A);
			root.add(guidelineRight);
			root.add(guidelineBottom);
			root.layout();
			Console.WriteLine("a) root: " + root + " guideline right: " + guidelineRight + " guideline bottom: " + guidelineBottom + " A: " + A);
			root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
			root.layout();
			Console.WriteLine("b) root: " + root + " guideline right: " + guidelineRight + " guideline bottom: " + guidelineBottom + " A: " + A);
			Assert.AreEqual(root.Height, 80);
		}

		//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		//ORIGINAL LINE: @Test public void testWrapGuideline2()
		[Test]
		public virtual void testWrapGuideline2()
		{
			ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 800, 600);
			ConstraintWidget A = new ConstraintWidget(100, 20);
			Guideline guideline = new Guideline();
			guideline.Orientation = Guideline.VERTICAL;
			guideline.GuideBegin = 60;
			root.DebugName = "Root";
			A.DebugName = "A";
			guideline.DebugName = "Guideline";
			A.connect(ConstraintAnchor.Type.LEFT, guideline, ConstraintAnchor.Type.LEFT, 5);
			A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 5);
			A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
			root.add(A);
			root.add(guideline);
	//        root.layout();
			Console.WriteLine("a) root: " + root + " guideline: " + guideline + " A: " + A);
			root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
			root.layout();
			Console.WriteLine("b) root: " + root + " guideline: " + guideline + " A: " + A);
			Assert.AreEqual(root.Width, 70);
		}
	}

}
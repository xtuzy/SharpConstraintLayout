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
	using Flow = androidx.constraintlayout.core.widgets.Flow;
	using Optimizer = androidx.constraintlayout.core.widgets.Optimizer;
	using VirtualLayout = androidx.constraintlayout.core.widgets.VirtualLayout;
	using BasicMeasure = androidx.constraintlayout.core.widgets.analyzer.BasicMeasure;

	//using Test = org.junit.Test;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.analyzer.BasicMeasure.EXACTLY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.analyzer.BasicMeasure.UNSPECIFIED;
using static androidx.constraintlayout.core.widgets.analyzer.BasicMeasure;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.Assert.AreEqual;
[TestFixture]
	public class FlowTest
	{
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

				if (widget is VirtualLayout)
				{
					VirtualLayout layout = (VirtualLayout) widget;
					int widthMode = UNSPECIFIED;
					int heightMode = UNSPECIFIED;
					int widthSize = 0;
					int heightSize = 0;
					if (layout.HorizontalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_PARENT)
					{
						widthSize = layout.Parent != null ? layout.Parent.Width : 0;
						widthMode = EXACTLY;
					}
					else if (horizontalBehavior == ConstraintWidget.DimensionBehaviour.FIXED)
					{
						widthSize = horizontalDimension;
						widthMode = EXACTLY;
					}
					if (layout.VerticalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_PARENT)
					{
						heightSize = layout.Parent != null ? layout.Parent.Height : 0;
						heightMode = EXACTLY;
					}
					else if (verticalBehavior == ConstraintWidget.DimensionBehaviour.FIXED)
					{
						heightSize = verticalDimension;
						heightMode = EXACTLY;
					}
					layout.measure(widthMode, widthSize, heightMode, heightSize);
					measure.measuredWidth = layout.MeasuredWidth;
					measure.measuredHeight = layout.MeasuredHeight;
				}
				else
				{
					if (horizontalBehavior == ConstraintWidget.DimensionBehaviour.FIXED)
					{
						measure.measuredWidth = horizontalDimension;
					}
					if (verticalBehavior == ConstraintWidget.DimensionBehaviour.FIXED)
					{
						measure.measuredHeight = verticalDimension;
					}
				}
			}

			public virtual void didMeasures()
			{

			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFlowBaseline()
[Test]
		public virtual void testFlowBaseline()
		{
			ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1080, 1536);
			ConstraintWidget A = new ConstraintWidget(100, 20);
			ConstraintWidget B = new ConstraintWidget(20, 15);
			Flow flow = new Flow();

			root.Measurer = sMeasurer;

			root.DebugName = "root";
			A.DebugName = "A";
			B.DebugName = "B";
			flow.DebugName = "Flow";

			flow.VerticalAlign = Flow.VERTICAL_ALIGN_BASELINE;
			flow.add(A);
			flow.add(B);
			A.BaselineDistance = 15;

			flow.Height = 30;
			flow.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
			flow.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			flow.connect(ConstraintAnchor.Type.LEFT,root, ConstraintAnchor.Type.LEFT);
			flow.connect(ConstraintAnchor.Type.RIGHT,root, ConstraintAnchor.Type.RIGHT);
			flow.connect(ConstraintAnchor.Type.TOP,root, ConstraintAnchor.Type.TOP);
			flow.connect(ConstraintAnchor.Type.BOTTOM,root, ConstraintAnchor.Type.BOTTOM);

			root.add(flow);
			root.add(A);
			root.add(B);

			root.measure(Optimizer.OPTIMIZATION_NONE, 0, 0, 0, 0, 0, 0, 0, 0);
			root.layout();
			Console.WriteLine("a) root: " + root);
			Console.WriteLine("flow: " + flow);
			Console.WriteLine("A: " + A);
			Console.WriteLine("B: " + B);
			Assert.AreEqual(flow.Width, 1080);
			Assert.AreEqual(flow.Height, 30);
			Assert.AreEqual(flow.Top, 753);
			Assert.AreEqual(A.Left, 320);
			Assert.AreEqual(A.Top, 758);
			Assert.AreEqual(B.Left, 740);
			Assert.AreEqual(B.Top, 761);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testComplexChain()
[Test]
		public virtual void testComplexChain()
		{
			ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1080, 1536);
			ConstraintWidget A = new ConstraintWidget(100, 20);
			ConstraintWidget B = new ConstraintWidget(100, 20);
			ConstraintWidget C = new ConstraintWidget(100, 20);
			Flow flow = new Flow();

			root.Measurer = sMeasurer;

			root.DebugName = "root";
			A.DebugName = "A";
			B.DebugName = "B";
			C.DebugName = "C";
			flow.DebugName = "Flow";

			flow.WrapMode = Flow.WRAP_CHAIN;
			flow.MaxElementsWrap = 2;

			flow.add(A);
			flow.add(B);
			flow.add(C);

			root.add(flow);
			root.add(A);
			root.add(B);
			root.add(C);

			A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
			B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
			C.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;

			flow.connect(ConstraintAnchor.Type.LEFT,root, ConstraintAnchor.Type.LEFT);
			flow.connect(ConstraintAnchor.Type.RIGHT,root, ConstraintAnchor.Type.RIGHT);
			flow.connect(ConstraintAnchor.Type.TOP,root, ConstraintAnchor.Type.TOP);
			flow.connect(ConstraintAnchor.Type.BOTTOM,root, ConstraintAnchor.Type.BOTTOM);

			flow.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_PARENT;
			flow.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;

			root.measure(Optimizer.OPTIMIZATION_NONE, 0, 0, 0, 0, 0, 0, 0, 0);
			root.layout();
			Console.WriteLine("a) root: " + root);
			Console.WriteLine("flow: " + flow);
			Console.WriteLine("A: " + A);
			Console.WriteLine("B: " + B);
			Console.WriteLine("C: " + C);

			Assert.AreEqual(A.Width, 540);
			Assert.AreEqual(B.Width, 540);
			Assert.AreEqual(C.Width, 1080);
			Assert.AreEqual(flow.Width, root.Width);
			Assert.AreEqual(flow.Height, Math.Max(A.Height, B.Height) + C.Height);
			Assert.AreEqual(flow.Top, 748);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFlowText()
[Test]
		public virtual void testFlowText()
		{
			ConstraintWidgetContainer root = new ConstraintWidgetContainer(20, 5);
			ConstraintWidget A = new ConstraintWidget(7, 1);
			ConstraintWidget B = new ConstraintWidget(6, 1);
			A.DebugName = "A";
			B.DebugName = "B";
			Flow flow = new Flow();
			flow.DebugName = "flow";
			flow.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
			flow.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
			flow.Width = 20;
			flow.Height = 2;
			flow.connect(ConstraintAnchor.Type.LEFT,root, ConstraintAnchor.Type.LEFT);
			flow.connect(ConstraintAnchor.Type.RIGHT,root, ConstraintAnchor.Type.RIGHT);
			flow.connect(ConstraintAnchor.Type.TOP,root, ConstraintAnchor.Type.TOP);
			flow.connect(ConstraintAnchor.Type.BOTTOM,root, ConstraintAnchor.Type.BOTTOM);
			flow.add(A);
			flow.add(B);
			root.add(flow);
			root.add(A);
			root.add(B);
			root.Measurer = new MeasurerAnonymousInnerClass2(this);
			root.Measurer = sMeasurer;
			root.measure(Optimizer.OPTIMIZATION_NONE, 0, 0, 0, 0, 0, 0, 0, 0);
			//root.layout();
			Console.WriteLine("root: " + root);
			Console.WriteLine("flow: " + flow);
			Console.WriteLine("A: " + A);
			Console.WriteLine("B: " + B);
		}

		private class MeasurerAnonymousInnerClass2 : BasicMeasure.Measurer
		{
			private readonly FlowTest outerInstance;

			public MeasurerAnonymousInnerClass2(FlowTest outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void measure(ConstraintWidget widget, BasicMeasure.Measure measure)
			{
				measure.measuredWidth = widget.Width;
				measure.measuredHeight = widget.Height;
			}

			public virtual void didMeasures()
			{

			}
		}
	}

}
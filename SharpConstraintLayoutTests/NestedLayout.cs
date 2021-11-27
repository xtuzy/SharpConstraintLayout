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

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.Assert.AreEqual;

	/// <summary>
	/// Test nested layout
	/// </summary>
	[TestFixture]
	public class NestedLayout
	{

		// @Test
		[Test]
		public virtual void testNestedLayout()
		{
			ConstraintWidgetContainer root = new ConstraintWidgetContainer(20, 20, 1000, 1000);
			ConstraintWidgetContainer container = new ConstraintWidgetContainer(0, 0, 100, 100);
			root.DebugName = "root";
			container.DebugName = "container";
			container.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
			container.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
			root.add(container);
			root.layout();
			Console.WriteLine("container: " + container);
			Assert.AreEqual(container.Left, 450);
			Assert.AreEqual(container.Width, 100);

			ConstraintWidget A = new ConstraintWidget(0, 0, 100, 20);
			ConstraintWidget B = new ConstraintWidget(0, 0, 50, 20);
			container.add(A);
			container.add(B);
			container.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
			A.connect(ConstraintAnchor.Type.LEFT, container, ConstraintAnchor.Type.LEFT);
			A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
			B.connect(ConstraintAnchor.Type.RIGHT, container, ConstraintAnchor.Type.RIGHT);
			root.layout();
			Console.WriteLine("container: " + container);
			Console.WriteLine("A: " + A);
			Console.WriteLine("B: " + B);
			Assert.AreEqual(container.Width, 150);
			Assert.AreEqual(container.Left, 425);
			Assert.AreEqual(A.Left, 425);
			Assert.AreEqual(B.Left, 525);
			Assert.AreEqual(A.Width, 100);
			Assert.AreEqual(B.Width, 50);
		}
	}

}
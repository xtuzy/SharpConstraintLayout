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
	public class CircleTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void basic()
[Test]
		public virtual void basic()
		{
			ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 600);
			ConstraintWidget A = new ConstraintWidget(100, 20);
			ConstraintWidget w1 = new ConstraintWidget(10, 10);
			ConstraintWidget w2 = new ConstraintWidget(10, 10);
			ConstraintWidget w3 = new ConstraintWidget(10, 10);
			ConstraintWidget w4 = new ConstraintWidget(10, 10);
			ConstraintWidget w5 = new ConstraintWidget(10, 10);
			ConstraintWidget w6 = new ConstraintWidget(10, 10);
			ConstraintWidget w7 = new ConstraintWidget(10, 10);
			ConstraintWidget w8 = new ConstraintWidget(10, 10);
			ConstraintWidget w9 = new ConstraintWidget(10, 10);
			ConstraintWidget w10 = new ConstraintWidget(10, 10);
			ConstraintWidget w11 = new ConstraintWidget(10, 10);
			ConstraintWidget w12 = new ConstraintWidget(10, 10);

			root.DebugName = "root";
			A.DebugName = "A";
			w1.DebugName = "w1";
			w2.DebugName = "w2";
			w3.DebugName = "w3";
			w4.DebugName = "w4";
			w5.DebugName = "w5";
			w6.DebugName = "w6";
			w7.DebugName = "w7";
			w8.DebugName = "w8";
			w9.DebugName = "w9";
			w10.DebugName = "w10";
			w11.DebugName = "w11";
			w12.DebugName = "w12";

			root.add(A);

			root.add(w1);
			root.add(w2);
			root.add(w3);
			root.add(w4);
			root.add(w5);
			root.add(w6);
			root.add(w7);
			root.add(w8);
			root.add(w9);
			root.add(w10);
			root.add(w11);
			root.add(w12);

			A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
			A.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
			A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
			A.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);

			w1.connectCircularConstraint(A, 30, 50);
			w2.connectCircularConstraint(A, 60, 50);
			w3.connectCircularConstraint(A, 90, 50);
			w4.connectCircularConstraint(A, 120, 50);
			w5.connectCircularConstraint(A, 150, 50);
			w6.connectCircularConstraint(A, 180, 50);
			w7.connectCircularConstraint(A, 210, 50);
			w8.connectCircularConstraint(A, 240, 50);
			w9.connectCircularConstraint(A, 270, 50);
			w10.connectCircularConstraint(A, 300, 50);
			w11.connectCircularConstraint(A, 330, 50);
			w12.connectCircularConstraint(A, 360, 50);

			root.layout();

			Console.WriteLine("w1: " + w1);
			Console.WriteLine("w2: " + w2);
			Console.WriteLine("w3: " + w3);
			Console.WriteLine("w4: " + w4);
			Console.WriteLine("w5: " + w5);
			Console.WriteLine("w6: " + w6);
			Console.WriteLine("w7: " + w7);
			Console.WriteLine("w8: " + w8);
			Console.WriteLine("w9: " + w9);
			Console.WriteLine("w10: " + w10);
			Console.WriteLine("w11: " + w11);
			Console.WriteLine("w12: " + w12);

			Assert.AreEqual(w1.Left, 520);
			Assert.AreEqual(w1.Top, 252);
			Assert.AreEqual(w2.Left, 538);
			Assert.AreEqual(w2.Top, 270);
			Assert.AreEqual(w3.Left, 545);
			Assert.AreEqual(w3.Top, 295);
			Assert.AreEqual(w4.Left, 538);
			Assert.AreEqual(w4.Top, 320);
			Assert.AreEqual(w5.Left, 520);
			Assert.AreEqual(w5.Top, 338);
			Assert.AreEqual(w6.Left, 495);
			Assert.AreEqual(w6.Top, 345);
			Assert.AreEqual(w7.Left, 470);
			Assert.AreEqual(w7.Top, 338);
			Assert.AreEqual(w8.Left, 452);
			Assert.AreEqual(w8.Top, 320);
			Assert.AreEqual(w9.Left, 445);
			Assert.AreEqual(w9.Top, 295);
			Assert.AreEqual(w10.Left, 452);
			Assert.AreEqual(w10.Top, 270);
			Assert.AreEqual(w11.Left, 470);
			Assert.AreEqual(w11.Top, 252);
			Assert.AreEqual(w12.Left, 495);
			Assert.AreEqual(w12.Top, 245);
		}
	}

}
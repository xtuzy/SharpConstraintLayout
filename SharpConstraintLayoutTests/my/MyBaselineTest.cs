using androidx.constraintlayout.core.widgets;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpConstraintLayoutTests.my
{
    [TestFixture]
    internal class MyBaselineTest
    {
        [Test]
        public virtual void testBaseline()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 600);
            ConstraintWidget A = new ConstraintWidget(100, 40);
            ConstraintWidget B = new ConstraintWidget(100, 30);
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
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.BASELINE, A, ConstraintAnchor.Type.BASELINE);
            C.connect(ConstraintAnchor.Type.LEFT,B, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.BASELINE, B, ConstraintAnchor.Type.BASELINE);
            //B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            //B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;

            A.BaselineDistance = 30;
            B.BaselineDistance = 20;
            C.BaselineDistance = 10;

            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;

            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            root.layout();
            root.layout();
            Console.WriteLine("root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(A.Top, 0);
            Assert.AreEqual(B.Top, 30-20);
            Assert.AreEqual(C.Top, 30-10);
        }
    }
}

using androidx.constraintlayout.core.widgets;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpConstraintLayoutTests.my
{
    [TestFixture]
    internal class MyWarpTest
    {
        [Test]
        public virtual void testNestedLayout()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 600);
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

            // --------
            // |A| | |
            // | |B| |
            // | | |C|
            // --------
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.BOTTOM, C, ConstraintAnchor.Type.TOP);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            //B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            //B.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;

            root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;

            root.OptimizationLevel = Optimizer.OPTIMIZATION_NONE;
            root.layout();
            Console.WriteLine("root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(root.Width, 300);
            Assert.AreEqual(root.Height, 60);

            
            root.Width = 0;
            root.Height = 0;
            root.layout();
            Console.WriteLine("root: " + root + " A: " + A + " B: " + B + " C: " + C);
            Assert.AreEqual(root.Width, 300);
            Assert.AreEqual(root.Height, 60);
        }
    }
}
